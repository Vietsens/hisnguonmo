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

namespace MPS.Core.Mps000145
{
    public class Mps000145RDO : RDOBase
    {
        V_HIS_CHMS_IMP_MEST _ChmsImpMest = null;
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines = null;
        internal List<Mps000145ADO> _ListAdo = null;

        public Mps000145RDO(V_HIS_CHMS_IMP_MEST chmsImpMest, List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines)
        {
            this._ChmsImpMest = chmsImpMest;
            this._ImpMestMedicines = impMestMedicines;
            this._ListAdo = new List<Mps000145ADO>();
        }

        internal override void SetSingleKey()
        {
            try
            {
                decimal sumPrice = 0;
                if (this._ImpMestMedicines != null && this._ImpMestMedicines.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần của id
                    this._ImpMestMedicines = this._ImpMestMedicines.OrderBy(o => o.ID).ToList();
                    foreach (var item in this._ImpMestMedicines)
                    {
                        if (item.IS_NEUROLOGICAL != 1 && item.IS_ADDICTIVE != 1)
                        {
                            this._ListAdo.Add(new Mps000145ADO(item));
                            sumPrice += item.AMOUNT * (item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0));
                        }
                    }
                }

                if (this._ChmsImpMest != null)
                {
                    keyValues.Add(new KeyValue(Mps000145ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ChmsImpMest.IMP_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000145ExtendSingleKey.APPROVAL_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this._ChmsImpMest.APPROVAL_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000145ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ChmsImpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000145ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ChmsImpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000145ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._ChmsImpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000145ExtendSingleKey.SUM_PRICE, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumPrice)));
                    string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumPrice));
                    keyValues.Add(new KeyValue(Mps000145ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                    GlobalQuery.AddObjectKeyIntoListkey(this._ChmsImpMest, this.keyValues, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
