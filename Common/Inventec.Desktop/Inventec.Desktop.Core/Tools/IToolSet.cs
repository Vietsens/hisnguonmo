#region License

// Created by phuongdt

#endregion

using System;
using Inventec.Desktop.Core.Actions;

namespace Inventec.Desktop.Core.Tools
{
    /// <summary>
    /// Defines the external interface to a tool set, which manages a set of tools.
    /// </summary>
    public interface IToolSet : IDisposable
    {
        /// <summary>
        /// Gets the tools contained in this tool set.
        /// </summary>
        ITool[] Tools { get; }

		/// <summary>
		/// Finds the tool of the specified type.
		/// </summary>
		/// <typeparam name="TTool"></typeparam>
		/// <returns>The instance of the tool of the specified type, or null if no such exists.</returns>
		TTool Find<TTool>()
			where TTool: ITool;
        
        /// <summary>
        /// Returns the union of all actions defined by all tools in this tool set.
        /// </summary>
        IActionSet Actions { get; }
    }
}
