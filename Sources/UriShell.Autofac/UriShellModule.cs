using Autofac;
using Autofac.Core;

using System.Collections.Generic;

using UriShell.Shell;
using UriShell.Shell.Connectors;
using UriShell.Shell.Registration;
using UriShell.Shell.Resolution;


namespace UriShell.Autofac
{
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

#warning OnActivated and inner module resolution? 

            builder
				.RegisterType<UriShell.Shell.Shell>()
				.As<IShell>()
				.As<IUriResolutionCustomization>()
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
				.As<AutofacViewModelViewMatcher>()
				.SingleInstance();

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
    }
}
