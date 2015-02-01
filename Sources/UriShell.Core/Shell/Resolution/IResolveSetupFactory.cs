using System;
using System.Diagnostics.Contracts;

using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Фабрика сервиса, реализующего настройку объектов, полученных через URI.
	/// </summary>
	[ContractClass(typeof(IResolveSetupFactoryContract))]
	internal interface IResolveSetupFactory
	{
		/// <summary>
		/// Создает сервис, реализующий настройку объектов, полученных через URI.
		/// </summary>
		/// <typeparam name="TResolved">Тип объекта, который ожидается от URI.</typeparam>
		/// <param name="args">Аргументы, необходимые для инициализации <see cref="ResolveSetup{TResolved}"/>.</param>
		/// <returns>Сервис, позволяющий настроить и открыть объект, полученный через URI.</returns>
		IShellResolveSetup<TResolved> Create<TResolved>(ResolveSetupArgs args);
	}
}