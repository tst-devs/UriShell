using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Registration
{
	/// <summary>
	/// Allows to open an object resolved from a URI.
	/// </summary>
	[ContractClass(typeof(IShellResolveOpenContract))]
	public interface IShellResolveOpen
	{
		/// <summary>
		/// Opens an object resolved from a URI.
		/// </summary>
		/// <returns>Service for object's closing via <see cref="IDisposable.Dispose"/>.</returns>
		IDisposable Open();

		/// <summary>
		/// Opens an object resolved from a URI and allows the calling site to handle an exception 
		/// when it occurs.
		/// </summary>
		/// <returns>Service for object's closing via <see cref="IDisposable.Dispose"/>,
		/// if an object was opened successfully.</returns>
		IDisposable OpenOrThrow();
	}
}
