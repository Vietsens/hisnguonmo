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
using MPS.Processor.Mps000216.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000216
{
    public class Mps000216Processor : AbstractProcessor
    {
        Mps000216PDO rdo;
        public Mps000216Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000216PDO)rdoBase;
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

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "ListMediMate1", rdo.listAdo);
                objectTag.AddObjectData(store, "ListMediMate2", rdo.listAdo);
                objectTag.AddObjectData(store, "ListMediMate3", rdo.listAdo);
                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());

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
                decimal discount = 0;
                if (rdo.OtherExpMest != null)
                {
                    string approvalLoginname = String.Join(", ", GetListStringApprovalLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                    string expLoginName = String.Join(", ", GetListStringExpLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                    string expTime = String.Join(", ", GetListStringExpTimeLogFromExpMestMedicineMaterial(rdo._Medicines, rdo._Materials));
                    SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.EXP_TIME, expTime));
                    SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.APPROVAL_LOGINNAME, approvalLoginname));
                    SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.EXP_LOGINNAME, expLoginName));
                    SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.OtherExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.OtherExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.CREATE_TIME, (rdo.OtherExpMest.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey(rdo.OtherExpMest, false);
                }

                if (rdo._Medicines != null && rdo._Medicines.Count > 0)
                {

                    discount += rdo._Medicines.Sum(o => o.DISCOUNT ?? 0);
                    var Group = rdo._Medicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PRICE, g.VAT_RATIO, g.DISCOUNT }).ToList();
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
                            totalPrice += item.AMOUNT * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1);
                        }
                        foreach (var dic in dicExpiredMedi)
                        {
                            rdo.listAdo.Add(new Mps000216ADO(dic.Value));
                        }
                    }
                }

                if (rdo._Materials != null && rdo._Materials.Count > 0)
                {
                    rdo._Materials = rdo._Materials.OrderBy(o => o.ID).ToList();
                    discount += rdo._Materials.Sum(s => s.DISCOUNT ?? 0);
                    var Group = rdo._Materials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PRICE, g.VAT_RATIO, g.DISCOUNT }).ToList();
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
                            totalPrice += item.AMOUNT * (item.PRICE ?? 0) * ((item.VAT_RATIO ?? 0) + 1);
                        }
                        foreach (var dic in dicExpiredMate)
                        {
                            rdo.listAdo.Add(new Mps000216ADO(dic.Value));
                        }
                    }
                }

                string sumTotalPriceSeparate = Inventec.Common.Number.Convert.NumberToString(totalPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                string discountSeparate = Inventec.Common.Number.Convert.NumberToString(discount, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                string SumTotalPriceAfterDiscountSeparate = Inventec.Common.Number.Convert.NumberToString((totalPrice - discount), HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.SUM_TOTAL_PRICE_SEPARATE, sumTotalPriceSeparate));
                SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.DISCOUNT, discount));
                SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.DISCOUNT_SEPARATE, discountSeparate));
                SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.SUM_TOTAL_PRICE_AFTER_DISCOUNT_SEPARATE, (totalPrice - discount)));
                string sumText = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000216ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));
                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();
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
                log = LogDataExpMest(rdo.OtherExpMest.TDL_TREATMENT_CODE, rdo.OtherExpMest.EXP_MEST_CODE, "");
                log += string.Format("Kho: {0}, Phòng yêu cầu: {1}", rdo.OtherExpMest.MEDI_STOCK_NAME, rdo.OtherExpMest.REQ_ROOM_NAME);
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
                if (rdo != null && rdo.OtherExpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}", printTypeCode, rdo.OtherExpMest.TDL_TREATMENT_CODE, rdo.OtherExpMest.MEDI_STOCK_CODE, rdo.OtherExpMest.EXP_MEST_CODE, rdo._Medicines.Count, rdo._Materials.Count);
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
