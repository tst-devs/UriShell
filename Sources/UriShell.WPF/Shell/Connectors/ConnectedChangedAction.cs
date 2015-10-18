using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Decribe the list of actions that invoke the event <see cref="IItemsPlacementConnector.ConnectedChanged"/>.
	/// </summary>
	public enum ConnectedChangedAction
	{
		/// <summary>
		/// An object was connected to thes UI.
		/// </summary>
		Connect,

		/// <summary>
		/// An object was disconnected from the UI.
		/// </summary>
		Disconnect,

		/// <summary>
		/// An object changed its index in the UI. 
		/// </summary>
		Move,
	}
}
