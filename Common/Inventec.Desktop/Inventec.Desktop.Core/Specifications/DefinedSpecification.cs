#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public class DefinedSpecification : Specification
	{
		private readonly ISpecification _lambda;

		public DefinedSpecification(ISpecification lambda)
		{
			_lambda = lambda;
		}

		protected override TestResult InnerTest(object exp, object root)
		{
			var r = _lambda.Test(exp);
			return r.Success ? new TestResult(true) : new TestResult(false, new TestResultReason(this.FailureMessage, r.Reasons));
		}
	}
}
