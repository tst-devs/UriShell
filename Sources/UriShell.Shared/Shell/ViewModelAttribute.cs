using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Shell
{
	/// <summary>
	/// Links a view with its view model via a constructor parameter or a property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
	public class ViewModelAttribute : Attribute
	{
	}
}