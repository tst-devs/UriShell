using System;
using System.Collections;
using System.Collections.Specialized;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// A metadata of an object resolved via an URI.
	/// </summary>
	public struct UriResolvedMetadata
	{		
		/// <summary>
		/// Gets the URI of the resolved object.
		/// </summary>
		private readonly Uri _uri;

		/// <summary>
		/// Gets the service for object's disposal.
		/// </summary>
		private readonly IDisposable _disposable;

		/// <summary>
		/// Gets the identifier of the object resolved via an URI.
		/// </summary>
		private readonly int _resolvedId;

		/// <summary>
		/// Initializes a new instance of the class <see cref="UriResolvedMetadata"/>.
		/// </summary>
		/// <param name="uri">The URI of the resolved object.</param>
		/// <param name="disposable">The service for object's disposal.</param>
		public UriResolvedMetadata(Uri uri, IDisposable disposable)
		{
			this._uri = uri;
			this._disposable = disposable;
			this._resolvedId = 0;
		}

		/// <summary>
		/// Initializes a new instance of the class <see cref="UriResolvedMetadata"/>.
		/// </summary>
		/// <param name="source">The metadata used as a source for the new metadata.</param>
		/// <param name="resolvedId">The identifier of the object resolved via an URI.</param>
		private UriResolvedMetadata(UriResolvedMetadata source, int resolvedId)
		{
			this = source;
			this._resolvedId = resolvedId;
		}

		/// <summary>
		/// Get the new metadata with the assigned identifier of the object, resolved via an URI.
		/// </summary>
		/// <param name="resolvedId">The identifier of the object resolved via an URI.</param>
		/// <returns>The new metadata with the assigned identifier of the object.</returns>
		public UriResolvedMetadata AssignId(int resolvedId)
		{
			return new UriResolvedMetadata(this, resolvedId);
		}

		/// <summary>
		/// Gets the URI of the resolved object.
		/// </summary>
		public Uri Uri
		{
			get
			{
				return this._uri;
			}
		}

		/// <summary>
		/// Gets the service for object's disposal.
		/// </summary>
		public IDisposable Disposable
		{
			get
			{
				return this._disposable;
			}
		}

		/// <summary>
		/// Gets the identifier of the object resolved via an URI.
		/// </summary>
		public int ResolvedId
		{
			get
			{
				return this._resolvedId;
			}
		}
	}
}
