using UriShell.Shell.Events;

namespace UriShell.Shell
{
	/// <summary>
	/// Describes keys of events sent by the shell.
	/// </summary>
	public static class ShellEventKeys
	{
		/// <summary>
		/// The key of the event sent every 1 second for synchronization.
		/// </summary>
		public static readonly EventKey<OneSecondElapsedBroadcastArgs> OneSecondElapsed = new EventKey<OneSecondElapsedBroadcastArgs>();

		/// <summary>
		/// The key of the event sent for refreshing data in an object opened via a URI.
		/// </summary>
		public static readonly EventKey<ResolvedIdBroadcastArgs> RefreshResolved = new EventKey<ResolvedIdBroadcastArgs>();

		/// <summary>
		/// The key of the event sent for requesting of an actual query string of an object opened via a URI.
		/// </summary>
		public static readonly EventKey RequestUriQueryString = new EventKey();
	}
}
