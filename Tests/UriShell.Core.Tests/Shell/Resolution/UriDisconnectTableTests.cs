using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using UriShell.Tests;

namespace UriShell.Shell.Resolution
{
	[TestClass]
	public class UriDisconnectTableTests
	{
		[TestMethod]
		public void SetsPlacementConnectorForResolved()
		{
			var resolved = new object();

			var shell = Substitute.For<IShell>();
			shell.IsResolvedOpen(resolved).Returns(true);

			var table = new UriDisconnectTable();
			var connector = Substitute.For<IUriPlacementConnector>();

			table[resolved] = connector;

			var connectorFromTable = table[resolved];

			Assert.AreSame(connector, connectorFromTable);
		}

		[TestMethod]
		public void ThrowsExceptionWhenGettingPlacementConnectorAfterRemove()
		{
			var resolved = new object();

			var shell = Substitute.For<IShell>();
			shell.IsResolvedOpen(resolved).Returns(true);

			var table = new UriDisconnectTable();
			var connector = Substitute.For<IUriPlacementConnector>();

			table[resolved] = connector;
			table.Remove(resolved);

			ExceptionAssert.Throws<KeyNotFoundException>(
				ex => ex.Message.Equals(string.Format(Properties.Resources.UriResolvedNotRegisteredForDisconnect, resolved)),
				() => connector = table[resolved]);
		}

		[TestMethod]
		public void ThrowsExceptionWhenGettingPlacementConnectorForUnknownResolved()
		{
			var table = new UriDisconnectTable();
			IUriPlacementConnector connector = null;

			var resolved = "unknown123";

			ExceptionAssert.Throws<KeyNotFoundException>(
				ex => ex.Message.Equals(string.Format(Properties.Resources.UriResolvedNotRegisteredForDisconnect, resolved)),
				() => connector = table[resolved]);
		}

		[TestMethod]
		public void ThrowsExceptionWhenRemovingUnknownResolved()
		{
			var table = new UriDisconnectTable();

			var resolved = "unknown456";

			ExceptionAssert.Throws<KeyNotFoundException>(
				ex => ex.Message.Equals(string.Format(Properties.Resources.UriResolvedNotRegisteredForDisconnect, resolved)),
				() => table.Remove(resolved));
		}
	}
}
