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
    /// Declares a menu button action with the specifed action identifier and path hint.
    /// </summary>
    public class MenuActionAttribute : ClickActionAttribute
    {
        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action.</param>
        /// <param name="pathHint">The suggested location of this action in the menu model.</param>
        public MenuActionAttribute(string actionID, string pathHint)
            : base(actionID, pathHint)
        {
        }

        /// <summary>
        /// Creates the <see cref="MenuAction"/> represented by this attribute.
        /// </summary>
        /// <param name="actionID">The logical action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="flags">Flags that specify the click behaviour of the action.</param>
        /// <param name="resolver">The object used to resolve the action path and icons.</param>
        protected override ClickAction CreateAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resolver)
        {
            return new MenuAction(actionID, path, flags, resolver);
        }
    }
}
