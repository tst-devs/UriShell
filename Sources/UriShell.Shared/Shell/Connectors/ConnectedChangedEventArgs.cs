using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Аргументы события <see cref="IItemsPlacementConnector.ConnectedChanged"/>.
	/// </summary>
	public class ConnectedChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Инициализирует новый объект класса <see cref="ConnectedChangedEventArgs"/>.
		/// </summary>
		/// <param name="action">Действие, вызвавшее событие <see cref="IItemsPlacementConnector.ConnectedChanged"/>.</param>
		/// <param name="changed">Объект, с которым было совершено действие.</param>
		public ConnectedChangedEventArgs(ConnectedChangedAction action, object changed)
		{
			this.Action = action;
			this.Changed = changed;
		}

		/// <summary>
		/// Задает инвариант класса.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.Changed != null);
		}

		/// <summary>
		/// Возвращает действие, вызвавшее событие <see cref="IItemsPlacementConnector.ConnectedChanged"/>.
		/// </summary>
		public ConnectedChangedAction Action
		{
			get;
			private set;
		}

		/// <summary>
		/// Возвращает объект, с которым было совершено действие.
		/// </summary>
		public object Changed
		{
			get;
			private set;
		}
	}
}
