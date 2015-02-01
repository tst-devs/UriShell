using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	[ContractClassFor(typeof(IUriModuleItemResolver))]
	internal abstract class IUriModuleItemResolverContract : IUriModuleItemResolver
	{
		public object Resolve(Uri uri, UriAttachmentSelector attachmentSelector)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentNullException>(attachmentSelector != null);

			Contract.Ensures(Contract.Result<object>() != null);

			return default(object);
		}
	}
}
