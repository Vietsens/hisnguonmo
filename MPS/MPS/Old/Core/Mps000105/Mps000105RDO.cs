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

namespace MPS.Core.Mps000105
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000105RDO : RDOBase
    {
        V_HIS_TRANSACTION _Transaction = null;
        V_HIS_BILL _Bill = null;
        V_HIS_PATIENT_TYPE_ALTER _PatyAlterBhyt = null;
        V_HIS_SERVICE_REQ _ServiceReq = null;
        HeinCardADO _HeinCard = null;
        internal List<V_HIS_SERE_SERV> _ListSereServ = null;
        internal string PatientCode = "";

        public Mps000105RDO(V_HIS_TRANSACTION transaction, V_HIS_BILL bill, List<V_HIS_SERE_SERV> sereServs, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, V_HIS_SERVICE_REQ serviceReq)
        {
            this._Transaction = transaction;
            this._Bill = bill;
            this._ListSereServ = sereServs;
            this._PatyAlterBhyt = patyAlterBhyt;
            this._ServiceReq = serviceReq;
        }

        public Mps000105RDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> sereServs, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, V_HIS_SERVICE_REQ serviceReq)
        {
            this._Transaction = transaction;
            this._ListSereServ = sereServs;
            this._PatyAlterBhyt = patyAlterBhyt;
            this._ServiceReq = serviceReq;
        }

        public Mps000105RDO(V_HIS_BILL bill, List<V_HIS_SERE_SERV> sereServs, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, V_HIS_SERVICE_REQ serviceReq)
        {
            this._Bill = bill;
            this._ListSereServ = sereServs;
            this._PatyAlterBhyt = patyAlterBhyt;
            this._ServiceReq = serviceReq;
        }

        public Mps000105RDO(V_HIS_BILL bill, List<V_HIS_SERE_SERV> sereServs, V_HIS_SERVICE_REQ serviceReq, HeinCardADO ado)
        {
            this._Bill = bill;
            this._ListSereServ = sereServs;
            this._HeinCard = ado;
            this._ServiceReq = serviceReq;
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
                }
                if (this._Transaction != null)
                {

                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Transaction.DOB??0)));

                    string temp = this._Transaction.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this._Transaction.DOB??0)));

                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice)));

                    string totalPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    string totalPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PRICE_TEXT, totalPriceText));
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPriceText)));

                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice)));

                    string totalPatientPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice));
                    string totalPatientPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT, totalPatientPriceText));
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPatientPriceText)));
                    PatientCode = this._Transaction.PATIENT_CODE;
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(this._Transaction, this.keyValues, false);
                }
                else if (this._Bill != null)
                {

                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Bill.DOB??0)));

                    string temp = this._Bill.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this._Bill.DOB??0)));

                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice)));

                    string totalPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    string totalPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PRICE_TEXT, totalPriceText));
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPriceText)));

                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice)));

                    string totalPatientPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice));
                    string totalPatientPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT, totalPatientPriceText));
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPatientPriceText)));
                    PatientCode = this._Bill.PATIENT_CODE;
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_BILL>(this._Bill, this.keyValues, false);
                }
                if (this._ServiceReq != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(this._ServiceReq, this.keyValues, false);
                }
                if (this._PatyAlterBhyt != null)
                {
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, GlobalQuery.SetHeinCardNumberDisplayByNumber(this._PatyAlterBhyt.HEIN_CARD_NUMBER)));
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._PatyAlterBhyt.HEIN_CARD_FROM_TIME??0)));
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_TO_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._PatyAlterBhyt.HEIN_CARD_TO_TIME??0)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(this._PatyAlterBhyt, this.keyValues, false);
                }
                else if (this._HeinCard != null)
                {
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, GlobalQuery.SetHeinCardNumberDisplayByNumber(this._HeinCard.HeinCardNumber)));
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, this._HeinCard.FromTime));
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_TO_TIME_STR, this._HeinCard.ToTime));
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.HEIN_MEDI_ORG_CODE, this._HeinCard.MediOrgCode));
                    keyValues.Add(new KeyValue(Mps000105ExtendSingleKey.HEIN_ADDRESS, this._HeinCard.Address));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
