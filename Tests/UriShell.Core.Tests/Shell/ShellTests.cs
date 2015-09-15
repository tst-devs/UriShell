using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using UriShell.Shell.Registration;
using UriShell.Shell.Resolution;
using UriShell.Tests;

namespace UriShell.Shell
{
	using ShellResolveFactory = Func<Uri, object[], IShellResolve>;

	[TestClass]
	public class ShellTests
	{
		private IUriResolvedObjectHolder _uriResolvedObjectHolder;

		private Shell CreateShell(
			ShellResolveFactory shellResolveFactory = null)
		{
			return new Shell(
				this._uriResolvedObjectHolder,
				shellResolveFactory ?? Substitute.For<ShellResolveFactory>());
		}

		[TestInitialize]
		public void Initialize()
		{
			if (Settings.Instance == null)
			{
				Settings.Initialize(b => { b.Scheme = "tst"; });
			}

			this._uriResolvedObjectHolder = Substitute.For<IUriResolvedObjectHolder>();
		}

        [TestMethod]
		public void ResolvesUriUsingShellResolveFactory()
		{
			Uri factoryUri = null;
			object[] factoryAttachments = null;

			var factory = new ShellResolveFactory(
				(uri, attachments) =>
				{
					factoryUri = uri;
					factoryAttachments = attachments;

					return Substitute.For<IShellResolve>();
				});

			var shell = this.CreateShell(shellResolveFactory: factory);
			shell.Resolve(new Uri("tst://tab/module/item"), 1, "2");

			Assert.AreEqual("tst://tab/module/item", factoryUri.OriginalString);
			CollectionAssert.AreEquivalent(new object[] { 1, "2", }, factoryAttachments);
		}
		
		[TestMethod]
		public void ReturnsResolvedIsOpenWhenContainedInUriResolvedObjectHolder()
		{
			var resolved = new object();

			var shell = this.CreateShell();
			shell.IsResolvedOpen(resolved);

			this._uriResolvedObjectHolder.Received().Contains(resolved);
		}

		[TestMethod]
		public void GetsResolvedIdFormUriResolvedObjectHolder()
		{
			var resolved = new object();
			var metadata = new UriResolvedMetadata(null, null).AssignId(1334);

			this._uriResolvedObjectHolder.Contains(resolved).Returns(true);
			this._uriResolvedObjectHolder.GetMetadata(resolved).Returns(metadata);

			var shell = this.CreateShell();
			var id = shell.GetResolvedId(resolved);

			Assert.AreEqual(metadata.ResolvedId, id);
		}

		[TestMethod]
		public void GetsResolvedUriFormUriResolvedObjectHolder()
		{
			var resolved = new object();
			var metadata = new UriResolvedMetadata(new Uri("tst://tab/module/item"), null);

			this._uriResolvedObjectHolder.Contains(resolved).Returns(true);
			this._uriResolvedObjectHolder.GetMetadata(resolved).Returns(metadata);

			var shell = this.CreateShell();
			var uri = shell.GetResolvedUri(resolved);

			Assert.AreEqual(metadata.Uri, uri);
		}

		[TestMethod]
		public void ClosesResolvedUsingMetadataDisposable()
		{
			var resolved = new object();
			var disposable = Substitute.For<IDisposable>();
			var metadata = new UriResolvedMetadata(new Uri("tst://tab/module/item"), disposable);

			this._uriResolvedObjectHolder.Contains(resolved).Returns(true);
			this._uriResolvedObjectHolder.GetMetadata(resolved).Returns(metadata);

			disposable
				.When(d => d.Dispose())
				.Do(_ => this._uriResolvedObjectHolder.Contains(resolved).Returns(false));

			var shell = this.CreateShell();
			shell.CloseResolved(resolved);

			disposable.Received(1).Dispose();
		}

		[TestMethod]
		public void ExposesUriModuleItemResolversInUriResolutionCustomization()
		{
			var shell = this.CreateShell();
            var expectedResolver = Substitute.For<IUriModuleItemResolver>();
            shell.AddUriModuleItemResolver(new UriModuleItemResolverKey("module", "item"), expectedResolver);

            IUriModuleItemResolver actualResolver1 = null;
            shell.ModuleItemResolvers.TryGetValue(new UriModuleItemResolverKey("othermodule", "otheritem"), out actualResolver1);
            Assert.IsNull(actualResolver1);

            IUriModuleItemResolver actualResolver2 = null;
            shell.ModuleItemResolvers.TryGetValue(new UriModuleItemResolverKey("module", "item"), out actualResolver2);
            Assert.AreEqual(expectedResolver, actualResolver2);
        }

        [TestMethod]
		public void ExposesUriPlacementResolversInUriResolutionCustomization()
		{
			var uriPlacementResolver1 = Substitute.For<IUriPlacementResolver>();
			var uriPlacementResolver2 = Substitute.For<IUriPlacementResolver>();

			var shell = this.CreateShell();
			shell.AddUriPlacementResolver(uriPlacementResolver1);
			shell.AddUriPlacementResolver(uriPlacementResolver2);

			CollectionAssert.AreEquivalent(
				new[] { uriPlacementResolver1, uriPlacementResolver2 },
				shell.PlacementResolvers.ToArray());
		}

		[TestMethod]
		public void DoesntAddTheSameUriPlacementResolverTwice()
		{
			var uriPlacementResolver = Substitute.For<IUriPlacementResolver>();

			var shell = this.CreateShell();
			shell.AddUriPlacementResolver(uriPlacementResolver);
			shell.AddUriPlacementResolver(uriPlacementResolver);

			Assert.AreEqual(1, shell.PlacementResolvers.Count());
		}

		[TestMethod]
		public void DoesntHoldStrongReferenceToUriPlacementResolvers()
		{
			var uriPlacementResolver1 = Substitute.For<IUriPlacementResolver>();
			var uriPlacementResolver2 = Substitute.For<IUriPlacementResolver>();
			var uriPlacementResolver3 = Substitute.For<IUriPlacementResolver>();

			var shell = this.CreateShell();
			shell.AddUriPlacementResolver(uriPlacementResolver1);
			shell.AddUriPlacementResolver(uriPlacementResolver2);
			shell.AddUriPlacementResolver(uriPlacementResolver3);

			Assert.AreEqual(3, shell.PlacementResolvers.Count());

			uriPlacementResolver1 = null;
			uriPlacementResolver2 = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();

			shell.Resolve(new Uri("tst://tab/module/item"));

			Assert.AreEqual(1, shell.PlacementResolvers.Count());

			GC.KeepAlive(uriPlacementResolver3);
		}

		[TestMethod]
		public void ReturnsNullWhenTryingToParseNonHyperlink()
		{
			var shell = this.CreateShell();
			var source = "hello world";

			Assert.IsNull(shell.TryParseHyperlink(source, 1));
		}

		[TestMethod]
		public void ReturnsNullWhenTryingToParseHyperlinkWithoutHref()
		{
			var shell = this.CreateShell();
			var source = "<a>hello world</a>";

			Assert.IsNull(shell.TryParseHyperlink(source, 1));
		}

		[TestMethod]
		public void ReturnsNullWhenTryingToParseParsesHyperlinkWithSingleQuotes()
		{
			var shell = this.CreateShell();
			var source = "<a href='tst://tab/web/browser'>hello world</a>";

			Assert.IsNull(shell.TryParseHyperlink(source, 1));
		}
		
		[TestMethod]
		public void OmitsOwnerIdWhenTryingToParseNonShellHyperlink()
		{
			var shell = this.CreateShell();
			var source = string.Format("<a href=\"http://ya.ru/logo.png\">hello world</a>");

			var hyperlink = shell.TryParseHyperlink(source, 3);

			Assert.AreEqual(new Uri("http://ya.ru/logo.png"), hyperlink.Uri);
		}

		[TestMethod]
		public void ParsesHyperlinkUriAndTextFromValidHypelink()
		{
			var shell = this.CreateShell();
			var source = string.Format("<a href=\"tst://tab/web/browser\">hello world</a>");

			var hyperlink = shell.TryParseHyperlink(source, 3);

			Assert.AreEqual("hello world", hyperlink.Text);
			Assert.AreEqual(new Uri("tst://tab:3/web/browser"), hyperlink.Uri);
		}

		[TestMethod]
		public void ParsesHyperlinkWithEmptyTitle()
		{
			var shell = this.CreateShell();
			var source = string.Format("<a href=\"tst://tab/web/browser\"></a>");

			Assert.IsNotNull(shell.TryParseHyperlink(source, 1));
		}

		[TestMethod]
		public void CreatesHyperlinkFromUri()
		{
			var uri = new Uri(string.Format(
				"tst://placement:1111/module/item?&title={0}&icon={1}",
				Uri.EscapeDataString("Test Title"),
				Uri.EscapeDataString("http://ya.ru/logo.png")));

			var shell = this.CreateShell();
			var hyperlink = shell.CreateHyperlink(uri);

			Assert.AreEqual("Test Title", hyperlink.Text);
			Assert.AreEqual(uri, hyperlink.Uri);
			Assert.AreEqual(new Uri("http://ya.ru/logo.png"), hyperlink.Icon);
		}

		[TestMethod]
		public void ThrowsWhenCreatesHyperlinkFromUriWithRelativeIcon()
		{
			var uri = new Uri(string.Format(
				"tst://placement:1111/module/item?&title={0}&icon={1}",
				Uri.EscapeDataString("Test Title"),
				Uri.EscapeDataString("/images/logo.png")));

			var shell = this.CreateShell();

			ExceptionAssert.Throws<ArgumentException>(
				() => shell.CreateHyperlink(uri));
		}
	}
}
