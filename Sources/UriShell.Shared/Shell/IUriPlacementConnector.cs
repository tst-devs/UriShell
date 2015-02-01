using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UriShell.Shell
{
	/// <summary>
	/// Интерфейс объекта, присоединяющего объект к пользовательскому интерфейсу.
	/// </summary>
	[ContractClass(typeof(IUriPlacementConnectorContract))]
	public interface IUriPlacementConnector
	{
		/// <summary>
		/// Присоединяет заданный объект к пользовательскому интерфейсу.
		/// </summary>
		/// <param name="resolved">Объект для присоединения к UI.</param>
		void Connect(object resolved);

		/// <summary>
		/// Отсоединяет заданный объект от пользовательского интерфейса.
		/// </summary>
		/// <param name="resolved">Объект для отсоединения от UI.</param>
		void Disconnect(object resolved);

		/// <summary>
		/// Возвращает признак того, что данный коннектор сам отвечает за 
		/// обновление данных в присоединенных объектах.
		/// </summary>
		bool IsResponsibleForRefresh
		{
			get;
		}
	}
}
