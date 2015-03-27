using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Shell
{
	/// <summary>
	/// Defines a method that can be used by shell to refresh the object.
	/// </summary>
	public interface IRefreshable
	{
		/// <summary>
		/// Signals this object to refresh.
		/// </summary>
		void Refresh();
	}
}
