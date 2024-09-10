#region License

// Created by phuongdt

#endregion

#if UNIT_TESTS

#pragma warning disable 1591

using System;
using NUnit.Framework;

namespace Inventec.Desktop.Core.Specifications.Tests
{
	[TestFixture]
	public class CountSpecificationTests : TestsBase
	{
		[Test]
		public void Test_MinMaxBothZero()
		{
			CountSpecification s = new CountSpecification(0, 0, null);
			Assert.IsTrue(s.Test(new int[] {}).Success);
			Assert.IsFalse(s.Test(new int[] { 1 }).Success);
			Assert.IsFalse(s.Test(new int[] { 1, 2 }).Success);
		}

		[Test]
		public void Test_MinMaxBothLargest()
		{
			// this should always be false unless someone has fills an array with MaxValue items
			CountSpecification s = new CountSpecification(int.MaxValue, int.MaxValue, null);
			Assert.IsFalse(s.Test(new int[] { }).Success);
			Assert.IsFalse(s.Test(new int[] { 1 }).Success);
			Assert.IsFalse(s.Test(new int[] { 1, 2 }).Success);
		}

		[Test]
		public void Test_MinMaxBothEqualAndNonZero()
		{
			CountSpecification s = new CountSpecification(2, 2, null);
			Assert.IsFalse(s.Test(new int[] { }).Success);
			Assert.IsFalse(s.Test(new int[] { 1 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2 }).Success);
			Assert.IsFalse(s.Test(new int[] { 1, 2, 3 }).Success);
		}

		[Test]
		public void Test_MinMaxRange1()
		{
			CountSpecification s = new CountSpecification(1, 2, null);
			Assert.IsFalse(s.Test(new int[] { }).Success);
			Assert.IsTrue(s.Test(new int[] { 1 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2 }).Success);
			Assert.IsFalse(s.Test(new int[] { 1, 2, 3 }).Success);
		}

		[Test]
		public void Test_MinMaxRange()
		{
			CountSpecification s = new CountSpecification(1, 4, null);
			Assert.IsFalse(s.Test(new int[] { }).Success);
			Assert.IsTrue(s.Test(new int[] { 1 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2, 3 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2, 3, 4 }).Success);
			Assert.IsFalse(s.Test(new int[] { 1, 2, 3, 4, 5 }).Success);
		}

		[Test]
		public void Test_Filter()
		{
			CountSpecification s1 = new CountSpecification(1, 1, AlwaysTrue);
			Assert.IsFalse(s1.Test(new int[] { }).Success);
			Assert.IsTrue(s1.Test(new int[] { 1 }).Success);
			Assert.IsFalse(s1.Test(new int[] { 1, 2 }).Success);

			CountSpecification s2 = new CountSpecification(1, 1, AlwaysFalse);
			Assert.IsFalse(s2.Test(new int[] { }).Success);
			Assert.IsFalse(s2.Test(new int[] { 1 }).Success);
			Assert.IsFalse(s2.Test(new int[] { 1, 2 }).Success);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_InvalidRange()
		{
			CountSpecification s = new CountSpecification(1, 0, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Test_NegativeRange()
		{
			CountSpecification s = new CountSpecification(-1, 0, null);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_NonEnumerable()
		{
			CountSpecification s = new CountSpecification(1, 1, null);

			// testing an object that does not implement IEnumerable should throw
			Assert.IsFalse(s.Test(new object()).Success);
		}
	}
}

#endif
