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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestChmsCreate.ADO
{
    public class HisBloodTypeInStockADO : HisBloodTypeInStockSDO
    {
        public bool IsCheck { get; set; }
        public long? MEDI_STOCK_ID { get; set; }
        public HisBloodTypeInStockADO()
        { }

        public HisBloodTypeInStockADO(HisBloodTypeInStockSDO item)
        {
            if (item != null)
            {
                this.Amount = item.Amount;
                this.BloodTypeCode = item.BloodTypeCode;
                this.BloodTypeHeinName = item.BloodTypeHeinName;
                this.BloodTypeName = item.BloodTypeName;
                this.Id = item.Id;
                this.IsActive = item.IsActive;
                this.IsLeaf = item.IsLeaf;
                this.MediStockId = item.MediStockId;
                this.NumOrder = item.NumOrder;
                this.ParentId = item.ParentId;
                this.ServiceId = item.ServiceId;
                this.Volume = item.Volume;
                this.MEDI_STOCK_ID = item.MediStockId;
            }
        }
    }
}
