using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UriShell.Shell;

namespace UriShell.Samples.TabApp.Figures
{
	internal sealed class CircleItemResolver : IUriModuleItemResolver
	{
		private readonly Func<CircleViewModel> _viewModelFactory;

		public CircleItemResolver(Func<CircleViewModel> viewModelFactory)
		{
			this._viewModelFactory = viewModelFactory;
		}

		public object Resolve(Uri uri, UriAttachmentSelector attachmentSelector)
		{
			return this._viewModelFactory();
		}
	}
}
