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
namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.ADO
{
    /// <summary>
    /// Số liệu trên dải thống kê đầu màn hình.
    /// </summary>
    public class BoardSummaryADO
    {
        public int IN_PATIENT_CURRENT { get; set; }
        public int WAIT_IN_PATIENT { get; set; }
        public int IN_PATIENT_TODAY { get; set; }
        public int OUT_PATIENT_TODAY { get; set; }

        public int BED_TOTAL { get; set; }
        public int BED_USED { get; set; }
        public int BED_EMPTY { get; set; }

        public int CARE_LEVEL_1 { get; set; }
        public int CARE_LEVEL_2 { get; set; }
        public int CARE_LEVEL_3 { get; set; }
        public int SPECIAL_ORDER { get; set; }
    }
}
