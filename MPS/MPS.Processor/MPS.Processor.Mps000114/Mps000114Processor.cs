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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000114.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000114
{
    public class Mps000114Processor : AbstractProcessor
    {
        Mps000114PDO rdo;
        public Mps000114Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000114PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo._Transaction != null)
                {
                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    string temp = rdo._Transaction.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    //SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.AMOUNT_TEXT, amountText));
                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));
                    decimal ratio = ((rdo._Transaction.EXEMPTION ?? 0) * 100) / rdo._Transaction.AMOUNT;
                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    //Ket Chuyen, Can Thu
                    if (rdo._Transaction.KC_AMOUNT.HasValue)
                    {
                        string kcAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.KC_AMOUNT.Value));
                        SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.KC_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(kcAmountText)));
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._CanThu_Amount));
                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._CanThu_Amount)));
                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(ctAmountText)));

                    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Transaction.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);
                    //if (rdo._Bill != null)
                    //{
                    //    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.DESCRIPTION, rdo._Bill.DESCRIPTION));
                    //    SetSingleKey(new KeyValue(Mps000114ExtendSingleKey.EXEMPTION_REASON, rdo._Bill.EXEMPTION_REASON));
                    //}
                }

                if (rdo._Patient != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo._Patient, false);
                }

                if (rdo._PatientTypeAlter != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatientTypeAlter, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
