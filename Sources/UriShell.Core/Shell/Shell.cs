﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

using Autofac.Features.Indexed;

using UriShell.Extensions;
using UriShell.Shell.Registration;
using UriShell.Shell.Resolution;

namespace UriShell.Shell
{
	using ShellResolveFactory = Func<Uri, object[], IShellResolve>;
	using UriModuleItemResolverIndex = IIndex<UriModuleItemResolverKey, IUriModuleItemResolver>;

#warning Change to default Uri Provider
    internal interface ISecurityService
    {

    }

	/// <summary>
	/// Implementation of the application shell <see cref="IShell"/>.
	/// </summary>
	internal sealed partial class Shell : IShell, IUriResolutionCustomization
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
		/// The factory of the list of services responsible for creating an object by a URI.
		/// </summary>
		private readonly Func<UriModuleItemResolverIndex> _uriModuleItemResolversFactory;

		/// <summary>
		/// Initializes a new instance of the class <see cref="Shell"/>.
		/// </summary>
		/// <param name="uriModuleItemResolversFactory">The factory of the list of services responsible for creating an object by a URI.</param>
		/// <param name="uriResolvedObjectHolder">The holder for objects opened by the shell via a URI.</param>
		/// <param name="shellResolveFactory">The factory of an object responsible for URI's resolution beginning.</param>
		public Shell(
			Func<UriModuleItemResolverIndex> uriModuleItemResolversFactory,
			IUriResolvedObjectHolder uriResolvedObjectHolder,
			ShellResolveFactory shellResolveFactory)
		{
			Contract.Requires<ArgumentNullException>(shellResolveFactory != null);
			Contract.Requires<ArgumentNullException>(uriResolvedObjectHolder != null);
			Contract.Requires<ArgumentNullException>(uriModuleItemResolversFactory != null);

			this._shellResolveFactory = shellResolveFactory;
			this._uriResolvedObjectHolder = uriResolvedObjectHolder;
			this._uriModuleItemResolversFactory = uriModuleItemResolversFactory;
		}

		/// <summary>
		/// Gets the list of services responsible for creating an object by a URI 
		/// and registered with <see cref="UriModuleItemResolverKey"/>.
		/// </summary>
		public IIndex<UriModuleItemResolverKey, IUriModuleItemResolver> ModuleItemResolvers
		{
			get
			{
				return this._uriModuleItemResolversFactory();
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
			return this._uriResolvedObjectHolder.GetMetadata(resolved).Uri;
		}

		/// <summary>
		/// Closes the given resolved object.
		/// </summary>
		/// <param name="resolved">The resolved object to be closed.</param>
		public void CloseResolved(object resolved)
		{
			this._uriResolvedObjectHolder.GetMetadata(resolved).Disposable.Dispose();
		}

		/// <summary>
		/// Tries to create a hyperlink from the given text description.
		/// </summary>
		/// <param name="hyperlink">The text description of a hyperlink in HTML anchor format.</param>
		/// <param name="ownerId">The identifier of the object that owns the object opened with the hyperlink.</param>
		/// <returns>The hyperlink, if the given description of a hyperlink is valid; otherwise null.</returns>
		public PhoenixHyperlink TryParseHyperlink(string hyperlink, int ownerId)
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

			if (uri.IsPhoenix())
			{
				// Add owner ID to the view URI. 
				var builder = new PhoenixUriBuilder(uri);
				builder.OwnerId = ownerId;
				uri = builder.Uri;
			}

			return new PhoenixHyperlink(uri, title, null);
		}

		/// <summary>
		/// Creates a hyperlink for openening of the given <see cref="Uri"/>.
		/// </summary>
		/// <param name="uri">The URI, for which a hyperlink is created.</param>
		/// <returns>The hyperlink created for openening of the given <see cref="Uri"/>.</returns>
		public PhoenixHyperlink CreateHyperlink(Uri uri)
		{
			var builder = new PhoenixUriBuilder(uri);
			var title = builder.Parameters["title"];

			var iconParameter = builder.Parameters["icon"];
			Uri icon = null;

			if (!Uri.TryCreate(iconParameter, UriKind.Absolute, out icon))
			{
				throw new Exception(string.Format(Properties.Resources.InvalidIconUri, iconParameter));
			}

			return new PhoenixHyperlink(uri, title, icon);
		}
	}
}
