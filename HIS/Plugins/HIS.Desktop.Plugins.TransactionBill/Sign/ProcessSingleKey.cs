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

namespace HIS.Desktop.Plugins.TransactionBill.Sign
{
    class ProcessSingleKey
    {
        internal Dictionary<string, object> singleValueDictionary = new Dictionary<string, object>();
        internal ProcessSingleKey()
        {
            singleValueDictionary = new Dictionary<string, object>();
        }

        internal void Set(V_HIS_TRANSACTION transaction, V_HIS_PATIENT patient, List<HIS_BILL_FUND> listBillFund)
        {
            if (transaction != null)
            {
                singleValueDictionary.Add(ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(transaction.TDL_PATIENT_DOB.Value));

                string temp = transaction.TDL_PATIENT_DOB.ToString();
                if (temp != null && temp.Length >= 8)
                {
                    singleValueDictionary.Add(ExtendSingleKey.YEAR_STR, temp.Substring(0, 4));
                }
                singleValueDictionary.Add(ExtendSingleKey.AGE_STR, MPS.AgeUtil.CalculateFullAge(transaction.TDL_PATIENT_DOB.Value));

                singleValueDictionary.Add(ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(transaction.AMOUNT));
                string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(transaction.AMOUNT));
                string amountText = Inventec.Common.String.Convert.CurrencyToVneseString(amountStr);
                singleValueDictionary.Add(ExtendSingleKey.AMOUNT_TEXT, amountText);
                singleValueDictionary.Add(ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, amountText);
                decimal amountAfterExem = transaction.AMOUNT - (transaction.EXEMPTION ?? 0);
                singleValueDictionary.Add(ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseString(amountAfterExemStr);
                singleValueDictionary.Add(ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText);
                singleValueDictionary.Add(ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, amountAfterExemText);
                decimal ratio = ((transaction.EXEMPTION ?? 0) * 100) / transaction.AMOUNT;
                singleValueDictionary.Add(ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio));

                //Ket Chuyen, Can Thu
                if (transaction.KC_AMOUNT.HasValue)
                {
                    string kcAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(transaction.KC_AMOUNT.Value));
                    singleValueDictionary.Add(ExtendSingleKey.KC_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(kcAmountText));
                }

                decimal canthu = transaction.AMOUNT - (transaction.KC_AMOUNT ?? 0) - (transaction.EXEMPTION ?? 0);
                if (listBillFund != null && listBillFund.Count > 0)
                {
                    canthu = canthu - listBillFund.Sum(s => s.AMOUNT);
                }

                string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                singleValueDictionary.Add(ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                singleValueDictionary.Add(ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(ctAmountText));

                singleValueDictionary.Add(ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(transaction.CREATE_TIME ?? 0));
                ObjectQuery.AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(transaction, singleValueDictionary, false);

                singleValueDictionary.Add(ExtendSingleKey.DESCRIPTION, transaction.DESCRIPTION);
                singleValueDictionary.Add(ExtendSingleKey.EXEMPTION_REASON, transaction.EXEMPTION_REASON);
            }

            if (patient != null)
            {
                ObjectQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT>(patient, singleValueDictionary, false);
            }
        }
    }
}
