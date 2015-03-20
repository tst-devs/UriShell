using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Registration
{
	/// <summary>
	/// Allows to setup an object of the given type resolved from a URI
	/// </summary>
	/// <typeparam name="TResolved">Object's type expected from URI.</typeparam>
	[ContractClass(typeof(IShellResolveSetupContract<>))]
	public interface IShellResolveSetup<TResolved> : IShellResolveOpen
	{
		/// <summary>
		/// Allows to assign an action invoked before object's opening.
		/// </summary>
		/// <param name="action">Action invoked with a resolved object before object's opening.</param>
		/// <returns>The service that allows to setup or open an object resolved from an URI.</returns>
		IShellResolveSetup<TResolved> OnReady(Action<TResolved> action);

		/// <summary>
		/// Allows to assign an action invoked when a resolved object is being closed. 
		/// </summary>
		/// <param name="action">The action invoked with a resolved object when the latter is being closed.</param>
		/// <returns>The service that allows to setup or open an object resolved from an URI.</returns>
		IShellResolveSetup<TResolved> OnFinished(Action<TResolved> action);
	}
}
