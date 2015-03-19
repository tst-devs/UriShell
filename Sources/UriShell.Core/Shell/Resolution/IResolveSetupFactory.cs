using System;
using System.Diagnostics.Contracts;

using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// A factory of a service that implements setup of objects resolved via an URI. 
	/// </summary>
	[ContractClass(typeof(IResolveSetupFactoryContract))]
	internal interface IResolveSetupFactory
	{
		/// <summary>
		/// Creates the service that implements setup of objects resolved via an URI. 
		/// </summary>
		/// <typeparam name="TResolved">The object's type expected from URI's resolution.</typeparam>
		/// <param name="args">Arguments for initialization <see cref="ResolveSetup{TResolved}"/>.</param>
		/// <returns>The service that allows to setup and open an object resolved via an URI.</returns>
		IShellResolveSetup<TResolved> Create<TResolved>(ResolveSetupArgs args);
	}
}