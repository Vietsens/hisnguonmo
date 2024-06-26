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

namespace MPS.Core.Mps000103
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000103RDO : RDOBase
    {
        V_HIS_BILL _Bill = null;
        V_HIS_TRANSACTION _Transaction = null;
        V_HIS_PATIENT _Patient = null;
        internal List<V_HIS_SERE_SERV> _ListSereServ = null;

        public Mps000103RDO(V_HIS_BILL bill, V_HIS_PATIENT patient, List<V_HIS_SERE_SERV> sereServs, V_HIS_TRANSACTION transaction)
        {
            this._Bill = bill;
            this._Patient = patient;
            this._Transaction = transaction;
            this._ListSereServ = sereServs;
        }
        public Mps000103RDO(V_HIS_BILL bill, V_HIS_PATIENT patient, List<V_HIS_SERE_SERV> sereServs)
        {
            this._Bill = bill;
            this._Patient = patient;
            this._ListSereServ = sereServs;
        }
        public Mps000103RDO(V_HIS_PATIENT patient, V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> sereServs)
        {
            this._Patient = patient;
            this._Transaction = transaction;
            this._ListSereServ = sereServs;
        }

        internal override void SetSingleKey()
        {
            try
            {
                decimal totalPrice = 0;
                decimal totalPatientPrice = 0;
                if (this._ListSereServ != null && this._ListSereServ.Count > 0)
                {
                    totalPrice = this._ListSereServ.Sum(s => s.VIR_TOTAL_PRICE ?? 0);
                    totalPatientPrice = this._ListSereServ.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    string serviceTypeName = "";
                    var Groups = this._ListSereServ.GroupBy(o => o.SERVICE_TYPE_ID).ToList();
                    foreach (var item in Groups)
                    {
                        if (String.IsNullOrEmpty(serviceTypeName))
                            serviceTypeName = item.ToList<V_HIS_SERE_SERV>().First().SERVICE_TYPE_NAME;
                        else
                            serviceTypeName = serviceTypeName + " - " + item.ToList<V_HIS_SERE_SERV>().First().SERVICE_TYPE_NAME;
                    }
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.SERVICE_TYPE_NAME, serviceTypeName));
                }

                if (this._Transaction != null)
                {

                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Transaction.DOB??0)));

                    string temp = this._Transaction.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this._Transaction.DOB??0)));

                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice)));

                    string totalPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    string totalPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PRICE_TEXT, totalPriceText));
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPriceText)));

                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice)));

                    string totalPatientPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice));
                    string totalPatientPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT, totalPatientPriceText));
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPatientPriceText)));

                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._Transaction.CREATE_TIME ?? 0)));
                    if (this._Bill != null)
                    {
                        keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.DESCRIPTION, this._Bill.DESCRIPTION));
                        keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.EXEMPTION_REASON, this._Bill.EXEMPTION_REASON));
                    }
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(this._Transaction, this.keyValues, false);
                }
                else if (this._Bill != null)
                {
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Bill.DOB??0)));

                    string temp = this._Bill.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this._Bill.DOB??0)));

                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice)));

                    string totalPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    string totalPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PRICE_TEXT, totalPriceText));
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPriceText)));

                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice)));

                    string totalPatientPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice));
                    string totalPatientPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT, totalPatientPriceText));
                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPatientPriceText)));

                    keyValues.Add(new KeyValue(Mps000103ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._Bill.CREATE_TIME ?? 0)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_BILL>(this._Bill, this.keyValues, false);
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
