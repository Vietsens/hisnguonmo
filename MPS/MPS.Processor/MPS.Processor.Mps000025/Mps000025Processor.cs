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
using DevExpress.XtraEditors;
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Common.QRCoder;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000025.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000025
{
    public partial class Mps000025Processor : AbstractProcessor
    {
        Mps000025PDO rdo;
        public Mps000025Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000025PDO)rdoBase;
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
                SetSingleKey();
                SetSingleKeyQrCode();
                SetBarcodeKey();

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "SereServ", rdo.sereServs);
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

        void SetBarcodeKey()
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

                        dicImage.Add(Mps000025ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeServiceReqCode);
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

                        dicImage.Add(Mps000025ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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

                        dicImage.Add(Mps000025ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
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
                    SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.EXECUTE_ROOM_NAME, rdo.ServiceReqPrint.EXECUTE_ROOM_NAME));
                    SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReqPrint.INTRUCTION_TIME)));
                    var tuoi = AgeUtil.CalculateFullAge(rdo.ServiceReqPrint.TDL_PATIENT_DOB);
                    SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.AGE, rdo.ServiceReqPrint.TDL_PATIENT_DOB));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.EXECUTE_ROOM_NAME, ""));
                    SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.INSTRUCTION_TIME_STR, ""));
                }

                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlter, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReqPrint);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.treatment);
                if (rdo.PatyAlter != null)
                {
                    SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.PATIENT_TYPE_NAME, rdo.PatyAlter.PATIENT_TYPE_NAME));
                    SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.HEIN_CARD_NUMBER, rdo.PatyAlter.HEIN_CARD_NUMBER));
                }

                string payFormCode = "";
                string cardCode = "";
                string bankCardCode = "";
                if (rdo.ListTransaction != null && rdo.ListTransaction.Count > 0)
                {
                    List<string> payFormCodes = rdo.ListTransaction.Select(s => s.PAY_FORM_CODE).Distinct().ToList();
                    payFormCode = string.Join(",", payFormCodes);
                    cardCode = string.Join(",", rdo.ListTransaction.Where(o => !string.IsNullOrWhiteSpace(o.TDL_CARD_CODE)).Select(s => s.TDL_CARD_CODE).Distinct());
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("cardCode___", cardCode));
                    bankCardCode = String.Join(",", rdo.ListTransaction.Where(o => !string.IsNullOrWhiteSpace(o.TDL_BANK_CARD_CODE)).Select(s => s.TDL_BANK_CARD_CODE).Distinct());
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("bankCardCode___", bankCardCode));

                }
                SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.PAY_FORM_CODE, payFormCode));
                SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.CARD_CODE, cardCode));
                SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.BANK_CARD_CODE, bankCardCode));

                string isPaid = "0";
                ProcesPaid(ref isPaid);
                SetSingleKey(new KeyValue("IS_PAID", isPaid));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKeyQrCode()
        {
            try
            {
                if (rdo.ListCard != null && rdo.ListCard.Count > 0)
                {
                    string bankCardCode = "";
                    rdo.ListCard = rdo.ListCard.OrderByDescending(o => o.ID).ToList();
                    foreach (var item in rdo.ListCard)
                    {
                        if (item.BANK_CARD_CODE != null && item.BANK_CARD_CODE.Substring(0, 6).Equals(CARD_PVCBANK.FIRST_BANK_CARD_CODE))
                        {
                            bankCardCode = item.BANK_CARD_CODE;
                            break;
                        }
                    }
                    if (!String.IsNullOrEmpty(bankCardCode))
                    {
                        Inventec.Common.BankQrCode.ADO.ConsumerAccountInfo consumerInfo = new Inventec.Common.BankQrCode.ADO.ConsumerAccountInfo();
                        consumerInfo.pointOTMethod = CARD_PVCBANK.POINT_OT_METHOD;
                        consumerInfo.bnbID = CARD_PVCBANK.FIRST_BANK_CARD_CODE;
                        consumerInfo.ConsumerID = bankCardCode;
                        consumerInfo.transferType = CARD_PVCBANK.TRANSFER_TYPE;
                        consumerInfo.ccy = CARD_PVCBANK.CCY;
                        consumerInfo.countryCode = CARD_PVCBANK.COUNTRY_CODE;
                        Inventec.Common.BankQrCode.ADO.BankQrCodeInputADO input = new Inventec.Common.BankQrCode.ADO.BankQrCodeInputADO();
                        input.ConsumerInfo = consumerInfo;
                        Inventec.Common.BankQrCode.BankQrCodeProcessor qrCodeProcessor = new Inventec.Common.BankQrCode.BankQrCodeProcessor(input);
                        Inventec.Common.BankQrCode.ADO.ResultQrCode result = qrCodeProcessor.GetConsumerQrCode(Inventec.Common.BankQrCode.ProvinceType.PVCB);
                        if (result != null && result.Data != null)
                        {
                            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                            QRCodeData data2 = qRCodeGenerator.CreateQrCode(result.Data, QRCodeGenerator.ECCLevel.Q);
                            BitmapByteQRCode bitmapByteQRCode = new BitmapByteQRCode(data2);
                            byte[] graphic = bitmapByteQRCode.GetGraphic(20);
                            SetSingleKey(new KeyValue(Mps000025ExtendSingleKey.DEPOSIT_QR_CODE_PVCB, graphic));
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
    }
}
