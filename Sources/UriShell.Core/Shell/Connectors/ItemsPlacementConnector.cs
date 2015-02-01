using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Присоединяет объекты к пользовательскому интерфейсу в виде списка их представлений.
	/// </summary>
    public sealed partial class ItemsPlacementConnector : ItemsPlacementConnectorBase
	{
		/// <summary>
		/// Сервис для поиска представлений присоединенных объектов.
		/// </summary>
		private readonly IViewModelViewMatcher _viewModelViewMatcher;

		/// <summary>
		/// Сервис перетаскивания объектов, присоединенных к пользовательскому интерфейсу.
		/// </summary>
		private readonly IConnectedDragDrop _connectedDragDrop;
		
		/// <summary>
		/// Инициализирует новый объект класса <see cref="ItemsPlacementConnector"/>.
		/// </summary>
		/// <param name="viewModelViewMatcher">Сервис для поиска представлений присоединенных объектов.</param>
		/// <param name="connectedDragDrop">Сервис перетаскивания объектов, присоединенных к пользовательскому интерфейсу.</param>
		public ItemsPlacementConnector(
			IViewModelViewMatcher viewModelViewMatcher,
			IConnectedDragDrop connectedDragDrop)
			: 
			this(viewModelViewMatcher, connectedDragDrop, ItemsPlacementConnectorFlags.Default)
		{
		}

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ItemsPlacementConnector"/>.
		/// </summary>
		/// <param name="viewModelViewMatcher">Сервис для поиска представлений присоединенных объектов.</param>
		/// <param name="connectedDragDrop">Сервис перетаскивания объектов, присоединенных к пользовательскому интерфейсу.</param>
		/// <param name="flags">Настроечные флаги создаваемого <see cref="ItemsPlacementConnector"/>.</param>
		public ItemsPlacementConnector(
			IViewModelViewMatcher viewModelViewMatcher,
			IConnectedDragDrop connectedDragDrop,
			ItemsPlacementConnectorFlags flags)
			: 
			base(flags)
		{
			Contract.Requires<ArgumentNullException>(viewModelViewMatcher != null);
			Contract.Requires<ArgumentNullException>(connectedDragDrop != null);

			this._viewModelViewMatcher = viewModelViewMatcher;
			this._connectedDragDrop = connectedDragDrop;
		}

		/// <summary>
		/// Присоединяет заданный объект к пользовательскому интерфейсу.
		/// </summary>
		/// <param name="resolved">Объект для присоединения к UI.</param>
		public override void Connect(object resolved)
		{
			using (var changeRec = this.BeginChange())
			{
				// Определяем представление присоединяемого объекта.
				// Оно либо должно быть найдено через matcher, либо
				// может поступить вместе с перетаскиванием.
				object view;
				if (this._connectedDragDrop.IsDragging(resolved))
				{
					view = this._connectedDragDrop.GetData(ConnectedDragDropKeys.UriConnectedView);
				}
				else
				{
					var viewMatch = this._viewModelViewMatcher.Match(resolved);
					view = viewMatch != null ? viewMatch.View : resolved;
				}				
				
				this.Connected.Add(resolved);
				this.Views.Add(view);
				changeRec.Connected(resolved);

				// Если указано, делаем присоединенный объект активным.
				if (this.IsFlagSet(ItemsPlacementConnectorFlags.ActivateOnConnect))
				{
					changeRec.NewActive = resolved;
				}
			}
		}

		/// <summary>
		/// Отсоединяет заданный объект от пользовательского интерфейса.
		/// </summary>
		/// <param name="resolved">Объект для отсоединения от UI.</param>
		public override void Disconnect(object resolved)
		{
			using (var changeRec = this.BeginChange())
			{
				var index = this.Connected.IndexOf(resolved);

				// Выбираем объект, который станет активным
				// после отсоединения заданного.
				if (resolved == this.Active)
				{
					if (index == this.Connected.Count - 1)
					{
						changeRec.NewActive = index > 0 ? this.Connected[index - 1] : null;
					}
					else
					{
						changeRec.NewActive = this.Connected[index + 1];
					}
				}

				// Решаем, как поступить с представлением. Если объект отсоединяют
				// с целью перетаскивания, нужно сохранить его в соответствующем
				// сервисе. Если нет, нужно обеспечить высвобождение его ресурсов.
				IDisposable disposableView = null;
				if (this._connectedDragDrop.IsDragging(resolved))
				{
					this._connectedDragDrop.SetData(
						ConnectedDragDropKeys.UriConnectedView, this.Views[index]);
				}
				else
				{
					disposableView = this.Views[index] as IDisposable;
				}

				this.Views.RemoveAt(index);
				this.Connected.RemoveAt(index);

				if (disposableView != null)
				{
					disposableView.Dispose();
				}

				changeRec.Disconnected(resolved);
			}
		}

		/// <summary>
		/// Меняет индекс присоединенного объекта на заданный.
		/// </summary>
		/// <param name="connected">Присоединенный объект, индекс которого нужно изменить.</param>
		/// <param name="newIndex">Новый индекс присоединенного объекта в <see cref="IItemsPlacementConnector.Connected"/>.</param>
		public override void MoveConnected(object connected, int newIndex)
		{
			var oldIndex = this.Connected.IndexOf(connected);
			if (oldIndex == newIndex)
			{
				return;
			}

			using (var changeRec = this.BeginChange())
			{
				this.Connected.RemoveAt(oldIndex);
				this.Connected.Insert(newIndex, connected);
				this.Views.Move(oldIndex, newIndex);

				changeRec.Moved(connected);
			}
		}
	}
}