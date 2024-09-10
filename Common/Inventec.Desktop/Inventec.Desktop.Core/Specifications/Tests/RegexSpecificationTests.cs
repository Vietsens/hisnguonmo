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
	public class RegexSpecificationTests : TestsBase
	{
		[Test]
		public void Test_StrictCasing()
		{
			RegexSpecification s = new RegexSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
		}

		[Test]
		public void Test_IgnoreCasing()
		{
			RegexSpecification s = new RegexSpecification("Foo", true, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsTrue(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("FOO").Success);
		}

		[Test]
		public void Test_NullMatches()
		{
			RegexSpecification s = new RegexSpecification("Foo", false, true);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);

			Assert.IsTrue(s.Test(null).Success);

			//TODO: it would seem that this test ought to succeed - consider changing this behaviour
			Assert.IsFalse(s.Test("").Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_InvalidType()
		{
			RegexSpecification s = new RegexSpecification("Foo", false, false);
			Assert.IsFalse(s.Test(1).Success);
		}
	}
}

#endif
