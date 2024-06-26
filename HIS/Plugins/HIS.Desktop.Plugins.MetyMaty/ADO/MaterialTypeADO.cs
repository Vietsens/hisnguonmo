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

namespace HIS.Desktop.Plugins.MetyMaty.ADO
{
    public class MaterialTypeADO : HIS_MATERIAL_TYPE
    {
        public long MATERIAL_TYPE_ID { get; set; }
        public long METY_PRODUCT_ID { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string REPLACE_MATERIAL_TYPE_NAME { get; set; }
        public long? REPLACE_MATERIAL_TYPE_ID { get; set; }


        public decimal amount { get; set; }
        public decimal DD_AMOUNT { get; set; }
        public decimal YCD_AMOUNT { get; set; }

        public decimal CURRENT_DD_AMOUNT { get; set; }
        public decimal CURRENT_YC_AMOUNT { get; set; }
        public decimal TT_AMOUNT { get; set; }
        public decimal TON_KHO { get; set; }
        public decimal AVAIL_AMOUNT { get; set; }

        public bool IsReplace { get; set; }
        public bool IsApproved { get; set; }
        public bool check { get; set; }

        public List<HIS_EXP_MEST_MATY_REQ> Requests { get; set; }

        public MaterialTypeADO()
        {

        }

        public MaterialTypeADO(List<HIS_EXP_MEST_MATY_REQ> materials)
        {
            var first = materials.FirstOrDefault();
            this.Requests = materials;
            this.MATERIAL_TYPE_ID = first.MATERIAL_TYPE_ID;
            this.amount = materials.Sum(s => s.AMOUNT);
            this.DD_AMOUNT = materials.Sum(s => s.DD_AMOUNT ?? 0);
            this.CURRENT_DD_AMOUNT = this.DD_AMOUNT;
        }

        public MaterialTypeADO(List<HIS_EXP_MEST_MATERIAL> materials)
        {
            var first = materials.FirstOrDefault();
            this.MATERIAL_TYPE_ID = first.TDL_MATERIAL_TYPE_ID ?? 0;
            this.amount = 0;
            this.DD_AMOUNT = materials.Sum(s => s.AMOUNT);
        }
    }
}
