#region License

// Created by phuongdt

#endregion

#if UNIT_TESTS

using Inventec.Desktop.Core.Specifications;

namespace Inventec.Desktop.Core.Actions.Tests
{
	/// <summary>
	/// Decorates an action to specify a desired outcome when the action permissions are evaluated.
	/// </summary>
	public sealed class MockActionPermissionAttribute : ActionDecoratorAttribute
	{
		private readonly bool _result = true;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionId">The ID of the action to which the mock action permission applies.</param>
		/// <param name="result">The desired result when the action permissions are evaluated. True indicates that permissions are granted; False indicates that permissions are denied.</param>
		public MockActionPermissionAttribute(string actionId, bool result)
			: base(actionId)
		{
			_result = result;
		}

		/// <summary>
		/// Applies permissions represented by this attribute to an action instance, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
		public override void Apply(IActionBuildingContext builder)
		{
			builder.Action.PermissionSpecification = new MockSpecification {Result = _result};
		}

		private class MockSpecification : ISpecification
		{
			public bool Result { private get; set; }

			public TestResult Test(object @object)
			{
				return new TestResult(Result);
			}
		}
	}
}

#endif