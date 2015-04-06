using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Disposables
{
	/// <summary>
	/// Represents a group of Disposables that are disposed together.
	/// </summary>
	internal sealed class CompositeDisposable : ICollection<IDisposable>, IEnumerable<IDisposable>, IEnumerable, IDisposable
	{
		/// <summary>
		/// The value that indicates whether the object is disposed.
		/// </summary>
		private bool _isDisposed;

		/// <summary>
		/// The list of disposables stored in the CompositeDisposable.
		/// </summary>
		private List<IDisposable> _disposables;
		
		/// <summary>
		/// Gets the number of disposables contained in the CompositeDisposable.
		/// </summary>
		public int Count
		{
			get
			{
				return this._disposables.Count;
			}
		}
		/// <summary>
		/// Always returns false.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets a value that indicates whether the object is disposed.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return this._isDisposed;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeDisposable" /> class from a group of disposables.
		/// </summary>
		public CompositeDisposable()
		{
			this._disposables = new List<IDisposable>(16);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeDisposable" /> class with the specified number of disposables.
		/// </summary>
		/// <param name="capacity">The number of disposables that the new CompositeDisposable can initially store.</param>
		public CompositeDisposable(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this._disposables = new List<IDisposable>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeDisposable" /> class from a group of disposables.
		/// </summary>
		/// <param name="disposables">Disposables that will be disposed together.</param>
		public CompositeDisposable(params IDisposable[] disposables)
		{
			if (disposables == null)
			{
				throw new ArgumentNullException("disposables");
			}
			this._disposables = new List<IDisposable>(disposables);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeDisposable" /> class from a group of disposables.
		/// </summary>
		/// <param name="disposables">Disposables that will be disposed together.</param>
		public CompositeDisposable(IEnumerable<IDisposable> disposables)
		{
			if (disposables == null)
			{
				throw new ArgumentNullException("disposables");
			}
			this._disposables = new List<IDisposable>(disposables);
		}

		/// <summary>
		/// Adds a disposable to the CompositeDisposable or disposes the disposable if the CompositeDisposable is disposed.
		/// </summary>
		/// <param name="item">Disposable to add.</param>
		public void Add(IDisposable item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			bool flag = false;
			lock (this._disposables)
			{
				flag = this._isDisposed;
				if (!this._isDisposed)
				{
					this._disposables.Add(item);
				}
			}
			if (flag)
			{
				item.Dispose();
			}
		}

		/// <summary>
		/// Removes and disposes the first occurrence of a disposable from the CompositeDisposable.
		/// </summary>
		/// <param name="item">Disposable to remove.</param>
		public bool Remove(IDisposable item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			bool flag = false;
			lock (this._disposables)
			{
				if (!this._isDisposed)
				{
					flag = this._disposables.Remove(item);
				}
			}
			if (flag)
			{
				item.Dispose();
			}
			return flag;
		}

		/// <summary>
		/// Disposes all disposables in the group and removes them from the group.
		/// </summary>
		public void Dispose()
		{
			IDisposable[] array = null;
			lock (this._disposables)
			{
				if (!this._isDisposed)
				{
					this._isDisposed = true;
					array = this._disposables.ToArray();
					this._disposables.Clear();
				}
			}
			if (array != null)
			{
				IDisposable[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					IDisposable disposable = array2[i];
					disposable.Dispose();
				}
			}
		}

		/// <summary>
		/// Removes and disposes all disposables from the GroupDisposable, but does not dispose the CompositeDisposable.
		/// </summary>
		public void Clear()
		{
			IDisposable[] array = null;
			lock (this._disposables)
			{
				array = this._disposables.ToArray();
				this._disposables.Clear();
			}
			IDisposable[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				IDisposable disposable = array2[i];
				disposable.Dispose();
			}
		}

		/// <summary>
		/// Determines whether the CompositeDisposable contains a specific disposable.
		/// </summary>
		/// <param name="item">Disposable to search for.</param>
		/// <returns>true if the disposable was found; otherwise, false.</returns>
		public bool Contains(IDisposable item)
		{
			bool result;
			lock (this._disposables)
			{
				result = this._disposables.Contains(item);
			}
			return result;
		}

		/// <summary>
		/// Copies the disposables contained in the CompositeDisposable to an array, starting at a particular array index.
		/// </summary>
		/// <param name="array">Array to copy the contained disposables to.</param>
		/// <param name="arrayIndex">Target index at which to copy the first disposable of the group.</param>
		public void CopyTo(IDisposable[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex >= array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			lock (this._disposables)
			{
				Array.Copy(this._disposables.ToArray(), 0, array, arrayIndex, array.Length - arrayIndex);
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the CompositeDisposable.
		/// </summary>
		/// <returns>An enumerator to iterate over the disposables.</returns>
		public IEnumerator<IDisposable> GetEnumerator()
		{
			IEnumerator<IDisposable> enumerator;
			lock (this._disposables)
			{
				enumerator = ((IEnumerable<IDisposable>)this._disposables.ToArray()).GetEnumerator();
			}
			return enumerator;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the CompositeDisposable.
		/// </summary>
		/// <returns>An enumerator to iterate over the disposables.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
