using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Modals
{
	public class Channel
	{
		public string Name { get; }
		public List<Macro> Macros { get; } = new List<Macro>();

		public Channel(string channelName)
		{
			Name = channelName;
		}

		public	IEnumerable<(string channel,string message)> GetMessageToSend(string message)
		{
			foreach (var macro in GetMacrosToRun(message))
			{
				yield return (Name, macro.Response);
			}
		}

		private IEnumerable<Macro> GetMacrosToRun(string message)
		{
			return Macros.Where(m => m.IsEnabled && m.Command == message);
		}

		public void AddMacro(Macro macro)
		{
			if ((macro.Channel == Name || macro.Channel == null) && (!Macros.Contains(macro)))
			{
				Macros.Add(macro);
			}
		}

		public void RemoveMacro(Macro macro)
		{
			Macros.Remove(macro);
		}
	}
}
