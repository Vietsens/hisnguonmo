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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Data
{
    public class ProductBasePlus : ProductBase
    {
        //public decimal? Discount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? AmountWithoutTax { get; set; }
        /// <summary>
        /// null: không thuế
        /// 0: 0%
        /// 1: 5%
        /// 2: 10%
        /// 3: 8%
        /// </summary>
        public int? TaxPercentage { get; set; }
        public decimal TaxConvert { get; set; }
    }
}
