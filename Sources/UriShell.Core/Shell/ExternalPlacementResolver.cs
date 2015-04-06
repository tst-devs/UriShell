using System;
using System.ComponentModel;
using System.Diagnostics;

namespace UriShell.Shell
{
	/// <summary>
	/// Implements object's placement in an external process.
	/// </summary>
	public sealed class ExternalPlacementResolver : IUriPlacementResolver, IUriPlacementConnector
	{
		/// <summary>
		/// Determines an object's placement from the given URI.
		/// </summary>
		/// <param name="resolved">The object to be placed with the given URI.</param>
		/// <param name="uri">The URI used to determine an object's placement.</param>
		/// <param name="attachmentSelector">The selector for acccess to attached to the given URI objects.</param>
		/// <returns>The <see cref="IUriPlacementConnector"/> that allows to connect the given object to the 
		/// user interface, if the placement was determined successfully; otherwise null.</returns>
		public IUriPlacementConnector Resolve(object resolved, Uri uri, UriAttachmentSelector attachmentSelector)
		{
			var builder = new PhoenixUriBuilder(uri);
			if (builder.Placement == "external" && resolved is Process)
			{
				return this;
			}

			return null;
		}

		/// <summary>
		/// Connects the given object to the user interface. 
		/// </summary>
		/// <param name="resolved">The object to be connected to the UI.</param>
		public void Connect(object resolved)
		{
			var process = (Process)resolved;
			try
			{
				process.Start();
			}
			catch (Win32Exception ex)
			{
				var message = string.Format(Properties.Resources.OpenExternalFileError, process.StartInfo.FileName, ex.Message);
				throw new Exception(message, ex);
			}
		}

		/// <summary>
		/// Disconnects the given object from the user interface. 
		/// </summary>
		/// <param name="resolved">The object to be disconnected from the given UI.</param>
		public void Disconnect(object resolved)
		{
			((Process)resolved).Kill();
		}

		/// <summary>
		/// Returns the flag that this connector is responsible for data refresh in connected objects.
		/// </summary>
		public bool IsResponsibleForRefresh
		{
			get 
			{
				return true;
			}
		}
	}
}
