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
using MPS.ADO.TrackingPrint;

namespace MPS.Core.Mps000072
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000072RDO : RDOBase
    {
        internal PatientADO currentPatient { get; set; }
        internal TreatmentADO currentTreatment { get; set; }
        internal List<HIS_DHST> lstDHST { get; set; }
        internal V_HIS_SERVICE_REQ currentServiceReq { get; set; }
        internal List<V_HIS_SERVICE_REQ> lstServiceReq { get; set; }
        internal List<V_HIS_SERE_SERV> lstSereServ { get; set; }
        internal List<HisTrackingGroupADOs> lstTrackingTimeADOs { get; set; }
        internal List<HisServiceReqTrackingId> lstServiceReqPrint { get; set; }
        internal List<Mps000062ADO> mps000062ADOs { get; set; }
        string departmentName;
        string roomName;
        internal List<Mps000062ADOMedicines> lstMps000062ADOMedicines { get; set; }
        internal List<Mps000062ADOServiceCLS> lstMps000062ADOServiceCLS { get; set; }

        public Mps000072RDO(
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

        public Mps000072RDO(
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

        internal override void SetSingleKey()
        {
            try
            {
                keyValues.Add(new KeyValue(Mps000072ExtendSingleKey.DEPARTMENT_NAME, departmentName));
                keyValues.Add(new KeyValue(Mps000072ExtendSingleKey.ROOM_NAME, roomName));
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(currentPatient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
