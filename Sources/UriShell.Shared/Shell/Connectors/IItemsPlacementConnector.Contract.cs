using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UriShell.Shell.Connectors
{
	[ContractClassFor(typeof(IItemsPlacementConnector))]
	internal abstract class IItemsPlacementConnectorContract : IItemsPlacementConnector
	{
		public ICollectionView Views
		{
			get
			{
				Contract.Ensures(Contract.Result<ICollectionView>() != null);

				return default(ICollectionView);
			}
		}

		public IEnumerable<object> Connected
		{
			get
			{
				Contract.Ensures(Contract.Result<IEnumerable<object>>() != null);

				return default(IEnumerable<object>);
			}
		}

		public object Active
		{
			get
			{
				Contract.Ensures(Contract.Result<object>() == null || this.Connected.Contains(Contract.Result<object>()));

				return default(object);
			}
			set
			{
				Contract.Requires<ArgumentException>(value == null || this.Connected.Contains(value));
			}
		}

		public abstract bool IsPlacementEmpty
		{
			get;
		}

		public object ConnectedFromView(object view)
		{
			Contract.Requires<ArgumentOutOfRangeException>(this.Views.Cast<object>().Contains(view));

			Contract.Ensures(this.Connected.Contains(Contract.Result<object>()));

			return default(object);
		}

		public void MoveConnected(object connected, int newIndex)
		{
			Contract.Requires<ArgumentOutOfRangeException>(this.Connected.Contains(connected));
		}

		public abstract event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;

		public abstract event EventHandler<ActiveChangedEventArgs> ActiveChanged;

		#region IUriPlacementConnector Members

		void IUriPlacementConnector.Connect(object resolved)
		{
			throw new NotImplementedException();
		}

		void IUriPlacementConnector.Disconnect(object resolved)
		{
			throw new NotImplementedException();
		}

		bool IUriPlacementConnector.IsResponsibleForRefresh
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}