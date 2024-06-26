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
using MPS.Processor.Mps000090.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000090
{
    class Mps000090Processor : AbstractProcessor
    {
        Mps000090PDO rdo;
        public Mps000090Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000090PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
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
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                if (this.rdo._ChmsExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000090ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000090ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000090ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey(this.rdo._ChmsExpMest, false);
                }

                if (this.rdo._Medicines != null && this.rdo._Medicines.Count > 0)
                {
                    this.rdo._Medicines = this.rdo._Medicines.OrderBy(o => o.ID).ToList();
                    var Group = this.rdo._Medicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).ToList();
                    foreach (var group in Group)
                    {
                        dicExpiredMedi.Clear();
                        var listByGroup = group.ToList<V_HIS_EXP_MEST_MEDICINE>();
                        foreach (var item in listByGroup)
                        {
                            if (item.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || item.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                                continue;
                            string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                            if (!dicExpiredMedi.ContainsKey(key))
                                dicExpiredMedi[key] = new List<V_HIS_EXP_MEST_MEDICINE>();
                            dicExpiredMedi[key].Add(item);
                            totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                        }
                        foreach (var dic in dicExpiredMedi)
                        {
                            rdo.listAdo.Add(new Mps000090ADO(dic.Value));
                        }
                    }
                }

                if (this.rdo._ListMaterial != null && this.rdo._ListMaterial.Count > 0)
                {
                    var Group = this.rdo._ListMaterial.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).ToList();
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
                            rdo.listAdo.Add(new Mps000090ADO(dic.Value));
                        }
                    }
                }

                SetSingleKey(new KeyValue(Mps000090ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                string sumText = String.Format("0:0.####", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000090ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));

                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
