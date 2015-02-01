using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	[ContractClassFor(typeof(IViewModelViewMatcher))]
	internal abstract class IViewModelViewMatcherContract : IViewModelViewMatcher
	{
		public IViewModelViewMatch Match(object viewModel)
		{
			Contract.Requires<ArgumentNullException>(viewModel != null);

			return default(IViewModelViewMatch);
		}
	}
}
