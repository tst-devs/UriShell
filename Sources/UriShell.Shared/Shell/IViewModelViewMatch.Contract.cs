using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	[ContractClassFor(typeof(IViewModelViewMatch))]
	internal abstract class IViewModelViewMatchContract : IViewModelViewMatch
	{
		public object View
		{
			get
			{
				Contract.Ensures(Contract.Result<object>() != null);

				return default(object);
			}
		}

		public abstract bool SupportsModelChange
		{
			get;
		}

		public bool IsMatchToModel(object viewModel)
		{
			Contract.Requires<InvalidOperationException>(this.SupportsModelChange);

			Contract.Requires<ArgumentNullException>(viewModel != null);

			return default(bool);
		}

		public void ChangeModel(object viewModel)
		{
			Contract.Requires<InvalidOperationException>(this.SupportsModelChange);

			Contract.Requires<ArgumentNullException>(viewModel != null);
			Contract.Requires<ArgumentException>(this.IsMatchToModel(viewModel));
		}
	}
}
