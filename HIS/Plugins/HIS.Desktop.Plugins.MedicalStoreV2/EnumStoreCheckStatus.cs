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

namespace HIS.Desktop.Plugins.MedicalStoreV2
{
    /// <summary>
    /// Value of the "Check status" filter on the treatment list of Medical store (new) screen.
    /// </summary>
    public enum EnumStoreCheckStatus
    {
        /// <summary>No filter on the check status. Kept as a real entry of the list so that
        /// it can be picked back after choosing one of the other two.</summary>
        All = 0,

        /// <summary>Already marked as checked - the treatment has a store check time</summary>
        Checked = 1,

        /// <summary>Not marked as checked yet</summary>
        NotChecked = 2
    }

    /// <summary>
    /// Constants of the "mark medical record as checked" feature.
    /// </summary>
    internal class StoreCheckConstant
    {
        /// <summary>Endpoint that marks treatments as checked</summary>
        internal const string URI__CHECK_STORE = "api/HisTreatment/CheckStore";

        /// <summary>
        /// ACS control code granting the right to mark a record as checked.
        /// Declared in the ACS control catalog as "Đánh dấu hồ sơ đã kiểm tra".
        /// A role flagged as full access owns every control automatically.
        /// </summary>
        internal const string CONTROL_CODE__STORE_CHECK = "HIS000059";
    }
}
