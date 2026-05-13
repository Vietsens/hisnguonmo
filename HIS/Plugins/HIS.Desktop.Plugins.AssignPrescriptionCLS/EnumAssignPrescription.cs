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

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS
{
    public class EnumAssignPrescription
    {
        public enum ACTION_TYPE
        {
            ADD,
            EDIT,
            SAVE
        }

        /// <summary>
        /// Option của config <c>MOS.HIS_SERVICE_REQ.PRESCRIPTION.IS_TRACKING_REQUIRED</c>.
        /// Mapping với giá trị raw int đọc từ HIS_CONFIG.
        /// </summary>
        public enum TRACKING_REQUIRED_OPTION
        {
            /// <summary>Không bắt buộc tờ điều trị — hành vi mặc định</summary>
            NotRequired = 0,

            /// <summary>Required cứng — set Maroon + validation chặn nếu chưa chọn (logic cũ, áp dụng cho mọi đơn)</summary>
            RequiredHardValidate = 1,

            /// <summary>
            /// Required mềm cho điều trị nội trú / cấp cứu — Maroon, KHÔNG validation cứng;
            /// chỉ chặn lưu khi đơn có thuốc + chưa chọn tờ điều trị. Đơn chỉ vật tư vẫn cho lưu.
            /// </summary>
            RequiredSoftForMedicine = 4
        }
    }
}
