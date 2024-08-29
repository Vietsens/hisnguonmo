#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Declares a keyboard action with the specifed action identifier and path hint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class KeyboardActionAttribute : ClickActionAttribute
	{
        /// <summary>
        /// Declares a keyboard action with the specified action ID and path hint.
        /// </summary>
        /// <param name="actionID">The fully qualified action ID.</param>
        /// <param name="pathHint">The suggested location of this action in the toolbar model.</param>
        public KeyboardActionAttribute(string actionID, string pathHint)
            : base(actionID, pathHint)
        {
        }

        /// <summary>
        /// Declares a keyboard action with the specified action ID, path hint and click-handler.
        /// </summary>
        /// <param name="actionID">The fully qualified action ID.</param>
        /// <param name="pathHint">The suggested location of this action in the toolbar model.</param>
        /// <param name="clickHandler">The name of the click handler to bind to on the target object.</param>
        public KeyboardActionAttribute(string actionID, string pathHint, string clickHandler)
            : base(actionID, pathHint, clickHandler)
        {
        }

        /// <summary>
        /// Creates the <see cref="KeyboardAction"/> represented by this attribute.
        /// </summary>
        /// <param name="actionID">The logical action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="flags">Flags that specify the click behaviour of the action.</param>
        /// <param name="resolver">The object used to resolve the action path and icons.</param>
        protected override ClickAction CreateAction(string actionID, ActionPath path, ClickActionFlags flags, Utilities.IResourceResolver resolver)
        {
            return new KeyboardAction(actionID, path, flags, resolver);
        }
    }
}
