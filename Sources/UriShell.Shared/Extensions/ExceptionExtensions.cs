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
		/// Определяет, является ли исключение критическим для дальнейшего исполнения.
		/// </summary>
		/// <param name="exception">Исключение, для которого определяется критичность.</param>
		/// <returns><see langword="true"/>, если исключение является критическим; иначе, <see langword="false"/>.</returns>
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
