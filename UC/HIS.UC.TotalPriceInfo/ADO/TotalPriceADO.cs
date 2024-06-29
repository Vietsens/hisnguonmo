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

namespace HIS.UC.TotalPriceInfo.ADO
{
    public class TotalPriceADO
    {
        public string TotalPrice { get; set; }
        public string TotalHeinPrice { get; set; }
        public string TotalPatientPrice { get; set; }
        public string TotalBillFundPrice { get; set; }
        public string Discount { get; set; }
        public string TotalReceivePrice { get; set; }
        public string TotalReceiveMorePrice { get; set; }
        public string TotalDepositPrice { get; set; }
        public string TotalServiceDepositPrice { get; set; }
        public string TotalBillPrice { get; set; }
        public string TotalBillTransferPrice { get; set; }
        public string TotalRepayPrice { get; set; }
        public string TotalDiscount { get; set; }
        public string TotalOtherBillAmount { get; set; }
        public string VirTotalPriceNoExpend { get; set; }
        public string TotalDebtAmount { get; set; }
        public string TotalOtherSourcePrice { get; set; }

        public string TotalOtherCopaidPrice { get; set; }
        public string LockingAmount { get; set; }
        public TotalPriceADO() { }

        public TotalPriceADO(string totalPrice, string totalHeinPrice, string totalPatientPrice, string totalBillFundPrice, string discount, string totalReceivePrice, string totalReceiveMorePrice, string totalDepositPrice, string totalBillPrice, string totalBillTransferPrice, string totalRepayPrice, string totalOtherCopaidPrice)
        {
            this.Discount = discount;
            this.TotalBillFundPrice = totalBillFundPrice;
            this.TotalBillPrice = totalBillPrice;
            this.TotalBillTransferPrice = totalBillTransferPrice;
            this.TotalDepositPrice = totalDepositPrice;
            this.TotalHeinPrice = totalHeinPrice;
            this.TotalPatientPrice = totalPatientPrice;
            this.TotalPrice = totalPrice;
            this.TotalReceiveMorePrice = totalReceiveMorePrice;
            this.TotalReceivePrice = totalReceivePrice;
            this.TotalRepayPrice = totalRepayPrice;
            this.TotalOtherCopaidPrice = totalOtherCopaidPrice;
        }

        public TotalPriceADO(string totalPrice, string totalHeinPrice, string totalPatientPrice, string totalBillFundPrice, string discount, string totalReceivePrice, string totalReceiveMorePrice, string totalDepositPrice, string totalBillPrice, string totalBillTransferPrice, string totalRepayPrice, string totalDiscount, string totalOtherCopaidPrice)
        {
            this.Discount = discount;
            this.TotalBillFundPrice = totalBillFundPrice;
            this.TotalBillPrice = totalBillPrice;
            this.TotalBillTransferPrice = totalBillTransferPrice;
            this.TotalDepositPrice = totalDepositPrice;
            this.TotalHeinPrice = totalHeinPrice;
            this.TotalPatientPrice = totalPatientPrice;
            this.TotalPrice = totalPrice;
            this.TotalReceiveMorePrice = totalReceiveMorePrice;
            this.TotalReceivePrice = totalReceivePrice;
            this.TotalRepayPrice = totalRepayPrice;
            this.TotalDiscount = totalDiscount;
            this.TotalOtherCopaidPrice = totalOtherCopaidPrice;
        }

        public TotalPriceADO(string totalPrice, string totalHeinPrice, string totalPatientPrice, string totalBillFundPrice, string discount, string totalReceivePrice, string totalReceiveMorePrice, string totalDepositPrice, string totalBillPrice, string totalBillTransferPrice, string totalRepayPrice, string totalDiscount, string totalOtherBillAmount, string totalOtherCopaidPrice)
        {
            this.Discount = discount;
            this.TotalBillFundPrice = totalBillFundPrice;
            this.TotalBillPrice = totalBillPrice;
            this.TotalBillTransferPrice = totalBillTransferPrice;
            this.TotalDepositPrice = totalDepositPrice;
            this.TotalHeinPrice = totalHeinPrice;
            this.TotalPatientPrice = totalPatientPrice;
            this.TotalPrice = totalPrice;
            this.TotalReceiveMorePrice = totalReceiveMorePrice;
            this.TotalReceivePrice = totalReceivePrice;
            this.TotalRepayPrice = totalRepayPrice;
            this.TotalDiscount = totalDiscount;
            this.TotalOtherBillAmount = totalOtherBillAmount;
            this.TotalOtherCopaidPrice = totalOtherCopaidPrice;
        }

        public TotalPriceADO(string totalPrice, string totalHeinPrice, string totalPatientPrice, string totalBillFundPrice, string discount, string totalReceivePrice, string totalReceiveMorePrice, string totalDepositPrice, string totalBillPrice, string totalBillTransferPrice, string totalRepayPrice, string totalDiscount, string totalOtherBillAmount, string virTotalPriceNoExpend, string totalOtherCopaidPrice)
        {
            this.Discount = discount;
            this.TotalBillFundPrice = totalBillFundPrice;
            this.TotalBillPrice = totalBillPrice;
            this.TotalBillTransferPrice = totalBillTransferPrice;
            this.TotalDepositPrice = totalDepositPrice;
            this.TotalHeinPrice = totalHeinPrice;
            this.TotalPatientPrice = totalPatientPrice;
            this.TotalPrice = totalPrice;
            this.TotalReceiveMorePrice = totalReceiveMorePrice;
            this.TotalReceivePrice = totalReceivePrice;
            this.TotalRepayPrice = totalRepayPrice;
            this.TotalDiscount = totalDiscount;
            this.TotalOtherBillAmount = totalOtherBillAmount;
            this.VirTotalPriceNoExpend = virTotalPriceNoExpend;
            this.TotalOtherCopaidPrice = totalOtherCopaidPrice;
        }

        public TotalPriceADO(string totalPrice, string totalHeinPrice, string totalPatientPrice, string totalBillFundPrice, string discount, string totalReceivePrice, string totalReceiveMorePrice, string totalDepositPrice, string totalBillPrice, string totalBillTransferPrice, string totalRepayPrice, string totalDiscount, string totalOtherBillAmount, string virTotalPriceNoExpend, string totalOtherCopaidPrice,string totalServiceDepositAmount)
        {
            this.Discount = discount;
            this.TotalBillFundPrice = totalBillFundPrice;
            this.TotalBillPrice = totalBillPrice;
            this.TotalBillTransferPrice = totalBillTransferPrice;
            this.TotalDepositPrice = totalDepositPrice;
            this.TotalHeinPrice = totalHeinPrice;
            this.TotalPatientPrice = totalPatientPrice;
            this.TotalPrice = totalPrice;
            this.TotalReceiveMorePrice = totalReceiveMorePrice;
            this.TotalReceivePrice = totalReceivePrice;
            this.TotalRepayPrice = totalRepayPrice;
            this.TotalDiscount = totalDiscount;
            this.TotalOtherBillAmount = totalOtherBillAmount;
            this.VirTotalPriceNoExpend = virTotalPriceNoExpend;
            this.TotalOtherCopaidPrice = totalOtherCopaidPrice;
            this.TotalServiceDepositPrice = totalServiceDepositAmount;
        }
    }
}
