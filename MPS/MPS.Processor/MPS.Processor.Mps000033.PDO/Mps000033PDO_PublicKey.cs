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

namespace MPS.Processor.Mps000033.PDO
{
  public partial class Mps000033PDO : RDOBase
  {
    public PatientADO Patient { get; set; }
    public V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
    public V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
    public V_HIS_SERE_SERV_PTTT sereServsPttt { get; set; }
    public V_HIS_TREATMENT treatment { get; set; }
    public string departmentName { get; set; }
    public List<V_HIS_EKIP_USER> ekipUsers { get; set; }
    public HisExecuteRoleCFGPrint executeRoleCFG { get; set; }
    public V_HIS_SERE_SERV_5 sereServ { get; set; }
    public HIS_SERE_SERV_EXT SereServExt { get; set; }
    public V_HIS_BED_LOG BedLog { get; set; }
    public V_HIS_BED_LOG LastBedLog { get; set; }
  }

  public class PatientADO : MOS.EFMODEL.DataModels.V_HIS_PATIENT
  {
    public string AGE { get; set; }
    public string DOB_STR { get; set; }
    public string CMND_DATE_STR { get; set; }
    public string DOB_YEAR { get; set; }
    public string GENDER_MALE { get; set; }
    public string GENDER_FEMALE { get; set; }

    public PatientADO()
    {

    }

    public PatientADO(V_HIS_PATIENT data)
    {
      try
      {
        if (data != null)
        {
          System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_PATIENT>();
          foreach (var item in pi)
          {
            item.SetValue(this, item.GetValue(data));
          }

          this.AGE = AgeUtil.CalculateFullAge(this.DOB);
          this.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.DOB);
          string temp = this.DOB.ToString();
          if (temp != null && temp.Length >= 8)
          {
            this.DOB_YEAR = temp.Substring(0, 4);
          }
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }

  public class HisExecuteRoleCFGPrint
  {
    public long EXECUTE_ROLE_ID__MAIN { get; set; }
    public long EXECUTE_ROLE_ID__TT { get; set; }
    public long EXECUTE_ROLE_ID__PM1 { get; set; }
    public long EXECUTE_ROLE_ID__PM2 { get; set; }
    public long EXECUTE_ROLE_ID__PME1 { get; set; }
    public long EXECUTE_ROLE_ID__PME2 { get; set; }
    public long EXECUTE_ROLE_ID__GMHS { get; set; }
    public long EXECUTE_ROLE_ID__GV { get; set; }
  }
}
