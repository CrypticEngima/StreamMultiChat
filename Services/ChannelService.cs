using StreamMultiChat.Blazor.Events;
using StreamMultiChat.Blazor.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Services
{
	public class ChannelService
	{
		private readonly AuthenticationService _authenticationService;
		private readonly TwitchService _twitchService;
		private readonly MacroService _macroService;

		public IList<Channel> Channels { get; } = new List<Channel>();
		public Channel AllChannel { get; }

		public event EventHandler<DisplayMessage> OnMessageReceived;

		public ChannelService(TwitchService twitchService, AuthenticationService authenticationService, MacroService macroService)
		{
			_authenticationService = authenticationService;
			_twitchService = twitchService;
			_macroService = macroService;

			Channels.Add(new Channel("All", _twitchService, _macroService, _authenticationService));
			AllChannel = Channels.FirstOrDefault(c => c.Id == "All");
			_twitchService.OnMessageReceived += ReceiveMessageHandler;
			_twitchService.OnModReceived += ReceiveModHandler;
			_twitchService.Connect();
		}

		public async Task<Channel> GetChannel(string channelId)
		{
			return await Task.FromResult(Channels.FirstOrDefault(c => c.Id == channelId));
		}

		public async Task AddChannel(string channelName)
		{
			var channel = new Channel(channelName, _twitchService, _macroService, _authenticationService);
			channel.AddChannelString(channelName);

			Channels.Add(channel);

			AllChannel.AddChannelString(channelName);
			await Task.CompletedTask;
		}

		public async Task RemoveChannel(string ChannelName)
		{
			var channel = Channels.FirstOrDefault(c => c.Id == ChannelName);
			await RemoveChannel(channel);
		}

		public async Task RemoveChannel(Channel channel)
		{
			AllChannel.RemoveChannelString(channel);
			Channels.Remove(channel);
			await Task.CompletedTask;
		}

		public async Task JoinChannels()
		{
			foreach (var channel in AllChannel.ChannelStrings)
			{
				await _twitchService.JoinChannel(channel);
				_twitchService.GetModerators(channel);
			}
		}

		private void ReceiveModHandler(object sender, ModReceivedEventArgs e)
		{
			var channel = GetChannel(e.Channel).Result;
			channel.AddModerators(e.Mods);
		}



		private void ReceiveMessageHandler(object sender, ChatMessageReceivedEventArgs e)
		{
			var channel = GetChannel(e.ChatMessage.Channel).Result;


			MessageReceived(e.ChatMessage.ToString(), channel.IsModerator(), channel, e.ChatMessage.Username);

		}

		private void MessageReceived(string e, bool modControl, Channel channel, string user)
		{
			var handler = OnMessageReceived;
			var displayMessage = new DisplayMessage(e, modControl, channel, user);
			handler.Invoke(this, displayMessage);
		}
	}
}
