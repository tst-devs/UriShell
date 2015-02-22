using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UriShell.Shell
{
	/// <summary>
	/// Extension methods for the <see cref="Shell"/>.
	/// </summary>
	public static class ShellExtensions
	{
		/// <summary>
		/// Closes objects from the given list.
		/// </summary>
		/// <param name="shell">Interface of the application shell.</param>
		/// <param name="resolvedList">The list of objects to be closed.</param>
		public static void CloseResolvedList(this IShell shell, IEnumerable<object> resolvedList)
		{
			Contract.Requires<ArgumentNullException>(shell != null);
			Contract.Requires<ArgumentNullException>(resolvedList != null);

			// Copy the list for preventing side-effects.
			Array.ForEach(resolvedList.ToArray(), shell.CloseResolved);
		}
	}
}
