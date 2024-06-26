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
using Inventec.Core;
using MPS.Processor.Mps000145.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000145
{
    public class Mps000145Processor : AbstractProcessor
    {
        Mps000145PDO rdo;
        public Mps000145Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000145PDO)rdoBase;
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

                singleTag.ProcessData(store, singleValueDictionary);
                if (rdo._ListAdo != null && rdo._ListAdo.Count > 0)
                {
                    rdo._ListAdo = rdo._ListAdo.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                }
                objectTag.AddObjectData(store, "MediMaties1", rdo._ListAdo);
                objectTag.AddObjectData(store, "MediMaties2", rdo._ListAdo);
                objectTag.AddObjectData(store, "MediMaties3", rdo._ListAdo);
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
                decimal sumPrice = 0;
                if (rdo._ImpMestMedicines != null && rdo._ImpMestMedicines.Count > 0)
                {
                    foreach (var item in rdo._ImpMestMedicines)
                    {
                        if (item.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && item.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN)
                        {
                            rdo._ListAdo.Add(new MPS.Processor.Mps000145.PDO.Mps000145PDO.Mps000145ADO(item));
                            sumPrice += item.AMOUNT * (item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0));
                        }
                    }
                }
                if (rdo._ImpMestMaterials != null && rdo._ImpMestMaterials.Count > 0)
                {
                    foreach (var item in rdo._ImpMestMaterials)
                    {
                        rdo._ListAdo.Add(new MPS.Processor.Mps000145.PDO.Mps000145PDO.Mps000145ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.VAT_RATIO ?? 0));
                    }
                }

                if (rdo._ChmsImpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000145ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ChmsImpMest.IMP_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000145ExtendSingleKey.APPROVAL_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo._ChmsImpMest.APPROVAL_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000145ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ChmsImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000145ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ChmsImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000145ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._ChmsImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000145ExtendSingleKey.SUM_PRICE, Inventec.Common.Number.Convert.NumberToString(sumPrice)));
                    string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToString(sumPrice));
                    SetSingleKey(new KeyValue(Mps000145ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                    AddObjectKeyIntoListkey(rdo._ChmsImpMest, false);
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
    }
}
