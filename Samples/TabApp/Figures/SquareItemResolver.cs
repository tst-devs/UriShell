using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UriShell.Shell;

namespace UriShell.Samples.TabApp.Figures
{
	internal sealed class SquareItemResolver : IUriModuleItemResolver
	{
		private readonly Func<SquareViewModel> _squareViewModelFactory;

		public SquareItemResolver(Func<SquareViewModel> squareViewModelFactory)
		{
			this._squareViewModelFactory = squareViewModelFactory;
		}

		public object Resolve(Uri uri, UriAttachmentSelector attachmentSelector)
		{
			return this._squareViewModelFactory();
		}
	}
}
