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
using MOS.SDO;
using System;

namespace HIS.Desktop.Plugins.ExpMestChmsUpdate.ADO
{
    /// <summary>
    /// ADO mở rộng HisMedicineInStockSDO phục vụ PTTK 36619 (BV HAGL):
    /// cho phép user nhập Số lượng xuất chuyển và Ghi chú trực tiếp trên grid bên trái.
    /// </summary>
    public class HisMedicineInStockADO : HisMedicineInStockSDO
    {
        // PTTK 36619: Số lượng xuất chuyển user nhập trên grid (không auto-fill — BR04)
        // FieldName trên GridColumn: "AMOUNT_TRANSFER_MEDI"
        public decimal? AMOUNT_TRANSFER_MEDI { get; set; }

        // PTTK 36619: Ghi chú xuất chuyển kho user nhập trên grid
        // FieldName trên GridColumn: "NOTE_TRANSFER_MEDI"
        public string NOTE_TRANSFER_MEDI { get; set; }

        public HisMedicineInStockADO()
        {
        }

        public HisMedicineInStockADO(HisMedicineInStockSDO item)
        {
            try
            {
                if (item == null) return;
                // PTTK 36619 BR04: Không mặc định AMOUNT_TRANSFER_MEDI — user phải tự nhập trên grid
                Inventec.Common.Mapper.DataObjectMapper.Map<HisMedicineInStockSDO>(this, item);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
