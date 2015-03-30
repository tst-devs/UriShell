using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace UriShell.Samples.TabApp.Input
{
	/// <summary>
	/// Базовый класс команд, предназначенных для использования в моделях представлений.
	/// </summary>
	public abstract class ViewModelCommandBase : ICommand
	{
		/// <summary>
		/// Список обработчиков <see cref="CanExecuteChanged"/> со слабыми ссылками
		/// на подписчиков.
		/// </summary>
		private readonly List<WeakDelegateReference> _canExecuteChangedHandlers = new List<WeakDelegateReference>();

		/// <summary>
		/// Выполняет обход списка обработчиков <see cref="CanExecuteChanged"/>,
		/// вызывая заданное действие для достижимых обработчиков и удаляя недостижимые.
		/// </summary>
		/// <param name="aliveHandlerAction">Действие, вызываемое для достижимого обработчика
		/// и соответствующей записи из <see cref="_canExecuteChangedHandlers"/>.</param>
		private void WalkCanExecuteChangedHandlers(Action<EventHandler, WeakDelegateReference> aliveHandlerAction)
		{
			for (int i = this._canExecuteChangedHandlers.Count - 1; i >= 0; i--)
			{
				var weak = this._canExecuteChangedHandlers[i];
				Delegate handler;

				// Пробуем получить делегат по ссылке.
				// Если делегат недостижим, удаляем запись из списка обработчиков.
				if (weak.TryGetDelegate(out handler))
				{
					aliveHandlerAction((EventHandler)handler, weak);
				}
				else
				{
					this._canExecuteChangedHandlers.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Вызывается при изменениях, влияющих на то, должна ли выполняться команда. 
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
		    add
		    {
				// Сохраняем ссылку на метод со слабой ссылкой на целевой объект.
				var weak = new WeakDelegateReference(value);
				lock (this._canExecuteChangedHandlers)
				{
					this._canExecuteChangedHandlers.Add(weak);
				}
		    }
		    remove
		    {
				lock (this._canExecuteChangedHandlers)
				{
					WeakDelegateReference itemToRemove = null;

					// Обходим достижимые обработчики и определяем
					// запись для удаления, сравнивая делегаты.
					this.WalkCanExecuteChangedHandlers(
						(handler, weak) =>
						{
							if (value.Equals(handler))
							{
								itemToRemove = weak;
							}
						});

					this._canExecuteChangedHandlers.Remove(itemToRemove);
				}
		    }
		}

		/// <summary>
		/// Инициирует проверку возможности выполнения команды ее пользователями.
		/// </summary>
		internal void Invalidate()
		{
			var handlers = new List<EventHandler>();

			lock (this._canExecuteChangedHandlers)
			{
				this.WalkCanExecuteChangedHandlers((handler, weak) => handlers.Add(handler));
			}

			foreach (var handler in handlers)
			{
				handler(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Определяет, может ли команда выполняться в ее текущем состоянии.
		/// </summary>
		/// <param name="parameter">Данные, используемые командой. Если команда не требует
		/// передачи данных, этот объект может быть установлен в null.</param>
		/// <returns><langword>true</langword>, если команда может быть выполнена;
		/// иначе <langword>false</langword>.</returns>
		public abstract bool CanExecute(object parameter);

		/// <summary>
		/// Выполняет данную команду.
		/// </summary>
		/// <param name="parameter">Данные, используемые командой. Если команда не требует
		/// передачи данных, этот объект может быть установлен в null.</param>
		public abstract void Execute(object parameter);
	}
}