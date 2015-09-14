using System;
using System.Diagnostics;

namespace UriShell.Shell
{
	/// <summary>
	/// Implements <see cref="IUriModuleItemResolver"/> for opening 
	/// applications and documents by means of the operating system.
	/// </summary>
	internal sealed  class OpenItemResolver : IUriModuleItemResolver
	{
		/// <summary>
		/// Creates an object from the given URI.
		/// </summary>
		/// <param name="uri">The URI that describes an object to be created.</param>
		/// <param name="attachmentSelector">The selector for acccess to attached to the given URI objects.</param>
		/// <returns>The object created from the given URI.</returns>
		public object Resolve(Uri uri, UriAttachmentSelector attachmentSelector)
		{
			var builder = new ShellUriBuilder(uri);
			var fileName = builder.Parameters["fileName"];

			if (string.IsNullOrWhiteSpace(fileName))
			{
				return null;
			}

			return new Process
			{
				StartInfo = new ProcessStartInfo(fileName),
			};
		}
	}
}
