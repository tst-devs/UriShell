using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Реализует базовый функционал <see cref="IItemsPlacementConnector"/>.
	/// </summary>
    public abstract partial class ItemsPlacementConnectorBase : IItemsPlacementConnector
	{
		/// <summary>
		/// Настроечные флаги данного <see cref="ItemsPlacementConnector"/>.
		/// </summary>
		private readonly ItemsPlacementConnectorFlags _flags;

		/// <summary>
		/// Список присоединенных объектов.
		/// </summary>
		private readonly List<object> _connected = new List<object>();

		/// <summary>
		/// Коллекция представлений присоединенных объектов.
		/// </summary>
		private readonly ObservableCollection<object> _views = new ObservableCollection<object>();

		/// <summary>
		/// <see cref="ICollectionView"/> коллекции представлений присоединенных объектов.
		/// </summary>
		private readonly ICollectionView _viewsCollectionView;

		/// <summary>
		/// Указывает, что идет изменение коннектора, начатое вызовом <see cref="BeginChange"/>.
		/// </summary>
		private bool _isChanging;
				
		/// <summary>
		/// Индекс объекта, представление которого активно.
		/// </summary>
		private int _activeIndex = -1;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ItemsPlacementConnectorBase"/>.
		/// </summary>
		/// <param name="flags">Настроечные флаги создаваемого <see cref="ItemsPlacementConnector"/>.</param>
		public ItemsPlacementConnectorBase(ItemsPlacementConnectorFlags flags)
		{
			this._flags = flags;

			this._viewsCollectionView = new ListCollectionView(this._views);
			this._viewsCollectionView.CurrentChanged += this.ViewsCollectionView_CurrentChanged;
		}

		/// <summary>
		/// Проверяет, установлен ли заданный настроечный флаг.
		/// </summary>
		/// <param name="flag">Проверяемый настроечный флаг.</param>
		/// <returns>true, если заданный флаг установлен; иначе false.</returns>
		protected bool IsFlagSet(ItemsPlacementConnectorFlags flag)
		{
			return (this._flags & flag) == flag;
		}

		/// <summary>
		/// Возвращает список присоединенных объектов.
		/// </summary>
		protected IList<object> Connected
		{
			get
			{
				return this._connected;
			}
		}

		/// <summary>
		/// Возвращает коллекцию представлений присоединенных объектов.
		/// </summary>
		protected ObservableCollection<object> Views
		{
			get
			{
				return this._views;
			}
		}

		#region Explicit IItemsPlacementConnector Members

		/// <summary>
		/// Возвращает список присоединенных объектов.
		/// </summary>
		IEnumerable<object> IItemsPlacementConnector.Connected
		{
			get
			{
				return this._connected;
			}
		}

		/// <summary>
		/// Возвращает <see cref="ICollectionView"/> коллекции представлений
		/// присоединенных объектов.
		/// </summary>
		ICollectionView IItemsPlacementConnector.Views
		{
			get
			{
				return this._viewsCollectionView;
			}
		}

		#endregion

		/// <summary>
		/// Присоединяет заданный объект к пользовательскому интерфейсу.
		/// </summary>
		/// <param name="resolved">Объект для присоединения к UI.</param>
		public abstract void Connect(object resolved);

		/// <summary>
		/// Отсоединяет заданный объект от пользовательского интерфейса.
		/// </summary>
		/// <param name="resolved">Объект для отсоединения от UI.</param>
		public abstract void Disconnect(object resolved);
		
		/// <summary>
		/// Возвращает присоединенный объект, представленный заданным.
		/// </summary>
		/// <param name="view">Представление искомого объекта.</param>
		/// <returns>Объект, представленный заданным.</returns>
		public object ConnectedFromView(object view)
		{
			var index = this._views.IndexOf(view);
			return this._connected[index];
		}

		/// <summary>
		/// Меняет индекс присоединенного объекта на заданный.
		/// </summary>
		/// <param name="connected">Присоединенный объект, индекс которого нужно изменить.</param>
		/// <param name="newIndex">Новый индекс присоединенного объекта в <see cref="Connected"/>.</param>
		public abstract void MoveConnected(object connected, int newIndex);

		/// <summary>
		/// Отмечает блок начала изменения состояния коннектора.
		/// </summary>
		/// <returns>Объект, позволяющий описать и среагировать на изменения.</returns>
		protected ChangeRec BeginChange()
		{
			if (this._isChanging)
			{
				throw new InvalidOperationException(string.Format(
					Properties.Resources.UnableToBeginChangePlacementConnector,
					this.GetType().Name));
			}

			this._isChanging = true;

			return new ChangeRec(this);
		}

		/// <summary>
		/// Вызывается по окончании блока изменения состояния коннектора.
		/// </summary>
		/// <param name="changeRec">Объект, содержащий сведения об изменении.</param>
		private void EndChange(ChangeRec changeRec)
		{
			try
			{
				this._activeIndex = this._connected.IndexOf(changeRec.NewActive);
				
				// При присоединении и перемещении объектов, вызываем ConnectedChanged до ActiveChanged.
				foreach (var change in changeRec.ConnectedChanges)
				{
					if (change.Action == ConnectedChangedAction.Connect
						|| change.Action == ConnectedChangedAction.Move)
					{
						this.OnConnectedChanged(change);
					}
				}

				if (changeRec.OldActive != changeRec.NewActive)
				{
					this.OnActiveChanged(new ActiveChangedEventArgs(changeRec.OldActive, changeRec.NewActive));
				}

				// При отсоединении объекта, вызываем ConnectedChanged после ActiveChanged.
				foreach (var change in changeRec.ConnectedChanges)
				{
					if (change.Action == ConnectedChangedAction.Disconnect)
					{
						this.OnConnectedChanged(change);
					}
				}

				// Синхронизируем выбранный элемент в списке представлений.
				this._viewsCollectionView.MoveCurrentToPosition(this._activeIndex);
			}
			finally
			{
				this._isChanging = false;
			}
		}

		/// <summary>
		/// Обрабатывает изменение текущего элемента во <see cref="Views"/>.
		/// </summary>
		/// <param name="sender">Объект, к которому прикреплен обработчик события.</param>
		/// <param name="e">Объект, содержащий аргументы события.</param>
		private void ViewsCollectionView_CurrentChanged(object sender, EventArgs e)
		{
			// Игнорируем оповещения в процессе изменения состояния.
			if (this._isChanging)
			{
				return;
			}

			if (this._viewsCollectionView.CurrentItem == null)
			{
				this.Active = null;
			}
			else
			{
				this.Active = this.ConnectedFromView(this._viewsCollectionView.CurrentItem);
			}
		}

		/// <summary>
		/// Вызывает событие <see cref="ConnectedChanged"/>.
		/// </summary>
		/// <param name="e">Объект, содержащий аргументы события.</param>
		private void OnConnectedChanged(ConnectedChangedEventArgs e)
		{
			var connectedChanged = this.ConnectedChanged;
			if (connectedChanged != null)
			{
				connectedChanged(this, e);
			}
		}

		/// <summary>
		/// Вызывает событие <see cref="ActiveChanged"/>.
		/// </summary>
		/// <param name="e">Объект, содержащий аргументы события.</param>
		private void OnActiveChanged(ActiveChangedEventArgs e)
		{
			var activeChanged = this.ActiveChanged;
			if (activeChanged != null)
			{
				activeChanged(this, e);
			}
		}

		/// <summary>
		/// Возвращает признак того, что данный коннектор сам отвечает за 
		/// обновление данных в присоединенных объектах.
		/// </summary>
		public bool IsResponsibleForRefresh
		{
			get
			{
				return this.IsFlagSet(ItemsPlacementConnectorFlags.IsResponsibleForRefresh);
			}
		}

		/// <summary>
		/// Возвращает или присваивает объект, представление которого активно.
		/// </summary>
		public object Active
		{
			get
			{
				if (this._activeIndex == -1)
				{
					return null;
				}

				return this._connected[this._activeIndex];
			}
			set
			{
				if (value == this.Active)
				{
					return;
				}

				using (var changeRec = this.BeginChange())
				{
					changeRec.NewActive = value;
				}
			}
		}

		/// <summary>
		/// Возвращает значение, указывающее, что нет ни одного присоединенного объекта.
		/// </summary>
		public bool IsPlacementEmpty
		{
			get
			{
				return this._connected.Count == 0;
			}
		}
		
		/// <summary>
		/// Вызывается при присоединении или отсоединении объекта. 
		/// </summary>
		public event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;

		/// <summary>
		/// Вызывается при смене активного представления.
		/// </summary>
		public event EventHandler<ActiveChangedEventArgs> ActiveChanged;
	}
}