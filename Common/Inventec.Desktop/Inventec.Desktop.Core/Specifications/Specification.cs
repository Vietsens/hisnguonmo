#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public abstract class Specification : ISpecification
	{
		protected Specification()
		{
			TestExpression = Expression.Null;
		}

		public Expression TestExpression { get; set; }

		public string FailureMessage { get; set; }

		#region ISpecification Members

		public TestResult Test(object obj)
		{
			return InnerTest(TestExpression.Equals(Expression.Null) ? obj : TestExpression.Evaluate(obj), obj);
		}

		#endregion

		protected abstract TestResult InnerTest(object exp, object root);
	}
}
