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

// Creater by phuongdt

#endregion

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// An abstract base class for extension filters.  
    /// </summary>
    /// <remarks>
	/// Extension filters are used to filter the extension points returned by 
	/// one of the <b>CreateExtensions</b> methods.  Subclasses of this
	/// class implement specific types of filters.
	/// </remarks>
    public abstract class ExtensionFilter
    {
        /// <summary>
        /// Tests the specified extension against the criteria of this filter.
        /// </summary>
        /// <param name="extension">The extension to test.</param>
        /// <returns>True if the extension meets the criteria, false otherwise.</returns>
        public abstract bool Test(ExtensionInfo extension);
    }
}
