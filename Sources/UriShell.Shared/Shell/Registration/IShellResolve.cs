using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace UriShell.Shell.Registration
{
	/// <summary>
	/// Allows to use an object resolved from a URI.
	/// </summary>
	[ContractClass(typeof(IShellResolveContract))]
	public interface IShellResolve : IShellResolveOpen
	{
		/// <summary>
		/// Allows to setup an object resolved from a URI 
		/// if its type is compatible with <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="TResolved">The object's type expected from a URI.</typeparam>
		/// <returns>The service for object's setup.</returns>
		IShellResolveSetup<TResolved> Setup<TResolved>();
	}
}
