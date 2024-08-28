#region License

// Created by phuongdt

#endregion

using System;
using Inventec.Desktop.Core.Actions;

namespace Inventec.Desktop.Core.Tools
{
    /// <summary>
    /// Defines a tool.
    /// </summary>
    /// <remarks>
	/// Developers are encouraged to subclass <see cref="Tool{TContextInterface}"/> 
	/// or one of its subclasses rather than implement this interface directly.
	/// </remarks>
    public interface ITool : IDisposable
    {
        /// <summary>
        /// Called by the framework to set the tool context.
        /// </summary>
        void SetContext(IToolContext context);

        /// <summary>
        /// Called by the framework to allow the tool to initialize itself.
        /// </summary>
        /// <remarks>
		/// This method will be called after <see cref="SetContext"/> has been called, 
		/// which guarantees that the tool will have access to its context when this method is called.
		/// </remarks>
        void Initialize();

        /// <summary>
        /// Gets the set of actions that act on this tool.
        /// </summary>
        /// <remarks>
		/// This property is not guaranteed to be a dynamic property - that is, you should not assume
		/// this property will always return a different set of actions depending on the internal state 
		/// of the tool.  The class that owns the tool decides when to access this property, and 
		/// whether or not the actions can be dynamic will be dependent on the implementation of that class.
		/// </remarks>
        IActionSet Actions { get; }
    }
}
