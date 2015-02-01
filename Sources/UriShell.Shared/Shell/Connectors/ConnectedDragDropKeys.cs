using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Типовые ключи для передачи данных при перетаскивании.
	/// </summary>
	public static class ConnectedDragDropKeys
	{
		/// <summary>
		/// Ключ для передачи представления объекта, открытого через URI.
		/// </summary>
		public static readonly ConnectedDragDropKey<object> UriConnectedView = new ConnectedDragDropKey<object>();
	}
}