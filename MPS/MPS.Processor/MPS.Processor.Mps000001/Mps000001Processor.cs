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
using HIS.Desktop.Common.BankQrCode;
using Inventec.Common.Logging;
using Inventec.Common.QRCoder;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000001.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000001
{
    class Mps000001Processor : AbstractProcessor
    {
        Mps000001PDO rdo;
        private const string pointOTMethod = "11";
        private const string bnbID = "970412";
        private const string transferType = "QRIBFTTC";
        private const string ccy = "704";
        private const string countryCode = "VN";
        public Mps000001Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000001PDO)rdoBase;
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.ServiceReq != null)
                {
                    if (!String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.TDL_PATIENT_CODE);
                        barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatientCode.IncludeLabel = false;
                        barcodePatientCode.Width = 120;
                        barcodePatientCode.Height = 40;
                        barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatientCode.IncludeLabel = true;

                        dicImage.Add(Mps000001ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.ServiceReq.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000001ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
                    }

                    if (!String.IsNullOrEmpty(rdo.ServiceReq.SERVICE_REQ_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeServiceReq = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.SERVICE_REQ_CODE);
                        barcodeServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeServiceReq.IncludeLabel = false;
                        barcodeServiceReq.Width = 120;
                        barcodeServiceReq.Height = 40;
                        barcodeServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeServiceReq.IncludeLabel = true;

                        dicImage.Add(Mps000001ExtendSingleKey.BARCODE_SERVICE_REQ_CODE_STR, barcodeServiceReq);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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
                SetSingleKey();
                SetQrCode();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "SereServ", rdo.sereServs);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private void SetQrCode()
        {
            try
            {
                if (rdo.TransReq != null && rdo.ListConfigs != null && rdo.ListConfigs.Count > 0)
                {
                    var data = QrCodeProcessor.CreateQrImage(rdo.TransReq, rdo.ListConfigs);
                    if(data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            SetSingleKey(new KeyValue(item.Key, item.Value));
                        }
                    }
                }
                if(rdo.lstCard != null && rdo.lstCard.Count > 0)
                {
                    var lstcard = rdo.lstCard.Where(o => o.BANK_CARD_CODE.StartsWith(bnbID)).OrderByDescending(o=>o.ID).ToList();
                    if(lstcard != null && rdo.lstCard.Count > 0)
                    {
                        Inventec.Common.BankQrCode.ADO.ConsumerAccountInfo input= new Inventec.Common.BankQrCode.ADO.ConsumerAccountInfo();
                        input.pointOTMethod = pointOTMethod;
                        input.countryCode = countryCode;
                        input.bnbID = bnbID;
                        input.ccy = ccy;
                        input.transferType = transferType;
                        input.ConsumerID = lstcard[0].BANK_CARD_CODE;
                        Inventec.Common.BankQrCode.ADO.BankQrCodeInputADO ado = new Inventec.Common.BankQrCode.ADO.BankQrCodeInputADO();
                        ado.ConsumerInfo = input;
                        Inventec.Common.BankQrCode.BankQrCodeProcessor processor = new Inventec.Common.BankQrCode.BankQrCodeProcessor(ado);
                        var qrCode = processor.GetConsumerQrCode(Inventec.Common.BankQrCode.ProvinceType.PVCB);
                        QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                        QRCodeData data2 = qRCodeGenerator.CreateQrCode(qrCode.Data, QRCodeGenerator.ECCLevel.Q);
                        BitmapByteQRCode bitmapByteQRCode = new BitmapByteQRCode(data2);
                        byte[] graphic = bitmapByteQRCode.GetGraphic(20);
                        SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.DEPOSIT_QR_CODE_PVCB, graphic));
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
                if (rdo.ServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.FINISH_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReq.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.START_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReq.START_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.PRIOTY, rdo.ServiceReq.PRIORITY));

                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.ServiceReq.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.ServiceReq.FINISH_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.ServiceReq.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        rdo.ServiceReq.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.INTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo.ServiceReq.INTRUCTION_TIME))));

                    List<string> infos = new List<string>();
                    infos.Add(rdo.ServiceReq.TDL_PATIENT_CODE);
                    infos.Add(rdo.ServiceReq.TDL_PATIENT_NAME);
                    infos.Add(rdo.ServiceReq.TDL_PATIENT_GENDER_NAME);
                    infos.Add(rdo.ServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    infos.Add(rdo.ServiceReq.SERVICE_REQ_CODE);

                    string totalInfo = string.Join("\t", infos);
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(totalInfo, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                    byte[] qrCodeImage = qrCode.GetGraphic(20);
                    SetSingleKey(Mps000001ExtendSingleKey.QRCODE_PATIENT, qrCodeImage);
                }

                if (rdo.Mps000001ADO != null)
                {
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.FIRST_EXAM_ROOM_NAME, rdo.Mps000001ADO.firstExamRoomName));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.RATIO_NG, rdo.Mps000001ADO.ratio_text));
                    if (rdo.Mps000001ADO.ExecuteRoom != null)
                    {
                        SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.RESPONSIBLE_LOGINNAME, rdo.Mps000001ADO.ExecuteRoom.RESPONSIBLE_LOGINNAME));
                        SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.RESPONSIBLE_USERNAME, rdo.Mps000001ADO.ExecuteRoom.RESPONSIBLE_USERNAME));
                        SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.EXECUTE_ROOM_ADDRESS, rdo.Mps000001ADO.ExecuteRoom.ADDRESS));
                    }
                }

                SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.LOGIN_USER_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.LOGIN_LOGIN_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()));

                if (rdo.PatyAlterBhyt != null)
                {
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.FROM_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.TO_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.HEIN_CARD_NUMBER_SEPERATOR, GlobalQuery.TrimHeinCardNumber(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER)));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                }

                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);

                if (rdo.ServiceReq != null && !string.IsNullOrEmpty(rdo.ServiceReq.ICD_CODE))
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReq, false);
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentTreatment, false);
                }
                else
                {
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentTreatment, false);
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReq, false);
                }
                if (rdo._HIS_WORK_PLACE != null && rdo._HIS_WORK_PLACE.ID > 0)
                {
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.WP_ADDRESS, rdo._HIS_WORK_PLACE.ADDRESS));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.WP_CONTACT_MOBILE, rdo._HIS_WORK_PLACE.CONTACT_MOBILE));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.WP_CONTACT_NAME, rdo._HIS_WORK_PLACE.CONTACT_NAME));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.WP_DIRECTOR_NAME, rdo._HIS_WORK_PLACE.DIRECTOR_NAME));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.WP_GROUP_CODE, rdo._HIS_WORK_PLACE.GROUP_CODE));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.WP_PHONE, rdo._HIS_WORK_PLACE.PHONE));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.WP_TAX_CODE, rdo._HIS_WORK_PLACE.TAX_CODE));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.WP_WORK_PLACE_CODE, rdo._HIS_WORK_PLACE.WORK_PLACE_CODE));
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.WP_WORK_PLACE_NAME, rdo._HIS_WORK_PLACE.WORK_PLACE_NAME));
                }
                if (rdo._HIS_DHST != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo._HIS_DHST, false);
                }
                AddObjectKeyIntoListkey<Mps000001ADO>(rdo.Mps000001ADO, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.currentPatient, false);

                string isPaid = "0";
                ProcesPaid(ref isPaid);

                SetSingleKey(new KeyValue("IS_PAID", isPaid));

                string payFormCode = "";
                if (rdo.ListTransaction != null && rdo.ListTransaction.Count > 0)
                {
                    List<string> payFormCodes = rdo.ListTransaction.Select(s => s.PAY_FORM_CODE).Distinct().ToList();
                    payFormCode = string.Join(",", payFormCodes);
                }

                SetSingleKey(new KeyValue("PAY_FORM_CODE", payFormCode));
                SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.GATE, rdo.Gate));
                if (rdo.TransReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000001ExtendSingleKey.PAYMENT_AMOUNT, rdo.TransReq.AMOUNT));
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
                if (rdo.sereServs != null && rdo.sereServs.Count > 0 && ((rdo.ListSereServBill != null && rdo.ListSereServBill.Count > 0) || (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)))
                {
                    Dictionary<long, decimal> dicTran = new Dictionary<long, decimal>();

                    if (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)
                    {
                        var sereServDeposit = rdo.ListSereServDeposit.Where(o => rdo.sereServs.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
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
                        var sereServBill = rdo.ListSereServBill.Where(o => rdo.sereServs.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
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
                        foreach (var item in rdo.sereServs)
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

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã điều trị: " + rdo.ServiceReq.TREATMENT_CODE;
                log += " , Mã yêu cầu: " + rdo.ServiceReq.SERVICE_REQ_CODE;
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
                if (rdo != null && rdo.ServiceReq != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.ServiceReq.TREATMENT_CODE;
                    string serviceReqCode = "SERVICE_REQ_CODE:" + rdo.ServiceReq.SERVICE_REQ_CODE;
                    string serviceCode = "";
                    string parentName = "";
                    if (rdo.sereServs != null && rdo.sereServs.Count > 0)
                    {
                        var serviceFirst = rdo.sereServs.OrderBy(o => o.TDL_SERVICE_CODE).First();
                        serviceCode = "SERVICE_CODE:" + serviceFirst.TDL_SERVICE_CODE;
                    }

                    if (rdo.Mps000001ADO != null && !String.IsNullOrWhiteSpace(rdo.Mps000001ADO.PARENT_NAME))
                    {
                        parentName = ProcessParentName(rdo.Mps000001ADO.PARENT_NAME);
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
