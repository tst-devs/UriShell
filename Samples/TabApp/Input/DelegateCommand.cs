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
		private readonly Action _executeHandler;

		private readonly Func<bool> _canExecuteHandler;

		public DelegateCommand(Action executeHandler)
			: this(executeHandler, null)
		{
		}

		public DelegateCommand(Action executeHandler, Func<bool> canExecuteHandler)
		{
			Contract.Requires<ArgumentNullException>(executeHandler != null);

			this._executeHandler = executeHandler;
			this._canExecuteHandler = canExecuteHandler;
		}

		public override bool CanExecute(object parameter)
		{
			if (this._canExecuteHandler != null)
			{
				return this._canExecuteHandler();
			}

			return true;
		}

		public override void Execute(object parameter)
		{
			this._executeHandler();
		}
	}
}