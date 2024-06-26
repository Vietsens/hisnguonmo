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
using MPS.Processor.Mps000168.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000168
{
    public class Mps000168Processor : AbstractProcessor
    {
        Mps000168PDO rdo;
        public Mps000168Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000168PDO)rdoBase;
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
                objectTag.AddObjectData(store, "ListExpMestUser", rdo._ExpMestUserPrint);
                objectTag.AddObjectData(store, "ListRoleUserEnd", rdo.roleAdo);

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

                decimal totalPrice = 0;
                if (rdo._expMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000168ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._expMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000168ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._expMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000168ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._expMest.CREATE_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000168ExtendSingleKey.EXP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._expMest.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000168ExtendSingleKey.EXP_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._expMest.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000168ExtendSingleKey.EXP_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._expMest.FINISH_TIME ?? 0)));
                    AddObjectKeyIntoListkey(rdo._expMest,  false);
                    AddObjectKeyIntoListkey(rdo._expMest, false);
                }

                if (rdo._Medicines != null && rdo._Medicines.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần của id
                    rdo._Medicines = rdo._Medicines.OrderBy(o => o.ID).ToList();
                    var Group = rdo._Medicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).ToList();
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
                            rdo.listAdo.Add(new Mps000168ADO(dic.Value));
                        }
                    }
                }

                if (rdo._Materials != null && rdo._Materials.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần của id
                    rdo._Materials = rdo._Materials.OrderBy(o => o.ID).ToList();
                    var Group = rdo._Materials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).ToList();
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
                            rdo.listAdo.Add(new Mps000168ADO(dic.Value));
                        }
                    }
                }

                SetSingleKey(new KeyValue(Mps000168ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                string sumText = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000168ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));

                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();
                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000168ExtendSingleKey.SUM_TOTAL_AMOUNT, rdo.listAdo.Count));
                }

                if (rdo._ExpMestUserPrint == null)
                {
                    rdo._ExpMestUserPrint = new List<V_HIS_EXP_MEST_USER>();
                }

                if (rdo._ExpMestUserPrint != null && rdo._ExpMestUserPrint.Count > 0)
                {
                    RoleADO role = new RoleADO();

                    int count = 1;
                    foreach (var item in rdo._ExpMestUserPrint)
                    {
                        if (count > 10)
                            break;
                        System.Reflection.PropertyInfo piRole = typeof(RoleADO).GetProperty("Role" + count);
                        System.Reflection.PropertyInfo piUser = typeof(RoleADO).GetProperty("User" + count);
                        piRole.SetValue(role, item.EXECUTE_ROLE_NAME);
                        piUser.SetValue(role, item.USERNAME);
                        count++;
                    }
                    rdo.roleAdo.Add(role);
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
