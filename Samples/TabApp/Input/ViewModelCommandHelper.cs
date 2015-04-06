using System;

namespace UriShell.Samples.TabApp.Input
{
	public static class ViewModelCommandHelper
	{
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