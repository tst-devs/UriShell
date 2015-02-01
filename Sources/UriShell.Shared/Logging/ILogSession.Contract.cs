using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.IO;

namespace UriShell.Logging
{
	[ContractClassFor(typeof(ILogSession))]
	internal abstract class ILogSessionContract : ILogSession
	{
		public ILogSession SupplyId(string subject)
		{
			Contract.Ensures(Contract.Result<ILogSession>() != null);

			return default(ILogSession);
		}

		public void LogMessage(string message)
		{
			Contract.Requires<ArgumentNullException>(message != null);
		}

		public void LogMessage(string message, LogCategory category)
		{
			Contract.Requires<ArgumentNullException>(message != null);
		}
		
		public void LogValue(string title, bool value)
		{
		}

		public void LogValue(string title, bool value, LogCategory category)
		{
		}

		public void LogValue(string title, byte value)
		{
		}

		public void LogValue(string title, byte value, LogCategory category)
		{
		}

		public void LogValue(string title, char value)
		{
		}

		public void LogValue(string title, char value, LogCategory category)
		{
		}

		public void LogValue(string title, int value)
		{
		}

		public void LogValue(string title, int value, LogCategory category)
		{
		}

		public void LogValue(string title, long value)
		{
		}

		public void LogValue(string title, long value, LogCategory category)
		{
		}

		public void LogValue(string title, double value)
		{
		}

		public void LogValue(string title, double value, LogCategory category)
		{
		}

		public void LogValue(string title, decimal value)
		{
		}

		public void LogValue(string title, decimal value, LogCategory category)
		{
		}

		public void LogValue(string title, string value)
		{
			Contract.Requires<ArgumentNullException>(value != null);
		}

		public void LogValue(string title, string value, LogCategory category)
		{
			Contract.Requires<ArgumentNullException>(value != null);
		}

		public void LogValue(string title, DateTime value)
		{
		}

		public void LogValue(string title, DateTime value, LogCategory category)
		{
		}

		public void LogValue(string title, object value)
		{
			Contract.Requires<ArgumentNullException>(value != null);
		}

		public void LogValue(string title, object value, LogCategory category)
		{
			Contract.Requires<ArgumentNullException>(value != null);
		}

		public void LogEnumerable(string title, IEnumerable list)
		{
			Contract.Requires<ArgumentNullException>(list != null);
		}

		public void LogEnumerable(string title, IEnumerable list, LogCategory category)
		{
			Contract.Requires<ArgumentNullException>(list != null);
		}

		public void LogDictionary(string title, IDictionary dictionary)
		{
			Contract.Requires<ArgumentNullException>(dictionary != null);
		}

		public void LogDictionary(string title, IDictionary dictionary, LogCategory category)
		{
			Contract.Requires<ArgumentNullException>(dictionary != null);
		}

		public void LogBinaryStream(string title, Stream stream)
		{
			Contract.Requires<ArgumentNullException>(stream != null);
		}

		public void LogBinaryStream(string title, Stream stream, LogCategory category)
		{
			Contract.Requires<ArgumentNullException>(stream != null);
		}

		public void LogTextStream(string title, Stream stream)
		{
			Contract.Requires<ArgumentNullException>(stream != null);
		}

		public void LogTextStream(string title, Stream stream, LogCategory category)
		{
			Contract.Requires<ArgumentNullException>(stream != null);
		}

		public void LogException(string title, Exception exception)
		{
			Contract.Requires<ArgumentNullException>(exception != null);
		}

		public void LogException(string title, Exception exception, LogCategory category)
		{
			Contract.Requires<ArgumentNullException>(exception != null);
		}

		public void WatchValue(string title, bool value)
		{
		}

		public void WatchValue(string title, bool value, LogCategory category)
		{
		}

		public void WatchValue(string title, byte value)
		{
		}

		public void WatchValue(string title, byte value, LogCategory category)
		{
		}

		public void WatchValue(string title, char value)
		{
		}

		public void WatchValue(string title, char value, LogCategory category)
		{
		}

		public void WatchValue(string title, int value)
		{
		}

		public void WatchValue(string title, int value, LogCategory category)
		{
		}

		public void WatchValue(string title, long value)
		{
		}

		public void WatchValue(string title, long value, LogCategory category)
		{
		}

		public void WatchValue(string title, double value)
		{
		}

		public void WatchValue(string title, double value, LogCategory category)
		{
		}

		public void WatchValue(string title, decimal value)
		{
		}

		public void WatchValue(string title, decimal value, LogCategory category)
		{
		}

		public void WatchValue(string title, string value)
		{
			Contract.Requires<ArgumentNullException>(value != null);
		}

		public void WatchValue(string title, string value, LogCategory category)
		{
			Contract.Requires<ArgumentNullException>(value != null);
		}

		public void WatchValue(string title, DateTime value)
		{
		}

		public void WatchValue(string title, DateTime value, LogCategory category)
		{
		}

		public void WatchValue(string title, object value)
		{
			Contract.Requires<ArgumentNullException>(value != null);
		}

		public void WatchValue(string title, object value, LogCategory category)
		{
			Contract.Requires<ArgumentNullException>(value != null);
		}
	}
}
