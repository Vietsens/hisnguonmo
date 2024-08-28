#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public class AllSpecification : EnumerableSpecification
	{
		public AllSpecification(ISpecification elementSpecification)
			: base(elementSpecification)
		{
		}

		protected override TestResult InnerTest(object exp, object root)
		{
			foreach (var element in AsEnumerable(exp))
			{
				var r = this.ElementSpec.Test(element);
				if (r.Fail)
					return new TestResult(false, new TestResultReason(this.FailureMessage, r.Reasons));
			}

			return new TestResult(true);
		}
	}
}
