using System;
using System.Diagnostics.Contracts;

using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Аргументы для создания объектов, реализующих <see cref="IShellResolveSetup{TResolved}"/>.
	/// </summary>
	internal sealed class ResolveSetupArgs
	{
		/// <summary>
		/// Инициализирует новый объект класса <see cref="ResolveSetupArgs"/>.
		/// </summary>
		/// <param name="resolveOpen">Сервис, позволяющий открыть объект, полученный через URI.</param>
		/// <param name="playerSender">Действие, позволяющее передать делегат вызова настроек.</param>
		public ResolveSetupArgs(IShellResolveOpen resolveOpen, Action<ResolveSetupPlayer> playerSender)
		{
			this.ResolveOpen = resolveOpen;
			this.PlayerSender = playerSender;
		}

		/// <summary>
		/// Задает инвариант класса.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.ResolveOpen != null);
			Contract.Invariant(this.PlayerSender != null);
		}

		/// <summary>
		/// Сервис, позволяющий открыть объект, полученный через URI.
		/// </summary>
		public IShellResolveOpen ResolveOpen
		{
			get;
			private set;
		}

		/// <summary>
		/// Действие, позволяющее передать делегат вызова настроек.
		/// </summary>
		public Action<ResolveSetupPlayer> PlayerSender
		{
			get;
			private set;
		}
	}
}
