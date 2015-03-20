using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Disposables;

using UriShell.Extensions;
using UriShell.Shell.Events;
using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Allows to use an object resolved via an URI.
	/// </summary>
	internal sealed partial class ResolveOpen : IShellResolve
	{
		/// <summary>
		/// URI to be resolved.
		/// </summary>
		private readonly Uri _unresolvedUri;

		/// <summary>
		/// The list of attached to an URI objects with identifiers.
		/// </summary>
		private readonly object[] _attachments;

		/// <summary>
		/// The factory of a service for setup of objects resolved via an URI.
		/// </summary>
		private readonly IResolveSetupFactory _resolveSetupFactory;

		/// <summary>
		/// The holder of objects resolved via an URI.
		/// </summary>
		private readonly IUriResolvedObjectHolder _uriResolvedObjectHolder;

		/// <summary>
		/// The object that provides custom components of the URI resolution process.
		/// </summary>
		private readonly IUriResolutionCustomization _uriResolutionCustomization;

		/// <summary>
		/// The function for calling setup for an object resolved via an URI.
		/// Функция для вызова настройки объекта, полученного через URI.
		/// </summary>
		private ResolveSetupPlayer _resolveSetupPlayer;

		/// <summary>
		/// The service for event broadcasting.
		/// </summary>
		private readonly IEventBroadcaster _eventBroadcaster;

		/// <summary>
		/// The table for disconnecting objects from an user interface.
		/// </summary>
		private readonly IUriDisconnectTable _uriDisconnectTable;

		/// <summary>
		/// Initializes a new instance of the class <see cref="ResolveOpen"/>.
		/// </summary>
		/// <param name="uri">URI to be resolved.</param>
		/// <param name="attachments">The list of attached to an URI objects with identifiers..</param>
		/// <param name="resolveSetupFactory">The factory of a service for setup of objects resolved via an URI.</param>
		/// <param name="uriResolvedObjectHolder">The holder of objects resolved via an URI.</param>
		/// <param name="uriDisconnectTable">The table for disconnecting objects from an user interface.</param>
		/// <param name="uriResolutionCustomization">The object that provides custom components of the URI resolution process.</param>
		/// <param name="eventBroadcaster">The service for event broadcasting.</param>
		public ResolveOpen(
			Uri uri,
			object[] attachments,
			IResolveSetupFactory resolveSetupFactory,
			IUriResolvedObjectHolder uriResolvedObjectHolder,
			IUriDisconnectTable uriDisconnectTable,
			IUriResolutionCustomization uriResolutionCustomization,
			IEventBroadcaster eventBroadcaster)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentNullException>(attachments != null);
			Contract.Requires<ArgumentNullException>(resolveSetupFactory != null);
			Contract.Requires<ArgumentNullException>(uriResolvedObjectHolder != null);
			Contract.Requires<ArgumentNullException>(uriDisconnectTable != null);
			Contract.Requires<ArgumentNullException>(uriResolutionCustomization != null);
			Contract.Requires<ArgumentNullException>(eventBroadcaster != null);

			this._unresolvedUri = uri;
			this._attachments = attachments;
			this._resolveSetupFactory = resolveSetupFactory;
			this._uriResolvedObjectHolder = uriResolvedObjectHolder;
			this._uriResolutionCustomization = uriResolutionCustomization;
			this._eventBroadcaster = eventBroadcaster;
			this._uriDisconnectTable = uriDisconnectTable;
		}

		/// <summary>
		/// Allows to setup an object resolved from a URI 
		/// if its type is compatible with <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="TResolved">The object's type expected from a URI.</typeparam>
		/// <returns>The service for object's setup.</returns>
		public IShellResolveSetup<TResolved> Setup<TResolved>()
		{
			return this._resolveSetupFactory.Create<TResolved>(new ResolveSetupArgs(this, this.ReceiveResolveSetupPlayer));
		}

		/// <summary>
		/// Receives the function for calling setup for an object resolved via an URI.
		/// </summary>
		/// <param name="player">The function for calling setup for an object resolved via an URI.</param>
		private void ReceiveResolveSetupPlayer(ResolveSetupPlayer player)
		{
			if (player == null)
			{
				throw new InvalidOperationException(
					string.Format(Properties.Resources.ShellResolveSetupPlayerCannotBeNull, this._unresolvedUri));
			}

			if (this._resolveSetupPlayer != null)
			{
				throw new InvalidOperationException(
					string.Format(Properties.Resources.AttemptToOverwriteShellResolveSetupPlayer, this._unresolvedUri));
			}

			this._resolveSetupPlayer = player;
		}

		/// <summary>
		/// Builds an URI, where parameter's values are replaced with generated identifiers of attached objects 
		/// and provides a selector for getting this objects.
		/// </summary>
		/// <param name="uri">When this method returns, contains the URI, where parameter's values are replaced 
		/// with generated identifiers of attached objects </param>
		/// <param name="attachmentSelector">When this method returns, contains the selector for getting objects 
		/// attached to the URI with identifiers.</param>
		private void EmbedAttachments(out Uri uri, out UriAttachmentSelector attachmentSelector)
		{
			if (this._attachments.Length == 0)
			{
				attachmentSelector = id => null;
				uri = this._unresolvedUri;

				return;
			}

			var attachmentDictionary = new HybridDictionary(this._attachments.Length);
			var idGenerator = new Random();

			// Replace parameter's values in the URI with identifiers.
			var uriBuilder = new PhoenixUriBuilder(this._unresolvedUri);
			for (int i = 0; i < uriBuilder.Parameters.Count; i++)
			{
				var value = uriBuilder.Parameters[i];

				// Detect object's placeholder by the string "{index}",
				// where index is integer from the attachment number's range.

				if (value.Length <= 2)
				{
					continue;
				}
				if (value[0] != '{')
				{
					continue;
				}
				if (value[value.Length - 1] != '}')
				{
					continue;
				}

				value = value.Substring(1, value.Length - 2);

				int attachmentIndex;
				if (int.TryParse(value, out attachmentIndex))
				{
					if (attachmentIndex >= this._attachments.Length)
					{
						continue;
					}

					value = idGenerator.Next().ToString();
					attachmentDictionary[value] = this._attachments[attachmentIndex];
				}

				uriBuilder.Parameters[uriBuilder.Parameters.AllKeys[i]] = value;
			}

			attachmentSelector = id => id != null ? attachmentDictionary[id] : null;
			uri = uriBuilder.Uri;
		}

		/// <summary>
		/// Gets the object responsible for resolution of the given URI. 
		/// </summary>
		/// <param name="uri">The URI of the object being resolved.</param>
		/// <param name="attachmentSelector">The selector for objects attached to the URI.</param>
		/// <returns>The object responsible for resolution of the given URI. </returns>
		private object ResolveModuleItem(Uri uri, UriAttachmentSelector attachmentSelector)
		{
			var builder = new PhoenixUriBuilder(uri);
			var moduleItemResolverKey = new UriModuleItemResolverKey(builder.Module, builder.Item);

			IUriModuleItemResolver resolver;
			if (this._uriResolutionCustomization.ModuleItemResolvers.TryGetValue(moduleItemResolverKey, out resolver))
			{
				return resolver.Resolve(uri, attachmentSelector);
			}

			throw new UriResolutionException(
				uri, 
				string.Format(Properties.Resources.UriModuleItemResolverNotFound, typeof(IUriModuleItemResolver).Name));
		}

		/// <summary>
		/// Gets the object responsible for the given URI's placement. 
		/// </summary>
		/// <param name="uri">The URI of the object being resolved.</param>
		/// <param name="attachmentSelector">The selector for objects attached to the URI.</param>
		/// <param name="resolved">The object resolved via the URI.</param>
		/// <returns><see cref="IUriPlacementConnector"/> for connecting the object to an user interface.</returns>
		private IUriPlacementConnector ResolvePlacement(Uri uri, UriAttachmentSelector attachmentSelector, object resolved)
		{
			var placementConnector = this._uriResolutionCustomization
				.PlacementResolvers
				.Select(r => r.Resolve(resolved, uri, attachmentSelector))
				.FirstOrDefault(c => c != null);

			if (placementConnector != null)
			{
				return placementConnector;
			}

			throw new UriResolutionException(
				uri,
				string.Format(Properties.Resources.UriPlacementNotFound, typeof(IUriPlacementResolver).Name));
		}

		/// <summary>
		/// Tries to make the object resolved via the URI available in the shell.
		/// </summary>
		/// <param name="uri">The URI of the resolved object.</param>
		/// <param name="resolved">The object resolved via the URI.</param>
		/// <param name="placementConnector"><see cref="IUriPlacementConnector"/> 
		/// for connecting the object to the user interface.</param>
		/// <param name="appendToDisposable">When this method returns, contains the action that allows to add a <see cref="IDisposable"/> 
		/// to the group of <see cref="IDisposable"/> in the metadata.</param>
		/// <returns><see cref="IDisposable"/> registered in the metadata.</returns>
		private IDisposable Connect(Uri uri, object resolved, IUriPlacementConnector placementConnector, out Action<IDisposable> appendToDisposable)
		{
			placementConnector.Connect(resolved);
			try
			{
				var composite = new CompositeDisposable();
				appendToDisposable = composite.Add;

				this._uriResolvedObjectHolder.Add(resolved, new UriResolvedMetadata(uri, composite));
				this._uriDisconnectTable[resolved] = placementConnector;

				return composite;
			}
			catch (Exception ex)
			{
				if (!ex.IsCritical())
				{
					// When failed to add the object in the holder - disconnect it from the UI.
					placementConnector.Disconnect(resolved);
				}

				throw;
			}
		}

		/// <summary>
		/// Calls setup of the object resolved via the URI.
		/// </summary>
		/// <param name="uri">The URI of the resolved object.</param>
		/// <param name="resolved">The object resolved via the URI.</param>
		/// <returns>The service called when object is disposed.</returns>
		private IDisposable PlaySetup(Uri uri, object resolved)
		{
			if (this._resolveSetupPlayer != null)
			{
				try
				{
					return this._resolveSetupPlayer(uri, resolved);
				}
				catch (Exception ex)
				{
					if (ex.IsCritical())
					{
						throw;
					}

					// Успешность вызова настроек не должна влиять на вызывающий код.
					Trace.TraceError(ex.ToString());
				}
			}

			return Disposable.Empty;
		}

		/// <summary>
		/// Creates the <see cref="IDisposable"/> for object's closing.
		/// </summary>
		/// <param name="resolved">The object resolved via the URI.</param>
		/// <param name="uriResolvedObjectHolder">The holder of objects via an URI.</param>
		/// <param name="uriDisconnectTable">The table for disonnectiong objects from the user interface.</param>
		/// <returns><see cref="IDisposable"/> for object's closing.</returns>
		private static IDisposable CreateCloseDisposable(
			object resolved,
			IUriResolvedObjectHolder uriResolvedObjectHolder,
			IUriDisconnectTable uriDisconnectTable)
		{
			return Disposable.Create(() =>
			{
				try
				{
					uriDisconnectTable[resolved].Disconnect(resolved);

					var disposableResolved = resolved as IDisposable;
					if (disposableResolved != null)
					{
						disposableResolved.Dispose();
					}

					uriDisconnectTable.Remove(resolved);
					uriResolvedObjectHolder.Remove(resolved);
				}
				catch (Exception ex)
				{
					if (ex.IsCritical())
					{
						throw;
					}

					Trace.TraceError(ex.ToString());
				}
			});
		}

		/// <summary>
		/// Sends the event for data refresh in the given object resolved via an URI. 
		/// </summary>
		/// <param name="resolved">The object resolved via an URI.</param>
		/// <param name="placementConnector">The <see cref="IUriPlacementConnector"/> 
		/// that connected the object to the user interface.</param>
		private void SendRefresh(object resolved, IUriPlacementConnector placementConnector)
		{
			if (placementConnector.IsResponsibleForRefresh)
			{
				return;
			}

			var id = this._uriResolvedObjectHolder.GetMetadata(resolved).ResolvedId;
			var args = new ResolvedIdBroadcastArgs(id);
			this._eventBroadcaster.Send(ShellEventKeys.RefreshResolved, args);
		}

		/// <summary>
		/// Opens an object resolved from an URI.
		/// </summary>
		/// <returns>The service for object's closing via <see cref="IDisposable.Dispose"/>.</returns>
		public IDisposable Open()
		{
			try
			{
				return this.OpenOrThrow();
			}
			catch (Exception ex)
			{
				if (ex.IsCritical())
				{
					throw;
				}

				Trace.TraceError(ex.ToString());
			}

			return Disposable.Empty;
		}

		/// <summary>
		/// Opens an object resolved from an URI and allows the calling site to handle an exception 
		/// when it occurs.
		/// </summary>
		/// <returns>The service for object's closing via <see cref="IDisposable.Dispose"/>,
		/// if an object was opened successfully.</returns>
		public IDisposable OpenOrThrow()
		{
			Uri uri;
			UriAttachmentSelector attachmentSelector;
			this.EmbedAttachments(out uri, out attachmentSelector);

			var resolved = this.ResolveModuleItem(uri, attachmentSelector);
			var placementConnector = this.ResolvePlacement(uri, attachmentSelector, resolved);

			Action<IDisposable> appendToDisposable;
			var disposable = this.Connect(uri, resolved, placementConnector, out appendToDisposable);
				
			appendToDisposable(this.PlaySetup(uri, resolved));
			appendToDisposable(ResolveOpen.CreateCloseDisposable(
				resolved,
				this._uriResolvedObjectHolder,
				this._uriDisconnectTable));

			Trace.TraceInformation(Properties.Resources.ShellResolveOpenComplete, uri);
			this.SendRefresh(resolved, placementConnector);

			return disposable;
		}
	}
}