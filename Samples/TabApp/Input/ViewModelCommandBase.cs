using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace UriShell.Samples.TabApp.Input
{
	public abstract class ViewModelCommandBase : ICommand
	{
		private readonly List<WeakDelegateReference> _canExecuteChangedHandlers = new List<WeakDelegateReference>();

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

		public abstract bool CanExecute(object parameter);

		public abstract void Execute(object parameter);
	}
}