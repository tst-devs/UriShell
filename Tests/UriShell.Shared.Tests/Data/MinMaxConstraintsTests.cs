using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TST.Phoenix.Arm.Data
{
	[TestClass]
	public class MinMaxConstraintsTests
	{
		[TestMethod]
		public void CtorAssignsMinMax()
		{
			const int Min = 10;
			const int Max = 100;

			var minMax = new MinMaxConstraints<int>(Min, Max);

			Assert.AreEqual(Min, minMax.Min);
			Assert.AreEqual(Max, minMax.Max);
		}

		[TestMethod]
		public void TestIEquatable()
		{
			IEquatable<MinMaxConstraints<int>> value1 = new MinMaxConstraints<int>(10, 100);
			
			// Проверяем равенство одинаковых значений.
			var value2 = new MinMaxConstraints<int>(10, 100);
			Assert.IsTrue(value1.Equals(value2));

			// Проверяем неравенство значений, с совпадающей верхней границей.
			value2 = new MinMaxConstraints<int>(5, 100);
			Assert.IsFalse(value1.Equals(value2));

			// Проверяем неравенство значений, с совпадающей нижней границей.
			value2 = new MinMaxConstraints<int>(10, 105);
			Assert.IsFalse(value1.Equals(value2));

			// Проверяем неравенство значений всех границ.
			value2 = new MinMaxConstraints<int>(25, 48);
			Assert.IsFalse(value1.Equals(value2));
		}

		[TestMethod]
		public void TestEquals()
		{
			object value1 = new MinMaxConstraints<int>(30, 60);

			// Проверяем равенство одинаковых значений.
			var value2 = new MinMaxConstraints<int>(30, 60);
			Assert.IsTrue(value1.Equals(value2));
			Assert.IsTrue(value1.GetHashCode() == value2.GetHashCode());

			// Проверяем неравенство значений, с совпадающей верхней границей.
			value2 = new MinMaxConstraints<int>(5, 60);
			Assert.IsFalse(value1.Equals(value2));
			Assert.IsFalse(value1.GetHashCode() == value2.GetHashCode());

			// Проверяем неравенство значений, с совпадающей нижней границей.
			value2 = new MinMaxConstraints<int>(30, 105);
			Assert.IsFalse(value1.Equals(value2));
			Assert.IsFalse(value1.GetHashCode() == value2.GetHashCode());

			// Проверяем неравенство значений всех границ.
			value2 = new MinMaxConstraints<int>(37, 54);
			Assert.IsFalse(value1.Equals(value2));
			Assert.IsFalse(value1.GetHashCode() == value2.GetHashCode());
		}

		[TestMethod]
		public void TestEqualityOperators()
		{
			var value1 = new MinMaxConstraints<int>(20, 80);

			// Проверяем равенство одинаковых значений.
			var value2 = new MinMaxConstraints<int>(20, 80);
			Assert.IsTrue(value1 == value2);
			Assert.IsFalse(value1 != value2);

			// Проверяем неравенство значений, с совпадающей верхней границей.
			value2 = new MinMaxConstraints<int>(5, 80);
			Assert.IsFalse(value1 == value2);
			Assert.IsTrue(value1 != value2);

			// Проверяем неравенство значений, с совпадающей нижней границей.
			value2 = new MinMaxConstraints<int>(20, 105);
			Assert.IsFalse(value1 == value2);
			Assert.IsTrue(value1 != value2);

			// Проверяем неравенство значений всех границ.
			value2 = new MinMaxConstraints<int>(35, 67);
			Assert.IsFalse(value1 == value2);
			Assert.IsTrue(value1 != value2);
		}
	}
}
