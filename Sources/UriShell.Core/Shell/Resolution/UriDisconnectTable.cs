using System.Collections.Generic;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Implementation of the table used for disconnecting objects from the user interface.
	/// </summary>
	internal sealed class UriDisconnectTable : IUriDisconnectTable
	{
		/// <summary>
		/// The dictionary, where every resolved object corresponds with a <see cref="IUriPlacementConnector"/> 
		/// used for its disconnection from the user interface.
		/// </summary>
		private readonly Dictionary<object, IUriPlacementConnector> _connectors = new Dictionary<object, IUriPlacementConnector>();

		/// <summary>
		/// Gets or sets <see cref="IUriPlacementConnector"/> for object's disconnection from an user interface.
		/// </summary>
		/// <param name="resolved">The object whose connector is got or set.</param>
		public IUriPlacementConnector this[object resolved]
		{
			get
			{
				IUriPlacementConnector connector;
				if (this._connectors.TryGetValue(resolved, out connector))
				{
					return connector;
				}

				throw new KeyNotFoundException(string.Format(
					Properties.Resources.UriResolvedNotRegisteredForDisconnect,
					resolved));
			}
			set
			{
				this._connectors[resolved] = value;
			}
		}

		/// <summary>
		/// Removes a disconnection entry of the given object.
		/// </summary>
		/// <param name="resolved">The object whose disconnection entry is deleted.</param>
		public void Remove(object resolved)
		{
			if (!this._connectors.Remove(resolved))
			{
				throw new KeyNotFoundException(string.Format(
					Properties.Resources.UriResolvedNotRegisteredForDisconnect,
					resolved));
			}
		}
	}
}
