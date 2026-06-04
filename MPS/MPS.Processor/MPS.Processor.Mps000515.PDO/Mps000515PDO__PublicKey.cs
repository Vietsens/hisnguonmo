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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System.Collections.Generic;

namespace MPS.Processor.Mps000515.PDO
{
    public partial class Mps000515PDO : RDOBase
    {
        public V_HIS_PATIENT currentPatient { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        public HIS_TREATMENT currentTreatment { get; set; }
        public List<Mps000515_ExamRoomRow> ExamRooms { get; set; }
        public string Gate { get; set; }
    }

    /// <summary>
    /// Một dòng trong bảng "Danh sách phòng khám đã đăng ký" của phiếu gộp MPS000515.
    /// Mỗi dòng tương ứng 1 yêu cầu khám (V_HIS_SERVICE_REQ) đã đăng ký trên màn hình Tiếp đón 2.
    /// </summary>
    public class Mps000515_ExamRoomRow
    {
        /// <summary>Số thứ tự dòng</summary>
        public int STT { get; set; }

        /// <summary>Mã phòng khám (EXECUTE_ROOM_CODE)</summary>
        public string ROOM_CODE { get; set; }

        /// <summary>Tên phòng khám (EXECUTE_ROOM_NAME)</summary>
        public string ROOM_NAME { get; set; }

        /// <summary>Tên khoa quản lý phòng (EXECUTE_DEPARTMENT_NAME)</summary>
        public string DEPARTMENT_NAME { get; set; }

        /// <summary>Địa chỉ phòng khám - Phòng/Tầng/Khu (EXECUTE_ROOM_ADDRESS)</summary>
        public string ROOM_ADDRESS { get; set; }

        /// <summary>Tên dịch vụ khám tương ứng - công khám (TDL_SERVICE_NAME)</summary>
        public string SERVICE_NAME { get; set; }

        /// <summary>Số thứ tự khám của BN trong phòng khám (gate number / call number)</summary>
        public long? NUM_ORDER { get; set; }

        /// <summary>Thông tin bổ sung nếu có (NOTE)</summary>
        public string NOTE { get; set; }
    }
}
