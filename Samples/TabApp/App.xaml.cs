using Autofac;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using UriShell.Autofac;
using UriShell.Samples.TabApp.Figures;
using UriShell.Shell;

namespace UriShell.Samples.TabApp
{
    public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var builder = new ContainerBuilder();

			builder.RegisterType<MainWindowViewModel>()
				.As<IUriPlacementResolver>()
				.As<MainWindowViewModel>();
			builder.RegisterType<MainWindow>();

			builder.RegisterType<SquareViewModel>();
			builder.RegisterType<SquareView>();
			builder
				.RegisterType<SquareItemResolver>()
				.Keyed<IUriModuleItemResolver>(new UriModuleItemResolverKey("main", "square"));
			builder.RegisterModule<UriShellModule>();

			this.Container =  builder.Build();

			this.Container.Resolve<MainWindow>().Show();
		}

		public IContainer Container 
		{
			get; 
			set; 
		}
	}
}
