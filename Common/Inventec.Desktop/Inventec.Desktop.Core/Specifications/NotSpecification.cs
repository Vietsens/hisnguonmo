#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591
namespace Inventec.Desktop.Core.Specifications
{
	public class NotSpecification : CompositeSpecification
	{
		protected override TestResult InnerTest(object exp, object root)
		{
			foreach (var subSpec in this.Elements)
			{
				var r = subSpec.Test(exp);
				if (r.Fail)
					return new TestResult(true);
			}
			return new TestResult(false, new TestResultReason(this.FailureMessage));
			
		}
	}
}
