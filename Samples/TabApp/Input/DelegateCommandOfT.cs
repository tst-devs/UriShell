using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace UriShell.Samples.TabApp.Input
{
	public class DelegateCommand<TParameter> : ViewModelCommandBase
	{
		private readonly Action<TParameter> _executeHandler;

		private readonly Func<TParameter, bool> _canExecuteHandler;

		private readonly bool _allowNulls;

		public DelegateCommand(Action<TParameter> executeHandler)
			: this(executeHandler, null)
		{
		}

		public DelegateCommand(Action<TParameter> executeHandler, Func<TParameter, bool> canExecuteHandler)
			: this(executeHandler, canExecuteHandler, false)
		{
		}

		public DelegateCommand(Action<TParameter> executeHandler, bool allowNulls)
			: this(executeHandler, null, allowNulls)
		{
			
		}

		public DelegateCommand(Action<TParameter> executeHandler, Func<TParameter, bool> canExecuteHandler, bool allowNulls)
		{
			Contract.Requires<ArgumentNullException>(executeHandler != null);

			this._executeHandler = executeHandler;
			this._canExecuteHandler = canExecuteHandler;
			this._allowNulls = allowNulls;
		}

		public override bool CanExecute(object parameter)
		{
			if (parameter != null)
			{
				if (!typeof(TParameter).IsAssignableFrom(parameter.GetType()))
				{
					return false;
				}
			}
			else if (!this._allowNulls)
			{
				return false;
			}
			else if (typeof(TParameter).IsValueType)
			{
				return false;
			}

			if (this._canExecuteHandler != null)
			{
				return this._canExecuteHandler((TParameter)parameter);
			}
			
			return true;
		}

		public override void Execute(object parameter)
		{
			this._executeHandler((TParameter)parameter);
		}
	}
}