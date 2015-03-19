using System.Collections.Generic;
using System.Diagnostics.Contracts;

using Autofac.Features.Indexed;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Provides custom components of the URI resolution process.
	/// </summary>
	[ContractClass(typeof(IUriResolutionCustomizationContract))]
	internal interface IUriResolutionCustomization
	{
		/// <summary>
		/// Gets the list of services capable of creating objects by an URI 
		/// and registered with <see cref="UriModuleItemResolverKey"/>.
		/// </summary>
		IIndex<UriModuleItemResolverKey, IUriModuleItemResolver> ModuleItemResolvers
		{
			get;
		}

		/// <summary>
		/// Gets the list of services capable of objects placement by an URI.
		/// </summary>
		IEnumerable<IUriPlacementResolver> PlacementResolvers
		{
			get;
		}
	}
}
