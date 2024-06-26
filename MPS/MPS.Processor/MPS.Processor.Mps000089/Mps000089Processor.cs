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
using MPS.Processor.Mps000089.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000089
{
    class Mps000089Processor : AbstractProcessor
    {
        Mps000089PDO rdo;
        public Mps000089Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000089PDO)rdoBase;
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

                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    rdo.listAdo = rdo.listAdo.OrderBy(p => p.MEDICINE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                }

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                objectTag.AddObjectData(store, "MedicineTypes1", rdo.listAdo);
                objectTag.AddObjectData(store, "MedicineTypes2", rdo.listAdo);
                objectTag.AddObjectData(store, "MedicineTypes3", rdo.listAdo);
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

                decimal totalPrice = 0;
                if (this.rdo._ChmsExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000089ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000089ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000089ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey(this.rdo._ChmsExpMest, false);
                }

                if (this.rdo._Medicines != null && this.rdo._Medicines.Count > 0)
                {
                    this.rdo._Medicines = this.rdo._Medicines.OrderBy(o => o.ID).ToList();
                    var Group = this.rdo._Medicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.BID_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID }).ToList();
                    foreach (var group in Group)
                    {
                        dicExpiredMedi.Clear();
                        var listByGroup = group.ToList<V_HIS_EXP_MEST_MEDICINE>();
                        foreach (var item in listByGroup)
                        {
                            if (item.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && item.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                                continue;

                            string key = item.EXPIRED_DATE.HasValue ? item.EXPIRED_DATE.Value.ToString().Substring(0, 8) : "0";
                            if (!dicExpiredMedi.ContainsKey(key))
                                dicExpiredMedi[key] = new List<V_HIS_EXP_MEST_MEDICINE>();
                            dicExpiredMedi[key].Add(item);
                            totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                        }
                        if (dicExpiredMedi.Count > 0)
                        {
                            foreach (var dic in dicExpiredMedi)
                            {
                                rdo.listAdo.Add(new Mps000089ADO(dic.Value));
                            }
                        }
                    }
                }

                SetSingleKey(new KeyValue(Mps000089ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                string sumText = String.Format("0:0.####", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                SetSingleKey(new KeyValue(Mps000089ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumText)));

                rdo.listAdo = rdo.listAdo.OrderByDescending(t => t.NUM_ORDER).ToList();
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
                if (rdo != null && rdo._ChmsExpMest != null)
                {
                    log = LogDataImpMest("", rdo._ChmsExpMest.EXP_MEST_CODE, rdo._ChmsExpMest.REQ_DEPARTMENT_NAME);
                }
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
                int countMedicine = 0;
                string medicineTypeCode = "";
                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    countMedicine = rdo.listAdo.Count;
                    medicineTypeCode = rdo.listAdo.FirstOrDefault().MEDICINE_TYPE_CODE;
                }

                if (rdo != null && rdo._ChmsExpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}", printTypeCode, rdo._ChmsExpMest.EXP_MEST_CODE, rdo._ChmsExpMest.MEDI_STOCK_CODE, countMedicine, medicineTypeCode);
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
        long medicineTypeId = 0;

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
            bool result = false;
            try
            {
                long mediId = Convert.ToInt64(parameters[0]);

                if (mediId > 0)
                {
                    if (this.medicineTypeId == mediId)
                    {
                        return true;
                    }
                    else
                    {
                        this.medicineTypeId = mediId;
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
