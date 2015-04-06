using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Collections
{
	/// <summary>
	/// Provides readonly access to a value by a key.
	/// </summary>
	/// <typeparam name="TKey">The type of a key in the collection.</typeparam>
	/// <typeparam name="TValue">The type of a value in the collection.</typeparam>
	public interface IIndex<in TKey, TValue>
	{
		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="value">When this method returns, contains the value associated
		/// with the specified key, if the key is found.</param>
		/// <returns>true - if the value was found; false otherwise.</returns>
		bool TryGetValue(TKey key, out TValue value);
	}
}
