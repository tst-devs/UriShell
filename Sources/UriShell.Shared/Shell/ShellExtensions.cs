using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UriShell.Shell
{
	/// <summary>
	/// Методы расширения для <see cref="Shell"/>.
	/// </summary>
	public static class ShellExtensions
	{
		/// <summary>
		/// Закрывает объекты из заданного списка.
		/// </summary>
		/// <param name="shell">Интерфейс оболочки АРМ.</param>
		/// <param name="resolvedList">Список объектов, которые необходимо закрыть.</param>
		public static void CloseResolvedList(this IShell shell, IEnumerable<object> resolvedList)
		{
			Contract.Requires<ArgumentNullException>(shell != null);
			Contract.Requires<ArgumentNullException>(resolvedList != null);

			// Копируем список во избежание side-эффектов.
			Array.ForEach(resolvedList.ToArray(), shell.CloseResolved);
		}
	}
}
