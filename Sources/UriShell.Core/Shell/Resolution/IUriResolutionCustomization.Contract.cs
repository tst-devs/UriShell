using System.Collections.Generic;
using System.Diagnostics.Contracts;

using UriShell.Collections;

namespace UriShell.Shell.Resolution
{
	using UriModuleItemResolverIndex = IIndex<UriModuleItemResolverKey, IUriModuleItemResolver>;

	[ContractClassFor(typeof(IUriResolutionCustomization))]
	internal abstract class IUriResolutionCustomizationContract : IUriResolutionCustomization
	{
		public UriModuleItemResolverIndex ModuleItemResolvers
		{
			get
			{
				Contract.Ensures(Contract.Result<UriModuleItemResolverIndex>() != null);

				return default(UriModuleItemResolverIndex);
			}
		}

		public IEnumerable<IUriPlacementResolver> PlacementResolvers
		{
			get
			{
				Contract.Ensures(Contract.Result<IEnumerable<IUriPlacementResolver>>() != null);

				return default(IEnumerable<IUriPlacementResolver>);
			}
		}
	}
}
