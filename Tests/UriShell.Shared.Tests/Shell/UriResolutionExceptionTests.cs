using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UriShell.Shell
{
	[TestClass]
	public class UriResolutionExceptionTests
	{
		[TestMethod]
		public void DeserializesSerializedUri()
		{
			var uri = new Uri("http://web.com");
			var exception = new UriResolutionException(uri, "test message");

			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, exception);

				stream.Seek(0, SeekOrigin.Begin);
				exception = (UriResolutionException)formatter.Deserialize(stream);
			}

			Assert.AreEqual(uri, exception.Uri);
		}

		[TestMethod]
		public void IncludesUriInMessage()
		{
			var uri = new Uri("http://web.com");
			var exception = new UriResolutionException(uri, "test message");

			StringAssert.Contains(exception.Message, uri.ToString());
		}
	}
}
