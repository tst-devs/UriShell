using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using UriShell.Shell;
using UriShell.Shell.Connectors;
using UriShell.Tests;

namespace UriShell.Shell.Connectors
{
	[TestClass]
	public class SidebarPlacementConnectorTests
	{
		private ISidebarPlacementKeySource _sidebarPlacementKeySource;
		private IScheduler _scheduler;
		private IViewModelViewMatcher _viewMatcher;
		private IConnectedDragDrop _connectedDragDrop;

		[TestInitialize]
		public void Initialize()
		{
			this._sidebarPlacementKeySource = Substitute.For<ISidebarPlacementKeySource>();
			this._scheduler = Substitute.For<IScheduler>();
			this._viewMatcher = Substitute.For<IViewModelViewMatcher>();
			this._connectedDragDrop = Substitute.For<IConnectedDragDrop>();
		}

		private SidebarPlacementConnector CreateTestObject(
			IScheduler scheduler = null,
			ItemsPlacementConnectorFlags flags = ItemsPlacementConnectorFlags.Default)
		{
			return new SidebarPlacementConnector(
				scheduler ?? this._scheduler,
				this._viewMatcher,
				this._connectedDragDrop,
				this._sidebarPlacementKeySource,
				flags);
		}

		[TestMethod]
		public void IsResponsibleForRefreshReturnsTrueByDefault()
		{
			var connector = new SidebarPlacementConnector(
				this._scheduler, 
				this._viewMatcher,
				this._connectedDragDrop, 
				this._sidebarPlacementKeySource);
			Assert.IsTrue(connector.IsResponsibleForRefresh);
		}

		[TestMethod]
		public void ExposesIsResponsibleForRefreshFromFlags()
		{
			var connector = this.CreateTestObject(
				flags: ItemsPlacementConnectorFlags.IsResponsibleForRefresh);
			Assert.IsTrue(connector.IsResponsibleForRefresh);

			connector = this.CreateTestObject(
				flags: ItemsPlacementConnectorFlags.ActivateOnConnect | ItemsPlacementConnectorFlags.IsResponsibleForRefresh);
			Assert.IsTrue(connector.IsResponsibleForRefresh);
		}

		[TestMethod]
		public void PostponesConnectAndDisconnectUsingScheduler()
		{
			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();

			var scheduleDisposables = new[]
			{
				Substitute.For<IDisposable>(),
				Substitute.For<IDisposable>(),
				Substitute.For<IDisposable>(),
				Substitute.For<IDisposable>(),
			};
			var counter = 0;

			this._scheduler
				.Schedule(Arg.Any<Action>(), null)
				.ReturnsForAnyArgs(_ => scheduleDisposables[counter++]);

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);
			connector.Connect(connected3);
			connector.Disconnect(connected2);

			foreach (var scheduleDisposable in scheduleDisposables.Take(counter - 2))
			{
				scheduleDisposable.Received(1).Dispose();
			}
			scheduleDisposables[counter - 1].DidNotReceive().Dispose();
			this._scheduler.ReceivedWithAnyArgs(4).Schedule(Arg.Any<Action>(), null);
		}

		[TestMethod]
		public void ExposesConnectedInConnected()
		{
			var connected1 = new object();
			var connected2 = new object();

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();

			Assert.IsTrue(connector.Connected.Contains(connected1));
			Assert.IsTrue(connector.Connected.Contains(connected2));
		}

		[TestMethod]
		public void RemovesDisconnectedFromConnected()
		{
			var connected1 = new object();
			var connected2 = new object();

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);
			connector.Disconnect(connected2);

			postponed();

			Assert.IsTrue(connector.Connected.Contains(connected1));
			Assert.IsFalse(connector.Connected.Contains(connected2));
		}

		[TestMethod]
		public void RaisesConnectedChangedOnConnectAndDisconnect()
		{
			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			var args = new List<ConnectedChangedEventArgs>();
			connector.ConnectedChanged += (sender, e) => args.Add(e);

			postponed();

			Assert.AreEqual(2, args.Count);

			Assert.AreEqual(1, args.Count(e => 
				e.Action == ConnectedChangedAction.Connect && e.Changed == connected1));
			Assert.AreEqual(1, args.Count(e =>
				e.Action == ConnectedChangedAction.Connect && e.Changed == connected2));

			args.Clear();

			connector.Connect(connected3);

			postponed();

			Assert.AreEqual(3, args.Count);

			Assert.AreEqual(1, args.Count(e =>
				e.Action == ConnectedChangedAction.Disconnect && e.Changed == connected1));
			Assert.AreEqual(1, args.Count(e =>
				e.Action == ConnectedChangedAction.Disconnect && e.Changed == connected2));
			Assert.AreEqual(1, args.Count(e =>
				e.Action == ConnectedChangedAction.Connect && e.Changed == connected3));
		}

		[TestMethod]
		public void ThrowsExceptionOnMoveConnected()
		{
			var connected1 = new object();
			var connected2 = new object();

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();

			ExceptionAssert.Throws<NotSupportedException>(() => connector.MoveConnected(connected1, 1));
		}

		[TestMethod]
		public void ExposesNewViewFromViewModelViewMatch()
		{
			var connected1 = new object();
			var connected2 = new object();

			var view1 = new object();
			var view2 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			var match2 = Substitute.For<IViewModelViewMatch>();

			match1.View.Returns(view1);
			match2.View.Returns(view2);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns(match2);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();

			Assert.IsTrue(connector.Views.Contains(view1));
			Assert.IsTrue(connector.Views.Contains(view2));
		}

		[TestMethod]
		public void ExposesExistingViewByKeyAndMatchToModel()
		{
			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();
			var connected4 = new object();
			var connected5 = new object();

			this._sidebarPlacementKeySource.GetKey(connected1).Returns(13);
			this._sidebarPlacementKeySource.GetKey(connected2).Returns(245);
			this._sidebarPlacementKeySource.GetKey(connected3).Returns(13);
			this._sidebarPlacementKeySource.GetKey(connected4).Returns(245);
			this._sidebarPlacementKeySource.GetKey(connected5).Returns(245);

			var view13 = new object();
			var view245 = new object();

			var match13 = Substitute.For<IViewModelViewMatch>();
			var match245 = Substitute.For<IViewModelViewMatch>();

			match13.View.Returns(view13);
			match13.SupportsModelChange.Returns(true);
			match13.IsMatchToModel(connected3).Returns(true);

			match245.View.Returns(view245);
			match245.SupportsModelChange.Returns(true);
			match245.IsMatchToModel(connected4).Returns(true);
			match245.IsMatchToModel(connected5).Returns(true);

			this._viewMatcher.Match(connected1).Returns(match13);
			this._viewMatcher.Match(connected2).Returns(match245);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();

			connector.Connect(connected4);

			postponed();
			
			connector.Connect(connected3);
			connector.Connect(connected5);

			postponed();

			this._viewMatcher.DidNotReceive().Match(connected3);
			this._viewMatcher.DidNotReceive().Match(connected4);
			this._viewMatcher.DidNotReceive().Match(connected5);

			match13.Received(1).ChangeModel(connected3);
			match245.Received(1).ChangeModel(connected4);
			match245.Received(1).ChangeModel(connected5);

			Assert.IsTrue(connector.Views.Contains(view13));
			Assert.IsTrue(connector.Views.Contains(view245));
		}

		[TestMethod]
		public void ExposesNewViewEverytimeWhenConnectedHasNoKey()
		{
			var connected = new object();
			
			var view1 = new object();
			var view2 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			var match2 = Substitute.For<IViewModelViewMatch>();

			match1.View.Returns(view1);
			match2.View.Returns(view2);

			var counter = 0;
			this._viewMatcher.Match(connected).Returns(_ => counter++ == 0 ? match1 : match2);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject(Scheduler.Immediate);
			connector.Connect(connected);

			Assert.IsTrue(connector.Views.Contains(view1));

			connector.Connect(connected);

			this._viewMatcher.Received(2).Match(connected);

			match1.DidNotReceiveWithAnyArgs().IsMatchToModel(null);
			match1.DidNotReceiveWithAnyArgs().ChangeModel(null);

			match2.DidNotReceiveWithAnyArgs().IsMatchToModel(null);
			match2.DidNotReceiveWithAnyArgs().ChangeModel(null);

			Assert.IsTrue(connector.Views.Contains(view2));
		}
		
		[TestMethod]
		public void ExposesNewViewEverytimeWhenMatchDoesntSupportModelChange()
		{
			var connected1 = new object();
			var connected2 = new object();

			this._sidebarPlacementKeySource.GetKey(connected1).Returns(0);
			this._sidebarPlacementKeySource.GetKey(connected2).Returns(0);

			var view1 = new object();
			var view2 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			var match2 = Substitute.For<IViewModelViewMatch>();

			match1.View.Returns(view1);
			match1.SupportsModelChange.Returns(false);

			match2.View.Returns(view2);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns(match2);

			ISidebarPlacementConnector connector = this.CreateTestObject(Scheduler.Immediate);
			connector.Connect(connected1);

			Assert.IsTrue(connector.Views.Contains(view1));

			connector.Connect(connected2);

			match1.DidNotReceiveWithAnyArgs().IsMatchToModel(null);
			match1.DidNotReceiveWithAnyArgs().ChangeModel(null);

			Assert.IsTrue(connector.Views.Contains(view2));
		}

		[TestMethod]
		public void ExposesNewViewEverytimeWhenMatchDoesntMatchToModel()
		{
			var connected1 = new object();
			var connected2 = new object();

			this._sidebarPlacementKeySource.GetKey(connected1).Returns(0);
			this._sidebarPlacementKeySource.GetKey(connected2).Returns(0);

			var view1 = new object();
			var view2 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			var match2 = Substitute.For<IViewModelViewMatch>();

			match1.View.Returns(view1);
			match1.SupportsModelChange.Returns(true);
			match1.IsMatchToModel(connected2).Returns(false);

			match2.View.Returns(view2);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns(match2);

			ISidebarPlacementConnector connector = this.CreateTestObject(Scheduler.Immediate);
			connector.Connect(connected1);

			Assert.IsTrue(connector.Views.Contains(view1));

			connector.Connect(connected2);

			match1.DidNotReceiveWithAnyArgs().ChangeModel(null);

			Assert.IsTrue(connector.Views.Contains(view2));
		}

		[TestMethod]
		public void ExposesViewsWhichMatchToModelInConnectedOrder()
		{
			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();
			var connected4 = new object();
			var connected5 = new object();
			var connected6 = new object();

			this._sidebarPlacementKeySource.GetKey(connected1).Returns(12);
			this._sidebarPlacementKeySource.GetKey(connected2).Returns(12);
			this._sidebarPlacementKeySource.GetKey(connected3).Returns(34);
			this._sidebarPlacementKeySource.GetKey(connected4).Returns(34);
			this._sidebarPlacementKeySource.GetKey(connected5).Returns(56);
			this._sidebarPlacementKeySource.GetKey(connected6).Returns(56);

			var view12 = Substitute.For<IViewModelViewMatch>();
			var view34 = Substitute.For<IViewModelViewMatch>();
			var view56 = Substitute.For<IViewModelViewMatch>();

			var match12 = Substitute.For<IViewModelViewMatch>();
			var match34 = Substitute.For<IViewModelViewMatch>();
			var match56 = Substitute.For<IViewModelViewMatch>();

			match12.View.Returns(view12);
			match12.SupportsModelChange.Returns(true);
			match12.IsMatchToModel(connected1).Returns(true);
			match12.IsMatchToModel(connected2).Returns(true);

			match34.View.Returns(view34);
			match34.SupportsModelChange.Returns(true);
			match34.IsMatchToModel(connected3).Returns(true);
			match34.IsMatchToModel(connected4).Returns(true);

			match56.View.Returns(view56);
			match56.SupportsModelChange.Returns(true);
			match56.IsMatchToModel(connected5).Returns(true);
			match56.IsMatchToModel(connected6).Returns(true);

			this._viewMatcher.Match(connected1).Returns(match12);
			this._viewMatcher.Match(connected3).Returns(match34);
			this._viewMatcher.Match(connected5).Returns(match56);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected3);

			postponed();

			var views = connector.Views.Cast<object>().ToArray();
			Assert.AreEqual(view12, views[0]);
			Assert.AreEqual(view34, views[1]);

			connector.Connect(connected4);
			connector.Connect(connected5);

			postponed();

			views = connector.Views.Cast<object>().ToArray();
			Assert.AreEqual(view34, views[0]);
			Assert.AreEqual(view56, views[1]);
			
			connector.Connect(connected2);
			connector.Connect(connected6);

			postponed();

			views = connector.Views.Cast<object>().ToArray();
			Assert.AreEqual(view12, views[0]);
			Assert.AreEqual(view56, views[1]);
		}

		[TestMethod]
		public void RaisesViewsCollectionChangedOnConnectAndDisconnect()
		{
			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();

			this._sidebarPlacementKeySource.GetKey(connected1).Returns(1);
			this._sidebarPlacementKeySource.GetKey(connected2).Returns(23);
			this._sidebarPlacementKeySource.GetKey(connected3).Returns(23);

			var view1a = new object();
			var view1b = new object();
			var view23 = new object();

			var match1a = Substitute.For<IViewModelViewMatch>();
			var match1b = Substitute.For<IViewModelViewMatch>();
			var match23 = Substitute.For<IViewModelViewMatch>();

			match1a.View.Returns(view1a);
			match1b.View.Returns(view1b);
			
			match23.View.Returns(view23);
			match23.SupportsModelChange.Returns(true);
			match23.IsMatchToModel(connected3).Returns(true);

			var counter = 0;
			this._viewMatcher.Match(connected1).Returns(_ => counter++ == 0 ? match1a : match1b);
			this._viewMatcher.Match(connected2).Returns(match23);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			var args = new List<NotifyCollectionChangedEventArgs>();
			connector.Views.CollectionChanged += (sender, e) => args.Add(e);

			postponed();

			Assert.AreEqual(2, args.Count);

			Assert.AreEqual(1, args.Count(e =>
				e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Contains(view1a)));
			Assert.AreEqual(1, args.Count(e =>
				e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Contains(view23)));

			args.Clear();

			connector.Connect(connected1);
			connector.Connect(connected3);

			postponed();

			Assert.AreEqual(2, args.Count);

			Assert.AreEqual(1, args.Count(e =>
				e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Contains(view1a)));
			Assert.AreEqual(1, args.Count(e =>
				e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Contains(view1b)));
		}

		[TestMethod]
		public void ExposesAssignedActiveAndItsView()
		{
			var connected1 = new object();
			var connected2 = new object();

			var view1 = new object();
			var view2 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			var match2 = Substitute.For<IViewModelViewMatch>();

			match1.View.Returns(view1);
			match2.View.Returns(view2);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns(match2);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();

			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();

			Assert.AreEqual(connected1, connector.Active);
			Assert.AreEqual(view1, connector.Views.CurrentItem);
			Assert.AreEqual(0, connector.Views.CurrentPosition);

			connector.Active = connected2;

			Assert.AreEqual(connected2, connector.Active);
			Assert.AreEqual(view2, connector.Views.CurrentItem);
			Assert.AreEqual(1, connector.Views.CurrentPosition);

			connector.Active = null;

			Assert.IsNull(connector.Active);
			Assert.IsNull(connector.Views.CurrentItem);
			Assert.AreEqual(-1, connector.Views.CurrentPosition);
		}

		[TestMethod]
		public void RaisesActiveChangedWhenActiveAssigned()
		{
			var connected1 = new object();
			var connected2 = new object();

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			
			ActiveChangedEventArgs args = null;
			connector.ActiveChanged += (sender, e) => args = e;

			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();

			Assert.IsNull(args.OldValue);
			Assert.AreEqual(connected1, args.NewValue);

			connector.Active = connected2;

			Assert.AreEqual(connected1, args.OldValue);
			Assert.AreEqual(connected2, args.NewValue);

			connector.Active = null;

			Assert.AreEqual(connected2, args.OldValue);
			Assert.IsNull(args.NewValue);
		}

		[TestMethod]
		public void DoesntRaiseActiveChangedWhenTheSameActiveAssigned()
		{
			var connected = new object();

			ISidebarPlacementConnector connector = this.CreateTestObject(Scheduler.Immediate);
			connector.Connect(connected);

			var wasRaised = false;
			connector.ActiveChanged += (sender, e) => wasRaised = true;

			connector.Active = connected;

			Assert.IsFalse(wasRaised);
		}

		[TestMethod]
		public void ActivatesConnectedWhenViewsCurrentChanged()
		{
			var connected1 = new object();
			var connected2 = new object();

			var view1 = new object();
			var view2 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			var match2 = Substitute.For<IViewModelViewMatch>();

			match1.View.Returns(view1);
			match2.View.Returns(view2);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns(match2);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();

			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();

			ActiveChangedEventArgs args = null;
			connector.ActiveChanged += (sender, e) => args = e;

			connector.Views.MoveCurrentTo(view2);
			Assert.AreEqual(connected2, connector.Active);
			Assert.AreEqual(connected1, args.OldValue);
			Assert.AreEqual(connected2, args.NewValue);

			connector.Views.MoveCurrentTo(view1);
			Assert.AreEqual(connected1, connector.Active);
			Assert.AreEqual(connected2, args.OldValue);
			Assert.AreEqual(connected1, args.NewValue);

			connector.Views.MoveCurrentTo(null);
			Assert.IsNull(connector.Active);
			Assert.AreEqual(connected1, args.OldValue);
			Assert.IsNull(args.NewValue);
		}

		[TestMethod]
		public void RestoresActivityWhenConnectedSequencesHasEqualsKeys()
		{
			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();
			var connected4 = new object();
			var connected5 = new object();
			var connected6 = new object();

			this._sidebarPlacementKeySource.GetKey(connected1).Returns(12);
			this._sidebarPlacementKeySource.GetKey(connected2).Returns(12);
			this._sidebarPlacementKeySource.GetKey(connected3).Returns(34);
			this._sidebarPlacementKeySource.GetKey(connected4).Returns(34);
			this._sidebarPlacementKeySource.GetKey(connected5).Returns(56);
			this._sidebarPlacementKeySource.GetKey(connected6).Returns(56);

			var match12 = Substitute.For<IViewModelViewMatch>();
			var match34 = Substitute.For<IViewModelViewMatch>();
			var match56 = Substitute.For<IViewModelViewMatch>();

			match12.View.Returns(new object());
			match12.SupportsModelChange.Returns(true);
			match12.IsMatchToModel(connected1).Returns(true);
			match12.IsMatchToModel(connected2).Returns(true);

			match34.View.Returns(new object());
			match34.SupportsModelChange.Returns(true);
			match34.IsMatchToModel(connected3).Returns(true);
			match34.IsMatchToModel(connected4).Returns(true);

			match56.View.Returns(new object());
			match56.SupportsModelChange.Returns(true);
			match56.IsMatchToModel(connected5).Returns(true);
			match56.IsMatchToModel(connected6).Returns(true);

			this._viewMatcher.Match(connected1).Returns(match12);
			this._viewMatcher.Match(connected3).Returns(match34);
			this._viewMatcher.Match(connected5).Returns(match56);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected3);
			connector.Connect(connected5);

			postponed();

			connector.Active = connected3;

			connector.Connect(connected2);
			connector.Connect(connected4);
			connector.Connect(connected6);

			postponed();

			Assert.AreEqual(connected4, connector.Active);

			connector.Active = connected2;

			connector.Connect(connected1);
			connector.Connect(connected3);
			connector.Connect(connected5);

			postponed();

			Assert.AreEqual(connected1, connector.Active);
		}

		[TestMethod]
		public void ExposesConnectedAsViewWhenFactoryReturnsNull()
		{
			var connected1 = new object();
			var connected2 = new object();

			var view1 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			match1.View.Returns(view1);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns((IViewModelViewMatch)null);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();

			Assert.IsFalse(connector.Views.Contains(connected1));
			Assert.IsTrue(connector.Views.Contains(connected2));
		}

		[TestMethod]
		public void ReturnsIsPlacementEmptyTrueWhenConnectedIsEmpty()
		{
			var connector = this.CreateTestObject();
			Assert.IsTrue(connector.IsPlacementEmpty);
		}

		[TestMethod]
		public void ReturnsIsPlacementEmptyFalseWhenConnectedIsNotEmpty()
		{
			var connector = this.CreateTestObject(Scheduler.Immediate);
			connector.Connect(new object());

			Assert.IsFalse(connector.IsPlacementEmpty);
		}

		[TestMethod]
		public void ReturnsIsPlacementEmptyTrueAfterAllDisconnected()
		{
			var connected = new object();

			var connector = this.CreateTestObject(Scheduler.Immediate);
			connector.Connect(connected);
			connector.Disconnect(connected);

			Assert.IsTrue(connector.IsPlacementEmpty);
		}

		[TestMethod]
		public void AvoidsUIEffectWhileConnecting()
		{
			// Задача теста: убедиться, что внешнее влияние на Views
			// в процессе вызова Connect игнорируется. Почему это важно?
			// Ряд контролов WPF, унаследованных от Selector, форсируют
			// выбор добавленного объекта, когда SelectedItem содержит null.
			// Это приводит к неожиданным событиям OnActiveChanged, а также
			// может нарушить синхронизацию между потребителями коннектора
			// и пользовательским интерфейсом.

			var connected1 = new object();
			var connected2 = new object();

			var view1 = new object();
			var view2 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			var match2 = Substitute.For<IViewModelViewMatch>();

			match1.View.Returns(view1);
			match2.View.Returns(view2);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns(match2);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			
			connector.Views.CollectionChanged += (sender, e) =>
			{
				((ICollectionView)sender).MoveCurrentTo(view2);
			};
			connector.ActiveChanged += (sender, e) =>
			{
				Assert.AreEqual(connected1, e.NewValue);
			};

			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();
		}

		[TestMethod]
		public void AvoidsUIEffectWhileDisconnecting()
		{
			// О целях данного теста, см. AvoidsUIEffectWhileConnecting.

			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();

			var view1 = new object();
			var view2 = new object();
			var view3 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			var match2 = Substitute.For<IViewModelViewMatch>();
			var match3 = Substitute.For<IViewModelViewMatch>();

			match1.View.Returns(view1);
			match2.View.Returns(view2);
			match3.View.Returns(view3);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns(match2);
			this._viewMatcher.Match(connected3).Returns(match3);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);
			connector.Connect(connected3);

			postponed();

			connector.Views.CollectionChanged += (sender, e) =>
			{
				((ICollectionView)sender).MoveCurrentTo(view2);
			};
			connector.ActiveChanged += (sender, e) =>
			{
				Assert.IsNull(e.NewValue);
			};

			postponed();
		}

		[TestMethod]
		public void RaisesActiveChangedAfterConnectedChangedWhenConnected()
		{
			ISidebarPlacementConnector connector = this.CreateTestObject(Scheduler.Immediate);
			
			var connectedChangedRaised = false;
			connector.ConnectedChanged += (sender, e) => connectedChangedRaised = true;
			connector.ActiveChanged += (sender, e) => Assert.IsTrue(connectedChangedRaised);

			connector.Connect(new object());
		}

		[TestMethod]
		public void RaisesActiveChangedBeforeConnectedChangedWhenDisconnected()
		{
			var connected1 = new object();
			var connected2 = new object();

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			ISidebarPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();

			connector.Active = connected2;

			var connectedChangedRaised = false;
			connector.ConnectedChanged += (sender, e) =>
			{
				if (e.Action == ConnectedChangedAction.Disconnect)
				{
					connectedChangedRaised = true;
				}
			};
			connector.ActiveChanged += (sender, e) => Assert.IsFalse(connectedChangedRaised);

			connector.Connect(connected1);

			postponed();
		}

		[TestMethod]
		public void ReturnsConnectedFromView()
		{
			var connected1 = new object();
			var connected2 = new object();

			var view1 = new object();
			var view2 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			var match2 = Substitute.For<IViewModelViewMatch>();

			match1.View.Returns(view1);
			match2.View.Returns(view2);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns(match2);

			Action postponed = null;

			this._scheduler
				.WhenForAnyArgs(s => s.Schedule(Arg.Any<Action>(), null))
				.Do(ci => postponed = ci.Arg<Action>());

			var connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			postponed();

			Assert.AreEqual(connected1, connector.ConnectedFromView(view1));
			Assert.AreEqual(connected2, connector.ConnectedFromView(view2));
		}

		[TestMethod]
		public void DisposesViewWithoutKeyOnDisconnect()
		{
			var connected = new object();
			var view = Substitute.For<IDisposable>();
			
			var match = Substitute.For<IViewModelViewMatch>();
			match.View.Returns(view);

			this._viewMatcher.Match(connected).Returns(match);

			ISidebarPlacementConnector connector = this.CreateTestObject(Scheduler.Immediate);
			connector.Connect(connected);

			view.DidNotReceive().Dispose();

			connector.Disconnect(connected);

			view.Received(1).Dispose();
		}

		[TestMethod]
		public void DoesntDisposeViewWithKeyAndMatchToModel()
		{
			var connected = new object();
			var view = Substitute.For<IDisposable>();

			this._sidebarPlacementKeySource.GetKey(connected).Returns(1);

			var match = Substitute.For<IViewModelViewMatch>();
			match.View.Returns(view);
			match.SupportsModelChange.Returns(true);

			this._viewMatcher.Match(connected).Returns(match);

			ISidebarPlacementConnector connector = this.CreateTestObject(Scheduler.Immediate);
			connector.Connect(connected);

			view.DidNotReceive().Dispose();

			connector.Disconnect(connected);

			view.DidNotReceive().Dispose();
		}

		[TestMethod]
		public void DisposesViewWithKeyAndMatchToModelOnDispose()
		{
			var connected = new object();
			var view = Substitute.For<IDisposable>();

			this._sidebarPlacementKeySource.GetKey(connected).Returns(1);

			var match = Substitute.For<IViewModelViewMatch>();
			match.View.Returns(view);
			match.SupportsModelChange.Returns(true);

			this._viewMatcher.Match(connected).Returns(match);

			var connector = this.CreateTestObject(Scheduler.Immediate);
			((ISidebarPlacementConnector)connector).Connect(connected);
			connector.Dispose();

			view.Received(1).Dispose();
		}

		[TestMethod]
		public void ThrowsExceptionOnAttemptToConnectFromDragDrop()
		{
			var connected = new object();

			this._connectedDragDrop.IsDragging(connected).Returns(true);

			var connector = this.CreateTestObject();
			ExceptionAssert.Throws<NotSupportedException>(() => connector.Connect(connected));
		}

		[TestMethod]
		public void ThrowsExceptionOnAttemptToDisconnectToDragDrop()
		{
			var connected = new object();

			var connector = this.CreateTestObject();
			connector.Connect(connected);

			this._connectedDragDrop.IsDragging(connected).Returns(true);

			ExceptionAssert.Throws<NotSupportedException>(() => connector.Disconnect(connected));
		}
	}
}