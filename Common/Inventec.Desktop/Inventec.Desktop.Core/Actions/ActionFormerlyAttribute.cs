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

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Indicates to the framework that an action used to have one or more different IDs
    /// and should replace the old entries in the action model configuration.
    /// </summary>
    public class ActionFormerlyAttribute : ActionDecoratorAttribute
    {
        private readonly string[] _formerActionIds;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The id of the action.</param>
        /// <param name="formerActionIds">One or more fully qualified "former" action IDs.</param>
        public ActionFormerlyAttribute(string actionID, params string[] formerActionIds) : base(actionID)
        {
            _formerActionIds = formerActionIds;
        }

        public override void Apply(IActionBuildingContext builder)
        {
            foreach (var formerActionId in _formerActionIds)
                builder.Action.FormerActionIDs.Add(formerActionId);
        }
    }
}
