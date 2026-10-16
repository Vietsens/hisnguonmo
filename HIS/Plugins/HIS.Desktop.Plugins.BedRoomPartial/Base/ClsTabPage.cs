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
using DevExpress.XtraTab;

namespace HIS.Desktop.Plugins.BedRoomPartial.Base
{
    /// <summary>
    /// Task 57971: one paraclinical group tab of the Bed room order tab bar (created at runtime).
    /// </summary>
    internal class ClsTabPage
    {
        /// <summary>Group code (ClsTabGroupStore.GROUP_*)</summary>
        internal string GroupCode { get; set; }
        /// <summary>Caption without the service count</summary>
        internal string BaseCaption { get; set; }
        /// <summary>Full group name used in messages (the caption may be an abbreviation, e.g. TDCN)</summary>
        internal string FullName { get; set; }
        /// <summary>Number of services of the current load</summary>
        internal int ServiceCount { get; set; }
        internal XtraTabPage Page { get; set; }
        internal UCTreeListService TreeUc { get; set; }
    }
}
