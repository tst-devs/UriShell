using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	[TestClass]
	public class ResolveSetupTests
	{
		private Uri _uri;
		private IShellResolveOpen _resolveOpen;
		private TraceListener _traceListener;
		
		[TestInitialize]
		public void Initialize()
		{
			this._uri = new Uri("tst://placement/module/item");
            this._resolveOpen = Substitute.For<IShellResolveOpen>();
			this._traceListener = Substitute.For<TraceListener>();

			Trace.Listeners.Add(this._traceListener);
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
				p => p(this._uri, passedToPlayer)));

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
				p => p(this._uri, passedToPlayer)));

			setup.OnReady(action).OpenOrThrow();

			Assert.AreEqual(1, onReadyCount);
			Assert.AreEqual(passedToPlayer, receivedInOnReady);
		}

		[TestMethod]
		public void DoesntInvokeOnFinishedWhenOpen()
		{
			var wasCalled = false;

			var action = new Action<object>(o => wasCalled = true);
			var setup = new ResolveSetup<object>(
				new ResolveSetupArgs(this._resolveOpen, p => p(this._uri, new object())));

			setup.OnFinished(action).Open();

			Assert.IsFalse(wasCalled);
		}

		[TestMethod]
		public void DoesntInvokeOnFinishedWhenOpenOrThrow()
		{
			var wasCalled = false;

			var action = new Action<object>(o => wasCalled = true);
			var setup = new ResolveSetup<object>(
				new ResolveSetupArgs(this._resolveOpen, p => p(this._uri, new object())));

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
					var disposable = p(this._uri, passedToPlayer);
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
					var disposable = p(this._uri, passedToPlayer);
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
				this._resolveOpen, p => p(this._uri, resolved)));

			var expectedTypeName = typeof(StringBuilder).Name;
			var resolvedTypeName = resolved.GetType().Name;

			setup.OnReady(_ => { }).Open();

			var calls = this._traceListener.ReceivedCalls();

			this._traceListener.Received(1).TraceEvent(
				Arg.Any<TraceEventCache>(),
				Arg.Any<string>(),
				TraceEventType.Warning,
				Arg.Any<int>(),
				Arg.Any<string>(),
				Arg.Is<object[]>(ps => ps.Contains(expectedTypeName) && ps.Contains(resolvedTypeName)));
		}

		[TestMethod]
		public void LogsWhenResolvedIncompatibleOnOpenOrThrow()
		{
			var resolved = new List<int>();

			var setup = new ResolveSetup<StringBuilder>(new ResolveSetupArgs(
				this._resolveOpen, p => p(this._uri, resolved)));

			var expectedTypeName = typeof(StringBuilder).Name;
			var resolvedTypeName = resolved.GetType().Name;

			setup.OnReady(_ => { }).OpenOrThrow();

			this._traceListener.Received(1).TraceEvent(
				Arg.Any<TraceEventCache>(),
				Arg.Any<string>(),
				TraceEventType.Warning,
				Arg.Any<int>(),
				Arg.Any<string>(),
				Arg.Is<object[]>(ps => ps.Contains(expectedTypeName) && ps.Contains(resolvedTypeName)));
		}
	}
}
