using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Modals
{
	public class Macro
	{
		public string Command { get; set; }
		public string Response { get; set; }
		public bool IsEnabled { get; set; } = false; 
		public string Channel { get; set; }

		public bool CanRun(string command)
		{
			return IsEnabled && Command == command;
		} 

		public bool MatchChannel(string channel)
		{
			return Channel == channel || string.IsNullOrEmpty(Channel);
		}
	}
}
