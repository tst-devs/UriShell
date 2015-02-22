using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Shell
{
	/// <summary>
	/// Represents a method that allows to get an object attached to the URI. 
	/// </summary>
	/// <param name="id">The identifier of an attached object.</param>
	/// <returns>The object attached to the URI, if the identifier was found; otherwise null.</returns>
	public delegate object UriAttachmentSelector(string id);
}
