using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Disposables
{
	/// <summary>
	/// Represents a disposable that does nothing on disposal.
	/// </summary>
	internal sealed class DefaultDisposable : IDisposable
	{
		/// <summary>
		/// Singleton default disposable.
		/// </summary>
		public static readonly DefaultDisposable Instance = new DefaultDisposable();
		
		/// <summary>
		/// Initializes a new instance of the class <see cref="DefaultDisposable"/>.
		/// </summary>
		private DefaultDisposable()
		{
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
