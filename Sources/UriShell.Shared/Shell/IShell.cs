using System;
using System.Diagnostics.Contracts;
using UriShell.Shell.Registration;

namespace UriShell.Shell
{
	/// <summary>
	/// Interface of the application shell.
	/// </summary>
	[ContractClass(typeof(IShellContract))]
	public interface IShell
	{
		/// <summary>
		/// Adds the given <see cref="IUriPlacementResolver"/> to the shell.
		/// </summary>
		/// <param name="uriPlacementResolver">The <see cref="IUriPlacementResolver"/>
		/// to be added with a weak reference. </param>
		void AddUriPlacementResolver(IUriPlacementResolver uriPlacementResolver);

		/// <summary>
		/// Adds the given <see cref="IUriModuleItemResolver"/> as an object resolver by the given key.
		/// </summary>
		/// <param name="key">Key for access to the <see cref="IUriModuleItemResolver"/> being added.</param>
		/// <param name="uriModuleItemResolver">The <see cref="IUriModuleItemResolver"/> being added.</param>
		void AddUriModuleItemResolver(UriModuleItemResolverKey key, IUriModuleItemResolver uriModuleItemResolver);

		/// <summary>
		/// Starts an opening flow of the given URI.
		/// </summary>
		/// <param name="uri">The URI to be opened.</param>
		/// <param name="attachments">The object list attached to the URI with identifiers.</param>
		/// <returns>The service that could customize and open the object received via the URI.</returns>
		IShellResolve Resolve(Uri uri, params object[] attachments);

		/// <summary>
		/// Checks whether the given object is open.
		/// </summary>
		/// <param name="resolved">The object to be checked.</param>
		/// <returns>true, if the object is opened; otherwise false.</returns>
		[Pure]
		bool IsResolvedOpen(object resolved);

		/// <summary>
		/// Returns the identifier of the given object opened via the URI.
		/// </summary>
		/// <param name="resolved">The object which identifier is gotten.</param>
		/// <returns>The identifier of the given object.</returns>
		[Pure]
		int GetResolvedId(object resolved);

		/// <summary>
		/// Returns a URI of the given opened object.
		/// </summary>
		/// <param name="resolved">The object which URI is gotten.</param>
		/// <returns>The URI of the given opened object.</returns>
		[Pure]
		Uri GetResolvedUri(object resolved);

		/// <summary>
		/// Closes the given resolved object.
		/// </summary>
		/// <param name="resolved">The resolved object to be closed.</param>
		void CloseResolved(object resolved);

		/// <summary>
		/// Tries to create a hyperlink from the given text description.
		/// </summary>
		/// <param name="hyperlink">The text description of a hyperlink in HTML anchor format.</param>
		/// <param name="ownerId">The identifier of the object that owns the object opened with the hyperlink.</param>
		/// <returns>The hyperlink, if the given description of a hyperlink is valid; otherwise null.</returns>
		[Pure]
		ShellHyperlink TryParseHyperlink(string hyperlink, int ownerId);

		/// <summary>
		/// Creates a hyperlink for opening of the given <see cref="Uri"/>.
		/// </summary>
		/// <param name="uri">The URI, for which a hyperlink is created.</param>
		/// <returns>The hyperlink created for opening of the given <see cref="Uri"/>.</returns>
		[Pure]
		ShellHyperlink CreateHyperlink(Uri uri);
	}
}
