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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;
using MOS.SDO;

namespace MPS.Core.Mps000106
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000106RDO : RDOBase
    {
        internal List<V_HIS_SERE_SERV> listSereServ { get; set; }
        V_HIS_BILL _Bill { get; set; }
        V_HIS_TRANSACTION _Transaction { get; set; }

        decimal _CanThu_Amount = 0;

        public Mps000106RDO(MOS.EFMODEL.DataModels.V_HIS_BILL bill, List<V_HIS_SERE_SERV> hisSereServs, decimal ctAmount)
        {
            try
            {
                this.listSereServ = hisSereServs;
                this._Bill = bill;
                this._CanThu_Amount = ctAmount;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000106RDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> hisSereServs, decimal ctAmount)
        {
            try
            {
                this.listSereServ = hisSereServs;
                this._Transaction = transaction;
                this._CanThu_Amount = ctAmount;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (this._Transaction != null)
                {
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Transaction.DOB??0)));

                    string temp = this._Transaction.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this._Transaction.DOB??0)));

                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Transaction.AMOUNT)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Transaction.AMOUNT));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_TEXT, amountText));
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));
                    decimal amountAfterExem = this._Transaction.AMOUNT - (this._Transaction.EXEMPTION ?? 0);
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));
                    decimal ratio = ((this._Transaction.EXEMPTION ?? 0) * 100) / this._Transaction.AMOUNT;
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    //Ket Chuyen, Can Thu
                    if (this._Transaction.KC_AMOUNT.HasValue)
                    {
                        string kcAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Transaction.KC_AMOUNT.Value));
                        keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.KC_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(kcAmountText)));
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._CanThu_Amount));
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._CanThu_Amount)));
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(ctAmountText)));

                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._Transaction.CREATE_TIME ?? 0)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(this._Transaction, this.keyValues, false);
                    if (this._Bill != null)
                    {
                        keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.DESCRIPTION, this._Bill.DESCRIPTION));
                        keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.EXEMPTION_REASON, this._Bill.EXEMPTION_REASON));
                    }
                }
                else if (_Bill != null)
                {
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Bill.DOB??0)));

                    string temp = this._Bill.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this._Bill.DOB??0)));

                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Bill.AMOUNT)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Bill.AMOUNT));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_TEXT, amountText));
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));
                    decimal amountAfterExem = this._Bill.AMOUNT - (this._Bill.EXEMPTION ?? 0);
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));
                    decimal ratio = ((this._Bill.EXEMPTION ?? 0) * 100) / this._Bill.AMOUNT;
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    //Ket Chuyen, Can Thu
                    if (this._Bill.KC_AMOUNT.HasValue)
                    {
                        string kcAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Bill.KC_AMOUNT.Value));
                        keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.KC_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(kcAmountText)));
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._CanThu_Amount));
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._CanThu_Amount)));
                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(ctAmountText)));

                    keyValues.Add(new KeyValue(Mps000106ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._Bill.CREATE_TIME ?? 0)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_BILL>(this._Bill, this.keyValues, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
