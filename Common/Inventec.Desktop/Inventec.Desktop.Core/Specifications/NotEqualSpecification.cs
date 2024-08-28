#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public class NotEqualSpecification : ComparisonSpecification
	{
		protected override bool CompareValues(object testValue, object refValue)
		{
			return !Equals(testValue, refValue);
		}
	}
}
