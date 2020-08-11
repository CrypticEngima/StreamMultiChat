using Microsoft.Extensions.Logging;
using StreamMultiChat.Blazor.Events;
using StreamMultiChat.Blazor.Modals;
using StreamMultiChat.Blazor.Settings;
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

namespace StreamMultiChat.Blazor.Services
{
	public class TwitchService
	{
		private readonly TwitchSettings _settings;
		private readonly ILogger<TwitchService> _logger;
		private TwitchClient _client;
		private bool _correctlyConnected = false;

		public bool _connected => _client.IsConnected;

		public event EventHandler<ChatMessageReceivedEventArgs> OnMessageReceived;
		public event EventHandler<ModReceivedEventArgs> OnModReceived;

		public TwitchService(TwitchSettings settings, ILogger<TwitchService> logger)
		{
			_settings = settings;
			_logger = logger;

			CreateClient();
			if (_client != null)
			{
				var creds = CreateCredentials();
				Initialize(creds);
				ConfigureHandlers();
			}
		}

		public void Connect()
		{
			if (_client != null)
			{
				_client.Connect();
				_logger.LogInformation("Connecting to Twitch");
			}
		}

		public void Disconnect()
		{
			_client.Disconnect();
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
					_client.JoinChannel(channel);
					_logger.LogInformation($"Joined channel : {channel}");
					_logger.LogInformation($"Current joined channels Count: {_client.JoinedChannels.Count()}");
				}
			}
			else
			{
				await Task.Delay(1000);
				await JoinChannel(channel);
			}
		}

		public Task LeaveChannel(string channel)
		{
			_client.LeaveChannel(channel);
			_logger.LogInformation($"Left channel : {channel}");
			return Task.CompletedTask;
		}

		
		public ChatMessage SendMessage(string channel, string message)
		{
			_client.SendMessage(channel, message);
			_logger.LogInformation($"Sending to {channel} the Message : {message}");
			return new ChatMessage(message, false, false, false, false, false, 0, null, channel, 0, false, null, _settings.Username);
		}

		private ConnectionCredentials CreateCredentials()
		{
			return new ConnectionCredentials(_settings.Username, _settings.Token);
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

		private void MessageReceived(object sender, OnMessageReceivedArgs args)
		{

			_logger.LogInformation($"Message Received from Chat user : {args.ChatMessage.Username} message: {args.ChatMessage.Message}");

			var eventArgs = new ChatMessageReceivedEventArgs(new ChatMessage(
					args.ChatMessage.Message,
					args.ChatMessage.IsVip,
					args.ChatMessage.IsSubscriber,
					args.ChatMessage.IsModerator,
					args.ChatMessage.IsMe,
					args.ChatMessage.IsBroadcaster,
					args.ChatMessage.SubscribedMonthCount,
					args.ChatMessage.Id,
					args.ChatMessage.Channel,
					args.ChatMessage.Bits,
					args.ChatMessage.IsHighlighted,
					args.ChatMessage.UserId,
					args.ChatMessage.Username
					));

			MessageReceived(eventArgs);
		}

		 public  void GetModerators(string channel)
		{
			_client.GetChannelModerators(channel);
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