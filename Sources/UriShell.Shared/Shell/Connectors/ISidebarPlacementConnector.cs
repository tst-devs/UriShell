using System;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Connects objects to the user interface of the SidebarView.
	/// </summary>
	[Obsolete("Not sure if this interface has to stay in this assembly. It's a part of the Phoenix application, not a common feature.")]
	public interface ISidebarPlacementConnector : IItemsPlacementConnector, IDisposable
	{
		
	}
}
