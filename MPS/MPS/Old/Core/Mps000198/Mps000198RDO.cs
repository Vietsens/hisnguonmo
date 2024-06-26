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

namespace MPS.Core.Mps000198
{
    public class Mps000198RDO : RDOBase
    {
        V_HIS_CHMS_EXP_MEST _ChmsExpMest;
        internal List<V_HIS_EXP_MEST_BLTY> _Bloods = null;

        long expMesttSttId__Draft = 1;// trạng thái nháp
        long expMesttSttId__Request = 2;// trạng thái yêu cầu
        long expMesttSttId__Reject = 3;// không duyệt
        long expMesttSttId__Approval = 4; // duyệt
        long expMesttSttId__Export = 5;// đã xuất

        public Mps000198RDO(V_HIS_CHMS_EXP_MEST chmsExpMest, List<V_HIS_EXP_MEST_BLTY> listBlood)
        {
            this._ChmsExpMest = chmsExpMest;
            this._Bloods = listBlood;
        }

        public Mps000198RDO(V_HIS_CHMS_EXP_MEST chmsExpMest, List<V_HIS_EXP_MEST_BLTY> listBlood, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export)
        {
            this._ChmsExpMest = chmsExpMest;
            this._Bloods = listBlood;         
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
        }

        internal override void SetSingleKey()
        {
            try
            {
                Dictionary<string, List<V_HIS_EXP_MEST_BLTY>> dicExpiredBlood = new Dictionary<string, 
List<V_HIS_EXP_MEST_BLTY>>();

                if (this._ChmsExpMest != null)
                {
                    keyValues.Add(new KeyValue(Mps000198ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ChmsExpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000198ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ChmsExpMest.CREATE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000198ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this._ChmsExpMest.CREATE_TIME ?? 0)));
                    GlobalQuery.AddObjectKeyIntoListkey(this._ChmsExpMest, keyValues, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
