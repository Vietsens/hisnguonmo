#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to decorate an action
    /// once it has been declared by an <see cref="ActionInitiatorAttribute"/>.
    /// </summary>
    public abstract class ActionDecoratorAttribute : ActionAttribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The unique identifer of the action.</param>
        protected ActionDecoratorAttribute(string actionID)
            : base(actionID)
        {
        }
    }
}
