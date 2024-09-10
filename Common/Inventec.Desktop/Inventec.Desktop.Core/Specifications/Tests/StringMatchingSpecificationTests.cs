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
	public class StringMatchingSpecificationTests : TestsBase
	{
		[Test]
		public void Test_StartsWith()
		{
			var s = new StartsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsFalse(s.Test("fooa").Success);
			Assert.IsFalse(s.Test("afoo").Success);
			Assert.IsFalse(s.Test("aFoo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("Fooa").Success);
		}

		[Test]
		public void Test_StartsWith_StrictCasing()
		{
			var s = new StartsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("Fooa").Success);
		}

		[Test]
		public void Test_StartsWith_IgnoreCasing()
		{
			var s = new StartsWithSpecification("Foo", true, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsTrue(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("FOO").Success);
			Assert.IsTrue(s.Test("fooa").Success);
			Assert.IsTrue(s.Test("Fooa").Success);
			Assert.IsTrue(s.Test("FOOa").Success);
		}

		[Test]
		public void Test_StartsWith_NullMatches()
		{
			var s = new StartsWithSpecification("Foo", false, true);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);

			Assert.IsTrue(s.Test(null).Success);

			//TODO: it would seem that this test ought to succeed - consider changing this behaviour
			Assert.IsFalse(s.Test("").Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_StartsWith_InvalidType()
		{
			var s = new StartsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test(1).Success);
		}

		[Test]
		public void Test_EndsWith()
		{
			var s = new EndsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsFalse(s.Test("Fooa").Success);
			Assert.IsFalse(s.Test("afoo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("aFoo").Success);
		}

		[Test]
		public void Test_EndsWith_StrictCasing()
		{
			var s = new EndsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsFalse(s.Test("afoo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("aFoo").Success);
			Assert.IsTrue(s.Test("AFoo").Success);
		}

		[Test]
		public void Test_EndsWith_IgnoreCasing()
		{
			var s = new EndsWithSpecification("Foo", true, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsTrue(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("FOO").Success);
		}

		[Test]
		public void Test_EndsWith_NullMatches()
		{
			var s = new EndsWithSpecification("Foo", false, true);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);

			Assert.IsTrue(s.Test(null).Success);

			//TODO: it would seem that this test ought to succeed - consider changing this behaviour
			Assert.IsFalse(s.Test("").Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_EndsWith_InvalidType()
		{
			var s = new EndsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test(1).Success);
		}

		[Test]
		public void Test_Contains()
		{
			var s = new ContainsSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsFalse(s.Test("afoo").Success);
			Assert.IsFalse(s.Test("fooa").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("aFoo").Success);
			Assert.IsTrue(s.Test("Fooa").Success);
			Assert.IsTrue(s.Test("aFooa").Success);
		}

		[Test]
		public void Test_Contains_StrictCasing()
		{
			var s = new ContainsSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsFalse(s.Test("afoo").Success);
			Assert.IsFalse(s.Test("fooa").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("aFoo").Success);
			Assert.IsTrue(s.Test("Fooa").Success);
			Assert.IsTrue(s.Test("aFooa").Success);
		}

		[Test]
		public void Test_Contains_IgnoreCasing()
		{
			var s = new ContainsSpecification("Foo", true, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsTrue(s.Test("foo").Success);
			Assert.IsTrue(s.Test("afoo").Success);
			Assert.IsTrue(s.Test("fooa").Success);
			Assert.IsTrue(s.Test("aFoo").Success);
			Assert.IsTrue(s.Test("FOOa").Success);
		}

		[Test]
		public void Test_Contains_NullMatches()
		{
			var s = new ContainsSpecification("Foo", false, true);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);

			Assert.IsTrue(s.Test(null).Success);

			//TODO: it would seem that this test ought to succeed - consider changing this behaviour
			Assert.IsFalse(s.Test("").Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_Contains_InvalidType()
		{
			var s = new ContainsSpecification("Foo", false, false);
			Assert.IsFalse(s.Test(1).Success);
		}
	}
}

#endif
