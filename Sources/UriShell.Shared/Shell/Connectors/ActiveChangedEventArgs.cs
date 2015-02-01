using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Аргументы события <see cref="IItemsPlacementConnector.ActiveChanged"/>.
	/// </summary>
	public class ActiveChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Инициализирует новый объект класса <see cref="ActiveChangedEventArgs"/>.
		/// </summary>
		/// <param name="oldValue">Объект, представление которого было активным.</param>
		/// <param name="newValue">Объект, представление которого стало активным.</param>
		public ActiveChangedEventArgs(object oldValue, object newValue)
		{
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}

		/// <summary>
		/// Задает инвариант класса.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.OldValue != null || this.NewValue != null);
		}

		/// <summary>
		/// Возвращает объект, представление которого было активным.
		/// </summary>
		public object OldValue
		{
			get;
			private set;
		}

		/// <summary>
		/// Возвращает объект, представление которого стало активным.
		/// </summary>
		public object NewValue
		{
			get;
			private set;
		}
	}
}
