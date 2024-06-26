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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;

namespace MPS.Core.Mps000117
{
    public class Mps000117RDO: RDOBase
    {
        internal V_HIS_ANTICIPATE HisAnticipate { get; set; }
        internal List<ADO.HisAnticipateMetyADO> HisAnticipateMetyAdo { get; set; }

        public Mps000117RDO(V_HIS_ANTICIPATE Anticipates, List<ADO.HisAnticipateMetyADO> AnticipateMetyAdo)
        {
            try
            {
                this.HisAnticipate = Anticipates;
                this.HisAnticipateMetyAdo = AnticipateMetyAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (HisAnticipate != null)
                {
                    keyValues.Add(new KeyValue(Mps000117ExtendSingleKey.ANTICIPATE_CODE, HisAnticipate.ANTICIPATE_CODE));
                    keyValues.Add(new KeyValue(Mps000117ExtendSingleKey.USE_TIME, HisAnticipate.USE_TIME));
                    keyValues.Add(new KeyValue(Mps000117ExtendSingleKey.REQUEST_USERNAME, HisAnticipate.REQUEST_USERNAME));

                    if (HisAnticipateMetyAdo != null)
                    {
                        decimal SumPrice = HisAnticipateMetyAdo.Sum(o => o.TotalMoney);
                        string sumPriceStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(SumPrice));
                        string sumPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(sumPriceStr);
                        keyValues.Add(new KeyValue(Mps000117ExtendSingleKey.SUM_PRICE, SumPrice));
                        keyValues.Add(new KeyValue(Mps000117ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.UppercaseFirst(sumPriceText)));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
