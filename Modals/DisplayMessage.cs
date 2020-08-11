using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Modals
{
	public class DisplayMessage
	{
		public string Message { get; set; }
		public bool ModControls { get; set; }
		public Channel Channel { get; set; }
		public string User { get; set; }

		public DisplayMessage(string message, bool modControls, Channel channel, string user)
		{
			Message = message;
			ModControls = modControls;
			Channel = channel;
			User = user;
		}
	}
}
