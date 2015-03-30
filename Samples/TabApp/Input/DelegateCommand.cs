using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace UriShell.Samples.TabApp.Input
{
	/// <summary>
	/// Представляет собой команду без параметра, делегирующую свои функции заданным методам.
	/// </summary>
	public class DelegateCommand : ViewModelCommandBase
	{
		/// <summary>
		/// Метод, выполняющий команду.
		/// </summary>
		private readonly Action _executeHandler;

		/// <summary>
		/// Метод, определяющий, может ли команда выполняться.
		/// </summary>
		private readonly Func<bool> _canExecuteHandler;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="DelegateCommand"/>.
		/// </summary>
		/// <param name="executeHandler">Метод, выполняющий команду.</param>
		public DelegateCommand(Action executeHandler)
			: this(executeHandler, null)
		{
		}

		/// <summary>
		/// Инициализирует новый объект класса <see cref="DelegateCommand"/>.
		/// </summary>
		/// <param name="executeHandler">Метод, выполняющий команду.</param>
		/// <param name="canExecuteHandler">Метод, определяющий, может ли команда выполняться.</param>
		public DelegateCommand(Action executeHandler, Func<bool> canExecuteHandler)
		{
			Contract.Requires<ArgumentNullException>(executeHandler != null);

			this._executeHandler = executeHandler;
			this._canExecuteHandler = canExecuteHandler;
		}

		/// <summary>
		/// Определяет, может ли команда выполняться в ее текущем состоянии.
		/// </summary>
		/// <param name="parameter">Данные, используемые командой. Если команда не требует
		/// передачи данных, этот объект может быть установлен в null.</param>
		/// <returns>true, если команда может быть выполнена; иначе false.</returns>
		public override bool CanExecute(object parameter)
		{
			if (this._canExecuteHandler != null)
			{
				return this._canExecuteHandler();
			}

			return true;
		}

		/// <summary>
		/// Выполняет данную команду.
		/// </summary>
		/// <param name="parameter">Данные, используемые командой. Если команда не требует
		/// передачи данных, этот объект может быть установлен в null.</param>
		public override void Execute(object parameter)
		{
			this._executeHandler();
		}
	}
}