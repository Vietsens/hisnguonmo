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

namespace MPS.Processor.Mps000387.PDO
{
    public class Mps000387PDO : RDOBase
    {
        public V_HIS_DEBATE CurrentHisDebate { get; set; }
        public V_HIS_DEPARTMENT_TRAN DepartmentTran { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        public V_HIS_TREATMENT Treatment { get; set; }
        public List<HIS_DEBATE_USER> LstHisDebateUser { get; set; }
        public List<V_HIS_DEBATE_EKIP_USER> LstHisDebateEkipUser { get; set; }
        public Mps000387SingleKey SingleKey { get; set; }

        public Mps000387PDO(
            V_HIS_TREATMENT treatment,
            V_HIS_DEBATE currentHisDebate,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            List<HIS_DEBATE_USER> lstHisDebateUser,
            List<V_HIS_DEBATE_EKIP_USER> lstHisDebateEkipUser,
            Mps000387SingleKey singleKey
            )
        {
            try
            {
                this.CurrentHisDebate = currentHisDebate;
                this.DepartmentTran = departmentTran;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.Treatment = treatment;
                this.LstHisDebateUser = lstHisDebateUser;
                this.LstHisDebateEkipUser = lstHisDebateEkipUser;
                this.SingleKey = singleKey;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public class Mps000387SingleKey
        {
            public string DEPARTMENT_NAME { get; set; }
            public string BEB_ROOM_NAME { get; set; }
            public string BED_NAME { get; set; }
            public string BED_CODE { get; set; }
        }
    }
}
