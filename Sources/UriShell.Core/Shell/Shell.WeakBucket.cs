using System;
using System.Collections.Generic;
using System.Linq;

namespace UriShell.Shell
{
	partial class Shell
	{
		/// <summary>
		/// Наполняет список объектов заданного типа, используя слабые ссылки.
		/// </summary>
		private sealed class WeakBucket<T> where T : class
		{
			/// <summary>
			/// Хранит слабые ссылки на добавленные объекты.
			/// </summary>
			private readonly List<WeakReference> _list = new List<WeakReference>();

			/// <summary>
			/// Добавляет объект к перечислению.
			/// </summary>
			/// <param name="object">Объект для добавления.</param>
			public void Add(T @object)
			{
				if (this.ExtractAlive().Contains(@object))
				{
					return;
				}

				this._list.Add(new WeakReference(@object));
			}

			/// <summary>
			/// Возвращает список достижимых объектов.
			/// </summary>
			/// <returns>Список достижимых объектов.</returns>
			public IEnumerable<T> ExtractAlive()
			{
				var alive = new List<T>(this._list.Count);
				for (int i = this._list.Count - 1; i >= 0; i--)
				{
					var target = this._list[i].Target as T;
					if (target != null)
					{
						alive.Add(target);
					}
					else
					{
						this._list.RemoveAt(i);
					}
				}

				return alive;
			}
		}
	}
}
