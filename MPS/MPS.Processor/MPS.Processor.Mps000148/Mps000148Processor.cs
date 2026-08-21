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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000148.ADO;
using MPS.Processor.Mps000148.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000148
{
    public class Mps000148Processor : AbstractProcessor
    {
        List<Mps000148ADO> serviceAdos = new List<Mps000148ADO>();
        Mps000148PDO rdo;
        public Mps000148Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000148PDO)rdoBase;
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
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                ProcessListData();
                SetBarcodeKey();
                SetQrCodeKey();
                SetServiceTypeAmountKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Services1", serviceAdos);
                objectTag.AddObjectData(store, "Services2", serviceAdos);
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

        void ProcessSingleKey()
        {
            try
            {
                if (rdo._Transaction != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT));
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(amountStr)));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));

                    string amountAwayZeroStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(Math.Round(rdo._Transaction.AMOUNT, 0, MidpointRounding.AwayFromZero)));
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.AMOUNT_AWAY_ZERO_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(amountAwayZeroStr)));

                    decimal canthu = rdo._Transaction.AMOUNT - (rdo._Transaction.KC_AMOUNT ?? 0) - (rdo._Transaction.EXEMPTION ?? 0);
                    if ((rdo._Transaction.TDL_BILL_FUND_AMOUNT ?? 0) > 0)
                    {
                        canthu = canthu - (rdo._Transaction.TDL_BILL_FUND_AMOUNT ?? 0);
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu)));
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.Number.Convert.NumberToStringRoundAuto(canthu, 4)));
                }

                //Key ngày vào viện / ngày vào khám (lấy từ hồ sơ điều trị nếu caller có truyền)
                if (rdo._Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.IN_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Treatment.IN_TIME)));
                    if (rdo._Treatment.CLINICAL_IN_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.CLINICAL_IN_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Treatment.CLINICAL_IN_TIME.Value)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessListData()
        {
            try
            {
                if (rdo._ListSereServBill != null && rdo._ListSereServBill.Count > 0)
                {
                    Mps000148ADO vpAdo = null;
                    Mps000148ADO bhytAdo = null;
                    foreach (var ssBill in rdo._ListSereServBill)
                    {
                        if (ssBill.TDL_AMOUNT.HasValue)
                        {
                            LogSystem.Info("ssbill.TDL_AMOUNT");
                            if (ssBill.TDL_PATIENT_TYPE_ID == rdo._PatientTypeId_Bhyt)
                            {
                                if (bhytAdo == null)
                                {
                                    bhytAdo = new Mps000148ADO();
                                    bhytAdo.SERVICE_NAME = "Chênh lệch BHYT";
                                    bhytAdo.SERVICE_UNIT_NAME = "Lần";
                                    bhytAdo.TOTAL_PRICE = 0;
                                }

                                bhytAdo.TOTAL_PRICE += ssBill.PATIENT_BHYT_PRICE;

                                if ((ssBill.PATIENT_PAY_PRICE ?? 0) > 0)
                                {
                                    if (vpAdo == null)
                                    {
                                        vpAdo = new Mps000148ADO();
                                        vpAdo.SERVICE_NAME = "Bệnh nhân tự túc";
                                        vpAdo.SERVICE_UNIT_NAME = "Lần";
                                        vpAdo.TOTAL_PRICE = 0;
                                    }
                                    vpAdo.TOTAL_PRICE += (ssBill.PATIENT_PAY_PRICE ?? 0);
                                }
                            }
                            else
                            {
                                if (vpAdo == null)
                                {
                                    vpAdo = new Mps000148ADO();
                                    vpAdo.SERVICE_NAME = "Bệnh nhân tự túc";
                                    vpAdo.SERVICE_UNIT_NAME = "Lần";
                                    vpAdo.TOTAL_PRICE = 0;
                                }
                                vpAdo.TOTAL_PRICE += ssBill.PRICE;

                            }
                        }
                        else
                        {
                            var ss = rdo._ListSereServ != null ? rdo._ListSereServ.FirstOrDefault(o => o.ID == ssBill.SERE_SERV_ID) : null;
                            if (ss == null)
                            {
                                continue;
                            }
                            else if (ss.PATIENT_TYPE_ID == rdo._PatientTypeId_Bhyt)
                            {
                                if (bhytAdo == null)
                                {
                                    bhytAdo = new Mps000148ADO();
                                    bhytAdo.SERVICE_NAME = "Chênh lệch BHYT";
                                    bhytAdo.SERVICE_UNIT_NAME = "Lần";
                                    bhytAdo.TOTAL_PRICE = 0;
                                }

                                if (ss.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                                {
                                    bhytAdo.TOTAL_PRICE += ssBill.PRICE;
                                }
                                else
                                {
                                    decimal bhyt_price = (ss.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                                    bhytAdo.TOTAL_PRICE += bhyt_price;

                                    if (ssBill.PRICE > bhyt_price)
                                    {
                                        if (vpAdo == null)
                                        {
                                            vpAdo = new Mps000148ADO();
                                            vpAdo.SERVICE_NAME = "Bệnh nhân tự túc";
                                            vpAdo.SERVICE_UNIT_NAME = "Lần";
                                            vpAdo.TOTAL_PRICE = 0;
                                        }
                                        vpAdo.TOTAL_PRICE += (ssBill.PRICE - bhyt_price);
                                    }
                                }
                            }
                            else
                            {
                                if (vpAdo == null)
                                {
                                    vpAdo = new Mps000148ADO();
                                    vpAdo.SERVICE_NAME = "Bệnh nhân tự túc";
                                    vpAdo.SERVICE_UNIT_NAME = "Lần";
                                    vpAdo.TOTAL_PRICE = 0;
                                }
                                vpAdo.TOTAL_PRICE += ssBill.PRICE;
                            }
                        }
                    }

                    if (vpAdo != null)
                    {
                        vpAdo.AMOUNT = 1;
                        vpAdo.PRICE = vpAdo.TOTAL_PRICE;
                        serviceAdos.Add(vpAdo);
                    }
                    if (bhytAdo != null)
                    {
                        bhytAdo.AMOUNT = 1;
                        bhytAdo.PRICE = bhytAdo.TOTAL_PRICE;
                        serviceAdos.Add(bhytAdo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (!String.IsNullOrEmpty(rdo._Transaction.TRANSACTION_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTransactionCode = new Inventec.Common.BarcodeLib.Barcode(rdo._Transaction.TRANSACTION_CODE);
                    barcodeTransactionCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTransactionCode.IncludeLabel = false;
                    barcodeTransactionCode.Width = 120;
                    barcodeTransactionCode.Height = 40;
                    barcodeTransactionCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTransactionCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTransactionCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTransactionCode.IncludeLabel = true;

                    dicImage.Add(Mps000148ExtendSingleKey.TRANSACTION_CODE_BAR, barcodeTransactionCode);
                }

                //Barcode mã điều trị
                if (rdo._Transaction != null && !String.IsNullOrEmpty(rdo._Transaction.TREATMENT_CODE))
                {
                    dicImage.Add(Mps000148ExtendSingleKey.TREATMENT_CODE_BAR, CreateBarcode(rdo._Transaction.TREATMENT_CODE));
                }

                //Barcode mã bệnh nhân
                if (rdo._Transaction != null && !String.IsNullOrEmpty(rdo._Transaction.TDL_PATIENT_CODE))
                {
                    dicImage.Add(Mps000148ExtendSingleKey.PATIENT_CODE_BAR, CreateBarcode(rdo._Transaction.TDL_PATIENT_CODE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Sinh anh QR ma tra cuu hoa don dien tu.
        /// Goi SAU ProcessSingleKey() (can singleValueDictionary da nap key tu V_HIS_TRANSACTION)
        /// va TRUOC singleTag.ProcessData().
        /// Truong nguon rong thi khong sinh anh -> o tren bieu mau de trong, khong loi.
        /// </summary>
        void SetQrCodeKey()
        {
            try
            {
                //QR chua So bao mat (dung de tra cuu tren cong hoa don dien tu)
                SetQrCodeByKeyBase("INVOICE_LOOKUP_CODE", Mps000148ExtendSingleKey.INVOICE_LOOKUP_CODE_QR);
                //QR chua duong dan tra cuu hoa don dien tu
                SetQrCodeByKeyBase("EINVOICE_URL", Mps000148ExtendSingleKey.EINVOICE_URL_QR);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bieu BIEN LAI (so bien lai) chi thu phan VIEN PHI -> CHI lay _VP.
        /// _BH la phan BHYT chi tra (benh nhan khong nop), _DV thuoc so hoa don (MPS000147).
        /// Kiem chung du lieu that: tong _VP = RECIEPT_AMOUNT = so tien tren phieu bien lai.
        /// </summary>
        static readonly string[] AMOUNT_SUFFIXES = new string[] { "_VP" };

        /// <summary>
        /// Ma loai dich vu (HIS_SERVICE_TYPE.SERVICE_TYPE_CODE) co trong TransactionInfoSDO.
        /// Moi ma tuong ung 3 truong tien: AMOUNT_&lt;MA&gt;_BH / _VP / _DV.
        /// </summary>
        static readonly string[] SERVICE_TYPE_CODES = new string[]
        {
            "AN", "CL", "CN", "GB", "GI", "HA", "KH", "MA",
            "NS", "PH", "PT", "SA", "TH", "TT", "VT", "XN"
        };

        /// <summary>
        /// Sinh key so tien da thu tach theo tung loai dich vu: "Ten loai(so tien); ...".
        /// Nguon chinh: HIS_TRANSACTION.TRANSACTION_INFO (JSON cua MOS.SDO.TransactionInfoSDO).
        /// Nguon du phong (rieng bieu nay - co san danh sach dich vu cua bill): gop _ListSereServBill theo loai dich vu.
        /// Goi SAU ProcessSingleKey() va TRUOC singleTag.ProcessData().
        /// Khong co du lieu -> khong set key, o tren bieu mau de trong, khong loi.
        /// </summary>
        void SetServiceTypeAmountKey()
        {
            try
            {
                //Danh muc loai dich vu: nap 1 lan, tra cuu O(1)
                Dictionary<string, string> nameByCode = new Dictionary<string, string>();
                Dictionary<long, string> nameById = new Dictionary<long, string>();
                var serviceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                if (serviceTypes != null)
                {
                    foreach (var st in serviceTypes)
                    {
                        if (st == null) continue;
                        if (!String.IsNullOrWhiteSpace(st.SERVICE_TYPE_CODE))
                        {
                            string code = st.SERVICE_TYPE_CODE.Trim().ToUpper();
                            if (!nameByCode.ContainsKey(code)) nameByCode.Add(code, st.SERVICE_TYPE_NAME);
                        }
                        if (!nameById.ContainsKey(st.ID)) nameById.Add(st.ID, st.SERVICE_TYPE_NAME);
                    }
                }

                List<string> amountPositives = new List<string>();
                List<string> amountAlls = new List<string>();

                MOS.SDO.TransactionInfoSDO info = null;
                if (rdo._Transaction != null && !String.IsNullOrWhiteSpace(rdo._Transaction.TRANSACTION_INFO))
                {
                    try
                    {
                        info = Newtonsoft.Json.JsonConvert.DeserializeObject<MOS.SDO.TransactionInfoSDO>(rdo._Transaction.TRANSACTION_INFO);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }

                if (info != null)
                {
                    //Nguon chinh: TRANSACTION_INFO
                    Type infoType = typeof(MOS.SDO.TransactionInfoSDO);
                    foreach (string code in SERVICE_TYPE_CODES)
                    {
                        string serviceTypeName = null;
                        if (!nameByCode.TryGetValue(code, out serviceTypeName) || String.IsNullOrWhiteSpace(serviceTypeName))
                            continue;

                        bool hasValue = false;
                        decimal total = GetAmountByServiceTypeCode(infoType, info, code, ref hasValue);
                        if (!hasValue) continue;

                        string display = String.Format("{0}({1})", serviceTypeName, Inventec.Common.Number.Convert.NumberToString(total));
                        amountAlls.Add(display);
                        if (total > 0) amountPositives.Add(display);
                    }
                }
                else
                {
                    //Nguon du phong: gop tien theo loai dich vu tu danh sach dich vu cua bill
                    if (rdo._ListSereServBill == null || rdo._ListSereServBill.Count == 0
                        || rdo._ListSereServ == null || rdo._ListSereServ.Count == 0)
                        return;

                    Dictionary<long, long> serviceTypeIdBySereServId = new Dictionary<long, long>();
                    foreach (var ss in rdo._ListSereServ)
                    {
                        if (ss == null || serviceTypeIdBySereServId.ContainsKey(ss.ID)) continue;
                        serviceTypeIdBySereServId.Add(ss.ID, ss.TDL_SERVICE_TYPE_ID);
                    }

                    Dictionary<long, decimal> amountByServiceTypeId = new Dictionary<long, decimal>();
                    foreach (var bill in rdo._ListSereServBill)
                    {
                        if (bill == null) continue;
                        long serviceTypeId;
                        if (!serviceTypeIdBySereServId.TryGetValue(bill.SERE_SERV_ID, out serviceTypeId)) continue;
                        if (amountByServiceTypeId.ContainsKey(serviceTypeId))
                            amountByServiceTypeId[serviceTypeId] += bill.PRICE;
                        else
                            amountByServiceTypeId.Add(serviceTypeId, bill.PRICE);
                    }

                    foreach (var pair in amountByServiceTypeId)
                    {
                        string serviceTypeName = null;
                        if (!nameById.TryGetValue(pair.Key, out serviceTypeName) || String.IsNullOrWhiteSpace(serviceTypeName))
                            continue;

                        string display = String.Format("{0}({1})", serviceTypeName, Inventec.Common.Number.Convert.NumberToString(pair.Value));
                        amountAlls.Add(display);
                        if (pair.Value > 0) amountPositives.Add(display);
                    }
                }

                SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.SERVICE_TYPE_AMOUNTs, String.Join("; ", amountPositives)));
                SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.SERVICE_TYPE_AMOUNT_ALLs, String.Join("; ", amountAlls)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tong tien 1 loai dich vu = AMOUNT_&lt;MA&gt;_BH + _VP + _DV.
        /// hasValue = true khi it nhat 1 trong 3 truong co gia tri (loai co phat sinh trong giao dich).
        /// </summary>
        decimal GetAmountByServiceTypeCode(Type infoType, MOS.SDO.TransactionInfoSDO info, string code, ref bool hasValue)
        {
            decimal total = 0;
            try
            {
                string[] suffixes = AMOUNT_SUFFIXES;
                foreach (string suffix in suffixes)
                {
                    var pi = infoType.GetProperty("AMOUNT_" + code + suffix);
                    if (pi == null) continue;
                    object value = pi.GetValue(info, null);
                    if (value == null) continue;
                    hasValue = true;
                    total += Convert.ToDecimal(value);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return total;
        }

        Inventec.Common.BarcodeLib.Barcode CreateBarcode(string code)
        {
            Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(code);
            barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
            barcode.Width = 120;
            barcode.Height = 40;
            barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
            barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
            barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
            barcode.IncludeLabel = true;
            return barcode;
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = LogDataTransaction(rdo._Transaction.TREATMENT_CODE, rdo._Transaction.TRANSACTION_CODE, "");
                log += "SoTien: " + rdo._Transaction.AMOUNT;
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
                if (rdo != null && rdo._Transaction != null)
                    result = String.Format("{0}_{1}_{2}_{3}", rdo._Transaction.TREATMENT_CODE, rdo._Transaction.TRANSACTION_CODE, rdo._Transaction.ACCOUNT_BOOK_CODE, printTypeCode);
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
