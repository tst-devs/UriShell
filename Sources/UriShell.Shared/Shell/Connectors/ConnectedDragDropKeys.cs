using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Predefined keys for data transfer during dragging.
	/// </summary>
	public static class ConnectedDragDropKeys
	{
		/// <summary>
		/// The key for transferring an object opened via an URI.
		/// </summary>
		public static readonly ConnectedDragDropKey<object> UriConnectedView = new ConnectedDragDropKey<object>();
	}
}