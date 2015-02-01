using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Registration
{
	[ContractClassFor(typeof(IShellResolve))]
	internal abstract class IShellResolveContract : IShellResolve
	{
		public IShellResolveSetup<TResolved> Setup<TResolved>()
		{
			Contract.Ensures(Contract.Result<IShellResolveSetup<TResolved>>() != null);

			return default(IShellResolveSetup<TResolved>);
		}

		#region IShellOpen Members

		IDisposable IShellResolveOpen.Open()
		{
			throw new NotImplementedException();
		}

		IDisposable IShellResolveOpen.OpenOrThrow()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
