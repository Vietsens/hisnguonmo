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
using MPS.Processor.Mps000351.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000351
{
    class Mps000351Processor : AbstractProcessor
    {
        Mps000351PDO rdo;
        public Mps000351Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000351PDO)rdoBase;
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
                objectTag.AddObjectData(store, "ListMediMate1", rdo.listAdo);
                objectTag.AddObjectData(store, "ListMediMate2", rdo.listAdo);
                objectTag.AddObjectData(store, "ListMediMate3", rdo.listAdo);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<string> GetListStringApprovalLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpTimeLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => p.EXP_TIME != null)
                      .GroupBy(o => o.EXP_TIME)
                      .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                      .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredMedi = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredMate = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();

                decimal totalPrice = 0;
                decimal totalPriceNoTh = 0;
                decimal totalPriceNotRound = 0;
                decimal discount = 0;

                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.NUM_ORDER, rdo.Transaction.NUM_ORDER));

                if (this.rdo._SaleExpMests != null && this.rdo._SaleExpMests.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._SaleExpMests.OrderByDescending(o => o.EXP_MEST_CODE).FirstOrDefault().CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._SaleExpMests.OrderByDescending(o => o.EXP_MEST_CODE).FirstOrDefault().CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._SaleExpMests.OrderByDescending(o => o.EXP_MEST_CODE).FirstOrDefault().CREATE_TIME ?? 0)));

                    var exp = this.rdo._SaleExpMests.FirstOrDefault(o => o.TDL_PATIENT_ID.HasValue);
                    if (exp != null)
                    {
                        SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.PATIENT_NAME, exp.TDL_PATIENT_NAME));
                        AddObjectKeyIntoListkey(exp, false);
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.PATIENT_NAME, this.rdo._SaleExpMests.FirstOrDefault().TDL_PATIENT_NAME));
                        SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.VIR_PATIENT_NAME, this.rdo._SaleExpMests.FirstOrDefault().TDL_PATIENT_NAME));
                        AddObjectKeyIntoListkey(this.rdo._SaleExpMests.FirstOrDefault(), false);
                    }
                    SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.EXP_MEST_CODES, String.Join(",", this.rdo._SaleExpMests.Select(s => s.EXP_MEST_CODE).ToList())));
                    discount = this.rdo._SaleExpMests.Sum(o => (o.DISCOUNT ?? 0));
                }

                if (this.rdo._Medicines != null && this.rdo._Medicines.Count > 0)
                {
                    this.rdo._Medicines = this.rdo._Medicines.OrderBy(o => o.ID).ToList();
                    var Group = this.rdo._Medicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PRICE, g.VAT_RATIO, g.DISCOUNT, g.PACKAGE_NUMBER }).ToList();
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
                            totalPriceNoTh += item.AMOUNT * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1) - (item.DISCOUNT ?? 0);
                            totalPrice += (item.AMOUNT - (item.TH_AMOUNT ?? 0)) * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1) - (item.DISCOUNT ?? 0);
                        }
                        foreach (var dic in dicExpiredMedi)
                        {
                            Mps000351ADO ado = new Mps000351ADO(dic.Value);
                            ado.EXPIRED_DATE_STR = ConvertToDataString(dic.Key);
                            totalPriceNotRound += ado.SUM_PRICE_NOT_ROUNT;
                            rdo.listAdo.Add(ado);
                        }
                    }
                }

                if (this.rdo._Materials != null && this.rdo._Materials.Count > 0)
                {
                    this.rdo._Materials = this.rdo._Materials.OrderBy(o => o.ID).ToList();
                    var Group = this.rdo._Materials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PRICE, g.VAT_RATIO, g.DISCOUNT, g.PACKAGE_NUMBER }).ToList();
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
                            totalPriceNoTh += item.AMOUNT * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1) - (item.DISCOUNT ?? 0);
                            totalPrice += (item.AMOUNT - (item.TH_AMOUNT ?? 0)) * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1) - (item.DISCOUNT ?? 0);
                        }
                        foreach (var dic in dicExpiredMate)
                        {
                            Mps000351ADO ado = new Mps000351ADO(dic.Value);
                            ado.EXPIRED_DATE_STR = ConvertToDataString(dic.Key);
                            totalPriceNotRound += ado.SUM_PRICE_NOT_ROUNT;
                            rdo.listAdo.Add(ado);
                        }
                    }
                }


                string approvalLoginname = String.Join(", ", GetListStringApprovalLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                string expLoginName = String.Join(", ", GetListStringExpLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                string expTime = String.Join(", ", GetListStringExpTimeLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.EXP_TIME, expTime));
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.APPROVAL_LOGINNAME, approvalLoginname));
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.EXP_LOGINNAME, expLoginName));
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.SUM_TOTAL_PRICE_NOT_ROUND, totalPriceNotRound));
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.DISCOUNT, discount));
                string sumText = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.SUM_TOTAL_PRICE_AFTER_DISCOUNT, Inventec.Common.Number.Convert.NumberToString(totalPrice - discount)));
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.SUM_TOTAL_PRICE_AFTER_DISCOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice - discount)))));

                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.SUM_TOTAL_PRICE_NO_TH, totalPriceNoTh));

                string sumNotRoundText = String.Format("{0:0}", totalPriceNotRound);
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.SUM_TOTAL_PRICE_NOT_ROUND_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumNotRoundText)));
                decimal sumAfterDiscountNotRound = Math.Round((totalPriceNotRound - discount), 0, MidpointRounding.AwayFromZero);
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.SUM_TOTAL_PRICE_AFTER_DISCOUNT_NOT_ROUND, sumAfterDiscountNotRound));
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.SUM_TOTAL_PRICE_AFTER_DISCOUNT_NOT_ROUND_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0}", sumAfterDiscountNotRound))));
                string mobaImpMestCount = "";
                if (this.rdo._HisImpMest != null && rdo._HisImpMest.Count > 0)
                {
                    mobaImpMestCount = rdo._HisImpMest.Count.ToString();
                }
                SetSingleKey(new KeyValue(Mps000351ExtendSingleKey.MOBA_IMP_MEST_COUNT, mobaImpMestCount));
                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();

                if (rdo.Transaction != null) AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo.Transaction, false);
                //AddObjectKeyIntoListkey<V_HIS_EXP_MEST>(rdo._SaleExpMest, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.Transaction != null && !String.IsNullOrEmpty(rdo.Transaction.TRANSACTION_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTransactionCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Transaction.TRANSACTION_CODE);
                    barcodeTransactionCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTransactionCode.IncludeLabel = false;
                    barcodeTransactionCode.Width = 120;
                    barcodeTransactionCode.Height = 40;
                    barcodeTransactionCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTransactionCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTransactionCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTransactionCode.IncludeLabel = true;

                    dicImage.Add(Mps000351ExtendSingleKey.TRANSACTION_CODE_BAR, barcodeTransactionCode);
                }

                if (rdo._SaleExpMests != null && rdo._SaleExpMests.Count > 0)
                {
                    string expMestCode = rdo._SaleExpMests.OrderByDescending(o => o.EXP_MEST_CODE).FirstOrDefault().EXP_MEST_CODE;
                    Inventec.Common.BarcodeLib.Barcode barcodeExpMestCode = new Inventec.Common.BarcodeLib.Barcode(expMestCode);
                    barcodeExpMestCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeExpMestCode.IncludeLabel = false;
                    barcodeExpMestCode.Width = 120;
                    barcodeExpMestCode.Height = 40;
                    barcodeExpMestCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeExpMestCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeExpMestCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeExpMestCode.IncludeLabel = true;

                    dicImage.Add(Mps000351ExtendSingleKey.EXP_MEST_CODE_BAR, barcodeExpMestCode);
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

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = this.LogDataExpMests(rdo._SaleExpMests.FirstOrDefault().TDL_TREATMENT_CODE, rdo._SaleExpMests.Select(s => s.EXP_MEST_CODE).ToList(), "");
                log += "MEDI_STOCK_NAME: " + rdo._SaleExpMests.FirstOrDefault().MEDI_STOCK_NAME;
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
                if (rdo != null)
                {
                    if (rdo.Transaction != null)
                    {
                        result = String.Format("{0}_{1}_{2}_{3}", this.printTypeCode, rdo.Transaction.ACCOUNT_BOOK_CODE, rdo.Transaction.TRANSACTION_CODE, rdo.Transaction.NUM_ORDER);
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

        private string LogDataExpMests(string treatmentCode, List<string> expMestCodes, string message)
        {
            string result = "";
            try
            {
                result += message + ". ";

                if (!String.IsNullOrWhiteSpace(treatmentCode))
                {
                    result += string.Format("TREATMENT_CODE: {0}. ", treatmentCode);
                }

                if (expMestCodes != null)
                {
                    foreach (var expMestCode in expMestCodes)
                    {
                        if (!String.IsNullOrWhiteSpace(expMestCode))
                        {
                            result += string.Format("EXP_MEST_CODE: {0}. ", expMestCode);
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
