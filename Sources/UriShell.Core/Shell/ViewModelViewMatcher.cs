using System;
using System.Diagnostics.Contracts;
using System.Linq;

using Autofac;
using Autofac.Core;

namespace UriShell.Shell
{
	/// <summary>
	/// Реализует <see cref="IViewModelViewMatcher"/>, выполняя поиск представлений
	/// в контейнерах Dependency Injection ядра и модулей.
	/// </summary>
	internal sealed class ViewModelViewMatcher : IViewModelViewMatcher
	{
		/// <summary>
		/// Контейнер Dependency Injection ядра.
		/// </summary>
		private readonly IComponentContext _coreContainer;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ViewModelViewMatcher"/>.
		/// </summary>
		/// <param name="coreContainer">Контейнер Dependency Injection ядра.</param>
		public ViewModelViewMatcher(IComponentContext coreContainer)
		{
			Contract.Requires<ArgumentNullException>(coreContainer != null);

			this._coreContainer = coreContainer;
		}

		/// <summary>
		/// Выполняет поиск объекта, реализующего представление по заданной модели.
		/// </summary>
		/// <param name="viewModel">Модель искомого представления.</param>
		/// <returns>Результат поиска, который в случае успеха содержит экземпляр
		/// <see cref="IViewModelViewMatch"/>, или null, когда ничего не найдено.</returns>
		public IViewModelViewMatch Match(object viewModel)
		{
			var matchFromCore = ViewModelViewMatcher.Match(viewModel, this._coreContainer);
			if (matchFromCore != null)
			{
				return matchFromCore;
			}

#warning Add Module Loader facade or maybe there should be only container handling
//            var moduleLoader = this._coreContainer.Resolve<IModuleLoader>();
//            if (moduleLoader == null)
//            {
//                return null;
//            }

            //return ViewModelViewMatcher.Match(viewModel, moduleLoader.ModuleContainer);

            return null;
		}

		/// <summary>
		/// Выполняет поиск объекта, реализующего представление по заданной модели,
		/// в заданном контейнере Dependency Injection.
		/// </summary>
		/// <param name="viewModel">Модель искомого представления.</param>
		/// <param name="diContainer">Контейнер Dependency Injection для поиска представлений.</param>
		/// <returns>Результат поиска, который в случае успеха содержит экземпляр
		/// <see cref="IViewModelViewMatch"/>, или null, когда ничего не найдено.</returns>
		private static IViewModelViewMatch Match(object viewModel, IComponentContext diContainer)
		{
			// Для поиска представления в контейнере, просматриваем все регистрации,
			// связанные с типом. У каждого типа просматриваем список свойств и
			// параметры конструкторов на предмет ViewModelAttribute.

			var serviceTypes = diContainer
				.ComponentRegistry
				.Registrations
				.SelectMany(r => r.Services)
				.OfType<IServiceWithType>()
				.Select(s => s.ServiceType);

			foreach (var serviceType in serviceTypes)
			{
				var propertyMatch = ViewModelPropertyMatch.TryMatch(
					viewModel,
					serviceType,
					() => diContainer.Resolve(serviceType));

				if (propertyMatch != null)
				{
					return propertyMatch;
				}

				var parameterMatch = ViewModelParameterMatch.TryMatch(
					viewModel,
					serviceType,
					pi => diContainer.Resolve(serviceType, new NamedParameter(pi.Name, viewModel)));

				if (parameterMatch != null)
				{
					return parameterMatch;
				}
			}

			return null;
		}
	}
}
