using Microsoft.Extensions.Logging;
using StreamMultiChat.Blazor.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using ChatMessage = StreamMultiChat.Blazor.Events.ChatMessage;
using WhisperMessage = StreamMultiChat.Blazor.Events.WhisperMessage;

namespace StreamMultiChat.Blazor.Services
{
	public class TwitchService
	{
		private readonly ILogger<TwitchService> _logger;
		private readonly AuthenticationService _authenticationService;
		private TwitchClient _client;
		private bool _correctlyConnected = false;

		public bool _connected => _client.IsConnected;

		public event EventHandler<ChatMessageReceivedEventArgs> OnMessageReceived;
		public event EventHandler<ModReceivedEventArgs> OnModReceived;
		public event EventHandler<WhisperReceivedEventArgs> OnWhisperReceived;

		public TwitchService(ILogger<TwitchService> logger, AuthenticationService authenticationService)
		{
			_logger = logger;
			_authenticationService = authenticationService;

			CreateClient();
			if (_client != null)
			{
				var creds = CreateCredentials();
				Initialize(creds);
				ConfigureHandlers();
			}
			
		}

		public async void  Connect()
		{
			if (_client != null)
			{
				await Task.Run(() => _client.Connect());
				await Task.Run(() => _logger.LogInformation("Connecting to Twitch"));
			}
		}

		public async Task Disconnect()
		{
			await Task.Run(() => _client.Disconnect());
		}

		public async Task JoinChannel(List<string> channelStrings)
		{
			foreach (var channelString in channelStrings)
			{
				await JoinChannel(channelString);
			}
		}

		public async Task JoinChannel(string channel)
		{
			if (_correctlyConnected)
			{
				var clientChannel = _client.JoinedChannels.Where(c => c.Channel.ToLower() == channel.ToLower()).FirstOrDefault();
				if (clientChannel is null)
				{
					await Task.Run(() => _client.JoinChannel(channel));
					await Task.Run(() => _logger.LogInformation($"Joined channel : {channel}"));
					await Task.Run(() => _logger.LogInformation($"Current joined channels Count: {_client.JoinedChannels.Count()}"));
				}
			}
			else
			{
				await Task.Delay(1000);
				await JoinChannel(channel);
			}
		}

		public async Task LeaveChannel(string channel)
		{
			await Task.Run(() => _client.LeaveChannel(channel));
			await Task.Run(() => _logger.LogInformation($"Left channel : {channel}"));
		}

		
		public async Task<ChatMessage> SendMessage(string channel, string message)
		{
			await Task.Run(() => _client.SendMessage(channel, message));

			await Task.Run(() => _logger.LogInformation($"Sending to {channel} the Message : {message}"));

			return new ChatMessage(message, false, false, false, false, false, 0, null, channel, 0, false, null, _authenticationService.TwitchUser.login);
		}

		private ConnectionCredentials CreateCredentials()
		{
			return new ConnectionCredentials(_authenticationService.TwitchUser.login,_authenticationService.Token);
		}

		private void CreateClient()
		{
			var clientOptions = new ClientOptions
			{
				MessagesAllowedInPeriod = 750,
				ThrottlingPeriod = TimeSpan.FromSeconds(30)
			};
			var customClient = new WebSocketClient(clientOptions);
			_client = new TwitchClient(customClient);

		}

		private void Initialize(ConnectionCredentials creds)
		{
			_client.Initialize(creds);
		}

		private void ConfigureHandlers()
		{
			if (_client != null)
			{
				_client.OnConnected += OnConnected;
				_client.OnMessageReceived += MessageReceived;
				_client.OnModeratorsReceived += ModeratorReceived;
				_client.OnWhisperReceived += WhisperReceived;
			}
		}

		
		private void OnConnected(object sender, OnConnectedArgs args)
		{
			_logger.LogInformation($"Connection To Twitch Started.");
			_correctlyConnected = true;
		}


		private void MessageReceived(ChatMessageReceivedEventArgs e)
		{
			OnMessageReceived.Invoke(this, e);
		}

		private void MessageReceived(object sender, OnMessageReceivedArgs e)
		{

			Task.Run (() => _logger.LogInformation($"Message Received from Chat user : {e.ChatMessage.Username} message: {e.ChatMessage.Message}"));

			var eventArgs = new ChatMessageReceivedEventArgs(new ChatMessage(
					e.ChatMessage.Message,
					e.ChatMessage.IsVip,
					e.ChatMessage.IsSubscriber,
					e.ChatMessage.IsModerator,
					e.ChatMessage.IsMe,
					e.ChatMessage.IsBroadcaster,
					e.ChatMessage.SubscribedMonthCount,
					e.ChatMessage.Id,
					e.ChatMessage.Channel,
					e.ChatMessage.Bits,
					e.ChatMessage.IsHighlighted,
					e.ChatMessage.UserId,
					e.ChatMessage.Username
					));

			MessageReceived(eventArgs);
		}

		private void WhisperReceived(WhisperReceivedEventArgs e)
		{
			OnWhisperReceived.Invoke(this, e);
		}

		private void WhisperReceived(object sender, OnWhisperReceivedArgs e)
		{
			Task.Run(() => _logger.LogInformation($"Message Received from Chat user : {e.WhisperMessage.Username} message: {e.WhisperMessage.Message}"));

			var eventArgs = new WhisperReceivedEventArgs(new WhisperMessage(
					e.WhisperMessage.Message,
					e.WhisperMessage.UserId,
					e.WhisperMessage.Username,
					e.WhisperMessage.DisplayName
					));

			WhisperReceived(eventArgs);
		}


		public async Task GetModerators(string channel)
		{
			await Task.Run(() => _client.GetChannelModerators(channel));
		}

		private void ModeratorReceived(object sender, OnModeratorsReceivedArgs e)
		{
			var mods = new ModReceivedEventArgs(e.Channel, e.Moderators);
			ModeratorReceived(mods);
		}

		private void ModeratorReceived(object sender, OnModeratorJoinedArgs e)
		{
			List<string> modsList = new List<string>();
			modsList.Add(e.Username);

			var mods = new ModReceivedEventArgs(e.Channel, modsList);
			ModeratorReceived(mods);
		}

		private void ModeratorReceived(ModReceivedEventArgs e)
		{
			OnModReceived.Invoke(this, e);
		}
	}
}