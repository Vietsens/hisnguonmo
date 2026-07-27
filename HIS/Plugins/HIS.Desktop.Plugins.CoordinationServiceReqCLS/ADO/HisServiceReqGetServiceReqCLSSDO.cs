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

namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS.ADO
{
    /// <summary>
    /// Đối tượng trả về của API api/HisServiceReq/GetServiceReqCLS (mục 4.1 tài liệu).
    /// TẠM khai báo cục bộ để frontend build độc lập khi backend chưa bổ sung type vào MOS.SDO.
    /// Khi MOS.SDO đã có HisServiceReqGetServiceReqCLSSDO: xóa file này và đổi using sang MOS.SDO.
    /// Tên class + field GIỮ NGUYÊN theo hợp đồng backend để không phải sửa code sử dụng.
    /// </summary>
    public class HisServiceReqGetServiceReqCLSSDO
    {
        /// <summary>ID y lệnh (HIS_SERVICE_REQ.ID).</summary>
        public long ID { get; set; }

        /// <summary>Mã điều trị (TDL_TREATMENT_CODE).</summary>
        public string TREATMENT_CODE { get; set; }

        /// <summary>Mã y lệnh (HIS_SERVICE_REQ.SERVICE_REQ_CODE).</summary>
        public string SERVICE_REQ_CODE { get; set; }

        /// <summary>Thời gian y lệnh (long yyyyMMddHHmmss).</summary>
        public long? INTRUCTION_TIME { get; set; }

        /// <summary>Danh sách tên dịch vụ trong y lệnh (gộp từ HIS_SERE_SERV).</summary>
        public string SERVICE_NAMES { get; set; }

        /// <summary>Số giường (nếu có y lệnh giường).</summary>
        public string BED_NAME { get; set; }

        /// <summary>Tên bệnh nhân (TDL_PATIENT_NAME).</summary>
        public string PATIENT_NAME { get; set; }

        /// <summary>Ngày sinh bệnh nhân (long).</summary>
        public long? PATIENT_DOB { get; set; }

        /// <summary>Địa chỉ bệnh nhân (TDL_PATIENT_ADDRESS).</summary>
        public string PATIENT_ADDRESS { get; set; }

        /// <summary>Trạng thái tổng hợp CLS: 1 Chưa TH / 2 Đang TH / 3 Đủ KQ.</summary>
        public long? SERVICE_REQ_STT_ID { get; set; }

        /// <summary>Hướng giải quyết (HIS_SERVICE_REQ.SOLUTION_DES).</summary>
        public string SOLUTION_DES { get; set; }

        /// <summary>Tài khoản người xem / xử lý (HIS_SERVICE_REQ.VIEW_LOGINNAME).</summary>
        public string VIEW_LOGINNAME { get; set; }

        /// <summary>Giới tính (TDL_PATIENT_GENDER_NAME).</summary>
        public string PATIENT_GENDER_NAME { get; set; }

        /// <summary>Đối tượng bệnh nhân (BHYT, Viện phí, Nước ngoài...).</summary>
        public string PATIENT_TYPE_NAME { get; set; }

        /// <summary>
        /// Trạng thái chỉ số xét nghiệm: 1 bình thường / 2 chưa vượt ngưỡng / 3 vượt ngưỡng.
        /// null khi y lệnh không thuộc loại xét nghiệm / giải phẫu bệnh lý (tô màu trắng).
        /// </summary>
        public long? WARNING { get; set; }
    }
}
