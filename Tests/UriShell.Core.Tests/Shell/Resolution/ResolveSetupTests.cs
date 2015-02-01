using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using UriShell.Logging;
using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	[TestClass]
	public class ResolveSetupTests
	{
		private Uri _uri;
		private IShellResolveOpen _resolveOpen;
		private ILogSession _logSession;
		
		[TestInitialize]
		public void Initialize()
		{
			this._uri = new Uri("tst://placement/module/item");
			this._logSession = Substitute.For<ILogSession>();
            this._resolveOpen = Substitute.For<IShellResolveOpen>();
		}

		[TestMethod]
		public void DoesntInvokeCallbackWhenSetupOnReady()
		{
			var wasCalled = false;

			var setup = new ResolveSetup<string>(
				new ResolveSetupArgs(this._resolveOpen, _ => wasCalled = true));
            setup.OnReady(s => { });

			Assert.IsFalse(wasCalled);
		}

		[TestMethod]
		public void DoesntInvokeCallbackWhenSetupOnFinished()
		{
			var wasCalled = false;

			var setup = new ResolveSetup<string>(
				new ResolveSetupArgs(this._resolveOpen, _ => wasCalled = true));
			setup.OnFinished(s => { });

			Assert.IsFalse(wasCalled);
		}

		[TestMethod]
		public void InvokesAppropriateResolveOpenMethodOnOpen()
		{
			var setup = new ResolveSetup<string>(new ResolveSetupArgs(this._resolveOpen, _ => { }));
			setup.Open();

			this._resolveOpen.Received(1).Open();
		}

		[TestMethod]
		public void InvokesAppropriateResolveOpenMethodOnOpenOrThrow()
		{
			var setup = new ResolveSetup<string>(new ResolveSetupArgs(this._resolveOpen, _ => { }));
			setup.OpenOrThrow();

			this._resolveOpen.Received(1).OpenOrThrow();
		}

		[TestMethod]
		public void InvokesOnReadyOnceWhenOpen()
		{
			object receivedInOnReady = null;

			var passedToPlayer = new object();
			var onReadyCount = 0;

			var action = new Action<object>(
				o =>
				{
					receivedInOnReady = o;
					onReadyCount++;
				});
			var setup = new ResolveSetup<object>(new ResolveSetupArgs(
				this._resolveOpen,
				p => p(this._uri, passedToPlayer, this._logSession)));

			setup.OnReady(action).Open();

			Assert.AreEqual(1, onReadyCount);
			Assert.AreEqual(passedToPlayer, receivedInOnReady);
		}

		[TestMethod]
		public void InvokesOnReadyOnceWhenOpenOrThrow()
		{
			object receivedInOnReady = null;

			var passedToPlayer = new object();
			var onReadyCount = 0;

			var action = new Action<object>(
				o =>
				{
					receivedInOnReady = o;
					onReadyCount++;
				});
			var setup = new ResolveSetup<object>(new ResolveSetupArgs(
				this._resolveOpen,
				p => p(this._uri, passedToPlayer, this._logSession)));

			setup.OnReady(action).OpenOrThrow();

			Assert.AreEqual(1, onReadyCount);
			Assert.AreEqual(passedToPlayer, receivedInOnReady);
		}

		[TestMethod]
		public void DoesntInvokeOnFinishedWhenOpen()
		{
			// По непонятным причинам, после VS2012 Update2, при запуске тестов скопом,
			// вызов LogMessage не детектируется. Поэтому тест не проходит. Запуск
			// теста в индивидуальном порядке дает положительный результат. Следующие
			// две строки позволяют исправить проблему для скопа тестов.
			this._logSession.LogMessage(string.Empty, LogCategory.Warning);
			this._logSession.ClearReceivedCalls();
	
			var wasCalled = false;

			var action = new Action<object>(o => wasCalled = true);
			var setup = new ResolveSetup<object>(
				new ResolveSetupArgs(
					this._resolveOpen,
					p => p(this._uri, new object(), this._logSession)));

			setup.OnFinished(action).Open();

			Assert.IsFalse(wasCalled);
		}

		[TestMethod]
		public void DoesntInvokeOnFinishedWhenOpenOrThrow()
		{
			var wasCalled = false;

			var action = new Action<object>(o => wasCalled = true);
			var setup = new ResolveSetup<object>(
				new ResolveSetupArgs(
					this._resolveOpen,
					p => p(this._uri, new object(), this._logSession)));

			setup.OnFinished(action).OpenOrThrow();

			Assert.IsFalse(wasCalled);
		}

		[TestMethod]
		public void InvokesOnFinishedOnceWhenOpenDisposed()
		{
			object receivedInOnFinished = null;

			var passedToPlayer = new object();
			var onFinishedCount = 0;

			var action = new Action<object>(
				o =>
				{
					receivedInOnFinished = o;
					onFinishedCount++;
				});
			var setup = new ResolveSetup<object>(new ResolveSetupArgs(
				this._resolveOpen,
				p =>
				{
					var disposable = p(this._uri, passedToPlayer, this._logSession);
					disposable.Dispose();
				}));

			setup.OnFinished(action).Open();

			Assert.AreEqual(1, onFinishedCount);
			Assert.AreEqual(passedToPlayer, receivedInOnFinished);
		}

		[TestMethod]
		public void InvokesOnFinishedOnceWhenOpenOrThrowDisposed()
		{
			object receivedInOnFinished = null;

			var passedToPlayer = new object();
			var onFinishedCount = 0;

			var action = new Action<object>(
				o =>
				{
					receivedInOnFinished = o;
					onFinishedCount++;
				});
			var setup = new ResolveSetup<object>(new ResolveSetupArgs(
				this._resolveOpen,
				p =>
				{
					var disposable = p(this._uri, passedToPlayer, this._logSession);
					disposable.Dispose();
				}));

			setup.OnFinished(action).OpenOrThrow();

			Assert.AreEqual(1, onFinishedCount);
			Assert.AreEqual(passedToPlayer, receivedInOnFinished);
		}

		[TestMethod]
		public void LogsWhenResolvedIncompatibleOnOpen()
		{
			var resolved = new List<int>();

			var setup = new ResolveSetup<StringBuilder>(new ResolveSetupArgs(
				this._resolveOpen,
				p => p(this._uri, resolved, this._logSession)));

			setup.OnReady(_ => { }).Open();

			this._logSession.Received(1).LogMessage(
				Arg.Is<string>(s => s.Contains(typeof(StringBuilder).Name) && s.Contains(resolved.GetType().Name)),
				LogCategory.Warning);
		}

		[TestMethod]
		public void LogsWhenResolvedIncompatibleOnOpenOrThrow()
		{
			var resolved = new List<int>();

			var setup = new ResolveSetup<StringBuilder>(new ResolveSetupArgs(
				this._resolveOpen,
				p => p(this._uri, resolved, this._logSession)));

			setup.OnReady(_ => { }).OpenOrThrow();

			this._logSession.Received(1).LogMessage(
				Arg.Is<string>(s => s.Contains(typeof(StringBuilder).Name) && s.Contains(resolved.GetType().Name)),
				LogCategory.Warning);
		}
	}
}
