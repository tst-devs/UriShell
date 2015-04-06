using System;
using System.Diagnostics.Contracts;

using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Describes arguments for initializing objects implemening <see cref="IShellResolveSetup{TResolved}"/>.
	/// </summary>
	public sealed class ResolveSetupArgs
	{
		/// <summary>
		/// Initializes a new instance of the class <see cref="ResolveSetupArgs"/>.
		/// </summary>
		/// <param name="resolveOpen">The service that opens an object resolved via an URI.</param>
		/// <param name="playerSender">The action that passes a delegate for calling object's setup.</param>
		public ResolveSetupArgs(IShellResolveOpen resolveOpen, Action<ResolveSetupPlayer> playerSender)
		{
			this.ResolveOpen = resolveOpen;
			this.PlayerSender = playerSender;
		}

		/// <summary>
		/// Describes the invariant of the class.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.ResolveOpen != null);
			Contract.Invariant(this.PlayerSender != null);
		}

		/// <summary>
		/// The service that opens an object resolved via an URI.
		/// </summary>
		public IShellResolveOpen ResolveOpen
		{
			get;
			private set;
		}

		/// <summary>
		/// The action that passes a delegate for calling object's setup.
		/// </summary>
		public Action<ResolveSetupPlayer> PlayerSender
		{
			get;
			private set;
		}
	}
}
