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
using MPS.Processor.Mps000230.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000230
{
    public class Mps000230Processor : AbstractProcessor
    {
        Mps000230PDO rdo;
        public Mps000230Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000230PDO)rdoBase;
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
                objectTag.AddObjectData(store, "mediMaties1", rdo._ListAdo);
                objectTag.AddObjectData(store, "mediMaties2", rdo._ListAdo);
                objectTag.AddObjectData(store, "mediMaties3", rdo._ListAdo);
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
                        rdo._ListAdo.Add(new MPS.Processor.Mps000230.PDO.Mps000230PDO.Mps000230ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.VAT_RATIO ?? 0));
                    }
                }
                if (rdo._ImpMestMaterials != null && rdo._ImpMestMaterials.Count > 0)
                {
                    foreach (var item in rdo._ImpMestMaterials)
                    {
                        rdo._ListAdo.Add(new MPS.Processor.Mps000230.PDO.Mps000230PDO.Mps000230ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.VAT_RATIO ?? 0));
                    }
                }

                if (rdo._ImpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000230ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ImpMest.IMP_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000230ExtendSingleKey.DOCUMENT_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ImpMest.DOCUMENT_DATE ?? 0)));
                    SetSingleKey(new KeyValue(Mps000230ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000230ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000230ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000230ExtendSingleKey.SUM_PRICE, Inventec.Common.Number.Convert.NumberToString(sumPrice)));
                    string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumPrice));
                    SetSingleKey(new KeyValue(Mps000230ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                    decimal sumAfterDiscount = sumPrice - rdo._ImpMest.DISCOUNT ?? 0;
                    SetSingleKey(new KeyValue(Mps000230ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT, sumAfterDiscount));
                    string sumAfterDiscountString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumAfterDiscount));
                    SetSingleKey(new KeyValue(Mps000230ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumAfterDiscountString)));

                    AddObjectKeyIntoListkey(rdo._ImpMest, false);
                    AddObjectKeyIntoListkey(rdo._ExpMest, true);
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
                log = LogDataImpMest(rdo._ImpMest.TDL_TREATMENT_CODE, rdo._ImpMest.IMP_MEST_CODE, "");
                log += string.Format("Kho: {0}, Phòng yêu cầu: {1}", rdo._ImpMest.MEDI_STOCK_NAME, rdo._ImpMest.REQ_ROOM_ID);
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
                if (rdo != null && rdo._ImpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}", printTypeCode, rdo._ImpMest.IMP_MEST_CODE, rdo._ImpMest.MEDI_STOCK_CODE, rdo._ImpMestMedicines.Count, rdo._ImpMestMaterials.Count);
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
