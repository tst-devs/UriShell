using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Connects objects to the user interface as the list of their views.
	/// </summary>
    public sealed partial class ItemsPlacementConnector : ItemsPlacementConnectorBase
	{
		/// <summary>
		/// The service for a connected object's view lookup.
		/// </summary>
		private readonly IViewModelViewMatcher _viewModelViewMatcher;

		/// <summary>
		/// The service for dragging an object connected to the user interface.
		/// </summary>
		private readonly IConnectedDragDrop _connectedDragDrop;
		
		/// <summary>
		/// Initializes a new instance of the class <see cref="ItemsPlacementConnector"/>.
		/// </summary>
		/// <param name="viewModelViewMatcher">The service for a connected object's view lookup.</param>
		/// <param name="connectedDragDrop">The service for dragging an object connected to the user interface.</param>
		public ItemsPlacementConnector(
			IViewModelViewMatcher viewModelViewMatcher,
			IConnectedDragDrop connectedDragDrop)
			: 
			this(viewModelViewMatcher, connectedDragDrop, ItemsPlacementConnectorFlags.Default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the class <see cref="ItemsPlacementConnector"/>.
		/// </summary>
		/// <param name="viewModelViewMatcher">The service for a connected object's view lookup.</param>
		/// <param name="connectedDragDrop">The service for dragging an object connected to the user interface.</param>
		/// <param name="flags">Configuration flags of the created <see cref="ItemsPlacementConnector"/>.</param>
		public ItemsPlacementConnector(
			IViewModelViewMatcher viewModelViewMatcher,
			IConnectedDragDrop connectedDragDrop,
			ItemsPlacementConnectorFlags flags)
			: 
			base(flags)
		{
			Contract.Requires<ArgumentNullException>(viewModelViewMatcher != null);
			Contract.Requires<ArgumentNullException>(connectedDragDrop != null);

			this._viewModelViewMatcher = viewModelViewMatcher;
			this._connectedDragDrop = connectedDragDrop;
		}

		/// <summary>
		/// Connects the given object to the user interface. 
		/// </summary>
		/// <param name="resolved">The object to be connected to the UI.</param>
		public override void Connect(object resolved)
		{
			using (var changeRec = this.BeginChange())
			{
				// Determine a view of the connected object. 
				// We either find it via the matcher 
				// or receive it from dragging service.
				object view;
				if (this._connectedDragDrop.IsDragging(resolved))
				{
					view = this._connectedDragDrop.GetData(ConnectedDragDropKeys.UriConnectedView);
				}
				else
				{
					var viewMatch = this._viewModelViewMatcher.Match(resolved);
					view = viewMatch != null ? viewMatch.View : resolved;
				}				
				
				this.Connected.Add(resolved);
				this.Views.Add(view);
				changeRec.Connected(resolved);

				// If the proper flag set, activate the connected object.
				if (this.IsFlagSet(ItemsPlacementConnectorFlags.ActivateOnConnect))
				{
					changeRec.NewActive = resolved;
				}
			}
		}

		/// <summary>
		/// Disconnects the given object from the user interface. 
		/// </summary>
		/// <param name="resolved">The object to be disconnected from the given UI.</param>
		public override void Disconnect(object resolved)
		{
			using (var changeRec = this.BeginChange())
			{
				var index = this.Connected.IndexOf(resolved);

				// Pick the next active object 
				// after disconnecting the active object's disconnection.
				if (resolved == this.Active)
				{
					if (index == this.Connected.Count - 1)
					{
						changeRec.NewActive = index > 0 ? this.Connected[index - 1] : null;
					}
					else
					{
						changeRec.NewActive = this.Connected[index + 1];
					}
				}

				// If the view is disconnected for dragging then we store it in the dragging service.
				// Otherwise, we need to dispose the view.
				IDisposable disposableView = null;
				if (this._connectedDragDrop.IsDragging(resolved))
				{
					this._connectedDragDrop.SetData(
						ConnectedDragDropKeys.UriConnectedView, this.Views[index]);
				}
				else
				{
					disposableView = this.Views[index] as IDisposable;
				}

				this.Views.RemoveAt(index);
				this.Connected.RemoveAt(index);

				if (disposableView != null)
				{
					disposableView.Dispose();
				}

				changeRec.Disconnected(resolved);
			}
		}

		/// <summary>
		/// Changes the index of the connected object to the given value.
		/// </summary>
		/// <param name="connected">The connected object whose index is changed.</param>
		/// <param name="newIndex">The new index of the connected object in the <see cref="Connected"/> list.</param>
		public override void MoveConnected(object connected, int newIndex)
		{
			var oldIndex = this.Connected.IndexOf(connected);
			if (oldIndex == newIndex)
			{
				return;
			}

			using (var changeRec = this.BeginChange())
			{
				this.Connected.RemoveAt(oldIndex);
				this.Connected.Insert(newIndex, connected);
				this.Views.Move(oldIndex, newIndex);

				changeRec.Moved(connected);
			}
		}
	}
}