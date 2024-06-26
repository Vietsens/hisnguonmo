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

namespace MPS.Processor.Mps000012.PDO
{
    public class Mps000012PDO : RDOBase
    {
        public V_HIS_PATY_ALTER_BHYT PatyAlterBhyt { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran { get; set; }
        public HIS_DHST Dhsts { get; set; }
        public V_HIS_TRAN_PATI tranPaties { get; set; }
        public V_HIS_EXAM_SERVICE_REQ ExamServiceReqs { get; set; }
        public V_HIS_TREATMENT currentTreatment { get; set; }
        public V_HIS_SERE_SERV sereServs { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> currentExpMestMedicines { get; set; }
        public long maxUserTime;
        public V_HIS_EXAM_SERVICE_REQ_1 vHisExamServiceReq1 { get; set; }
        public string requestDepartmentName { get; set; }

        public Mps000012PDO(
            V_HIS_PATIENT patient,
            List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran,
            V_HIS_PATY_ALTER_BHYT patyAlterBhyt,
           V_HIS_TRAN_PATI tranPaties,
            HIS_DHST dhsts,
            V_HIS_EXAM_SERVICE_REQ examServiceReqs,
            V_HIS_TREATMENT currentTreaatment,
            V_HIS_SERE_SERV sereServs,
            List<V_HIS_EXP_MEST_MEDICINE> currentExpMestMedicines,

            long maxUserTime
            )
        {
            try
            {
                this.Patient = patient;
                this.lstDepartmentTran = lstDepartmentTran;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.tranPaties = tranPaties;
                this.Dhsts = dhsts;
                this.ExamServiceReqs = examServiceReqs;
                this.currentTreatment = currentTreaatment;
                this.sereServs = sereServs;
                this.currentExpMestMedicines = currentExpMestMedicines;
                this.maxUserTime = maxUserTime;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000012PDO(
           V_HIS_PATIENT patient,
           List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran,
           V_HIS_PATY_ALTER_BHYT patyAlterBhyt,
          V_HIS_TRAN_PATI tranPaties,
           V_HIS_EXAM_SERVICE_REQ_1 vHisExamServiceReq1,
           V_HIS_TREATMENT currentTreaatment,
           List<V_HIS_EXP_MEST_MEDICINE> currentExpMestMedicines,
            string requestDepartmentName,
           long maxUserTime
           )
        {
            try
            {
                this.Patient = patient;
                this.lstDepartmentTran = lstDepartmentTran;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.tranPaties = tranPaties;
                this.vHisExamServiceReq1 = vHisExamServiceReq1;
                this.currentTreatment = currentTreaatment;
                this.currentExpMestMedicines = currentExpMestMedicines;
                this.maxUserTime = maxUserTime;
                this.requestDepartmentName = requestDepartmentName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
