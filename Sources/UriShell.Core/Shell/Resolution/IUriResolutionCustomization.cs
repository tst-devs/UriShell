using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using UriShell.Collections;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Provides custom components of the URI resolution process.
	/// </summary>
	[ContractClass(typeof(IUriResolutionCustomizationContract))]
	public interface IUriResolutionCustomization
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
