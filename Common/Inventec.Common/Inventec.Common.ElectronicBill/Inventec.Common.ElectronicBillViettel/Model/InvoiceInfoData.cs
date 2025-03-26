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

namespace Inventec.Common.ElectronicBillViettel.Model
{
    public class InvoiceInfoData
    {
        public long invoiceId { get; set; }
        public string invoiceType { get; set; }
        public string adjustmentType { get; set; }
        public string templateCode { get; set; }
        public string invoiceSeri { get; set; }
        public string invoiceNumber { get; set; }
        public string invoiceNo { get; set; }
        public string currency { get; set; }
        public decimal? total { get; set; }

        //thông tin thời gian giao dịch.
        public double? issueDate { get; set; }
        public string issueDateStr { get; set; }
        public string state { get; set; }
        public object requestDate { get; set; }
        public string description { get; set; }
        public string buyerIdNo { get; set; }
        public string stateCode { get; set; }
        public string subscriberNumber { get; set; }
        public long? paymentStatus { get; set; }
        public long? viewStatus { get; set; }
        public object downloadStatus { get; set; }
        public object exchangeStatus { get; set; }
        public object numOfExchange { get; set; }
        public long? createTime { get; set; }
        public object contractId { get; set; }
        public string contractNo { get; set; }
        public string supplierTaxCode { get; set; }
        public string buyerTaxCode { get; set; }
        public decimal? totalBeforeTax { get; set; }
        public decimal? taxAmount { get; set; }
        public string taxRate { get; set; }
        public string paymentMethod { get; set; }
        public object paymentTime { get; set; }
        public object customerId { get; set; }
        public string buyerName { get; set; }
        public object no { get; set; }
        public string paymentStatusName { get; set; }
    }
}
