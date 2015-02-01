using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;
using UriShell.Tests;

namespace UriShell.Shell.Resolution
{
	[TestClass]
	public class UriResolvedObjectHolderTests
	{
		private static readonly int ValidIdCount = PhoenixUriBuilder.MaxResolvedId - PhoenixUriBuilder.MinResolvedId + 1;

		private UriResolvedMetadata _uriResolvedMetadata;

		[TestInitialize]
		public void Initialize()
		{
			this._uriResolvedMetadata = new UriResolvedMetadata(new Uri("tst://p/m/v"), Substitute.For<IDisposable>());
		}

		[TestMethod]
		public void AddsObjectsToHolder()
		{
			var object1 = new object();
			var object2 = new object();

			var holder = new UriResolvedObjectHolder();
			holder.Add(object1, this._uriResolvedMetadata);
			holder.Add(object2, this._uriResolvedMetadata);

			CollectionAssert.AreEquivalent(new[] { object1, object2 }, holder.ToArray());
		}

		[TestMethod]
		public void AddsObjectsMetadataToHolder()
		{
			var object1 = new object();
			var object2 = new object();

			var metadata1 = new UriResolvedMetadata(new Uri("tst://p1/md1/vw1"), Substitute.For<IDisposable>());
			var metadata2 = new UriResolvedMetadata(new Uri("tst://p2/md2/vw2"), Substitute.For<IDisposable>());

			var holder = new UriResolvedObjectHolder();
			holder.Add(object1, metadata1);
			holder.Add(object2, metadata2);

			var holderMetadata1 = holder.GetMetadata(object1);
			var holderMetadata2 = holder.GetMetadata(object2);

			// Идентификатор назначается холдером, поэтому придется изменить
			// локальные метаданные, ипользуя сведения от GetMetadata.
			// Подобное допустимо, т.к. нас интересуют не идентификаторы,
			// а все остальные поля.
			metadata1 = metadata1.AssignId(holderMetadata1.ResolvedId);
			metadata2 = metadata2.AssignId(holderMetadata2.ResolvedId);

			Assert.AreEqual(metadata1, holderMetadata1);
			Assert.AreEqual(metadata2, holderMetadata2);
		}
		
		[TestMethod]
		public void RaisesExceptionWhenAddingTheSameObjectTwice()
		{
			var object1 = new object();

			var holder = new UriResolvedObjectHolder();
			holder.Add(object1, this._uriResolvedMetadata);

			ExceptionAssert.Throws<ArgumentException>(
				ex => ex.ParamName == "resolved",
				() => holder.Add(object1, this._uriResolvedMetadata));
		}

		[TestMethod]
		public void RemovesObjectFromHolder()
		{
			var object1 = new object();
			var object2 = new object();

			var holder = new UriResolvedObjectHolder();
			holder.Add(object1, this._uriResolvedMetadata);
			holder.Add(object2, this._uriResolvedMetadata);

			holder.Remove(object1);

			CollectionAssert.AreEquivalent(new[] { object2 }, holder.ToArray());
		}

		[TestMethod]
		public void DoesntRaiseExceptionWhenRemovingTheSameObjectTwice()
		{
			var object1 = new object();

			var holder = new UriResolvedObjectHolder();
			holder.Add(object1, this._uriResolvedMetadata);

			holder.Remove(object1);
			holder.Remove(object1);
		}

		[TestMethod]
		[Timeout(300)]
		public void GeneratesUniqueIds()
		{
			var objects = Enumerable
				.Range(PhoenixUriBuilder.MinResolvedId, UriResolvedObjectHolderTests.ValidIdCount)
				.Select(_ => new object());

			var ids = new HashSet<int>();
			var holder = new UriResolvedObjectHolder();

			foreach (var obj in objects)
			{
				holder.Add(obj, this._uriResolvedMetadata);

				var id = holder.GetMetadata(obj).ResolvedId;

				Assert.IsTrue(ids.Add(id));
			}
		}

		[TestMethod]
		[Timeout(300)]
		[ExpectedException(typeof(InvalidOperationException))]
		public void RaisesExceptionWhenIdsExceedViewIdContraints()
		{
			var objects = Enumerable
				.Range(PhoenixUriBuilder.MinResolvedId, UriResolvedObjectHolderTests.ValidIdCount)
				.Select(_ => new object());

			var holder = new UriResolvedObjectHolder();

			foreach (var obj in objects)
			{
				holder.Add(obj, this._uriResolvedMetadata);
			}

			holder.Add(new object(), this._uriResolvedMetadata);
		}

		[TestMethod]
		[Timeout(300)]
		public void ReuseIdAfterObjectRemoving()
		{
			var object1 = new object();
			var object2 = new object();

			var objects = Enumerable
				.Range(PhoenixUriBuilder.MinResolvedId, UriResolvedObjectHolderTests.ValidIdCount - 2)
				.Select(_ => new object())
				.Concat(Enumerable.Repeat(object1, 1))
				.Concat(Enumerable.Repeat(object2, 1));

			var holder = new UriResolvedObjectHolder();

			foreach (var obj in objects)
			{
				holder.Add(obj, this._uriResolvedMetadata);
			}

			var id1 = holder.GetMetadata(object1).ResolvedId;
			var id2 = holder.GetMetadata(object2).ResolvedId;

			holder.Remove(object1);
			holder.Remove(object2);

			var object3 = new object();
			holder.Add(object3, this._uriResolvedMetadata);

			var id3 = holder.GetMetadata(object3).ResolvedId;
			Assert.IsTrue(id3 == id1 || id3 == id2);

			var object4 = new object();
			holder.Add(object4, this._uriResolvedMetadata);

			var id4 = holder.GetMetadata(object4).ResolvedId;
			Assert.AreNotEqual(id3, id4);
			Assert.IsTrue(id4 == id1 || id4 == id2);
		}

		[TestMethod]
		public void ContainsReturnsTrueForAddedObjects()
		{
			var object1 = new object();
			var object2 = new object();

			var holder = new UriResolvedObjectHolder();
			holder.Add(object1, this._uriResolvedMetadata);
			holder.Add(object2, this._uriResolvedMetadata);

			Assert.IsTrue(holder.Contains(object1));
			Assert.IsTrue(holder.Contains(object2));
		}

		[TestMethod]
		public void ContainsReturnsFalseForRemovedObjects()
		{
			var object1 = new object();
			var object2 = new object();

			var holder = new UriResolvedObjectHolder();
			holder.Add(object1, this._uriResolvedMetadata);
			holder.Remove(object1);

			Assert.IsFalse(holder.Contains(object1));
		}

		[TestMethod]
		public void ContainsReturnsFalseForUnknownObjectsAndNull()
		{
			var holder = new UriResolvedObjectHolder();

			Assert.IsFalse(holder.Contains(new object()));
			Assert.IsFalse(holder.Contains(null));
		}

		[TestMethod]
		public void GetsObjectsById()
		{
			var object1 = new object();
			var object2 = new object();

			var holder = new UriResolvedObjectHolder();
			holder.Add(object1, this._uriResolvedMetadata);
			holder.Add(object2, this._uriResolvedMetadata);

			var id1 = holder.GetMetadata(object1).ResolvedId;
			var id2 = holder.GetMetadata(object2).ResolvedId;

			Assert.AreEqual(object1, holder.Get(id1));
			Assert.AreEqual(object2, holder.Get(id2));
		}

		[TestMethod]
		public void RaisesExceptionWhenGettingObjectByUnknownId()
		{
			var holder = new UriResolvedObjectHolder();

			ExceptionAssert.Throws<ArgumentOutOfRangeException>(
				ex => ex.ParamName == "id",
				() => holder.Get(9901));
		}

		[TestMethod]
		public void RaisesExceptionWhenGettingMetadataForUnknownObject()
		{
			var holder = new UriResolvedObjectHolder();

			ExceptionAssert.Throws<ArgumentOutOfRangeException>(
				ex => ex.ParamName == "resolved",
				() => holder.GetMetadata(new object()));
		}
	}
}
