using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Arguments of the event <see cref="IItemsPlacementConnector.ConnectedChanged"/>.
	/// </summary>
	public class ConnectedChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the class <see cref="ConnectedChangedEventArgs"/>.
		/// </summary>
		/// <param name="action">The action invoked the event <see cref="IItemsPlacementConnector.ConnectedChanged"/>.</param>
		/// <param name="changed">The object the action was applied to.</param>
		public ConnectedChangedEventArgs(ConnectedChangedAction action, object changed)
		{
			this.Action = action;
			this.Changed = changed;
		}

		/// <summary>
		/// Describes the invariant of the class.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.Changed != null);
		}

		/// <summary>
		/// Gets the action invoked the event <see cref="IItemsPlacementConnector.ConnectedChanged"/>.
		/// </summary>
		public ConnectedChangedAction Action
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the object the action was applied to.
		/// </summary>
		public object Changed
		{
			get;
			private set;
		}
	}
}
