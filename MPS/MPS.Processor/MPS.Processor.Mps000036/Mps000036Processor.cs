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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000036.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using Inventec.Common.QRCoder;
using System.IO;
using HIS.Desktop.Common.BankQrCode;

namespace MPS.Processor.Mps000036
{
    public partial class Mps000036Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        private PatyAlterBhytADO PatyAlter { get; set; }

        Mps000036PDO rdo;
        public Mps000036Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000036PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                if (rdo != null)
                {
                    if (rdo.Treatment != null && !String.IsNullOrWhiteSpace(rdo.Treatment.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.Treatment.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000036ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
                    }

                    if (rdo.ServiceReq != null && !String.IsNullOrWhiteSpace(rdo.ServiceReq.SERVICE_REQ_CODE))
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

                        dicImage.Add(Mps000036ExtendSingleKey.BARCODE_SERVICE_REQ_CODE, barcodeServiceReq);
                    }

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

                        dicImage.Add(Mps000036ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
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
                if (rdo.Treatment != null && !String.IsNullOrEmpty(rdo.Treatment.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(Mps000036ExtendSingleKey.IMG_AVATAR, rdo.Treatment.TDL_PATIENT_AVATAR_URL);
                    isBhytAndAvtNull = false;
                }

                if (isBhytAndAvtNull)
                {
                    SetSingleKey(Mps000036ExtendSingleKey.AVT_AND_BHYT_NULL, "1");
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

                DataInputProcess();
                ProcessSingleKey();
                SetBarcodeKey();
                SetQrCode();

                SetImageKey();

                this.SetSignatureKeyImageByCFG();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                if (rdo.SereServs != null && rdo.SereServs.Count > 0)
                {
                    objectTag.AddObjectData(store, "SereServ", rdo.SereServs);
                }
                else if (rdo.sereServs11 != null && rdo.sereServs11.Count > 0)
                {
                    objectTag.AddObjectData(store, "SereServ", rdo.sereServs11);
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        class ReplaceValueFunction : FlexCel.Report.TFlexCelUserFunction
        {
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                try
                {
                    string value = parameters[0] + "";
                    if (!String.IsNullOrEmpty(value))
                    {
                        value = value.Replace(';', '/');
                    }
                    return value;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    return parameters[0];
                }
            }
        }

        void ProcessSingleKey()
        {
            try
            {
                if (rdo.ServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReq.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.ServiceReq.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.STR_YEAR, rdo.ServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.ServiceReq.TDL_PATIENT_DOB)));

                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.FINISH_TIME_STR,
                         Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReq.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.START_TIME_STR,
                         Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReq.START_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.FINISH_TIME_FULL_STR,
                         GlobalQuery.GetCurrentTimeSeparateBeginTime(
                         Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                         rdo.ServiceReq.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.FINISH_DATE_FULL_STR,
                         Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.ServiceReq.FINISH_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                         GlobalQuery.GetCurrentTimeSeparateBeginTime(
                         Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                         rdo.ServiceReq.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                         Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                         rdo.ServiceReq.INTRUCTION_TIME)));

                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.REQ_ICD_CODE, rdo.ServiceReq.ICD_CODE));
                    //SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.REQ_ICD_MAIN_TEXT, rdo.ServiceReq.ICD_MAIN_TEXT));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.REQ_ICD_NAME, rdo.ServiceReq.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.REQ_ICD_SUB_CODE, rdo.ServiceReq.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.REQ_ICD_TEXT, rdo.ServiceReq.ICD_TEXT));

                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.DEPARTMENT_NAME, rdo.ServiceReq.EXECUTE_DEPARTMENT_NAME));

                    if (rdo.Treatment == null) rdo.Treatment = new HIS_TREATMENT();

                    string Address = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_ADDRESS) ?
                        rdo.ServiceReq.TDL_PATIENT_ADDRESS :
                        rdo.Treatment.TDL_PATIENT_ADDRESS;
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.ADDRESS, Address));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.VIR_ADDRESS, Address));

                    string career = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_CAREER_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_CAREER_NAME :
                        rdo.Treatment.TDL_PATIENT_CAREER_NAME;
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.CAREER_NAME, career));

                    string code = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_CODE) ?
                        rdo.ServiceReq.TDL_PATIENT_CODE :
                        rdo.Treatment.TDL_PATIENT_CODE;
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.PATIENT_CODE, code));

                    long dob = rdo.ServiceReq.TDL_PATIENT_DOB > 0 ?
                        rdo.ServiceReq.TDL_PATIENT_DOB :
                        rdo.Treatment.TDL_PATIENT_DOB;
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.DOB, dob > 0 ? dob : 00000000000000));

                    string gender = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_GENDER_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_GENDER_NAME :
                        rdo.Treatment.TDL_PATIENT_GENDER_NAME;
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.GENDER_NAME, gender));

                    string rank = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_MILITARY_RANK_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_MILITARY_RANK_NAME :
                        rdo.Treatment.TDL_PATIENT_MILITARY_RANK_NAME;
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.MILITARY_RANK_NAME, rank));

                    string name = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_NAME :
                        rdo.Treatment.TDL_PATIENT_NAME;
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.VIR_PATIENT_NAME, name));

                    string qg = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_NATIONAL_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_NATIONAL_NAME :
                        rdo.Treatment.TDL_PATIENT_NATIONAL_NAME;
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.NATIONAL_NAME, qg));

                    string work = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_WORK_PLACE) ?
                        rdo.ServiceReq.TDL_PATIENT_WORK_PLACE :
                        rdo.Treatment.TDL_PATIENT_WORK_PLACE;
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WORK_PLACE, work));

                    string work_name = !String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_WORK_PLACE_NAME) ?
                        rdo.ServiceReq.TDL_PATIENT_WORK_PLACE_NAME :
                        rdo.Treatment.TDL_PATIENT_WORK_PLACE_NAME;
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WORK_PLACE_NAME, work_name));

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
                    SetSingleKey(Mps000036ExtendSingleKey.QRCODE_PATIENT, qrCodeImage);

                    SetSingleKey(new KeyValue("IS_EMERGENCY_REQ", rdo.ServiceReq.IS_EMERGENCY));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.REQ_PROVISIONAL_DIAGNOSIS, rdo.ServiceReq.PROVISIONAL_DIAGNOSIS));
                }

                decimal bhytthanhtoan_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (rdo.SereServs != null && rdo.SereServs.Count > 0)
                {
                    bhytthanhtoan_tong = rdo.SereServs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = rdo.SereServs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;

                    rdo.SereServs = rdo.SereServs.OrderBy(o => o.ID).ToList();
                }

                if (PatyAlter != null)
                {
                    if (PatyAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (PatyAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (PatyAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (PatyAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }
                }

                SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));

                if (rdo.SingleKeyValue != null)
                {
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.FIRST_EXAM_ROOM_NAME, rdo.SingleKeyValue.firstExamRoomName));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.RATIO_STR, rdo.SingleKeyValue.RatioText));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.PARENT_NAME, rdo.SingleKeyValue.PARENT_NAME));
                    AddObjectKeyIntoListkey<SingleKeyValue>(rdo.SingleKeyValue, false);
                }

                if (rdo._HIS_WORK_PLACE != null && rdo._HIS_WORK_PLACE.ID > 0)
                {
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WP_ADDRESS, rdo._HIS_WORK_PLACE.ADDRESS));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WP_CONTACT_MOBILE, rdo._HIS_WORK_PLACE.CONTACT_MOBILE));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WP_CONTACT_NAME, rdo._HIS_WORK_PLACE.CONTACT_NAME));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WP_DIRECTOR_NAME, rdo._HIS_WORK_PLACE.DIRECTOR_NAME));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WP_GROUP_CODE, rdo._HIS_WORK_PLACE.GROUP_CODE));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WP_PHONE, rdo._HIS_WORK_PLACE.PHONE));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WP_TAX_CODE, rdo._HIS_WORK_PLACE.TAX_CODE));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WP_WORK_PLACE_CODE, rdo._HIS_WORK_PLACE.WORK_PLACE_CODE));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.WP_WORK_PLACE_NAME, rdo._HIS_WORK_PLACE.WORK_PLACE_NAME));
                }
                if (rdo._HIS_DHST != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo._HIS_DHST, false);
                }

                SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.LOGIN_USER_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.LOGIN_LOGIN_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()));

                if (rdo.SereServs != null && rdo.SereServs.Count == 1)
                {
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.SERVICE_CODE, rdo.SereServs.FirstOrDefault().TDL_SERVICE_CODE));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.SERVICE_NAME, rdo.SereServs.FirstOrDefault().TDL_SERVICE_NAME));
                }
                else if (rdo.sereServs11 != null && rdo.sereServs11.Count == 1)
                {
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.SERVICE_CODE, rdo.sereServs11.FirstOrDefault().TDL_SERVICE_CODE));
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.SERVICE_NAME, rdo.sereServs11.FirstOrDefault().TDL_SERVICE_NAME));
                }

                AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlter, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReq, true);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.Treatment);
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

                SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.PAY_FORM_CODE, payFormCode));
                SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.CARD_CODE, cardCode));
                SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.BANK_CARD_CODE, bankCardCode));
                if (rdo.TransReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000036ExtendSingleKey.PAYMENT_AMOUNT, rdo.TransReq.AMOUNT));
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
                if (rdo.sereServs11 != null && rdo.sereServs11.Count > 0 && ((rdo.ListSereServBill != null && rdo.ListSereServBill.Count > 0) || (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)))
                {
                    Dictionary<long, decimal> dicTran = new Dictionary<long, decimal>();

                    if (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)
                    {
                        var sereServDeposit = rdo.ListSereServDeposit.Where(o => rdo.sereServs11.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
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
                        var sereServBill = rdo.ListSereServBill.Where(o => rdo.sereServs11.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
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
                        foreach (var item in rdo.sereServs11)
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
                    if (rdo.SereServs != null && rdo.SereServs.Count > 0)
                    {
                        var serviceFirst = rdo.SereServs.OrderBy(o => o.TDL_SERVICE_CODE).First();
                        serviceCode = "SERVICE_CODE:" + serviceFirst.TDL_SERVICE_CODE;
                    }

                    if (rdo.SingleKeyValue != null && !String.IsNullOrWhiteSpace(rdo.SingleKeyValue.PARENT_NAME))
                    {
                        parentName = ProcessParentName(rdo.SingleKeyValue.PARENT_NAME);
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
