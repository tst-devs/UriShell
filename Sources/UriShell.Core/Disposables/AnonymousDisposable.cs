using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UriShell.Disposables
{
	partial class Disposable
	{
		/// <summary>
		/// Represents an Action-based disposable.
		/// </summary>
		private sealed class AnonymousDisposable : IDisposable
		{
			/// <summary>
			/// The disposal action.
			/// </summary>
			private readonly Action _disposalAction;

			/// <summary>
			/// The value that indicates whether the object is disposed.
			/// </summary>
			private int _isDisposed;

			/// <summary>
			/// Constructs a new disposable with the given action used for disposal.
			/// </summary>
			/// <param name="disposalAction">The disposal action.</param>
			public AnonymousDisposable(Action disposalAction)
			{
				this._disposalAction = disposalAction;
			}

			/// <summary>
			/// Calls the disposal action.
			/// </summary>
			public void Dispose()
			{
				if (Interlocked.Exchange(ref this._isDisposed, 1) == 0)
				{
					this._disposalAction();
				}
			}
		}
	}
}
