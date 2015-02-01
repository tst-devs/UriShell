using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Resolution
{
	[ContractClassFor(typeof(IUriDisconnectTable))]
	internal abstract class IUriDisconnectTableContract : IUriDisconnectTable
	{
		public IUriPlacementConnector this[object resolved]
		{
			get
			{
				Contract.Requires<ArgumentNullException>(resolved != null);
				Contract.Ensures(Contract.Result<IUriPlacementConnector>() != null);

				return default(IUriPlacementConnector);
			}
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);
				Contract.Requires<ArgumentNullException>(resolved != null);
			}
		}

		public void Remove(object resolved)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);
		}
	}
}
