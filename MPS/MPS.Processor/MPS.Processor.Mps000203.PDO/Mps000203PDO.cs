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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000203.PDO
{
    public partial class Mps000203PDO : RDOBase
    {
        public List<V_HIS_EXP_MEST_BLOOD> _Bloods = null;

        public long expMesttSttId__Draft = 1;// trạng thái nháp
        public long expMesttSttId__Request = 2;// trạng thái yêu cầu
        public long expMesttSttId__Reject = 3;// không duyệt
        public long expMesttSttId__Approval = 4; // duyệt
        public long expMesttSttId__Export = 5;// đã xuất
        public List<HIS_BLOOD_GIVER> listBloodGiver = null;// danh sách người hiến máu
        public Mps000203PDO() { }

        public Mps000203PDO(V_HIS_EXP_MEST ExpMest, List<V_HIS_EXP_MEST_BLOOD> listBlood)
        {
            this._ExpMest = ExpMest;
            this._Bloods = listBlood;        
        }

        public Mps000203PDO(V_HIS_EXP_MEST ExpMest, List<V_HIS_EXP_MEST_BLOOD> listBlood, Mps000203ADO mps000203ADO)
        {
            this._ExpMest = ExpMest;
            this._Bloods = listBlood;
            this._mps000203ADO = mps000203ADO;
        }

        public Mps000203PDO(V_HIS_EXP_MEST ExpMest, List<V_HIS_EXP_MEST_BLOOD> listBlood, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export)
        {
            this._ExpMest = ExpMest;
            this._Bloods = listBlood;
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
        }

        public Mps000203PDO(V_HIS_EXP_MEST ExpMest, List<V_HIS_EXP_MEST_BLOOD> listBlood, Mps000203ADO mps000203ADO, List<HIS_BLOOD_GIVER> listBloodGiver)
        {
            this._ExpMest = ExpMest;
            this._Bloods = listBlood;
            this._mps000203ADO = mps000203ADO;
            this.listBloodGiver = listBloodGiver;
        }
    }
}
