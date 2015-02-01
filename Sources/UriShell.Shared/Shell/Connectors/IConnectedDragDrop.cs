using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Сервис перетаскивания объектов, присоединенных к пользовательскому интерфейсу.
	/// </summary>
	[ContractClass(typeof(IConnectedDragDropContract))]
	public interface IConnectedDragDrop
	{
		/// <summary>
		/// Возвращает значение, указывающее, что перетаскивание находится в процессе.
		/// </summary>
		[Pure]
		bool IsActive
		{
			get;
		}

		/// <summary>
		/// Начинает перетаскивание заданного объекта, присоединенного к
		/// пользовательскому интерфейсу.
		/// </summary>
		/// <param name="connected">Объект, присоединенный к пользовательскому
		/// интерфейсу, для перетаскивания.</param>
		void Drag(object connected);

		/// <summary>
		/// Завершает перетаскивание присоединением объекта к заданному коннектору.
		/// </summary>
		/// <param name="target">Коннектор для присоединения перетаскиваемого объекта.</param>
		void Drop(IUriPlacementConnector target);

		/// <summary>
		/// Проверяет, перетаскивается ли заданный объект.
		/// </summary>
		/// <param name="resolved">Объект, проверяемый на перетаскивание.</param>
		/// <returns>true, если объект перетаскивается; иначе false.</returns>
		[Pure]
		bool IsDragging(object resolved);

		/// <summary>
		/// Сохраняет указанные данные на время перетаскивания.
		/// </summary>
		/// <typeparam name="TFormat">Тип, определяющий формат данных.</typeparam>
		/// <param name="key">Ключ сохраняемых данных.</param>
		/// <param name="data">Данные для сохранения на время перетаскивания.</param>
		void SetData<TFormat>(ConnectedDragDropKey<TFormat> key, TFormat data);

		/// <summary>
		/// Возвращает данные, сохраненные на время перетаскивания.
		/// </summary>
		/// <typeparam name="TFormat">Тип, определяющий формат данных.</typeparam>
		/// <param name="key">Ключ для поиска данных.</param>
		/// <returns>Запрошенные данные или значение типа <typeparamref name="TFormat"/> по умолчанию,
		/// если данные не найдены.</returns>
		[Pure]
		TFormat GetData<TFormat>(ConnectedDragDropKey<TFormat> key);

		/// <summary>
		/// Проверяет наличие данных, сохраненных на время перетаскивания.
		/// </summary>
		/// <typeparam name="TFormat">Тип, определяющий формат данных.</typeparam>
		/// <param name="key">Ключ для поиска данных.</param>
		/// <returns>true, если данные есть; иначе false.</returns>
		[Pure]
		bool GetDataPresent<TFormat>(ConnectedDragDropKey<TFormat> key);

		/// <summary>
		/// Вызывается при закрытии оболочкой перетаскиваемого объекта.
		/// </summary>
		event EventHandler DraggedClosed;
	}
}