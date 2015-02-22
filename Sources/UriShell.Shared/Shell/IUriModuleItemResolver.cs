using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Interface of a service that is responsible for object creation from the given URI.
	/// </summary>
	[ContractClass(typeof(IUriModuleItemResolverContract))]
	public interface IUriModuleItemResolver
	{
		/// <summary>
		/// Creates an object from the given URI.
		/// </summary>
		/// <param name="uri">The URI that describes an object to be created.</param>
		/// <param name="attachmentSelector">The selector for acccess to attached to the given URI objects.</param>
		/// <returns>The object created from the given URI.</returns>
		object Resolve(Uri uri, UriAttachmentSelector attachmentSelector);
	}
}
