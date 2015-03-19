using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Concurrency;

namespace UriShell.Shell.Connectors
{
	using KeyedViewModelViewMatch = KeyValuePair<object, IViewModelViewMatch>;
	
	/// <summary>
	/// Присоединяет объекты к пользовательскому интерфейсу SidebarView,
	/// используя <see cref="IScheduler"/> для разделения наборов объектов.
	/// </summary>
	[Obsolete("Not sure if this class should be common")]
    public sealed class SidebarPlacementConnector : ItemsPlacementConnectorBase, ISidebarPlacementConnector
	{
		/// <summary>
		/// <see cref="IScheduler"/> для вызова обновления коннектора.
		/// </summary>
		private readonly IScheduler _scheduler;

		/// <summary>
		/// Предоставляет ключи, по которым определяется идентичность присоединенных объектов.
		/// </summary>
		private readonly ISidebarPlacementKeySource _keySource;
		
		/// <summary>
		/// Сервис для поиска представлений присоединенных объектов.
		/// </summary>
		private readonly IViewModelViewMatcher _viewModelViewMatcher;

		/// <summary>
		/// Список объектов, накапливаемый при вызове <see cref="IUriPlacementConnector.Connect"/>.
		/// </summary>
		private readonly List<object> _connectQueue = new List<object>();

		/// <summary>
		/// Список <see cref="IViewModelViewMatch"/> представлений, которые поддерживают
		/// смену своей модели, каждое из которых связано с ключом модели, генерируемом
		/// при помощи <see cref="ISidebarPlacementKeySource"/>.
		/// </summary>
		private readonly List<KeyedViewModelViewMatch> _viewMatches = new List<KeyedViewModelViewMatch>();

		/// <summary>
		/// История активности присоединенных объектов, в которой ключом
		/// выступает идентификатор <see cref="SidebarActivityHistoryId"/>,
		/// а значением - ключ записанного активного объекта.
		/// </summary>
		private readonly Dictionary<SidebarActivityHistoryId, object> _activityHistory = new Dictionary<SidebarActivityHistoryId, object>();

		/// <summary>
		/// Ключи присоединенных объектов, доступные по этим объектам,
		/// используемые для сохранения истории активности.
		/// </summary>
		private readonly Dictionary<object, object> _activityKeys = new Dictionary<object, object>();

		/// <summary>
		/// Сервис перетаскивания объектов, присоединенных к пользовательскому интерфейсу.
		/// </summary>
		private readonly IConnectedDragDrop _connectedDragDrop;

		/// <summary>
		/// Отложенный вызов <see cref="Update"/>.
		/// </summary>
		private IDisposable _scheduledUpdate;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="SidebarPlacementConnector"/>.
		/// </summary>
		/// <param name="scheduler"><see cref="IScheduler"/> для вызова обновления коннектора.</param>
		/// <param name="viewModelViewMatcher">Сервис для поиска представлений присоединенных объектов.</param>
		/// <param name="connectedDragDrop">Сервис перетаскивания объектов, присоединенных к пользовательскому интерфейсу.</param>
		/// <param name="keySource">Объект, предоставляющий ключи, по которым определяется идентичность
		/// объектов, показываемых в SidebarView.</param>
		public SidebarPlacementConnector(
			IScheduler scheduler,
			IViewModelViewMatcher viewModelViewMatcher,
			IConnectedDragDrop connectedDragDrop,
			ISidebarPlacementKeySource keySource)
			:
			this(
				scheduler,
				viewModelViewMatcher,
				connectedDragDrop,
				keySource,
				ItemsPlacementConnectorFlags.IsResponsibleForRefresh)
		{
		}

		/// <summary>
		/// Инициализирует новый объект класса <see cref="SidebarPlacementConnector"/>.
		/// </summary>
		/// <param name="scheduler"><see cref="IScheduler"/> для вызова обновления коннектора.</param>
		/// <param name="viewModelViewMatcher">Сервис для поиска представлений присоединенных объектов.</param>
		/// <param name="connectedDragDrop">Коннектор объектов при их перетаскивании.</param>
		/// <param name="keySource">Объект, предоставляющий ключи, по которым определяется идентичность
		/// объектов, показываемых в SidebarView.</param>
		/// <param name="flags">Настроечные флаги создаваемого <see cref="SidebarPlacementConnector"/>.</param>
		public SidebarPlacementConnector(
			IScheduler scheduler,
			IViewModelViewMatcher viewModelViewMatcher,
			IConnectedDragDrop connectedDragDrop,
			ISidebarPlacementKeySource keySource,
			ItemsPlacementConnectorFlags flags)
			:
			base(flags)
		{
			Contract.Requires<ArgumentNullException>(scheduler != null);
			Contract.Requires<ArgumentNullException>(connectedDragDrop != null);
			Contract.Requires<ArgumentNullException>(keySource != null);
			Contract.Requires<ArgumentNullException>(viewModelViewMatcher != null);

			this._scheduler = scheduler;
			this._keySource = keySource;
			this._viewModelViewMatcher = viewModelViewMatcher;
			this._connectedDragDrop = connectedDragDrop;
		}

		/// <summary>
		/// Вызывается при необходимости освободить ресурсы, занятые объектом.
		/// </summary>
		public void Dispose()
		{
			foreach (var keyedMatch in this._viewMatches)
			{
				var disposableView = keyedMatch.Value.View as IDisposable;
				if (disposableView != null)
				{
					disposableView.Dispose();
				}
			}
		}

		/// <summary>
		/// Присоединяет заданный объект к пользовательскому интерфейсу.
		/// </summary>
		/// <param name="resolved">Объект для присоединения к UI.</param>
		public override void Connect(object resolved)
		{
			this.CheckDragDropNotSupported(resolved);

			this._connectQueue.Add(resolved);
			this.ScheduleUpdate();
		}

		/// <summary>
		/// Отсоединяет заданный объект от пользовательского интерфейса.
		/// </summary>
		/// <param name="resolved">Объект для отсоединения от UI.</param>
		public override void Disconnect(object resolved)
		{
			this.CheckDragDropNotSupported(resolved);

			this._connectQueue.Remove(resolved);
			this.ScheduleUpdate();
		}

		/// <summary>
		/// Меняет индекс присоединенного объекта на заданный.
		/// </summary>
		/// <param name="connected">Присоединенный объект, индекс которого нужно изменить.</param>
		/// <param name="newIndex">Новый индекс присоединенного объекта в <see cref="IItemsPlacementConnector.Connected"/>.</param>
		public override void MoveConnected(object connected, int newIndex)
		{
			throw new NotSupportedException(string.Format(
				Properties.Resources.SidebarPlacementConnectorDoesntSupportMoveConnected,
				this.GetType().Name));
		}

		/// <summary>
		/// Размещает отложенный вызов <see cref="Update"/> через <see cref="IScheduler"/>.
		/// </summary>
		private void ScheduleUpdate()
		{
			if (this._scheduledUpdate != null)
			{
				this._scheduledUpdate.Dispose();
			}

			this._scheduledUpdate = this._scheduler.Schedule(this.Update);
		}

		/// <summary>
		/// Выполняет обновление пользовательского интерфейса, согласно очереди
		/// присоединенных объектов.
		/// </summary>
		private void Update()
		{
			var connectQueue = this._connectQueue.ToArray();
			this._connectQueue.Clear();

			using (var changeRec = this.BeginChange())
			{
				this.RecordActivity();

				// Отсоединяем все имеющиеся объекты.
				for (int i = this.Connected.Count - 1; i >= 0; i--)
				{
					var disconnected = this.Connected[i];
					this.Connected.RemoveAt(i);
					changeRec.Disconnected(disconnected);
				}

				// Убираем все представления, которые не подходят для
				// нового набора присоединенных объектов.
				var connectedViewMatches = this.MatchSavedViewsToConnected(connectQueue);
				for (int i = this.Views.Count - 1; i >= 0; i--)
				{
					var view = this.Views[i];
					if (connectedViewMatches.Any(km => km.Value.View == view))
					{
						continue;
					}

					this.Views.RemoveAt(i);

					// Заботимся об освобождении ресурсов в представлениях,
					// которые не поддерживают изменение своей модели.

					var disposableView = view as IDisposable;
					if (disposableView != null && !this._viewMatches.Any(km => km.Value.View == view))
					{
						disposableView.Dispose();
					}
				}

				// Присоединяем новые объекты. Сохраненным представлениям
				// меняем модель, а если подходящего нет, инициируем поиск
				// нового представления для присоединенного объекта.
				for (int i = 0; i < connectQueue.Length; i++)
				{
					var connected = connectQueue[i];
					this.Connected.Add(connected);

					IViewModelViewMatch viewMatch;
					if (connectedViewMatches.TryGetValue(connected, out viewMatch))
					{
						viewMatch.ChangeModel(connected);

						var viewIndex = this.Views.IndexOf(viewMatch.View);
						if (viewIndex == -1)
						{
							this.Views.Insert(i, viewMatch.View);
						}
						else if (viewIndex != i)
						{
							// Учитываем возможное изменение местоположения представления.
							this.Views.Move(viewIndex, i);
						}
					}
					else
					{
						viewMatch = this.MatchNewViewToConnected(connected);
						this.Views.Insert(i, viewMatch == null ? connected : viewMatch.View);
					}

					changeRec.Connected(connected);
				}

				changeRec.NewActive = this.RestoreActivity();
			}
		}

		/// <summary>
		/// Отбирает сохраненные <see cref="IViewModelViewMatch"/> для заданного списка объектов,
		/// которые ожидают присоединения к SidebarView
		/// </summary>
		/// <param name="connectedList">Список объектов, которые ожидают присоединения к SidebarView.</param>
		/// <returns>Словарь, в котором ключами выступает объекты из заданного списка, а значениями -
		/// подходящие для них представления.</returns>
		private Dictionary<object, IViewModelViewMatch> MatchSavedViewsToConnected(IEnumerable<object> connectedList)
		{
			var result = new Dictionary<object, IViewModelViewMatch>();

			foreach (var connected in connectedList)
			{
				var key = this._keySource.GetKey(connected);
				if (key == null)
				{
					continue;
				}

				foreach (var keyedMatch in this._viewMatches)
				{
					if (keyedMatch.Key.Equals(key) && keyedMatch.Value.IsMatchToModel(connected))
					{
						result.Add(connected, keyedMatch.Value);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Подбирает представление для заданного присоединенного объекта.
		/// </summary>
		/// <param name="connected">Объект, присоединенный к SidebarView.</param>
		/// <returns><see cref="IViewModelViewMatch"/> подобранного представления
		/// или null, если представление на найдено.</returns>
		private IViewModelViewMatch MatchNewViewToConnected(object connected)
		{
			var viewMatch = this._viewModelViewMatcher.Match(connected);
			if (viewMatch == null)
			{
				return null;
			}

			var key = this._keySource.GetKey(connected);
			if (key != null && viewMatch.SupportsModelChange)
			{
				this._viewMatches.Add(new KeyedViewModelViewMatch(key, viewMatch));
			}

			return viewMatch;
		}

		/// <summary>
		/// Делает запись в истории активности объектов.
		/// </summary>
		private void RecordActivity()
		{
			if (this.IsPlacementEmpty)
			{
				return;
			}

			if (this._activityKeys.Count != this.Connected.Count)
			{
				return;
			}

			var historyId = new SidebarActivityHistoryId(this._activityKeys.Values);
			if (this.Active == null)
			{
				this._activityHistory.Remove(historyId);
				return;
			}

			this._activityHistory[historyId] = this._activityKeys[this.Active];
		}

		/// <summary>
		/// Определяет активный объект, используя историю активности.
		/// </summary>
		/// <returns>Объект, который должен стать активным.</returns>
		private object RestoreActivity()
		{
			this._activityKeys.Clear();

			if (this.IsPlacementEmpty)
			{
				return null;
			}

			foreach (var connected in this.Connected)
			{
				var key = this._keySource.GetKey(connected);
				if (key != null)
				{
					this._activityKeys.Add(connected, key);
				}
			}

			var historyId = new SidebarActivityHistoryId(this._activityKeys.Values);

			object activeKey;
			if (this._activityHistory.TryGetValue(historyId, out activeKey))
			{
				// Когда запись истории найдена, выбираем активный
				// объект по сохраненному в истории ключу.
				foreach (var connectedKeyPair in this._activityKeys)
				{
					if (connectedKeyPair.Value.Equals(activeKey))
					{
						return connectedKeyPair.Key;
					}
				}
			}
			
			return this.Connected[0];
		}

		/// <summary>
		/// Убеждается, что заданный объект не участвует в перетаскивании.
		/// </summary>
		/// <param name="resolved">Объект для проверки на перетаскивание.</param>
		private void CheckDragDropNotSupported(object resolved)
		{
			if (this._connectedDragDrop.IsDragging(resolved))
			{
				throw new NotSupportedException(string.Format(
					Properties.Resources.SidebarPlacementConnectorDoesntSupportDragDrop,
					this.GetType().Name));
			}
		}
	}
}
