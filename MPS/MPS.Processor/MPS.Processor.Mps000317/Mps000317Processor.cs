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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MPS.Processor.Mps000317.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000317
{
    public class Mps000317Processor : AbstractProcessor
    {
        Mps000317PDO rdo;
        public Mps000317Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000317PDO)rdoBase;
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
                SetBarcodeKey();
                SetQrCodeKey();
                SetServiceTypeAmountKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
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
                    SetSingleKey(new KeyValue(Mps000317ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(amountStr)));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000317ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    SetSingleKey(new KeyValue(Mps000317ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000317ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));

                    string amountAwayZeroStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(Math.Round(rdo._Transaction.AMOUNT, 0, MidpointRounding.AwayFromZero)));
                    SetSingleKey(new KeyValue(Mps000317ExtendSingleKey.AMOUNT_AWAY_ZERO_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(amountAwayZeroStr)));

                    decimal canthu = rdo._Transaction.AMOUNT - (rdo._Transaction.KC_AMOUNT ?? 0) - (rdo._Transaction.EXEMPTION ?? 0);
                    if ((rdo._Transaction.TDL_BILL_FUND_AMOUNT ?? 0) > 0)
                    {
                        canthu = canthu - (rdo._Transaction.TDL_BILL_FUND_AMOUNT ?? 0);
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                    SetSingleKey(new KeyValue(Mps000317ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu)));
                    SetSingleKey(new KeyValue(Mps000317ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.Number.Convert.NumberToStringRoundAuto(canthu, 4)));
                    if (!String.IsNullOrWhiteSpace(rdo._Transaction.TRANSACTION_INFO))
                    {
                        try
                        {
                            TransactionInfoSDO sdo = Newtonsoft.Json.JsonConvert.DeserializeObject<TransactionInfoSDO>(rdo._Transaction.TRANSACTION_INFO);
                            if (sdo != null)
                            {
                                AddObjectKeyIntoListkey<TransactionInfoSDO>(sdo, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
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

                    dicImage.Add(Mps000317ExtendSingleKey.TRANSACTION_CODE_BAR, barcodeTransactionCode);
                }

                //Barcode mã điều trị
                if (rdo._Transaction != null && !String.IsNullOrEmpty(rdo._Transaction.TREATMENT_CODE))
                {
                    dicImage.Add(Mps000317ExtendSingleKey.TREATMENT_CODE_BAR, CreateBarcode(rdo._Transaction.TREATMENT_CODE));
                }

                //Barcode mã bệnh nhân
                if (rdo._Transaction != null && !String.IsNullOrEmpty(rdo._Transaction.TDL_PATIENT_CODE))
                {
                    dicImage.Add(Mps000317ExtendSingleKey.PATIENT_CODE_BAR, CreateBarcode(rdo._Transaction.TDL_PATIENT_CODE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                SetQrCodeByKeyBase("INVOICE_LOOKUP_CODE", Mps000317ExtendSingleKey.INVOICE_LOOKUP_CODE_QR);
                //QR chua duong dan tra cuu hoa don dien tu
                SetQrCodeByKeyBase("EINVOICE_URL", Mps000317ExtendSingleKey.EINVOICE_URL_QR);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
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
        /// Bieu BIEN LAI (so bien lai, kieu HCM-115/QBH-CUBA) chi thu phan VIEN PHI -> CHI lay _VP.
        /// _BH la phan BHYT chi tra (benh nhan khong nop) nen khong tinh vao phieu thu.
        /// </summary>
        static readonly string[] AMOUNT_SUFFIXES = new string[] { "_VP" };

        /// <summary>
        /// Sinh key so tien da thu tach theo tung loai dich vu: "Ten loai(so tien); ...".
        /// Nguon: HIS_TRANSACTION.TRANSACTION_INFO (JSON cua MOS.SDO.TransactionInfoSDO) - backend ghi san khi tao giao dich.
        /// Goi SAU ProcessSingleKey() va TRUOC singleTag.ProcessData().
        /// TRANSACTION_INFO rong / JSON hong -> khong set key, o tren bieu mau de trong, khong loi.
        /// </summary>
        void SetServiceTypeAmountKey()
        {
            try
            {
                if (rdo._Transaction == null || String.IsNullOrWhiteSpace(rdo._Transaction.TRANSACTION_INFO))
                    return;

                MOS.SDO.TransactionInfoSDO info = null;
                try
                {
                    info = Newtonsoft.Json.JsonConvert.DeserializeObject<MOS.SDO.TransactionInfoSDO>(rdo._Transaction.TRANSACTION_INFO);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                if (info == null) return;

                //Danh muc loai dich vu: nap 1 lan, tra cuu O(1) theo SERVICE_TYPE_CODE
                Dictionary<string, string> serviceTypeNameByCode = new Dictionary<string, string>();
                var serviceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                if (serviceTypes != null)
                {
                    foreach (var st in serviceTypes)
                    {
                        if (st == null || String.IsNullOrWhiteSpace(st.SERVICE_TYPE_CODE)) continue;
                        string code = st.SERVICE_TYPE_CODE.Trim().ToUpper();
                        if (!serviceTypeNameByCode.ContainsKey(code))
                            serviceTypeNameByCode.Add(code, st.SERVICE_TYPE_NAME);
                    }
                }

                List<string> amountPositives = new List<string>();
                List<string> amountAlls = new List<string>();
                Type infoType = typeof(MOS.SDO.TransactionInfoSDO);

                foreach (string code in SERVICE_TYPE_CODES)
                {
                    string serviceTypeName = null;
                    if (!serviceTypeNameByCode.TryGetValue(code, out serviceTypeName) || String.IsNullOrWhiteSpace(serviceTypeName))
                        continue;   //ma khong co trong danh muc -> bo qua

                    bool hasValue = false;
                    decimal total = GetAmountByServiceTypeCode(infoType, info, code, ref hasValue);
                    if (!hasValue) continue;    //loai khong phat sinh trong giao dich

                    string display = String.Format("{0}({1})", serviceTypeName, Inventec.Common.Number.Convert.NumberToString(total));
                    amountAlls.Add(display);
                    if (total > 0) amountPositives.Add(display);
                }

                SetSingleKey(new KeyValue(Mps000317ExtendSingleKey.SERVICE_TYPE_AMOUNTs, String.Join("; ", amountPositives)));
                SetSingleKey(new KeyValue(Mps000317ExtendSingleKey.SERVICE_TYPE_AMOUNT_ALLs, String.Join("; ", amountAlls)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tong tien 1 loai dich vu theo cac hau to trong AMOUNT_SUFFIXES.
        /// hasValue = true khi truong tuong ung co gia tri (loai co phat sinh trong giao dich).
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
