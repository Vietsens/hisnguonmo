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

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    /// <summary>
    /// Task 57971: values of config key HIS.Desktop.Plugins.BedRoomPartial.ClsTabOption
    /// (split the "CLS" order tab of the Bed room screen into one tab per paraclinical group).
    /// </summary>
    public enum EnumClsTabOption
    {
        /// <summary>Empty / 0 / other value: former single "CLS" tab, Subclinical result screen as a tree</summary>
        Off = 0,

        /// <summary>1: group tabs; the main groups (Test, Ultrasound, X-ray, Functional, Endoscopy) are always shown, others only with orders</summary>
        ShowMainTabs = 1,

        /// <summary>2: group tabs; only groups having orders are shown</summary>
        ShowTabsWithData = 2
    }
}
