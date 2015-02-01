using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

namespace TST.Phoenix.Arm.Data
{
	[TestClass]
	public class MinMaxConstraintsExtensionsTests
	{
		[TestMethod]
		public void ApplyAppliesConstraints()
		{
			var minMax = new MinMaxConstraints<int>(10, 100);
			
			const int Smaller = 5;
			const int Greater = 105;
			const int Fits1 = 11;
			const int Fits2 = 99;

			Assert.AreEqual(minMax.Max, minMax.Apply(Greater));
			Assert.AreEqual(minMax.Min, minMax.Apply(Smaller));
			Assert.AreEqual(Fits1, minMax.Apply(Fits1));
			Assert.AreEqual(Fits2, minMax.Apply(Fits2));
		}

		[TestMethod]
		public void ApplyUsesSpecifiedComparer()
		{
			var minMax = new MinMaxConstraints<int>(10, 100);
						
			const int Smaller = 5;
			const int Greater = 105;

			var comparer = Substitute.For<Comparer<int>>();

			// Настраиваем Comparer так, что Greater считается числом, меньшим Max.
			comparer.Compare(Arg.Is(Greater), Arg.Is(minMax.Max)).Returns(-1);
			comparer.Compare(Arg.Is(minMax.Max), Arg.Is(Greater)).Returns(1);
			Assert.AreEqual(Greater, minMax.Apply(Greater, comparer));
			comparer.Received().Compare(Arg.Any<int>(), Arg.Any<int>());

			comparer.ClearReceivedCalls();

			// Настраиваем Comparer так, что Smaller считается числом, большим Min.
			comparer.Compare(Arg.Is(Smaller), Arg.Is(minMax.Min)).Returns(1);
			comparer.Compare(Arg.Is(minMax.Min), Arg.Is(Smaller)).Returns(-1);
			Assert.AreEqual(Smaller, minMax.Apply(Smaller, comparer));
			comparer.Received().Compare(Arg.Any<int>(), Arg.Any<int>());
		}
	}
}
