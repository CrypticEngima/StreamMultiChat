using StreamMultiChat.Blazor.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Extensions
{
	public static class MacroExtensions
	{
		public static IEnumerable<Macro> GetMacrosForMessage(this IEnumerable<Macro> macros, string message)
		{
			return macros.Where(m => m.Command == message);
		}
	}
}
