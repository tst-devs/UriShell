using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;
using UriShell.Shell.Resolution;

namespace UriShell.Shell.Connectors
{
	[TestClass]
	public class ItemsPlacementConnectorTests
	{
		private IViewModelViewMatcher _viewMatcher;
		private IConnectedDragDrop _connectedDragDrop;

		private ItemsPlacementConnector CreateTestObject(ItemsPlacementConnectorFlags flags = ItemsPlacementConnectorFlags.Default)
		{
			return new ItemsPlacementConnector(this._viewMatcher, this._connectedDragDrop, flags);
		}

		[TestInitialize]
		public void Initialize()
		{
			this._viewMatcher = Substitute.For<IViewModelViewMatcher>();
			this._connectedDragDrop = Substitute.For<IConnectedDragDrop>();
		}

		[TestMethod]
		public void IsResponsibleForRefreshReturnsFalseByDefault()
		{
			var connector = this.CreateTestObject();
			Assert.IsFalse(connector.IsResponsibleForRefresh);
		}

		[TestMethod]
		public void ExposesIsResponsibleForRefreshFromFlags()
		{
			var connector = this.CreateTestObject(
				ItemsPlacementConnectorFlags.IsResponsibleForRefresh);
			Assert.IsTrue(connector.IsResponsibleForRefresh);

			connector = this.CreateTestObject(
				ItemsPlacementConnectorFlags.ActivateOnConnect | ItemsPlacementConnectorFlags.IsResponsibleForRefresh);
			Assert.IsTrue(connector.IsResponsibleForRefresh);
		}

		[TestMethod]
		public void ExposesConnectedInConnected()
		{
			var connected1 = new object();
			var connected2 = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			Assert.IsTrue(connector.Connected.Contains(connected1));
			Assert.IsTrue(connector.Connected.Contains(connected2));
		}
		
		[TestMethod]
		public void RemovesDisconnectedFromConnected()
		{
			var connected1 = new object();
			var connected2 = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);
			connector.Disconnect(connected2);

			Assert.IsTrue(connector.Connected.Contains(connected1));
			Assert.IsFalse(connector.Connected.Contains(connected2));
		}

		[TestMethod]
		public void RaisesConnectedChangedOnConnectAndDisconnect()
		{
			var connected1 = new object();
			var connected2 = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();

			ConnectedChangedEventArgs args = null;
			connector.ConnectedChanged += (sender, e) => args = e;

			connector.Connect(connected1);
			Assert.AreEqual(ConnectedChangedAction.Connect, args.Action);
			Assert.AreEqual(connected1, args.Changed);

			connector.Connect(connected2);
			Assert.AreEqual(ConnectedChangedAction.Connect, args.Action);
			Assert.AreEqual(connected2, args.Changed);

			connector.Disconnect(connected1);
			Assert.AreEqual(ConnectedChangedAction.Disconnect, args.Action);
			Assert.AreEqual(connected1, args.Changed);
		}

		[TestMethod]
		public void MovesConnectedWithinConnected()
		{
			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);
			connector.Connect(connected3);

			connector.MoveConnected(connected1, 2);

			Assert.AreEqual(2, connector.Connected.ToList().IndexOf(connected1));
		}

		[TestMethod]
		public void RaisesConnectedChangedOnMoveConnected()
		{
			var connected1 = new object();
			var connected2 = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			ConnectedChangedEventArgs args = null;
			connector.ConnectedChanged += (sender, e) => args = e;

			connector.MoveConnected(connected1, 1);
			Assert.AreEqual(ConnectedChangedAction.Move, args.Action);
			Assert.AreEqual(connected1, args.Changed);

			connector.MoveConnected(connected2, 1);
			Assert.AreEqual(ConnectedChangedAction.Move, args.Action);
			Assert.AreEqual(connected2, args.Changed);
		}

		[TestMethod]
		public void DoesntRaiseConnectedChangedOnMoveConnectedToTheSameIndex()
		{
			var connected1 = new object();
			var connected2 = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			var wasRaised = false;
			connector.ConnectedChanged += (sender, e) => wasRaised = true;

			connector.MoveConnected(connected1, 0);
			Assert.IsFalse(wasRaised);

			connector.MoveConnected(connected2, 1);
			Assert.IsFalse(wasRaised);
		}
		
		[TestMethod]
		public void ExposesViewFromViewModelViewMatch()
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

			IItemsPlacementConnector connector = this.CreateTestObject();

			connector.Connect(connected1);
			Assert.IsTrue(connector.Views.Contains(view1));

			connector.Connect(connected2);
			Assert.IsTrue(connector.Views.Contains(view1));
			Assert.IsTrue(connector.Views.Contains(view2));
		}

		[TestMethod]
		public void RemovesDisconnectedViewFromViews()
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

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);
			connector.Disconnect(connected1);

			Assert.IsFalse(connector.Views.Contains(view1));
			Assert.IsTrue(connector.Views.Contains(view2));
		}

		[TestMethod]
		public void RaisesViewsCollectionChangedOnConnectAndDisconnect()
		{
			var connected = new object();
			var view = new object();
			var match = Substitute.For<IViewModelViewMatch>();

			match.View.Returns(view);
			this._viewMatcher.Match(connected).Returns(match);

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(new object()); // чтобы проверяемый индекс отличался от нуля.
			
			NotifyCollectionChangedEventArgs args = null;
			connector.Views.CollectionChanged += (sender, e) => args = e;

			connector.Connect(connected);

			Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
			Assert.AreEqual(1, args.NewStartingIndex);
			CollectionAssert.Contains(args.NewItems, view);

			connector.Disconnect(connected);

			Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
			Assert.AreEqual(1, args.OldStartingIndex);
			CollectionAssert.Contains(args.OldItems, view);
		}

		[TestMethod]
		public void MovesViewInRespectToConnected()
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

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			connector.MoveConnected(connected1, 1);

			Assert.AreEqual(1, connector.Views.Cast<object>().ToList().IndexOf(view1));
		}

		[TestMethod]
		public void RaisesViewsCollectionChangedOnMoveConnected()
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

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			NotifyCollectionChangedEventArgs args = null;
			connector.Views.CollectionChanged += (sender, e) => args = e;

			connector.MoveConnected(connected1, 1);

			Assert.AreEqual(NotifyCollectionChangedAction.Move, args.Action);
			Assert.AreEqual(1, args.NewStartingIndex);
			CollectionAssert.Contains(args.NewItems, view1);

			connector.MoveConnected(connected2, 1);

			Assert.AreEqual(NotifyCollectionChangedAction.Move, args.Action);
			Assert.AreEqual(1, args.NewStartingIndex);
			CollectionAssert.Contains(args.NewItems, view2);
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

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			Assert.IsNull(connector.Active);
			Assert.IsNull(connector.Views.CurrentItem);

			connector.Active = connected1;

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

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			ActiveChangedEventArgs args = null;
			connector.ActiveChanged += (sender, e) => args = e;

			connector.Active = connected1;

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

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected);

			connector.Active = connected;

			var wasRaised = false;
			connector.ActiveChanged += (sender, e) => wasRaised = true;

			connector.Active = connected;

			Assert.IsFalse(wasRaised);
		}

		[TestMethod]
		public void ActivatesNextWhenActiveDisconnectedAtTheMiddle()
		{
			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);
			connector.Connect(connected3);

			connector.Active = connected2;
			
			ActiveChangedEventArgs args = null;
			connector.ActiveChanged += (sender, e) => args = e;

			connector.Disconnect(connected2);

			Assert.AreEqual(connected3, connector.Active);
			Assert.AreEqual(connected2, args.OldValue);
			Assert.AreEqual(connected3, args.NewValue);
		}

		[TestMethod]
		public void ActivatesPreviousWhenActiveIsLastAndHaveDisconnected()
		{
			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);
			connector.Connect(connected3);

			connector.Active = connected3;

			ActiveChangedEventArgs args = null;
			connector.ActiveChanged += (sender, e) => args = e;

			connector.Disconnect(connected3);

			Assert.AreEqual(connected2, connector.Active);
			Assert.AreEqual(connected3, args.OldValue);
			Assert.AreEqual(connected2, args.NewValue);
		}

		[TestMethod]
		public void MakesActiveNullWhenActiveIsSingleAndHaveDisconnected()
		{
			var connected = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected);

			connector.Active = connected;

			ActiveChangedEventArgs args = null;
			connector.ActiveChanged += (sender, e) => args = e;

			connector.Disconnect(connected);

			Assert.IsNull(connector.Active);
			Assert.AreEqual(connected, args.OldValue);
			Assert.IsNull(args.NewValue);
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

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			ActiveChangedEventArgs args = null;
			connector.ActiveChanged += (sender, e) => args = e;

			connector.Views.MoveCurrentTo(view2);
			Assert.AreEqual(connected2, connector.Active);
			Assert.IsNull(args.OldValue);
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
		public void ActivatesOnConnectWhenActivateOnConnectFlagSet()
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

			IItemsPlacementConnector connector = this.CreateTestObject(ItemsPlacementConnectorFlags.ActivateOnConnect);
			
			connector.Connect(connected1);

			Assert.AreEqual(connected1, connector.Active);
			Assert.AreEqual(view1, connector.Views.CurrentItem);

			connector.Connect(connected2);

			Assert.AreEqual(connected2, connector.Active);
			Assert.AreEqual(view2, connector.Views.CurrentItem);
		}

		[TestMethod]
		public void ExposesConnectedAsViewWhenViewModelHasNoMatchedView()
		{
			var connected1 = new object();
			var connected2 = new object();

			var view1 = new object();

			var match1 = Substitute.For<IViewModelViewMatch>();
			match1.View.Returns(view1);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns((IViewModelViewMatch)null);

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

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
			var connector = this.CreateTestObject();
			connector.Connect(new object());

			Assert.IsFalse(connector.IsPlacementEmpty);
		}

		[TestMethod]
		public void ReturnsIsPlacementEmptyTrueAfterAllDisconnected()
		{
			var connected1 = new object();
			var connected2 = new object();

			var connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);
			
			connector.Disconnect(connected1);
			connector.Disconnect(connected2);

			Assert.IsTrue(connector.IsPlacementEmpty);
		}

		[TestMethod]
		public void DoesntConnectWhenMatchRaiseException()
		{
			this._viewMatcher.Match(null).ReturnsForAnyArgs(ci => { throw new Exception(); });

			IItemsPlacementConnector connector = this.CreateTestObject();
			try
			{
				var resolved = new object();
				connector.Connect(resolved);
			}
			catch (Exception)
			{
			}
			Assert.IsFalse(connector.Connected.Any());
			Assert.IsTrue(connector.Views.IsEmpty);
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
			var connected3 = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			connector.Views.CollectionChanged += (sender, e) =>
				{
					((ICollectionView)sender).MoveCurrentToFirst();
				};
			connector.ActiveChanged += (sender, e) =>
				{
					Assert.Fail("Вызвано событие ActiveChanged");
				};

			connector.Connect(connected3);
		}

		[TestMethod]
		public void AvoidsUIEffectWhileDisconnecting()
		{
			// О целях данного теста, см. AvoidsUIEffectWhileConnecting.

			var connected1 = new object();
			var connected2 = new object();
			var connected3 = new object();

			IItemsPlacementConnector connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);
			connector.Connect(connected3);

			connector.Active = connected2;

			connector.Views.CollectionChanged += (sender, e) =>
			{
				((ICollectionView)sender).MoveCurrentToFirst();
			};
			connector.ActiveChanged += (sender, e) =>
			{
				Assert.AreEqual(connected2, e.OldValue);
				Assert.AreEqual(connected3, e.NewValue);
			};

			connector.Disconnect(connected2);
		}

		[TestMethod]
		public void RaisesActiveChangedAfterConnectedChangedWhenConnected()
		{
			var connector = this.CreateTestObject(ItemsPlacementConnectorFlags.ActivateOnConnect);

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

			var connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			connector.Active = connected2;

			var connectedChangedRaised = false;
			connector.ConnectedChanged += (sender, e) => connectedChangedRaised = true;
			connector.ActiveChanged += (sender, e) => Assert.IsFalse(connectedChangedRaised);

			connector.Disconnect(connected2);
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

			var connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			Assert.AreEqual(connected1, connector.ConnectedFromView(view1));
			Assert.AreEqual(connected2, connector.ConnectedFromView(view2));
		}

		[TestMethod]
		public void DisposesDisconnectedViews()
		{
			var connected1 = new object();
			var connected2 = new object();

			var view1 = Substitute.For<IDisposable>();
			var view2 = Substitute.For<IDisposable>();

			var match1 = Substitute.For<IViewModelViewMatch>();
			var match2 = Substitute.For<IViewModelViewMatch>();

			match1.View.Returns(view1);
			match2.View.Returns(view2);

			this._viewMatcher.Match(connected1).Returns(match1);
			this._viewMatcher.Match(connected2).Returns(match2);

			var connector = this.CreateTestObject();
			connector.Connect(connected1);
			connector.Connect(connected2);

			view1.DidNotReceive().Dispose();
			view2.DidNotReceive().Dispose();

			connector.Disconnect(connected1);
			connector.Disconnect(connected2);

			view1.Received(1).Dispose();
			view2.Received(1).Dispose();
		}

		[TestMethod]
		public void DoesntDisposeDraggingDisconnectedView()
		{
			var connected = new object();
			var view = Substitute.For<IDisposable>();
			var match = Substitute.For<IViewModelViewMatch>();
			
			match.View.Returns(view);
			this._viewMatcher.Match(connected).Returns(match);

			var connector = this.CreateTestObject();
			
			connector.Connect(connected);
			this._connectedDragDrop.IsDragging(connected).Returns(true);
			connector.Disconnect(connected);

			view.DidNotReceive().Dispose();
		}

		[TestMethod]
		public void SetsDraggingDisconnectedViewAsDragDropData()
		{
			var connected = new object();
			var view = Substitute.For<IDisposable>();
			var match = Substitute.For<IViewModelViewMatch>();

			match.View.Returns(view);
			this._viewMatcher.Match(connected).Returns(match);

			var connector = this.CreateTestObject();

			connector.Connect(connected);
			this._connectedDragDrop.IsDragging(connected).Returns(true);
			connector.Disconnect(connected);

			this._connectedDragDrop
				.Received(1)
				.SetData(ConnectedDragDropKeys.UriConnectedView, view);
		}

		[TestMethod]
		public void ExposesViewFromDragDropData()
		{
			var connected = new object();
			var view = Substitute.For<IDisposable>();

			this._connectedDragDrop.IsDragging(connected).Returns(true);
			this._connectedDragDrop.GetData(ConnectedDragDropKeys.UriConnectedView).Returns(view);

			IItemsPlacementConnector connector = this.CreateTestObject();
			
			connector.Connect(connected);
			Assert.IsTrue(connector.Views.Contains(view));

			this._viewMatcher.DidNotReceiveWithAnyArgs().Match(null);
		}
	}
}
