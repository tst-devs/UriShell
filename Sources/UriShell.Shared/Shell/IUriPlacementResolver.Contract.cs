using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	[ContractClassFor(typeof(IUriPlacementResolver))]
	internal abstract class IUriPlacementResolverContract : IUriPlacementResolver
	{
		public IUriPlacementConnector Resolve(object resolved, Uri uri, UriAttachmentSelector attachmentSelector)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentNullException>(attachmentSelector != null);

			return default(IUriPlacementConnector);
		}
	}
}
