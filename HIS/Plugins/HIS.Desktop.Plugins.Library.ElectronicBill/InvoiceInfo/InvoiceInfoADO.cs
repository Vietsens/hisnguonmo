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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.InvoiceInfo
{
    class InvoiceInfoADO
    {
        public string BuyerOrganization { get; set; }
        public string BuyerAccountNumber { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public string BuyerGender { get; set; }
        public string BuyerDob { get; set; }
        public string BuyerAddress { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerTaxCode { get; set; }
        public string BuyerEmail { get; set; }
        public string Note { get; set; }
        public string PaymentMethod { get; set; }
        public long TransactionTime { get; set; }
    }
}
