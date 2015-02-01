using System;
using System.Diagnostics.Contracts;

using Autofac;

using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Реализация фабрики сервиса настройки объектов, полученных через URI.
	/// </summary>
	internal sealed class ResolveSetupFactory : IResolveSetupFactory
	{
		/// <summary>
		/// Контейнер Dependency Injection.
		/// </summary>
		private readonly IComponentContext _diContainer;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ResolveSetupFactory"/>.
		/// </summary>
		/// <param name="diContainer">Контейнер Dependency Injection.</param>
		public ResolveSetupFactory(IComponentContext diContainer)
		{
			Contract.Requires<ArgumentNullException>(diContainer != null);
			this._diContainer = diContainer;
		}

		/// <summary>
		/// Создает сервис, реализующий настройку объектов, полученных через URI.
		/// </summary>
		/// <typeparam name="TResolved">Тип объекта, который ожидается от URI.</typeparam>
		/// <param name="args">Аргументы, необходимые для инициализации <see cref="ResolveSetup{TResolved}"/>.</param>
		/// <returns>Сервис, позволяющий настроить и открыть объект, полученный через URI.</returns>
		public IShellResolveSetup<TResolved> Create<TResolved>(ResolveSetupArgs args)
		{
			return this._diContainer.Resolve<IShellResolveSetup<TResolved>>(TypedParameter.From(args));
		}
	}
}
