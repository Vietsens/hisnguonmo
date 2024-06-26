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
using MPS.Processor.Mps000134.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000134
{
    public class Mps000134Processor : AbstractProcessor
    {
        Mps000134PDO rdo;
        public Mps000134Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000134PDO)rdoBase;
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

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListMediMate1", rdo.listAdo);
                objectTag.AddObjectData(store, "ListMediMate2", rdo.listAdo);
                objectTag.AddObjectData(store, "ListMediMate3", rdo.listAdo);
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

        void ProcessSingleKey()
        {
            try
            {
                Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredMedi = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredMate = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();

                SetSingleKey(new KeyValue(Mps000134ExtendSingleKey.TITLED_BILL, rdo.Title));
                decimal totalPrice = 0;
                if (rdo._ExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000134ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000134ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000134ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._ExpMest.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey(rdo._ExpMest, false);


                    if (rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Approval
                        || rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                    {
                        //Lấy medicine và material
                        if (rdo._ExpMestMedicines != null && rdo._ExpMestMedicines.Count > 0)
                        {
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
                                    rdo.listAdo.Add(new Mps000134ADO(dic.Value));
                                }
                            }
                        }
                        if (rdo._ExpMestMaterials != null && rdo._ExpMestMaterials.Count > 0)
                        {
                            if (rdo._ExpMestMatyReqs != null && rdo._ExpMestMatyReqs.Count > 0)
                            {
                                List<long> materialTypeIds = rdo._ExpMestMatyReqs.Select(p => p.MATERIAL_TYPE_ID).Distinct().ToList();
                                var Group = rdo._ExpMestMaterials.Where(p => materialTypeIds.Contains(p.MATERIAL_TYPE_ID)).GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).ToList();
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
                                        rdo.listAdo.Add(new Mps000134ADO(dic.Value));
                                    }
                                }
                            }
                        }

                        if (rdo._ExpMest != null)
                        {
                            AddObjectKeyIntoListkey(rdo._ExpMest, false);
                        }

                    }
                    else if (rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Request
                        || rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Reject
                        || rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Draft)
                    {
                        //lay mety_req và mate_req
                        if (rdo._ExpMestMetyReqs != null && rdo._ExpMestMetyReqs.Count > 0)
                        {
                            var Group = rdo._ExpMestMetyReqs.GroupBy(g => new { g.MEDICINE_TYPE_ID }).Select(p => p.ToList()).ToList();
                            foreach (var group in Group)
                            {
                                Mps000134ADO ado = new Mps000134ADO(group, rdo._MedicineTypes);
                                rdo.listAdo.Add(ado);
                                //totalPrice += (ado.AMOUNT * (ado.IMP_PRICE));
                            }
                        }
                        if (rdo._ExpMestMatyReqs != null && rdo._ExpMestMatyReqs.Count > 0)
                        {
                            var Group = rdo._ExpMestMatyReqs.GroupBy(g => new { g.MATERIAL_TYPE_ID }).Select(p => p.ToList()).ToList();
                            foreach (var group in Group)
                            {
                                Mps000134ADO ado = new Mps000134ADO(group, rdo._MaterialTypes);
                                rdo.listAdo.Add(ado);
                            }
                        }
                    }
                }

                SetSingleKey(new KeyValue(Mps000134ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                string sumText = String.Format("0:0.####", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000134ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));

                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();
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
