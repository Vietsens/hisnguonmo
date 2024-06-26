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
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Data
{
    public class ProductBase
    {
        public string ProdCode { get; set; }

        public string ProdName { get; set; }

        public string ProdUnit { get; set; }

        public decimal? ProdQuantity { get; set; }

        public decimal? ProdPrice { get; set; }

        public decimal Amount { get; set; }

        public int Type { get; set; }

        /// <summary>
        /// Loai thue suat hoa don. 1 - 0%, 2 - 5%, 3 - 10%, 4 - khong chiu thue, 5 - khong ke khai thue, 6 - khac
        /// </summary>
        public int TaxRateID { get; set; }

        public long Stt { get; set; }
        public bool IsBHYT { get; set; }
    }
}
