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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ADO
{
    public class AssignServiceADO
    {
        public delegate void DelegateProcessDataResult(object data);
        public delegate void DelegateProcessRefeshIcd(string icdCode, string icdMainText, string ictExtraCodes, string ictExtraNames);

        public long TreatmentId { get; set; }
        public long PreviusTreatmentId { get; set; }
        public long IntructionTime { get; set; }
        public long? ServiceReqId { get; set; }
        public string PatientName { get; set; }
        public long PatientDob { get; set; }
        public string GenderName { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_SERE_SERV SereServ { get; set; }
        public MOS.EFMODEL.DataModels.HIS_DHST Dhst { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> SereServsInTreatment { get; set; }
        public MOS.EFMODEL.DataModels.HIS_SERVICE_REQ IcdExam { get; set; }
        public MOS.EFMODEL.DataModels.HIS_SERVICE_REQ ServiceReqInfo { get; set; }
        public DelegateProcessDataResult DgProcessDataResult { get; set; }
        public DelegateProcessRefeshIcd DgProcessRefeshIcd { get; set; }
        public bool IsInKip { get; set; }
        public bool IsAutoEnableEmergency { get; set; }
        public string ProvisionalDiagnosis { get; set; }
        public bool IsPriority { get; set; }
        public bool IsAssignInPttt { get; set; }
        public long? ExamRegisterRoomId { get; set; }
        public bool? IsNotUseBhyt { get; set; }
        public MOS.EFMODEL.DataModels.HIS_TRACKING Tracking { get; set; }
        public bool OpenFromBedRoomPartial { get; set; }
        public MOS.Filter.HisTreatmentBedRoomLViewFilter FilterFromBedRoomPartiral { get; set; }
        public AssignServiceADO(long _treatmentId, long _intructionTime, long _serviceReqId)
            : this(_treatmentId, _intructionTime, _serviceReqId, null, null, false)
        {

        }

        public AssignServiceADO(long _treatmentId, long _intructionTime, long _serviceReqId, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV _sereServ)
            : this(_treatmentId, _intructionTime, _serviceReqId, _sereServ, null, false)
        {

        }

        public AssignServiceADO(long _treatmentId, long _intructionTime, long _serviceReqId, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV _sereServ, HIS.Desktop.ADO.AssignServiceADO.DelegateProcessDataResult processDataResult, bool isInkip)
        {
            this.TreatmentId = _treatmentId;
            this.IntructionTime = _intructionTime;
            this.ServiceReqId = _serviceReqId;
            this.SereServ = _sereServ;
            this.DgProcessDataResult = processDataResult;
            this.IsInKip = isInkip;
        }

        public AssignServiceADO(long _treatmentId, long _previusTreatmentId, HIS.Desktop.ADO.AssignServiceADO.DelegateProcessDataResult processDataResult)
        {
            this.TreatmentId = _treatmentId;
            this.PreviusTreatmentId = _previusTreatmentId;
            this.DgProcessDataResult = processDataResult;
        }
    }
}
