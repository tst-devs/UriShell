using System;
using System.Linq;

namespace UriShell.Shell
{
	/// <summary>
	/// Implements <see cref="IUriModuleItemResolver"/> for creating an object with a factory 
	/// of the given result type and a "title" input parameter stored in a URI.
	/// </summary>
	/// <typeparam name="T">The type of the object created by this resolver.</typeparam>
	public sealed class UriModuleEntitledItemResolver<T> : IUriModuleItemResolver
	{
		/// <summary>
		/// The factory for object's creating.
		/// </summary>
		private readonly Func<string, T> _entitledItemFactory;

		/// <summary>
		/// Initializes a new instance of the class <see cref="UriModuleEntitledItemResolver{T}"/>.
		/// </summary>
		/// <param name="entitledItemFactory">The factory for object's creating.</param>
		public UriModuleEntitledItemResolver(Func<string, T> entitledItemFactory)
		{
			this._entitledItemFactory = entitledItemFactory;
		}

		/// <summary>
		/// Creates an object from the given URI.
		/// </summary>
		/// <param name="uri">The URI that describes an object to be created.</param>
		/// <param name="attachmentSelector">The selector for acccess to attached to the given URI objects.</param>
		/// <returns>The object created from the given URI.</returns>
		public object Resolve(Uri uri, UriAttachmentSelector attachmentSelector)
		{
			var uriBuilder = new PhoenixUriBuilder(uri);
			var title = uriBuilder.Parameters["title"] ?? uri.ToString();

			return this._entitledItemFactory(title);	
		}
	}
}
