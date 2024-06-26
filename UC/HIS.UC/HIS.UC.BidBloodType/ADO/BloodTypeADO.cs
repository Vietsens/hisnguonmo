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

namespace HIS.UC.BloodType.ADO
{
    public class BloodTypeADO : V_HIS_BLOOD_TYPE
    {      
        public bool? IsLeaf { get; set; }
        public decimal? AMOUNT { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public BloodTypeADO()
        {
        }

        public BloodTypeADO(V_HIS_BLOOD_TYPE BloodType)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<BloodTypeADO>(this, BloodType);
            IsLeaf = (BloodType.IS_LEAF == 1);                                   

        }

        public BloodTypeADO(BloodTypeADO BloodType)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<BloodTypeADO>(this, BloodType);
            IsLeaf = (BloodType.IS_LEAF == 1);
            this.AMOUNT = BloodType.AMOUNT;
            this.SUPPLIER_ID = BloodType.SUPPLIER_ID;
            this.SUPPLIER_CODE = BloodType.SUPPLIER_CODE;
            this.SUPPLIER_NAME = BloodType.SUPPLIER_NAME;
        }
    }
}
