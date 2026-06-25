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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000518.PDO
{
    /// <summary>
    /// PDO biểu in Biên bản/Hợp đồng cung ứng thuốc - vật tư của nhà cung cấp.
    /// Đầu vào:
    ///   - V_HIS_MEDICAL_CONTRACT  : thông tin chung của hợp đồng (header).
    ///   - HIS_SUPPLIER            : thông tin nhà cung cấp (kèm ngày cấp giấy ủy quyền).
    ///   - V_HIS_MEDI_CONTRACT_METY: danh sách thuốc trong hợp đồng (band "Mety").
    ///   - V_HIS_MEDI_CONTRACT_MATY: danh sách vật tư trong hợp đồng (band "Maty").
    /// Tổng tiền = SUM(VIR_CONTRACT_PRICE) của cả METY và MATY.
    /// </summary>
    public partial class Mps000518PDO : RDOBase
    {
        public V_HIS_MEDICAL_CONTRACT MedicalContact { get; set; }
        public HIS_SUPPLIER Supplier { get; set; }
        public List<V_HIS_MEDI_CONTRACT_METY> ListMety { get; set; }
        public List<V_HIS_MEDI_CONTRACT_MATY> ListMaty { get; set; }

        public Mps000518PDO(
            V_HIS_MEDICAL_CONTRACT MedicalContact,
            HIS_SUPPLIER Supplier,
            List<V_HIS_MEDI_CONTRACT_METY> ListMety,
            List<V_HIS_MEDI_CONTRACT_MATY> ListMaty
            )
        {
            try
            {
                this.MedicalContact = MedicalContact;
                this.Supplier = Supplier;
                this.ListMety = ListMety;
                this.ListMaty = ListMaty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
