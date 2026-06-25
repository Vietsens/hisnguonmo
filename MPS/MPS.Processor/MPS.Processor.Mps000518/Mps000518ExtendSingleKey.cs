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
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000518
{
    /// <summary>
    /// Các key phái sinh (ngoài cột thô của V_HIS_MEDICAL_CONTACT / HIS_SUPPLIER) cho template Mps000518.
    /// Các key thô của 2 đối tượng trên được sinh tự động qua AddObjectKeyIntoListkey (reflection),
    /// nên ở đây chỉ khai báo các key cần xử lý thêm.
    /// </summary>
    class Mps000518ExtendSingleKey : CommonKey
    {
        /// <summary>Ngày cấp giấy ủy quyền dạng "ngày dd tháng mm năm yyyy" (từ HIS_SUPPLIER.AUTH_LETTER_ISSUE_DATE).</summary>
        internal const string AUTH_LETTER_ISSUE_DATE_STR = "AUTH_LETTER_ISSUE_DATE_STR";

        /// <summary>Tổng tiền bằng số = SUM(VIR_CONTACT_PRICE) của METY + MATY.</summary>
        internal const string SUM_CONTACT_PRICE = "SUM_CONTACT_PRICE";

        /// <summary>Tổng tiền bằng chữ (đọc từ SUM_CONTACT_PRICE).</summary>
        internal const string SUM_CONTACT_PRICE_TEXT = "SUM_CONTACT_PRICE_TEXT";
    }
}
