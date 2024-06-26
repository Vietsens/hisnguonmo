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
using Inventec.Common.QRCoder;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000340.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000340
{
    public class Mps000340Processor : AbstractProcessor
    {
        Mps000340PDO rdo;
        List<Mps000340ServiceReq> Req = new List<Mps000340ServiceReq>();
        public Mps000340Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000340PDO)rdoBase;
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

                        dicImage.Add(Mps000340ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);

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

                        dicImage.Add(Mps000340ExtendSingleKey.TREATMENT_CODE_BAR_5, barcodeTreatment5);
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

                        dicImage.Add(Mps000340ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatient);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                SetSingleKey();
                SetListRdo();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                rdo.SereServs = (rdo.SereServs != null && rdo.SereServs.Count > 0) ? rdo.SereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER ?? 0).ThenBy(p => p.ID).ToList() : rdo.SereServs;

                objectTag.AddObjectData(store, "ServiceReq", Req);
                objectTag.AddObjectData(store, "SereServ", rdo.SereServs);
                objectTag.AddObjectData(store, "ServiceType", rdo.ListServiceType);
                objectTag.AddRelationship(store, "ServiceReq", "ServiceType", "ID", "SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ServiceReq", "SereServ", "ID", "SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ServiceType", "SereServ", "SERVICE_TYPE_GROUP_ID", "SERVICE_TYPE_GROUP_ID");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetListRdo()
        {
            try
            {
                if (rdo.SereServs != null && rdo.SereServs.Count > 0)
                {
                    if (rdo.SereServs.Exists(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA))
                    {
                        var ct540 = QrCT540(rdo.SereServs.First());

                        foreach (var sereServADO in rdo.SereServs)
                        {
                            if (sereServADO.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA) continue;
                            sereServADO.QrCT540 = ct540;
                        }
                    }
                    var SereServSereServIdQr =  SereServSereServIdQrQR(rdo.SereServs.First());
                    foreach (var sereServADO in rdo.SereServs)
                    {
                        sereServADO.SereServIdQr = SereServSereServIdQr;
                    }
                }

                if (rdo.ListServiceReqPrint != null && rdo.ListServiceReqPrint.Count > 0)
                {
                    List<Task> taskall = new List<Task>();
                    foreach (var item in rdo.ListServiceReqPrint)
                    {
                        Mps000340ServiceReq rq = new Mps000340ServiceReq();
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000340ServiceReq>(rq, item);

                        if (rdo.MaxNumOrderSDO != null && rdo.MaxNumOrderSDO.Count > 0)
                        {
                            var roomSdo = rdo.MaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == item.EXECUTE_ROOM_ID);
                            if (roomSdo != null)
                            {
                                rq.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo.MAX_NUM_ORDER;
                            }
                        }

                        if (!String.IsNullOrEmpty(item.SERVICE_REQ_CODE))
                        {
                            Task req = Task.Factory.StartNew((object obj) =>
                            {
                                if (obj != null)
                                {
                                    Mps000340ServiceReq data = obj as Mps000340ServiceReq;
                                    data.REQ_BAR = ProcessBarcode(data.SERVICE_REQ_CODE);
                                }
                            }, rq);
                            taskall.Add(req);
                        }

                        if (!String.IsNullOrEmpty(item.BARCODE))
                        {
                            Task bar = Task.Factory.StartNew((object obj) =>
                            {
                                if (obj != null)
                                {
                                    Mps000340ServiceReq data = obj as Mps000340ServiceReq;
                                    data.TEST_BAR = ProcessBarcode(data.BARCODE);
                                }
                            }, rq);
                            taskall.Add(bar);
                        }

                        if (!String.IsNullOrEmpty(item.ASSIGN_TURN_CODE))
                        {
                            Task bar = Task.Factory.StartNew((object obj) =>
                            {
                                if (obj != null)
                                {
                                    Mps000340ServiceReq data = obj as Mps000340ServiceReq;
                                    data.ASSIGN_TURN_BAR = ProcessBarcode(data.ASSIGN_TURN_CODE);
                                }
                            }, rq);
                            taskall.Add(bar);
                        }

                        Req.Add(rq);
                    }

                    Task.WaitAll(taskall.ToArray());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region barcode
        private byte[] ProcessBarcode(string data)
        {
            byte[] result = null;
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTestServiceReq = new Inventec.Common.BarcodeLib.Barcode(data);
                barcodeTestServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTestServiceReq.IncludeLabel = false;
                barcodeTestServiceReq.Width = 120;
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
        #endregion

        #region QRCODE
        private void processQrCode(SereServADO sereServADO)
        {
            try
            {
                //QrDiim(sereServADO);
                //QrDiimV2(sereServADO);
                QrCT540(sereServADO);
                SereServSereServIdQrQR(sereServADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private byte[] SereServSereServIdQrQR(SereServADO sereServADO)
        {
            byte[] result = null;
            try
            {
              
                List<string> qrInfos = new List<string>();
                qrInfos.Add(sereServADO.ID.ToString());
                string totalQrInfo = string.Join("\t", qrInfos);
                QRCodeGenerator qrInfo = new QRCodeGenerator();
                QRCodeData qrInfoData = qrInfo.CreateQrCode(totalQrInfo, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrICode = new BitmapByteQRCode(qrInfoData);
                byte[] qrICodeImage = qrICode.GetGraphic(20);
                result = qrICodeImage;
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void QrDiim(SereServADO sereServADO)
        {
            try
            {
                if (sereServADO != null)
                {
                    List<string> qrInfos = new List<string>();
                    qrInfos.Add(rdo.CurrentHisTreatment.TDL_PATIENT_CODE);

                    int namSinh = 0;
                    int.TryParse(rdo.CurrentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4), out namSinh);
                    DateTime dtNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sereServADO.TDL_INTRUCTION_TIME) ?? DateTime.Now;
                    int tuoi = dtNow.Year - namSinh;
                    if (tuoi < 6)
                    {
                        qrInfos.Add(Inventec.Common.String.Convert.UnSignVNese2(rdo.CurrentHisTreatment.TDL_PATIENT_NAME) + " " + CalculatorAge(sereServADO.TDL_INTRUCTION_TIME, rdo.CurrentHisTreatment.TDL_PATIENT_DOB));
                        qrInfos.Add(namSinh.ToString());
                        qrInfos.Add(" ");
                    }
                    else
                    {
                        qrInfos.Add(Inventec.Common.String.Convert.UnSignVNese2(rdo.CurrentHisTreatment.TDL_PATIENT_NAME));
                        qrInfos.Add(namSinh.ToString());
                        qrInfos.Add(tuoi.ToString());
                    }

                    if (rdo.CurrentHisTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        qrInfos.Add("M");
                    }
                    else if (rdo.CurrentHisTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
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

        private void QrDiimV2(SereServADO sereServADO)
        {
            try
            {
                if (sereServADO != null)
                {
                    List<string> qrInfos = new List<string>();
                    qrInfos.Add(rdo.CurrentHisTreatment.TDL_PATIENT_CODE);

                    int namSinh = 0;
                    int.TryParse(rdo.CurrentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4), out namSinh);
                    DateTime dtNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sereServADO.TDL_INTRUCTION_TIME) ?? DateTime.Now;
                    int tuoi = dtNow.Year - namSinh;
                    if (tuoi < 6)
                    {
                        qrInfos.Add(Inventec.Common.String.Convert.UnSignVNese2(rdo.CurrentHisTreatment.TDL_PATIENT_NAME) + " " + CalculatorAge(sereServADO.TDL_INTRUCTION_TIME, rdo.CurrentHisTreatment.TDL_PATIENT_DOB));
                        qrInfos.Add(namSinh.ToString());
                        qrInfos.Add(" ");
                    }
                    else
                    {
                        qrInfos.Add(Inventec.Common.String.Convert.UnSignVNese2(rdo.CurrentHisTreatment.TDL_PATIENT_NAME));
                        qrInfos.Add(namSinh.ToString());
                        qrInfos.Add(tuoi.ToString());
                    }

                    if (rdo.CurrentHisTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        qrInfos.Add("M");
                    }
                    else if (rdo.CurrentHisTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
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

        private byte[] QrCT540(SereServADO sereServADO)
        {
            byte[] result = null;
            try
            {
                if (sereServADO != null)
                {
                    List<string> qrInfos = new List<string>();
                    qrInfos.Add(sereServADO.TDL_SERVICE_REQ_CODE);
                    qrInfos.Add(sereServADO.TDL_TREATMENT_CODE);
                    qrInfos.Add(rdo.CurrentHisTreatment.TDL_PATIENT_NAME);

                    if (rdo.CurrentHisTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        qrInfos.Add("M");
                    }
                    else if (rdo.CurrentHisTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        qrInfos.Add("F");
                    }
                    else
                    {
                        qrInfos.Add("O");
                    }

                    qrInfos.Add("");

                    int namSinh = 0;
                    int.TryParse(rdo.CurrentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4), out namSinh);
                    DateTime dtNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sereServADO.TDL_INTRUCTION_TIME) ?? DateTime.Now;
                    int tuoi = dtNow.Year - namSinh;
                    qrInfos.Add(tuoi.ToString());

                    string totalQrInfo = string.Join("\t", qrInfos);
                    QRCodeGenerator qrInfo = new QRCodeGenerator();
                    QRCodeData qrInfoData = qrInfo.CreateQrCode(totalQrInfo, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrICode = new BitmapByteQRCode(qrInfoData);
                    byte[] qrICodeImage = qrICode.GetGraphic(20);
                    result = qrICodeImage;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        #endregion

        void SetSingleKey()
        {
            try
            {
                if (rdo.CurrentHisTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.CurrentHisTreatment.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.STR_YEAR, rdo.CurrentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentHisTreatment.TDL_PATIENT_DOB)));
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
                            SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (rdo.Mps000340ADO != null && rdo.PatyAlterBhyt != null && rdo.Mps000340ADO.PatientTypeId__Bhyt == rdo.PatyAlterBhyt.PATIENT_TYPE_ID)
                    {
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)))));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)))));
                        SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.HEIN_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000340ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));

                if (rdo.Mps000340ADO != null)
                {
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.FIRST_EXAM_ROOM_NAME, rdo.Mps000340ADO.firstExamRoomName));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.RATIO, rdo.Mps000340ADO.ratio));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.RATIO_STR, (rdo.Mps000340ADO.ratio * 100) + "%"));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.BED_ROOM_NAME, rdo.Mps000340ADO.bebRoomName));

                    AddObjectKeyIntoListkey<Mps000340ADO>(rdo.Mps000340ADO, false);
                }

                if (rdo._HIS_WORK_PLACE != null && rdo._HIS_WORK_PLACE.ID > 0)
                {
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.WP_ADDRESS, rdo._HIS_WORK_PLACE.ADDRESS));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.WP_CONTACT_MOBILE, rdo._HIS_WORK_PLACE.CONTACT_MOBILE));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.WP_CONTACT_NAME, rdo._HIS_WORK_PLACE.CONTACT_NAME));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.WP_DIRECTOR_NAME, rdo._HIS_WORK_PLACE.DIRECTOR_NAME));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.WP_GROUP_CODE, rdo._HIS_WORK_PLACE.GROUP_CODE));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.WP_PHONE, rdo._HIS_WORK_PLACE.PHONE));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.WP_TAX_CODE, rdo._HIS_WORK_PLACE.TAX_CODE));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.WP_WORK_PLACE_CODE, rdo._HIS_WORK_PLACE.WORK_PLACE_CODE));
                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.WP_WORK_PLACE_NAME, rdo._HIS_WORK_PLACE.WORK_PLACE_NAME));
                }

                if (rdo._HIS_DHST != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo._HIS_DHST, false);
                }

                if (rdo.ListServiceReqPrint != null && rdo.ListServiceReqPrint.Count > 0)
                {
                    var reqOrder = rdo.ListServiceReqPrint.OrderBy(o => o.ESTIMATE_TIME_FROM ?? 99999999999999).ThenBy(o => o.INTRUCTION_TIME).ThenBy(o => o.ID).Select(s => s.EXECUTE_ROOM_NAME).ToList();
                    //Lúc in ra, cần xử lý để ko in ra dữ liệu có 2 phòng trùng nhau và xếp liên tiếp nhau:
                    List<string> order = new List<string>();
                    order.Add(reqOrder[0]);
                    if (reqOrder.Count > 1)
                    {
                        for (int i = 1; i < reqOrder.Count; i++)
                        {
                            if (reqOrder[i] == reqOrder[i - 1])
                            {
                                continue;
                            }

                            order.Add(reqOrder[i]);
                        }
                    }

                    string estimateTimeOrder = string.Join(" --> ", order);

                    SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.ESTIMATE_TIME_ORDER, estimateTimeOrder));
                }

                SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.LOGIN_USER_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                SetSingleKey(new KeyValue(Mps000340ExtendSingleKey.LOGIN_LOGIN_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()));

                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.CurrentHisTreatment, true);
                AddObjectKeyIntoListkey<V_HIS_BED_LOG>(rdo.BedLog, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
