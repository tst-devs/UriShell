using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UriShell.Shell
{
	/// <summary>
	/// Interface of a service that connects a resolved object to the user interface.
	/// </summary>
	[ContractClass(typeof(IUriPlacementConnectorContract))]
	public interface IUriPlacementConnector
	{
		/// <summary>
		/// Connects the given object to the user interface. 
		/// </summary>
		/// <param name="resolved">The object to be connected to the UI.</param>
		void Connect(object resolved);

		/// <summary>
		/// Disconnects the given object from the user interface. 
		/// </summary>
		/// <param name="resolved">The object to be disconnected from the given UI.</param>
		void Disconnect(object resolved);

		/// <summary>
		/// Returns the flag that this connector is responsible for data refresh in connected objects.
		/// </summary>
		bool IsResponsibleForRefresh
		{
			get;
		}
	}
}
