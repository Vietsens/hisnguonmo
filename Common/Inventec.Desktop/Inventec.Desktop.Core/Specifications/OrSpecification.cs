#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public class OrSpecification : CompositeSpecification
	{
		protected override TestResult InnerTest(object exp, object root)
		{
			foreach (var subSpec in this.Elements)
			{
				var r = subSpec.Test(exp);
				if (r.Success)
					return new TestResult(true);
			}

			// note that we can only return the immediate reason - there is no sensible way to return sub-reasons 
			return new TestResult(false, new TestResultReason(this.FailureMessage));
		}
	}
}
