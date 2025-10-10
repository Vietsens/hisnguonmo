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
using MPS.Processor.Mps000143.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000143
{
    public class Mps000143Processor : AbstractProcessor
    {
        Mps000143PDO rdo;
        public Mps000143Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000143PDO)rdoBase;
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

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                var groupedByOtherPaySource = rdo._ListAdo
                                                .GroupBy(o => o.OTHER_PAY_SOURCE_ID ?? 0)
                                                .ToList();

                var allListAdo = new List<Mps000143PDO.Mps000143ADO>();
                var allMedicineGroups = new List<Mps000143PDO.Mps000143ADO>();
                var allParents = new List<Mps000143PDO.Mps000143ADO>();

                foreach (var group in groupedByOtherPaySource)
                {
                    var paySourceId = group.Key;
                    if(rdo._MedicineTypes?.FirstOrDefault(m => m.OTHER_PAY_SOURCE_ID == paySourceId) != null) { 
                    }
                    var paySourceType = rdo._MedicineTypes?.FirstOrDefault(m => m.OTHER_PAY_SOURCE_ID == paySourceId);

                    //?? rdo._MaterialTypes?.FirstOrDefault(m => m.OTHER_PAY_SOURCE_ID == paySourceId);
                    string paySourceName = paySourceType?.OTHER_PAY_SOURCE_NAME ?? "Không xác định";


                    var listAdo = group.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                    foreach (var item in listAdo)
                    {
                        item.OTHER_PAY_SOURCE_NAME = paySourceName;
                    }

                    var groupMedicineGroups = listAdo
                        .Where(o => o.MEDICINE_GROUP_ID.HasValue)
                        .GroupBy(o => new { o.MEDICINE_GROUP_ID, o.MEDICINE_GROUP_CODE, o.MEDICINE_GROUP_NAME })
                        .Select(g => g.First())
                        .ToList();
                    var groupParents = listAdo
                        .GroupBy(o => new { o.PARENT_ID, o.PARENT_CODE, o.PARENT_NAME })
                        .Select(g => g.First())
                        .ToList();

                    allListAdo.AddRange(listAdo);
                    allMedicineGroups.AddRange(groupMedicineGroups);
                    allParents.AddRange(groupParents);
                }

                objectTag.AddObjectData(store, "MediMaties1", allListAdo);
                objectTag.AddObjectData(store, "MedicineGroups", allMedicineGroups);
                objectTag.AddObjectData(store, "Parents", allParents);
                objectTag.AddObjectData(store, "OtherPaySource", groupedByOtherPaySource);

                objectTag.AddRelationship(store, "MedicineGroups", "MediMaties1", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "Parents", "MediMaties1", "PARENT_ID", "PARENT_ID");

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.SetUserFunction(store, "FuncMergeData", new CalculateMergerData());
                result = true;

                //singleTag.ProcessData(store, singleValueDictionary);
                //if (rdo._ListAdo != null && rdo._ListAdo.Count > 0)
                //{
                //    rdo._ListAdo = rdo._ListAdo.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                //}
                //objectTag.AddObjectData(store, "MediMaties1", rdo._ListAdo);
                //objectTag.AddObjectData(store, "MediMaties2", rdo._ListAdo);
                //objectTag.AddObjectData(store, "MediMaties3", rdo._ListAdo);
                //result = true;
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
                decimal sumPrice = 0;
                if (rdo._ImpMestMedicines != null && rdo._ImpMestMedicines.Count > 0)
                {
                    foreach (var item in rdo._ImpMestMedicines)
                    {
                        rdo._ListAdo.Add(new MPS.Processor.Mps000143.PDO.Mps000143PDO.Mps000143ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.VAT_RATIO ?? 0));
                    }
                }
                if (rdo._ImpMestMaterials != null && rdo._ImpMestMaterials.Count > 0)
                {
                    foreach (var item in rdo._ImpMestMaterials)
                    {
                        rdo._ListAdo.Add(new MPS.Processor.Mps000143.PDO.Mps000143PDO.Mps000143ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.VAT_RATIO ?? 0));
                    }
                }

                if (rdo._ChmsImpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000143ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ChmsImpMest.IMP_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000143ExtendSingleKey.APPROVAL_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo._ChmsImpMest.APPROVAL_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000143ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ChmsImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000143ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ChmsImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000143ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._ChmsImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000143ExtendSingleKey.SUM_PRICE, Inventec.Common.Number.Convert.NumberToString(sumPrice, rdo.NumberDigit)));
                    string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToString(sumPrice, rdo.NumberDigit));
                    SetSingleKey(new KeyValue(Mps000143ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                    AddObjectKeyIntoListkey<V_HIS_IMP_MEST>(rdo._ChmsImpMest, false);
                }

                if (rdo._mps000143Key != null)
                {
                    AddObjectKeyIntoListkey<MPS.Processor.Mps000143.PDO.Mps000143PDO.Mps000143Key>(rdo._mps000143Key, false);
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
                if (rdo != null && rdo._ChmsImpMest != null)
                {
                    log = LogDataImpMest("", rdo._ChmsImpMest.IMP_MEST_CODE, rdo._ChmsImpMest.REQ_DEPARTMENT_NAME + "_" + rdo._ChmsImpMest.MEDI_STOCK_NAME);
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
                if (rdo != null && rdo._ChmsImpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}", printTypeCode, rdo._ChmsImpMest.IMP_MEST_CODE, rdo._ChmsImpMest.MEDI_STOCK_CODE, rdo._ImpMestMedicines.Count, rdo._ImpMestMaterials.Count);
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
