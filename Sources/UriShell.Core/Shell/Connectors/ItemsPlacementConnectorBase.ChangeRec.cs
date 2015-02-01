using System;
using System.Collections.Generic;
using System.Linq;

namespace UriShell.Shell.Connectors
{
	partial class ItemsPlacementConnectorBase
	{
		/// <summary>
		/// Обеспечивает реакцию на изменение состояния коннектора.
		/// </summary>
		protected sealed class ChangeRec : IDisposable
		{
			/// <summary>
			/// Коннектор, состояние изменяется.
			/// </summary>
			private readonly ItemsPlacementConnectorBase _connector;

			/// <summary>
			/// Список изменений содержимого коннектора.
			/// </summary>
			private List<ConnectedChangedEventArgs> _connectionChanges;

			/// <summary>
			/// Инициализирует новый объект класса <see cref="ChangeRec"/>.
			/// </summary>
			/// <param name="connector">Коннектор, состояние которого будет изменено.</param>
			public ChangeRec(ItemsPlacementConnectorBase connector)
			{
				this._connector = connector;

				this.OldActive = connector.Active;
				this.NewActive = connector.Active;
			}

			/// <summary>
			/// Вызывается при необходимости освободить ресурсы, занятые объектом.
			/// </summary>
			public void Dispose()
			{
				this._connector.EndChange(this);
			}

			/// <summary>
			/// Добавляет изменение содержимого коннектора.
			/// </summary>
			/// <param name="change">Аргументы, описывающие изменение.</param>
			private void AddConnectionChange(ConnectedChangedEventArgs change)
			{
				if (this._connectionChanges == null)
				{
					this._connectionChanges = new List<ConnectedChangedEventArgs>();
				}

				this._connectionChanges.Add(change);
			}

			/// <summary>
			/// Возвращает объект, представление которого было активно до начала изменения.
			/// </summary>
			public object OldActive
			{
				get;
				private set;
			}

			/// <summary>
			/// Возвращает или присваивает объект, представление которого должно
			/// стать активным после изменения.
			/// </summary>
			public object NewActive
			{
				get;
				set;
			}

			/// <summary>
			/// Записывает присоединение объекта.
			/// </summary>
			/// <param name="connected">Объект, который был присоединен.</param>
			public void Connected(object connected)
			{
				this.AddConnectionChange(
					new ConnectedChangedEventArgs(ConnectedChangedAction.Connect, connected));
			}

			/// <summary>
			/// Записывает отсоединение объекта.
			/// </summary>
			/// <param name="connected">Объект, который был отсоединен.</param>
			public void Disconnected(object connected)
			{
				this.AddConnectionChange(
					new ConnectedChangedEventArgs(ConnectedChangedAction.Disconnect, connected));
			}

			/// <summary>
			/// Записывает изменение индекса объекта.
			/// </summary>
			/// <param name="connected">Объект, у которого изменился индекс.</param>
			public void Moved(object connected)
			{
				this.AddConnectionChange(
					new ConnectedChangedEventArgs(ConnectedChangedAction.Move, connected));
			}

			/// <summary>
			/// Возвращает список изменений содержимого коннектора.
			/// </summary>
			public IEnumerable<ConnectedChangedEventArgs> ConnectedChanges
			{
				get
				{
					return this._connectionChanges ?? Enumerable.Empty<ConnectedChangedEventArgs>();
				}
			}
		}
	}
}
