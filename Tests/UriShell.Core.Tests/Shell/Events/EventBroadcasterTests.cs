using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

namespace UriShell.Shell.Events
{
	[TestClass]
	public class EventBroadcasterTests
	{
		private EventBroadcaster _eventBroadcaster;

		private IShell _shell;

		private EventKey _testKey;

		[TestInitialize]
		public void Initialize()
		{
			this._testKey = new EventKey();
			this._shell = Substitute.For<IShell>();
			this._eventBroadcaster = new EventBroadcaster(this._shell);
		}

		[TestMethod]
		public void SendCommonEventToAllReceivers()
		{
			var resolved1 = new TestResolved(10);
			var resolved2 = new TestResolved(20);
			var resolved3 = new TestResolved(30);
			var resolved4 = new TestResolved(40);
			var resolved5 = new TestResolved(50);

			this._shell.GetResolvedId(resolved1).Returns(resolved1.Id);
			this._shell.GetResolvedId(resolved2).Returns(resolved2.Id);
			this._shell.GetResolvedId(resolved3).Returns(resolved3.Id);

			this._shell.IsResolvedOpen(null).ReturnsForAnyArgs(true);

			resolved1.Subscribe(this._eventBroadcaster, this._testKey);
			resolved2.Subscribe(this._eventBroadcaster, this._testKey);
			resolved3.Subscribe(this._eventBroadcaster, this._testKey);
			resolved4.Subscribe(this._eventBroadcaster, ShellEventKeys.OneSecondElapsed);
			resolved5.Subscribe(this._eventBroadcaster, ShellEventKeys.OneSecondElapsed);

			this._eventBroadcaster.Send(this._testKey);
			this._eventBroadcaster.Send(this._testKey);
			this._eventBroadcaster.Send(this._testKey);
			this._eventBroadcaster.Send(this._testKey);
			this._eventBroadcaster.Send(this._testKey);

			this._eventBroadcaster.Send(ShellEventKeys.OneSecondElapsed, Arg.Any<OneSecondElapsedBroadcastArgs>());
			this._eventBroadcaster.Send(ShellEventKeys.OneSecondElapsed, Arg.Any<OneSecondElapsedBroadcastArgs>());
			this._eventBroadcaster.Send(ShellEventKeys.OneSecondElapsed, Arg.Any<OneSecondElapsedBroadcastArgs>());

			Assert.AreEqual(5, resolved1.RegisteredCalls);
			Assert.AreEqual(5, resolved2.RegisteredCalls);
			Assert.AreEqual(5, resolved3.RegisteredCalls);
			Assert.AreEqual(3, resolved4.RegisteredCalls);
			Assert.AreEqual(3, resolved5.RegisteredCalls);
		}

		[TestMethod]
		public void SendRefreshEventToOneReceiverOnly()
		{
			var resolved1 = new TestResolved(10);
			var resolved2 = new TestResolved(20);

			this._shell.GetResolvedId(resolved1).Returns(resolved1.Id);
			this._shell.GetResolvedId(resolved2).Returns(resolved2.Id);

			this._shell.IsResolvedOpen(null).ReturnsForAnyArgs(true);

			resolved1.Subscribe(this._eventBroadcaster, ShellEventKeys.RefreshResolved);
			resolved2.Subscribe(this._eventBroadcaster, ShellEventKeys.RefreshResolved);

			this._eventBroadcaster.Send(ShellEventKeys.RefreshResolved, new ResolvedIdBroadcastArgs(resolved1.Id));
			this._eventBroadcaster.Send(ShellEventKeys.RefreshResolved, new ResolvedIdBroadcastArgs(resolved1.Id));
			this._eventBroadcaster.Send(ShellEventKeys.RefreshResolved, new ResolvedIdBroadcastArgs(resolved1.Id));

			this._eventBroadcaster.Send(ShellEventKeys.RefreshResolved, new ResolvedIdBroadcastArgs(resolved2.Id));
			this._eventBroadcaster.Send(ShellEventKeys.RefreshResolved, new ResolvedIdBroadcastArgs(resolved2.Id));

			this._eventBroadcaster.Send(ShellEventKeys.RefreshResolved, new ResolvedIdBroadcastArgs(100));
			this._eventBroadcaster.Send(ShellEventKeys.RefreshResolved, new ResolvedIdBroadcastArgs(101));
			this._eventBroadcaster.Send(ShellEventKeys.RefreshResolved, new ResolvedIdBroadcastArgs(102));

			Assert.AreEqual(3, resolved1.RegisteredCalls);
			Assert.AreEqual(2, resolved2.RegisteredCalls);
		}

		[TestMethod]
		public void EnsuresUnsubscribeOnDispose()
		{
			var calls = 0;
			var timerCalls = 0;

			Action handler = () => calls++;
			Action<OneSecondElapsedBroadcastArgs> timerHandler = e => timerCalls++;

			var disposable = this._eventBroadcaster.Subscribe(this._testKey, handler);
			var timerDisposable = this._eventBroadcaster.Subscribe(ShellEventKeys.OneSecondElapsed, timerHandler);

			this._eventBroadcaster.Send(this._testKey);
			this._eventBroadcaster.Send(this._testKey);
			this._eventBroadcaster.Send(this._testKey);

			disposable.Dispose();

			this._eventBroadcaster.Send(this._testKey);
			this._eventBroadcaster.Send(this._testKey);
			this._eventBroadcaster.Send(this._testKey);

			this._eventBroadcaster.Send(ShellEventKeys.OneSecondElapsed, Arg.Any<OneSecondElapsedBroadcastArgs>());
			this._eventBroadcaster.Send(ShellEventKeys.OneSecondElapsed, Arg.Any<OneSecondElapsedBroadcastArgs>());
			this._eventBroadcaster.Send(ShellEventKeys.OneSecondElapsed, Arg.Any<OneSecondElapsedBroadcastArgs>());
			this._eventBroadcaster.Send(ShellEventKeys.OneSecondElapsed, Arg.Any<OneSecondElapsedBroadcastArgs>());

			timerDisposable.Dispose();

			this._eventBroadcaster.Send(ShellEventKeys.OneSecondElapsed, Arg.Any<OneSecondElapsedBroadcastArgs>());
			this._eventBroadcaster.Send(ShellEventKeys.OneSecondElapsed, Arg.Any<OneSecondElapsedBroadcastArgs>());

			Assert.AreEqual(3, calls);
			Assert.AreEqual(4, timerCalls);
		}

		/// <summary>
		/// Вспомогательный класс для регистрации вызовов.
		/// </summary>
		private sealed class TestResolved
		{
			public TestResolved(int id)
			{
				this.Id = id;
			}

			public void Subscribe<TEventArgs>(IEventBroadcaster broadcaster, EventKey<TEventArgs> eventKey)
			{
				broadcaster.Subscribe(eventKey, this.Handle);
			}

			public void Subscribe(IEventBroadcaster broadcaster, EventKey eventKey)
			{
				broadcaster.Subscribe(eventKey, this.Handle);
			}

			private void Handle<TEventArgs>(TEventArgs e)
			{
				this.RegisteredCalls++;
			}

			private void Handle()
			{
				this.RegisteredCalls++;
			}

			public int RegisteredCalls
			{
				get;
				private set;
			}

			public int Id
			{
				get;
				private set;
			}
		}
	}
}
