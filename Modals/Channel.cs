using StreamMultiChat.Blazor.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Modals
{
	public class Channel
	{
		public string Id { get; }

		public IList<string> ChannelStrings { get; } = new List<string>();
		public List<string> Moderators { get; } = new List<string>();

		public Channel(string channelId)
		{
			Id = channelId;

		}

		public void AddChannelString(string channel)
		{
			ChannelStrings.Add(channel);
		}

		public void RemoveChannelString(string channel)
		{
			ChannelStrings.Remove(channel);
		}

		public void RemoveChannelString(Channel channel)
		{
			ChannelStrings.Remove(channel.Id);
		}

		public IEnumerable<Macro> GetMacros(IEnumerable<Macro> macros)
		{
			if (Id == "All") return macros;

			return macros.Where(m => m.Channel == this);
		}

		public void AddModerators(IList<string> mods)
		{
			Moderators.AddUnique(mods);
		}

		public bool IsModerator(string userName)
		{
			bool broadcaster = false;

			foreach (var channelName in ChannelStrings)
			{
				broadcaster = channelName.ToLower() == userName.ToLower();
			}


			return Moderators.Contains(userName) || broadcaster;
		}
	}
}
