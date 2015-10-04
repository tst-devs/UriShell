using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

using UriShell.Collections;
using UriShell.Extensions;
using UriShell.Shell.Registration;
using UriShell.Shell.Resolution;

namespace UriShell.Shell
{
	using ShellResolveFactory = Func<Uri, object[], IShellResolve>;

	/// <summary>
	/// Implementation of the application shell <see cref="IShell"/>.
	/// </summary>
	public sealed partial class Shell : IShell, IUriResolutionCustomization
	{
		/// <summary>
		/// The regular expression for parsing hyperlinks.
		/// </summary>
		private static readonly Regex _HyperLinkRegex = new Regex("<a\\s+href=\"([^\"]+)\">(.*)</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// The factory of an object responsible for URI's resolution beginning.
		/// </summary>
		private readonly ShellResolveFactory _shellResolveFactory;

		/// <summary>
		/// The holder for objects opened by the shell via a URI.
		/// </summary>
		private readonly IUriResolvedObjectHolder _uriResolvedObjectHolder;
	
		/// <summary>
		/// The list of services for looking for object's placement by a URI.
		/// </summary>
		private readonly WeakBucket<IUriPlacementResolver> _uriPlacementResolvers = new WeakBucket<IUriPlacementResolver>();

        /// <summary>
        /// The dictionary of object's resolvers stored by the key.
        /// </summary>
		private readonly Dictionary<UriModuleItemResolverKey, IUriModuleItemResolver> _uriModuleItemResolvers = new Dictionary<UriModuleItemResolverKey, IUriModuleItemResolver>();

        /// <summary>
        /// The dictionary with updated <see cref="Uri"/> for resolved objects.
        /// </summary>
        private readonly Dictionary<object, Uri> _updatedUris = new Dictionary<object, Uri>();

        /// <summary>
        /// Initializes a new instance of the class <see cref="Shell"/>.
        /// </summary>
        /// <param name="uriResolvedObjectHolder">The holder for objects opened by the shell via a URI.</param>
        /// <param name="shellResolveFactory">The factory of an object responsible for URI's resolution beginning.</param>
        public Shell(
			IUriResolvedObjectHolder uriResolvedObjectHolder,
			ShellResolveFactory shellResolveFactory)
		{
			Contract.Requires<ArgumentNullException>(shellResolveFactory != null);
			Contract.Requires<ArgumentNullException>(uriResolvedObjectHolder != null);

			// If user hasn't specified settings, initialize with defaults.
			if (Settings.Instance == null)
			{
				Settings.Initialize(b => { });
			}

			this._shellResolveFactory = shellResolveFactory;
			this._uriResolvedObjectHolder = uriResolvedObjectHolder;
		}

		/// <summary>
		/// Gets the list of services responsible for creating an object by a URI 
		/// and registered with <see cref="UriModuleItemResolverKey"/>.
		/// </summary>
		public IIndex<UriModuleItemResolverKey, IUriModuleItemResolver> ModuleItemResolvers
		{
			get
			{
				return new DictionaryIndex<UriModuleItemResolverKey, IUriModuleItemResolver>(this._uriModuleItemResolvers);
			}
		}

		/// <summary>
		/// Gets the list of services for looking for object's placement by a URI.
		/// </summary>
		public IEnumerable<IUriPlacementResolver> PlacementResolvers
		{
			get
			{
				return this._uriPlacementResolvers.ExtractAlive();
			}
		}

		/// <summary>
		/// Adds the given <see cref="IUriPlacementResolver"/> to the shell.
		/// </summary>
		/// <param name="uriPlacementResolver">The <see cref="IUriPlacementResolver"/>
		/// to be added with a weak reference. </param>
		public void AddUriPlacementResolver(IUriPlacementResolver uriPlacementResolver)
		{
			this._uriPlacementResolvers.Add(uriPlacementResolver);
		}

		/// <summary>
		/// Adds the given <see cref="IUriModuleItemResolver"/> as an object resolver by the given key.
		/// </summary>
		/// <param name="key">Key for access to the <see cref="IUriModuleItemResolver"/> being added.</param>
		/// <param name="uriModuleItemResolver">The <see cref="IUriModuleItemResolver"/> being added.</param>
		public void AddUriModuleItemResolver(UriModuleItemResolverKey key, IUriModuleItemResolver uriModuleItemResolver)
		{
			this._uriModuleItemResolvers.Add(key, uriModuleItemResolver);
		}

		/// <summary>
		/// Starts an opening flow of the given URI.
		/// </summary>
		/// <param name="uri">The URI to be opened.</param>
		/// <param name="attachments">The object list attached to the URI with identifiers.</param>
		/// <returns>The service that could customize and open the object received via the URI.</returns>
		public IShellResolve Resolve(Uri uri, params object[] attachments)
		{
			return this._shellResolveFactory(uri, attachments);
		}

		/// <summary>
		/// Checks whether the given object is open.
		/// </summary>
		/// <param name="resolved">The object to be checked.</param>
		/// <returns>true, if the object is opened; otherwise false.</returns>
		public bool IsResolvedOpen(object resolved)
		{
			return this._uriResolvedObjectHolder.Contains(resolved);
		}

		/// <summary>
		/// Returns the identifier of the given object opened via the URI.
		/// </summary>
		/// <param name="resolved">The object which identifier is gotten.</param>
		/// <returns>The identifier of the given object.</returns>
		public int GetResolvedId(object resolved)
		{
			return this._uriResolvedObjectHolder.GetMetadata(resolved).ResolvedId;
		}

		/// <summary>
		/// Returns a URI of the given opened object.
		/// </summary>
		/// <param name="resolved">The object which URI is gotten.</param>
		/// <returns>The URI of the given opened object.</returns>
		public Uri GetResolvedUri(object resolved)
		{
            Uri updatedUri;
            if (this._updatedUris.TryGetValue(resolved, out updatedUri))
            {
                return updatedUri;
            }

            return this._uriResolvedObjectHolder.GetMetadata(resolved).Uri;
		}

		/// <summary>
		/// Closes the given resolved object.
		/// </summary>
		/// <param name="resolved">The resolved object to be closed.</param>
		public void CloseResolved(object resolved)
		{
            this._updatedUris.Remove(resolved);
            this._uriResolvedObjectHolder.GetMetadata(resolved).Disposable.Dispose();
		}

		/// <summary>
		/// Tries to create a hyperlink from the given text description.
		/// </summary>
		/// <param name="hyperlink">The text description of a hyperlink in HTML anchor format.</param>
		/// <param name="ownerId">The identifier of the object that owns the object opened with the hyperlink.</param>
		/// <returns>The hyperlink, if the given description of a hyperlink is valid; otherwise null.</returns>
		public ShellHyperlink TryParseHyperlink(string hyperlink, int ownerId)
		{
			var matches = Shell._HyperLinkRegex.Matches(hyperlink);
			if (matches.Count == 0)
			{
				return null;
			}

			// If the text matches the hyperlink template
			// then return a hyperlink.

			var match = matches[0];
			var uri = new Uri(match.Groups[1].Value);
			var title = match.Groups[2].Value;

			if (uri.IsUriShell())
			{
				// Add owner ID to the view URI. 
				var builder = new ShellUriBuilder(uri);
				builder.OwnerId = ownerId;
				uri = builder.Uri;
			}

			return new ShellHyperlink(uri, title, null);
		}

		/// <summary>
		/// Creates a hyperlink for openening of the given <see cref="Uri"/>.
		/// </summary>
		/// <param name="uri">The URI, for which a hyperlink is created.</param>
		/// <returns>The hyperlink created for openening of the given <see cref="Uri"/>.</returns>
		public ShellHyperlink CreateHyperlink(Uri uri)
		{
			var builder = new ShellUriBuilder(uri);
			var title = builder.Parameters["title"];

			var iconParameter = builder.Parameters["icon"];
			Uri icon = null;

			if (!Uri.TryCreate(iconParameter, UriKind.Absolute, out icon))
			{
				throw new ArgumentException(string.Format(Properties.Resources.InvalidIconUri, iconParameter));
			}

			return new ShellHyperlink(uri, title, icon);
		}

        /// <summary>
        /// Updates <see cref="Uri"/> of the given resolved object. 
        /// </summary>
        /// <param name="resolved">The object, whose <see cref="Uri"/> needs to be updated.</param>
        /// <param name="newUri">The new <see cref="Uri"/> for the given resolved object.</param>
        public void UpdateResolvedUri(object resolved, Uri newUri)
        {
            this._updatedUris[resolved] = newUri;
        }
    }
}
