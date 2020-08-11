using StreamMultiChat.Blazor.Events;
using StreamMultiChat.Blazor.Extensions;
using StreamMultiChat.Blazor.Modals;
using StreamMultiChat.Blazor.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Services
{
	public class DisplayService
	{
		public IList<Channel> Channels { get; } = new List<Channel>();
		public Channel AllChannel { get; }

		private IList<Macro> Macros = new List<Macro>();
		private TwitchService _twitchService;
		private TwitchSettings _twitchSettings;

		public event EventHandler<DisplayMessage> OnMessageReceived;

		public DisplayService(TwitchService twitchService,TwitchSettings settings)
		{
			_twitchService = twitchService;
			_twitchSettings = settings;
			Channels.Add(new Channel("All"));
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

		private async Task<IEnumerable<Macro>> GetMacrosForMessage(Channel channel, string message)
		{
			return await Task.FromResult(channel.GetMacros(Macros).GetMacrosForMessage(message));
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
			var channel = GetChannel(e.ChatMessage.Channel).Result;

			var msg = FormatMessageForDisplay(e.ChatMessage);
			MessageReceived(msg, channel.IsModerator(_twitchSettings.Username),channel,e.ChatMessage.Username);

		}

		private void MessageReceived(string e,bool modControl,Channel channel,string user)
		{
			var handler = OnMessageReceived;
			var displayMessage = new DisplayMessage(e,modControl,channel,user);
			handler.Invoke(this, displayMessage);
		}

		private string  FormatMessageForDisplay(ChatMessage inMessage)
		{
			

			return $"{DateTime.Now:t}   [{inMessage.Channel}] ({inMessage.Username}) : {inMessage.Message}";
		}

		public async Task JoinChannels()
		{
			
			foreach (var channel in AllChannel.ChannelStrings)
			{
				await _twitchService.JoinChannel(channel);
				_twitchService.GetModerators(channel);
			}
		}

		public async Task<IList<DisplayMessage>> SendMessage(Channel channel, string message)
		{
			IList<DisplayMessage> returnMessages = new List<DisplayMessage>();

			var macros = await GetMacrosForMessage(channel, message);
			var messages = GenerateMessages(channel, message, macros);

			foreach (var messageToSend in messages)
			{
				var sentMessage = _twitchService.SendMessage(messageToSend.Channel, messageToSend.message);
				var displayMessage = new DisplayMessage(FormatMessageForDisplay(sentMessage), channel.IsModerator(_twitchSettings.Username), channel,_twitchSettings.Username);
				returnMessages.Add(displayMessage);
			}

			return returnMessages;
		}

		private IEnumerable<(string Channel, string message)> GenerateMessages(Channel channel, string message, IEnumerable<Macro> macros)
		{
			if (macros.Count() == 0)
			{
				foreach (var channelString in channel.ChannelStrings)
				{
					yield return (channelString, message);
				}
			}
			else
			{
				foreach (var channelString in channel.ChannelStrings)
				{
					foreach (var macro in macros)
					{
						if (macro.Channel.Id != channelString) continue;
						yield return (channelString, macro.Response);
					}
				}
			}
		}

		private void ReceiveModHandler(object sender, ModReceivedEventArgs e)
		{
			var channel = GetChannel(e.Channel).Result;
			channel.AddModerators(e.Mods);
		}
	}
}
