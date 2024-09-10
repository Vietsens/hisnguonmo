#region License

// Created by phuongdt

#endregion

using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Used by the action attribute mechanism to maintain context 
    /// between attributes that co-operate to build the same action.
    /// </summary>
    public interface IActionBuildingContext
    {
        /// <summary>
        /// Gets or sets the action that is being built.
        /// </summary>
        Action Action { get; set; }

        /// <summary>
        /// Gets the resource resolver that is supplied to the action.
        /// </summary>
        IResourceResolver ResourceResolver { get; }

        /// <summary>
        /// Gets the target object on which the action operates.
        /// </summary>
        object ActionTarget { get; }

        /// <summary>
        /// Gets the logical action ID for the action.
        /// </summary>
        string ActionID { get; }
	}
}
