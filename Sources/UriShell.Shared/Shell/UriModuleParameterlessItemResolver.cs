using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Implements <see cref="IUriModuleItemResolver"/> for creating an object 
	/// with a factory of the given type with no parameters.
	/// </summary>
	/// <typeparam name="T">The type of the object created by this resolver.</typeparam>
	public sealed class UriModuleParameterlessItemResolver<T> : IUriModuleItemResolver
	{
		/// <summary>
		/// The factory for object's creating.
		/// </summary>
		private readonly Func<T> _factory;

		/// <summary>
		/// Initializes a new instance of the class <see cref="UriModuleParameterlessItemResolver{T}"/>.
		/// </summary>
		/// <param name="factory">The factory for object's creating.</param>
		public UriModuleParameterlessItemResolver(Func<T> factory)
		{
			Contract.Requires<ArgumentNullException>(factory != null);

			this._factory = factory;
		}

		/// <summary>
		/// Creates an object from the given URI.
		/// </summary>
		/// <param name="uri">The URI that describes an object to be created.</param>
		/// <param name="attachmentSelector">The selector for acccess to attached to the given URI objects.</param>
		/// <returns>The object created from the given URI.</returns>
		public object Resolve(Uri uri, UriAttachmentSelector attachmentSelector)
		{
			return this._factory();
		}
	}
}
