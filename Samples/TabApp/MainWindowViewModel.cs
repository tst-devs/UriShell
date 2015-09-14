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
	internal sealed class MainWindowViewModel : ObservableObject, IUriPlacementResolver
	{
		private readonly IItemsPlacementConnector _figurePlacementConnector;

		private readonly IShell _shell;

		public MainWindowViewModel(Func<IItemsPlacementConnector> itemsPlacementConnectorFactory, IShell shell)
		{
			this._figurePlacementConnector = itemsPlacementConnectorFactory();
			this._shell = shell;

			this._shell.AddUriPlacementResolver(this);
			
			this.SquareLength = 125;
			this.SquareBackground = "DeepSkyBlue";
			this.CircleDiameter = 150;
			this.CircleBackground = "Lime";

			this.AddSquareCommand = new DelegateCommand(() => 
				{
					var uri = ShellUriBuilder.StartUri()
						.Placement("figures")
						.Module("main")
						.Item("square")
						.Parameter("length", this.SquareLength.ToString())
						.Parameter("background", this.SquareBackground)
						.End();

					this._shell.Resolve(uri).Open();
				});

			this.AddCircleCommand = new DelegateCommand(() =>
				{
					var uri = ShellUriBuilder.StartUri()
						.Placement("figures")
						.Module("main")
						.Item("circle")
						.Parameter("diameter", this.CircleDiameter.ToString())
						.Parameter("background", this.CircleBackground)
						.End();

					this._shell.Resolve(uri).Open();
				});
		}

		public ICommand AddSquareCommand
		{
			get;
			private set;
		}

		public ICommand AddCircleCommand
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

		private double _squareLength;

		public double SquareLength
		{
			get
			{
				return this._squareLength; 
			}
			set
			{
				if (this._squareLength != value)
				{
					this._squareLength = value;
					this.OnPropertyChanged("SquareLength");
				}
			}
		}

		private string _squareBackground;

		public string SquareBackground
		{
			get
			{
				return this._squareBackground;
			}
			set
			{
				if (this._squareBackground != value)
				{
					this._squareBackground = value;
					this.OnPropertyChanged("SquareBackground");
				}
			}
		}

		private double _circleDiameter;

		public double CircleDiameter
		{
			get
			{
				return this._circleDiameter;
			}
			set
			{
				if (this._circleDiameter != value)
				{
					this._circleDiameter = value;
					this.OnPropertyChanged("CircleDiameter");
				}
			}
		}

		private string _circleBackground;

		public string CircleBackground
		{
			get
			{
				return this._circleBackground;
			}
			set
			{
				if (this._circleBackground != value)
				{
					this._circleBackground = value;
					this.OnPropertyChanged("CircleBackground");
				}
			}
		}

		public IUriPlacementConnector Resolve(object resolved, Uri uri, UriAttachmentSelector attachmentSelector)
		{
			var builder = new ShellUriBuilder(uri);
			if (builder.Placement == "figures")
			{
				return this._figurePlacementConnector;
			}

			return null;
		}
	}
}
