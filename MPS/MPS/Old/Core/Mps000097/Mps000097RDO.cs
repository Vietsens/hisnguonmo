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

namespace MPS.Core.Mps000097
{
    public class Mps000097RDO : RDOBase
    {
        //internal List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTUS> listVhisSereServ { get; set; }
        internal List<UserInfoPTTTADO> listVhisSereServ { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ vhisServiceReq { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT vHisSereServPttt { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_SERE_SERV HisSereServ { get; set; }
        internal List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> executeRole { get; set; }
        internal List<V_HIS_EKIP_USER> ekipUsers { get; set; }
        internal PatientADO patient { get; set; }
            internal V_HIS_TREATMENT vHisTreatment { get; set; }

        public Mps000097RDO(
            PatientADO patient,
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

        internal override void SetSingleKey()
        {
            try
            {
                System.DateTime now = System.DateTime.Now;
                keyValues.Add(new KeyValue(Mps000097ExtendSingleKey.NOW_TIME, Inventec.Common.DateTime.Convert.SystemDateTimeToDateSeparateString(now)));
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(vHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patient, keyValues);

                if (vHisSereServPttt != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT>(vHisSereServPttt, keyValues);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
