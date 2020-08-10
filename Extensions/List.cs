using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Extensions
{
	public static class ListExtensions
	{
		public static void AddUnique<T>(this IList<T> self, IEnumerable<T> items)
		{
			foreach (var item in items)
				if (!self.Contains(item))
					self.Add(item);
		}
	}
}
