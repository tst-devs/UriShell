using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Arguments of the event <see cref="IItemsPlacementConnector.ActiveChanged"/>.
	/// </summary>
	public class ActiveChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the class <see cref="ActiveChangedEventArgs"/>.
		/// </summary>
		/// <param name="oldValue">The object whose view was active.</param>
		/// <param name="newValue">The object whose view became active.</param>
		public ActiveChangedEventArgs(object oldValue, object newValue)
		{
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}

		/// <summary>
		/// Describes the invariant of the class.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.OldValue != null || this.NewValue != null);
		}

		/// <summary>
		/// Gets the object whose view was active.
		/// </summary>
		public object OldValue
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the object whose view became active.
		/// </summary>
		public object NewValue
		{
			get;
			private set;
		}
	}
}
