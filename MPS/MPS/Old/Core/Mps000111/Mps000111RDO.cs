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

namespace MPS.Core.Mps000111
{
    public class Mps000111RDO : RDOBase
    {
        V_HIS_TRANSACTION _Transaction = null;
        V_HIS_BILL _Bill = null;
        V_HIS_PATIENT _Patient = null;
        List<HIS_BILL_FUND> _ListBillFund = null;
        List<HIS_SERE_SERV> _ListSereServ = null;
        long _PatientTypeId;

        public Mps000111RDO(V_HIS_TRANSACTION transaction, V_HIS_BILL bill, V_HIS_PATIENT patient, List<HIS_BILL_FUND> listBillFund, List<HIS_SERE_SERV> listSereServ, long patientTypeId)
        {
            this._Transaction = transaction;
            this._Bill = bill;
            this._Patient = patient;
            this._ListBillFund = listBillFund;
            this._ListSereServ = listSereServ;
            this._PatientTypeId = patientTypeId;
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (this._Transaction != null)
                {
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Transaction.DOB??0)));

                    string temp = this._Transaction.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this._Transaction.DOB??0)));

                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Transaction.AMOUNT)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Transaction.AMOUNT));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_TEXT, amountText));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));
                    decimal amountAfterExem = this._Transaction.AMOUNT - (this._Transaction.EXEMPTION ?? 0);
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));
                    decimal ratio = ((this._Transaction.EXEMPTION ?? 0) * 100) / this._Transaction.AMOUNT;
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    //Ket Chuyen, Can Thu
                    if (this._Transaction.KC_AMOUNT.HasValue)
                    {
                        string kcAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Transaction.KC_AMOUNT.Value));
                        keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.KC_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(kcAmountText)));
                    }

                    decimal canthu = this._Transaction.AMOUNT - (this._Transaction.KC_AMOUNT ?? 0) - (this._Transaction.EXEMPTION ?? 0);
                    if (this._ListBillFund != null && this._ListBillFund.Count > 0)
                    {
                        canthu = canthu - this._ListBillFund.Sum(s => s.AMOUNT);
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu)));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(ctAmountText)));

                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._Transaction.CREATE_TIME ?? 0)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(this._Transaction, this.keyValues, false);
                    if (this._Bill != null)
                    {
                        keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.DESCRIPTION, this._Bill.DESCRIPTION));
                        keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.EXEMPTION_REASON, this._Bill.EXEMPTION_REASON));
                    }
                }
                else if (this._Bill != null)
                {
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Bill.DOB??0)));

                    string temp = this._Bill.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this._Bill.DOB??0)));

                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Bill.AMOUNT)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Bill.AMOUNT));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_TEXT, amountText));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));
                    decimal amountAfterExem = this._Bill.AMOUNT - (this._Bill.EXEMPTION ?? 0);
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));
                    decimal ratio = ((this._Bill.EXEMPTION ?? 0) * 100) / this._Bill.AMOUNT;
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    //Ket Chuyen, Can Thu
                    if (this._Bill.KC_AMOUNT.HasValue)
                    {
                        string kcAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Bill.KC_AMOUNT.Value));
                        keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.KC_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(kcAmountText)));
                    }

                    decimal canthu = this._Bill.AMOUNT - (this._Bill.KC_AMOUNT ?? 0) - (this._Bill.EXEMPTION ?? 0);
                    if (this._ListBillFund != null && this._ListBillFund.Count > 0)
                    {
                        canthu = canthu - this._ListBillFund.Sum(s => s.AMOUNT);
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu)));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(ctAmountText)));

                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._Bill.CREATE_TIME ?? 0)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_BILL>(this._Bill, this.keyValues, false);
                }

                if (this._ListSereServ != null && this._ListSereServ.Count > 0)
                {
                    var totalBhyt = this._ListSereServ.Where(o => o.PATIENT_TYPE_ID == this._PatientTypeId).ToList().Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    var totalNd = this._ListSereServ.Where(o => o.PATIENT_TYPE_ID != this._PatientTypeId).ToList().Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.TOTAL_AMOUNT_BHYT, totalBhyt));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.TOTAL_AMOUNT_ND, totalNd));

                    string totalBhytText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalBhyt));
                    string totalNdText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalNd));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.TOTAL_AMOUNT_BHYT_UPPER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(totalBhytText)));
                    keyValues.Add(new KeyValue(Mps000111ExtendSingleKey.TOTAL_AMOUNT_ND_UPPER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(totalNdText)));
                }

                if (this._Patient != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT>(this._Patient, this.keyValues, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
