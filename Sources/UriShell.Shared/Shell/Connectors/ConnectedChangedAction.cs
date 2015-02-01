using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Задает список действий, вызывающих событие <see cref="IItemsPlacementConnector.ConnectedChanged"/>.
	/// </summary>
	public enum ConnectedChangedAction
	{
		/// <summary>
		/// Объект был присоединен к пользовательскому интерфейсу.
		/// </summary>
		Connect,

		/// <summary>
		/// Объект был отсоединен от пользовательского интерфейса.
		/// </summary>
		Disconnect,

		/// <summary>
		/// Объект изменил свой индекс в пользовательском интерфейсе.
		/// </summary>
		Move,
	}
}
