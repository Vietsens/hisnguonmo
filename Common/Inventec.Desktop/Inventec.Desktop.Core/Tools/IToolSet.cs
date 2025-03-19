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
