using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Events
{
	[ContractClassFor(typeof(IEventBroadcaster))]
	internal abstract class IEventBroadcasterContract : IEventBroadcaster
	{
		public IDisposable Subscribe(EventKey key, Action handler)
		{
			Contract.Requires<ArgumentNullException>(key != null);
			Contract.Requires<ArgumentNullException>(handler != null);

			Contract.Ensures(Contract.Result<IDisposable>() != null);
			return default(IDisposable);
		}

		public IDisposable Subscribe<TEventArgs>(EventKey<TEventArgs> key, Action<TEventArgs> handler)
		{
			Contract.Requires<ArgumentNullException>(key != null);
			Contract.Requires<ArgumentNullException>(handler != null);

			Contract.Ensures(Contract.Result<IDisposable>() != null);
			return default(IDisposable);
		}

		public void Send(EventKey key)
		{
			Contract.Requires<ArgumentNullException>(key != null);
		}

		public void Send<TEventArgs>(EventKey<TEventArgs> key, TEventArgs args)
		{
			Contract.Requires<ArgumentNullException>(key != null);
		}
	}
}
