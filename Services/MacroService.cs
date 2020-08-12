using StreamMultiChat.Blazor.Modals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Services
{
	public class MacroService
	{

		private IList<Macro> Macros = new List<Macro>();


		public IEnumerable<Macro> GetAllMacros()
		{
			return Macros;
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
