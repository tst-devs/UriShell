using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

namespace UriShell.Shell.Resolution
{
	[TestClass]
	public class UriResolvedMetadataTests
	{
		[TestMethod]
		public void AssignIdReturnsTheSameMetadataWithTheSpecifiedId()
		{
			var metadata1 = new UriResolvedMetadata(new Uri("about:blank"), Substitute.For<IDisposable>());
			var metadata2 = metadata1.AssignId(1005);

			Assert.AreEqual(1005, metadata2.ResolvedId);
			Assert.AreEqual(metadata1.Disposable, metadata2.Disposable);
			Assert.AreEqual(metadata1.Uri, metadata2.Uri);
		}
	}
}
