#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to declare an action.
    /// </summary>
    public abstract class ActionInitiatorAttribute : ActionAttribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The unique identifer of the action.</param>
        protected ActionInitiatorAttribute(string actionID)
            : base(actionID)
        {
        }
    }
}
