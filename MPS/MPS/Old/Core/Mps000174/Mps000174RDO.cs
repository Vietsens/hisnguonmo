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
using MPS.Old.Config;

namespace MPS.Core.Mps000174
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000174RDO : RDOBase
    {
        internal PatyAlterBhytADO PatyAlterBhytADO { get; set; }
        internal PatientADO PatientADO { get; set; }
        internal List<V_HIS_DEPARTMENT_TRAN> DepartmentTrans { get; set; }
        internal V_HIS_TRAN_PATI TranPaties { get; set; }
        internal V_HIS_PATIENT Patient { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER PatyAlter { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER patientTypeAlter { get; set; }
        internal V_HIS_TREATMENT Treatment { get; set; }
        internal List<V_HIS_PRESCRIPTION> Prescriptions { get; set; }
        internal List<V_HIS_EXP_MEST_MEDICINE> ExpMestMedicines { get; set; }
        internal List<V_HIS_SERE_SERV_5> sereServ5s { get; set; }
        internal HisExpMestSttCFGPrint HisExpMestSttCFG { get; set; }
        internal V_HIS_EXAM_SERVICE_REQ ExamServiceReq { get; set; }
        internal string RequestDepartmentName { get; set; }

        public Mps000174RDO(
           V_HIS_PATIENT _patient,
           List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
           V_HIS_PATIENT_TYPE_ALTER _patyAlter,
            V_HIS_TRAN_PATI _tranPaties,
           V_HIS_EXAM_SERVICE_REQ _examServiceReq,
           V_HIS_TREATMENT _treatment,
            List<V_HIS_PRESCRIPTION> _precriptions,
           List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            List<V_HIS_SERE_SERV_5> _sereServ5s,
            HisExpMestSttCFGPrint _expMestSttCFG,
           string _requestDepartmentName
           )
        {
            try
            {
                this.Patient = _patient;
                this.DepartmentTrans = _departmentTrans;
                this.PatyAlter = _patyAlter;
                this.TranPaties = _tranPaties;
                this.ExamServiceReq = _examServiceReq;
                this.Treatment = _treatment;
                this.Prescriptions = _precriptions;
                this.ExpMestMedicines = _expMestMedicines;
                this.HisExpMestSttCFG = _expMestSttCFG;
                this.RequestDepartmentName = _requestDepartmentName;
                this.sereServ5s = _sereServ5s;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void DataInputProcess()
        {
            try
            {
                PatientADO = DataRawProcess.PatientRawToADO(Patient);
                PatyAlterBhytADO = DataRawProcess.PatyAlterBHYTRawToADO(PatyAlter);


                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                foreach (var prescription in Prescriptions)
                {
                    if (prescription.EXP_MEST_STT_ID == HisExpMestSttCFG.HisExpMestSttId__Approved
                        || prescription.EXP_MEST_STT_ID == HisExpMestSttCFG.HisExpMestSttId__Exported)
                    {
                        List<V_HIS_EXP_MEST_MEDICINE> expMestTemps = ExpMestMedicines.Where(o => (o.IN_EXECUTE ?? -1) == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE && o.EXP_MEST_ID == prescription.EXP_MEST_ID).ToList();
                        expMestMedicines.AddRange(expMestTemps);
                    }
                    else if (prescription.EXP_MEST_STT_ID == HisExpMestSttCFG.HisExpMestSttId__Request
                        || prescription.EXP_MEST_STT_ID == HisExpMestSttCFG.HisExpMestSttId__Rejected
                        || prescription.EXP_MEST_STT_ID == HisExpMestSttCFG.HisExpMestSttId__Draft)
                    {
                        List<V_HIS_EXP_MEST_MEDICINE> expMestTemps = ExpMestMedicines.Where(o => (o.IN_REQUEST ?? -1) == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE
                            && o.EXP_MEST_ID == prescription.EXP_MEST_ID).ToList();
                        expMestMedicines.AddRange(expMestTemps);
                    }
                }

                var expMestMedicineGroups = expMestMedicines.GroupBy(o =>
                    new { o.MEDICINE_TYPE_ID, o.MEDICINE_ID, o.PRICE, o.IN_EXECUTE, o.IN_REQUEST, o.IS_EXPEND });
                ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                foreach (var expMestMedicineGroup in expMestMedicineGroups)
                {
                    V_HIS_EXP_MEST_MEDICINE expMestMedicine = expMestMedicineGroup.First();
                    var sereServ5 = sereServ5s.FirstOrDefault(o => o.MEDICINE_ID == expMestMedicine.MEDICINE_ID);
                    if (sereServ5 != null && sereServ5.AMOUNT > 0)
                    {
                        expMestMedicine.AMOUNT = sereServ5.AMOUNT;
                        ExpMestMedicines.Add(expMestMedicine);
                    }

                }
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
                if (!String.IsNullOrEmpty(RequestDepartmentName))
                {
                    keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.REQUEST_DEPARTMENT_NAME, RequestDepartmentName)));
                }

                DepartmentTrans = DepartmentTrans.OrderBy(o => o.LOG_TIME).ToList();
                if (DepartmentTrans != null && DepartmentTrans.Count > 0)
                {
                    keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.CREATE_TIME_TRAN_PATI, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(DepartmentTrans[0].LOG_TIME))));
                }
                else
                {
                    keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.CREATE_TIME_TRAN_PATI, "")));
                    keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.FINISH_TIME_TRAN_PATI, "")));
                }

                if (ExamServiceReq != null)
                    keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ExamServiceReq.INTRUCTION_TIME))));

                long maxUseTimeTo = 0;
                if (ExpMestMedicines != null && ExpMestMedicines.Count > 0)
                {
                    maxUseTimeTo = ExpMestMedicines.Max(a => a.USE_TIME_TO ?? 0);
                    if (maxUseTimeTo > 0)
                    {
                        keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxUseTimeTo))));
                        keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.TREATMENT_OUT_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxUseTimeTo))));
                    }
                }
                if (Treatment != null)
                {
                    if (maxUseTimeTo == 0 && Treatment.OUT_TIME.HasValue)
                    {
                        keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.TREATMENT_OUT_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Treatment.OUT_TIME.Value))));
                    }
                    if (string.IsNullOrEmpty(Treatment.ICD_MAIN_TEXT))
                    {
                        keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.ICD_TREATMENT_NAME, Treatment.ICD_NAME)));
                    }
                    else
                    {
                        keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.ICD_TREATMENT_NAME, Treatment.ICD_MAIN_TEXT)));
                    }
                    if (Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.TREATMENT_IN_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Treatment.CLINICAL_IN_TIME.Value))));
                    }
                    else
                    {
                        keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.TREATMENT_IN_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Treatment.IN_TIME))));
                    }
                    keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.ICD_TREATMENT_CODE, Treatment.ICD_CODE)));
                    keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.ICD_TREATMENT_TEXT, Treatment.ICD_TEXT)));
                }

                if (TranPaties != null && TranPaties.TRAN_PATI_TYPE_ID == 1)
                {
                    if (!String.IsNullOrEmpty(TranPaties.ICD_NAME))
                    {
                        keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.ICD_NGT_TEXT, TranPaties.ICD_NAME)));
                    }
                    else
                    {
                        keyValues.Add((new KeyValue(Mps000174ExtendSingleKey.ICD_NGT_TEXT, TranPaties.ICD_MAIN_TEXT)));
                    }
                }

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ>(ExamServiceReq, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(TranPaties, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(Treatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhytADO, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(PatientADO, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
