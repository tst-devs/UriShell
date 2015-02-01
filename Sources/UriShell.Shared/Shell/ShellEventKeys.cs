using UriShell.Shell.Events;

namespace UriShell.Shell
{
	/// <summary>
	/// Ключи событий, рассылаемых оболочкой.
	/// </summary>
	public static class ShellEventKeys
	{
		/// <summary>
		/// Ключ события, рассылаемого каждую секунду в целях синхронизации времени.
		/// </summary>
		public static readonly EventKey<OneSecondElapsedBroadcastArgs> OneSecondElapsed = new EventKey<OneSecondElapsedBroadcastArgs>();

		/// <summary>
		/// Ключ события, рассылаемого с целью вызова обновления данных в объекте, открытом через URI.
		/// </summary>
		public static readonly EventKey<ResolvedIdBroadcastArgs> RefreshResolved = new EventKey<ResolvedIdBroadcastArgs>();

		/// <summary>
		/// Ключ события, рассылаемого с целью получить актуальную строку запроса объекта, открытого через URI.
		/// </summary>
		public static readonly EventKey RequestUriQueryString = new EventKey();

		/// <summary>
		/// Ключ события сигнализирующего об изменении состава избранного.
		/// </summary>
		public static readonly EventKey<string> FavoritesChanged = new EventKey<string>(); 
	}
}
