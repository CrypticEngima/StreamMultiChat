using StreamMultiChat.Blazor.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Events
{
	public class ModReceivedEventArgs
	{
		public string Channel { get; }
		public IList<string> Mods { get; }

		public ModReceivedEventArgs(string channel, IList<string> mods)
		{
			Channel = channel;
			Mods = mods;
		}
	}
}
