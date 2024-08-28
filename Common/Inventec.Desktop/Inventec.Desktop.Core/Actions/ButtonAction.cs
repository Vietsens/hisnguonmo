#region License

// Created by phuongdt

#endregion

using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Models a toolbar button action.
    /// </summary>
    public class ButtonAction : ClickAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The fully qualified action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="flags">Flags that control the style of the action.</param>
        /// <param name="resourceResolver">A resource resolver that will be used to resolve text and image resources.</param>
        public ButtonAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resourceResolver)
            : base(actionID, path, flags, resourceResolver)
        {
        }
    }
}
