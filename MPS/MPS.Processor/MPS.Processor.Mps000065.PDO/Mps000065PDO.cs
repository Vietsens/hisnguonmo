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

namespace MPS.Processor.Mps000065.PDO
{
    public class Mps000065PDO : RDOBase
    { 
        public V_HIS_PATIENT Patient { get; set; }
        public V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        public List<HIS_DEBATE_USER> lstHisDebateUser { get; set; }
        public List<HIS_DEBATE_USER> lstHisDebateUserTGia { get; set; }
        public HIS_DEBATE HisDebateRow { get; set; }
        public string bedRoomName;

        public Mps000065PDO(
            V_HIS_PATIENT patient,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            List<HIS_DEBATE_USER> lstHisDebateUser,
            List<HIS_DEBATE_USER> lstHisDebateUserTGia,
            HIS_DEBATE HisDebateRow,
            string bedRoomName
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.lstHisDebateUser = lstHisDebateUser;
                this.lstHisDebateUserTGia = lstHisDebateUserTGia;
                this.HisDebateRow = HisDebateRow;
                this.bedRoomName = bedRoomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
