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

namespace MPS.Core.Mps000012
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000012RDO : RDOBase
    {
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal PatientADO Patient { get; set; }
        internal List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran { get; set; }
        internal HIS_DHST Dhsts { get; set; }
        internal V_HIS_TRAN_PATI tranPaties { get; set; }
        internal V_HIS_EXAM_SERVICE_REQ ExamServiceReqs { get; set; }
        internal TreatmentADO currentTreaatment { get; set; }
        internal V_HIS_SERE_SERV sereServs { get; set; }
        internal List<V_HIS_EXP_MEST_MEDICINE> currentExpMestMedicines { get; set; }
        long maxUserTime;
        internal V_HIS_EXAM_SERVICE_REQ_1 vHisExamServiceReq1 { get; set; }
        string requestDepartmentName { get; set; }

        public Mps000012RDO(
            PatientADO patient,
            List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran,
            PatyAlterBhytADO patyAlterBhyt,
           V_HIS_TRAN_PATI tranPaties,
            HIS_DHST dhsts,
            V_HIS_EXAM_SERVICE_REQ examServiceReqs,
            TreatmentADO currentTreaatment,
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
                this.currentTreaatment = currentTreaatment;
                this.sereServs = sereServs;
                this.currentExpMestMedicines = currentExpMestMedicines;
                this.maxUserTime = maxUserTime;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000012RDO(
           PatientADO patient,
           List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran,
           PatyAlterBhytADO patyAlterBhyt,
          V_HIS_TRAN_PATI tranPaties,
           V_HIS_EXAM_SERVICE_REQ_1 vHisExamServiceReq1,
           TreatmentADO currentTreaatment,
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
                this.currentTreaatment = currentTreaatment;
                this.currentExpMestMedicines = currentExpMestMedicines;
                this.maxUserTime = maxUserTime;
                this.requestDepartmentName = requestDepartmentName;
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
                if (!String.IsNullOrEmpty(requestDepartmentName))
                {
                    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.REQUEST_DEPARTMENT_NAME, requestDepartmentName)));
                }

                keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.FINISH_TIME_TRAN_PATI, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxUserTime))));

                lstDepartmentTran = lstDepartmentTran.OrderBy(o => o.LOG_TIME).ToList();
                if (lstDepartmentTran != null && lstDepartmentTran.Count > 0)
                {
                    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.CREATE_TIME_TRAN_PATI, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(lstDepartmentTran[0].LOG_TIME))));

                    //if (lstDepartmentTran[lstDepartmentTran.Count - 1] != null)
                    //{
                    //    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.FINISH_TIME_TRAN_PATI, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(lstDepartmentTran[lstDepartmentTran.Count - 1].LOG_TIME))));
                    //}
                    //else
                    //{
                    //    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.FINISH_TIME_TRAN_PATI, "")));
                    //}
                }
                else
                {
                    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.CREATE_TIME_TRAN_PATI, "")));
                    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.FINISH_TIME_TRAN_PATI, "")));
                }

                if (ExamServiceReqs != null)
                    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ExamServiceReqs.INTRUCTION_TIME))));
                if (vHisExamServiceReq1 != null)
                    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vHisExamServiceReq1.INTRUCTION_TIME))));

                if (currentExpMestMedicines != null && currentExpMestMedicines.Count > 0)
                {
                    long maxUseTimeTo = currentExpMestMedicines.Max(a => a.USE_TIME_TO ?? 0);
                    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxUseTimeTo))));

                    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.TREATMENT_OUT_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxUseTimeTo))));

                    //keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.FINISH_TIME_TRAN_PATI, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxUseTimeTo))));

                }
                else
                {
                    if (currentTreaatment != null && currentTreaatment.OUT_TIME.HasValue)
                    {
                        keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.TREATMENT_OUT_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreaatment.OUT_TIME.Value))));
                    }
                }

                if (Dhsts != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<HIS_DHST>(Dhsts, keyValues);
                }
                if (currentTreaatment != null)
                {
                    if (string.IsNullOrEmpty(currentTreaatment.ICD_MAIN_TEXT))
                    {
                        keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.ICD_TREATMENT_NAME, currentTreaatment.ICD_NAME)));
                    }
                    else
                    {
                        keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.ICD_TREATMENT_NAME, currentTreaatment.ICD_MAIN_TEXT)));
                    }

                    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.ICD_TREATMENT_CODE, currentTreaatment.ICD_CODE)));
                    keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.ICD_TREATMENT_TEXT, currentTreaatment.ICD_TEXT)));
                }
                if (tranPaties != null && tranPaties.TRAN_PATI_TYPE_ID == 1)
                {
                    if (string.IsNullOrEmpty(tranPaties.ICD_NAME))
                    {
                        keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.ICD_NGT_TEXT, tranPaties.ICD_NAME)));
                    }
                    else
                    {
                        keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.ICD_NGT_TEXT, tranPaties.ICD_MAIN_TEXT)));
                    }
                }

                if (currentTreaatment != null)
                {

                    if (currentTreaatment.CLINICAL_IN_TIME.HasValue)
                    {
                        keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.TREATMENT_IN_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreaatment.CLINICAL_IN_TIME.Value))));
                    }
                    else
                    {
                        keyValues.Add((new KeyValue(Mps000012ExtendSingleKey.TREATMENT_IN_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreaatment.IN_TIME))));
                    }
                }

                if (ExamServiceReqs != null)
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ>(ExamServiceReqs, keyValues, false);
                if (vHisExamServiceReq1 != null)
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ_1>(vHisExamServiceReq1, keyValues, false);

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(tranPaties, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentTreaatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(sereServs, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
