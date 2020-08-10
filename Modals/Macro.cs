using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Modals
{
	public class Macro
	{
		public string Command { get; set; }
		public string Response { get; set; }
		public bool IsEnabled { get; set; } 
		public Channel Channel { get; set; }
	}
}
