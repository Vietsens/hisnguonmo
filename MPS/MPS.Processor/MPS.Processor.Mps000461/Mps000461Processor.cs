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
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Common.QRCoder;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000461.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MPS.Processor.Mps000461
{
    public partial class Mps000461Processor : AbstractProcessor
    {
        Mps000461PDO rdo;
        List<AssignTurnADO> lstAssignTurnCode = new List<AssignTurnADO>();
        public Mps000461Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000461PDO)rdoBase;
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

                SetImageKey();


                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                List<SereServProcessor> sereServProcessors = new List<SereServProcessor>();
                foreach (var item in rdo.SereServs)
                {
                    V_HIS_SERVICE_REQ sr = null;
                    if(rdo.ListServiceReqPrint != null && rdo.ListServiceReqPrint.Count > 0)
                    {
                        sr = rdo.ListServiceReqPrint.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                    }
                    SereServProcessor sere = new SereServProcessor(item, sr);
                    sereServProcessors.Add(sere);
                }
                objectTag.AddObjectData(store, "listSereServ", sereServProcessors);
                objectTag.AddObjectData(store, "listAssignTurn", lstAssignTurnCode);
                objectTag.AddRelationship(store, "listAssignTurn", "listSereServ", "ASSIGN_TURN_CODE", "ASSIGN_TURN_CODE");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void SetSingleKey()
        {
            try
            {
                if (rdo.CurrentHisTreatment != null)
                {
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.CurrentHisTreatment, false);
                    SetSingleKey(new KeyValue(Mps000461ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.CurrentHisTreatment.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000461ExtendSingleKey.STR_YEAR, rdo.CurrentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000461ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentHisTreatment.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000461ExtendSingleKey.GENDER, rdo.CurrentHisTreatment.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000461ExtendSingleKey.ADDRESS, rdo.CurrentHisTreatment.TDL_PATIENT_ADDRESS));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.CurrentHisTreatment != null)
                {

                    if (!String.IsNullOrEmpty(rdo.CurrentHisTreatment.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.CurrentHisTreatment.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000461ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);

                        string treatmentCode = rdo.CurrentHisTreatment.TREATMENT_CODE.Substring(rdo.CurrentHisTreatment.TREATMENT_CODE.Length - 5, 5);
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment5 = new Inventec.Common.BarcodeLib.Barcode(treatmentCode);
                        barcodeTreatment5.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment5.IncludeLabel = false;
                        barcodeTreatment5.Width = 120;
                        barcodeTreatment5.Height = 40;
                        barcodeTreatment5.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment5.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment5.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment5.IncludeLabel = true;

                        dicImage.Add(Mps000461ExtendSingleKey.TREATMENT_CODE_BAR_5, barcodeTreatment5);
                    }

                    if (!string.IsNullOrWhiteSpace(rdo.CurrentHisTreatment.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.CurrentHisTreatment.TDL_PATIENT_CODE);
                        barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatient.IncludeLabel = false;
                        barcodePatient.Width = 120;
                        barcodePatient.Height = 40;
                        barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatient.IncludeLabel = true;

                        dicImage.Add(Mps000461ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatient);
                    }
                }
                if (rdo.ListServiceReqPrint != null && rdo.ListServiceReqPrint.Count > 0)
                {
                    var srTurnCode = rdo.ListServiceReqPrint.Where(o => !string.IsNullOrEmpty(o.ASSIGN_TURN_CODE)).ToList();
                    if (srTurnCode != null && srTurnCode.Count > 0)
                    {
                        var assignTunrCodes = srTurnCode.Select(o => o.ASSIGN_TURN_CODE).Distinct().ToList();
                        foreach (var item in assignTunrCodes)
                        {
                            lstAssignTurnCode.Add(new AssignTurnADO() { ASSIGN_TURN_CODE = item, ATCODE_BAR = ProcessBarcode(item) });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private byte[] ProcessBarcode(string data)
        {
            byte[] result = null;
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeAssignTurnCode = new Inventec.Common.BarcodeLib.Barcode(data);
                barcodeAssignTurnCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeAssignTurnCode.IncludeLabel = false;
                barcodeAssignTurnCode.Height = 50;
                barcodeAssignTurnCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeAssignTurnCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeAssignTurnCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeAssignTurnCode.IncludeLabel = true;
                result = Inventec.Common.FlexCellExport.Common.ConverterImageToArray(barcodeAssignTurnCode.Encode(barcodeAssignTurnCode.EncodedType, barcodeAssignTurnCode.RawData));
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        internal void SetImageKey()
        {
            try
            {
                bool isBhytAndAvtNull = true;
                if (rdo.CurrentHisTreatment != null && !String.IsNullOrEmpty(rdo.CurrentHisTreatment.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(Mps000461ExtendSingleKey.IMG_AVATAR, rdo.CurrentHisTreatment.TDL_PATIENT_AVATAR_URL);
                    isBhytAndAvtNull = false;
                }

                if (isBhytAndAvtNull)
                {
                    SetSingleKey(Mps000461ExtendSingleKey.AVT_AND_BHYT_NULL, "1");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetSingleImage(string key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                if (stream != null)
                {
                    SetSingleKey(new KeyValue(key, stream.ToArray()));
                }
                else
                {
                    SetSingleKey(new KeyValue(key, ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

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

                }

                return result;
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã điều trị: " + rdo.CurrentHisTreatment.TREATMENT_CODE;
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
                if (rdo != null && rdo.ListServiceReqPrint != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.CurrentHisTreatment.TREATMENT_CODE;
                    List<string> serviceReqCodes = new List<string>();
                    foreach (var item in rdo.ListServiceReqPrint.Select(s => s.SERVICE_REQ_CODE).Distinct().ToList())
                    {
                        serviceReqCodes.Add("SERVICE_REQ_CODE:" + item);
                    }

                    string serviceReqCode = string.Join(",", serviceReqCodes);
                    result = String.Format("{0} {1} {2}", printTypeCode, treatmentCode, serviceReqCode);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
