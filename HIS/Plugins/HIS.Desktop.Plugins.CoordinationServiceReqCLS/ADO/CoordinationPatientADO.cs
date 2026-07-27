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

namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS.ADO
{
    /// <summary>
    /// Dòng lưới danh sách bệnh nhân (lưới trái, mục 5.3) — gom theo mã điều trị.
    /// Giữ danh sách y lệnh con để đổ sang lưới chi tiết (lưới phải, mục 5.4).
    /// </summary>
    public class CoordinationPatientADO
    {
        /// <summary>Mã điều trị — khóa gom nhóm.</summary>
        public string TREATMENT_CODE { get; set; }

        /// <summary>Số giường.</summary>
        public string BED_NAME { get; set; }

        /// <summary>Tên bệnh nhân.</summary>
        public string PATIENT_NAME { get; set; }

        /// <summary>Ngày sinh (long).</summary>
        public long? PATIENT_DOB { get; set; }

        /// <summary>Địa chỉ.</summary>
        public string PATIENT_ADDRESS { get; set; }

        /// <summary>Giới tính.</summary>
        public string PATIENT_GENDER_NAME { get; set; }

        /// <summary>Đối tượng bệnh nhân.</summary>
        public string PATIENT_TYPE_NAME { get; set; }

        /// <summary>Hướng giải quyết đại diện của điều trị (rỗng → hiển thị "Chưa xử lý").</summary>
        public string SOLUTION_DES { get; set; }

        /// <summary>
        /// Mức cảnh báo cao nhất trong các y lệnh của điều trị (dùng tô màu dòng, mục 5.5).
        /// null/1 trắng, 2 vàng, 3 đỏ.
        /// </summary>
        public long? WARNING { get; set; }

        /// <summary>
        /// Trạng thái tổng hợp CLS đại diện của điều trị (mức thấp nhất trong các y lệnh).
        /// 1 Chưa thực hiện / 2 Đang thực hiện / 3 Đủ kết quả (theo SERVICE_REQ_STT_ID tài liệu mục c).
        /// </summary>
        public long? SERVICE_REQ_STT_ID { get; set; }

        /// <summary>Trạng thái hiển thị dạng chữ (pre-compute trước khi bind).</summary>
        public string StatusDisplay { get; set; }

        /// <summary>Cảnh báo hiển thị dạng chữ theo WARNING (pre-compute trước khi bind).</summary>
        public string WarningDisplay { get; set; }

        /// <summary>Ngày sinh dạng chuỗi hiển thị (pre-compute trước khi bind).</summary>
        public string PatientDobStr { get; set; }

        /// <summary>Hướng giải quyết hiển thị: rỗng → "Chưa xử lý" (pre-compute trước khi bind).</summary>
        public string SolutionDesDisplay { get; set; }

        /// <summary>Danh sách y lệnh CLS của điều trị (đổ sang lưới chi tiết).</summary>
        public List<HisServiceReqGetServiceReqCLSSDO> ServiceReqs { get; set; }

        public CoordinationPatientADO()
        {
            this.ServiceReqs = new List<HisServiceReqGetServiceReqCLSSDO>();
        }
    }
}
