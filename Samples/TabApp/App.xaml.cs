using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;

using Autofac;

using UriShell.Autofac;
using UriShell.Collections;
using UriShell.Samples.TabApp.Figures;
using UriShell.Shell;
using UriShell.Shell.Resolution;
using UriShell.Shell.Registration;
using UriShell.Shell.Connectors;

using UriShell.Samples.TabApp.UriShellPrerequisites;

namespace UriShell.Samples.TabApp
{
	using Shell = UriShell.Shell.Shell;

	public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

		    //this.RunUriShellManually();
			this.RunUriShellWithDi();
		}

		public void RunUriShellWithDi()
		{
			Settings.Initialize(b =>
			{
				b.Scheme = "tabapp";
			});

			var builder = new ContainerBuilder();

			builder.RegisterModule<UriShellModule>();

			builder.RegisterType<MainWindowViewModel>()
				.As<IUriPlacementResolver>()
				.As<MainWindowViewModel>();
			builder.RegisterType<MainWindow>();

			this.Container = builder.Build();

			var moduleContainer = this.Container.BeginLifetimeScope(b =>
			{
				b.RegisterType<SquareViewModel>();
				b.RegisterType<SquareView>();
				b.RegisterType<CircleViewModel>();
				b.RegisterType<CircleView>();
			});

            var matcher = moduleContainer.Resolve<AutofacViewModelViewMatcher>();
            matcher.AddContainer(moduleContainer);

			var shell = moduleContainer.Resolve<IShell>();
			shell.AddUriModuleItemResolver(new UriModuleItemResolverKey("main", "square"),
				new SquareItemResolver(moduleContainer.Resolve<Func<SquareViewModel>>()));
			shell.AddUriModuleItemResolver(new UriModuleItemResolverKey("main", "circle"),
				new CircleItemResolver(moduleContainer.Resolve<Func<CircleViewModel>>()));

			this.Container.Resolve<MainWindow>().Show();			
		}

		public void RunUriShellManually()
		{
			Settings.Initialize(b =>
			{
				b.Scheme = "tabapp";
			});

			var holder = new UriResolvedObjectHolder();
			var resolveSetupFactory = new DefaultResolveSetupFactory();
			var uriDisconnectTable = new UriDisconnectTable();
			var connectedDragDrop = new ConnectedDragDrop(uriDisconnectTable);
			var viewModelViewMatcher = new ViewModelViewMatcher();

			Func<Uri, object[], IShellResolve> shellResolveFactory = (uri, attachments) => 
				new ResolveOpen(uri, attachments, resolveSetupFactory, holder, uriDisconnectTable, this.Shell);

			var resolvers = new Dictionary<UriModuleItemResolverKey, IUriModuleItemResolver>();
			resolvers.Add(new UriModuleItemResolverKey("main", "square"), new SquareItemResolver(() => new SquareViewModel(this.Shell)));
			resolvers.Add(new UriModuleItemResolverKey("main", "circle"), new CircleItemResolver(() => new CircleViewModel(this.Shell)));

			this.Shell = new Shell(holder, shellResolveFactory);

			this.Shell.AddUriModuleItemResolver(
				new UriModuleItemResolverKey("main", "square"), 
				new SquareItemResolver(() => new SquareViewModel(this.Shell)));

			this.Shell.AddUriModuleItemResolver(
				new UriModuleItemResolverKey("main", "circle"), 
				new CircleItemResolver(() => new CircleViewModel(this.Shell)));

			var viewModel = new MainWindowViewModel(() => new ItemsPlacementConnector(viewModelViewMatcher, connectedDragDrop), this.Shell);
			var view = new MainWindow(viewModel);
			view.Show();			
		}

		public IContainer Container
		{
			get;
			set;
		}

		public Shell Shell
		{
			get;
			private set;
		}
	}
}
