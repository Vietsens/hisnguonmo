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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    public class ElectronicBillDataInput
    {
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }
        public string Converter { get; set; }

        public decimal? Discount { get; set; }
        public decimal? DiscountRatio { get; set; }
        public decimal? Amount { get; set; }

        public long PartnerInvoiceID { get; set; }
        public long? EinvoiceTypeId { get; set; }

        public string TemplateCode { get; set; }
        public string SymbolCode { get; set; }
        public long NumOrder { get; set; }
        public long TransactionTime { get; set; }

        public long? CancelTime { get; set; }
        public string CancelReason { get; set; }
        public string CancelUsername { get; set; }
        public string InvoiceCode { get; set; }
        public string ENumOrder { get; set; }
        public string TransactionCode { get; set; }

        public HIS_BRANCH Branch { get; set; }
        public V_HIS_TREATMENT_FEE Treatment { get; set; }
        public HIS_TRANSACTION Transaction { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }
        public List<V_HIS_SERE_SERV_5> SereServs { get; set; }
        public List<HIS_SERE_SERV_BILL> SereServBill { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER LastPatientTypeAlter { get; set; }

        public bool IsTransactionList { get; set; }

        public string SaveFileName { get; set; }

        public ElectronicBillDataInput()
        {

        }

        public ElectronicBillDataInput(ElectronicBillDataInput data)
        {
            if (data != null)
            {
                this.Amount = data.Amount;
                this.Discount = data.Discount;
                this.DiscountRatio = data.DiscountRatio;
                this.PaymentMethod = data.PaymentMethod;
                this.Currency = data.Currency;
                this.SymbolCode = data.SymbolCode;
                this.TemplateCode = data.TemplateCode;
                this.EinvoiceTypeId = data.EinvoiceTypeId;
                this.TransactionTime = data.TransactionTime;
                this.Transaction = data.Transaction;
                this.ListTransaction = data.ListTransaction;
                this.Branch = data.Branch;
                this.Treatment = data.Treatment;
                this.LastPatientTypeAlter = data.LastPatientTypeAlter;
            }
        }
    }
}
