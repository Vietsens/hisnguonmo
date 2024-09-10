#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public class TrueSpecification : PrimitiveSpecification
	{
		protected override TestResult InnerTest(object exp, object root)
		{
			if (exp is bool)
			{
				return DefaultTestResult((bool)exp);
			}
			throw new SpecificationException(SR.ExceptionCastExpressionBoolean);
		}
	}
}
