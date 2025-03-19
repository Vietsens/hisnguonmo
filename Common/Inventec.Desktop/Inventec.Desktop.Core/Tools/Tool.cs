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

namespace Inventec.Desktop.Core.Tools
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="ITool"/>.
    /// </summary>
    /// <remarks>
	/// Tool classes should inherit this class rather than implement <see cref="ITool"/> directly.
	/// </remarks>
    public abstract class Tool<TContextInterface> : ToolBase
        where TContextInterface: IToolContext
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected Tool()
		{
		}

    	/// <summary>
        /// Provides a typed reference to the context in which the tool is operating.
        /// </summary>
        /// <remarks>
		/// Attempting to access this property before <see cref="ITool.SetContext"/> has 
		/// been called (e.g in the constructor of this tool) will return null.
		/// </remarks>
        protected TContextInterface Context
        {
            get { return (TContextInterface)this.ContextBase; }
        }
    }
}
