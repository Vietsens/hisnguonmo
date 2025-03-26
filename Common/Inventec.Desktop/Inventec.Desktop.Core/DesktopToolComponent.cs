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

using Inventec.Desktop.Core.Tools;

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// Defines an extension point for desktop tools, which are instantiated once per desktop window.
    /// </summary>
    /// <remarks>
    /// Desktop tools are owned by a desktop window. A desktop tool is instantiated once per desktop window.
    /// Extensions should expect to recieve a tool context of type <see cref="IDesktopToolContext"/>.
    /// </remarks>
    [ExtensionPoint()]
    public sealed class DesktopToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    /// <summary>
    /// Tool context interface provided to tools that extend <see cref="DesktopToolExtensionPoint"/>.
    /// </summary>
    public interface IDesktopToolContext : IToolContext
    {
        
    }
}
