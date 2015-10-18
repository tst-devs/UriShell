using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Implements basic features of <see cref="IItemsPlacementConnector"/>.
	/// </summary>
    public abstract partial class ItemsPlacementConnectorBase : IItemsPlacementConnector
	{
		/// <summary>
		/// Setup flags for the <see cref="ItemsPlacementConnector"/>.
		/// </summary>
		private readonly ItemsPlacementConnectorFlags _flags;

		/// <summary>
		/// The list of connected objects.
		/// </summary>
		private readonly List<object> _connected = new List<object>();

		/// <summary>
		/// The collection of views of connected objects.
		/// </summary>
		private readonly ObservableCollection<object> _views = new ObservableCollection<object>();

		/// <summary>
		/// <see cref="ICollectionView"/> of the collection of views of connected objects
		/// </summary>
		private readonly ICollectionView _viewsCollectionView;

		/// <summary>
		/// Shows that changing of the connector started by <see cref="BeginChange"/>
		/// is in progress.
		/// </summary>
		private bool _isChanging;
				
		/// <summary>
		/// The index of the active object.
		/// </summary>
		private int _activeIndex = -1;

		/// <summary>
		/// Initializes a new instance of the class <see cref="ItemsPlacementConnectorBase"/>.
		/// </summary>
		/// <param name="flags">Configuration flags of the created <see cref="ItemsPlacementConnector"/>.</param>
		public ItemsPlacementConnectorBase(ItemsPlacementConnectorFlags flags)
		{
			this._flags = flags;

			this._viewsCollectionView = new ListCollectionView(this._views);
			this._viewsCollectionView.CurrentChanged += this.ViewsCollectionView_CurrentChanged;
		}

		/// <summary>
		/// Checks whether the given configuration flag is set.
		/// </summary>
		/// <param name="flag">The configuration flag to check.</param>
		/// <returns>true, if the given flag is set; false otherwise.</returns>
		protected bool IsFlagSet(ItemsPlacementConnectorFlags flag)
		{
			return (this._flags & flag) == flag;
		}

		/// <summary>
		/// Gets the list of connected objects.
		/// </summary>
		protected IList<object> Connected
		{
			get
			{
				return this._connected;
			}
		}

		/// <summary>
		/// Gets the list of views of connected objects.
		/// </summary>
		protected ObservableCollection<object> Views
		{
			get
			{
				return this._views;
			}
		}

		#region Explicit IItemsPlacementConnector Members

		/// <summary>
		/// Gets the list of connected objects.
		/// </summary>
		IEnumerable<object> IItemsPlacementConnector.Connected
		{
			get
			{
				return this._connected;
			}
		}

		/// <summary>
		/// Gets the <see cref="ICollectionView"/> of the view collection of connected objects.
		/// </summary>
		ICollectionView IItemsPlacementConnector.Views
		{
			get
			{
				return this._viewsCollectionView;
			}
		}

		#endregion

		/// <summary>
		/// Connects the given object to the user interface. 
		/// </summary>
		/// <param name="resolved">The object to be connected to the UI.</param>
		public abstract void Connect(object resolved);

		/// <summary>
		/// Disconnects the given object from the user interface. 
		/// </summary>
		/// <param name="resolved">The object to be disconnected from the given UI.</param>
		public abstract void Disconnect(object resolved);

		/// <summary>
		/// Gets the connected object for the given view.
		/// </summary>
		/// <param name="view">The view of the object being looked for.</param>
		/// <returns>The object for the given view.</returns>
		public object ConnectedFromView(object view)
		{
			var index = this._views.IndexOf(view);
			return this._connected[index];
		}

		/// <summary>
		/// Changes the index of the connected object to the given value.
		/// </summary>
		/// <param name="connected">The connected object whose index is changed.</param>
		/// <param name="newIndex">The new index of the connected object in the <see cref="Connected"/> list.</param>
		public abstract void MoveConnected(object connected, int newIndex);

		/// <summary>
		/// Starts changing of connector's state.
		/// </summary>
		/// <returns>The object that allows to record changes.</returns>
		protected ChangeRec BeginChange()
		{
			if (this._isChanging)
			{
				throw new InvalidOperationException(string.Format(
					Properties.Resources.UnableToBeginChangePlacementConnector,
					this.GetType().Name));
			}

			this._isChanging = true;

			return new ChangeRec(this);
		}

		/// <summary>
		/// Finishes changing of connector's state.
		/// </summary>
		/// <param name="changeRec">The object that allows to record changes.</param>
		private void EndChange(ChangeRec changeRec)
		{
			try
			{
				this._activeIndex = this._connected.IndexOf(changeRec.NewActive);
				
				// When objects are added and moved, 
				// raise ConnectedChanged before ActiveChanged.
				foreach (var change in changeRec.ConnectedChanges)
				{
					if (change.Action == ConnectedChangedAction.Connect
						|| change.Action == ConnectedChangedAction.Move)
					{
						this.OnConnectedChanged(change);
					}
				}

				if (changeRec.OldActive != changeRec.NewActive)
				{
					this.OnActiveChanged(new ActiveChangedEventArgs(changeRec.OldActive, changeRec.NewActive));
				}

				// When objects are disconnected, 
				// raise ConnectedChanged after ActiveChanged.
				foreach (var change in changeRec.ConnectedChanges)
				{
					if (change.Action == ConnectedChangedAction.Disconnect)
					{
						this.OnConnectedChanged(change);
					}
				}

				// Synchronize the selected element in the list of views.
				this._viewsCollectionView.MoveCurrentToPosition(this._activeIndex);
			}
			finally
			{
				this._isChanging = false;
			}
		}

		/// <summary>
		/// Handles change of the active element in the <see cref="Views"/>.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The object with event arguments.</param>
		private void ViewsCollectionView_CurrentChanged(object sender, EventArgs e)
		{
			// Ignore notifications when connector's state changing is in progress.
			if (this._isChanging)
			{
				return;
			}

			if (this._viewsCollectionView.CurrentItem == null)
			{
				this.Active = null;
			}
			else
			{
				this.Active = this.ConnectedFromView(this._viewsCollectionView.CurrentItem);
			}
		}

		/// <summary>
		/// Raises the event <see cref="ConnectedChanged"/>.
		/// </summary>
		/// <param name="e">The object with event's arguments.</param>
		private void OnConnectedChanged(ConnectedChangedEventArgs e)
		{
			var connectedChanged = this.ConnectedChanged;
			if (connectedChanged != null)
			{
				connectedChanged(this, e);
			}
		}

		/// <summary>
		/// Raises the event <see cref="ActiveChanged"/>.
		/// </summary>
		/// <param name="e">The object with event's arguments.</param>
		private void OnActiveChanged(ActiveChangedEventArgs e)
		{
			var activeChanged = this.ActiveChanged;
			if (activeChanged != null)
			{
				activeChanged(this, e);
			}
		}

		/// <summary>
		/// Returns the flag that this connector is responsible for data refresh in connected objects.
		/// </summary>
		public bool IsResponsibleForRefresh
		{
			get
			{
				return this.IsFlagSet(ItemsPlacementConnectorFlags.IsResponsibleForRefresh);
			}
		}

		/// <summary>
		/// Gets or sets the object whose view is active.
		/// </summary>
		public object Active
		{
			get
			{
				if (this._activeIndex == -1)
				{
					return null;
				}

				return this._connected[this._activeIndex];
			}
			set
			{
				if (value == this.Active)
				{
					return;
				}

				using (var changeRec = this.BeginChange())
				{
					changeRec.NewActive = value;
				}
			}
		}

		/// <summary>
		/// Gets the value indicating that there is no connected objects.
		/// </summary>
		public bool IsPlacementEmpty
		{
			get
			{
				return this._connected.Count == 0;
			}
		}

		/// <summary>
		/// Is raised when an object is connected or disconnected.
		/// </summary>
		public event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;

		/// <summary>
		/// Is raised when the active view is changed.
		/// </summary>
		public event EventHandler<ActiveChangedEventArgs> ActiveChanged;
	}
}