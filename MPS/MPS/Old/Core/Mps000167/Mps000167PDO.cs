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

namespace MPS.Core.Mps000167
{
    public class Mps000167PDO : RDOBase
    {
        internal string PATIENT_CODE;
        HIS_PATY_ALTER_BHYT _PatyAlterBhyt = null;
        V_HIS_PAAN_SERVICE_REQ _PaanServiceReq = null;
        V_HIS_SERE_SERV_5 _SereServ = null;

        public Mps000167PDO(V_HIS_PAAN_SERVICE_REQ paanServiceReq, V_HIS_SERE_SERV_5 sereServ, HIS_PATY_ALTER_BHYT patyAlterBhyt)
        {
            this._PatyAlterBhyt = patyAlterBhyt;
            this._SereServ = sereServ;
            this._PaanServiceReq = paanServiceReq;
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (this._PaanServiceReq != null)
                {
                    this.PATIENT_CODE = this._PaanServiceReq.PATIENT_CODE;
                    if (this._PaanServiceReq.IS_SURGERY == (short)1)
                    {
                        keyValues.Add(new KeyValue(Mps000167ExtendSingleKey.IS_SURGERY, "x"));
                    }
                    else
                    {
                        keyValues.Add(new KeyValue(Mps000167ExtendSingleKey.IS_NOT_SURGERY, "x"));
                    }
                    keyValues.Add(new KeyValue(Mps000167ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(this._PaanServiceReq.DOB)));
                    keyValues.Add(new KeyValue(Mps000167ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this._PaanServiceReq.INTRUCTION_TIME)));
                    keyValues.Add(new KeyValue(Mps000167ExtendSingleKey.LIQUID_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this._PaanServiceReq.LIQUID_TIME ?? 0)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PAAN_SERVICE_REQ>(this._PaanServiceReq, keyValues, false);
                }
                if (this._SereServ != null)
                {
                    keyValues.Add(new KeyValue(Mps000167ExtendSingleKey.SERVICE_CODE, this._SereServ.SERVICE_CODE));
                    keyValues.Add(new KeyValue(Mps000167ExtendSingleKey.SERVICE_NAME, this._SereServ.SERVICE_NAME));
                }
                if (this._PatyAlterBhyt != null)
                {
                    keyValues.Add(new KeyValue(Mps000167ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._PatyAlterBhyt.HEIN_CARD_FROM_TIME)));
                    keyValues.Add(new KeyValue(Mps000167ExtendSingleKey.HEIN_CARD_TO_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._PatyAlterBhyt.HEIN_CARD_TO_TIME)));
                    GlobalQuery.AddObjectKeyIntoListkey<HIS_PATY_ALTER_BHYT>(this._PatyAlterBhyt, keyValues, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
