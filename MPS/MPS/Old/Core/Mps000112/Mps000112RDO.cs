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

namespace MPS.Core.Mps000112
{
    public class Mps000112RDO : RDOBase
    {
        V_HIS_TRANSACTION _Transaction = null;
        V_HIS_DEPOSIT _Deposit = null;
        V_HIS_PATIENT _Patient = null;

        public Mps000112RDO(V_HIS_TRANSACTION transaction, V_HIS_DEPOSIT deposit, V_HIS_PATIENT patient)
        {
            this._Transaction = transaction;
            this._Deposit = deposit;
            this._Patient = patient;
        }

        public Mps000112RDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT patient)
        {
            this._Transaction = transaction;
            this._Patient = patient;
        }

        public Mps000112RDO(V_HIS_DEPOSIT deposit, V_HIS_PATIENT patient)
        {
            this._Deposit = deposit;
            this._Patient = patient;
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (this._Transaction != null)
                {
                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Transaction.DOB??0)));

                    string temp = this._Transaction.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this._Transaction.DOB??0)));

                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Transaction.AMOUNT)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Transaction.AMOUNT));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.AMOUNT_TEXT, amountText));
                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));
                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._Transaction.CREATE_TIME ?? 0)));

                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(this._Transaction, this.keyValues, false);
                }
                else if (this._Deposit != null)
                {
                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._Deposit.DOB)));

                    string temp = this._Deposit.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this._Deposit.DOB)));

                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Deposit.AMOUNT)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Deposit.AMOUNT));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.AMOUNT_TEXT, amountText));
                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));
                    keyValues.Add(new KeyValue(Mps000112ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._Deposit.CREATE_TIME ?? 0)));

                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPOSIT>(this._Deposit, this.keyValues, false);
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
