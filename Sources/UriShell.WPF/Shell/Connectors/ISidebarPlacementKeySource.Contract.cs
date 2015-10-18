using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	[ContractClassFor(typeof(ISidebarPlacementKeySource))]
	internal abstract class ISidebarPlacementKeySourceContract : ISidebarPlacementKeySource
	{
		public object GetKey(object connected)
		{
			Contract.Requires<ArgumentNullException>(connected != null);

			return default(object);
		}
	}
}
