using System;
using System.Diagnostics.Contracts;

using UriShell.Shell;

namespace UriShell.Extensions
{
	/// <summary>
	/// Методы расширения для <see cref="Uri"/>.
	/// </summary>
	public static class UriExtensions
	{
		/// <summary>
		/// Возвращает значение, позволяющее определить, что заданный <see cref="Uri" /> представляет
		/// URI для открытия представлений в АРМ Феникс
		/// </summary>
		/// <param name="uri"><see cref="Uri"/> для проверки отношения к АРМ Феникс.</param>
		/// <returns>true, если заданный <see cref="Uri" /> представляет URI для открытия представлений
		/// в АРМ Феникс; иначе false.</returns>
		[Pure]
		public static bool IsPhoenix(this Uri uri)
		{
			Contract.Requires<ArgumentNullException>(uri != null);

			if (!uri.IsAbsoluteUri)
			{
				throw new InvalidOperationException(Properties.Resources.UriIsNotAbsolute);
			}

			return string.CompareOrdinal(uri.Scheme, PhoenixUriBuilder.UriSchemePhoenix) == 0;
		}
	}
}
