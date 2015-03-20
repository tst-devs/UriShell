using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Arguments of the event sent for refreshing data in an object opened via a URI.
	/// </summary>
	public sealed class ResolvedIdBroadcastArgs
	{
		/// <summary>
		/// Initializes a new instance of the class <see cref="ResolvedIdBroadcastArgs"/>.
		/// </summary>
		/// <param name="resolvedId">The identifier of the object opened via the URI 
		/// that should refresh data.</param>
		public ResolvedIdBroadcastArgs(int resolvedId)
		{
			this.ResolvedId = resolvedId;
		}

		/// <summary>
		/// Describes the invariant of the class.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.ResolvedId >= PhoenixUriBuilder.MinResolvedId);
			Contract.Invariant(this.ResolvedId <= PhoenixUriBuilder.MaxResolvedId);
		}

		/// <summary>
		/// Gets the identifier of the object opened via the URI 
		/// that should refresh data.
		/// </summary>
		public int ResolvedId
		{
			get;
			private set;
		}
	}
}
