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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130.ADO
{
    public class ConfigSyncADO
    {
        public List<long> branchIds { get; set; }
        public List<long> patientTypeIds { get; set; }
        public List<long> patientTypeTTIds { get; set; }
        public List<long> treatmentTypeIds { get; set; }
        public int? statusId { get; set; }
        public decimal period { get; set; }
        public bool isCheckOutTime { get; set; }
        public bool isCheckCollinearXml { get; set; }
        public string folderPath { get; set; }
        public bool dontSend { get; set; }
        public bool isXML3176 { get; set; }
        //Đồng bộ Khám chữa bệnh (Kết thúc khám/Xuất viện) lên CSDL 4750
        public bool isSyncKcb { get; set; }
        //Đồng bộ KCB (hoàn tất) lên Cổng tiếp nhận — Kho dữ liệu y tế tỉnh Vĩnh Long
        public bool isSyncKcbVlg { get; set; }
        //Đồng bộ KCB theo QĐ 3176 lên Trung tâm điều hành y tế (cổng dữ liệu y tế)
        public bool isSyncKcbHoc { get; set; }
        //Đồng bộ KCB theo QĐ 3176 lên Cổng CSDL Y tế Cần Thơ
        public bool isSyncKcbCt { get; set; }
        //Việc 61380 - Tự động xuất XML hồ sơ nhập viện đủ 48 giờ (CLINICAL_IN_TIME, chạy theo chu kỳ gửi tự động)
        public bool isAutoXml48h { get; set; }

        #region vCong60263 - Gửi tự động theo mốc giờ + ghi nhớ trạng thái bật
        // Tất cả thuộc tính dưới đây nhận giá trị mặc định của kiểu khi đọc lại thiết lập CŨ
        // (ControlState cũ không có các khóa này) -> hành vi giữ nguyên y như trước:
        //   sendModeOption = 0  -> Theo chu kỳ (như cũ)
        //   lookBackDays   = null -> chưa khai, dùng mặc định 1 ngày theo QT08
        //   isResendFailed = false, isAutoStart = false

        /// <summary>Chế độ gửi: 0/1 = Theo chu kỳ (mặc định, như cũ), 2 = Theo mốc giờ.</summary>
        public int sendModeOption { get; set; }

        /// <summary>
        /// Danh sách mốc giờ gửi trong ngày, phân cách bởi ';', dạng HH:mm. VD "10:00;16:00".
        /// Chỉ có hiệu lực khi sendModeOption = 2.
        /// </summary>
        public string scheduleTimes { get; set; }

        /// <summary>
        /// Ngày trong tuần áp dụng lịch, phân cách bởi ',', 1 = Thứ 2 ... 7 = Chủ nhật.
        /// Màn Thiết lập lưu đủ cả bảy ngày khi người dùng chọn hết, nên rỗng nghĩa là
        /// người dùng đã bỏ chọn toàn bộ -> không ngày nào chạy (QT04).
        /// </summary>
        public string scheduleDays { get; set; }

        /// <summary>
        /// Số ngày quét lùi (0 - 31). Rỗng = chưa khai -> dùng mặc định 1 ngày (QT08).
        /// Đặt 0 là cố ý chỉ quét trong ngày hiện tại. Đọc qua GetLookBackDays().
        /// </summary>
        public Nullable<int> lookBackDays { get; set; }

        /// <summary>
        /// Số ngày quét lùi thực dùng: chưa khai thì lấy mặc định 1 ngày theo QT08,
        /// đủ phủ hồ sơ khóa viện phí vào cuối ngày hôm trước.
        /// </summary>
        public int GetLookBackDays()
        {
            return this.lookBackDays.HasValue
                ? this.lookBackDays.Value
                : HIS.Desktop.Plugins.ExportXmlQD130.Base.Xml130Schedule.DEFAULT_LOOK_BACK_DAYS;
        }

        /// <summary>Lượt gửi lấy thêm hồ sơ đã gửi thất bại trong khoảng quét để gửi lại.</summary>
        public bool isResendFailed { get; set; }

        /// <summary>Ghi nhớ tiến trình gửi tự động đang bật: mở lại màn hình thì tự chạy tiếp.</summary>
        public bool isAutoStart { get; set; }

        //Giá trị chuẩn cho sendModeOption
        public const int SEND_MODE_PERIOD = 1;
        public const int SEND_MODE_SCHEDULE = 2;

        /// <summary>Đang dùng chế độ mốc giờ hay không.</summary>
        public bool IsScheduleMode()
        {
            return this.sendModeOption == SEND_MODE_SCHEDULE;
        }
        #endregion
    }
}
