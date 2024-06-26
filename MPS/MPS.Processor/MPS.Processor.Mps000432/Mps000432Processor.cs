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
using MPS.Processor.Mps000432.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000432
{
    public class Mps000432Processor : AbstractProcessor
    {
        Mps000432PDO rdo;
        List<SereServAdo> listAdo = new List<SereServAdo>();
        List<ServiceReqAdo> listServiceReqAdo = new List<ServiceReqAdo>();

        public Mps000432Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000432PDO)rdoBase;
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
                SetListData();

                this.SetSignatureKeyImageByCFG();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "SereServ", listAdo);
                objectTag.AddObjectData(store, "ServiceReq", listServiceReqAdo);
                objectTag.AddRelationship(store, "ServiceReq", "SereServ", "ID", "SERVICE_REQ_ID");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.HisTreatment != null)
                {
                    if (!String.IsNullOrEmpty(rdo.HisTreatment.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTreatment.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000432ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);

                        string treatmentCode = rdo.HisTreatment.TREATMENT_CODE.Substring(rdo.HisTreatment.TREATMENT_CODE.Length - 5, 5);
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment5 = new Inventec.Common.BarcodeLib.Barcode(treatmentCode);
                        barcodeTreatment5.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment5.IncludeLabel = false;
                        barcodeTreatment5.Width = 120;
                        barcodeTreatment5.Height = 40;
                        barcodeTreatment5.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment5.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment5.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment5.IncludeLabel = true;

                        dicImage.Add(Mps000432ExtendSingleKey.TREATMENT_CODE_BAR_5, barcodeTreatment5);
                    }

                    if (!String.IsNullOrEmpty(rdo.HisTreatment.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTreatment.TDL_PATIENT_CODE);
                        barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatientCode.IncludeLabel = false;
                        barcodePatientCode.Width = 120;
                        barcodePatientCode.Height = 40;
                        barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatientCode.IncludeLabel = true;

                        dicImage.Add(Mps000432ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
                    }
                }

                if (rdo.ServiceReqPrints != null && rdo.ServiceReqPrints.Count > 0)
                {
                    var req = rdo.ServiceReqPrints.OrderBy(o => o.ID).FirstOrDefault();
                    if (!String.IsNullOrEmpty(req.ASSIGN_TURN_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTurnCode = new Inventec.Common.BarcodeLib.Barcode(req.ASSIGN_TURN_CODE);
                        barcodeTurnCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTurnCode.IncludeLabel = false;
                        barcodeTurnCode.Width = 120;
                        barcodeTurnCode.Height = 40;
                        barcodeTurnCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTurnCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTurnCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTurnCode.IncludeLabel = true;

                        dicImage.Add(Mps000432ExtendSingleKey.ASSIGN_TURN_CODE_BARCODE, barcodeTurnCode);
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
                        listAdo.Add(new SereServAdo(item, rdo.ListService, rdo.ListServiceCondition));
                    }

                    listAdo = listAdo.OrderByDescending(o => o.SERVICE_PARENT_ORDER).ThenByDescending(o => o.SERVICE_ORDER).ThenBy(o => o.ID).ThenBy(p => p.TDL_SERVICE_NAME).ToList();
                }

                if (rdo.ServiceReqPrints != null && rdo.ServiceReqPrints.Count > 0)
                {
                    foreach (var item in rdo.ServiceReqPrints)
                    {
                        var ado = new ServiceReqAdo(item);

                        if (rdo.ReqMaxNumOrderSDO != null && rdo.ReqMaxNumOrderSDO.Count > 0)
                        {
                            var roomSdo = rdo.ReqMaxNumOrderSDO.FirstOrDefault(o => o.EXECUTE_ROOM_ID == item.EXECUTE_ROOM_ID);
                            if (roomSdo != null)
                            {
                                ado.CURRENT_EXECUTE_ROOM_NUM_ORDER = roomSdo.MAX_NUM_ORDER;
                            }
                        }

                        List<Task> taskall = new List<Task>();
                        Task tsServiceReqCode = Task.Factory.StartNew((object obj) =>
                        {
                            ServiceReqAdo data = obj as ServiceReqAdo;
                            data.SERVICE_REQ_CODE_BAR = ProcessBarcode(data.SERVICE_REQ_CODE);
                        }, ado);
                        taskall.Add(tsServiceReqCode);

                        Task tsAssignTurnCodeBarcode = Task.Factory.StartNew((object obj) =>
                        {
                            ServiceReqAdo data = obj as ServiceReqAdo;
                            data.ASSIGN_TURN_CODE_BARCODE = ProcessBarcode(data.ASSIGN_TURN_CODE);
                        }, ado);
                        taskall.Add(tsAssignTurnCodeBarcode);

                        Task tsBarcode = Task.Factory.StartNew((object obj) =>
                        {
                            ServiceReqAdo data = obj as ServiceReqAdo;
                            data.TEST_SERVICE_REQ_BAR = ProcessBarcode(data.BARCODE);
                        }, ado);
                        taskall.Add(tsBarcode);

                        Task tsProvisional = Task.Factory.StartNew((object obj) =>
                        {
                            ServiceReqAdo data = obj as ServiceReqAdo;
                            List<string> infos = new List<string>();
                            infos.Add(rdo.HisTreatment.TDL_PATIENT_CODE);
                            infos.Add(rdo.HisTreatment.TDL_PATIENT_NAME);
                            infos.Add(rdo.HisTreatment.TDL_PATIENT_GENDER_NAME);
                            infos.Add(rdo.HisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                            infos.Add(data.SERVICE_REQ_CODE);

                            string totalInfo = string.Join("\t", infos);
                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(totalInfo, QRCodeGenerator.ECCLevel.Q);
                            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);

                            data.QRCODE_PATIENT = qrCode.GetGraphic(20);
                        }, ado);
                        taskall.Add(tsProvisional);

                        Task.WaitAll(taskall.ToArray());

                        listServiceReqAdo.Add(ado);
                    }

                    listServiceReqAdo = listServiceReqAdo.OrderBy(o => o.EXECUTE_ROOM_CODE).ThenBy(o => o.SERVICE_REQ_CODE).ToList();
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
                if (!String.IsNullOrWhiteSpace(data))
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
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.HisTreatment == null) rdo.HisTreatment = new HIS_TREATMENT();

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
                            SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (rdo.Mps000432ADO != null && rdo.PatyAlterBhyt != null && rdo.Mps000432ADO.PatientTypeId__Bhyt == rdo.PatyAlterBhyt.PATIENT_TYPE_ID)
                    {
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)))));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)))));
                        SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.HEIN_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000432ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));

                if (rdo.Mps000432ADO != null)
                {
                    AddObjectKeyIntoListkey<Mps000432ADO>(rdo.Mps000432ADO, false);
                }

                if (rdo.WorkPlace != null && rdo.WorkPlace.ID > 0)
                {
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.WP_ADDRESS, rdo.WorkPlace.ADDRESS));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.WP_CONTACT_MOBILE, rdo.WorkPlace.CONTACT_MOBILE));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.WP_CONTACT_NAME, rdo.WorkPlace.CONTACT_NAME));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.WP_DIRECTOR_NAME, rdo.WorkPlace.DIRECTOR_NAME));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.WP_GROUP_CODE, rdo.WorkPlace.GROUP_CODE));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.WP_PHONE, rdo.WorkPlace.PHONE));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.WP_TAX_CODE, rdo.WorkPlace.TAX_CODE));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.WP_WORK_PLACE_CODE, rdo.WorkPlace.WORK_PLACE_CODE));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.WP_WORK_PLACE_NAME, rdo.WorkPlace.WORK_PLACE_NAME));
                }

                if (rdo.Dhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.Dhst, false);
                }

                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);
                if (rdo.ServiceReqPrints != null && rdo.ServiceReqPrints.Count > 0)
                {
                    var req = rdo.ServiceReqPrints.OrderBy(o => o.ID).FirstOrDefault();

                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.REQ_ICD_CODE, req.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.REQ_ICD_NAME, req.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.REQ_ICD_SUB_CODE, req.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.REQ_ICD_TEXT, req.ICD_TEXT));

                    AddObjectKeyIntoListkey(req, true);
                }

                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.HisTreatment, true);

                if (rdo.BedLog != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_BED_LOG>(rdo.BedLog, false);
                }

                string isPaid = "0";
                ProcesPaid(ref isPaid);

                SetSingleKey(new KeyValue("IS_PAID", isPaid));

                string payFormCode = "";
                if (rdo.ListTransaction != null && rdo.ListTransaction.Count > 0)
                {
                    List<string> payFormCodes = rdo.ListTransaction.Select(s => s.PAY_FORM_CODE).Distinct().ToList();
                    payFormCode = string.Join(",", payFormCodes);
                }
                if (rdo.ListTransaction.Count > 0)
                {
                    var transaction_card_code = rdo.ListTransaction.FirstOrDefault(o => o.TDL_CARD_CODE != null);
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.CARD_CODE, transaction_card_code != null ? transaction_card_code.TDL_CARD_CODE : null));
                    var transaction_bank_card_code = rdo.ListTransaction.FirstOrDefault(o => o.TDL_BANK_CARD_CODE != null);
                    SetSingleKey(new KeyValue(Mps000432ExtendSingleKey.BANK_CARD_CODE, transaction_bank_card_code != null ? transaction_bank_card_code.TDL_BANK_CARD_CODE : null));
                }

                SetSingleKey(new KeyValue("PAY_FORM_CODE", payFormCode));
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

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã điều trị: " + rdo.HisTreatment.TREATMENT_CODE;
                log += " , Mã yêu cầu: " + string.Join(",", rdo.ServiceReqPrints.Select(s => s.SERVICE_REQ_CODE).Distinct());
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
                if (rdo != null && rdo.HisTreatment != null && rdo.ServiceReqPrints != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.HisTreatment.TREATMENT_CODE;
                    List<string> serviceReqCode = new List<string>();
                    foreach (var item in rdo.ServiceReqPrints)
                    {
                        serviceReqCode.Add("SERVICE_REQ_CODE:" + item.SERVICE_REQ_CODE);
                    }

                    result = String.Format("{0} {1} {2}", printTypeCode, treatmentCode, string.Join(" ", serviceReqCode));
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                return result;
            }
        }
    }
}
