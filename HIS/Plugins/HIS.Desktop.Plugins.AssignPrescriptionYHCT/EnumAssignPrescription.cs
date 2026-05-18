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

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT
{
    public class EnumAssignPrescription
    {
        /// <summary>
        /// Option của config <c>MOS.HIS_SERVICE_REQ.PRESCRIPTION.IS_TRACKING_REQUIRED</c>.
        /// Mapping với giá trị raw int đọc từ HIS_CONFIG.
        /// </summary>
        public enum TRACKING_REQUIRED_OPTION
        {
            /// <summary>Không bắt buộc tờ điều trị — hành vi mặc định</summary>
            NotRequired = 0,

            /// <summary>Bắt buộc nhập tờ điều trị khi kê đơn điều trị (mở từ buồng) - mọi BN, và kê đơn tủ trực - BN nội/ngoại trú</summary>
            RequiredForTreatmentRoomOrCabinet = 1,

            /// <summary>Bắt buộc nhập tờ điều trị khi kê đơn phòng khám/tủ trực/điều trị - BN nội trú; phòng khám/điều trị - BN cấp cứu</summary>
            RequiredForInpatientOrEmergency = 2,

            /// <summary>Bắt buộc nhập tờ điều trị khi kê đơn THUỐC (vật tư không bắt buộc) khi kê đơn điều trị - mọi BN, tủ trực - BN nội/ngoại trú</summary>
            RequiredSoftForMedicineTreatmentOrCabinet = 3,

            /// <summary>
            /// Bắt buộc nhập tờ điều trị khi kê đơn THUỐC (vật tư không bắt buộc) khi kê đơn phòng khám/tủ trực/điều trị/CLS/YHCT —
            /// áp dụng cho BN nội trú hoặc cấp cứu. Maroon (không validate cứng); chỉ chặn lưu khi đơn có thuốc + chưa chọn tờ điều trị.
            /// </summary>
            RequiredSoftForMedicine = 4
        }
    }
}
