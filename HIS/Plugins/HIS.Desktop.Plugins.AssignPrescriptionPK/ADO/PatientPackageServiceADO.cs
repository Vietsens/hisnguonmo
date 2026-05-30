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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ADO
{
    /// <summary>
    /// Kết quả popup chọn gói trả về form kê đơn — mỗi dịch vụ thuốc/vật tư được chọn kèm số lượng lần này.
    /// </summary>
    public class PatientPackageServiceADO
    {
        /// <summary>ID dịch vụ (V_HIS_PATIENT_PACKAGE_DT.SERVICE_ID).</summary>
        public long ServiceId { get; set; }

        /// <summary>Loại dịch vụ — phân biệt thuốc / vật tư.</summary>
        public long ServiceTypeId { get; set; }

        /// <summary>Số lượng sử dụng lần này.</summary>
        public decimal Amount { get; set; }

        /// <summary>ID gói bệnh nhân (HIS_PATIENT_PACKAGE.ID).</summary>
        public long PatientPackageId { get; set; }

        /// <summary>Tên gói bệnh nhân đại diện.</summary>
        public string PatientPackageName { get; set; }

        /// <summary>Mã dịch vụ — phục vụ log/cảnh báo.</summary>
        public string ServiceCode { get; set; }

        /// <summary>Tên dịch vụ — phục vụ log/cảnh báo.</summary>
        public string ServiceName { get; set; }

        /// <summary>Đối tượng thanh toán của gói (HIS_PATIENT_PACKAGE.PATIENT_TYPE_ID) — override mặc định.</summary>
        public long PatientTypeId { get; set; }

        /// <summary>Đơn giá theo gói (HIS_PATIENT_PACKAGE_DT.UNIT_PRICE) — override giá mặc định.</summary>
        public decimal UnitPrice { get; set; }
    }
}
