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
using MPS.Processor.Mps000167.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000167
{
    public class Mps000167Processor : AbstractProcessor
    {
        Mps000167PDO rdo;
        public Mps000167Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000167PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (!String.IsNullOrEmpty(rdo.PATIENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.PATIENT_CODE);
                    barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodePatientCode.IncludeLabel = false;
                    barcodePatientCode.Width = 120;
                    barcodePatientCode.Height = 40;
                    barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodePatientCode.IncludeLabel = true;

                    dicImage.Add(Mps000167ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
                }
                if (!String.IsNullOrEmpty(rdo.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.TREATMENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(Mps000167ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);
                }
                if (!String.IsNullOrEmpty(rdo.SERVICE_REQ_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeServiceReqCode = new Inventec.Common.BarcodeLib.Barcode(rdo.SERVICE_REQ_CODE);
                    barcodeServiceReqCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeServiceReqCode.IncludeLabel = false;
                    barcodeServiceReqCode.Width = 120;
                    barcodeServiceReqCode.Height = 40;
                    barcodeServiceReqCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeServiceReqCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeServiceReqCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeServiceReqCode.IncludeLabel = true;

                    dicImage.Add(Mps000167ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeServiceReqCode);
                }

                if (!String.IsNullOrEmpty(rdo._PaanServiceReq.ASSIGN_TURN_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeAssignTurnBar = new Inventec.Common.BarcodeLib.Barcode(rdo._PaanServiceReq.ASSIGN_TURN_CODE);
                    barcodeAssignTurnBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeAssignTurnBar.IncludeLabel = false;
                    barcodeAssignTurnBar.Width = 120;
                    barcodeAssignTurnBar.Height = 40;
                    barcodeAssignTurnBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeAssignTurnBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeAssignTurnBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeAssignTurnBar.IncludeLabel = true;

                    dicImage.Add(Mps000167ExtendSingleKey.ASSIGN_TURN_BAR, barcodeAssignTurnBar);
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
                //SetBarcodeKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();

                SetBarcodeKey();
                SetSingleKeyQrCode();
                //hàm sinh key chữ ký dựa vào key tài khoản do lớp base cung cấp
                this.SetSignatureKeyImageByCFG();
                if (rdo.ListSereServ == null)
                {
                    rdo.ListSereServ = new List<V_HIS_SERE_SERV>();
                }

                if (rdo.ListSereServ.Count == 0 && rdo._SereServ != null)
                {
                    V_HIS_SERE_SERV ss = new V_HIS_SERE_SERV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(ss, rdo._SereServ);
                    rdo.ListSereServ.Add(ss);
                }

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "SereServ", rdo.ListSereServ);
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
                if (rdo.transReq != null && rdo.lstConfig != null && rdo.lstConfig.Count > 0)
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
        void ProcessSingleKey()
        {
            try
            {
                if (rdo._PaanServiceReq != null)
                {
                    rdo.PATIENT_CODE = rdo._PaanServiceReq.TDL_PATIENT_CODE;
                    rdo.TREATMENT_CODE = rdo._PaanServiceReq.TDL_TREATMENT_CODE;
                    rdo.SERVICE_REQ_CODE = rdo._PaanServiceReq.SERVICE_REQ_CODE;
                    if (rdo._PaanServiceReq.IS_EMERGENCY == (short)1)
                    {
                        SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.IS_SURGERY, "x"));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.IS_NOT_SURGERY, "x"));
                    }
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._PaanServiceReq.TDL_PATIENT_DOB)));

                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.VIR_PATIENT_NAME, rdo._PaanServiceReq.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.GENDER_NAME, rdo._PaanServiceReq.TDL_PATIENT_GENDER_NAME));

                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo._PaanServiceReq.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.LIQUID_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo._PaanServiceReq.LIQUID_TIME ?? 0)));
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo._PaanServiceReq, false);

                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.NATIONAL_NAME, rdo._PaanServiceReq.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.WORK_PLACE, rdo._PaanServiceReq.TDL_PATIENT_WORK_PLACE_NAME));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.ADDRESS, rdo._PaanServiceReq.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.CAREER_NAME, rdo._PaanServiceReq.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.PATIENT_CODE, rdo._PaanServiceReq.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.DISTRICT_CODE, rdo._PaanServiceReq.TDL_PATIENT_DISTRICT_CODE));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.GENDER_NAME, rdo._PaanServiceReq.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.MILITARY_RANK_NAME, rdo._PaanServiceReq.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.VIR_ADDRESS, rdo._PaanServiceReq.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.DOB, rdo._PaanServiceReq.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.VIR_PATIENT_NAME, rdo._PaanServiceReq.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.BED_ROOM_NAME, rdo._PaanServiceReq.REQUEST_ROOM_NAME));
                    //SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.TREATMENT_CODE_BAR, rdo._PaanServiceReq.TDL_TREATMENT_CODE));
                    //SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.SERVICE_REQ_CODE_BAR, rdo._PaanServiceReq.SERVICE_REQ_CODE));
                    //SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.PATIENT_CODE_BAR, rdo._PaanServiceReq.TDL_PATIENT_CODE));

                    List<string> infos = new List<string>();
                    infos.Add(rdo._PaanServiceReq.TDL_PATIENT_CODE);
                    infos.Add(rdo._PaanServiceReq.TDL_PATIENT_NAME);
                    infos.Add(rdo._PaanServiceReq.TDL_PATIENT_GENDER_NAME);
                    infos.Add(rdo._PaanServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    infos.Add(rdo._PaanServiceReq.SERVICE_REQ_CODE);

                    string totalInfo = string.Join("\t", infos);
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(totalInfo, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                    byte[] qrCodeImage = qrCode.GetGraphic(20);
                    SetSingleKey(Mps000167ExtendSingleKey.QRCODE_PATIENT, qrCodeImage);
                }
                if (rdo._SereServ != null)
                {
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.SERVICE_CODE, rdo._SereServ.TDL_SERVICE_CODE));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.SERVICE_NAME, rdo._SereServ.TDL_SERVICE_NAME));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.SERE_SERV_PATIENT_TYPE_NAME, rdo._SereServ.PATIENT_TYPE_NAME));
                    AddObjectKeyIntoListkey<V_HIS_SERE_SERV_5>(rdo._SereServ, false);
                }
                if (rdo._PatyAlterBhyt != null)
                {
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.HEIN_CARD_TO_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatyAlterBhyt, false);
                }
                if (rdo.Mps000167ADO != null)
                {
                    AddObjectKeyIntoListkey<Mps000167ADO>(rdo.Mps000167ADO, false);
                }

                if (rdo._HisTreatment != null)
                {
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo._HisTreatment, false);
                }

                string isPaid = "0";
                ProcesPaid(ref isPaid);

                SetSingleKey(new KeyValue("IS_PAID", isPaid));

                string payFormCode = "";
                if (rdo.ListTransaction != null && rdo.ListTransaction.Count > 0)
                {
                    List<string> payFormCodes = rdo.ListTransaction.Select(s => s.PAY_FORM_CODE).Distinct().ToList();
                    payFormCode = string.Join(",", payFormCodes);
                    var transaction_card_code = rdo.ListTransaction.FirstOrDefault(o => o.TDL_CARD_CODE != null);
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.CARD_CODE, transaction_card_code != null ? transaction_card_code.TDL_CARD_CODE : null));
                    var transaction_bank_card_code = rdo.ListTransaction.FirstOrDefault(o => o.TDL_BANK_CARD_CODE != null);
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.BANK_CARD_CODE, transaction_bank_card_code != null ? transaction_bank_card_code.TDL_BANK_CARD_CODE : null));
                }
                SetSingleKey(new KeyValue("PAY_FORM_CODE", payFormCode));
                if (rdo.transReq != null)
                    SetSingleKey(new KeyValue(Mps000167ExtendSingleKey.PAYMENT_AMOUNT, rdo.transReq.AMOUNT));
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
                if (rdo.ListSereServ != null && rdo.ListSereServ.Count > 0 && ((rdo.ListSereServBill != null && rdo.ListSereServBill.Count > 0) || (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)))
                {
                    Dictionary<long, decimal> dicTran = new Dictionary<long, decimal>();

                    if (rdo.ListSereServDeposit != null && rdo.ListSereServDeposit.Count > 0)
                    {
                        var sereServDeposit = rdo.ListSereServDeposit.Where(o => rdo.ListSereServ.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
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
                        var sereServBill = rdo.ListSereServBill.Where(o => rdo.ListSereServ.Exists(e => e.ID == o.SERE_SERV_ID) && !o.IS_CANCEL.HasValue && o.IS_ACTIVE == 1 && o.IS_DELETE == 0).ToList();
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
                        foreach (var item in rdo.ListSereServ)
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
