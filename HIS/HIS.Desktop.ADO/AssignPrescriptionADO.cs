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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ADO
{
    public class AssignPrescriptionADO
    {
        public delegate void DelegateProcessDataResult(object data);
        public delegate void DelegateProcessRefeshIcd(string icdCode, string icdMainText, string ictExtraCodes, string ictExtraNames);
        public delegate void DelegateProcessWhileAutoTreatmentEnd();

        public long TreatmentId { get; set; }
        public long PatientId { get; set; }
        public long PatientTypeId { get; set; }
        public string TreatmentCode { get; set; }
        public string HeinCardnumber { get; set; }
        public long? HeinCardFromTime { get; set; }
        public long? HeinCardToTime { get; set; }
        public string PatientName { get; set; }
        public long PatientDob { get; set; }
        public string GenderName { get; set; }
        public long PrescriptionTypeId { get; set; }
        public long? ExpMestTemplateId { get; set; }
        public long IntructionTime { get; set; }
        public long ServiceReqId { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_SERE_SERV SereServ { get; set; }
        public MOS.EFMODEL.DataModels.HIS_DHST Dhst { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> SereServsInTreatment { get; set; }
        public MOS.EFMODEL.DataModels.HIS_SERVICE_REQ IcdExam { get; set; }
        public MOS.EFMODEL.DataModels.HIS_SERVICE_REQ ServiceReqInfo { get; set; }
        public AssignPrescriptionEditADO AssignPrescriptionEditADO { get; set; }
        public DelegateProcessWhileAutoTreatmentEnd DlgWhileAutoTreatmentEnd { get; set; }
        public DelegateProcessDataResult DgProcessDataResult { get; set; }
        public DelegateProcessRefeshIcd DgProcessRefeshIcd { get; set; }     
        public bool IsAutoCheckExpend { get; set; }
        public bool IsInKip { get; set; }
        public bool IsCabinet { get; set; }
        public bool IsExecutePTTT { get; set; }
        public string ProvisionalDiagnosis { get; set; }
        public MOS.EFMODEL.DataModels.HIS_TRACKING Tracking { get; set; }

        public AssignPrescriptionADO(long _treatmentId, long _intructionTime, long _serviceReqId)
            : this(_treatmentId, _intructionTime, _serviceReqId, null, null, false, false, null)
        {

        }

        public AssignPrescriptionADO(long _treatmentId, long _intructionTime, long _serviceReqId, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV _sereServ)
            : this(_treatmentId, _intructionTime, _serviceReqId, _sereServ, null, false, false, null)
        {

        }

        public AssignPrescriptionADO(long _treatmentId, long _intructionTime, long _serviceReqId, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV _sereServ, HIS.Desktop.ADO.AssignPrescriptionADO.DelegateProcessDataResult processDataResult)
            : this(_treatmentId, _intructionTime, _serviceReqId, _sereServ, processDataResult, false, false, null)
        {

        }

        public AssignPrescriptionADO(long _treatmentId, long _intructionTime, long _serviceReqId, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV _sereServ, HIS.Desktop.ADO.AssignPrescriptionADO.DelegateProcessDataResult processDataResult, AssignPrescriptionEditADO assignPrescriptionEditADO)
            : this(_treatmentId, _intructionTime, _serviceReqId, _sereServ, processDataResult, false, false, assignPrescriptionEditADO)
        {

        }

        public AssignPrescriptionADO(long _treatmentId, long _intructionTime, long _serviceReqId, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV _sereServ, HIS.Desktop.ADO.AssignPrescriptionADO.DelegateProcessDataResult processDataResult, bool isInkip, bool isAutoCheckExpend, AssignPrescriptionEditADO assignPrescriptionEditADO)
        {
            this.TreatmentId = _treatmentId;
            this.IntructionTime = _intructionTime;
            this.ServiceReqId = _serviceReqId;
            this.SereServ = _sereServ;
            this.DgProcessDataResult = processDataResult;
            this.IsAutoCheckExpend = isAutoCheckExpend;
            this.IsInKip = isInkip;
            this.AssignPrescriptionEditADO = assignPrescriptionEditADO;
        }
    }
}
