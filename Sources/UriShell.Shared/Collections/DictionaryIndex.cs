using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UriShell.Collections
{
	/// <summary>
	/// Implements  IIndex &lt;TKey, TValue&gt; around a dictionary.
	/// </summary>
	/// <typeparam name="TKey">The type of a key in the collection.</typeparam>
	/// <typeparam name="TValue">The type of a value in the collection.</typeparam>
	public sealed class DictionaryIndex<TKey, TValue> : IIndex<TKey, TValue>
	{
		/// <summary>
		/// The underlying dictionary.
		/// </summary>
		private readonly Dictionary<TKey, TValue> _source;

		/// <summary>
		/// Initializes a new instance of the class <see cref="DictionaryIndex"/>.
		/// </summary>
		/// <param name="source">The underlying dictionary.</param>
		public DictionaryIndex(Dictionary<TKey, TValue> source)
		{
			Contract.Requires<ArgumentNullException>(source != null);

			this._source = source.ToDictionary(pair => pair.Key, pair => pair.Value);
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
			return this._source.TryGetValue(key, out value);
		}
	}
}
