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

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// A base interface for all classes that represent UI views.
    /// </summary>
    /// <remarks>
    /// <para>
	/// A view is a class that provides a UI representation for another object (the model).
	/// </para>
	/// <para>
    /// The purpose of a view class is to separate the presentation from the application logic,
    /// allowing the application to work with different GUI toolkits.
	/// </para>
    /// </remarks>
    public interface IView
    {
        /// <summary>
        /// Gets the toolkitID of the GUI tookit in which the view is implemented.
        /// </summary>
        string GuiToolkitID
        {
            get;
        }

        /// <summary>
        /// Gets the underlying GUI component for this view.
        /// </summary>
        /// <remarks>
        /// The type of the returned object is specific to a given GUI toolkit.  For example,
        /// a view implemented in Windows Forms would return a Windows Forms Control object.
        /// </remarks>
        object GuiElement
        {
            get;
        }
    }
}
