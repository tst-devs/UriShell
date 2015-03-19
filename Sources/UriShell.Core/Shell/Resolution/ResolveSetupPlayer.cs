using System;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// A function for calling setup of the object resolved via the URI.
	/// </summary>
	/// <param name="uri">The URI of the resolved object.</param>
	/// <param name="resolved">The object resolved via the URI.</param>
	/// <returns>The service for object's disposal.</returns>
	internal delegate IDisposable ResolveSetupPlayer(Uri uri, object resolved);
}
