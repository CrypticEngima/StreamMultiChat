using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Modals
{
	public struct Macro
	{
		public string Command { get; set; }
		public string Response { get; set; }
		public bool IsEnabled { get; set; }

		public bool CanRun(string command)
		{
			return  IsEnabled && Command == command;
		}
	}
}
