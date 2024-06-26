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
using MPS.Processor.Mps000221.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000221
{
    public class Mps000221Processor : AbstractProcessor
    {
        Mps000221PDO rdo;
        public Mps000221Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000221PDO)rdoBase;
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
                        rdo._ListAdo.Add(new MPS.Processor.Mps000221.PDO.Mps000221PDO.Mps000221ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.VAT_RATIO ?? 0));
                    }
                }
                if (rdo._ImpMestMaterials != null && rdo._ImpMestMaterials.Count > 0)
                {
                    foreach (var item in rdo._ImpMestMaterials)
                    {
                        rdo._ListAdo.Add(new MPS.Processor.Mps000221.PDO.Mps000221PDO.Mps000221ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.VAT_RATIO ?? 0));
                    }
                }

                if (rdo._ImpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000221ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ImpMest.IMP_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000221ExtendSingleKey.DOCUMENT_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ImpMest.DOCUMENT_DATE ?? 0)));
                    SetSingleKey(new KeyValue(Mps000221ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000221ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000221ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000221ExtendSingleKey.SUM_PRICE, Inventec.Common.Number.Convert.NumberToString(sumPrice)));
                    string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumPrice));
                    SetSingleKey(new KeyValue(Mps000221ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                    decimal sumAfterDiscount = sumPrice - rdo._ImpMest.DISCOUNT ?? 0;
                    SetSingleKey(new KeyValue(Mps000221ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT, sumAfterDiscount));
                    string sumAfterDiscountString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumAfterDiscount));
                    SetSingleKey(new KeyValue(Mps000221ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumAfterDiscountString)));

                    AddObjectKeyIntoListkey(rdo._ImpMest, false);
                    AddObjectKeyIntoListkey(rdo._ExpMest, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
