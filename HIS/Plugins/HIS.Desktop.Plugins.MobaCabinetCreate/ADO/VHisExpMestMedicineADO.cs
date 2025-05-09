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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MobaCabinetCreate.ADO
{
    public class VHisExpMestMedicineADO : V_HIS_EXP_MEST_MEDICINE
    {
        public decimal MOBA_AMOUNT { get; set; }
        public bool IsMoba { get; set; }
        public decimal CAN_MOBA_AMOUNT { get; set; }

        public decimal? VIR_TOTAL_IMP_PRICE { get; set; }
        public long? IMP_MEDI_STOCK_ID { get; set; }
        public string IMP_MEDI_STOCK_NAME { get; set; }

        public VHisExpMestMedicineADO()
        {
        }

        public VHisExpMestMedicineADO(V_HIS_EXP_MEST_MEDICINE data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<VHisExpMestMedicineADO>(this, data);
                    this.MEDICINE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                    this.VIR_TOTAL_IMP_PRICE = this.AMOUNT * (this.PRICE ?? 0) * (1 + (this.VAT_RATIO ?? 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
