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

namespace MPS.Processor.Mps000315.PDO
{
    public class Mps000315PDO : RDOBase
    {
        public List<V_HIS_TREATMENT_4> _KSK_Treatments { get; set; }
        public List<V_HIS_SERVICE_REQ> _KSK_ServiceReqs { get; set; }
        public List<V_HIS_SERE_SERV> _KSK_SereServs { get; set; }
        public List<HIS_SERE_SERV_EXT> _KSK_SereServExts { get; set; }
        public List<V_HIS_BED_LOG> _KSK_BedLogs { get; set; }
        public List<V_HIS_PATIENT_TYPE_ALTER> _KSK_PatientTypeAlters { get; set; }
        public List<V_HIS_DHST> _KSK_Dhsts { get; set; }
        public List<V_HIS_SERE_SERV_TEIN> _KSK_SereServTeins { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> _KSK_HealthExamRank { get; set; }
        public List<HIS_PATIENT> _KSK_Patients { get; set; }

        public List<HIS_KSK_GENERAL> _KskGeneral { get; set; }
        public List<HIS_KSK_DRIVER> _KskDriver { get; set; }

        public Mps000315PDO() { }

        public Mps000315PDO(List<HIS_KSK_GENERAL> _KskGeneral,
               List<V_HIS_SERVICE_REQ> _KSK_ServiceReqss,
            List<V_HIS_DHST> _KSK_Dhsts,
            List<HIS_HEALTH_EXAM_RANK> _KSK_HealthExamRank
            )
        {
            try
            {
                this._KskGeneral = _KskGeneral;
                this._KSK_ServiceReqs = _KSK_ServiceReqss;
                this._KSK_Dhsts = _KSK_Dhsts;
                this._KSK_HealthExamRank = _KSK_HealthExamRank;
            }
            catch (Exception ex)
            {
                
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000315PDO(List<HIS_KSK_GENERAL> _KskGeneral,
              List<V_HIS_SERVICE_REQ> _KSK_ServiceReqss,
           List<V_HIS_DHST> _KSK_Dhsts,
           List<HIS_HEALTH_EXAM_RANK> _KSK_HealthExamRank,
           List<V_HIS_TREATMENT_4> _KSK_Treatmentss
           )
        {
            try
            {
                this._KskGeneral = _KskGeneral;
                this._KSK_ServiceReqs = _KSK_ServiceReqss;
                this._KSK_Dhsts = _KSK_Dhsts;
                this._KSK_HealthExamRank = _KSK_HealthExamRank;
                this._KSK_Treatments = _KSK_Treatmentss;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000315PDO(
            List<V_HIS_TREATMENT_4> _KSK_Treatmentss,
            List<V_HIS_SERVICE_REQ> _KSK_ServiceReqss,
            List<V_HIS_SERE_SERV> _KSK_SereServss,
            List<HIS_SERE_SERV_EXT> _KSK_SereServExtss,
            List<V_HIS_BED_LOG> _KSK_BedLogss,
            List<V_HIS_PATIENT_TYPE_ALTER> _KSK_PatientTypeAlterss,
            List<V_HIS_DHST> _KSK_Dhsts,
            List<V_HIS_SERE_SERV_TEIN> _KSK_SereServTeins,
            List<HIS_HEALTH_EXAM_RANK> _KSK_HealthExamRank,
            List<HIS_PATIENT> _KSK_Patients
            )
        {
            try
            {
                this._KSK_Treatments = _KSK_Treatmentss;
                this._KSK_ServiceReqs = _KSK_ServiceReqss;
                this._KSK_SereServs = _KSK_SereServss;
                this._KSK_SereServExts = _KSK_SereServExtss;
                this._KSK_BedLogs = _KSK_BedLogss;
                this._KSK_PatientTypeAlters = _KSK_PatientTypeAlterss;
                this._KSK_Dhsts = _KSK_Dhsts;
                this._KSK_SereServTeins = _KSK_SereServTeins;
                this._KSK_HealthExamRank = _KSK_HealthExamRank;
                this._KSK_Patients = _KSK_Patients;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000315PDO(List<HIS_KSK_GENERAL> _KskGeneral,
               List<V_HIS_SERVICE_REQ> _KSK_ServiceReqss,
            List<V_HIS_DHST> _KSK_Dhsts,
            List<HIS_HEALTH_EXAM_RANK> _KSK_HealthExamRank,
            List<HIS_KSK_DRIVER> _KSK_Driver
            )
        {
            try
            {
                this._KskGeneral = _KskGeneral;
                this._KSK_ServiceReqs = _KSK_ServiceReqss;
                this._KSK_Dhsts = _KSK_Dhsts;
                this._KSK_HealthExamRank = _KSK_HealthExamRank;
                this._KskDriver = _KSK_Driver;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000315PDO(
            List<V_HIS_TREATMENT_4> _KSK_Treatmentss,
            List<V_HIS_SERVICE_REQ> _KSK_ServiceReqss,
            List<V_HIS_SERE_SERV> _KSK_SereServss,
            List<HIS_SERE_SERV_EXT> _KSK_SereServExtss,
            List<V_HIS_BED_LOG> _KSK_BedLogss,
            List<V_HIS_PATIENT_TYPE_ALTER> _KSK_PatientTypeAlterss,
            List<V_HIS_DHST> _KSK_Dhsts,
            List<V_HIS_SERE_SERV_TEIN> _KSK_SereServTeins,
            List<HIS_HEALTH_EXAM_RANK> _KSK_HealthExamRank,
            List<HIS_PATIENT> _KSK_Patients,
            List<HIS_KSK_DRIVER> _KSK_Driver
            )
        {
            try
            {
                this._KSK_Treatments = _KSK_Treatmentss;
                this._KSK_ServiceReqs = _KSK_ServiceReqss;
                this._KSK_SereServs = _KSK_SereServss;
                this._KSK_SereServExts = _KSK_SereServExtss;
                this._KSK_BedLogs = _KSK_BedLogss;
                this._KSK_PatientTypeAlters = _KSK_PatientTypeAlterss;
                this._KSK_Dhsts = _KSK_Dhsts;
                this._KSK_SereServTeins = _KSK_SereServTeins;
                this._KSK_HealthExamRank = _KSK_HealthExamRank;
                this._KSK_Patients = _KSK_Patients;
                this._KskDriver = _KSK_Driver;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
