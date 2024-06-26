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
using MPS.Processor.Mps000135.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000135
{
    public class Mps000135Processor : AbstractProcessor
    {
        Mps000135PDO rdo;

        List<Mps000135ADO> lstMedicineType = new List<Mps000135ADO>();
        List<Mps000135ADO> lstMedicineParent = new List<Mps000135ADO>();
        public Mps000135Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000135PDO)rdoBase;
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

                ProcessSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                GetMedicineGroup();
                GetMedicineParent();
                //Inventec.Common.Logging.LogSystem.Info("ListMediMate3: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.listAdo), rdo.listAdo));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "List", rdo.listAdo);
                //                objectTag.AddObjectData(store, "List", rdo._BloodTypes ?? new List<V_HIS_BLOOD_TYPE>());
                objectTag.AddObjectData(store, "ListMediMate1", rdo.listAdo);
                objectTag.AddObjectData(store, "ListMediMate2", rdo.listAdo);
                objectTag.AddObjectData(store, "ListMediMate3", rdo.listAdo);
                objectTag.AddObjectData(store, "MedicineGroup", lstMedicineType);
                objectTag.AddObjectData(store, "MedicineParent", lstMedicineParent);

                objectTag.AddRelationship(store, "MedicineGroup", "ListMediMate1", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "ListMediMate2", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineGroup", "ListMediMate3", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ListMediMate1", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ListMediMate2", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ListMediMate3", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");

                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData21", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData22", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData31", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData32", new CalculateMergerData());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void GetMedicineGroup()
        {
            try
            {
                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    var group = rdo.listAdo.GroupBy(o => new { o.MEDICINE_GROUP_ID, o.MEDICINE_GROUP_CODE, o.MEDICINE_GROUP_NAME });
                    foreach (var item in group)
                    {
                        lstMedicineType.Add(item.ToList().First());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetMedicineParent()
        {
            try
            {
                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    var group = rdo.listAdo.GroupBy(o => new { o.MEDICINE_PARENT_ID });
                    foreach (var item in group)
                    {
                        lstMedicineParent.Add(item.ToList().First());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<string> GetListStringExpLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroup = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {

                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.FirstOrDefault().EXP_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {

                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.FirstOrDefault().EXP_LOGINNAME)
                    .ToList();
                }
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {

                    expMestBloodGroup = expMestBloodList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                   .GroupBy(o => o.EXP_LOGINNAME)
                   .Select(p => p.FirstOrDefault().EXP_LOGINNAME)
                   .ToList();
                }

                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
                if (result.Count <= 0)
                {
                    result.AddRange(expMestBloodGroup.ToList());
                }
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpTimeLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroup = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.FirstOrDefault().EXP_TIME ?? 0))
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => p.EXP_TIME != null)
                      .GroupBy(o => o.EXP_TIME)
                      .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.FirstOrDefault().EXP_TIME ?? 0))
                      .ToList();
                }
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroup = expMestBloodList.Where(p => p.EXP_TIME != null)
                      .GroupBy(o => o.EXP_TIME)
                      .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.FirstOrDefault().EXP_TIME ?? 0))
                      .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
                if (result.Count <= 0)
                {
                    result.AddRange(expMestBloodGroup.ToList());
                }
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpUserLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroup = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList
                    .Select(p => p.EXP_USERNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList
                    .Select(p => p.EXP_USERNAME)
                    .ToList();
                }
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroup = expMestBloodList
                   .Select(p => p.EXP_USERNAME)
                   .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
                if (result.Count <= 0)
                {
                    result.AddRange(expMestBloodGroup.ToList());
                }
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }


        void ProcessSingleKey()
        {
            try
            {
                if (rdo._ExpMest != null)
                {
                    AddObjectKeyIntoListkey(rdo._ExpMest, false);
                }

                Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredMedi = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredMate = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();
                Dictionary<string, List<V_HIS_EXP_MEST_BLOOD>> dicExpiredBlood = new Dictionary<string, List<V_HIS_EXP_MEST_BLOOD>>();

                decimal totalPrice = 0;
                if (rdo._ExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000135ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000135ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000135ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._ExpMest.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey(rdo._ExpMest, false);
                    Inventec.Common.Logging.LogSystem.Warn("rdo._ExpMest.EXP_MEST_STT_ID___________" + rdo._ExpMest.EXP_MEST_STT_ID);
                    // lấy thời gian thực xuất, người xuất
                    if (rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                    {
                        string expTime = String.Join(", ", GetListStringExpTimeLogFromExpMestMedicineMaterial(rdo._ExpMestMedicines, rdo._ExpMestMaterials, rdo._ExpMestBloods));
                        string expLoginName = String.Join(", ", GetListStringExpLogFromExpMestMedicineMaterial(rdo._ExpMestMedicines, rdo._ExpMestMaterials, rdo._ExpMestBloods));
                        string expUserName = String.Join(", ", GetListStringExpUserLogFromExpMestMedicineMaterial(rdo._ExpMestMedicines, rdo._ExpMestMaterials, rdo._ExpMestBloods));
                        SetSingleKey(new KeyValue(Mps000135ExtendSingleKey.EXP_TIME, expTime));
                        SetSingleKey(new KeyValue(Mps000135ExtendSingleKey.EXP_USERNAME, expUserName));
                        SetSingleKey(new KeyValue(Mps000135ExtendSingleKey.EXP_LOGINNAME, expLoginName));
                    }

                    if (rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Approval
                        || rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                    {
                        //Lấy medicine và material
                        if (rdo._ExpMestMedicines != null && rdo._ExpMestMedicines.Count > 0 && rdo._ExpMestMetyReqs != null && rdo._ExpMestMetyReqs.Count > 0)
                        {
                            rdo._ExpMestMedicines = rdo._ExpMestMedicines.Where(p => rdo._ExpMestMetyReqs.Select(o => o.MEDICINE_TYPE_ID).ToList().Contains(p.MEDICINE_TYPE_ID)).ToList();
                            var Group = rdo._ExpMestMedicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).Select(p => p.ToList()).ToList();
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
                                    totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                                }
                                foreach (var dic in dicExpiredMedi)
                                {
                                    decimal REQ_AMOUNT = rdo._ExpMestMetyReqs.Where(o => o.MEDICINE_TYPE_ID == dic.Value.First().MEDICINE_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                                    decimal DD_AMOUNT = group.Sum(o => o.AMOUNT);
                                    var pdo = new Mps000135ADO(dic.Value, rdo._MedicineTypes, REQ_AMOUNT, DD_AMOUNT);
                                    pdo.TOTAL_PRICE_AFTER_VAT = group.Sum(s => s.AMOUNT * s.IMP_PRICE * (1 + s.IMP_VAT_RATIO));
                                    rdo.listAdo.Add(pdo);
                                }
                            }
                        }
                        if (rdo._ExpMestMaterials != null && rdo._ExpMestMaterials.Count > 0 && rdo._ExpMestMatyReqs != null && rdo._ExpMestMatyReqs.Count > 0)
                        {
                            rdo._ExpMestMaterials = rdo._ExpMestMaterials.Where(p => rdo._ExpMestMatyReqs.Select(o => o.MATERIAL_TYPE_ID).ToList().Contains(p.MATERIAL_TYPE_ID)).ToList();
                            var Group = rdo._ExpMestMaterials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID, g.IMP_PRICE }).ToList();
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
                                    totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                                }
                                foreach (var dic in dicExpiredMate)
                                {
                                    decimal REQ_AMOUNT = rdo._ExpMestMatyReqs.Where(o => o.MATERIAL_TYPE_ID == dic.Value.First().MATERIAL_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                                    decimal DD_AMOUNT = group.Sum(o => o.AMOUNT);
                                    var pdo = new Mps000135ADO(dic.Value, REQ_AMOUNT, DD_AMOUNT);
                                    pdo.TOTAL_PRICE_AFTER_VAT = group.Sum(s => s.AMOUNT* s.IMP_PRICE * (1 + s.IMP_VAT_RATIO));
                                    rdo.listAdo.Add(pdo);
                                }
                            }
                        }
                        //if (rdo._ExpMestBloods != null && rdo._ExpMestBloods.Count > 0 && rdo._ExpMestBltyReqs != null && rdo._ExpMestBltyReqs.Count > 0)
                        //{

                        //    rdo._ExpMestBloods = rdo._ExpMestBloods.Where(p => rdo._ExpMestBltyReqs.Select(o => o.BLOOD_TYPE_ID).ToList().Contains(p.BLOOD_TYPE_ID)).ToList();
                        //    var Group = rdo._ExpMestBloods.GroupBy(g => new { g.BLOOD_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).ToList();
                        //    foreach (var group in Group)
                        //    {
                        //        dicExpiredBlood.Clear();
                        //        var listByGroup = group.ToList<V_HIS_EXP_MEST_BLOOD>();
                        //        foreach (var item in listByGroup)
                        //        {
                        //            string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                        //            if (!dicExpiredBlood.ContainsKey(key))
                        //                dicExpiredBlood[key] = new List<V_HIS_EXP_MEST_BLOOD>();
                        //            dicExpiredBlood[key].Add(item);
                        //            //      totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                        //        }
                        //        foreach (var dic in dicExpiredBlood)
                        //        {
                        //            rdo.listAdo.Add(new Mps000135ADO(dic.Value));
                        //        }
                        //    }
                        //}
                        Inventec.Common.Logging.LogSystem.Info("rdo._ExpMestBloods: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._ExpMestBloods), rdo._ExpMestBloods));

                        Inventec.Common.Logging.LogSystem.Info("rdo._ExpMestBltyReqs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._ExpMestBltyReqs), rdo._ExpMestBltyReqs));

                        if (rdo._ExpMestBloods != null && rdo._ExpMestBloods.Count > 0)
                        {
                            var Group = rdo._ExpMestBloods.GroupBy(g => new { g.BLOOD_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).ToList();
                            foreach (var group in Group)
                            {
                                dicExpiredBlood.Clear();
                                var listByGroup = group.ToList<V_HIS_EXP_MEST_BLOOD>();
                                foreach (var item in listByGroup)
                                {
                                    string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                                    if (!dicExpiredBlood.ContainsKey(key))
                                        dicExpiredBlood[key] = new List<V_HIS_EXP_MEST_BLOOD>();
                                    dicExpiredBlood[key].Add(item);

                                }
                                foreach (var dic in dicExpiredBlood)
                                {
                                    decimal REQ_AMOUNT = rdo._ExpMestBltyReqs.Where(o => o.BLOOD_TYPE_ID == dic.Value.First().BLOOD_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                                    decimal DD_AMOUNT = group.Count();
                                    var pdo = new Mps000135ADO(dic.Value, REQ_AMOUNT, DD_AMOUNT);
                                    pdo.TOTAL_PRICE_AFTER_VAT = group.Sum(s=>s.IMP_PRICE*(1+s.IMP_VAT_RATIO));
                                    rdo.listAdo.Add(pdo);
                                }
                            }
                        }

                        else if (rdo._ExpMestBltyReqs != null && rdo._ExpMestBltyReqs.Count > 0 && rdo._BloodTypes != null && rdo._BloodTypes.Count > 0)
                        {
                            var Group = rdo._ExpMestBltyReqs.GroupBy(g => new { g.BLOOD_TYPE_ID }).ToList();
                            foreach (var group in Group)
                            {
                                dicExpiredBlood.Clear();
                                var listByGroup = group.ToList<HIS_EXP_MEST_BLTY_REQ>();
                                foreach (var item in listByGroup)
                                {
                                    List<V_HIS_EXP_MEST_BLOOD> lstadd = new List<V_HIS_EXP_MEST_BLOOD>();

                                    var BloodType = rdo._BloodTypes.FirstOrDefault(o => o.ID == item.BLOOD_TYPE_ID);

                                    if (BloodType != null)
                                    {
                                        V_HIS_EXP_MEST_BLOOD bl = new V_HIS_EXP_MEST_BLOOD();
                                        bl.BLOOD_TYPE_CODE = BloodType.BLOOD_TYPE_CODE;
                                        bl.BLOOD_TYPE_NAME = BloodType.BLOOD_TYPE_NAME;
                                        bl.VOLUME = BloodType.VOLUME;
                                        bl.SERVICE_UNIT_NAME = BloodType.SERVICE_UNIT_NAME;
                                        lstadd.Add(bl);
                                    }

                                    rdo.listAdo.Add(new Mps000135ADO(lstadd));

                                }
                            }
                        }


                        if (rdo._ExpMest != null)
                        {
                            AddObjectKeyIntoListkey(rdo._ExpMest, false);
                        }

                    }
                    //else
                    //{
                    //    //lay mety_req và mate_req
                    //    if (rdo._ExpMestMetyReqs != null && rdo._ExpMestMetyReqs.Count > 0)
                    //    {
                    //        var Group = rdo._ExpMestMetyReqs.GroupBy(g => new { g.MEDICINE_TYPE_ID }).Select(p => p.ToList()).ToList();
                    //        foreach (var group in Group)
                    //        {
                    //            Mps000135ADO ado = new Mps000135ADO(group, rdo._MedicineTypes);
                    //            rdo.listAdo.Add(ado);
                    //            //totalPrice += (ado.AMOUNT * (ado.IMP_PRICE));
                    //        }
                    //    }
                    //    if (rdo._ExpMestMatyReqs != null && rdo._ExpMestMatyReqs.Count > 0)
                    //    {
                    //        var Group = rdo._ExpMestMatyReqs.GroupBy(g => new { g.MATERIAL_TYPE_ID }).Select(p => p.ToList()).ToList();
                    //        foreach (var group in Group)
                    //        {
                    //            Mps000135ADO ado = new Mps000135ADO(group, rdo._MaterialTypes);
                    //            rdo.listAdo.Add(ado);
                    //        }
                    //    }
                    //    if (rdo._ExpMestBltyReqs != null && rdo._ExpMestBltyReqs.Count > 0)
                    //    {
                    //        var Group = rdo._ExpMestBltyReqs.GroupBy(g => new { g.BLOOD_TYPE_ID }).Select(p => p.ToList()).ToList();
                    //        foreach (var group in Group)
                    //        {
                    //            Mps000135ADO ado = new Mps000135ADO(group, rdo._BloodTypes);
                    //            rdo.listAdo.Add(ado);
                    //        }
                    //    }
                    //}
                    //
                    if (rdo._ExpMestMetyReqs != null && rdo._ExpMestMetyReqs.Count > 0)
                    {
                        var ds = rdo._ExpMestMedicines.Select(o => o.MEDICINE_TYPE_ID).ToList();
                        var Group = rdo._ExpMestMetyReqs.Where(o => !ds.Contains(o.MEDICINE_TYPE_ID)).GroupBy(g => new { g.MEDICINE_TYPE_ID}).Select(p => p.ToList()).ToList();
                        foreach (var group in Group)
                        {
                            decimal REQ_AMOUNT = rdo._ExpMestMetyReqs.Where(o => o.MEDICINE_TYPE_ID == group.First().MEDICINE_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                            decimal DD_AMOUNT = 0;

                            Mps000135ADO ado = new Mps000135ADO(group, rdo._MedicineTypes, REQ_AMOUNT, DD_AMOUNT);
                            rdo.listAdo.Add(ado);
                            //totalPrice += (ado.AMOUNT * (ado.IMP_PRICE));
                        }
                    }
                    if (rdo._ExpMestMatyReqs != null && rdo._ExpMestMatyReqs.Count > 0)
                    {
                        var ds = rdo._ExpMestMaterials.Select(o => o.MATERIAL_TYPE_ID).ToList();
                        var Group = rdo._ExpMestMatyReqs.Where(o => !ds.Contains(o.MATERIAL_TYPE_ID)).GroupBy(g => new { g.MATERIAL_TYPE_ID }).Select(p => p.ToList()).ToList();
                        foreach (var group in Group)
                        {
                            decimal REQ_AMOUNT = rdo._ExpMestMatyReqs.Where(o => o.MATERIAL_TYPE_ID == group.First().MATERIAL_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                            decimal DD_AMOUNT = 0;
                            Mps000135ADO ado = new Mps000135ADO(group, rdo._MaterialTypes,REQ_AMOUNT,DD_AMOUNT);
                            rdo.listAdo.Add(ado);
                        }
                    }
                    if (rdo._ExpMestBltyReqs != null && rdo._ExpMestBltyReqs.Count > 0)
                    {
                        var ds = rdo._ExpMestBloods.Select(o => o.BLOOD_TYPE_ID).ToList();
                        var Group = rdo._ExpMestBltyReqs.Where(o => !ds.Contains(o.BLOOD_TYPE_ID)).GroupBy(g => new { g.BLOOD_TYPE_ID }).Select(p => p.ToList()).ToList();
                        foreach (var group in Group)
                        {
                            decimal REQ_AMOUNT = rdo._ExpMestBltyReqs.Where(o => o.BLOOD_TYPE_ID == group.First().BLOOD_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                            decimal DD_AMOUNT = 0;
                            Mps000135ADO ado = new Mps000135ADO(group, rdo._BloodTypes,REQ_AMOUNT,DD_AMOUNT);
                            rdo.listAdo.Add(ado);
                        }
                    }
                }

                SetSingleKey(new KeyValue(Mps000135ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                string sumText = String.Format("0:0.####", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000135ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));

                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();
                string _Title_str = "";
                switch (rdo._Title)
                {
                    case Mps000135PDO.keyTitles.phieuTongHop:
                        _Title_str = "TỔNG HỢP";
                        break;
                    case Mps000135PDO.keyTitles.phieuThuocThuong:
                        _Title_str = "THUỐC THƯỜNG";
                        break;
                    case Mps000135PDO.keyTitles.phieuVatTu:
                        _Title_str = "VẬT TƯ";
                        break;
                    case Mps000135PDO.keyTitles.phieuHoaChat:
                        _Title_str = "HÓA CHẤT";
                        break;
                    case Mps000135PDO.keyTitles.phieuGayNghien:
                        _Title_str = "THUỐC GÂY NGHIỆN";
                        break;
                    case Mps000135PDO.keyTitles.phieuHuongThan:
                        _Title_str = "THUỐC HƯỚNG THẦN";
                        break;
                    case Mps000135PDO.keyTitles.phieuGN_HT:
                        _Title_str = "THUỐC GÂY NGHIỆN, THUỐC HƯỚNG THẦN";
                        break;
                    case Mps000135PDO.keyTitles.phieuThuocDoc:
                        _Title_str = "THUỐC ĐỘC";
                        break;
                    case Mps000135PDO.keyTitles.phieuThuocPhongXa:
                        _Title_str = "THUỐC PHÓNG XẠ";
                        break;
                    case Mps000135PDO.keyTitles.Corticoid:
                        _Title_str = "THUỐC CORTICOID";
                        break;
                    case Mps000135PDO.keyTitles.DichTruyen:
                        _Title_str = "THUỐC DỊCH TRUYỀN";
                        break;
                    case Mps000135PDO.keyTitles.KhangSinh:
                        _Title_str = "THUỐC KHÁNG SINH";
                        break;
                    case Mps000135PDO.keyTitles.Lao:
                        _Title_str = "THUỐC LAO";
                        break;
                    case Mps000135PDO.keyTitles.Mau:
                        _Title_str = "MÁU";
                        break;
                    default:
                        break;
                }
                SetSingleKey(new KeyValue(Mps000135ExtendSingleKey.KEY_NAME_TITLES, _Title_str));
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
                log = LogDataExpMest(rdo._ExpMest.TDL_TREATMENT_CODE, rdo._ExpMest.EXP_MEST_CODE, "");
                log += string.Format("Kho: {0}, Phòng yêu cầu: {1}", rdo._ExpMest.MEDI_STOCK_NAME, rdo._ExpMest.REQ_ROOM_NAME);
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
                string _keyname = "";
                switch (rdo._Title)
                {
                    case Mps000135PDO.keyTitles.phieuTongHop:
                        _keyname = "TH";
                        break;
                    case Mps000135PDO.keyTitles.phieuThuocThuong:
                        _keyname = "TT";
                        break;
                    case Mps000135PDO.keyTitles.phieuVatTu:
                        _keyname = "VT";
                        break;
                    case Mps000135PDO.keyTitles.phieuHoaChat:
                        _keyname = "HC";
                        break;
                    case Mps000135PDO.keyTitles.phieuGayNghien:
                        _keyname = "TGN";
                        break;
                    case Mps000135PDO.keyTitles.phieuHuongThan:
                        _keyname = "THT";
                        break;
                    case Mps000135PDO.keyTitles.phieuGN_HT:
                        _keyname = "TGN_THT";
                        break;
                    case Mps000135PDO.keyTitles.phieuThuocDoc:
                        _keyname = "TĐ";
                        break;
                    case Mps000135PDO.keyTitles.phieuThuocPhongXa:
                        _keyname = "TPX";
                        break;
                    case Mps000135PDO.keyTitles.Mau:
                        _keyname = "M";
                        break;
                    default:
                        break;
                }

                if (rdo != null && rdo._ExpMest != null && rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    if (rdo.listAdo.Select(o => o.MEDI_MATE_TYPE_CODE).ToList() != null && rdo.listAdo.Select(o => o.MEDI_MATE_TYPE_CODE).ToList().Count > 0)
                    {
                        result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}", printTypeCode, rdo._ExpMest.REQ_DEPARTMENT_CODE, rdo._ExpMest.MEDI_STOCK_CODE, rdo._ExpMest.EXP_MEST_CODE, _keyname, rdo.listAdo.First().MEDI_MATE_TYPE_CODE);
                    }
                    if (rdo.listAdo.Select(o => o.BLOOD_TYPE_CODE).ToList() != null && rdo.listAdo.Select(o => o.BLOOD_TYPE_CODE).ToList().Count > 0)
                    {
                        result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}", printTypeCode, rdo._ExpMest.REQ_DEPARTMENT_CODE, rdo._ExpMest.MEDI_STOCK_CODE, rdo._ExpMest.EXP_MEST_CODE, _keyname, rdo.listAdo.First().BLOOD_TYPE_CODE);

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
