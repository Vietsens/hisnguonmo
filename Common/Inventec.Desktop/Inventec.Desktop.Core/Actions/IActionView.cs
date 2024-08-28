#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
    /// Defines the interface for a view onto an action.
    /// </summary>
    public interface IActionView : IView
    {
        /// <summary>
        /// Gets the action view's context; the property is set by the framework.
        /// </summary>
		IActionViewContext Context { get; set; }
    }
}
