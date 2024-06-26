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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000339.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000339
{
    public class Mps000339Processor : AbstractProcessor
    {
        Mps000339PDO rdo;
        List<Mps000339ADO> ListAdo = new List<Mps000339ADO>();
        public Mps000339Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000339PDO)rdoBase;
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

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListMediMate1", ListAdo);
                objectTag.AddObjectData(store, "ListMediMate2", ListAdo);
                objectTag.AddObjectData(store, "ListMediMate3", ListAdo);
                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData13", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData21", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData22", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData23", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData31", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData32", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData33", new CalculateMergerData());
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
                if (rdo.Transaction != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo.Transaction, false);

                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.TDL_PATIENT_ADDRESS, rdo.Transaction.BUYER_ADDRESS));
                    string amountText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.Transaction.AMOUNT, 0);
                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, amountText));
                    decimal amountAfterExem = rdo.Transaction.AMOUNT - (rdo.Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, amountAfterExem));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(amountAfterExem, 4);
                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, amountAfterExemText));
                    decimal ratio = ((rdo.Transaction.EXEMPTION ?? 0) * 100) / rdo.Transaction.AMOUNT;
                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    decimal canthu = rdo.Transaction.AMOUNT - (rdo.Transaction.KC_AMOUNT ?? 0) - (rdo.Transaction.EXEMPTION ?? 0);
                    if ((rdo.Transaction.TDL_BILL_FUND_AMOUNT ?? 0) > 0)
                    {
                        canthu = canthu - (rdo.Transaction.TDL_BILL_FUND_AMOUNT ?? 0);
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu)));
                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.Number.Convert.NumberToStringRoundAuto(canthu, 4)));

                    Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredMedi = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                    Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredMate = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();
                    decimal totalPrice = 0;
                    decimal totalPriceNotRound = 0;

                    if (this.rdo.Medicines != null && this.rdo.Medicines.Count > 0)
                    {
                        this.rdo.Medicines = this.rdo.Medicines.OrderBy(o => o.ID).ToList();
                        var Group = this.rdo.Medicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.VIR_PRICE }).ToList();
                        foreach (var group in Group)
                        {
                            dicExpiredMedi.Clear();
                            var listByGroup = group.ToList<V_HIS_EXP_MEST_MEDICINE>();
                            foreach (var item in listByGroup)
                            {
                                string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                                if (!dicExpiredMedi.ContainsKey(key))
                                    dicExpiredMedi[key] = new List<V_HIS_EXP_MEST_MEDICINE>();
                                dicExpiredMedi[key].Add(item);
                                totalPrice += (item.AMOUNT - (item.TH_AMOUNT ?? 0)) * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1) - (item.DISCOUNT ?? 0);
                            }
                            foreach (var dic in dicExpiredMedi)
                            {
                                Mps000339ADO ado = new Mps000339ADO(dic.Value);
                                ado.EXPIRED_DATE_STR = ConvertToDataString(dic.Key);
                                totalPriceNotRound += ado.SUM_TOTAL_PRICE_ROUND;
                                ListAdo.Add(ado);
                            }
                        }
                    }

                    if (this.rdo.Materials != null && this.rdo.Materials.Count > 0)
                    {
                        this.rdo.Materials = this.rdo.Materials.OrderBy(o => o.ID).ToList();
                        var Group = this.rdo.Materials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.VIR_PRICE }).ToList();
                        foreach (var group in Group)
                        {
                            dicExpiredMate.Clear();
                            var listByGroup = group.ToList<V_HIS_EXP_MEST_MATERIAL>();
                            foreach (var item in listByGroup)
                            {
                                string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                                if (!dicExpiredMate.ContainsKey(key))
                                    dicExpiredMate[key] = new List<V_HIS_EXP_MEST_MATERIAL>();
                                dicExpiredMate[key].Add(item);
                                totalPrice += (item.AMOUNT - (item.TH_AMOUNT ?? 0)) * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1) - (item.DISCOUNT ?? 0);
                            }
                            foreach (var dic in dicExpiredMate)
                            {
                                Mps000339ADO ado = new Mps000339ADO(dic.Value);
                                ado.EXPIRED_DATE_STR = ConvertToDataString(dic.Key);
                                totalPriceNotRound += ado.SUM_TOTAL_PRICE_ROUND;
                                ListAdo.Add(ado);
                            }
                        }
                    }

                    if (this.rdo.ListBillGoods != null && this.rdo.ListBillGoods.Count > 0 && ListAdo.Count <= 0)
                    {
                        foreach (var item in this.rdo.ListBillGoods)
                        {
                            dicExpiredMate.Clear();
                            totalPrice += (item.AMOUNT * item.PRICE) - (item.DISCOUNT ?? 0);
                            Mps000339ADO ado = new Mps000339ADO(item);
                            totalPriceNotRound += ado.SUM_TOTAL_PRICE_ROUND;
                            ListAdo.Add(ado);
                        }
                    }

                    ListAdo = ListAdo.Where(o => o.TOTAL_AMOUNT > 0).ToList();

                    string sumText = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));
                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.SUM_TOTAL_PRICE_ROUND, totalPriceNotRound));
                    string sumNotRoundText = String.Format("{0:0}", totalPriceNotRound);
                    SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.SUM_TOTAL_PRICE_ROUND_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumNotRoundText)));
                }
                string mobaImpMestCount = "";
                if (this.rdo._HisImpMest != null && rdo._HisImpMest.Count > 0)
                {
                    mobaImpMestCount = rdo._HisImpMest.Count.ToString();
                }
                SetSingleKey(new KeyValue(Mps000339ExtendSingleKey.MOBA_IMP_MEST_COUNT, mobaImpMestCount));
                if (rdo.ListExpMest != null && rdo.ListExpMest.Count > 0)
                {
                    var expMest = rdo.ListExpMest.OrderByDescending(o => o.ID).FirstOrDefault();
                    if (expMest != null)
                    {
                        AddObjectKeyIntoListkey(expMest, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static string ConvertToDataString(string date)
        {
            string result = "";
            if (!String.IsNullOrWhiteSpace(date) && date.Length == 8)
            {
                string year = date.Substring(0, 4);
                string month = date.Substring(4, 2);
                string day = date.Substring(6, 2);
                result = day + "/" + month + "/" + year;//27/10/2019
            }
            return result;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (!String.IsNullOrEmpty(rdo.Transaction.TRANSACTION_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTransactionCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Transaction.TRANSACTION_CODE);
                    barcodeTransactionCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTransactionCode.IncludeLabel = false;
                    barcodeTransactionCode.Width = 120;
                    barcodeTransactionCode.Height = 40;
                    barcodeTransactionCode.RotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipXY;
                    barcodeTransactionCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTransactionCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTransactionCode.IncludeLabel = true;

                    dicImage.Add(Mps000339ExtendSingleKey.TRANSACTION_CODE_BAR, barcodeTransactionCode);
                }

                if (!String.IsNullOrEmpty(rdo.Transaction.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Transaction.TREATMENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(Mps000339ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);
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
                log = "TRANSACTION_CODE: " + rdo.Transaction.TRANSACTION_CODE;
                log += " Người mua: " + rdo.Transaction.TDL_PATIENT_NAME;
                log += " SoTien: " + rdo.Transaction.AMOUNT;
                log += " Lý do hủy: " + rdo.Transaction.CANCEL_REASON;
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
                if (rdo != null && rdo.Transaction != null)
                    result = String.Format("{0} {1} {2} {3}", printTypeCode, "TREATMENT_CODE:" + rdo.Transaction.TREATMENT_CODE, "TRANSACTION_CODE:" + rdo.Transaction.TRANSACTION_CODE, rdo.Transaction.ACCOUNT_BOOK_CODE);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        class CalculateMergerData : TFlexCelUserFunction
        {
            long typeId = 0;
            long mediMateTypeId = 0;

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    long servicetypeId = Convert.ToInt64(parameters[0]);
                    long mediMateId = Convert.ToInt64(parameters[1]);

                    if (servicetypeId > 0 && mediMateId > 0)
                    {
                        if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
                        {
                            return true;
                        }
                        else
                        {
                            this.typeId = servicetypeId;
                            this.mediMateTypeId = mediMateId;
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    result = false;
                }
                return result;
            }
        }
    }
}
