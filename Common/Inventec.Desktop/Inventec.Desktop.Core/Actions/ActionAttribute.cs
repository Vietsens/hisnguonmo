#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to specify actions declaratively.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public abstract class ActionAttribute : Attribute
    {
        private readonly string _actionID;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">A logical action identifier.</param>
        protected ActionAttribute(string actionID)
        {
            _actionID = actionID;
        }

        /// <summary>
        /// Returns the logical action ID qualified by the type name of the specified target object.
        /// </summary>
        /// <param name="target">The object whose type should be used to qualify the action ID.</param>
        public string QualifiedActionID(object target)
        {
            // create a fully qualified action ID
            return string.Format("{0}:{1}", target.GetType().FullName, _actionID);
        }

		/// <summary>
		/// Applies this attribute to an <see cref="IAction"/> instance, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
		public abstract void Apply(IActionBuildingContext builder);
    }
}
