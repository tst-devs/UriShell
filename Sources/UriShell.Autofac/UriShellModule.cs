using Autofac;
using Autofac.Core;
using Autofac.Features.Indexed;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UriShell.Input;
using UriShell.Shell;
using UriShell.Shell.Connectors;
using UriShell.Shell.Registration;
using UriShell.Shell.Resolution;


namespace UriShell.Autofac
{
	using AutofacUriModuleItemResolverIndex = IIndex<UriModuleItemResolverKey, IUriModuleItemResolver>;

	using UriShellUriModuleItemResolverIndex = UriShell.Collections.IIndex<UriModuleItemResolverKey, IUriModuleItemResolver>;

	/// <summary>
    /// The Autofac module that registers components of the UriShell library.
    /// </summary>
    public sealed class UriShellModule : Module
    {
        /// <summary>
        /// Setup and adds components of the UriShell to the given <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <param name="builder">The target <see cref="ContainerBuilder"/>.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<OpenUriCommand>()
                .SingleInstance();

#warning OnActivated and inner module resolution? 

            builder
				.RegisterType<UriShell.Shell.Shell>()
				.As<IShell>()
				.As<IUriResolutionCustomization>()
				.WithParameter(
					(pi, c) => pi.ParameterType == typeof(Func<UriShellUriModuleItemResolverIndex>),
					(pi, c) => 
						{
							var autofacIndexFactory = c.Resolve<Func<AutofacUriModuleItemResolverIndex>>()();
							return new Func<UriShellUriModuleItemResolverIndex>(() => new AutofacIndexWrapper<UriModuleItemResolverKey, IUriModuleItemResolver>(autofacIndexFactory));
							/*UriShellModule.ResolveFactoryIncludingModules<UriModuleItemResolverIndex>(c)*/
						})
				//.OnActivated(ShellModule.OnShellActivated)
				.SingleInstance();

			builder
				.RegisterType<ResolveOpen>()
				.As<IShellResolve>();
			builder
				.RegisterGeneric(typeof(ResolveSetup<>))
				.As(typeof(IShellResolveSetup<>));
			builder
				.RegisterType<AutofacResolveSetupFactory>()
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
#warning Remove?
			//builder
			//	.RegisterType<BlankPlacementResolver>()
			//	.As<IUriPlacementResolver>()
			//	.SingleInstance();

			builder
				.RegisterType<AutofacViewModelViewMatcher>()
				.As<IViewModelViewMatcher>()
				.SingleInstance();

			builder
				.RegisterType<ItemsPlacementConnector>()
				.As<IItemsPlacementConnector>();

#warning Remove? 
			//builder
			//	.RegisterType<SidebarPlacementConnector>()
			//	.As<ISidebarPlacementConnector>()
			//	.ExternallyOwned();

#warning OnActivated? 
			builder
				.RegisterType<ConnectedDragDrop>()
				.As<IConnectedDragDrop>()
				//.OnActivated(e => e.Instance.DraggedClosed += FlexibleItemsSourceBehavior.OnConnectedDragDropDraggedClosed)
				.SingleInstance();
		}

		/// <summary>
		/// Handles the event of <see cref="IShell"/> activation by the dependency injection container.
		/// </summary>
		/// <param name="e">The object with event arguments.</param>
		private static void OnShellActivated(IActivatedEventArgs<IShell> e)
		{
			// Add global IUriPlacementResolvers registered by the core.
			foreach (var placementResolver in e.Context.Resolve<IEnumerable<IUriPlacementResolver>>())
			{
				e.Instance.AddUriPlacementResolver(placementResolver);
			}
		}

		///// <summary>
		///// Creates the factory of an object of the given type that opens access to registrations 
		///// of pluggable modules.
		///// </summary>
		///// <typeparam name="T">The type of an object created by the factory.</typeparam>
		///// <param name="coreContainer">The core dependency injection container.</param>
		///// <returns>The factory of an object of the given type.</returns>
		//private static Func<T> ResolveFactoryIncludingModules<T>(IComponentContext coreContainer)
		//{
		//	var diContainer = coreContainer.Resolve<IComponentContext>(); // Autofac claim.

		//	return () =>
		//	{
		//		// The core container doen't have access to registrations of pluggable modules.
		//		// Hence their components are invisible. 
		//		// We can reach them only via IModuleLoader.
		//		var moduleLoader = diContainer.Resolve<IModuleLoader>();
		//		if (moduleLoader.ModuleContainer != null)
		//		{
		//			diContainer = moduleLoader.ModuleContainer;
		//		}

		//		return diContainer.Resolve<T>();
		//	};
		//}
    }
}
