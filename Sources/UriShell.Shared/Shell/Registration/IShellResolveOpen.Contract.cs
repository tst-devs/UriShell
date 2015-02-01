using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Registration
{
	[ContractClassFor(typeof(IShellResolveOpen))]
	internal abstract class IShellResolveOpenContract : IShellResolveOpen
	{
		public IDisposable Open()
		{
			Contract.Ensures(Contract.Result<IDisposable>() != null);

			return default(IDisposable);
		}

		public IDisposable OpenOrThrow()
		{
			Contract.Ensures(Contract.Result<IDisposable>() != null);

			return default(IDisposable);
		}
	}
}
