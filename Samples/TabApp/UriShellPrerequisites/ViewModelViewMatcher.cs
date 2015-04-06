using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UriShell.Samples.TabApp.Figures;
using UriShell.Shell;

namespace UriShell.Samples.TabApp.UriShellPrerequisites
{
	internal sealed class ViewModelViewMatcher : IViewModelViewMatcher
	{
		public IViewModelViewMatch Match(object viewModel)
		{
			if (viewModel is SquareViewModel)
			{
				return ViewModelParameterMatch.TryMatch(
					viewModel,
					typeof(SquareView),
					new Func<ParameterInfo, object>(p => new SquareView((SquareViewModel)viewModel)));
			}
			else if (viewModel is CircleViewModel)
			{
				return ViewModelParameterMatch.TryMatch(
					viewModel,
					typeof(CircleView),
					new Func<ParameterInfo, object>(p => new CircleView((CircleViewModel)viewModel)));
			}
			return null;
		}
	}
}
