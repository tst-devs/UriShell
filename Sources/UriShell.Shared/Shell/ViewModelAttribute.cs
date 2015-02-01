using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Shell
{
	/// <summary>
	/// Связывает представление с его моделью через параметр конструктора или свойство.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
	public class ViewModelAttribute : Attribute
	{
	}
}