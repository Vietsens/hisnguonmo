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
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000113.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000113
{
    public class Mps000113Processor : AbstractProcessor
    {
        Mps000113PDO rdo;
        public Mps000113Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000113PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                SetSingleKey();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
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

        private void SetSingleKey()
        {
            try
            {
                if (rdo._Transaction != null)
                {
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    string temp = rdo._Transaction.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.AMOUNT_NUM, rdo._Transaction.AMOUNT));
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToString(rdo._Transaction.AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT));
                    string amountText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo._Transaction.AMOUNT, 4);
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.AMOUNT_TEXT, amountStr));
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(amountStr)));
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Transaction.CREATE_TIME ?? 0)));

                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);
                }

                if (rdo._Patient != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo._Patient, false);
                }
                if (rdo._PatyAlterBHYT != null)
                {
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.HEIN_CARD_ADDRESS, rdo._PatyAlterBHYT.ADDRESS));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatyAlterBHYT, false);
                }
                if (rdo._DepartmentTran != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo._DepartmentTran, false);
                }
                SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.RATIO, rdo.ratio));
                SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.RATIO_STR, (rdo.ratio * 100) + "%"));

                if (rdo.Treatment != null)
                {
                    V_HIS_TRANSACTION billTransaction = new V_HIS_TRANSACTION();
                    decimal directlyBillingPrice = 0;
                    string totalDepositNumOrder = "";
                    if (rdo.ListBill != null && rdo.ListBill.Count > 0)
                    {
                        billTransaction = rdo.ListBill.Where(o => !o.SALE_TYPE_ID.HasValue && o.IS_DIRECTLY_BILLING != 1
                                                            && o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && o.IS_CANCEL != 1).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault(o => o.TRANSACTION_TIME >= (rdo.Treatment.OUT_TIME ?? 0));
                        if (billTransaction == null) billTransaction = rdo.ListBill.Where(o=> o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && o.IS_CANCEL != 1).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault() ?? new V_HIS_TRANSACTION();

                        var listDirectlyBilling = rdo.ListBill.Where(o => o.IS_DIRECTLY_BILLING == 1
                                                                    && o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && o.IS_CANCEL != 1).ToList();
                        directlyBillingPrice = listDirectlyBilling.Sum(s => s.AMOUNT);

                        var listBill_TamUng = rdo.ListBill.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU && o.IS_CANCEL != 1).ToList() ?? new List<V_HIS_TRANSACTION>();
                        var listDepositNumOrder = listBill_TamUng.OrderBy(o => o.NUM_ORDER).Select(o => o.NUM_ORDER).ToList() ?? new List<long>();
                        totalDepositNumOrder = String.Join(", ", listDepositNumOrder);
                    }

                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.TOTAL_DIRECTLY_BILL_PRICE, Inventec.Common.Number.Convert.NumberToString(directlyBillingPrice, ConfigApplications.NumberSeperator)));
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.BILL_NUM_ORDER, billTransaction.NUM_ORDER));
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.BILL_ACCOUNT_BOOK_CODE, billTransaction.ACCOUNT_BOOK_CODE));
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.BILL_ACCOUNT_BOOK_NAME, billTransaction.ACCOUNT_BOOK_NAME));
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.BILL_TEMPLATE_CODE, billTransaction.TEMPLATE_CODE));
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.BILL_SYMBOL_CODE, billTransaction.SYMBOL_CODE));
                    SetSingleKey(new KeyValue(Mps000113ExtendSingleKey.TOTAL_DEPOSIT_NUM_ORDER, totalDepositNumOrder));
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

                    dicImage.Add(Mps000113ExtendSingleKey.TRANSACTION_CODE_BAR, barcodeTransactionCode);
                }
                if (!String.IsNullOrEmpty(rdo._Transaction.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo._Transaction.TREATMENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(Mps000113ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                    result = String.Format("{0}_{1}_{2}_{3}", this.printTypeCode, rdo._Transaction.TREATMENT_CODE, rdo._Transaction.TRANSACTION_CODE, rdo._Transaction.ACCOUNT_BOOK_CODE);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }

    public static class AgeUtil
    {
        public static string CalculateFullAge(long ageNumber)
        {
            string tuoi;
            string cboAge;
            try
            {
                DateTime dtNgSinh = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                TimeSpan diff = DateTime.Now - dtNgSinh;
                long tongsogiay = diff.Ticks;
                if (tongsogiay < 0)
                {
                    tuoi = "";
                    cboAge = "Tuổi";
                    return "";
                }
                DateTime newDate = new DateTime(tongsogiay);

                int nam = newDate.Year - 1;
                int thang = newDate.Month - 1;
                int ngay = newDate.Day - 1;
                int gio = newDate.Hour;
                int phut = newDate.Minute;
                int giay = newDate.Second;

                if (nam > 0)
                {
                    tuoi = nam.ToString();
                    cboAge = "Tuổi";
                }
                else
                {
                    if (thang > 0)
                    {
                        tuoi = thang.ToString();
                        cboAge = "Tháng";
                    }
                    else
                    {
                        if (ngay > 0)
                        {
                            tuoi = ngay.ToString();
                            cboAge = "ngày";
                        }
                        else
                        {
                            tuoi = "";
                            cboAge = "Giờ";
                        }
                    }
                }
                return tuoi + " " + cboAge;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }
}
