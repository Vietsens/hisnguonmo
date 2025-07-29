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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    public class ElectronicBillResult
    {
        public bool Success { get; set; }
        public string InvoiceCode { get; set; }
        public string InvoiceSys { get; set; }
        public List<string> Messages { get; set; }
        public string InvoiceLink { get; set; }
        public string InvoiceNumOrder { get; set; }
        public string InvoiceLoginname { get; set; }
        public string InvoiceLookupCode { get; set; }
        public long? InvoiceTime { get; set; }
        //qtcode
        public byte[] PdfFileData { get; set; }
        public Guid hoadon68_id { get; set; }
        public string Status { get; set; }
    }
}
