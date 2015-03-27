using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UriShell.Shell;

namespace UriShell.Samples.TabApp.Figures
{
	internal partial class SquareView : UserControl
	{
		public SquareView([ViewModel]SquareViewModel viewModel)
		{
			InitializeComponent();
			this.DataContext = viewModel;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			//return base.MeasureOverride(constraint);

			var model = (SquareViewModel)this.DataContext;
			return new Size(model.Radius * 2, model.Radius * 2);
		}
	}
}
