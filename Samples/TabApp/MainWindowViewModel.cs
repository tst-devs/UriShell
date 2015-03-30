using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UriShell.Samples.TabApp.Input;
using UriShell.Shell;
using UriShell.Shell.Connectors;

namespace UriShell.Samples.TabApp
{
	internal sealed class MainWindowViewModel : IUriPlacementResolver
	{
		private readonly IItemsPlacementConnector _figurePlacementConnector;

		private readonly IShell _shell;

		public MainWindowViewModel(Func<IItemsPlacementConnector> itemsPlacementConnectorFactory, IShell shell)
		{
			this._figurePlacementConnector = itemsPlacementConnectorFactory();
			this._shell = shell;

			this._shell.AddUriPlacementResolver(this);

			this.AddSquareCommand = new DelegateCommand(() => 
				{
					var uri = PhoenixUriBuilder.StartUri()
						.Placement("figures")
						.Module("main")
						.Item("square")
						.End();

					try
					{
						this._shell.Resolve(uri).OpenOrThrow();
					}
					catch (Exception ex)
					{ 
					
					}
				});
		}

		public ICommand AddSquareCommand
		{
			get;
			private set;
		}

		public ICollectionView Figures
		{ 
			get
			{
				return this._figurePlacementConnector.Views;
			}
		}

		public IUriPlacementConnector Resolve(object resolved, Uri uri, UriAttachmentSelector attachmentSelector)
		{
			var builder = new PhoenixUriBuilder(uri);
			if (builder.Placement == "figures")
			{
				return this._figurePlacementConnector;
			}

			return null;
		}
	}
}
