using System;
using System.ComponentModel;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using UriShell.Shell.Resolution;
using UriShell.Tests;

namespace UriShell.Shell.Connectors
{
	[TestClass]
	public class ConnectedDragDropTests
	{
		private IUriDisconnectTable _uriDisconnectTable;
		
		[TestInitialize]
		public void Initialize()
		{
			this._uriDisconnectTable = Substitute.For<IUriDisconnectTable>();
		}

		[TestMethod]
		public void IsDraggingReturnsTrueAfterDrag()
		{
			var connected = new object();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			Assert.IsTrue(dragDrop.IsDragging(connected));
			Assert.IsTrue(dragDrop.IsActive);
		}

		[TestMethod]
		public void IsDraggingReturnsFalseAfterDrop()
		{
			var connected = new object();
			var connector = Substitute.For<IUriPlacementConnector>();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);
			dragDrop.Drop(connector);

			Assert.IsFalse(dragDrop.IsDragging(connected));
			Assert.IsFalse(dragDrop.IsActive);
		}

		[TestMethod]
		public void IsDraggingReturnsFalseAfterDisconnect()
		{
			var connected = new object();
			var connector = Substitute.For<IUriPlacementConnector>();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			IUriPlacementConnector dragDropConnector = dragDrop;
			dragDropConnector.Disconnect(connected);

			Assert.IsFalse(dragDrop.IsDragging(connected));
			Assert.IsFalse(dragDrop.IsActive);
		}

		[TestMethod]
		public void IsDraggingReturnsFalseForUnkownObject()
		{
			var connected = new object();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);
			
			Assert.IsFalse(dragDrop.IsDragging(new object()));
			Assert.IsFalse(dragDrop.IsDragging(new object()));
		}

		[TestMethod]
		public void DisconnectsFromUriDisconnectTableEntryOnDrag()
		{
			var connected = new object();
			var sourceConnector = Substitute.For<IUriPlacementConnector>();

			this._uriDisconnectTable[connected].Returns(sourceConnector);

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			sourceConnector.Received(1).Disconnect(connected);
		}

		[TestMethod]
		public void IsDraggingReturnsTrueBeforeDisconnectFromUriDisconnectTableEntry()
		{
			var connected = new object();
			var sourceConnector = Substitute.For<IUriPlacementConnector>();

			this._uriDisconnectTable[connected].Returns(sourceConnector);

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);

			sourceConnector
				.When(_ => _.Disconnect(connected))
				.Do(args => Assert.IsTrue(dragDrop.IsDragging(args[0])));

			dragDrop.Drag(connected);
		}

		[TestMethod]
		public void RegistersInUriDisconnectTableAfterDisconnectFromSourcePlacementConnector()
		{
			var connected = new object();
			var sourceConnector = Substitute.For<IUriPlacementConnector>();
			
			this._uriDisconnectTable[connected].Returns(sourceConnector);

			sourceConnector
				.When(_ => _.Disconnect(connected))
				.Do(_ => this._uriDisconnectTable.DidNotReceiveWithAnyArgs()[connected] = null);

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			this._uriDisconnectTable.Received(1)[connected] = dragDrop;
		}

		[TestMethod]
		public void ConnectsToTargetPlacementConnectorOnDrop()
		{
			var connected = new object();
			var targetConnector = Substitute.For<IUriPlacementConnector>();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);
			dragDrop.Drop(targetConnector);

			targetConnector.Received(1).Connect(connected);
		}

		[TestMethod]
		public void IsDraggingReturnsTrueBeforeConnectToTargetPlacementConnector()
		{
			var connected = new object();
			var targetConnector = Substitute.For<IUriPlacementConnector>();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			targetConnector
				.When(_ => _.Connect(connected))
				.Do(args => Assert.IsTrue(dragDrop.IsDragging(args[0])));

			dragDrop.Drop(targetConnector);
		}

		[TestMethod]
		public void RegistersTargetConnectorInUriDisconnectTableOnDrop()
		{
			var connected = new object();
			var targetConnector = Substitute.For<IUriPlacementConnector>();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);
			dragDrop.Drop(targetConnector);

			this._uriDisconnectTable.Received(1)[connected] = targetConnector;
		}

		[TestMethod]
		public void SetsMultipleFormatData()
		{
			var connected = new object();

			var key1 = new ConnectedDragDropKey<Type>();
			var key2 = new ConnectedDragDropKey<Assembly>();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			dragDrop.SetData(key1, typeof(ConnectedDragDropTests));
			dragDrop.SetData(key2, typeof(ConnectedDragDropTests).Assembly);

			Assert.AreSame(typeof(ConnectedDragDropTests), dragDrop.GetData(key1));
			Assert.AreSame(typeof(ConnectedDragDropTests).Assembly, dragDrop.GetData(key2));
		}

		[TestMethod]
		public void ThrowsExceptionWhenSettingDataWhileNotDragging()
		{
			var key = new ConnectedDragDropKey<Type>();
			var data = typeof(ConnectedDragDropTests);

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);

			ExceptionAssert.Throws<InvalidOperationException>(() => dragDrop.SetData(key, data));
		}

		[TestMethod]
		public void ThrowsExceptionWhenGettingDataForResolvedNotConnected()
		{
			var key = new ConnectedDragDropKey<Type>();
			var data = typeof(ConnectedDragDropTests);

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);

			ExceptionAssert.Throws<InvalidOperationException>(() => dragDrop.GetData(key));
		}

		[TestMethod]
		public void ReturnsDataPresentForDataHasBeenSet()
		{
			var connected = new object();

			var key1 = new ConnectedDragDropKey<Type>();
			var key2 = new ConnectedDragDropKey<Assembly>();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			dragDrop.SetData(key1, typeof(ConnectedDragDropTests));
			dragDrop.SetData(key2, typeof(ConnectedDragDropTests).Assembly);

			Assert.IsTrue(dragDrop.GetDataPresent(key1));
			Assert.IsTrue(dragDrop.GetDataPresent(key2));
		}

		[TestMethod]
		public void ReturnsDataNotPresentForDataHasNotBeenSet()
		{
			var connected = new object();

			var key1 = new ConnectedDragDropKey<Type>();
			var key2 = new ConnectedDragDropKey<Assembly>();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			dragDrop.SetData(key1, typeof(ConnectedDragDropTests));

			Assert.IsFalse(dragDrop.GetDataPresent(key2));
		}

		[TestMethod]
		public void DisposesDataOnDisconnect()
		{
			var connected = new object();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			var key1 = new ConnectedDragDropKey<IDisposable>();
			var key2 = new ConnectedDragDropKey<IContainer>();

			var disposable1 = Substitute.For<IDisposable>();
			var disposable2 = Substitute.For<IContainer>();

			dragDrop.SetData(key1, disposable1);
			dragDrop.SetData(key2, disposable2);

			IUriPlacementConnector dragDropConnector = dragDrop;
			dragDropConnector.Disconnect(connected);

			disposable1.Received(1).Dispose();
			disposable2.Received(1).Dispose();
		}

		[TestMethod]
		public void DoNotDisposesDataOnDrop()
		{
			var connected = new object();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			var key1 = new ConnectedDragDropKey<IDisposable>();
			var key2 = new ConnectedDragDropKey<IContainer>();

			var disposable1 = Substitute.For<IDisposable>();
			var disposable2 = Substitute.For<IContainer>();

			dragDrop.SetData(key1, disposable1);
			dragDrop.SetData(key2, disposable2);

			var targetConnector = Substitute.For<IUriPlacementConnector>();
			dragDrop.Drop(targetConnector);

			disposable1.DidNotReceive().Dispose();
			disposable2.DidNotReceive().Dispose();
		}

		[TestMethod]
		public void RaisesDraggedClosedOnDisconnect()
		{
			var connected = new object();
			var connector = Substitute.For<IUriPlacementConnector>();

			var dragDrop = new ConnectedDragDrop(this._uriDisconnectTable);
			dragDrop.Drag(connected);

			var wasRaised = false;
			dragDrop.DraggedClosed += (sender, e) => wasRaised = true;

			IUriPlacementConnector dragDropConnector = dragDrop;
			dragDropConnector.Disconnect(connected);

			Assert.IsTrue(wasRaised);
		}
	}
}
