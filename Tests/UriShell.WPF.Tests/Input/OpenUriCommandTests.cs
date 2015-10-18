using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using UriShell.Input;
using UriShell.Shell.Registration;

namespace UriShell.Shell
{
	[TestClass]
	public class OpenUriCommandTests
	{
		IShell _shell;
		IShellResolve _shellResolve;
			
		[TestInitialize]
		public void Initialize()
		{
			if (Settings.Instance == null)
			{
				Settings.Initialize(b => { b.Scheme = "tst"; });
			}

			this._shellResolve = Substitute.For<IShellResolve>();
			
			this._shell = Substitute.For<IShell>();
			this._shell.Resolve(null).ReturnsForAnyArgs(this._shellResolve);
		}

		[TestMethod]
		public void DisallowsExecutionForNull()
		{
			var openUriCommand = new OpenUriCommand(this._shell);

			Assert.IsFalse(openUriCommand.CanExecute(null));
		}

		[TestMethod]
		public void AllowsExecutionForAnyUri()
		{
			var openUriCommand = new OpenUriCommand(this._shell);

			Assert.IsTrue(openUriCommand.CanExecute(new Uri("http://anysite.com")));
			Assert.IsTrue(openUriCommand.CanExecute(new Uri("tst://tab/contactchart/primary")));
			Assert.IsTrue(openUriCommand.CanExecute(new Uri("D:/Work/Phoenix/PhoenixSrc/Client")));
		}

		[TestMethod]
		public void AllowsExecutionForStringContainingUri()
		{
			var openUriCommand = new OpenUriCommand(this._shell);

			Assert.IsTrue(openUriCommand.CanExecute("http://anysiteinstring.com"));
			Assert.IsTrue(openUriCommand.CanExecute("tst://tab/contactchart/appearance"));
			Assert.IsTrue(openUriCommand.CanExecute("E:/Work/Phoenix256/PhoenixSrc/Client"));
		}

		[TestMethod]
		public void DisallowsExecutionForStringDoesnNotContainingAbsoluteUri()
		{
			var openUriCommand = new OpenUriCommand(this._shell);

			Assert.IsFalse(openUriCommand.CanExecute("string1"));
			Assert.IsFalse(openUriCommand.CanExecute("contactchart/appearance"));
			Assert.IsFalse(openUriCommand.CanExecute("/contactchart/appearance"));
		}

		[TestMethod]
		public void OpensShellUriAsIs()
		{
			var openUriCommand = new OpenUriCommand(this._shell);
			openUriCommand.Execute(new Uri("tst://tab/contactchart/tools"));

			this._shell.Received(1).Resolve(new Uri("tst://tab/contactchart/tools"));
			this._shellResolve.Received(1).Open();
		}

		[TestMethod]
		public void OpensStringContainingShellUriAsIs()
		{
			var openUriCommand = new OpenUriCommand(this._shell);
			openUriCommand.Execute("tst://external/arm/log");

			this._shell.Received(1).Resolve(new Uri("tst://external/arm/log"));
			this._shellResolve.Received(1).Open();
		}

		[TestMethod]
		public void OpensNonShellUriUsingArmOpen()
		{
			var openUriCommand = new OpenUriCommand(this._shell);
			openUriCommand.Execute(new Uri("E:/Tests/Opens/String/Contains Something"));

			var expectedUriString = string.Format(
				"tst://external/arm/open?fileName={0}",
				Uri.EscapeDataString("file:///E:/Tests/Opens/String/Contains Something"));

			this._shell.Received(1).Resolve(new Uri(expectedUriString));
			this._shellResolve.Received(1).Open();
		}

		[TestMethod]
		public void OpensStringContainingNonShellUriUsingArmOpen()
		{
			var openUriCommand = new OpenUriCommand(this._shell);
			openUriCommand.Execute("http://address-to-any-site.com/index.htm?data=091&p==145");

			var expectedUriString = string.Format(
				"tst://external/arm/open?fileName={0}",
				Uri.EscapeDataString("http://address-to-any-site.com/index.htm?data=091&p==145"));

			this._shell.Received(1).Resolve(new Uri(expectedUriString));
			this._shellResolve.Received(1).Open();
		}
	}
}
