using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;

using UriShell.Shell.Resolution;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Реализует сервис перетаскивания объектов, присоединенных к пользовательскому интерфейсу.
	/// </summary>
	internal sealed class ConnectedDragDrop : IConnectedDragDrop, IUriPlacementConnector
	{
		/// <summary>
		/// Таблица отсоединения объектов от пользовательского интерфейса.
		/// </summary>
		private readonly IUriDisconnectTable _uriDisconnectTable;

		/// <summary>
		/// Словарь для хранения данных на время перетаскивания.
		/// </summary>
		private readonly HybridDictionary _data = new HybridDictionary();

		/// <summary>
		/// Объект, который перетаскивается.
		/// </summary>
		private object _connected;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ConnectedDragDrop"/>.
		/// </summary>
		/// <param name="uriDisconnectTable">Таблица отсоединения объектов от пользовательского интерфейса.</param>
		public ConnectedDragDrop(IUriDisconnectTable uriDisconnectTable)
		{
			Contract.Requires<ArgumentNullException>(uriDisconnectTable != null);

			this._uriDisconnectTable = uriDisconnectTable;
		}

		/// <summary>
		/// Возвращает значение, указывающее, что перетаскивание находится в процессе.
		/// </summary>
		public bool IsActive
		{
			get
			{
				return this._connected != null;
			}
		}

		/// <summary>
		/// Начинает перетаскивание заданного объекта, присоединенного к
		/// пользовательскому интерфейсу.
		/// </summary>
		/// <param name="connected">Объект, присоединенный к пользовательскому
		/// интерфейсу, для перетаскивания.</param>
		public void Drag(object connected)
		{
			this._connected = connected;
			
			this._uriDisconnectTable[connected].Disconnect(connected);
			this._uriDisconnectTable[connected] = this;
		}

		/// <summary>
		/// Завершает перетаскивание присоединением объекта к заданному коннектору.
		/// </summary>
		/// <param name="target">Коннектор для присоединения перетаскиваемого объекта.</param>
		public void Drop(IUriPlacementConnector target)
		{
			var dragged = this._connected;

			target.Connect(dragged);
			this._uriDisconnectTable[dragged] = target;

			this._connected = null;
			this._data.Clear();
		}

		/// <summary>
		/// Проверяет, перетаскивается ли заданный объект.
		/// </summary>
		/// <param name="resolved">Объект, проверяемый на перетаскивание.</param>
		/// <returns>true, если объект перетаскивается; иначе false.</returns>
		public bool IsDragging(object resolved)
		{
			return resolved == this._connected;
		}

		/// <summary>
		/// Сохраняет указанные данные на время перетаскивания.
		/// </summary>
		/// <typeparam name="TFormat">Тип, определяющий формат данных.</typeparam>
		/// <param name="key">Ключ сохраняемых данных.</param>
		/// <param name="data">Данные для сохранения на время перетаскивания.</param>
		public void SetData<TFormat>(ConnectedDragDropKey<TFormat> key, TFormat data)
		{
			this._data[key] = data;
		}

		/// <summary>
		/// Возвращает данные, сохраненные на время перетаскивания.
		/// </summary>
		/// <typeparam name="TFormat">Тип, определяющий формат данных.</typeparam>
		/// <param name="key">Ключ для поиска данных.</param>
		/// <returns>Запрошенные данные или значение типа <typeparamref name="TFormat"/> по умолчанию,
		/// если данные не найдены.</returns>
		public TFormat GetData<TFormat>(ConnectedDragDropKey<TFormat> key)
		{
			if (this._data.Contains(key))
			{
				return (TFormat)this._data[key];
			}

			return default(TFormat);
		}

		/// <summary>
		/// Проверяет наличие данных, сохраненных на время перетаскивания.
		/// </summary>
		/// <typeparam name="TFormat">Тип, определяющий формат данных.</typeparam>
		/// <param name="key">Ключ для поиска данных.</param>
		/// <returns>true, если данные есть; иначе false.</returns>
		public bool GetDataPresent<TFormat>(ConnectedDragDropKey<TFormat> key)
		{
			return this._data.Contains(key);
		}

		/// <summary>
		/// Присоединяет заданный объект к пользовательскому интерфейсу.
		/// </summary>
		/// <param name="resolved">Объект для присоединения к UI.</param>
		void IUriPlacementConnector.Connect(object resolved)
		{
			// Никто не должен явно использовать ConnectorDragDrop как коннектор.
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Отсоединяет заданный объект от пользовательского интерфейса.
		/// </summary>
		/// <param name="resolved">Объект для отсоединения от UI.</param>
		void IUriPlacementConnector.Disconnect(object resolved)
		{
			this.OnDraggedClosed(EventArgs.Empty);

			// Вызов Disconnect означает, что оболочка закрывает объект
			// в процессе перетаскивания. В этом случае объектом владеет
			// ConnectedDragDrop и отвечает за высвобождение связанных данных.

			foreach (var disposable in this._data.Values.OfType<IDisposable>())
			{
				disposable.Dispose();
			}

			this._connected = null;
			this._data.Clear();
		}

		/// <summary>
		/// Возвращает признак того, что данный коннектор сам отвечает за 
		/// обновление данных в присоединенных объектах.
		/// </summary>
		bool IUriPlacementConnector.IsResponsibleForRefresh
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Вызывает событие <see cref="DraggedClosed"/>.
		/// </summary>
		/// <param name="e">Объект, содержащий аргументы события.</param>
		private void OnDraggedClosed(EventArgs e)
		{
			var draggedClosed = this.DraggedClosed;
			if (draggedClosed != null)
			{
				draggedClosed(this, e);
			}
		}

		/// <summary>
		/// Вызывается при закрытии оболочкой перетаскиваемого объекта.
		/// </summary>
		public event EventHandler DraggedClosed;
	}
}