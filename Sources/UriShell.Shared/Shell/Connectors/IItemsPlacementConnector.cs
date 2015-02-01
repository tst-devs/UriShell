using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Присоединяет объекты к пользовательскому интерфейсу в виде списка их представлений.
	/// </summary>
	[ContractClass(typeof(IItemsPlacementConnectorContract))]
	public interface IItemsPlacementConnector : IUriPlacementConnector
	{
		/// <summary>
		/// Возвращает <see cref="ICollectionView"/> коллекции представлений
		/// присоединенных объектов.
		/// </summary>
		ICollectionView Views
		{
			get;
		}

		/// <summary>
		/// Возвращает список присоединенных объектов.
		/// </summary>
		IEnumerable<object> Connected
		{
			get;
		}
		
		/// <summary>
		/// Возвращает или присваивает объект, представление которого активно.
		/// </summary>
		object Active
		{
			get;
			set;
		}

		/// <summary>
		/// Возвращает значение, указывающее, что нет ни одного присоединенного объекта.
		/// </summary>
		bool IsPlacementEmpty
		{
			get;
		}

		/// <summary>
		/// Возвращает присоединенный объект, представленный заданным.
		/// </summary>
		/// <param name="view">Представление искомого объекта.</param>
		/// <returns>Объект, представленный заданным.</returns>
		object ConnectedFromView(object view);

		/// <summary>
		/// Меняет индекс присоединенного объекта на заданный.
		/// </summary>
		/// <param name="connected">Присоединенный объект, индекс которого нужно изменить.</param>
		/// <param name="newIndex">Новый индекс присоединенного объекта в <see cref="Connected"/>.</param>
		void MoveConnected(object connected, int newIndex);

		/// <summary>
		/// Вызывается при присоединении или отсоединении объекта. 
		/// </summary>
		event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;

		/// <summary>
		/// Вызывается при смене активного представления.
		/// </summary>
		event EventHandler<ActiveChangedEventArgs> ActiveChanged;
	}
}