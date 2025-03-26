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
using Inventec.Common.ElectronicBill.MD;
using Inventec.Common.ElectronicBill.ModelTT78;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Base
{
    public class ElectronicBillInput
    {
        public string serviceUrl { get; set; }
        public string account { get; set; }
        public string acPass { get; set; }
        public List<Invoice> invoices { get; set; }
        public List<Invoice_BM> invoicesBm { get; set; }
        public string pattern { get; set; }
        public string serial { get; set; }
        public string userName { get; set; }
        public string passWord { get; set; }
        public int convert { get; set; }
        public string fKey { get; set; }
        public string DataXmlStringPlus { get; set; }
        public List<HDon> invoiceTT78s { get; set; }
    }
}
