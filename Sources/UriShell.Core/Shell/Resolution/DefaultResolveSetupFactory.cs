using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// A factory of a service that implements setup of objects resolved via an URI. 
	/// </summary>
	public sealed class DefaultResolveSetupFactory : IResolveSetupFactory
	{
		/// <summary>
		/// Creates the service that implements setup of objects resolved via an URI. 
		/// </summary>
		/// <typeparam name="TResolved">The object's type expected from URI's resolution.</typeparam>
		/// <param name="args">Arguments for initialization <see cref="ResolveSetup{TResolved}"/>.</param>
		/// <returns>The service that allows to setup and open an object resolved via an URI.</returns>
		public IShellResolveSetup<TResolved> Create<TResolved>(ResolveSetupArgs args)
		{
			return new ResolveSetup<TResolved>(args);
		}
	}
}
