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
using MPS.Processor.Mps000109.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000109
{
    public class Mps000109Processor : AbstractProcessor
    {
        Mps000109PDO rdo;
        public Mps000109Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000109PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                this.SetSingleKey();
                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);


                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = LogDataTransaction(rdo.hisDeposit.TDL_TREATMENT_CODE, rdo.hisDeposit.TRANSACTION_CODE, "");
                log += "SoTien: " + rdo.hisDeposit.AMOUNT;
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.hisDeposit != null)
                    result = String.Format("{0}_{1}_{2}_{3}", "Mps000109", rdo.hisDeposit.TDL_TREATMENT_CODE, rdo.hisDeposit.TRANSACTION_CODE, rdo.hisDeposit.ACCOUNT_BOOK_CODE);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }


        private void SetSingleKey()
        {
            try
            {
                if (rdo.hisDeposit != null)
                {
                    string amount = Inventec.Common.Number.Convert.NumberToString(rdo.hisDeposit.AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    SetSingleKey(new KeyValue(Mps000109ExtendSingleKey.AMOUNT, amount));
                    SetSingleKey(new KeyValue(Mps000109ExtendSingleKey.AMOUNT_NUM, rdo.hisDeposit.AMOUNT));
                }

                if (rdo.V_HIS_PATIENT_TYPE_ALTER != null)
                {
                    var ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_TREATMENT_TYPE_CODE, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER, rdo.V_HIS_PATIENT_TYPE_ALTER.LEVEL_CODE, rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_CODE);
                    if (ratio.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000109ExtendSingleKey.RATIO, ratio.Value.ToString()));
                        SetSingleKey(new KeyValue(Mps000109ExtendSingleKey.HEIN_RATIO, (1 - ratio.Value) * 100));
                    }

                    if (!String.IsNullOrEmpty(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey(new KeyValue(Mps000109ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000109ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_TO_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000109ExtendSingleKey.IS_VIENPHI, "X")));
                    }

                    SetSingleKey(new KeyValue(Mps000109ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.V_HIS_PATIENT_TYPE_ALTER.ADDRESS));
                }

                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.V_HIS_PATIENT_TYPE_ALTER, false);
                AddObjectKeyIntoListkey<V_HIS_TRANSACTION_5>(rdo.hisDeposit, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(rdo.hisTreatment, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }

    public class HeinCardHelper
    {
        public static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
                {
                    string separateSymbol = "-";
                    result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        public static string TrimHeinCardNumber(string chucodau)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
