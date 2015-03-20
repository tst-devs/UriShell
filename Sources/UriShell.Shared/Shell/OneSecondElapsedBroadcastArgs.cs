using System;

namespace UriShell.Shell
{
	/// <summary>
	/// Arguments of the event for synchronization sent every 1 second.
	/// </summary>
	public sealed class OneSecondElapsedBroadcastArgs
	{
		/// <summary>
		/// Initializes a new instance of the class <see cref="OneSecondElapsedBroadcastArgs"/>.
		/// </summary>
		/// <param name="totalElapsed">The total time elapsed since the start of the event broadcasting.</param>
		public OneSecondElapsedBroadcastArgs(TimeSpan totalElapsed)
		{
			this.TotalElapsed = totalElapsed;
		}

		/// <summary>
		/// The total time elapsed since the start of the event broadcasting
		/// </summary>
		public TimeSpan TotalElapsed
		{
			get;
			private set;
		}
	}
}
