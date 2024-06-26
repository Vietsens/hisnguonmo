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
using HIS.Desktop.Common.BankQrCode;
using Inventec.Common.QRCoder;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000071.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000071
{
    class Mps000071Processor : AbstractProcessor
    {
        Mps000071PDO rdo;

        public Mps000071Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000071PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                //Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                //barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodePatientCode.IncludeLabel = false;
                //barcodePatientCode.Width = 120;
                //barcodePatientCode.Height = 40;
                //barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodePatientCode.IncludeLabel = true;

                //dicImage.Add(Mps000071ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                if (rdo.vHisServiceReq != null)
                {
                    if (!String.IsNullOrEmpty(rdo.vHisServiceReq.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisServiceReq.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000071ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
                    }

                    if (!String.IsNullOrEmpty(rdo.vHisServiceReq.SERVICE_REQ_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeServiceReqCode = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisServiceReq.SERVICE_REQ_CODE);
                        barcodeServiceReqCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeServiceReqCode.IncludeLabel = false;
                        barcodeServiceReqCode.Width = 120;
                        barcodeServiceReqCode.Height = 40;
                        barcodeServiceReqCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeServiceReqCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeServiceReqCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeServiceReqCode.IncludeLabel = true;

                        dicImage.Add(Mps000071ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeServiceReqCode);
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
                SetBarcodeKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                SetQrCode();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "SereServExamServiceReqs", rdo.sereServExamServiceReqs);
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
                if (rdo.V_HIS_PATIENT_TYPE_ALTER != null)
                {

                    if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (!String.IsNullOrEmpty(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, SetHeinCardNumberDisplayByNumber(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_FROM_TIME ?? 0)))));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_TO_TIME ?? 0)))));
                        SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.HEIN_ADDRESS, rdo.V_HIS_PATIENT_TYPE_ALTER.ADDRESS));
                        SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.V_HIS_PATIENT_TYPE_ALTER.ADDRESS));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.IS_NOT_HEIN, "X"));
                        SetSingleKey((new KeyValue(Mps000071ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }
                if (rdo.vHisServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.FINISH_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.vHisServiceReq.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.START_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.vHisServiceReq.START_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.vHisServiceReq.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.vHisServiceReq.FINISH_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.vHisServiceReq.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        rdo.vHisServiceReq.INTRUCTION_TIME)));

                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.REQ_ICD_CODE, rdo.vHisServiceReq.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.REQ_ICD_NAME, rdo.vHisServiceReq.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.REQ_ICD_SUB_CODE, rdo.vHisServiceReq.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.REQ_ICD_TEXT, rdo.vHisServiceReq.ICD_TEXT));

                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.DEPARTMENT_NAME, rdo.vHisServiceReq.EXECUTE_DEPARTMENT_NAME));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.NATIONAL_NAME, rdo.vHisServiceReq.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.WORK_PLACE, rdo.vHisServiceReq.TDL_PATIENT_WORK_PLACE_NAME));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.ADDRESS, rdo.vHisServiceReq.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.CAREER_NAME, rdo.vHisServiceReq.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.PATIENT_CODE, rdo.vHisServiceReq.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.DISTRICT_CODE, rdo.vHisServiceReq.TDL_PATIENT_DISTRICT_CODE));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.GENDER_NAME, rdo.vHisServiceReq.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.MILITARY_RANK_NAME, rdo.vHisServiceReq.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.VIR_ADDRESS, rdo.vHisServiceReq.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.DOB, rdo.vHisServiceReq.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.VIR_PATIENT_NAME, rdo.vHisServiceReq.TDL_PATIENT_NAME));

                    List<string> infos = new List<string>();
                    infos.Add(rdo.vHisServiceReq.TDL_PATIENT_CODE);
                    infos.Add(rdo.vHisServiceReq.TDL_PATIENT_NAME);
                    infos.Add(rdo.vHisServiceReq.TDL_PATIENT_GENDER_NAME);
                    infos.Add(rdo.vHisServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    infos.Add(rdo.vHisServiceReq.SERVICE_REQ_CODE);

                    string totalInfo = string.Join("\t", infos);
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(totalInfo, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                    byte[] qrCodeImage = qrCode.GetGraphic(20);
                    SetSingleKey(Mps000071ExtendSingleKey.QRCODE_PATIENT, qrCodeImage);

                    SetSingleKey(new KeyValue("IS_EMERGENCY_REQ", rdo.vHisServiceReq.IS_EMERGENCY));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.REQ_PROVISIONAL_DIAGNOSIS, rdo.vHisServiceReq.PROVISIONAL_DIAGNOSIS));
                }

                if (rdo.serviceReqPrevios != null)
                {
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.PREV_REQ_ICD_CODE, rdo.serviceReqPrevios.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.PREV_REQ_ICD_NAME, rdo.serviceReqPrevios.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.PREV_REQ_ICD_SUB_CODE, rdo.serviceReqPrevios.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.PREV_REQ_ICD_TEXT, rdo.serviceReqPrevios.ICD_TEXT));
                }

                SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.FIRST_EXAM_ROOM_NAME, rdo.firstExamRoomName));

                SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.RATIO, rdo.ratio));
                SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.RATIO_STR, (rdo.ratio * 100) + "%"));

                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.V_HIS_PATIENT_TYPE_ALTER, false);

                if (rdo.sereServExamSerivceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(rdo.sereServExamSerivceReq, false);
                }
                if (rdo.patientADO != null)
                {
                    AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);
                }

                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.vHisServiceReq, true);

                string isPaid = "0";
                ProcesPaid(ref isPaid);

                SetSingleKey(new KeyValue("IS_PAID", isPaid));

                string payFormCode = "";
                if (rdo.ListTransaction != null && rdo.ListTransaction.Count > 0)
                {
                    List<string> payFormCodes = rdo.ListTransaction.Select(s => s.PAY_FORM_CODE).Distinct().ToList();
                    payFormCode = string.Join(",", payFormCodes);
                    var transaction_card_code = rdo.ListTransaction.FirstOrDefault(o => o.TDL_CARD_CODE != null);
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.CARD_CODE, transaction_card_code != null ? transaction_card_code.TDL_CARD_CODE : null));
                    var transaction_bank_card_code = rdo.ListTransaction.FirstOrDefault(o => o.TDL_BANK_CARD_CODE != null);
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.BANK_CARD_CODE, transaction_bank_card_code != null ? transaction_bank_card_code.TDL_BANK_CARD_CODE : null));
                }
                SetSingleKey(new KeyValue("PAY_FORM_CODE", payFormCode));

                if (rdo.HisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.HisDhst, false);
                }
                if (rdo.TransReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000071ExtendSingleKey.PAYMENT_AMOUNT, rdo.TransReq.AMOUNT));
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
                if (rdo.sereServExamServiceReqs != null && rdo.sereServExamServiceReqs.Count > 0 && ((rdo.ListSereServBill != null && rdo.ListSereServBill.Count > 0) || (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)))
                {
                    Dictionary<long, decimal> dicTran = new Dictionary<long, decimal>();

                    if (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)
                    {
                        var sereServDeposit = rdo.ListSereServDeposit.Where(o => rdo.sereServExamServiceReqs.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
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
                        var sereServBill = rdo.ListSereServBill.Where(o => rdo.sereServExamServiceReqs.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
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
                        foreach (var item in rdo.sereServExamServiceReqs)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        public override string ProcessPrintLogData()
        {
            string mess = LogDataExpMest("", "", "");
            return mess;
        }
    }
}
