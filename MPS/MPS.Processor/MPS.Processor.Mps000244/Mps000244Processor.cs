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
using MPS.Processor.Mps000244.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000244
{
    class Mps000244Processor : AbstractProcessor
    {
        Mps000244PDO rdo;
        public Mps000244Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000244PDO)rdoBase;
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
                //Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>> dicExpiredExpMestMaterial = new Dictionary<string, List<V_HIS_EXP_MEST_MATERIAL>>();
                //Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>> dicExpiredExpMestMedicine = new Dictionary<string, List<V_HIS_EXP_MEST_MEDICINE>>();
                List<Mps000244ADO> impMestExpMestTemps = new List<PDO.Mps000244ADO>();

                decimal totalPrice = 0;
                if (this.rdo.DisPense != null)
                {
                    AddObjectKeyIntoListkey(this.rdo.DisPense, false);
                }


                if (this.rdo.ImpMestMedicines != null && this.rdo.ImpMestMedicines.Count > 0)
                {
                    this.rdo.ImpMestMedicines = this.rdo.ImpMestMedicines.OrderBy(o => o.ID).ToList();
                    var Group = this.rdo.ImpMestMedicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PRICE }).ToList();
                    foreach (var group in Group)
                    {
                        var listByGroup = group.ToList<V_HIS_IMP_MEST_MEDICINE>();
                        foreach (var item in listByGroup)
                        {
                            totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1));
                        }
                        impMestExpMestTemps.Add(new Mps000244ADO(listByGroup));
                    }
                }

                if (this.rdo.ExpMestMaterials != null && this.rdo.ExpMestMaterials.Count > 0)
                {
                    var Group = this.rdo.ExpMestMaterials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PRICE }).ToList();
                    foreach (var group in Group)
                    {
                        //dicExpiredExpMestMaterial.Clear();
                        var listByGroup = group.ToList<V_HIS_EXP_MEST_MATERIAL>();
                        foreach (var item in listByGroup)
                        {
                            //string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                            //if (!dicExpiredExpMestMaterial.ContainsKey(key))
                            //    dicExpiredExpMestMaterial[key] = new List<V_HIS_EXP_MEST_MATERIAL>();
                            //dicExpiredExpMestMaterial[key].Add(item);
                            
                            totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                        }
                        impMestExpMestTemps.Add(new Mps000244ADO(listByGroup));
                        //foreach (var dic in dicExpiredExpMestMaterial)
                        //{
                        //    impMestExpMestTemps.Add(new Mps000244ADO(dic.Value));
                        //}
                    }
                }

                if (this.rdo.ExpMestMedicines != null && this.rdo.ExpMestMedicines.Count > 0)
                {
                    var Group = this.rdo.ExpMestMedicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PRICE }).ToList();
                    foreach (var group in Group)
                    {
                        //dicExpiredExpMestMedicine.Clear();
                        var listByGroup = group.ToList<V_HIS_EXP_MEST_MEDICINE>();
                        foreach (var item in listByGroup)
                        {
                            //string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                            //if (!dicExpiredExpMestMedicine.ContainsKey(key))
                            //    dicExpiredExpMestMedicine[key] = new List<V_HIS_EXP_MEST_MEDICINE>();
                            //dicExpiredExpMestMedicine[key].Add(item);
                            totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                        }
                        impMestExpMestTemps.Add(new Mps000244ADO(listByGroup));
                        //foreach (var dic in dicExpiredExpMestMedicine)
                        //{
                        //    impMestExpMestTemps.Add(new Mps000244ADO(dic.Value));
                        //}
                    }
                }

                if (impMestExpMestTemps != null && impMestExpMestTemps.Count > 0)
                {
                    var Group = impMestExpMestTemps.GroupBy(g => new { g.TYPE_ID, g.MEDI_MATE_TYPE_ID, g.IMP_EXP_PRICE }).ToList();
                    foreach (var group in Group)
                    {
                        var listByGroup = group.ToList<Mps000244ADO>();
                        Mps000244ADO mps000244ADO = listByGroup.FirstOrDefault();
                        mps000244ADO.NL_AMOUNT = (listByGroup != null && listByGroup.Count > 0) ? listByGroup.Sum(o => o.NL_AMOUNT) : null;
                        if (mps000244ADO.NL_AMOUNT == 0)
                        {
                            mps000244ADO.NL_AMOUNT = null;
                        }
                        rdo.listAdo.Add(mps000244ADO);
                    }
                }

                SetSingleKey(new KeyValue(Mps000244ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                string sumText = String.Format("0:0.####", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000244ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));

                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();

                if (this.rdo.Medistocks != null && this.rdo.Medistocks.Count > 0)
                {
                    var medistock = this.rdo.Medistocks.FirstOrDefault(o => o.ID == this.rdo.DisPense.MEDI_STOCK_ID);
                    if (medistock != null)
                    {
                        AddObjectKeyIntoListkey(medistock, false);
                    }
                }
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
