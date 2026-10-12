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

namespace HIS.Desktop.Plugins.AssignService
{
    /// <summary>
    /// Task 59656 - value of HIS_CONFIG key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption:
    /// an order (medicine, material, technical service) may only be attached to a treatment sheet
    /// (HIS_TRACKING) created by the ordering user (REQUEST_LOGINNAME).
    /// </summary>
    internal enum EnumTrackingOwnerOption
    {
        /// <summary>Empty or any other value: no check, behavior unchanged.</summary>
        None = 0,

        /// <summary>1: warn and ask whether to continue.</summary>
        Warning = 1,

        /// <summary>2: block saving.</summary>
        Block = 2
    }
}
