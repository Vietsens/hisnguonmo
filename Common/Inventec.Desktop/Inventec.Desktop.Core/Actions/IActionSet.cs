/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
