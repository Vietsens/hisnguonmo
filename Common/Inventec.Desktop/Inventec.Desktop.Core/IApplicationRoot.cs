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
    /// Defines an application entry point.
    /// </summary>
    /// <seealso cref="ApplicationRootExtensionPoint"/>
    public interface IApplicationRoot
    {
        /// <summary>
        /// Called by the platform to run the application.
        /// </summary>
        /// <remarks>
        /// It is expected that this method may block for the duration of the application's execution, if
        /// for instance, a GUI event message pump is started.
        /// </remarks>
        void RunApplication(string[] args);
    }
}
