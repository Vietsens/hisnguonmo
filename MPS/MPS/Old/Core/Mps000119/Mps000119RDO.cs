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

namespace MPS.Core.Mps000119
{
    public class Mps000119RDO : RDOBase
    {
        internal MOS.EFMODEL.DataModels.HIS_BID hisBid { get; set; }
        internal List<ADO.HisBidMetyADO> bidMetyAdos { get; set; }

        public Mps000119RDO(MOS.EFMODEL.DataModels.HIS_BID bid, List<ADO.HisBidMetyADO> bidMety)
        {
            try
            {
                this.bidMetyAdos = bidMety;
                this.hisBid = bid;
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
                if (hisBid != null)
                {
                    keyValues.Add(new KeyValue(Mps000119ExtendSingleKey.BID_NAME, hisBid.BID_NAME));
                    keyValues.Add(new KeyValue(Mps000119ExtendSingleKey.BID_NUMBER, hisBid.BID_NUMBER));
                    
                    if (bidMetyAdos != null)
                    {
                        decimal SumPrice = bidMetyAdos.Sum(o => o.TotalMoney);
                        string sumPriceStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(SumPrice));
                        string sumPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(sumPriceStr);
                        keyValues.Add(new KeyValue(Mps000119ExtendSingleKey.SUM_PRICE, SumPrice));
                        keyValues.Add(new KeyValue(Mps000119ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.UppercaseFirst(sumPriceText)));
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
