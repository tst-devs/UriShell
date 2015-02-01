using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Registration
{
	[ContractClassFor(typeof(IShellResolveSetup<>))]
	internal abstract class IShellResolveSetupContract<TResolved> : IShellResolveSetup<TResolved>
	{
		public IShellResolveSetup<TResolved> OnReady(Action<TResolved> action)
		{
			Contract.Requires<ArgumentNullException>(action != null);
			Contract.Ensures(Contract.Result<IShellResolveSetup<TResolved>>() != null);

			return default(IShellResolveSetup<TResolved>);
		}
		
		public IShellResolveSetup<TResolved> OnFinished(Action<TResolved> action)
		{
			Contract.Requires<ArgumentNullException>(action != null);
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
