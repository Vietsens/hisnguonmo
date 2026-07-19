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
namespace HIS.Desktop.Plugins.HisMediStock
{
    /// <summary>
    /// Loại kho lựa chọn tại ô "chọn nhiều loại kho" trên màn hình danh mục kho (PTTK_42516).
    /// Mỗi giá trị tương ứng với 1 cờ độc lập trên HIS_MEDI_STOCK.
    /// BR1: các cờ độc lập nhau - một kho có thể bật nhiều loại cùng lúc.
    /// Value dùng làm khóa của từng CheckedListBoxItem trong cboStockTypes.
    /// </summary>
    public enum EnumMediStockType
    {
        /// <summary>Là tủ trực - ánh xạ cột HIS_MEDI_STOCK.IS_CABINET.</summary>
        Cabinet = 1,

        /// <summary>Là kho điều trị - ánh xạ cột HIS_MEDI_STOCK.IS_TREATMENT_STOCK.</summary>
        Treatment = 2,

        /// <summary>Là kho thuốc ngoại trú - ánh xạ cột HIS_MEDI_STOCK.IS_OUTPATIENT_STOCK.</summary>
        Outpatient = 3
    }
}
