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
using MPS.Processor.Mps000026.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MPS.Processor.Mps000026
{
    public partial class Mps000026Processor : AbstractProcessor
    {
        Mps000026PDO rdo;
        List<SereServAdo> listAdo = new List<SereServAdo>();
        List<V_HIS_SERVICE> Listparent = new List<V_HIS_SERVICE>();
        public Mps000026Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000026PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.ServiceReqPrint != null)
                {
                    if (!String.IsNullOrEmpty(rdo.ServiceReqPrint.SERVICE_REQ_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeServiceReqCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReqPrint.SERVICE_REQ_CODE);
                        barcodeServiceReqCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeServiceReqCode.IncludeLabel = false;
                        barcodeServiceReqCode.Width = 120;
                        barcodeServiceReqCode.Height = 40;
                        barcodeServiceReqCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeServiceReqCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeServiceReqCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeServiceReqCode.IncludeLabel = true;

                        dicImage.Add(Mps000026ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeServiceReqCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.ServiceReqPrint.ASSIGN_TURN_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeServiceReqCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReqPrint.ASSIGN_TURN_CODE);
                        barcodeServiceReqCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeServiceReqCode.IncludeLabel = false;
                        barcodeServiceReqCode.Width = 120;
                        barcodeServiceReqCode.Height = 40;
                        barcodeServiceReqCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeServiceReqCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeServiceReqCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeServiceReqCode.IncludeLabel = true;

                        dicImage.Add(Mps000026ExtendSingleKey.ASSIGN_TURN_CODE_BARCODE, barcodeServiceReqCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.ServiceReqPrint.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReqPrint.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000026ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);

                        string treatmentCode = rdo.ServiceReqPrint.TREATMENT_CODE.Substring(rdo.ServiceReqPrint.TREATMENT_CODE.Length - 5, 5);
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment5 = new Inventec.Common.BarcodeLib.Barcode(treatmentCode);
                        barcodeTreatment5.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment5.IncludeLabel = false;
                        barcodeTreatment5.Width = 120;
                        barcodeTreatment5.Height = 40;
                        barcodeTreatment5.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment5.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment5.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment5.IncludeLabel = true;

                        dicImage.Add(Mps000026ExtendSingleKey.TREATMENT_CODE_BAR_5, barcodeTreatment5);
                    }

                    if (!String.IsNullOrEmpty(rdo.ServiceReqPrint.BARCODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTestServiceReq = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReqPrint.BARCODE);
                        barcodeTestServiceReq.RawData = rdo.ServiceReqPrint.BARCODE;
                        barcodeTestServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTestServiceReq.IncludeLabel = false;
                        barcodeTestServiceReq.Width = 120;
                        barcodeTestServiceReq.Height = 40;
                        barcodeTestServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTestServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTestServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTestServiceReq.IncludeLabel = true;

                        dicImage.Add(Mps000026ExtendSingleKey.TEST_SERVICE_REQ_BAR, barcodeTestServiceReq);
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

                        dicImage.Add(Mps000026ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetImageKey()
        {
            try
            {
                bool isBhytAndAvtNull = true;
                if (rdo.CurrentHisTreatment != null && !String.IsNullOrEmpty(rdo.CurrentHisTreatment.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(Mps000026ExtendSingleKey.IMG_AVATAR, rdo.CurrentHisTreatment.TDL_PATIENT_AVATAR_URL);
                    isBhytAndAvtNull = false;
                }

                if (isBhytAndAvtNull)
                {
                    SetSingleKey(Mps000026ExtendSingleKey.AVT_AND_BHYT_NULL, "1");
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
                SetListData();
                SetSingleKey();
                SetSingleKeyQrCode();
                SetImageKey();

                this.SetSignatureKeyImageByCFG();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "SereServ", listAdo);
                objectTag.AddObjectData(store, "ServiceParent", Listparent);
                objectTag.AddRelationship(store, "ServiceParent", "SereServ", "ID", "SERVICE_PARENT_ID");
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

        private void SetSingleKeyQrCode()
        {
            try
            {
                if(rdo.transReq != null && rdo.lstConfig != null && rdo.lstConfig.Count > 0)
                {
                    var data = HIS.Desktop.Common.BankQrCode.QrCodeProcessor.CreateQrImage(rdo.transReq, rdo.lstConfig);
                    foreach (var item in data)
                    {
                        SetSingleKey(new KeyValue(item.Key, item.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetListData()
        {
            try
            {
                if (rdo.SereServs != null && rdo.SereServs.Count > 0)
                {
                    foreach (var item in rdo.SereServs)
                    {
                        var ado = new SereServAdo(item);

                        V_HIS_SERVICE service = rdo.ListService != null ? rdo.ListService.FirstOrDefault(o => o.ID == item.SERVICE_ID) : null;
                        if (service != null)
                        {
                            ado.SERVICE_NUM_ORDER = service.NUM_ORDER;
                            ado.ESTIMATE_DURATION = service.ESTIMATE_DURATION;

                            ado.SERVICE_ORDER = service.NUM_ORDER ?? -1;
                            ado.SERVICE_PARENT_ID = service.PARENT_ID ?? 0;
                            if (service.PARENT_ID.HasValue)
                            {
                                V_HIS_SERVICE parent = rdo.ListService != null ? rdo.ListService.FirstOrDefault(o => o.ID == service.PARENT_ID.Value) : null;
                                if (parent != null)
                                {
                                    ado.SERVICE_PARENT_ORDER = parent.NUM_ORDER ?? -1;
                                }
                            }

                            if (item.SERVICE_CONDITION_ID != null && rdo.ServiceCondition != null)
                            {
                                var Condition = rdo.ServiceCondition.FirstOrDefault(o => o.ID == item.SERVICE_CONDITION_ID);
                                if (Condition != null)
                                {
                                    ado.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                                    ado.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                                }
                            }

                            if (rdo.SereServExts != null && rdo.SereServExts.Count > 0)
                            {
                                var ssExt = rdo.SereServExts.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                                if (ssExt != null)
                                {
                                    ado.CONCLUDE = ssExt.CONCLUDE;
                                    ado.BEGIN_TIME = ssExt.BEGIN_TIME;
                                    ado.END_TIME = ssExt.END_TIME;
                                    ado.INSTRUCTION_NOTE = ssExt.INSTRUCTION_NOTE;
                                    ado.NOTE = ssExt.NOTE;
                                }
                            }
                        }
                        else
                        {
                            ado.SERVICE_ORDER = -1;
                            ado.SERVICE_PARENT_ID = 0;
                            ado.SERVICE_PARENT_ORDER = -1;
                        }

                        listAdo.Add(ado);
                    }
                    listAdo = listAdo.OrderByDescending(o => o.SERVICE_PARENT_ORDER).ThenByDescending(o => o.SERVICE_ORDER).ThenBy(o => o.ID).ThenBy(p => p.SERVICE_NAME).ToList();
                }
                else if (rdo.sereServs26 != null && rdo.sereServs26.Count > 0)
                {
                    foreach (var item in rdo.sereServs26)
                    {
                        var ado = new SereServAdo(item);

                        V_HIS_SERVICE service = rdo.ListService != null ? rdo.ListService.FirstOrDefault(o => o.ID == item.SERVICE_ID) : null;
                        if (service != null)
                        {
                            ado.SERVICE_ORDER = service.NUM_ORDER ?? -1;
                            ado.SERVICE_PARENT_ID = service.PARENT_ID ?? 0;
                            if (service.PARENT_ID.HasValue)
                            {
                                V_HIS_SERVICE parent = rdo.ListService != null ? rdo.ListService.FirstOrDefault(o => o.ID == service.PARENT_ID.Value) : null;
                                if (parent != null)
                                {
                                    ado.SERVICE_PARENT_ORDER = parent.NUM_ORDER ?? -1;
                                }
                            }
                        }
                        else
                        {
                            ado.SERVICE_ORDER = -1;
                            ado.SERVICE_PARENT_ID = 0;
                            ado.SERVICE_PARENT_ORDER = -1;
                        }

                        listAdo.Add(ado);
                    }
                    listAdo = listAdo.OrderByDescending(o => o.SERVICE_PARENT_ORDER).ThenByDescending(o => o.SERVICE_ORDER).ThenBy(o => o.ID).ThenBy(p => p.SERVICE_NAME).ToList();
                }

                var grParent = listAdo.GroupBy(o => o.SERVICE_PARENT_ID).ToList();
                foreach (var gr in grParent)
                {
                    if (rdo.ListService != null && rdo.ListService.Count > 0)
                    {
                        var parent = rdo.ListService.FirstOrDefault(o => o.ID == gr.First().SERVICE_PARENT_ID);
                        if (parent != null)
                        {
                            Listparent.Add(parent);
                        }
                        else
                        {
                            Listparent.Add(new V_HIS_SERVICE() { SERVICE_TYPE_ID = gr.First().TDL_SERVICE_TYPE_ID, SERVICE_NAME = gr.First().SERVICE_TYPE_NAME });
                        }
                    }
                    else
                    {
                        Listparent.Add(new V_HIS_SERVICE() { SERVICE_TYPE_ID = gr.First().TDL_SERVICE_TYPE_ID, SERVICE_NAME = gr.First().SERVICE_TYPE_NAME });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetSingleKey()
        {
            try
            {
                if (rdo.ServiceReqPrint != null)
                {
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.ServiceReqPrint.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.STR_YEAR, rdo.ServiceReqPrint.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.ServiceReqPrint.TDL_PATIENT_DOB)));

                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.BARCODE_PATIENT_CODE_STR, rdo.ServiceReqPrint.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, rdo.ServiceReqPrint.TREATMENT_CODE));

                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.ASSIGN_TURN_CODE, rdo.ServiceReqPrint.ASSIGN_TURN_CODE));

                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.FINISH_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.START_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.START_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.ServiceReqPrint.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.ServiceReqPrint.FINISH_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.ServiceReqPrint.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        rdo.ServiceReqPrint.INTRUCTION_TIME)));

                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.REQ_ICD_CODE, rdo.ServiceReqPrint.ICD_CODE));
                    //SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.REQ_ICD_MAIN_TEXT, rdo.ServiceReqPrint.ICD_MAIN_TEXT));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.REQ_ICD_NAME, rdo.ServiceReqPrint.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.REQ_ICD_SUB_CODE, rdo.ServiceReqPrint.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.REQ_ICD_TEXT, rdo.ServiceReqPrint.ICD_TEXT));

                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.DEPARTMENT_NAME, rdo.ServiceReqPrint.EXECUTE_DEPARTMENT_NAME));

                    if (rdo.CurrentHisTreatment == null) rdo.CurrentHisTreatment = new HIS_TREATMENT();

                    string Address = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_ADDRESS) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_ADDRESS :
                        rdo.CurrentHisTreatment.TDL_PATIENT_ADDRESS;
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.ADDRESS, Address));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.VIR_ADDRESS, Address));

                    string career = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_CAREER_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_CAREER_NAME :
                        rdo.CurrentHisTreatment.TDL_PATIENT_CAREER_NAME;
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.CAREER_NAME, career));

                    string code = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_CODE) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_CODE :
                        rdo.CurrentHisTreatment.TDL_PATIENT_CODE;
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.PATIENT_CODE, code));

                    long dob = rdo.ServiceReqPrint.TDL_PATIENT_DOB > 0 ?
                        rdo.ServiceReqPrint.TDL_PATIENT_DOB :
                        rdo.CurrentHisTreatment.TDL_PATIENT_DOB;
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.DOB, dob > 0 ? dob : 00000000000000));

                    string gender = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_GENDER_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_GENDER_NAME :
                        rdo.CurrentHisTreatment.TDL_PATIENT_GENDER_NAME;
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.GENDER_NAME, gender));

                    string rank = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_MILITARY_RANK_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_MILITARY_RANK_NAME :
                        rdo.CurrentHisTreatment.TDL_PATIENT_MILITARY_RANK_NAME;
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.MILITARY_RANK_NAME, rank));

                    string name = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_NAME :
                        rdo.CurrentHisTreatment.TDL_PATIENT_NAME;
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.VIR_PATIENT_NAME, name));

                    string qg = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_NATIONAL_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_NATIONAL_NAME :
                        rdo.CurrentHisTreatment.TDL_PATIENT_NATIONAL_NAME;
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.NATIONAL_NAME, qg));

                    string work = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE :
                        rdo.CurrentHisTreatment.TDL_PATIENT_WORK_PLACE;
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WORK_PLACE, work));

                    string work_name = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE_NAME :
                        rdo.CurrentHisTreatment.TDL_PATIENT_WORK_PLACE_NAME;
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WORK_PLACE_NAME, work_name));

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
                    SetSingleKey(Mps000026ExtendSingleKey.QRCODE_PATIENT, qrCodeImage);

                    SetSingleKey(new KeyValue("IS_EMERGENCY_REQ", rdo.ServiceReqPrint.IS_EMERGENCY));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.REQ_PROVISIONAL_DIAGNOSIS, rdo.ServiceReqPrint.PROVISIONAL_DIAGNOSIS));
                }

                decimal bhytthanhtoan_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (rdo.SereServs != null && rdo.SereServs.Count > 0)
                {
                    bhytthanhtoan_tong = rdo.SereServs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = rdo.SereServs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                }

                if (rdo.PatyAlterBhyt != null)
                {
                    if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (rdo.Mps000026ADO != null && rdo.PatyAlterBhyt != null && rdo.Mps000026ADO.PatientTypeId__Bhyt == rdo.PatyAlterBhyt.PATIENT_TYPE_ID)
                    {
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)))));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)))));
                        SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.HEIN_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000026ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));

                if (rdo.Mps000026ADO != null)
                {
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.FIRST_EXAM_ROOM_NAME, rdo.Mps000026ADO.firstExamRoomName));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.RATIO, rdo.Mps000026ADO.ratio));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.RATIO_STR, (rdo.Mps000026ADO.ratio * 100) + "%"));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.BED_ROOM_NAME, rdo.Mps000026ADO.bebRoomName));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.PARENT_NAME, rdo.Mps000026ADO.PARENT_NAME));
                    AddObjectKeyIntoListkey<Mps000026ADO>(rdo.Mps000026ADO, false);
                }

                if (rdo.ServiceParent != null)
                {
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.SERVICE_NAME_PARENT, rdo.ServiceParent.SERVICE_NAME));
                }

                if (rdo._HIS_WORK_PLACE != null && rdo._HIS_WORK_PLACE.ID > 0)
                {
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WP_ADDRESS, rdo._HIS_WORK_PLACE.ADDRESS));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WP_CONTACT_MOBILE, rdo._HIS_WORK_PLACE.CONTACT_MOBILE));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WP_CONTACT_NAME, rdo._HIS_WORK_PLACE.CONTACT_NAME));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WP_DIRECTOR_NAME, rdo._HIS_WORK_PLACE.DIRECTOR_NAME));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WP_GROUP_CODE, rdo._HIS_WORK_PLACE.GROUP_CODE));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WP_PHONE, rdo._HIS_WORK_PLACE.PHONE));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WP_TAX_CODE, rdo._HIS_WORK_PLACE.TAX_CODE));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WP_WORK_PLACE_CODE, rdo._HIS_WORK_PLACE.WORK_PLACE_CODE));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.WP_WORK_PLACE_NAME, rdo._HIS_WORK_PLACE.WORK_PLACE_NAME));
                }
                if (rdo._HIS_DHST != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo._HIS_DHST, false);
                }

                SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.LOGIN_USER_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.LOGIN_LOGIN_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()));

                if (listAdo != null && listAdo.Count == 1)
                {
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.SERVICE_CODE, listAdo.FirstOrDefault().TDL_SERVICE_CODE));
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.SERVICE_NAME, listAdo.FirstOrDefault().TDL_SERVICE_NAME));
                }

                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReqPrint, true);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.CurrentHisTreatment, true);
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
                }

                SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.PAY_FORM_CODE, payFormCode));
                SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.CARD_CODE, cardCode));
                SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.BANK_CARD_CODE, bankCardCode));

                AddObjectKeyIntoListkeyWithPrefix(rdo.LisSample, "LIS_SAMPLE_", false);
                if(rdo.transReq != null)
                    SetSingleKey(new KeyValue(Mps000026ExtendSingleKey.PAYMENT_AMOUNT, rdo.transReq.AMOUNT));
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
                if (rdo.SereServs != null && rdo.SereServs.Count > 0 && ((rdo.ListSereServBill != null && rdo.ListSereServBill.Count > 0) || (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)))
                {
                    Dictionary<long, decimal> dicTran = new Dictionary<long, decimal>();

                    if (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)
                    {
                        var sereServDeposit = rdo.ListSereServDeposit.Where(o => rdo.SereServs.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
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
                        var sereServBill = rdo.ListSereServBill.Where(o => rdo.SereServs.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
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
                        foreach (var item in rdo.SereServs)
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
                    LogSystem.Error(ex);
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
                    if (rdo.SereServs != null && rdo.SereServs.Count > 0)
                    {
                        var serviceFirst = rdo.SereServs.OrderBy(o => o.TDL_SERVICE_CODE).First();
                        serviceCode = "SERVICE_CODE:" + serviceFirst.TDL_SERVICE_CODE;
                    }

                    if (rdo.Mps000026ADO != null && !String.IsNullOrWhiteSpace(rdo.Mps000026ADO.PARENT_NAME))
                    {
                        parentName = ProcessParentName(rdo.Mps000026ADO.PARENT_NAME);
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
    }
}
