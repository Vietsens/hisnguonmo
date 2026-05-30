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
    /// Dòng dịch vụ phí gói (HIS_NONE_MEDI_SERVICE) hiển thị trong popup chọn phí gói.
    /// Các trường _NAME đã được tra cứu từ BackendDataWorker (đơn vị tính, loại hàng hóa).
    /// </summary>
    public class NoneMediServiceADO
    {
        public long SERVICE_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        // Đơn giá gốc
        public decimal PRICE { get; set; }
        public decimal VAT_RATIO { get; set; }
        // Tên đơn vị tính (HIS_SERVICE_UNIT theo SERVICE_UNIT_ID)
        public string SERVICE_UNIT_NAME { get; set; }
        // Tên loại (HIS_GOODS_TYPE theo GOODS_TYPE_ID)
        public string GOODS_TYPE_NAME { get; set; }
    }
}
