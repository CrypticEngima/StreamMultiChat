using StreamMultiChat.Blazor.Events;
using StreamMultiChat.Blazor.Extensions;
using StreamMultiChat.Blazor.Modals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;

namespace StreamMultiChat.Blazor.Services
{
	public class DisplayService
	{
		public IList<Channel> Channels { get; } = new List<Channel>();
		public Channel AllChannel { get; }
		private IList<Macro> Macros = new List<Macro>();
		private TwitchService _twitchService;

		public event EventHandler<string> OnMessageReceived;

		public DisplayService(TwitchService twitchService)
		{
			_twitchService = twitchService;
			Channels.Add(new Channel("All"));
			AllChannel = Channels.FirstOrDefault(c => c.Id == "All");
			_twitchService.OnMessageReceived += ReceiveMessageHandler;
		}

		public async Task AddChannel(string channelName)
		{
			var channel = new Channel(channelName);
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

		public async Task<IEnumerable<Macro>> GetAllMacros()
		{
			return await Task.FromResult(Macros);
		}

		public async Task<IEnumerable<Macro>> GetMacrosForMessage(Channel channel, string message)
		{
			return await Task.FromResult(channel.GetChannelMacros(Macros).GetMacrosForMessage(message));
		}

		public async Task AddMacro(Macro macro)
		{
			if (!Macros.Contains(macro))
			{
				Macros.Add(macro);
			}
			await Task.CompletedTask;
		}

		public async Task RemoveMacro(Macro macro)
		{
			Macros.Remove(macro);
			await Task.CompletedTask;
		}

		private void ReceiveMessageHandler(object sender, ChatMessageReceivedEventArgs e)
		{
			var msg = FormatMessageForDisplay(e.ChatMessage);
			MessageReceived(msg);
			
		}

		private void MessageReceived(string e)
		{
			var handler = OnMessageReceived;
			handler.Invoke(this, e);
		}

		private string FormatMessageForDisplay(ChatMessage inMessage)
		{
			return $"{DateTime.Now:t}   [{inMessage.Channel}] ({inMessage.Username}) : {inMessage.Message}";
		}

		public async Task JoinChannels()
		{
			foreach (var channel in AllChannel.ChannelStrings)
			{
				await _twitchService.JoinChannel(channel);
			}	   
		}

		public async Task<string> SendMessage(Channel channel,string message)
		{

		}
	}
}
