using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	[ContractClassFor(typeof(IConnectedDragDrop))]
	internal abstract class IConnectedDragDropContract : IConnectedDragDrop
	{
		public abstract bool IsActive
		{
			get;
		}

		public void Drag(object connected)
		{
			Contract.Requires<ArgumentNullException>(connected != null);
			Contract.Requires<InvalidOperationException>(!this.IsActive);
		}

		public void Drop(IUriPlacementConnector target)
		{
			Contract.Requires<ArgumentNullException>(target != null);
			Contract.Requires<InvalidOperationException>(this.IsActive);
		}

		public abstract bool IsDragging(object resolved);

		public void SetData<TFormat>(ConnectedDragDropKey<TFormat> key, TFormat data)
		{
			Contract.Requires<ArgumentNullException>(key != null);
			Contract.Requires<InvalidOperationException>(this.IsActive);
		}

		public TFormat GetData<TFormat>(ConnectedDragDropKey<TFormat> key)
		{
			Contract.Requires<ArgumentNullException>(key != null);
			Contract.Requires<InvalidOperationException>(this.IsActive);

			return default(TFormat);
		}

		public bool GetDataPresent<TFormat>(ConnectedDragDropKey<TFormat> key)
		{
			Contract.Requires<ArgumentNullException>(key != null);

			return default(bool);
		}

		public abstract event EventHandler DraggedClosed;
	}
}