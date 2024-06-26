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
using MPS.Processor.Mps000028.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase;
using HIS.Desktop.Common.BankQrCode;

namespace MPS.Processor.Mps000028
{
    class Mps000028Processor : AbstractProcessor
    {
        Mps000028PDO rdo;
        List<SereServQrADO> ListSereServADOs = new List<SereServQrADO>();
        byte[] bPatientQr;
        byte[] bPatientNameQr;
        byte[] bStudyDescriptionQr;
        byte[] qrCT540;

        public Mps000028Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000028PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.ServiceReqPrint != null)
                {
                    if (!String.IsNullOrEmpty(rdo.ServiceReqPrint.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReqPrint.TREATMENT_CODE);
                        barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatmentCode.IncludeLabel = false;
                        barcodeTreatmentCode.Width = 120;
                        barcodeTreatmentCode.Height = 40;
                        barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatmentCode.IncludeLabel = true;

                        dicImage.Add(Mps000028ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);
                    }
                    if (!String.IsNullOrEmpty(rdo.ServiceReqPrint.SERVICE_REQ_CODE))
                    {

                        Inventec.Common.BarcodeLib.Barcode barcodeServiceReq = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReqPrint.SERVICE_REQ_CODE);
                        barcodeServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeServiceReq.IncludeLabel = false;
                        barcodeServiceReq.Width = 120;
                        barcodeServiceReq.Height = 40;
                        barcodeServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeServiceReq.IncludeLabel = true;

                        dicImage.Add(Mps000028ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeServiceReq);
                    }
                    if (!String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReqPrint.TDL_PATIENT_CODE);
                        barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatientCode.IncludeLabel = false;
                        barcodePatientCode.Width = 120;
                        barcodePatientCode.Height = 40;
                        barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatientCode.IncludeLabel = true;

                        dicImage.Add(Mps000028ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetQRcodeSereServKey()
        {
            try
            {
                if (rdo.sereServADOs != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("count " + rdo.sereServADOs.Count);
                    Inventec.Common.Logging.LogSystem.Info("begin qr");
                    List<Task> taskall = new List<Task>();
                    Task tsPatient = Task.Factory.StartNew((object obj) =>
                    {
                        SereServADO data = obj as SereServADO;
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(data.patientIdQr, QRCodeGenerator.ECCLevel.Q);
                        BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                        byte[] qrCodeImage = qrCode.GetGraphic(20);
                        bPatientQr = qrCodeImage;
                    }, rdo.sereServADOs.First());
                    taskall.Add(tsPatient);

                    Task tsPatientName = Task.Factory.StartNew((object obj) =>
                    {
                        SereServADO data = obj as SereServADO;
                        QRCodeGenerator qrGeneratorPatientName = new QRCodeGenerator();
                        QRCodeData qrCodeDataPatientName = qrGeneratorPatientName.CreateQrCode(data.patientNameQr, QRCodeGenerator.ECCLevel.Q);
                        BitmapByteQRCode qrCodePatientName = new BitmapByteQRCode(qrCodeDataPatientName);
                        byte[] qrCodeImagePatientName = qrCodePatientName.GetGraphic(20);
                        bPatientNameQr = qrCodeImagePatientName;
                    }, rdo.sereServADOs.First());
                    taskall.Add(tsPatientName);

                    Task tsDescription = Task.Factory.StartNew((object obj) =>
                    {
                        SereServADO data = obj as SereServADO;
                        QRCodeGenerator qrGeneratorStudyDescription = new QRCodeGenerator();
                        QRCodeData qrCodeDataStudyDescription = qrGeneratorStudyDescription.CreateQrCode(data.studyDescriptionQr, QRCodeGenerator.ECCLevel.Q);
                        BitmapByteQRCode qrCodeStudyDescription = new BitmapByteQRCode(qrCodeDataStudyDescription);
                        byte[] qrCodeImageStudyDescription = qrCodeStudyDescription.GetGraphic(20);
                        bStudyDescriptionQr = qrCodeImageStudyDescription;
                    }, rdo.sereServADOs.First());
                    taskall.Add(tsDescription);

                    Task tsPaInfo = Task.Factory.StartNew(() => { QrCodePatient(); });
                    taskall.Add(tsPaInfo);

                    Task tsCT540 = Task.Factory.StartNew((object obj) => { QrCT540((SereServADO)obj); }, rdo.sereServADOs.First());
                    taskall.Add(tsCT540);
                    Task.WaitAll(taskall.ToArray());

                    taskall = new List<Task>();
                    for (int i = 0; i < rdo.sereServADOs.Count; i++)
                    {
                        SereServQrADO ado = new SereServQrADO(rdo.sereServADOs[i]);

                        ado.bPatientQr = bPatientQr;
                        ado.bPatientNameQr = bPatientNameQr;
                        ado.bStudyDescriptionQr = bStudyDescriptionQr;
                        ado.QrCT540 = qrCT540;

                        Task ts1 = Task.Factory.StartNew((object obj) =>
                        {
                            SereServQrADO data = obj as SereServQrADO;
                            QRCodeGenerator qrGeneratorStudyDescription = new QRCodeGenerator();
                            QRCodeData qrCodeDataStudyDescription = qrGeneratorStudyDescription.CreateQrCode(data.studyDescriptionQr, QRCodeGenerator.ECCLevel.Q);
                            BitmapByteQRCode qrCodeStudyDescription = new BitmapByteQRCode(qrCodeDataStudyDescription);
                            byte[] qrCodeImageStudyDescription = qrCodeStudyDescription.GetGraphic(20);
                            data.bStudyDescriptionQr = qrCodeImageStudyDescription;
                        }, ado);
                        taskall.Add(ts1);

                        Task ts2 = Task.Factory.StartNew((object obj) =>
                        {
                            SereServQrADO data = obj as SereServQrADO;
                            QrDiim(data);
                        }, ado);
                        taskall.Add(ts2);

                        Task ts3 = Task.Factory.StartNew((object obj) =>
                        {
                            SereServQrADO data = obj as SereServQrADO;
                            QrDiimV2(data);
                        }, ado);

                        taskall.Add(ts3);

                        Task bar = Task.Factory.StartNew((object obj) =>
                        {
                            if (obj != null)
                            {
                                SereServQrADO data = obj as SereServQrADO;
                                List<string> lstService = new List<string>();
                                string serviceCode = "";
                                for (int y = 0; y < rdo.ServiceReqPrint.SERVICE_REQ_CODE.Length; y++)
                                {
                                    var str = rdo.ServiceReqPrint.SERVICE_REQ_CODE.Substring(y, 1);
                                    if (str != "0")
                                    {
                                        serviceCode = rdo.ServiceReqPrint.SERVICE_REQ_CODE.Substring(y, (rdo.ServiceReqPrint.SERVICE_REQ_CODE.Length - y));
                                        break;
                                    }

                                }
                                //ado.BARCODE = ProcessBarcode(rdo.ServiceReqPrint.SERVICE_REQ_CODE + "." + data.TDL_SERVICE_CODE);
                                ado.BARCODE = ProcessBarcode(serviceCode + "." + data.TDL_SERVICE_CODE);
                                Inventec.Common.Logging.LogSystem.Debug("TDL_SERVICE_CODE" + data.TDL_SERVICE_CODE);
                            }
                        }, ado);
                        taskall.Add(bar);

                        Task barAccess = Task.Factory.StartNew((object obj) =>
                        {
                            if (obj != null)
                            {
                                SereServQrADO data = obj as SereServQrADO;
                                ado.AccessNo = ProcessBarcode(data.ID + "");
                            }
                        }, ado);
                        taskall.Add(barAccess);

                        ListSereServADOs.Add(ado);
                    }

                    Task.WaitAll(taskall.ToArray());
                    Inventec.Common.Logging.LogSystem.Info("end qr");
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
                Inventec.Common.BarcodeLib.Barcode barcodeTestServiceReq = new Inventec.Common.BarcodeLib.Barcode(data);
                barcodeTestServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTestServiceReq.IncludeLabel = false;
                barcodeTestServiceReq.Width = 220;
                barcodeTestServiceReq.Height = 40;
                barcodeTestServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTestServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTestServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTestServiceReq.IncludeLabel = true;
                result = Inventec.Common.FlexCellExport.Common.ConverterImageToArray(barcodeTestServiceReq.Encode(barcodeTestServiceReq.EncodedType, barcodeTestServiceReq.RawData));
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #region QRCODE
        private void processQrCode(SereServQrADO sereServADO)
        {
            try
            {
                QrDiim(sereServADO);
                QrDiimV2(sereServADO);
                QrCT540(sereServADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void QrDiim(SereServQrADO sereServADO)
        {
            try
            {
                if (sereServADO != null)
                {
                    List<string> qrInfos = new List<string>();
                    qrInfos.Add(rdo.ServiceReqPrint.TDL_PATIENT_CODE);

                    int namSinh = 0;
                    int.TryParse(rdo.ServiceReqPrint.TDL_PATIENT_DOB.ToString().Substring(0, 4), out namSinh);
                    DateTime dtNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo.ServiceReqPrint.INTRUCTION_TIME) ?? DateTime.Now;
                    int tuoi = dtNow.Year - namSinh;
                    if (tuoi < 6)
                    {
                        qrInfos.Add(Inventec.Common.String.Convert.UnSignVNese2(rdo.ServiceReqPrint.TDL_PATIENT_NAME) + " " + CalculatorAge(rdo.ServiceReqPrint.INTRUCTION_TIME, rdo.ServiceReqPrint.TDL_PATIENT_DOB));
                        qrInfos.Add(namSinh.ToString());
                        qrInfos.Add(" ");
                    }
                    else
                    {
                        qrInfos.Add(Inventec.Common.String.Convert.UnSignVNese2(rdo.ServiceReqPrint.TDL_PATIENT_NAME));
                        qrInfos.Add(namSinh.ToString());
                        qrInfos.Add(tuoi.ToString());
                    }

                    if (rdo.ServiceReqPrint.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        qrInfos.Add("M");
                    }
                    else if (rdo.ServiceReqPrint.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        qrInfos.Add("F");
                    }
                    else
                    {
                        qrInfos.Add("O");
                    }

                    qrInfos.Add(sereServADO.ID.ToString());
                    qrInfos.Add(" ");

                    string totalQrInfo = string.Join("\t", qrInfos);
                    QRCodeGenerator qrInfo = new QRCodeGenerator();
                    QRCodeData qrInfoData = qrInfo.CreateQrCode(totalQrInfo, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrICode = new BitmapByteQRCode(qrInfoData);
                    byte[] qrICodeImage = qrICode.GetGraphic(20);
                    sereServADO.ServiceReqExecuteQr = qrICodeImage;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void QrDiimV2(SereServQrADO sereServADO)
        {
            try
            {
                if (sereServADO != null)
                {
                    List<string> qrInfos = new List<string>();
                    qrInfos.Add(rdo.ServiceReqPrint.TDL_PATIENT_CODE);

                    int namSinh = 0;
                    int.TryParse(rdo.ServiceReqPrint.TDL_PATIENT_DOB.ToString().Substring(0, 4), out namSinh);
                    DateTime dtNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo.ServiceReqPrint.INTRUCTION_TIME) ?? DateTime.Now;
                    int tuoi = dtNow.Year - namSinh;
                    if (tuoi < 6)
                    {
                        qrInfos.Add(Inventec.Common.String.Convert.UnSignVNese2(rdo.ServiceReqPrint.TDL_PATIENT_NAME) + " " + CalculatorAge(rdo.ServiceReqPrint.INTRUCTION_TIME, rdo.ServiceReqPrint.TDL_PATIENT_DOB));
                        qrInfos.Add(namSinh.ToString());
                        qrInfos.Add(" ");
                    }
                    else
                    {
                        qrInfos.Add(Inventec.Common.String.Convert.UnSignVNese2(rdo.ServiceReqPrint.TDL_PATIENT_NAME));
                        qrInfos.Add(namSinh.ToString());
                        qrInfos.Add(tuoi.ToString());
                    }

                    if (rdo.ServiceReqPrint.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        qrInfos.Add("M");
                    }
                    else if (rdo.ServiceReqPrint.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        qrInfos.Add("F");
                    }
                    else
                    {
                        qrInfos.Add("O");
                    }

                    qrInfos.Add("");
                    qrInfos.Add(sereServADO.ID.ToString());

                    string totalQrInfo = string.Join("\t", qrInfos);
                    QRCodeGenerator qrInfo = new QRCodeGenerator();
                    QRCodeData qrInfoData = qrInfo.CreateQrCode(totalQrInfo, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrICode = new BitmapByteQRCode(qrInfoData);
                    byte[] qrICodeImage = qrICode.GetGraphic(20);
                    sereServADO.QrDiimV2 = qrICodeImage;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void QrCT540(SereServADO sereServADO)
        {
            try
            {
                if (sereServADO != null)
                {
                    List<string> qrInfos = new List<string>();
                    qrInfos.Add(sereServADO.TDL_SERVICE_REQ_CODE);
                    qrInfos.Add(sereServADO.TDL_TREATMENT_CODE);
                    qrInfos.Add(rdo.ServiceReqPrint.TDL_PATIENT_NAME);

                    if (rdo.ServiceReqPrint.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        qrInfos.Add("M");
                    }
                    else if (rdo.ServiceReqPrint.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        qrInfos.Add("F");
                    }
                    else
                    {
                        qrInfos.Add("O");
                    }

                    qrInfos.Add("");

                    int namSinh = 0;
                    int.TryParse(rdo.ServiceReqPrint.TDL_PATIENT_DOB.ToString().Substring(0, 4), out namSinh);
                    DateTime dtNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo.ServiceReqPrint.INTRUCTION_TIME) ?? DateTime.Now;
                    int tuoi = dtNow.Year - namSinh;
                    qrInfos.Add(tuoi.ToString());

                    string totalQrInfo = string.Join("\t", qrInfos);
                    QRCodeGenerator qrInfo = new QRCodeGenerator();
                    QRCodeData qrInfoData = qrInfo.CreateQrCode(totalQrInfo, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrICode = new BitmapByteQRCode(qrInfoData);
                    byte[] qrICodeImage = qrICode.GetGraphic(20);
                    qrCT540 = qrICodeImage;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void QrCodePatient()
        {
            try
            {
                if (rdo.ServiceReqPrint != null)
                {
                    List<string> infos = new List<string>();
                    infos.Add(rdo.ServiceReqPrint.TDL_PATIENT_CODE);
                    infos.Add(rdo.ServiceReqPrint.TDL_PATIENT_NAME);
                    infos.Add(rdo.ServiceReqPrint.TDL_PATIENT_GENDER_NAME);
                    infos.Add(rdo.ServiceReqPrint.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    infos.Add(rdo.ServiceReqPrint.SERVICE_REQ_CODE);

                    string totalInfo = string.Join("\t", infos);
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(totalInfo, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                    byte[] qrCodeImage = qrCode.GetGraphic(20);
                    SetSingleKey(Mps000028ExtendSingleKey.QRCODE_PATIENT, qrCodeImage);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
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

                SetQRcodeSereServKey();

                ProcessSingleKey();
                SetQrCode();

                SetImageKey();

                this.SetSignatureKeyImageByCFG();

                singleTag.ProcessData(store, singleValueDictionary);
                //if (rdo.sereServADOs != null && rdo.sereServADOs.Count > 0)
                //    objectTag.AddObjectData(store, "SereServ", rdo.sereServADOs);
                objectTag.AddObjectData(store, "SereServ", ListSereServADOs);

                barCodeTag.ProcessData(store, dicImage);
                objectTag.SetUserFunction(store, "FuncRownumber", new CustomerFuncRownumberData());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                if (rdo.ServiceReqPrint != null)
                {
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.ServiceReqPrint.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.STR_YEAR, rdo.ServiceReqPrint.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.ServiceReqPrint.TDL_PATIENT_DOB)));

                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.FINISH_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.START_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.START_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.ServiceReqPrint.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.ServiceReqPrint.FINISH_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.ServiceReqPrint.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        rdo.ServiceReqPrint.INTRUCTION_TIME)));

                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.REQ_ICD_CODE, rdo.ServiceReqPrint.ICD_CODE));
                    //SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.REQ_ICD_MAIN_TEXT, rdo.ServiceReqPrint.ICD_MAIN_TEXT));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.REQ_ICD_NAME, rdo.ServiceReqPrint.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.REQ_ICD_SUB_CODE, rdo.ServiceReqPrint.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.REQ_ICD_TEXT, rdo.ServiceReqPrint.ICD_TEXT));

                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.DEPARTMENT_NAME, rdo.ServiceReqPrint.EXECUTE_DEPARTMENT_NAME));

                    if (rdo.currentHisTreatment == null) rdo.currentHisTreatment = new HIS_TREATMENT();

                    string Address = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_ADDRESS) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_ADDRESS :
                        rdo.currentHisTreatment.TDL_PATIENT_ADDRESS;
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.ADDRESS, Address));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.VIR_ADDRESS, Address));

                    string career = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_CAREER_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_CAREER_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_CAREER_NAME;
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.CAREER_NAME, career));

                    string code = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_CODE) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_CODE :
                        rdo.currentHisTreatment.TDL_PATIENT_CODE;
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.PATIENT_CODE, code));

                    long dob = rdo.ServiceReqPrint.TDL_PATIENT_DOB > 0 ?
                        rdo.ServiceReqPrint.TDL_PATIENT_DOB :
                        rdo.currentHisTreatment.TDL_PATIENT_DOB;
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.DOB, dob > 0 ? dob : 00000000000000));

                    string gender = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_GENDER_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_GENDER_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_GENDER_NAME;
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.GENDER_NAME, gender));

                    string rank = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_MILITARY_RANK_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_MILITARY_RANK_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_MILITARY_RANK_NAME;
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.MILITARY_RANK_NAME, rank));

                    string name = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_NAME;
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.VIR_PATIENT_NAME, name));

                    string qg = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_NATIONAL_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_NATIONAL_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_NATIONAL_NAME;
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.NATIONAL_NAME, qg));

                    string work = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE :
                        rdo.currentHisTreatment.TDL_PATIENT_WORK_PLACE;
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WORK_PLACE, work));

                    string work_name = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_WORK_PLACE_NAME;
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WORK_PLACE_NAME, work_name));

                    SetSingleKey(new KeyValue("IS_EMERGENCY_REQ", rdo.ServiceReqPrint.IS_EMERGENCY));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.REQ_PROVISIONAL_DIAGNOSIS, rdo.ServiceReqPrint.PROVISIONAL_DIAGNOSIS));
                }

                decimal bhytthanhtoan_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (rdo.sereServADOs != null && rdo.sereServADOs.Count > 0)
                {
                    bhytthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                }

                if (rdo.PatyAlterBhyt != null)
                {
                    if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (rdo.Mps000028ADO != null && rdo.Mps000028ADO.PatientTypeId__Bhyt == rdo.PatyAlterBhyt.PATIENT_TYPE_ID)
                    {
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, SetHeinCardNumberDisplayByNumber(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.IS_NOT_HEIN, ""));
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)))));
                        SetSingleKey((new KeyValue(Mps000028ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)))));
                        SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.HEIN_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.IS_HEIN, ""));
                        SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.IS_VIENPHI, "X"));
                        SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.IS_NOT_HEIN, "X"));
                    }
                }

                SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));

                if (rdo.Mps000028ADO != null)
                {
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.RATIO, rdo.Mps000028ADO.ratio));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.RATIO_STR, (rdo.Mps000028ADO.ratio * 100) + "%"));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.FIRST_EXAM_ROOM_NAME, rdo.Mps000028ADO.firstExamRoomName));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.BED_ROOM_NAME, rdo.Mps000028ADO.bebRoomName));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.PARENT_NAME, rdo.Mps000028ADO.PARENT_NAME));

                    AddObjectKeyIntoListkey<Mps000028ADO>(rdo.Mps000028ADO, false);
                }

                if (rdo._HIS_WORK_PLACE != null && rdo._HIS_WORK_PLACE.ID > 0)
                {
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WP_ADDRESS, rdo._HIS_WORK_PLACE.ADDRESS));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WP_CONTACT_MOBILE, rdo._HIS_WORK_PLACE.CONTACT_MOBILE));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WP_CONTACT_NAME, rdo._HIS_WORK_PLACE.CONTACT_NAME));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WP_DIRECTOR_NAME, rdo._HIS_WORK_PLACE.DIRECTOR_NAME));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WP_GROUP_CODE, rdo._HIS_WORK_PLACE.GROUP_CODE));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WP_PHONE, rdo._HIS_WORK_PLACE.PHONE));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WP_TAX_CODE, rdo._HIS_WORK_PLACE.TAX_CODE));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WP_WORK_PLACE_CODE, rdo._HIS_WORK_PLACE.WORK_PLACE_CODE));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.WP_WORK_PLACE_NAME, rdo._HIS_WORK_PLACE.WORK_PLACE_NAME));
                }
                if (rdo._HIS_DHST != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo._HIS_DHST, false);
                }

                SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.LOGIN_USER_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.LOGIN_LOGIN_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()));

                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReqPrint, true);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentHisTreatment, true);
                AddObjectKeyIntoListkey<V_HIS_BED_LOG>(rdo.BedLog, false);

                string isPaid = "0";
                ProcesPaid(ref isPaid);

                SetSingleKey(new KeyValue("IS_PAID", isPaid));

                string payFormCode = "";
                string cardCode = "";
                string bankCardCode = "";
                if (rdo.ListTransaction != null && rdo.ListTransaction.Count > 0)
                {
                    List<string> payFormCodes = rdo.ListTransaction.Select(s => s.PAY_FORM_CODE).Distinct().ToList();
                    payFormCode = string.Join(",", payFormCodes);
                    cardCode = rdo.ListTransaction.Select(s => s.TDL_CARD_CODE).FirstOrDefault(o => !String.IsNullOrWhiteSpace(o));
                    bankCardCode = rdo.ListTransaction.Select(s => s.TDL_BANK_CARD_CODE).FirstOrDefault(o => !String.IsNullOrWhiteSpace(o));
                    var transaction_card_code = rdo.ListTransaction.FirstOrDefault(o => o.TDL_CARD_CODE != null);
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.CARD_CODE, transaction_card_code != null ? transaction_card_code.TDL_CARD_CODE : null));
                    var transaction_bank_card_code = rdo.ListTransaction.FirstOrDefault(o => o.TDL_BANK_CARD_CODE != null);
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.BANK_CARD_CODE, transaction_bank_card_code != null ? transaction_bank_card_code.TDL_BANK_CARD_CODE : null));
                }

                SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.PAY_FORM_CODE, payFormCode));
                SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.CARD_CODE, cardCode));
                SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.BANK_CARD_CODE, bankCardCode));

                if (ListSereServADOs != null && ListSereServADOs.Count == 1)
                {
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.SERVICE_CODE, ListSereServADOs.FirstOrDefault().TDL_SERVICE_CODE));
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.SERVICE_NAME, ListSereServADOs.FirstOrDefault().TDL_SERVICE_NAME));
                }
                if (rdo.TransReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000028ExtendSingleKey.PAYMENT_AMOUNT, rdo.TransReq.AMOUNT));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetQrCode()
        {
            try
            {
                if (rdo.TransReq != null && rdo.ListConfigs != null && rdo.ListConfigs.Count > 0)
                {
                    var data = QrCodeProcessor.CreateQrImage(rdo.TransReq, rdo.ListConfigs);
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            SetSingleKey(new KeyValue(item.Key, item.Value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcesPaid(ref string isPaid)
        {
            try
            {
                if (rdo.sereServADOs != null && rdo.sereServADOs.Count > 0 && ((rdo.ListSereServBill != null && rdo.ListSereServBill.Count > 0) || (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)))
                {
                    Dictionary<long, decimal> dicTran = new Dictionary<long, decimal>();

                    if (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)
                    {
                        var sereServDeposit = rdo.ListSereServDeposit.Where(o => rdo.sereServADOs.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
                        if (sereServDeposit != null && sereServDeposit.Count > 0)
                        {
                            var grSereServDeposit = sereServDeposit.GroupBy(s => s.SERE_SERV_ID).ToList();
                            foreach (var item in grSereServDeposit)
                            {
                                dicTran[item.Key] = item.Sum(s => s.AMOUNT);
                            }
                        }
                    }

                    if (rdo.ListSereServBill != null && rdo.ListSereServBill.Count > 0)
                    {
                        var sereServBill = rdo.ListSereServBill.Where(o => rdo.sereServADOs.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
                        if (sereServBill != null && sereServBill.Count > 0)
                        {
                            var grSereServBill = sereServBill.GroupBy(s => s.SERE_SERV_ID).ToList();
                            foreach (var item in grSereServBill)
                            {
                                dicTran[item.Key] = item.Sum(s => s.PRICE);
                            }
                        }
                    }

                    //TỒN TẠI dữ liệu thanh toán/tạm ứng tương ứng
                    if (dicTran.Count > 0)
                    {
                        bool paid = true;
                        foreach (var item in rdo.sereServADOs)
                        {
                            //TẤT CẢ các dịch vụ có số tiền BN cần thanh toán đều có bản ghi tạm ứng dịch vụ/thanh toán tương ứng
                            if (item.VIR_TOTAL_PATIENT_PRICE > 0 && !dicTran.ContainsKey(item.ID))
                            {
                                paid = false;
                                break;
                            }
                        }

                        isPaid = paid ? "1" : "0";
                    }
                }
            }
            catch (Exception ex)
            {
                isPaid = "0";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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
                LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        private string CalculatorAge(long dtNow, long ageYearNumber)
        {
            string tuoi = "";
            try
            {
                string caption__Tuoi = "T";
                string caption__ThangTuoi = "TH";
                string caption__NgayTuoi = "NT";
                string caption__GioTuoi = "GT";

                if (ageYearNumber > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ageYearNumber).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    DateTime current = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dtNow) ?? DateTime.Now;

                    TimeSpan diff__hour = (current - dtNgSinh);
                    TimeSpan diff__month = (current.Date - dtNgSinh.Date);

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;
                    if (hour < 24)
                    {
                        tuoi = ((int)hour + caption__GioTuoi);
                    }
                    else
                    {
                        long tongsogiay__hour = diff__hour.Ticks;
                        System.DateTime newDate__hour = new System.DateTime(tongsogiay__hour);
                        int month__hour = ((newDate__hour.Year - 1) * 12 + newDate__hour.Month - 1);
                        if (month__hour == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            tuoi = ((int)diff__month.TotalDays + caption__NgayTuoi);
                        }
                        else
                        {
                            long tongsogiay = diff__month.Ticks;
                            System.DateTime newDate = new System.DateTime(tongsogiay);
                            int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                            if (month == 0)
                            {
                                //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                                tuoi = ((int)diff__month.TotalDays + caption__NgayTuoi);
                            }
                            else
                            {
                                //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                                if (month < 72)
                                {
                                    tuoi = (month + caption__ThangTuoi);
                                }
                                //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                                else
                                {
                                    int year = current.Year - dtNgSinh.Year;
                                    tuoi = (year + caption__Tuoi);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                tuoi = "";
            }
            return tuoi;
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã điều trị: " + rdo.ServiceReqPrint.TREATMENT_CODE;
                log += " , Mã yêu cầu: " + rdo.ServiceReqPrint.SERVICE_REQ_CODE;
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
                if (rdo != null && rdo.ServiceReqPrint != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.ServiceReqPrint.TREATMENT_CODE;
                    string serviceReqCode = "SERVICE_REQ_CODE:" + rdo.ServiceReqPrint.SERVICE_REQ_CODE;
                    string serviceCode = "";
                    string parentName = "";
                    if (rdo.sereServADOs != null && rdo.sereServADOs.Count > 0)
                    {
                        var serviceFirst = rdo.sereServADOs.OrderBy(o => o.TDL_SERVICE_CODE).First();
                        serviceCode = "SERVICE_CODE:" + serviceFirst.TDL_SERVICE_CODE;
                    }

                    if (rdo.Mps000028ADO != null && !String.IsNullOrWhiteSpace(rdo.Mps000028ADO.PARENT_NAME))
                    {
                        parentName = ProcessParentName(rdo.Mps000028ADO.PARENT_NAME);
                    }

                    result = String.Format("{0} {1} {2} {3} {4}", printTypeCode, treatmentCode, serviceReqCode, serviceCode, parentName);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string ProcessParentName(string name)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    List<string> word = name.Split(' ').ToList();
                    foreach (string item in word)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            result += char.ToUpper(item[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal void SetImageKey()
        {
            try
            {
                bool isBhytAndAvtNull = true;
                if (rdo.currentHisTreatment != null && !String.IsNullOrEmpty(rdo.currentHisTreatment.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(Mps000028ExtendSingleKey.IMG_AVATAR, rdo.currentHisTreatment.TDL_PATIENT_AVATAR_URL);
                    isBhytAndAvtNull = false;
                }

                if (isBhytAndAvtNull)
                {
                    SetSingleKey(Mps000028ExtendSingleKey.AVT_AND_BHYT_NULL, "1");
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
    }

    class CustomerFuncRownumberData : TFlexCelUserFunction
    {
        public CustomerFuncRownumberData()
        {
        }
        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length < 1)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

            long result = 0;
            try
            {
                long rownumber = Convert.ToInt64(parameters[0]);
                result = (rownumber + 1);
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }

            return result;
        }
    }
}
