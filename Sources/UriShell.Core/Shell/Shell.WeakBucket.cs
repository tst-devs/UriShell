using System;
using System.Collections.Generic;
using System.Linq;

namespace UriShell.Shell
{
	partial class Shell
	{
		/// <summary>
		/// Implements a list of objects of the given type 
		/// that holds them with weak references.
		/// </summary>
		private sealed class WeakBucket<T> where T : class
		{
			/// <summary>
			/// Holds weak references to added objects.
			/// </summary>
			private readonly List<WeakReference> _list = new List<WeakReference>();

			/// <summary>
			/// Adds the object to the list.
			/// </summary>
			/// <param name="object">The object to be added.</param>
			public void Add(T @object)
			{
				if (this.ExtractAlive().Contains(@object))
				{
					return;
				}

				this._list.Add(new WeakReference(@object));
			}

			/// <summary>
			/// Gets the list of alive objects.
			/// </summary>
			/// <returns>The list of alive objects.</returns>
			public IEnumerable<T> ExtractAlive()
			{
				var alive = new List<T>(this._list.Count);
				for (int i = this._list.Count - 1; i >= 0; i--)
				{
					var target = this._list[i].Target as T;
					if (target != null)
					{
						alive.Add(target);
					}
					else
					{
						this._list.RemoveAt(i);
					}
				}

				return alive;
			}
		}
	}
}
