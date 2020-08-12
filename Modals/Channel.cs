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

		public bool IsModerator()
		{
			bool broadcaster = false;

			foreach (var channelName in ChannelStrings)
			{
				broadcaster = channelName.ToLower() == _authenticationService.TwitchUser.login.ToLower();
			}

			return Moderators.Contains(_authenticationService.TwitchUser.login) || broadcaster;
		}

		public IEnumerable<DisplayMessage> SendMessage(string message)
		{
			foreach (var messageToSend in GenerateMessages(message))
			{
				var sentMessage = _twitchService.SendMessage(messageToSend.channel, messageToSend.message);
				var displayMessage = new DisplayMessage(sentMessage.ToString(), IsModerator(), this, sentMessage.Username);

				yield return displayMessage;
			}
		}

		private IEnumerable<(string channel, string message)> GenerateMessages(string message)
		{
			var macrosToRun = Id == "all" ? 
				_macroService.GetMacroByCommand(message) : 
				_macroService.GetMacrosByChannelCommand(this, message);
				
			if (macrosToRun.Count() == 0)
			{
				foreach (var channelString in ChannelStrings)
				{
					yield return (channelString, message);
				}
			}
			else
			{
				foreach (var channelString in ChannelStrings)
				{
					foreach (var macro in macrosToRun)
					{
						if (macro.Channel.ChannelStrings.Contains(channelString))
						yield return (channelString, macro.Response);
					}
				}
			}
		}
	}
}
