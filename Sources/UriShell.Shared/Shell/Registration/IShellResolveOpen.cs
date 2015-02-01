using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Registration
{
	/// <summary>
	/// Позволяет открыть объект, полученный через URI.
	/// </summary>
	[ContractClass(typeof(IShellResolveOpenContract))]
	public interface IShellResolveOpen
	{
		/// <summary>
		/// Открывает объект, полученный через URI.
		/// </summary>
		/// <returns>Сервис, позволяющий закрыть объект вызовом <see cref="IDisposable.Dispose"/>.</returns>
		IDisposable Open();

		/// <summary>
		/// Открывает объект, полученный через URI, позволяя вызывающему коду обработать
		/// исключение в случае неудачи.
		/// </summary>
		/// <returns>Сервис, позволяющий закрыть объект вызовом <see cref="IDisposable.Dispose"/>,
		/// если объект открыт успешно.</returns>
		IDisposable OpenOrThrow();
	}
}
