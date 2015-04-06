using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Holds objects opened via an URI and their metadata.
	/// </summary>
	public sealed class UriResolvedObjectHolder : IUriResolvedObjectHolder
	{
		/// <summary>
		/// The dictionaty containing objects, resolved via an URI, along with their metadata.
		/// </summary>
		private readonly Dictionary<object, UriResolvedMetadata> _data = new Dictionary<object, UriResolvedMetadata>();

		/// <summary>
		/// Contains identifiers that are in use currently.
		/// </summary>
		private readonly HashSet<int> _usedIds = new HashSet<int>();

		/// <summary>
		/// The object's identifier generator.
		/// </summary>
		private readonly Random _random = new Random(PhoenixUriBuilder.MinResolvedId);

		/// <summary>
		/// Generates a new unique identifier of an object.
		/// </summary>
		/// <returns>The newly generated identifier.</returns>
		private int GenerateNewId()
		{
			if (this._usedIds.Count > PhoenixUriBuilder.MaxResolvedId - PhoenixUriBuilder.MinResolvedId)
			{
				throw new InvalidOperationException(Properties.Resources.NoAvailableUriResolutionId);
			}

			int id;
			do
			{
				id = this._random.Next(PhoenixUriBuilder.MaxResolvedId + 1);
			}
			while (!this._usedIds.Add(id));

			return id;
		}

		/// <summary>
		/// Adds the new object opened via an URI. 
		/// </summary>
		/// <param name="resolved">The added object.</param>
		/// <param name="metadata">The metadata of the added object.</param>
		public void Add(object resolved, UriResolvedMetadata metadata)
		{
			try
			{
				this._data.Add(resolved, metadata.AssignId(this.GenerateNewId()));
			}
			catch (ArgumentException)
			{
				throw new ArgumentException(
					string.Format(Properties.Resources.UriResolvedObjectAlreadyExists, resolved, typeof(IUriResolvedObjectHolder).Name),
					"resolved");
			}
		}

		/// <summary>
		/// Removes the object opened via an URI previously. 
		/// </summary>
		/// <param name="resolved">The removed object.</param>
		public void Remove(object resolved)
		{
			UriResolvedMetadata metadata;

			if (this._data.TryGetValue(resolved, out metadata))
			{
				this._data.Remove(resolved);
				this._usedIds.Remove(metadata.ResolvedId);
			}
		}

		/// <summary>
		/// Checks whether the object is present in the holder.
		/// </summary>
		/// <param name="resolved">The object whose presence in the holder is checked.</param>
		/// <returns>true, if the holder contains the object; false otherwise.</returns>
		public bool Contains(object resolved)
		{
			if (resolved == null)
			{
				return false;
			}

			if (this._data.ContainsKey(resolved))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets an object stored in the holder by its identifier.
		/// </summary>
		/// <param name="id">The identifier of a requested object.</param>
		/// <returns>The object with the given identifier.</returns>
		public object Get(int id)
		{
			foreach (var item in this._data)
			{
				if (item.Value.ResolvedId == id)
				{
					return item.Key;
				}
			}

			throw new ArgumentOutOfRangeException(
				"id",
				string.Format(Properties.Resources.UriResolvedObjectIdDoesntExist, id, typeof(IUriResolvedObjectHolder).Name));
		}

		/// <summary>
		/// Get the metadata of the given object stored in the holder.
		/// </summary>
		/// <param name="resolved">The object whose metadata is requested.</param>
		/// <returns>The metadata of the given object.</returns>
		public UriResolvedMetadata GetMetadata(object resolved)
		{
			UriResolvedMetadata metadata;

			if (this._data.TryGetValue(resolved, out metadata))
			{
				return metadata;
			}

			throw new ArgumentOutOfRangeException(
				"resolved",
				string.Format(Properties.Resources.UriResolvedObjectDoesntExist, resolved, typeof(IUriResolvedObjectHolder).Name));
		}

		/// <summary>
		/// Replace the metadata of the given object with a new metadata based on the given URI.
		/// </summary>
		/// <param name="resolved">The objec whose metadata are replaced.</param>
		/// <param name="overrideUri">The URI for the new metadata.</param>
		public void ModifyMetadata(object resolved, Uri overrideUri)
		{
			throw new NotImplementedException();
		}

		#region IEnumerable<object> Members

		/// <summary>
		/// Gets the iterator for iterating through objects held in the holder.
		/// </summary>
		/// <returns>The iterator for iterating through objects held in the holder.</returns>
		public IEnumerator<object> GetEnumerator()
		{
			return this._data.Keys.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Gets the iterator for iterating through objects held in the holder.
		/// </summary>
		/// <returns>The iterator for iterating through objects held in the holder.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}