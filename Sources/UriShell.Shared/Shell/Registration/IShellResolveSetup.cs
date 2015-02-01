using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Registration
{
	/// <summary>
	/// Позволяет настроить объект заданного типа, полученный через URI.
	/// </summary>
	/// <typeparam name="TResolved">Тип объекта, который ожидается от URI.</typeparam>
	[ContractClass(typeof(IShellResolveSetupContract<>))]
	public interface IShellResolveSetup<TResolved> : IShellResolveOpen
	{
		/// <summary>
		/// Позволяет задать действие, вызываемое с объектом, полученным через URI,
		/// перед открытием.
		/// </summary>
		/// <param name="action">Действие, вызываемое с объектом, полученным через URI,
		/// перед открытием.</param>
		/// <returns>Сервис, позволяющий настроить и открыть объект, полученный через URI.</returns>
		IShellResolveSetup<TResolved> OnReady(Action<TResolved> action);

		/// <summary>
		/// Позволяет задать действие, вызываемое с объектом, полученным через URI,
		/// когда в нем больше нет необходимости.
		/// </summary>
		/// <param name="action">Действие, вызываемое с объектом, полученным через URI,
		/// когда в нем больше нет необходимости.</param>
		/// <returns>Сервис, позволяющий настроить и открыть объект, полученный через URI.</returns>
		IShellResolveSetup<TResolved> OnFinished(Action<TResolved> action);
	}
}
