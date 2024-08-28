#region License

// Created by phuongdt

#endregion

using System;
using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
	/// A specialization of the <see cref="ResourceResolver"/> class for use in resolving resources
	/// related to actions.
	/// </summary>
	public class ActionResourceResolver : ApplicationThemeResourceResolver
	{
		/// <summary>
		/// Constructs an instance of this object for the specified action target.
		/// </summary>
		/// <remarks>
		/// The class of the target object determines the primary assembly that will be used to resolve resources.
		/// </remarks>
		/// <param name="actionTarget">The action target for which resources will be resolved.</param>
		public ActionResourceResolver(object actionTarget)
			: base(actionTarget.GetType(), true) {}

		/// <summary>
		/// Constructs an instance of this object for the specified action target.
		/// </summary>
		/// <remarks>
		/// The class of the target object determines the primary assembly that will be used to resolve resources.
		/// </remarks>
		/// <param name="targetType">The action target type for which resources will be resolved.</param>
		public ActionResourceResolver(Type targetType)
			: base(targetType, true) {}
	}
}