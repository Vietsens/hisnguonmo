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
    /// Filter cho API api/HisServiceReq/GetServiceReqCLS (mục 4.1 tài liệu).
    /// TẠM khai báo cục bộ; khi MOS.Filter có HisServiceReqViewFilterQuery thì đổi using sang MOS.Filter.
    /// </summary>
    public class HisServiceReqViewFilterQuery
    {
        /// <summary>Ngày y lệnh - từ ngày (long yyyyMMddHHmmss). BẮT BUỘC.</summary>
        public long? INTRUCTION_DATE_FROM { get; set; }

        /// <summary>Ngày y lệnh - đến ngày (long yyyyMMddHHmmss). BẮT BUỘC.</summary>
        public long? INTRUCTION_DATE_TO { get; set; }

        /// <summary>Mã điều trị (tùy chọn).</summary>
        public string TREATMENT_CODE { get; set; }

        /// <summary>Mã điều trị - tìm chính xác (đã pad 0 cho đủ 12 số).</summary>
        public string TREATMENT_CODE__EXACT { get; set; }

        /// <summary>Mã bệnh nhân (mã y tế) (tùy chọn).</summary>
        public string PATIENT_CODE { get; set; }

        /// <summary>Họ tên bệnh nhân (tùy chọn).</summary>
        public string PATIENT_NAME { get; set; }

        /// <summary>ID phòng xử lý hiện tại. BẮT BUỘC.</summary>
        public long? REQUEST_ROOM_ID { get; set; }
    }
}
