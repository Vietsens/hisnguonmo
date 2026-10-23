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
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.ADO
{
    /// <summary>
    /// Filter conditions chosen on the form, used to load and build the disclosure rows
    /// </summary>
    public class PublicMedicineFilterADO
    {
        /// <summary>Tạo từ (yyyyMMddHHmmss)</summary>
        public long TimeFrom { get; set; }
        /// <summary>Tạo đến (yyyyMMddHHmmss)</summary>
        public long TimeTo { get; set; }
        /// <summary>Khoa yêu cầu, null = no filter</summary>
        public long? RequestDepartmentId { get; set; }
        /// <summary>Phòng yêu cầu, null = no filter</summary>
        public long? RequestRoomId { get; set; }
        /// <summary>ĐTTT selected, null or empty = all</summary>
        public List<long> PatientTypeIds { get; set; }

        public bool IsMedicine { get; set; }
        public bool IsMaterial { get; set; }
        public bool IsBlood { get; set; }
        public bool IsChemical { get; set; }
        /// <summary>Hao phí: true = keep rows with IS_EXPEND = 1</summary>
        public bool IsExpend { get; set; }
        /// <summary>Tách HDSD: true = split medicine rows by TUTORIAL</summary>
        public bool IsTachHDSD { get; set; }
    }
}
