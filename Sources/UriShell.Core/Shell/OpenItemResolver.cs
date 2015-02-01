using System;
using System.Diagnostics;

namespace UriShell.Shell
{
	/// <summary>
	/// Реализует <see cref="IUriModuleItemResolver"/>, позволяя открыть заданное
	/// приложение или документ средствами операционной системы.
	/// </summary>
	internal sealed  class OpenItemResolver : IUriModuleItemResolver
	{
		/// <summary>
		/// Создает объект по заданному URI.
		/// </summary>
		/// <param name="uri">URI, по которому требуется создать объект.</param>
		/// <param name="attachmentSelector">Селектор, предоставляющий доступ к объектам,
		/// прикрепленным к URI через параметры.</param>
		/// <returns>Объект, созданный по заданному URI.</returns>
		public object Resolve(Uri uri, UriAttachmentSelector attachmentSelector)
		{
			var builder = new PhoenixUriBuilder(uri);
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
