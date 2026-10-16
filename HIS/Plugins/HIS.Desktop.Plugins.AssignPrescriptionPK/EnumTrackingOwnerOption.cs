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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    /// <summary>
    /// Task 59656 - value of HIS_CONFIG key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption (revised 14/10/2026):
    /// which treatment sheets (HIS_TRACKING) the "Treatment sheet" combo of the order screens shows.
    /// </summary>
    internal enum EnumTrackingOwnerOption
    {
        /// <summary>Empty or any other value: all sheets, behavior unchanged.</summary>
        None = 0,

        /// <summary>1: only sheets created by the ordering user (CREATOR = REQUEST_LOGINNAME); the latest one of the
        /// instruction date is selected by default.</summary>
        OwnOnly = 1
    }
}
