using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UriShell.Shell.Connectors
{
	[TestClass]
	public class SidebarActivityHistoryIdTests
	{
		[TestMethod]
		public void AcceptsNoKeys()
		{
			var activity1 = new SidebarActivityHistoryId(Enumerable.Empty<object>());
			var activity2 = new SidebarActivityHistoryId(Enumerable.Empty<object>());

			Assert.AreEqual(activity1, activity2);
			Assert.AreEqual(activity1.GetHashCode(), activity2.GetHashCode());
		}

		[TestMethod]
		public void EqualsIndependentOfKeyOrder()
		{
			object key1 = 100;
			object key2 = 455;
			object key3 = 780;

			var activity1 = new SidebarActivityHistoryId(new[] { key1, key2, key3 });
			var activity2 = new SidebarActivityHistoryId(new[] { key3, key1, key2 });

			Assert.AreEqual(activity1, activity2);
			Assert.AreEqual(activity1.GetHashCode(), activity2.GetHashCode());
		}

		[TestMethod]
		public void DoesntEqualsWithDifferentItemCount()
		{
			object key1 = 100;
			object key2 = 200;
			object key3 = 600;

			var activity1 = new SidebarActivityHistoryId(new[] { key1, key2, key3 });
			var activity2 = new SidebarActivityHistoryId(new[] { key1, key2 });

			Assert.AreNotEqual(activity1, activity2);
		}

		[TestMethod]
		public void DoesntEqualsWithDifferentItems()
		{
			object key1 = 100;
			object key2 = 200;
			
			var activity1 = new SidebarActivityHistoryId(new[] { key1, key2, 3 });
			var activity2 = new SidebarActivityHistoryId(new[] { key1, key2, 4 });

			Assert.AreNotEqual(activity1, activity2);
		}
	}
}
