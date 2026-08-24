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
using System.Drawing;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.ADO
{
    /// <summary>
    /// Thông tin một giường bệnh — đúng những gì cần để vẽ một thẻ giường trên bảng.
    /// </summary>
    public class BedInfoADO
    {
        /// <summary>HIS_BED.ID. Để 0 nếu nguồn dữ liệu không có ID, khi đó khóa theo BED_CODE.</summary>
        public long BED_ID { get; set; }

        /// <summary>Mã giường, ví dụ 201-A</summary>
        public string BED_CODE { get; set; }

        /// <summary>Giường trống thì các trường bên dưới bỏ trống hết</summary>
        public bool IS_EMPTY { get; set; }

        public string PATIENT_NAME { get; set; }
        public int AGE { get; set; }
        public string GENDER_NAME { get; set; }

        /// <summary>Chẩn đoán hiển thị, thường ghép sẵn ICD_CODE + ICD_NAME</summary>
        public string ICD_NAME { get; set; }

        public string DOCTOR_USERNAME { get; set; }

        /// <summary>HIS_CARE_LEVEL.ID gốc, giữ lại để lọc và đối chiếu</summary>
        public long CARE_LEVEL_ID { get; set; }

        /// <summary>Mức tô màu, quy đổi từ CARE_LEVEL_ID</summary>
        public CareLevel CARE_LEVEL { get; set; }

        /// <summary>
        /// Màu do danh mục HIS cấu hình (DISPLAY_COLOR). Có giá trị thì lấn át màu suy từ CARE_LEVEL,
        /// nhờ vậy bệnh viện đổi màu trên danh mục là đổi được ngay, không phải build lại.
        /// </summary>
        public Color? DISPLAY_COLOR { get; set; }

        public bool HAS_VITAL_SIGN { get; set; }
        public string PULSE { get; set; }
        public string TEMPERATURE { get; set; }
        public string BLOOD_PRESSURE { get; set; }

        /// <summary>
        /// Khóa đối chiếu khi làm mới. Ưu tiên ID thật; nguồn không có ID thì rơi về BED_CODE.
        /// </summary>
        public string BedKey
        {
            get { return BED_ID != 0 ? "#" + BED_ID : (BED_CODE ?? string.Empty); }
        }

        /// <summary>
        /// Dấu vân tay để biết dữ liệu có thực sự đổi hay không.
        /// THÊM TRƯỜNG MỚI HIỂN THỊ TRÊN THẺ THÌ PHẢI THÊM VÀO ĐÂY,
        /// không thì trường đó sẽ không bao giờ tự cập nhật.
        /// </summary>
        public string BuildSignature()
        {
            return string.Concat(
                BED_CODE, "|", IS_EMPTY ? "1" : "0", "|",
                PATIENT_NAME, "|", AGE.ToString(), "|", GENDER_NAME, "|",
                ICD_NAME, "|", DOCTOR_USERNAME, "|", CARE_LEVEL_ID.ToString(), "|",
                DISPLAY_COLOR.HasValue ? DISPLAY_COLOR.Value.ToArgb().ToString() : "-", "|",
                HAS_VITAL_SIGN ? "1" : "0", "|", PULSE, "|", TEMPERATURE, "|", BLOOD_PRESSURE);
        }
    }
}
