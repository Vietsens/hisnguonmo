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
using MPS.Processor.Mps000363.ADO;
using MPS.Processor.Mps000363.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Common.BankQrCode;

namespace MPS.Processor.Mps000363
{
    public class Mps000363Processor : AbstractProcessor
    {
        Mps000363PDO rdo;
        List<SereServADO> ListSereServADOs = new List<SereServADO>();
        byte[] bPatientQr;
        byte[] bPatientNameQr;
        byte[] bStudyDescriptionQr;
        byte[] qrCT540;

        public Mps000363Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000363PDO)rdoBase;
        }

        private void SetBarcodeKey()
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

                        dicImage.Add(Mps000363ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);
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

                        dicImage.Add(Mps000363ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeServiceReq);
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

                        dicImage.Add(Mps000363ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetQRcodeSereServKey()
        {
            try
            {
                if (rdo.sereServADOs != null)
                {
                    var data = from r in rdo.sereServADOs select new SereServADO(r, rdo);
                    foreach (var item in data)
                    {
                        item.AccessNo = ProcessBarcode(item.ID + "");
                        ListSereServADOs.Add(item);
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
                SetQRcodeSereServKey();
                SetConfigPaymentQrCode();

                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "SereServ", ListSereServADOs);

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

        private void SetConfigPaymentQrCode()
        {
            try
            {
                if (rdo.TransReq != null && rdo.Configs != null && rdo.Configs.Count() > 0)
                {
                    var dataQr = QrCodeProcessor.CreateQrImage(rdo.TransReq, rdo.Configs);
                    if (dataQr != null && dataQr.Count() > 0)
                    {
                        foreach (var item in dataQr)
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

        private void ProcessSingleKey()
        {
            try
            {
                if (rdo.ServiceReqPrint != null)
                {
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.ServiceReqPrint.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.STR_YEAR, rdo.ServiceReqPrint.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.ServiceReqPrint.TDL_PATIENT_DOB)));

                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.FINISH_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.START_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.START_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.ServiceReqPrint.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.ServiceReqPrint.FINISH_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.ServiceReqPrint.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        rdo.ServiceReqPrint.INTRUCTION_TIME)));

                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.REQ_ICD_CODE, rdo.ServiceReqPrint.ICD_CODE));
                    //SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.REQ_ICD_MAIN_TEXT, rdo.ServiceReqPrint.ICD_MAIN_TEXT));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.REQ_ICD_NAME, rdo.ServiceReqPrint.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.REQ_ICD_SUB_CODE, rdo.ServiceReqPrint.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.REQ_ICD_TEXT, rdo.ServiceReqPrint.ICD_TEXT));

                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.DEPARTMENT_NAME, rdo.ServiceReqPrint.EXECUTE_DEPARTMENT_NAME));

                    if (rdo.currentHisTreatment == null) rdo.currentHisTreatment = new HIS_TREATMENT();

                    string Address = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_ADDRESS) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_ADDRESS :
                        rdo.currentHisTreatment.TDL_PATIENT_ADDRESS;
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.ADDRESS, Address));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.VIR_ADDRESS, Address));

                    string career = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_CAREER_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_CAREER_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_CAREER_NAME;
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.CAREER_NAME, career));

                    string code = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_CODE) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_CODE :
                        rdo.currentHisTreatment.TDL_PATIENT_CODE;
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.PATIENT_CODE, code));

                    long dob = rdo.ServiceReqPrint.TDL_PATIENT_DOB > 0 ?
                        rdo.ServiceReqPrint.TDL_PATIENT_DOB :
                        rdo.currentHisTreatment.TDL_PATIENT_DOB;
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.DOB, dob > 0 ? dob : 00000000000000));

                    string gender = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_GENDER_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_GENDER_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_GENDER_NAME;
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.GENDER_NAME, gender));

                    string rank = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_MILITARY_RANK_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_MILITARY_RANK_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_MILITARY_RANK_NAME;
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.MILITARY_RANK_NAME, rank));

                    string name = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_NAME;
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.VIR_PATIENT_NAME, name));

                    string qg = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_NATIONAL_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_NATIONAL_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_NATIONAL_NAME;
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.NATIONAL_NAME, qg));

                    string work = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE :
                        rdo.currentHisTreatment.TDL_PATIENT_WORK_PLACE;
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WORK_PLACE, work));

                    string work_name = !String.IsNullOrEmpty(rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE_NAME) ?
                        rdo.ServiceReqPrint.TDL_PATIENT_WORK_PLACE_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_WORK_PLACE_NAME;
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WORK_PLACE_NAME, work_name));

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
                    SetSingleKey(Mps000363ExtendSingleKey.QRCODE_PATIENT, qrCodeImage);

                    SetSingleKey(new KeyValue("IS_EMERGENCY_REQ", rdo.ServiceReqPrint.IS_EMERGENCY));
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
                            SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (rdo.Mps000363ADO != null && rdo.Mps000363ADO.PatientTypeId__Bhyt == rdo.PatyAlterBhyt.PATIENT_TYPE_ID)
                    {
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, SetHeinCardNumberDisplayByNumber(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.IS_NOT_HEIN, ""));
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)))));
                        SetSingleKey((new KeyValue(Mps000363ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)))));
                        SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.HEIN_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.IS_HEIN, ""));
                        SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.IS_VIENPHI, "X"));
                        SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.IS_NOT_HEIN, "X"));
                    }
                }

                SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));

                if (rdo.Mps000363ADO != null)
                {
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.RATIO, rdo.Mps000363ADO.ratio));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.RATIO_STR, (rdo.Mps000363ADO.ratio * 100) + "%"));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.FIRST_EXAM_ROOM_NAME, rdo.Mps000363ADO.firstExamRoomName));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.BED_ROOM_NAME, rdo.Mps000363ADO.bebRoomName));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.PARENT_NAME, rdo.Mps000363ADO.PARENT_NAME));

                    AddObjectKeyIntoListkey<Mps000363ADO>(rdo.Mps000363ADO, false);
                }

                if (rdo._HIS_WORK_PLACE != null && rdo._HIS_WORK_PLACE.ID > 0)
                {
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WP_ADDRESS, rdo._HIS_WORK_PLACE.ADDRESS));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WP_CONTACT_MOBILE, rdo._HIS_WORK_PLACE.CONTACT_MOBILE));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WP_CONTACT_NAME, rdo._HIS_WORK_PLACE.CONTACT_NAME));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WP_DIRECTOR_NAME, rdo._HIS_WORK_PLACE.DIRECTOR_NAME));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WP_GROUP_CODE, rdo._HIS_WORK_PLACE.GROUP_CODE));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WP_PHONE, rdo._HIS_WORK_PLACE.PHONE));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WP_TAX_CODE, rdo._HIS_WORK_PLACE.TAX_CODE));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WP_WORK_PLACE_CODE, rdo._HIS_WORK_PLACE.WORK_PLACE_CODE));
                    SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.WP_WORK_PLACE_NAME, rdo._HIS_WORK_PLACE.WORK_PLACE_NAME));
                }
                if (rdo._HIS_DHST != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo._HIS_DHST, false);
                }

                SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.LOGIN_USER_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.LOGIN_LOGIN_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()));
                string paymentAmount = "";
                if (rdo.TransReq != null)
                {
                    paymentAmount = rdo.TransReq.AMOUNT.ToString();
                }
                SetSingleKey(new KeyValue(Mps000363ExtendSingleKey.PAYMENT_AMOUNT, paymentAmount));
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReqPrint, true);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentHisTreatment, true);
                AddObjectKeyIntoListkey<V_HIS_BED_LOG>(rdo.BedLog, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
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

                    if (rdo.Mps000363ADO != null && !String.IsNullOrWhiteSpace(rdo.Mps000363ADO.PARENT_NAME))
                    {
                        parentName = ProcessParentName(rdo.Mps000363ADO.PARENT_NAME);
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

        private byte[] ProcessBarcode(string data)
        {
            byte[] result = null;
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(data);
                barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcode.IncludeLabel = false;
                barcode.Width = 220;
                barcode.Height = 40;
                barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcode.IncludeLabel = true;
                result = Inventec.Common.FlexCellExport.Common.ConverterImageToArray(barcode.Encode(barcode.EncodedType, barcode.RawData));
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
