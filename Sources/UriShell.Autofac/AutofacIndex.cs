using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autofac.Features.Indexed;
using System.Diagnostics.Contracts;

namespace UriShell.Autofac
{
	/// <summary>
	/// Implements access by a key to a value around an Autofac IIndex&lt;TKey, TValue&gt; object.
	/// </summary>
	/// <typeparam name="TKey">The type of a key in the collection.</typeparam>
	/// <typeparam name="TValue">The type of a value in the collection.</typeparam>
	internal sealed class AutofacIndexWrapper<TKey, TValue> : UriShell.Collections.IIndex<TKey, TValue>
	{
		/// <summary>
		/// The underlying Autofac IIndex&lt;TKey, TValue&gt; object.
		/// </summary>
		private readonly IIndex<TKey, TValue> _source;

		/// <summary>
		/// Initializes a new instance of the class <see cref="AutofacIndexWrapper"/>.
		/// </summary>
		/// <param name="source">The underlying Autofac IIndex&lt;TKey, TValue&gt; object.</param>
		public AutofacIndexWrapper(IIndex<TKey, TValue> source)
		{
			Contract.Requires<ArgumentNullException>(source != null);

			this._source = source;
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="value">When this method returns, contains the value associated
		/// with the specified key, if the key is found.</param>
		/// <returns>true - if the value was found; false otherwise.</returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			value = default(TValue);
			return this._source.TryGetValue(key, out value);
		}
	}
}
