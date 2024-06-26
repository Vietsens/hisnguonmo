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

namespace MPS.Core.Mps000149
{
    public class Mps000149RDO : RDOBase
    {
        V_HIS_MANU_IMP_MEST _ManuImpMest = null;
        List<V_HIS_IMP_MEST_BLOOD> _ListImpMestBlood = null;

        internal List<Mps000149ADO> _ListAdo = new List<Mps000149ADO>();

        public Mps000149RDO(V_HIS_MANU_IMP_MEST manuImpMest, List<V_HIS_IMP_MEST_BLOOD> listImpMestBlood)
        {
            this._ManuImpMest = manuImpMest;
            this._ListImpMestBlood = listImpMestBlood;
        }

        internal override void SetSingleKey()
        {
            try
            {
                decimal totalPrice = 0;
                if (this._ListImpMestBlood != null && this._ListImpMestBlood.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần id
                    this._ListImpMestBlood = this._ListImpMestBlood.OrderBy(p => p.ID).ToList();
                    this._ListAdo = (from r in this._ListImpMestBlood select new Mps000149ADO(r)).ToList();
                    totalPrice = this._ListImpMestBlood.Sum(s => s.IMP_PRICE);
                    string totalPriceStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    keyValues.Add(new KeyValue(Mps000149ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(totalPriceStr)));
                }
                if (this._ManuImpMest != null)
                {
                    if (this._ManuImpMest.DOCUMENT_DATE.HasValue)
                    {
                        keyValues.Add(new KeyValue(Mps000149ExtendSingleKey.DOCUMENT_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ManuImpMest.DOCUMENT_DATE.Value)));
                    }
                    var totalPriceDiscount = totalPrice - (this._ManuImpMest.DISCOUNT ?? 0);
                    keyValues.Add(new KeyValue(Mps000149ExtendSingleKey.TOTAL_PRICE_AFTER_DISCOUNT, Inventec.Common.Number.Convert.NumberToStringRoundMax4(totalPriceDiscount)));
                    string totalPriceDiscountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPriceDiscount));
                    keyValues.Add(new KeyValue(Mps000149ExtendSingleKey.TOTAL_PRICE_AFTER_DISCOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(totalPriceDiscountStr)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_MANU_IMP_MEST>(this._ManuImpMest, keyValues, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
