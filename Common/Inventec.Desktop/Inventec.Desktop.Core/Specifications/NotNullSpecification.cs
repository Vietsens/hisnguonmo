#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public class NotNullSpecification : PrimitiveSpecification
	{
		protected override TestResult InnerTest(object exp, object root)
		{
			// treat string "" as null, in the case where exp is a string
			return DefaultTestResult(exp != null && ((exp as string) != ""));
		}
	}
}
