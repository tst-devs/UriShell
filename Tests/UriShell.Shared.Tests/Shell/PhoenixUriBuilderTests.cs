using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UriShell.Shell
{
	[TestClass]
	public class PhoenixUriBuilderTests
	{
		[TestMethod]
		public void ParsesUriWhenCreatedWithUri()
		{
			var builder = new PhoenixUriBuilder(new Uri("tst://testplacement:102/testmodule/testitem"));

			Assert.AreEqual("testplacement", builder.Placement);
			Assert.AreEqual(102, builder.OwnerId);
			Assert.AreEqual("testmodule", builder.Module);
			Assert.AreEqual("testitem", builder.Item);
		}

		[TestMethod]
		public void ParsesUriParametersWhenCreatedWithUri()
		{
			var builder = new PhoenixUriBuilder(new Uri("tst://testplacement:102/testmodule/testitem?p1=test Param1&p2=test Param2"));

			Assert.AreEqual(2, builder.Parameters.Count);

			Assert.AreEqual("test Param1", builder.Parameters["p1"]);
			Assert.AreEqual("test Param2", builder.Parameters["p2"]);
		}

		[TestMethod]
		public void BuildsUriWhenCreatedEmpty()
		{
			var builder = new PhoenixUriBuilder();

			builder.Placement = "newPlacement";
			builder.OwnerId = 405;
			builder.Module = "newModule";
			builder.Item = "newItem";

			Assert.AreEqual(new Uri("tst://newPlacement:405/newModule/newItem"), builder.Uri);
		}

		[TestMethod]
		public void BuildsUriWithParametersWhenCreatedEmpty()
		{
			var builder = new PhoenixUriBuilder();

			builder.Placement = "newPlacement";
			builder.OwnerId = 405;
			builder.Module = "newModule";
			builder.Item = "newItem";

			builder.Parameters.Set("n1", "v10");
			builder.Parameters.Set("n2", "v20");
			builder.Parameters.Set("n3", "\t");

			Assert.AreEqual(new Uri("tst://newPlacement:405/newModule/newItem?n1=v10&n2=v20&n3=%09"), builder.Uri);
		}

		[TestMethod]
		public void BuildsUriFluently()
		{
			var uri = PhoenixUriBuilder
				.StartUri()
				.Placement("fluentPlacement")
				.OwnerId(134)
				.Module("fluentModule")
				.Item("fluentItem")
				.End();

			Assert.AreEqual(new Uri("tst://fluentPlacement:134/fluentModule/fluentItem"), uri);
		}

		[TestMethod]
		public void BuildsUriWithParametersAndAttachmentsFluently()
		{
			var uri = PhoenixUriBuilder
				.StartUri()
				.Placement("fluentPlacement")
				.Module("fluentModule")
				.Item("fluentItem")
				.Parameter("p1", "v1")
				.Attachment("a1", 0)
				.Attachment("a2", 1)
				.End();

			Assert.AreEqual(new Uri("tst://fluentPlacement/fluentModule/fluentItem?p1=v1&a1={0}&a2={1}"), uri);
		}
	}
}
