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
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000149.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000149
{
    public class Mps000149Processor : AbstractProcessor
    {
        Mps000149PDO rdo;
        public Mps000149Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000149PDO)rdoBase;
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
                objectTag.AddObjectData(store, "Bloods", rdo._ListAdo);
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
                decimal totalPrice = 0;
                if (rdo._ListImpMestBlood != null && rdo._ListImpMestBlood.Count > 0)
                {
                    rdo._ListAdo = (from r in rdo._ListImpMestBlood select new MPS.Processor.Mps000149.PDO.Mps000149PDO.Mps000149ADO(r)).ToList();
                    totalPrice = rdo._ListImpMestBlood.Sum(s => ((s.PRICE ?? 0) * (1 + s.VAT_RATIO ?? 0)));
                    SetSingleKey(new KeyValue(Mps000149ExtendSingleKey.TOTAL_PRICE, totalPrice));
                    SetSingleKey(new KeyValue(Mps000149ExtendSingleKey.TOTAL_PRICE_SEPARATE, Inventec.Common.Number.Convert.NumberToString(totalPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                    string totalPriceStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    SetSingleKey(new KeyValue(Mps000149ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(totalPriceStr)));
                }
                if (rdo._ManuImpMest != null)
                {
                    if (rdo._ManuImpMest.DOCUMENT_DATE.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000149ExtendSingleKey.DOCUMENT_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ManuImpMest.DOCUMENT_DATE.Value)));
                    }
                    var totalPriceDiscount = totalPrice - (rdo._ManuImpMest.DISCOUNT ?? 0);
                    SetSingleKey(new KeyValue(Mps000149ExtendSingleKey.TOTAL_PRICE_AFTER_DISCOUNT, totalPrice));
                    SetSingleKey(new KeyValue(Mps000149ExtendSingleKey.TOTAL_PRICE_AFTER_DISCOUNT, Inventec.Common.Number.Convert.NumberToStringRoundMax4(totalPriceDiscount)));
                    string totalPriceDiscountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPriceDiscount));
                    SetSingleKey(new KeyValue(Mps000149ExtendSingleKey.TOTAL_PRICE_AFTER_DISCOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(totalPriceDiscountStr)));
                    AddObjectKeyIntoListkey<V_HIS_IMP_MEST>(rdo._ManuImpMest, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
