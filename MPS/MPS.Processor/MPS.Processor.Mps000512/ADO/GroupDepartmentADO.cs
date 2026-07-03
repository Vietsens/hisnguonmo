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
namespace MPS.Processor.Mps000512.ADO
{
    /// <summary>
    /// Dòng master gom theo khoa xử lý / phòng xử lý (port từ Mps000508 / Mps000510). 
    /// Nối với bộ ServiceExeRoom qua GROUP_DEPARTMENT_ID (khoa) hoặc GROUP_ROOM_ID (phòng).
    /// </summary>
    public class GroupDepartmentADO
    {
        // Khóa đối tượng BHYT - để gom/nối subtotal khoa-phòng theo từng đối tượng (giống Mps000304 DepaRoom).
        public string KEY_PATY_ALTER { get; set; }
        public long GROUP_DEPARTMENT_ID { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public short? IS_CLINICAL { get; set; }

        public long GROUP_ROOM_ID { get; set; }
        public string ROOM_CODE { get; set; }
        public string ROOM_NAME { get; set; }

        // Khóa sắp xếp hiển thị: NUM_ORDER nhỏ nhất của loại dịch vụ nằm trong khoa/phòng
        // -> khoa/phòng nào chứa loại dịch vụ có NUM_ORDER nhỏ (vd khám) sẽ lên trước. Không ảnh hưởng tiền.
        public long MIN_NUM_ORDER { get; set; }

        // Tổng tiền của khoa/phòng
        public decimal TOTAL_PRICE { get; set; }                 // VIR_TOTAL_PRICE_NO_EXPEND
        public decimal TOTAL_PRICE_BHYT { get; set; }
        public decimal TOTAL_HEIN_PRICE { get; set; }            // BHYT trả
        public decimal TOTAL_PATIENT_PRICE { get; set; }         // BN cùng chi trả
        public decimal TOTAL_PATIENT_PRICE_SELF { get; set; }    // BN tự trả
        public decimal OTHER_SOURCE_PRICE { get; set; }
        public decimal TOTAL_PRICE_VP { get; set; }
        public decimal TOTAL_PATIENT_PRICE_LEFT { get; set; }

        // Alias trùng tên với bộ HeinServiceType để template dùng chung tên key (cùng giá trị).
        public decimal TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE { get; set; }
        public decimal TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE { get; set; }
        // Nullable: khoa khám bệnh sẽ để trống (null) - giống Mps000304
        public decimal? TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE { get; set; }
    }
}
