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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;
using MPS.Config;

namespace MPS.Core.Mps000033
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000033RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        internal V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        internal V_HIS_SERE_SERV_PTTT sereServsPttt { get; set; }
        internal V_HIS_TREATMENT treatment { get; set; }
        string departmentName { get; set; }
        internal List<V_HIS_EKIP_USER> ekipUsers { get; set; }
        internal HisExecuteRoleCFGPrint executeRoleCFG { get; set; }
        internal V_HIS_SERE_SERV_5 sereServ { get; set; }
        public Mps000033RDO(
            PatientADO patient,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            V_HIS_SERE_SERV_PTTT sereServsPttt
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServsPttt = sereServsPttt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000033RDO(
            PatientADO patient,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            V_HIS_SERE_SERV_5 sereServ,
            V_HIS_SERE_SERV_PTTT sereServsPttt,
            V_HIS_TREATMENT treatment,
            List<V_HIS_EKIP_USER> ekipUsers,
            HisExecuteRoleCFGPrint executeRoleCFG
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServsPttt = sereServsPttt;
                this.treatment = treatment;
                this.departmentName = departmentName;
                this.executeRoleCFG = executeRoleCFG;
                this.ekipUsers = ekipUsers;
                this.sereServ = sereServ;
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
                if (ServiceReqPrint != null)
                {
                    //keyValues.Add(new KeyValue(Mps000033ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.LOCK_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000033ExtendSingleKey.START_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.START_TIME ?? 0)));
                    if (ServiceReqPrint.FINISH_TIME.HasValue)
                        keyValues.Add(new KeyValue(Mps000033ExtendSingleKey.FINISH_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.FINISH_TIME ?? 0)));
                }
                else
                {
                    //keyValues.Add(new KeyValue(Mps000033ExtendSingleKey.OPEN_TIME_SEPARATE_STR, ""));
                    keyValues.Add(new KeyValue(Mps000033ExtendSingleKey.START_TIME_STR, ""));
                }

                if (treatment != null)
                {
                    keyValues.Add(new KeyValue(Mps000033ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.IN_TIME)));
                }

                var ekipUserGroups = ekipUsers.GroupBy(o => o.EXECUTE_ROLE_ID).ToList();
                foreach (var ekipUserGroup in ekipUserGroups)
                {
                    string userName = "";
                    string loginName = "";
                    List<V_HIS_EKIP_USER> ekipUserTemps = ekipUserGroup.ToList();
                    if (ekipUserTemps.Count > 1)
                    {

                        foreach (var ekipUserTemp in ekipUserTemps)
                        {
                            userName += ekipUserTemp.USERNAME + ",";
                            loginName += ekipUserTemp.LOGINNAME + ",";
                        }

                    }
                    else
                    {
                        userName = ekipUserTemps.FirstOrDefault().USERNAME;
                        loginName = ekipUserTemps.FirstOrDefault().LOGINNAME;
                    }

                    keyValues.Add(new KeyValue("USERNAME_EXECUTE_ROLE_" + ekipUserTemps.FirstOrDefault().EXECUTE_ROLE_CODE, userName));
                    keyValues.Add(new KeyValue("LOGIN_NAME_EXECUTE_ROLE_" + ekipUserTemps.FirstOrDefault().EXECUTE_ROLE_CODE, loginName));
                }

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(treatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERE_SERV_PTTT>(sereServsPttt, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERE_SERV_5>(sereServ, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(departmentTran, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReqPrint, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
