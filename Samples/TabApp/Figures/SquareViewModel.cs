using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

using UriShell.Samples.TabApp.Input;
using UriShell.Shell;

namespace UriShell.Samples.TabApp.Figures
{
	internal sealed class SquareViewModel : ObservableObject, IRefreshable
	{
		private readonly IShell _shell;

		public SquareViewModel(IShell shell)
		{
			this._shell = shell;

			this.CloseCommand = new DelegateCommand(() => this._shell.CloseResolved(this));
		}

		public void Refresh()
		{
			var builder = new ShellUriBuilder(this._shell.GetResolvedUri(this));

			var lengthString = builder.Parameters["length"];
			double length;
			if (!double.TryParse(lengthString, out length))
			{
				length = 125;
			}

			this.Length = length;

			var backgroundString = builder.Parameters["background"];
			this.Background = !string.IsNullOrEmpty(backgroundString) ? backgroundString : "Red";
		}

		private double _length;

		public double Length
		{
			get
			{
				return this._length;
			}
			private set
			{
				if (this._length != value)
				{
					this._length = value;
					this.OnPropertyChanged("Length");
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
				return string.Format("I'm a square view of color {0} and size {1}", this.Background, this.Length);
			}
		}

		public ICommand CloseCommand
		{
			get;
			private set;
		}
	}
}
