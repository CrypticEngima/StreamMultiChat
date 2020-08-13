using StreamMultiChat.Blazor.Modals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Services
{
	public class MacroService
	{

		private IList<Macro> Macros;

		public MacroService()
		{
			Macros = new List<Macro>();
		}


		public IEnumerable<Macro> GetAllMacros()
		{
			return Macros;
		}

		public IEnumerable<Macro> GetMacroByCommand(string command)
		{
			return Macros.Where(m => m.Command == command && m.IsEnabled);
		}

		public IEnumerable<Macro> GetMacrosByChannelCommand(Channel channel,string command)
		{
			return Macros.Where(m => m.Command == command && m.Channel == channel && m.IsEnabled);
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

	}

}
