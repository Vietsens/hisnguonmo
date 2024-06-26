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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000336.PDO
{
    public class MaterialTypeViewADO : MOS.SDO.HisMaterialTypeView1SDO
    {
        public decimal? EXP_PRICE_1 { get; set; }
        public decimal? EXP_PRICE_2 { get; set; }
        public decimal? EXP_PRICE_3 { get; set; }
        public decimal? EXP_PRICE_4 { get; set; }
        public decimal? EXP_VAT_RATIO_1 { get; set; }
        public decimal? EXP_VAT_RATIO_2 { get; set; }
        public decimal? EXP_VAT_RATIO_3 { get; set; }
        public decimal? EXP_VAT_RATIO_4 { get; set; }
        public decimal? IMP_PRICE_1 { get; set; }
        public decimal? IMP_PRICE_2 { get; set; }
        public decimal? IMP_PRICE_3 { get; set; }
        public decimal? IMP_PRICE_4 { get; set; }
        public decimal? IMP_VAT_RATIO_1 { get; set; }
        public decimal? IMP_VAT_RATIO_2 { get; set; }
        public decimal? IMP_VAT_RATIO_3 { get; set; }
        public decimal? IMP_VAT_RATIO_4 { get; set; }
        public string SUPPLIER_NAME_1 { get; set; }
        public string SUPPLIER_NAME_2 { get; set; }
        public string SUPPLIER_NAME_3 { get; set; }
        public string SUPPLIER_NAME_4 { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string MATERIAL_PARENT_CODE { get; set; }
        public string MATERIAL_PARENT_NAME { get; set; }

        public decimal? TLLN { get; set; }
        public decimal? LOINHUAN { get; set; }
        public decimal? IMP_PRICE_AFTER_VAT { get; set; }
        public decimal? GIADUKIEN { get; set; }

    }
}
