#region License

// Created by phuongdt

#endregion

using System;
using System.Collections.Generic;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Represents an unordered set of actions.
    /// </summary>
    public interface IActionSet : IEnumerable<IAction>
    {
        /// <summary>
        /// Gets the number of actions in the set.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns a subset of this set containing only the elements for which the predicate is true.
        /// </summary>
        /// <param name="predicate">The predicate to test.</param>
        IActionSet Select(Predicate<IAction> predicate);

        /// <summary>
        /// Returns a set that corresponds to the union of this set with another set.
        /// </summary>
        IActionSet Union(IActionSet other);
    }
}
