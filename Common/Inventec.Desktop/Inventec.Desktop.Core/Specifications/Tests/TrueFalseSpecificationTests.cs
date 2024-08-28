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
	public class TrueFalseSpecificationTests: TestsBase
	{
		[Test]
		public void Test_True()
		{
			TrueSpecification s = new TrueSpecification();
			Assert.IsTrue(s.Test(true).Success);
			Assert.IsFalse(s.Test(false).Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_True_InvalidType()
		{
			// test something that is not a boolean value
			TrueSpecification s = new TrueSpecification();
			s.Test(1);
		}

		[Test]
		public void Test_False()
		{
			FalseSpecification s = new FalseSpecification();
			Assert.IsTrue(s.Test(false).Success);
			Assert.IsFalse(s.Test(true).Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_False_InvalidType()
		{
			// test something that is not a boolean value
			TrueSpecification s = new TrueSpecification();
			s.Test(1);
		}

		[Test]
		public void Test_IsNull()
		{
			IsNullSpecification s = new IsNullSpecification();
			Assert.IsTrue(s.Test(null).Success);
			Assert.IsTrue(s.Test("").Success);	// treat empty string as null
			Assert.IsFalse(s.Test(new object()).Success);
			Assert.IsFalse(s.Test(0).Success);
		}

		[Test]
		public void Test_NotNull()
		{
			NotNullSpecification s = new NotNullSpecification();
			Assert.IsTrue(s.Test(new object()).Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("").Success); 	// treat empty string as null
			Assert.IsTrue(s.Test(0).Success);
		}
	}
}

#endif
