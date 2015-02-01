using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

namespace UriShell.Shell
{
	[TestClass]
	public class UriModuleItemResolverKeyTests
	{
		[TestMethod]
		public void EqualsIndependentOfCase()
		{
			var module = "Module";
			var item = "Item";

			var sampleKey = new UriModuleItemResolverKey(module, item);
			var upperKey = new UriModuleItemResolverKey(module.ToUpper(), item.ToUpper());
			var lowerKey = new UriModuleItemResolverKey(module.ToLower(), item.ToLower());

			Assert.AreEqual(sampleKey, upperKey);
			Assert.AreEqual(sampleKey.GetHashCode(), upperKey.GetHashCode());

			Assert.AreEqual(sampleKey, lowerKey);
			Assert.AreEqual(sampleKey.GetHashCode(), lowerKey.GetHashCode());

			Assert.AreEqual(upperKey, lowerKey);
			Assert.AreEqual(upperKey.GetHashCode(), lowerKey.GetHashCode());
		}

		[TestMethod]
		public void DoesntEqualForDifferentModules()
		{
			var key1 = new UriModuleItemResolverKey("module1", "item");
			var key2 = new UriModuleItemResolverKey("module2", "item");

			Assert.AreNotEqual(key1, key2);
		}

		[TestMethod]
		public void DoesntEqualForDifferentItems()
		{
			var key1 = new UriModuleItemResolverKey("module", "item1");
			var key2 = new UriModuleItemResolverKey("module", "item2");

			Assert.AreNotEqual(key1, key2);
		}
	}
}
