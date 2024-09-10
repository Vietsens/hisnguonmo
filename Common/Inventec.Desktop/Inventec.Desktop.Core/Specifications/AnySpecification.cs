#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public class AnySpecification : EnumerableSpecification
	{
		public AnySpecification(ISpecification elementSpecification)
			: base(elementSpecification)
		{

		}

		protected override TestResult InnerTest(object exp, object root)
		{
			foreach (var element in AsEnumerable(exp))
			{
				var r = this.ElementSpec.Test(element);
				if (r.Success)
					return new TestResult(true);
			}

			// note that we can only return the immediate reason - there is no sensible way to return sub-reasons 
			return new TestResult(false, new TestResultReason(this.FailureMessage));
		}
	}
}
