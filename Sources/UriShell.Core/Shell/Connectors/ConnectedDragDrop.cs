using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;

using UriShell.Shell.Resolution;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Implements a service for dragging objects connected to the UI. 
	/// </summary>
	public sealed class ConnectedDragDrop : IConnectedDragDrop, IUriPlacementConnector
	{
		/// <summary>
		/// The table for disconnecting objects from the UI.
		/// </summary>
		private readonly IUriDisconnectTable _uriDisconnectTable;

		/// <summary>
		/// The dictionary for storing data during dragging process.
		/// </summary>
		private readonly HybridDictionary _data = new HybridDictionary();

		/// <summary>
		/// The object being dragged.
		/// </summary>
		private object _connected;

		/// <summary>
		/// Initializes a new instance of the class <see cref="ConnectedDragDrop"/>.
		/// </summary>
		/// <param name="uriDisconnectTable">The table for disconnecting 
		/// objects from the UI.</param>
		public ConnectedDragDrop(IUriDisconnectTable uriDisconnectTable)
		{
			Contract.Requires<ArgumentNullException>(uriDisconnectTable != null);

			this._uriDisconnectTable = uriDisconnectTable;
		}

		/// <summary>
		/// Gets the value indicating that dragging is in process.
		/// </summary>
		public bool IsActive
		{
			get
			{
				return this._connected != null;
			}
		}

		/// <summary>
		/// Begins dragging of the given object, connected to the UI.
		/// </summary>
		/// <param name="connected">The object for dragging, connected to the UI.</param>
		public void Drag(object connected)
		{
			this._connected = connected;
			
			this._uriDisconnectTable[connected].Disconnect(connected);
			this._uriDisconnectTable[connected] = this;
		}

		/// <summary>
		/// Ends dragging of an object by attaching it to the given connector.
		/// </summary>
		/// <param name="target">The connector for attaching of a dragging object.</param>
		public void Drop(IUriPlacementConnector target)
		{
			var dragged = this._connected;

			target.Connect(dragged);
			this._uriDisconnectTable[dragged] = target;

			this._connected = null;
			this._data.Clear();
		}

		/// <summary>
		/// Checks if the given object is being dragged.
		/// </summary>
		/// <param name="resolved">The object to be checked.</param>
		/// <returns>true, if the object is being dragged; otherwise false.</returns>
		public bool IsDragging(object resolved)
		{
			return resolved == this._connected;
		}

		/// <summary>
		/// Saves the given data for dragging.
		/// </summary>
		/// <typeparam name="TFormat">The data type.</typeparam>
		/// <param name="key">The key for the data being saved.</param>
		/// <param name="data">The data being saved for dragging.</param>
		public void SetData<TFormat>(ConnectedDragDropKey<TFormat> key, TFormat data)
		{
			this._data[key] = data;
		}

		/// <summary>
		/// Gets the data saved for dragging.
		/// </summary>
		/// <typeparam name="TFormat">The data type.</typeparam>
		/// <param name="key">The key for data lookup.</param>
		/// <returns>The requested data or default value of the <typeparamref name="TFormat"/> type,
		/// if the data wasn't found.</returns>
		public TFormat GetData<TFormat>(ConnectedDragDropKey<TFormat> key)
		{
			if (this._data.Contains(key))
			{
				return (TFormat)this._data[key];
			}

			return default(TFormat);
		}

		/// <summary>
		/// Checks presence of the data saved for dragging.
		/// </summary>
		/// <typeparam name="TFormat">The data type.</typeparam>
		/// <param name="key">The key for data lookup.</param>
		/// <returns>true, if the data are present; otherwise false.</returns>
		public bool GetDataPresent<TFormat>(ConnectedDragDropKey<TFormat> key)
		{
			return this._data.Contains(key);
		}

		/// <summary>
		/// Connects the given object to the user interface. 
		/// </summary>
		/// <param name="resolved">The object to be connected to the UI.</param>
		void IUriPlacementConnector.Connect(object resolved)
		{
			// No one has to use ConnectorDragDrop as a connector. 
			throw new NotImplementedException();
		}

		/// <summary>
		/// Disconnects the given object from the user interface. 
		/// </summary>
		/// <param name="resolved">The object to be disconnected from the given UI.</param>
		void IUriPlacementConnector.Disconnect(object resolved)
		{
			this.OnDraggedClosed(EventArgs.Empty);

			// The call of Disconnect means the the shell closes the object
			// during dragging. In this case ConnectedDragDrop owns the object 
			// hence is responsible disposing stored data.
			foreach (var disposable in this._data.Values.OfType<IDisposable>())
			{
				disposable.Dispose();
			}

			this._connected = null;
			this._data.Clear();
		}

		/// <summary>
		/// Returns the flag that this connector is responsible for data refresh in connected objects.
		/// </summary>
		bool IUriPlacementConnector.IsResponsibleForRefresh
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Raises the event <see cref="DraggedClosed"/>.
		/// </summary>
		/// <param name="e">The object with event's arguments.</param>
		private void OnDraggedClosed(EventArgs e)
		{
			var draggedClosed = this.DraggedClosed;
			if (draggedClosed != null)
			{
				draggedClosed(this, e);
			}
		}

		/// <summary>
		/// Is raised when a dragging object is closed by the shell.
		/// </summary>
		public event EventHandler DraggedClosed;
	}
}