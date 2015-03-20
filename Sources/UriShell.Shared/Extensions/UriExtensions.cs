using System;
using System.Diagnostics.Contracts;

using UriShell.Shell;

namespace UriShell.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="Uri"/>.
	/// </summary>
	public static class UriExtensions
	{
		/// <summary>
		/// Gets the value indicating that the given <see cref="Uri" /> is used 
		/// for a view of the Phoenix application.
		/// </summary>
		/// <param name="uri">The <see cref="Uri"/> to be checked.</param>
		/// <returns>true, if the given <see cref="Uri" /> represents a URI 
		/// for a view of the Phoenix application; otherwise false. 
		/// </returns>
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
