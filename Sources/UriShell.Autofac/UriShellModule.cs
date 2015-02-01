using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UriShell.Input;
using UriShell.Shell;
using UriShell.Shell.Connectors;

namespace UriShell.Autofac
{
    /// <summary>
    /// Модуль Autofac, регистрирующий компоненты реализации АРМ Феникс.
    /// </summary>
    internal sealed class UriShellModule : Module
    {
        /// <summary>
        /// Настраивает и добавляет компоненты АРМ в заданный <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <param name="builder"><see cref="ContainerBuilder"/> для добавления компонентов АРМ.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<OpenUriCommand>()
                .SingleInstance();

            builder
				.RegisterType<Shell>()
				.As<IShell>()
				.As<IUriResolutionCustomization>()
				.WithParameter(
					(pi, c) => pi.ParameterType == typeof(Func<UriModuleItemResolverIndex>),
					(pi, c) => ShellModule.ResolveFactoryIncludingModules<UriModuleItemResolverIndex>(c))
				.OnActivated(ShellModule.OnShellActivated)
				.SingleInstance();

			builder
				.RegisterType<ResolveOpen>()
				.As<IShellResolve>();
			builder
				.RegisterGeneric(typeof(ResolveSetup<>))
				.As(typeof(IShellResolveSetup<>));
			builder
				.RegisterType<ResolveSetupFactory>()
				.As<IResolveSetupFactory>();

			builder
				.RegisterType<UriResolvedObjectHolder>()
				.As<IUriResolvedObjectHolder>()
				.SingleInstance();
			builder
				.RegisterType<UriDisconnectTable>()
				.As<IUriDisconnectTable>()
				.SingleInstance();
			
			builder
				.RegisterType<ExternalPlacementResolver>()
				.As<IUriPlacementResolver>()
				.SingleInstance();
			builder
				.RegisterType<BlankPlacementResolver>()
				.As<IUriPlacementResolver>()
				.SingleInstance();

			builder
				.RegisterType<EventBroadcaster>()
				.As<IEventBroadcaster>()
				.SingleInstance();
			
			builder
				.RegisterType<ViewModelViewMatcher>()
				.As<IViewModelViewMatcher>()
				.SingleInstance();

			builder
				.RegisterType<ItemsPlacementConnector>()
				.As<IItemsPlacementConnector>();
			builder
				.RegisterType<SidebarPlacementConnector>()
				.As<ISidebarPlacementConnector>()
				.ExternallyOwned();
			builder
				.RegisterType<ConnectedDragDrop>()
				.As<IConnectedDragDrop>()
				.OnActivated(e => e.Instance.DraggedClosed += FlexibleItemsSourceBehavior.OnConnectedDragDropDraggedClosed)
				.SingleInstance();
		}

		/// <summary>
		/// Обрабатывает событие активации <see cref="IShell"/> контейнером Dependency Injection.
		/// </summary>
		/// <param name="e">Объект, содержащий аргументы события.</param>
		private static void OnShellActivated(IActivatedEventArgs<IShell> e)
		{
			// Подключаем глобальные IUriPlacementResolver-ы, зарегистрированные ядром.
			foreach (var placementResolver in e.Context.Resolve<IEnumerable<IUriPlacementResolver>>())
			{
				e.Instance.AddUriPlacementResolver(placementResolver);
			}
		}

		/// <summary>
		/// Создает фабрику заданного типа объектов, которая открывает доступ к регистрациям
		/// подключаемых модулей.
		/// </summary>
		/// <typeparam name="T">Тип объектов, возвращаемых фабрикой.</typeparam>
		/// <param name="coreContainer">Контейнер Dependency Injection ядра.</param>
		/// <returns>Созданную фабрику заданного типа объектов.</returns>
		private static Func<T> ResolveFactoryIncludingModules<T>(IComponentContext coreContainer)
		{
			var diContainer = coreContainer.Resolve<IComponentContext>(); // требование Autofac.

			return () =>
			{
				// Контейнер ядра не имеет доступа к регистрациям подключаемых модулей.
				// Поэтому зарегистрированные ими компоненты оказываются недоступны.
				// Обратиться к ним можно только через IModuleLoader.
				var moduleLoader = diContainer.Resolve<IModuleLoader>();
				if (moduleLoader.ModuleContainer != null)
				{
					diContainer = moduleLoader.ModuleContainer;
				}

				return diContainer.Resolve<T>();
			};
		}
        }
    }
}
