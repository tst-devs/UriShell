using System;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Представляет собой функцию для вызова настройки объекта, полученного через URI.
	/// </summary>
	/// <param name="uri">URI из которого был получен объект.</param>
	/// <param name="resolved">Объект, полученный через URI.</param>
	/// <returns>Сервис, вызываемый, когда в объекте больше нет необходимости.</returns>
	internal delegate IDisposable ResolveSetupPlayer(Uri uri, object resolved);
}
