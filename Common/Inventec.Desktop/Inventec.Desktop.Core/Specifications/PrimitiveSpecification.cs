#region License

// Created by phuongdt

#endregion

#pragma warning disable 1591

namespace Inventec.Desktop.Core.Specifications
{
	public abstract class PrimitiveSpecification : Specification
	{
		protected PrimitiveSpecification()
		{

		}

		protected TestResult DefaultTestResult(bool success)
		{
			return new TestResult(success, success ? null : new TestResultReason(this.FailureMessage));
		}
	}
}
