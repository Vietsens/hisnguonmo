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

namespace MPS.Processor.Mps000097.PDO
{
    public class Mps000097PDO : RDOBase
    {
        public List<UserInfoPTTTADO> listVhisSereServ { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ vhisServiceReq { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT vHisSereServPttt { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_SERE_SERV HisSereServ { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> executeRole { get; set; }
        public List<V_HIS_EKIP_USER> ekipUsers { get; set; }
        public V_HIS_PATIENT patient { get; set; }
        public V_HIS_TREATMENT vHisTreatment { get; set; }

        public Mps000097PDO() { }

        public Mps000097PDO(
            V_HIS_PATIENT patient,
            MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT vHisSereServPttt,
            List<V_HIS_EKIP_USER> ekipUsers,
            V_HIS_TREATMENT vHisTreatment
            )
        {
            this.patient = patient;
            this.vHisSereServPttt = vHisSereServPttt;
            this.ekipUsers = ekipUsers;
            this.vHisSereServPttt = vHisSereServPttt;
        }
    }
    public class UserInfoPTTTADO
    {
        public string LOGIN_NAME { get; set; }
        public string EXECUTE_ROLE_NAME { get; set; }
    }
}
