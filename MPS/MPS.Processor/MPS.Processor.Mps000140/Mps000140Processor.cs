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
using MPS.Processor.Mps000140.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000140
{
    public class Mps000140Processor : AbstractProcessor
    {
        Mps000140PDO rdo;
        public Mps000140Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000140PDO)rdoBase;
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
                //objectTag.AddObjectData(store, "ListBlood", rdo.listAdo);
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
                if (rdo._ImpMestBloods != null && rdo._ImpMestBloods.Count > 0)
                {
                    foreach (var item in rdo._ImpMestBloods)
                    {
                        rdo._ListAdo.Add(new Mps000140ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.PRICE.Value;
                    }
                }

                if (rdo._ManuImpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000140ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ManuImpMest.IMP_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000140ExtendSingleKey.DOCUMENT_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ManuImpMest.DOCUMENT_DATE ?? 0)));
                    SetSingleKey(new KeyValue(Mps000140ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ManuImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000140ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ManuImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000140ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._ManuImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000140ExtendSingleKey.SUM_PRICE, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumPrice)));
                    string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumPrice));
                    SetSingleKey(new KeyValue(Mps000140ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                    decimal sumAfterDiscount = sumPrice - rdo._ManuImpMest.DISCOUNT ?? 0;
                    SetSingleKey(new KeyValue(Mps000140ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT, sumAfterDiscount));
                    string sumAfterDiscountString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumAfterDiscount));
                    SetSingleKey(new KeyValue(Mps000140ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumAfterDiscountString)));

                    AddObjectKeyIntoListkey(rdo._ManuImpMest, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
