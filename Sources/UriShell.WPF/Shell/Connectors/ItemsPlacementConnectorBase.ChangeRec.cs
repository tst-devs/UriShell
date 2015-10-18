using System;
using System.Collections.Generic;
using System.Linq;

namespace UriShell.Shell.Connectors
{
	partial class ItemsPlacementConnectorBase
	{
		/// <summary>
		/// Records changes of the connector's state.
		/// </summary>
		protected sealed class ChangeRec : IDisposable
		{
			/// <summary>
			/// The connector being changed.
			/// </summary>
			private readonly ItemsPlacementConnectorBase _connector;

			/// <summary>
			/// The list of the connector's state changes.
			/// </summary>
			private List<ConnectedChangedEventArgs> _connectionChanges;

			/// <summary>
			/// Initializes a new instance of the class <see cref="ChangeRec"/>.
			/// </summary>
			/// <param name="connector">The connector being changed.</param>
			public ChangeRec(ItemsPlacementConnectorBase connector)
			{
				this._connector = connector;

				this.OldActive = connector.Active;
				this.NewActive = connector.Active;
			}

			/// <summary>
			/// Releases resources held by the objects.
			/// </summary>
			public void Dispose()
			{
				this._connector.EndChange(this);
			}

			/// <summary>
			/// Records the connector's state change.
			/// </summary>
			/// <param name="change">Arguments that describes a change.</param>
			private void AddConnectionChange(ConnectedChangedEventArgs change)
			{
				if (this._connectionChanges == null)
				{
					this._connectionChanges = new List<ConnectedChangedEventArgs>();
				}

				this._connectionChanges.Add(change);
			}

			/// <summary>
			/// Gets the object, whose view was active before connector's state changing. 
			/// </summary>
			public object OldActive
			{
				get;
				private set;
			}

			/// <summary>
			/// Gets or sets the object, whose view is active 
			/// after connector's state changing.
			/// </summary>
			public object NewActive
			{
				get;
				set;
			}

			/// <summary>
			/// Records connection of the object.
			/// </summary>
			/// <param name="connected">The object to connect.</param>
			public void Connected(object connected)
			{
				this.AddConnectionChange(
					new ConnectedChangedEventArgs(ConnectedChangedAction.Connect, connected));
			}

			/// <summary>
			/// Records disconnection of the object.
			/// </summary>
			/// <param name="connected">The object to disconnect.</param>
			public void Disconnected(object connected)
			{
				this.AddConnectionChange(
					new ConnectedChangedEventArgs(ConnectedChangedAction.Disconnect, connected));
			}

			/// <summary>
			/// Records the object's index change.
			/// </summary>
			/// <param name="connected">The object with the changed index.</param>
			public void Moved(object connected)
			{
				this.AddConnectionChange(
					new ConnectedChangedEventArgs(ConnectedChangedAction.Move, connected));
			}

			/// <summary>
			/// Gets the list of changes of the connector's state.
			/// </summary>
			public IEnumerable<ConnectedChangedEventArgs> ConnectedChanges
			{
				get
				{
					return this._connectionChanges ?? Enumerable.Empty<ConnectedChangedEventArgs>();
				}
			}
		}
	}
}
