using System;
using System.Threading;

namespace UriShell.Extensions
{
	/// <summary>
	/// Методы расширения для <see cref="Exception"/>.
	/// </summary>
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Determines whether the exception is critical for running the program.
		/// </summary>
		/// <param name="exception">Exception that is used for check.</param>
		/// <returns><see langword="true"/>, if the exception is critical; otherwise, <see langword="false"/>.</returns>
		public static bool IsCritical(this Exception exception)
		{
			if (exception is StackOverflowException
				||
				exception is OutOfMemoryException
				||
				exception is ThreadAbortException
				||
				exception is AccessViolationException)
			{
				return true;
			}

			return false;
		}
	}
}
