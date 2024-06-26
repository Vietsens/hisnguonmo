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

namespace MPS.Processor.Mps000079.PDO
{
  public partial class Mps000079PDO : RDOBase
  {
    public Mps000079PDO(
         PatientADO patientADO,
         PatyAlterBhytADO patyAlterBhytADO,
          string departmentName,
          List<SereServKTCADO> sereServKTCADOs,
          List<V_HIS_SERE_SERV> sereServs,
          List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans,
          V_HIS_TREATMENT currentHisTreatment,

          List<HIS_HEIN_SERVICE_TYPE> ServiceReports,
          List<HIS_EXECUTE_GROUP> executeGroups,
          long totalDay

          )
    {
      try
      {
        this.patientADO = patientADO;        
        this.patyAlterBhytADO = patyAlterBhytADO;
        this.departmentName = departmentName;
        this.sereServKTCADOs = sereServKTCADOs;
        this.sereServs = sereServs;
        this.departmentTrans = departmentTrans;
        this.currentHisTreatment = currentHisTreatment;
        this.executeGroups = executeGroups;
        this.ServiceReports = ServiceReports;
        this.totalDay = totalDay;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }
}
