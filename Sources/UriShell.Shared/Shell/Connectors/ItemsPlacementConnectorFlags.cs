using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Flags for the <see cref="IItemsPlacementConnector"/> setup.
	/// </summary>
	[Flags]
	public enum ItemsPlacementConnectorFlags
	{
		/// <summary>
		/// Default setup.
		/// </summary>
		Default = 0,

		/// <summary>
		/// The flag indicating that the property <see cref="IUriPlacementConnector.IsResponsibleForRefresh"/>
		/// should return true.
		/// </summary>
		IsResponsibleForRefresh = 1,

		/// <summary>
		/// The connector should make a connecting object active on connection.  
		/// </summary>
		ActivateOnConnect = 2,
	}
}
