﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Disposables
{
	/// <summary>
	/// Provides a set of static methods for creating Disposables.
	/// </summary>
	internal static partial class Disposable
	{
		/// <summary>
		/// Gets the disposable that does nothing when disposed.
		/// </summary>
		public static IDisposable Empty
		{
			get
			{
				return DefaultDisposable.Instance;
			}
		}
		/// <summary>
		/// Creates the disposable that invokes the specified action when disposed.
		/// </summary>
		/// <param name="dispose">Action to run during IDisposable.Dispose.</param>
		/// <returns>The disposable object that runs the given action upon disposal.</returns>
		public static IDisposable Create(Action dispose)
		{
			if (dispose == null)
			{
				throw new ArgumentNullException("dispose");
			}
			return new AnonymousDisposable(dispose);
		}
	}
}
