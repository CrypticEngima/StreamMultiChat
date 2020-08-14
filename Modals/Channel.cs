using StreamMultiChat.Blazor.Extensions;
using StreamMultiChat.Blazor.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Modals
{
	public class Channel
	{
		private readonly TwitchService _twitchService;
		private readonly AuthenticationService _authenticationService;
		private readonly MacroService _macroService;

		public string Id { get; }

		public IList<string> ChannelStrings { get; } = new List<string>();
		public List<string> Moderators { get; } = new List<string>();

		public Channel(string channelId, TwitchService twitchService, MacroService macroService, AuthenticationService authenticationService)
		{
			Id = channelId.ToLower();
			_twitchService = twitchService;
			_macroService = macroService;
			_authenticationService = authenticationService;
		}

		public void AddChannelString(string channel)
		{
			ChannelStrings.Add(channel.ToLower());
		}

		public void RemoveChannelString(Channel channel)
		{
			ChannelStrings.Remove(channel.Id);
		}

		public void AddModerators(IList<string> mods)
		{
			Moderators.AddUnique(mods);
		}

		public async Task JoinChannel()
		{
			await _twitchService.JoinChannel(ChannelStrings.FirstOrDefault());
		}

		public async Task LeaveChannel()
		{
			await _twitchService.LeaveChannel(ChannelStrings.FirstOrDefault());
		}

		public bool IsModerator()
		{
			bool broadcaster = false;

			foreach (var channelName in ChannelStrings)
			{
				broadcaster = channelName.ToLower() == _authenticationService.TwitchUser.login.ToLower();
			}

			return Moderators.Contains(_authenticationService.TwitchUser.login) || broadcaster;
		}

		public async Task<List<DisplayMessage>> SendMessage(string message)
		{
			var messagesReturn = new List<DisplayMessage>();

			System.Console.WriteLine("entered send message on channel");
			foreach (var messageToSend in GenerateMessages(message))
			{
				var sentMessage = await _twitchService.SendMessage(messageToSend.channel, messageToSend.message);
				var displayMessage = new DisplayMessage(sentMessage.ToString(), IsModerator(), this, sentMessage.Username);

				messagesReturn.Add(displayMessage);
			}

			return messagesReturn;
		}

		private IEnumerable<(string channel, string message)> GenerateMessages(string message)
		{
			List<(string channel, string message)> macrosReturn = new List<(string channel, string message)>();

			var macrosToRun = Id == "all" ? 
				_macroService.GetMacroByCommand(message) : 
				_macroService.GetMacrosByChannelCommand(this, message);
				
			if (macrosToRun.Count() == 0)
			{
				foreach (var channelString in ChannelStrings)
				{
					macrosReturn.Add((channelString, message));
				}
			}
			else
			{
				foreach (var channelString in ChannelStrings)
				{
					foreach (var macro in macrosToRun)
					{
						if (macro.Channel.ChannelStrings.Contains(channelString))
							macrosReturn.Add((channelString, macro.Response));
					}
				}
			}

			return macrosReturn;
		}

	}
}
