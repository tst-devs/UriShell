using System;
using System.Diagnostics.Contracts;

using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	[ContractClassFor(typeof(IResolveSetupFactory))]
	internal abstract class IResolveSetupFactoryContract : IResolveSetupFactory
	{
		public IShellResolveSetup<TResolved> Create<TResolved>(ResolveSetupArgs args)
		{
			Contract.Ensures(Contract.Result<IShellResolveSetup<TResolved>>() != null);

			return default(IShellResolveSetup<TResolved>);
		}
	}
}