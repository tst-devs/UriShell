using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	[ContractClassFor(typeof(IUriPlacementConnector))]
	internal abstract class IUriPlacementConnectorContract : IUriPlacementConnector
	{
		public void Connect(object resolved)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);
		}

		public void Disconnect(object resolved)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);
		}

		public abstract bool IsResponsibleForRefresh
		{
			get;
		}
	}
}
