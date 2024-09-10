#region License

// Created by phuongdt

#endregion

#if UNIT_TESTS

#pragma warning disable 1591

using System;
using NUnit.Framework;

namespace Inventec.Desktop.Core.Specifications.Tests
{
	public abstract class TestsBase
	{
		private class ConstantSpecification : Specification
		{
			private readonly TestResult _result;

			public ConstantSpecification(TestResult result)
			{
				_result = result;
			}

			protected override TestResult InnerTest(object exp, object root)
			{
				return _result;
			}
		}

		public ISpecification AlwaysTrue = new ConstantSpecification(new TestResult(true, new TestResultReason("Always true")));
		public ISpecification AlwaysFalse = new ConstantSpecification(new TestResult(false, new TestResultReason("Always false")));

		protected class PredicateSpecification<T> : Specification
		{
			private readonly Predicate<T> _predicate;

			public PredicateSpecification(Predicate<T> predicate)
			{
				_predicate = predicate;
			}

			protected override TestResult InnerTest(object exp, object root)
			{
				return new TestResult(_predicate((T)exp));
			}
		}

		protected class ConstantExpression : Expression
		{
			private readonly object _value;

			public ConstantExpression(object constantValue)
				:base("")
			{
				_value = constantValue;
			}

			public ConstantExpression(string text, object constantValue)
				:base(text)
			{
				_value = constantValue;
			}

			public object Value
			{
				get { return _value; }
			}

			public override object Evaluate(object arg)
			{
				return _value;
			}
		}

		protected class ConstantExpressionFactory : IExpressionFactory
		{
			public Expression CreateExpression(string text)
			{
				return new ConstantExpression(text, null);
			}
		}
	}
}

#endif
