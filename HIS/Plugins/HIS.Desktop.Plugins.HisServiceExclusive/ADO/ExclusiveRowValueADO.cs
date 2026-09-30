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
namespace HIS.Desktop.Plugins.HisServiceExclusive.ADO
{
    /// <summary>
    /// Values declared on ONE row of the right grid (one exclusive pair of the selected base service):
    /// handle type (HANDLE_TYPE_ID), in-use state (IS_ACTIVE) and note (NOTE) of HIS_SERVICE_EXCLUSIVE.
    /// Task 57452 - test feedback 23/09/2026: values are declared and saved per row instead of one shared panel.
    /// </summary>
    public class ExclusiveRowValueADO
    {
        /// <summary>HANDLE_TYPE_ID: HandleType.Warning or HandleType.Block</summary>
        public short HandleTypeId { get; set; }

        /// <summary>true = IS_ACTIVE__TRUE (in use), false = IS_ACTIVE__FALSE (not in use)</summary>
        public bool IsActive { get; set; }

        /// <summary>NOTE, null when empty</summary>
        public string Note { get; set; }
    }
}
