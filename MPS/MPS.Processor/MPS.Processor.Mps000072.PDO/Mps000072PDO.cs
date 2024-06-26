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

namespace MPS.Processor.Mps000072.PDO
{
  public partial class Mps000072PDO : RDOBase
  {
    public Mps000072PDO(
            string departmentName,
            string roomName,
            PatientADO currentPatient,
            TreatmentADO currentTreatment,
            List<HIS_DHST> lstDHST,
            List<Mps000062ADO> mps000062ADOs,
            List<Mps000062ADOMedicines> lstMps000062ADOMedicines,
            List<Mps000062ADOServiceCLS> lstMps000062ADOServiceCLS
            )
    {
      try
      {
        this.departmentName = departmentName;
        this.roomName = roomName;
        this.currentPatient = currentPatient;
        this.currentTreatment = currentTreatment;
        this.lstDHST = lstDHST;
        this.mps000062ADOs = mps000062ADOs;
        this.lstMps000062ADOMedicines = lstMps000062ADOMedicines;
        this.lstMps000062ADOServiceCLS = lstMps000062ADOServiceCLS;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    public Mps000072PDO(
        string departmentName,
        string roomName,
        PatientADO currentPatient,
        TreatmentADO currentTreatment,
        List<HIS_DHST> lstDHST,
        List<HisTrackingGroupADOs> lstTrackingTimeADOs,
        List<V_HIS_SERVICE_REQ> lstServiceReq
        )
    {
      try
      {
        this.departmentName = departmentName;
        this.roomName = roomName;
        this.currentPatient = currentPatient;
        this.currentTreatment = currentTreatment;
        this.lstDHST = lstDHST;
        this.lstTrackingTimeADOs = lstTrackingTimeADOs;
        this.lstServiceReq = lstServiceReq;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }
}
