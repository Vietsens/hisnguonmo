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
namespace HIS.Desktop.Plugins.PatientPackageRegister
{
    /// <summary>
    /// 1 dòng dịch vụ trong gói — nguồn dữ liệu cho lưới "Dịch vụ trong gói".
    /// SL (AMOUNT) và Thành tiền (TOTAL_PRICE) cho phép sửa trên lưới.
    /// </summary>
    public class PackageServiceADO
    {
        public long SERVICE_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        // Đơn giá lấy từ chính sách giá (để tính Thành tiền)
        public decimal PRICE { get; set; }
        // Số lượng
        public decimal AMOUNT { get; set; }
        // Thành tiền = AMOUNT * PRICE (tự tính khi đổi SL, vẫn cho sửa tay)
        public decimal TOTAL_PRICE { get; set; }
        // 1 = dịch vụ phí gói (HIS_NONE_MEDI_SERVICE), 0 = dịch vụ kỹ thuật thường
        public int IS_NONE_SERVICE { get; set; }
        // SL đã sử dụng (chỉ định/kê đơn) — chỉ có ở gói đã tồn tại; tạo mới = 0
        public decimal AMOUNT_USED { get; set; }
        // SL đã thanh toán — > 0 nghĩa là đã thanh toán (không cho xóa); tạo mới = 0
        public decimal AMOUNT_PREPAID { get; set; }
        // ID dòng HIS_PATIENT_PACKAGE_DT khi sửa (0 = dòng mới thêm, chưa lưu)
        public long DT_ID { get; set; }
    }
}
