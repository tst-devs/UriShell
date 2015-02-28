using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Interface of a service that determines an object's placement from a given URI.
	/// </summary>
	[ContractClass(typeof(IUriPlacementResolverContract))]
	public interface IUriPlacementResolver
	{
		/// <summary>
		/// Determines an object's placement from the given URI.
		/// </summary>
		/// <param name="resolved">The object to be placed with the given URI.</param>
		/// <param name="uri">The URI used to determine an object's placement.</param>
		/// <param name="attachmentSelector">The selector for acccess to attached to the given URI objects.</param>
		/// <returns>The <see cref="IUriPlacementConnector"/> that allows to connect the given object to the 
		/// user interface, if the placement was determined successfully; otherwise null.</returns>
		IUriPlacementConnector Resolve(object resolved, Uri uri, UriAttachmentSelector attachmentSelector);
	}
}
