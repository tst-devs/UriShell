using System;

namespace UriShell.Samples.TabApp.Input
{
	/// <summary>
	/// Вспомогательный класс для работы с командами, производными от <see cref="ViewModelCommandBase"/>.
	/// </summary>
	public static class ViewModelCommandHelper
	{
		/// <summary>
		/// Инициирует проверку возможности выполнения перечисленных команд.
		/// </summary>
		/// <param name="command1">Первая команда для проверки.</param>
		/// <param name="commandsN">Список команд, которые следует проверить, наряду с первой.</param>
		public static void Invalidate(ViewModelCommandBase command1, params ViewModelCommandBase[] commandsN)
		{
			if (command1 != null)
			{
				command1.Invalidate();
			}

			foreach (var commandN in commandsN)
			{
				if (commandN != null)
				{
					commandN.Invalidate();
				}
			}
		}
	}
}