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
using MOS.EFMODEL.DataModels;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000140
{
    public class Mps000140RDO : RDOBase
    {
        V_HIS_MANU_IMP_MEST _ManuImpMest = null;
        List<V_HIS_IMP_MEST_BLOOD> _ImpMestBloods = null;
        public List<Mps000140ADO> _ListAdo = null;

        public Mps000140RDO(V_HIS_MANU_IMP_MEST manuImpMest, List<V_HIS_IMP_MEST_BLOOD> impMestMedicines, List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials)
        {
            this._ManuImpMest = manuImpMest;
            this._ImpMestBloods = impMestMedicines;
            this._ListAdo = new List<Mps000140ADO>();
        }

        internal override void SetSingleKey()
        {
            try
            {
                decimal sumPrice = 0;
                if (this._ImpMestBloods != null && this._ImpMestBloods.Count > 0)
                {
                    foreach (var item in this._ImpMestBloods)
                    {
                        this._ListAdo.Add(new Mps000140ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.PRICE.Value;
                    }
                }

                if (this._ManuImpMest != null)
                {
                    keyValues.Add(new KeyValue(Mps000140ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ManuImpMest.IMP_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000140ExtendSingleKey.DOCUMENT_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ManuImpMest.DOCUMENT_DATE ?? 0)));
                    keyValues.Add(new KeyValue(Mps000140ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ManuImpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000140ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ManuImpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000140ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._ManuImpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000140ExtendSingleKey.SUM_PRICE, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumPrice)));
                    string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumPrice));
                    keyValues.Add(new KeyValue(Mps000140ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                    decimal sumAfterDiscount = sumPrice - this._ManuImpMest.DISCOUNT ?? 0;
                    keyValues.Add(new KeyValue(Mps000140ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT, sumAfterDiscount));
                    string sumAfterDiscountString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumAfterDiscount));
                    keyValues.Add(new KeyValue(Mps000140ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumAfterDiscountString)));

                    GlobalQuery.AddObjectKeyIntoListkey(this._ManuImpMest, this.keyValues, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
