using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Список флагов настройки <see cref="IItemsPlacementConnector"/> при его создании.
	/// </summary>
	[Flags]
	public enum ItemsPlacementConnectorFlags
	{
		/// <summary>
		/// Настройки по умолчанию.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Указывает, что свойство <see cref="IUriPlacementConnector.IsResponsibleForRefresh"/>
		/// должно возвращать значение true.
		/// </summary>
		IsResponsibleForRefresh = 1,

		/// <summary>
		/// При присоединении объекта, коннектор должен делать его представление активным.
		/// </summary>
		ActivateOnConnect = 2,
	}
}
