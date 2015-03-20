using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// A service for dragging of objects connected to the UI. 
	/// </summary>
	[ContractClass(typeof(IConnectedDragDropContract))]
	public interface IConnectedDragDrop
	{
		/// <summary>
		/// Gets the value indicating that dragging is in process.
		/// </summary>
		[Pure]
		bool IsActive
		{
			get;
		}

		/// <summary>
		/// Begins dragging of the given object, connected to the UI.
		/// </summary>
		/// <param name="connected">The object for dragging, connected to the UI.</param>
		void Drag(object connected);

		/// <summary>
		/// Ends dragging of an object by attaching it to the given connector.
		/// </summary>
		/// <param name="target">The connector for attaching of a dragging object.</param>
		void Drop(IUriPlacementConnector target);

		/// <summary>
		/// Checks if the given object is being dragged.
		/// </summary>
		/// <param name="resolved">The object to be checked.</param>
		/// <returns>true, if the object is being dragged; otherwise false.</returns>
		[Pure]
		bool IsDragging(object resolved);

		/// <summary>
		/// Saves the given data for dragging.
		/// </summary>
		/// <typeparam name="TFormat">The data type.</typeparam>
		/// <param name="key">The key for the data being saved.</param>
		/// <param name="data">The data being saved for dragging.</param>
		void SetData<TFormat>(ConnectedDragDropKey<TFormat> key, TFormat data);

		/// <summary>
		/// Gets the data saved for dragging.
		/// </summary>
		/// <typeparam name="TFormat">The data type.</typeparam>
		/// <param name="key">The key for data lookup.</param>
		/// <returns>The requested data or default value of the <typeparamref name="TFormat"/> type,
		/// if the data wasn't found.</returns>
		[Pure]
		TFormat GetData<TFormat>(ConnectedDragDropKey<TFormat> key);

		/// <summary>
		/// Checks presence of the data saved for dragging.
		/// </summary>
		/// <typeparam name="TFormat">The data type.</typeparam>
		/// <param name="key">The key for data lookup.</param>
		/// <returns>true, if the data are present; otherwise false.</returns>
		[Pure]
		bool GetDataPresent<TFormat>(ConnectedDragDropKey<TFormat> key);

		/// <summary>
		/// Is raised when a dragging object is closed by the shell.
		/// </summary>
		event EventHandler DraggedClosed;
	}
}