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

using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Models a toolbar button action.
    /// </summary>
    public class ButtonAction : ClickAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The fully qualified action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="flags">Flags that control the style of the action.</param>
        /// <param name="resourceResolver">A resource resolver that will be used to resolve text and image resources.</param>
        public ButtonAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resourceResolver)
            : base(actionID, path, flags, resourceResolver)
        {
        }
    }
}
