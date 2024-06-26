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
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000165.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000165
{
    public class Mps000165Processor : AbstractProcessor
    {
        Mps000165PDO rdo;
        public Mps000165Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000165PDO)rdoBase;
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
                Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredMedi = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredMate = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();
                decimal totalPrice = 0;

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                Inventec.Common.Logging.LogSystem.Info("Mps165: rdo.listAdo: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.listAdo), rdo.listAdo));

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

                store.SetCommonFunctions();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
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

        List<string> GetListStringImpMedistockFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList)
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
                //Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredMedi = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                //Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredMate = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();
                decimal totalPrice = 0;
                decimal discount = 0;
                decimal impPriceAfterVATSum = 0;
                if (rdo.OtherExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.OtherExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.OtherExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.OtherExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.RECIPIENT, rdo.OtherExpMest.RECIPIENT ?? ""));
                    SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.RECEIVING_PLACE, rdo.OtherExpMest.RECEIVING_PLACE ?? ""));
                    AddObjectKeyIntoListkey(rdo.OtherExpMest, false);
                }

                if (rdo._Medicines != null && rdo._Medicines.Count > 0)
                {

                    discount += rdo._Medicines.Sum(o => o.DISCOUNT ?? 0);
                    var Group = rdo._Medicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.IMP_PRICE, g.IMP_VAT_RATIO, g.DISCOUNT }).ToList();
                    foreach (var group in Group)
                    {
                        var listByGroup = group.ToList<V_HIS_EXP_MEST_MEDICINE>();
                        rdo.listAdo.Add(new Mps000165ADO(listByGroup));
                        foreach (var item in listByGroup)
                        {
                            totalPrice += item.AMOUNT * item.IMP_PRICE * (item.IMP_VAT_RATIO + 1);
                        }
                    }
                }

                if (rdo._Materials != null && rdo._Materials.Count > 0)
                {
                    rdo._Materials = rdo._Materials.OrderBy(o => o.ID).ToList();
                    discount += rdo._Materials.Sum(s => s.DISCOUNT ?? 0);
                    var Group = rdo._Materials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.IMP_PRICE, g.IMP_VAT_RATIO, g.DISCOUNT }).ToList();
                    foreach (var group in Group)
                    {
                        var listByGroup = group.ToList<V_HIS_EXP_MEST_MATERIAL>();
                        rdo.listAdo.Add(new Mps000165ADO(listByGroup));
                        foreach (var item in listByGroup)
                        {
                            totalPrice += item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1);
                        }
                    }
                }

                string approvalLoginname = String.Join(", ", GetListStringApprovalLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                string expLoginName = String.Join(", ", GetListStringExpLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                string expTime = String.Join(", ", GetListStringExpTimeLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.EXP_TIME, expTime));
                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.APPROVAL_LOGINNAME, approvalLoginname));
                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.EXP_LOGINNAME, expLoginName));
                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                string sumText = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));
                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.SUM_TOTAL_PRICE_AFTER_DISCOUNT, Inventec.Common.Number.Convert.NumberToString(totalPrice - discount)));
                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();

                impPriceAfterVATSum += rdo.listAdo.Sum(O => O.IMP_PRICE_AFTER_VAT_TOTAL);
                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.IMP_PRICE_AFTER_VAT_SUM, impPriceAfterVATSum));
                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.IMP_PRICE_AFTER_VAT_SUM_FORMAT, Inventec.Common.Number.Convert.NumberToString(impPriceAfterVATSum)));
                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.IMP_PRICE_AFTER_VAT_SUM_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(impPriceAfterVATSum.ToString())));
                string impPriceAfterVATSumSeparate = "";
                if (ConfigApplications.NumberSeperator == 0)
                {
                    impPriceAfterVATSumSeparate = String.Format("{0:0}", Inventec.Common.Number.Convert.NumberToNumberRoundAuto(impPriceAfterVATSum, ConfigApplications.NumberSeperator));
                }
                else if (ConfigApplications.NumberSeperator == 1)
                {
                    impPriceAfterVATSumSeparate = String.Format("{0:0.#}", Inventec.Common.Number.Convert.NumberToNumberRoundAuto(impPriceAfterVATSum, ConfigApplications.NumberSeperator));
                }
                else if (ConfigApplications.NumberSeperator == 2)
                {
                    impPriceAfterVATSumSeparate = String.Format("{0:0.##}", Inventec.Common.Number.Convert.NumberToNumberRoundAuto(impPriceAfterVATSum, ConfigApplications.NumberSeperator));
                }
                else if (ConfigApplications.NumberSeperator == 3)
                {
                    impPriceAfterVATSumSeparate = String.Format("{0:0.###}", Inventec.Common.Number.Convert.NumberToNumberRoundAuto(impPriceAfterVATSum, ConfigApplications.NumberSeperator));
                }
                else if (ConfigApplications.NumberSeperator == 4)
                {
                    impPriceAfterVATSumSeparate = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundAuto(impPriceAfterVATSum, ConfigApplications.NumberSeperator));
                }

                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.IMP_PRICE_AFTER_VAT_SUM_SEPARATE, impPriceAfterVATSumSeparate));
                SetSingleKey(new KeyValue(Mps000165ExtendSingleKey.IMP_PRICE_AFTER_VAT_SUM_SEPARATE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(impPriceAfterVATSumSeparate)));

                if (rdo.ListMachine != null && rdo.ListMachine.Count > 0)
                {
                    HIS_MACHINE machine = rdo.ListMachine.FirstOrDefault(o => o.ID == rdo.OtherExpMest.MACHINE_ID);
                    if (machine != null)
                    {
                        AddObjectKeyIntoListkey(machine, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
}
