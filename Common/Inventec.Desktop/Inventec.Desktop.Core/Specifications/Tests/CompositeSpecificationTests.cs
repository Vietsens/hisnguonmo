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
	public class CompositeSpecificationTests : TestsBase
	{
		[Test]
		public void Test_And_Degenerate()
		{
			// TODO: this is the current behaviour - perhaps it should throw an exception instead
			AndSpecification s = new AndSpecification();
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_And_AllTrue()
		{
			AndSpecification s = new AndSpecification();
			s.Add(AlwaysTrue);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_And_AllFalse()
		{
			AndSpecification s = new AndSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysFalse);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_And_Mixed()
		{
			AndSpecification s = new AndSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysTrue);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_Degenerate()
		{
			// TODO: this is the current behaviour - perhaps it should throw an exception instead
			OrSpecification s = new OrSpecification();
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_AllTrue()
		{
			OrSpecification s = new OrSpecification();
			s.Add(AlwaysTrue);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_AllFalse()
		{
			OrSpecification s = new OrSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysFalse);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_Mixed()
		{
			OrSpecification s = new OrSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_Degenerate()
		{
			// TODO: this is the current behaviour - perhaps it should throw an exception instead
			NotSpecification s = new NotSpecification();
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_AllTrue()
		{
			NotSpecification s = new NotSpecification();
			s.Add(AlwaysTrue);
			s.Add(AlwaysTrue);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_AllFalse()
		{
			NotSpecification s = new NotSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysFalse);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_Mixed()
		{
			NotSpecification s = new NotSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}
	}
}

#endif
