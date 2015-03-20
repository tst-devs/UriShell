using System.Diagnostics.Contracts;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// A table for disconnecting objects from an user interface.
	/// </summary>
	[ContractClass(typeof(IUriDisconnectTableContract))]
	internal interface IUriDisconnectTable
	{
		/// <summary>
		/// Gets or sets <see cref="IUriPlacementConnector"/> for object's disconnection from an user interface.
		/// </summary>
		/// <param name="resolved">The object whose connector is got or set.</param>
		IUriPlacementConnector this[object resolved]
		{
			get;
			set;
		}

		/// <summary>
		/// Removes a disconnection entry of the given object.
		/// </summary>
		/// <param name="resolved">The object whose disconnection entry is deleted.</param>
		void Remove(object resolved);
	}
}
