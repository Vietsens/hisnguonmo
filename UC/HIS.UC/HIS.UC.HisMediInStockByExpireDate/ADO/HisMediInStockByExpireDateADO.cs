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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.HisMediInStockByExpireDate.ADO
{
    public class HisMediInStockByExpireDateADO : HisMedicineInStockSDO
    {
        public bool? bIsLeaf { get; set; }
        public string CONCRETE_ID__IN_DATE { get; set; }
        public string PARENT_ID__IN_DATE { get; set; }
        public bool IS_MEDI_MATE { get; set; }

        public HisMediInStockByExpireDateADO()
        {
        }

        public HisMediInStockByExpireDateADO(HisMedicineInStockSDO HisMediInStockByExpireDate)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<HisMediInStockByExpireDateADO>(this, HisMediInStockByExpireDate);
            bIsLeaf = (HisMediInStockByExpireDate.IS_LEAF == 1);

        }
    }
}
