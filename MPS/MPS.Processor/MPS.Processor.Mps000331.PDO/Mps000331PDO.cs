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

namespace MPS.Processor.Mps000331.PDO
{
    public partial class Mps000331PDO : RDOBase
    {
        public List<V_HIS_DEPARTMENT_TRAN> DepartmentTrans { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER patientTypeAlter { get; set; }
        public HIS_TREATMENT Treatment { get; set; }
        public List<HIS_ICD> Icds { get; set; }
        public HIS_DHST Dhst { get; set; }
        public List<HIS_EXP_MEST> ExpMests { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> ExpMestMedicines { get; set; }
        public V_HIS_SERVICE_REQ ServiceReq { get; set; }
        public string RequestDepartmentName { get; set; }

        public Mps000331PDO() { }

        public Mps000331PDO(
           V_HIS_PATIENT _patient,
           List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
           V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter,
           V_HIS_SERVICE_REQ _serviceReq,
           HIS_DHST _dhst,
           HIS_TREATMENT _treatment,
           List<HIS_ICD> _icds,
           List<HIS_EXP_MEST> _expMest,
           List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
           string _requestDepartmentName
           )
        {
            try
            {
                this.Patient = _patient;
                this.DepartmentTrans = _departmentTrans;
                this.patientTypeAlter = _patientTypeAlter;
                this.ServiceReq = _serviceReq;
                this.Treatment = _treatment;
                this.ExpMests = _expMest;
                this.ExpMestMedicines = _expMestMedicines;
                this.RequestDepartmentName = _requestDepartmentName;
                this.Icds = _icds;
                this.Dhst = _dhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public class PatientADO : V_HIS_PATIENT
        {
            public string AGE { get; set; }
            public string DOB_STR { get; set; }
            public string CMND_DATE_STR { get; set; }
            public string DOB_YEAR { get; set; }
            public string GENDER_MALE { get; set; }
            public string GENDER_FEMALE { get; set; }

            public PatientADO() { }

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
    }
}
