using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// The interface of a holder of opened via an URI objects and their metadata.
	/// </summary>
	[ContractClass(typeof(IUriResolvedObjectHolderContract))]
	internal interface IUriResolvedObjectHolder : IEnumerable<object>
	{
		/// <summary>
		/// Adds the new object opened via an URI. 
		/// </summary>
		/// <param name="resolved">The added object.</param>
		/// <param name="metadata">The metadata of the added object.</param>
		void Add(object resolved, UriResolvedMetadata metadata);

		/// <summary>
		/// Removes the object opened via an URI previously. 
		/// </summary>
		/// <param name="resolved">The removed object.</param>
		void Remove(object resolved);

		/// <summary>
		/// Checks whether the object is present in the holder.
		/// </summary>
		/// <param name="resolved">The object whose presence in the holder is checked.</param>
		/// <returns>true, if the holder contains the object; false otherwise.</returns>
		[Pure]
		bool Contains(object resolved);

		/// <summary>
		/// Gets an object stored in the holder by its identifier.
		/// </summary>
		/// <param name="id">The identifier of a requested object.</param>
		/// <returns>The object with the given identifier.</returns>
		[Pure]
		object Get(int id);

		/// <summary>
		/// Get the metadata of the given object stored in the holder.
		/// </summary>
		/// <param name="resolved">The object whose metadata is requested.</param>
		/// <returns>The metadata of the given object.</returns>
		[Pure]
		UriResolvedMetadata GetMetadata(object resolved);

		/// <summary>
		/// Replace the metadata of the given object with a new metadata based on the given URI.
		/// </summary>
		/// <param name="resolved">The objec whose metadata are replaced.</param>
		/// <param name="overrideUri">The URI for the new metadata.</param>
		void ModifyMetadata(object resolved, Uri overrideUri);
	}
}
