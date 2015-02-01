using System;

namespace UriShell.Shell
{
	/// <summary>
	/// Аргументы синхронизирующего события <see cref="ShellEventKeys.OneSecondElapsed"/>, 
	/// рассылаемого каждую секунду.
	/// </summary>
	public sealed class OneSecondElapsedBroadcastArgs
	{
		/// <summary>
		/// Инициализирует новый объект <see cref="OneSecondElapsedBroadcastArgs"/>.
		/// </summary>
		/// <param name="totalElapsed">Общее время, которое работает рассылка события.</param>
		public OneSecondElapsedBroadcastArgs(TimeSpan totalElapsed)
		{
			this.TotalElapsed = totalElapsed;
		}

		/// <summary>
		/// Возвращает общее время, которое работает рассылка события.
		/// </summary>
		public TimeSpan TotalElapsed
		{
			get;
			private set;
		}
	}
}
