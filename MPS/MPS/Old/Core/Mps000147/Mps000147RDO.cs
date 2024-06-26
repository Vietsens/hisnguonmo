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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000147
{
    public class Mps000147RDO : RDOBase
    {
        V_HIS_BILL _Bill = null;
        List<V_HIS_SERE_SERV_5> _ListSereServ = null;

        public Mps000147RDO(V_HIS_BILL bill,List<V_HIS_SERE_SERV_5> listSereServ)
        {
            this._Bill = bill;
            this._ListSereServ = listSereServ;
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (this._Bill != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_BILL>(this._Bill, keyValues, false);
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this._Bill.AMOUNT));
                    keyValues.Add(new KeyValue(Mps000147ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(amountStr)));
                }

                if (this._ListSereServ != null && this._ListSereServ.Count > 0)
                {
                    var listServiceType = this._ListSereServ.Select(s => s.SERVICE_TYPE_NAME).Distinct().ToList();
                    keyValues.Add(new KeyValue(Mps000147ExtendSingleKey.SERVICE_TYPE_NAMEs, String.Join(",", listServiceType)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
