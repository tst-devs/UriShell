using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UriShell.Samples.TabApp.Input;
using UriShell.Shell;

namespace UriShell.Samples.TabApp.Figures
{
	internal sealed class CircleViewModel: ObservableObject, IRefreshable
	{
		private readonly IShell _shell;

		public CircleViewModel(IShell shell)
		{
			this._shell = shell;

			this.CloseCommand = new DelegateCommand(() => this._shell.CloseResolved(this));
		}

		public void Refresh()
		{
			var builder = new PhoenixUriBuilder(this._shell.GetResolvedUri(this));

			var diameterString = builder.Parameters["diameter"];
			double diameter;
			if (!double.TryParse(diameterString, out diameter))
			{
				diameter = 150;
			}

			this.Diameter = diameter;

			var backgroundString = builder.Parameters["background"];
			this.Background = !string.IsNullOrEmpty(backgroundString) ? backgroundString : "Red";
		}

		private double _diameter;

		public double Diameter
		{
			get
			{
				return this._diameter;
			}
			private set
			{
				if (this._diameter != value)
				{
					this._diameter = value;
					this.OnPropertyChanged("Diameter");
					this.OnPropertyChanged("Title");
				}
			}
		}

		private string _background;

		public string Background
		{
			get
			{
				return this._background;
			}
			private set
			{ 
				if (this._background != value)
				{
					this._background = value;
					this.OnPropertyChanged("Background");
					this.OnPropertyChanged("Title");
				}
			}
		}

		public string Title
		{
			get
			{
				return string.Format("I'm a circle view of color {0} and radius {1}", this.Background, this.Diameter);
			}
		}

		public ICommand CloseCommand
		{
			get;
			private set;
		}
	}
}
