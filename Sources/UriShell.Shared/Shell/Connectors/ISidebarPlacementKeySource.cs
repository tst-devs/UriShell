using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// The interface of the object that provides keys used 
	/// for getting identity of objects being showed in the SidebarView.
	/// </summary>
	[Obsolete("Not sure if this interface has to stay in this assembly. It's a part of the Phoenix application, not a common feature.")]
	[ContractClass(typeof(ISidebarPlacementKeySourceContract))]
	public interface ISidebarPlacementKeySource
	{
		/// <summary>
		/// Gets the key of the given object connected to the SidebarView.
		/// </summary>
		/// <param name="connected">Объект, для которого запрашивается ключ.</param>
		/// <returns>The key of the given object or null, if the object doesn't have a key.</returns>
		object GetKey(object connected);
	}
}
