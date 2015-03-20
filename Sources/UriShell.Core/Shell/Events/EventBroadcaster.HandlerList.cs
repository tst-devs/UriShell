using System;
using System.Collections.Generic;

namespace UriShell.Shell.Events
{
	partial class EventBroadcaster
	{
		/// <summary>
		/// Список обработчиков событий, удерживающий слабые ссылки на подписчиков.
		/// </summary>
		[Obsolete]
		private sealed class HandlerList
		{
			/// <summary>
			/// Ссылки на обработчики, добавленные в список.
			/// </summary>
			private readonly List<WeakDelegateReference> _list = new List<WeakDelegateReference>();

			/// <summary>
			/// Добавляет обработчик в список.
			/// </summary>
			/// <param name="handler">Обработчик события для добавления.</param>
			public void Add(Delegate handler)
			{
				this._list.Add(new WeakDelegateReference(handler));
			}

			/// <summary>
			/// Удаляет обработчик из списка.
			/// </summary>
			/// <param name="handler">Обработчик события для удаления.</param>
			public void Remove(Delegate handler)
			{
				for (int i = this._list.Count - 1; i >= 0; i--)
				{
					Delegate item;
					if (!this._list[i].TryGetDelegate(out item) || item == handler)
					{
						this._list.RemoveAt(i);
					}
				}
			}

			/// <summary>
			/// Перечисляет обработчики события, актуальные на момент вызова.
			/// </summary>
			/// <returns>Объект для перечисления обработчиков события.</returns>
			public IEnumerable<Delegate> EnumerateHandlers()
			{
				for (int i = this._list.Count - 1; i >= 0; i--)
				{
					Delegate item;
					if (this._list[i].TryGetDelegate(out item))
					{
						yield return item;
					}
					else
					{
						this._list.RemoveAt(i);
					}
				}
			}
		}
	}
}
