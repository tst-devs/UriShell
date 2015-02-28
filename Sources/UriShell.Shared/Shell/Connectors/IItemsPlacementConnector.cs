using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Connects objects to the a user interface as a list of its views.
	/// </summary>
	[ContractClass(typeof(IItemsPlacementConnectorContract))]
	public interface IItemsPlacementConnector : IUriPlacementConnector
	{
		/// <summary>
		/// Gets the <see cref="ICollectionView"/> of the view collection of connected objects.
		/// </summary>
		ICollectionView Views
		{
			get;
		}

		/// <summary>
		/// Gets the list of connected objects.
		/// </summary>
		IEnumerable<object> Connected
		{
			get;
		}
		
		/// <summary>
		/// Gets or sets the object whose view is active.
		/// </summary>
		object Active
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the value indicating that there is no connected objects.
		/// </summary>
		bool IsPlacementEmpty
		{
			get;
		}

		/// <summary>
		/// Gets the connected object for the given view.
		/// Возвращает присоединенный объект, представленный заданным.
		/// </summary>
		/// <param name="view">The view of the object being looked for.</param>
		/// <returns>The object for the given view.</returns>
		object ConnectedFromView(object view);

		/// <summary>
		/// Changes the index of the connected object to the given value.
		/// </summary>
		/// <param name="connected">The connected object whose index is changed.</param>
		/// <param name="newIndex">The new index of the connected object in the <see cref="Connected"/> list.</param>
		void MoveConnected(object connected, int newIndex);

		/// <summary>
		/// Is raised when an object is connected or disconnected.
		/// </summary>
		event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;

		/// <summary>
		/// Is raised when the active view is changed.
		/// </summary>
		event EventHandler<ActiveChangedEventArgs> ActiveChanged;
	}
}