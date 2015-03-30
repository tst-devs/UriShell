using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace UriShell.Samples.TabApp.Input
{
	/// <summary>
	/// Представляет собой команду с параметром, делегирующую свои функции заданным методам.
	/// </summary>
	/// <typeparam name="TParameter">Тип данных, используемых командой.</typeparam>
	public class DelegateCommand<TParameter> : ViewModelCommandBase
	{
		/// <summary>
		/// Метод, выполняющий команду.
		/// </summary>
		private readonly Action<TParameter> _executeHandler;

		/// <summary>
		/// Метод, определяющий, может ли команда выполняться.
		/// </summary>
		private readonly Func<TParameter, bool> _canExecuteHandler;

		/// <summary>
		/// Указывает, что команда допускает null в качестве параметра.
		/// </summary>
		private readonly bool _allowNulls;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="DelegateCommand{TParameter}"/>.
		/// </summary>
		/// <param name="executeHandler">Метод, выполняющий команду.</param>
		public DelegateCommand(Action<TParameter> executeHandler)
			: this(executeHandler, null)
		{
		}

		/// <summary>
		/// Инициализирует новый объект класса <see cref="DelegateCommand{TParameter}"/>.
		/// </summary>
		/// <param name="executeHandler">Метод, выполняющий команду.</param>
		/// <param name="canExecuteHandler">Метод, определяющий, может ли команда выполняться.</param>
		public DelegateCommand(Action<TParameter> executeHandler, Func<TParameter, bool> canExecuteHandler)
			: this(executeHandler, canExecuteHandler, false)
		{
		}

		/// <summary>
		/// Инициализирует новый объект класса <see cref="DelegateCommand{TParameter}"/>.
		/// </summary>
		/// <param name="executeHandler">Метод, выполняющий команду.</param>
		/// <param name="allowNulls">Указывает, что команда допускает null в качестве параметра.</param>
		public DelegateCommand(Action<TParameter> executeHandler, bool allowNulls)
			: this(executeHandler, null, allowNulls)
		{
			
		}

		/// <summary>
		/// Инициализирует новый объект класса <see cref="DelegateCommand{TParameter}"/>.
		/// </summary>
		/// <param name="executeHandler">Метод, выполняющий команду.</param>
		/// <param name="canExecuteHandler">Метод, определяющий, может ли команда выполняться.</param>
		/// <param name="allowNulls">Указывает, что команда допускает null в качестве параметра.</param>
		public DelegateCommand(Action<TParameter> executeHandler, Func<TParameter, bool> canExecuteHandler, bool allowNulls)
		{
			Contract.Requires<ArgumentNullException>(executeHandler != null);

			this._executeHandler = executeHandler;
			this._canExecuteHandler = canExecuteHandler;
			this._allowNulls = allowNulls;
		}

		/// <summary>
		/// Определяет, может ли команда выполняться в ее текущем состоянии.
		/// </summary>
		/// <param name="parameter">Данные, используемые командой. Если команда не требует
		/// передачи данных, этот объект может быть установлен в null.</param>
		/// <returns>true, если команда может быть выполнена; иначе false.</returns>
		public override bool CanExecute(object parameter)
		{
			if (parameter != null)
			{
				// Если параметр задан, но не может быть приведен к типу,
				// которым параметризована команда, запрещаем выполнение.
				if (!typeof(TParameter).IsAssignableFrom(parameter.GetType()))
				{
					return false;
				}
			}
			else if (!this._allowNulls)
			{
				// Если параметр содержит null, но команда не допускает
				// этого, запрещаем выполнение.
				return false;
			}
			else if (typeof(TParameter).IsValueType)
			{
				// Если параметр содержит null, а команда параметризована
				// значимым типом, запрещаем выполнение.
				return false;
			}

			if (this._canExecuteHandler != null)
			{
				return this._canExecuteHandler((TParameter)parameter);
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
			this._executeHandler((TParameter)parameter);
		}
	}
}