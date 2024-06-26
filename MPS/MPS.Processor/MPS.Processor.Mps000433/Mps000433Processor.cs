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
using MPS.Processor.Mps000433.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000433
{
    public class Mps000433Processor : AbstractProcessor
    {
        Mps000433PDO rdo;
        public Mps000433Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000433PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetBarcodeKey();
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "HIS_SERE_SERV", rdo.ListSereServ);
                barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.Treatment != null && !String.IsNullOrEmpty(rdo.Treatment.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTransactionCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Treatment.TREATMENT_CODE);
                    barcodeTransactionCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTransactionCode.IncludeLabel = false;
                    barcodeTransactionCode.Width = 120;
                    barcodeTransactionCode.Height = 40;
                    barcodeTransactionCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTransactionCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTransactionCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTransactionCode.IncludeLabel = true;

                    dicImage.Add(Mps000433ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTransactionCode);
                }

                if (rdo.ReqChange != null && !String.IsNullOrEmpty(rdo.ReqChange.SERVICE_REQ_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeExpMestCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ReqChange.SERVICE_REQ_CODE);
                    barcodeExpMestCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeExpMestCode.IncludeLabel = false;
                    barcodeExpMestCode.Width = 120;
                    barcodeExpMestCode.Height = 40;
                    barcodeExpMestCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeExpMestCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeExpMestCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeExpMestCode.IncludeLabel = true;

                    dicImage.Add(Mps000433ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeExpMestCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.PatientTypeAlter != null)
                {
                    AddObjectKeyIntoListkey(rdo.PatientTypeAlter, false);

                    if (!String.IsNullOrWhiteSpace(rdo.PatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.PatientTypeAlter.HEIN_CARD_NUMBER)));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.IS_HEIN, "X"));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.IS_VIENPHI, ""));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.PatientTypeAlter.HEIN_CARD_NUMBER.Substring(0, 2)));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.PatientTypeAlter.HEIN_CARD_NUMBER.Substring(2, 1)));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.PatientTypeAlter.HEIN_CARD_NUMBER.Substring(3, 2)));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.PatientTypeAlter.HEIN_CARD_NUMBER.Substring(5, 2)));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.PatientTypeAlter.HEIN_CARD_NUMBER.Substring(7, 3)));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.PatientTypeAlter.HEIN_CARD_NUMBER.Substring(10, 5)));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0))));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatientTypeAlter.HEIN_CARD_TO_TIME ?? 0))));
                        SetSingleKey(new KeyValue(Mps000433ExtendSingleKey.HEIN_ADDRESS, rdo.PatientTypeAlter.ADDRESS));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000433ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000433ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                if (rdo.Treatment != null)
                {
                    AddObjectKeyIntoListkey(rdo.Treatment, false);
                }

                if (rdo.ServiceReq != null)
                {
                    AddObjectKeyIntoListkey(rdo.ServiceReq, false);
                }

                if (rdo.ReqChange != null)
                {
                    AddObjectKeyIntoListkey(rdo.ReqChange, false);

                    if (rdo.ListSereServ != null && rdo.ListSereServ.Count > 0)
                    {
                        var sereServ = rdo.ListSereServ.FirstOrDefault(o => o.ID == rdo.ReqChange.SERE_SERV_ID);
                        if (sereServ != null)
                            AddKeyIntoListkey(sereServ, "ORIGIN");

                        var sereServAlter = rdo.ListSereServ.FirstOrDefault(o => o.ID == rdo.ReqChange.ALTER_SERE_SERV_ID);
                        if (sereServAlter != null)
                            AddKeyIntoListkey(sereServAlter, "ALTER");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddKeyIntoListkey<T>(T data, string prefix)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        if (pi.GetGetMethod().IsVirtual) continue;
                        string keyName = "";
                        keyName = string.Format("{0}_{1}", prefix, pi.Name);

                        bool searchKey = singleValueDictionary.ContainsKey(keyName);
                        if (!searchKey)
                        {
                            SetSingleKey(new KeyValue(keyName, (data != null ? pi.GetValue(data) : null)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (rdo.ReqChange != null)
                {
                    log = string.Format("Mã hồ sơ:{0}, dịch vụ đã chỉ định: {1}, dịc vụ đổi: {2} ", rdo.ReqChange.TDL_TREATMENT_CODE, rdo.ReqChange.TDL_SERVICE_CODE, rdo.ReqChange.ALTER_SERVICE_CODE);
                }
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
                if (rdo != null)
                {
                    string treatmentCode = "";
                    string serviceReqCode = "";
                    string serviceCode = "";
                    string sereServ = "";

                    if (rdo != null && rdo.ReqChange != null)
                    {
                        treatmentCode = rdo.ReqChange.TDL_TREATMENT_CODE;
                        serviceReqCode = rdo.ReqChange.SERVICE_REQ_CODE;
                        serviceCode = string.Format("{0}|{1}", rdo.ReqChange.TDL_SERVICE_CODE, rdo.ReqChange.ALTER_SERVICE_CODE);
                        sereServ = string.Format("{0}|{1}", rdo.ReqChange.SERE_SERV_ID, rdo.ReqChange.ALTER_SERE_SERV_ID);
                    }

                    result = String.Format("{0} {1} {2} {3} {4}", printTypeCode, treatmentCode, serviceReqCode, serviceCode, sereServ);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        class HeinCardHelper
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
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                return result;
            }
        }
    }
}
