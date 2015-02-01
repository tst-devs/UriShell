using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Аргументы событий, предназначенных для объектов, открытых оболочкой через URI.
	/// </summary>
	public sealed class ResolvedIdBroadcastArgs
	{
		/// <summary>
		/// Инициализирует новый объект <see cref="ResolvedIdBroadcastArgs"/>.
		/// </summary>
		/// <param name="resolvedId">Идентификатор объекта, открытого через URI,
		/// которому предназначено событие.</param>
		public ResolvedIdBroadcastArgs(int resolvedId)
		{
			this.ResolvedId = resolvedId;
		}

		/// <summary>
		/// Задает инвариант класса.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.ResolvedId >= PhoenixUriBuilder.MinResolvedId);
			Contract.Invariant(this.ResolvedId <= PhoenixUriBuilder.MaxResolvedId);
		}

		/// <summary>
		/// Возвращает идентификатор объекта, открытого через URI, которому
		/// предназначено событие.
		/// </summary>
		public int ResolvedId
		{
			get;
			private set;
		}
	}
}
