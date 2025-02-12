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
using Inventec.Common.LocalStorage.SdaConfig;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000011.PDO
{
    public class Mps000011PDO : RDOBase
    {
        public const string printTypeCode = "Mps000011";

        public V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        public HIS_TREATMENT currentTreatment { get; set; }
        public List<TranPatiReasonADO> tranpatiReasonADOs { get; set; }
        public HIS_TRAN_PATI_FORM tranPatiForms { get; set; }
        public PatientADO PatientADO { get; set; }
        public Mps000011ADO Mps000011ADO { get; set; }
        public HIS_TRAN_PATI_TECH _TranPatiTech { get; set; }
        public List<HIS_MEDI_ORG> _MediOrgs { get; set; }

        public HIS_SERVICE_REQ _ServiceReq { get; set; }
        public HIS_DHST _Dhst { get; set; }
        public HIS_TREATMENT_EXT _TreatmentExt { get; set; }
        public Mps000011PDO() { }

        public Mps000011PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
            HIS_TREATMENT hisTreatment,
            List<TranPatiReasonADO> tranpatiReasonADOs,
            HIS_TRAN_PATI_FORM tranPatiForms,
            Mps000011ADO _Mps000011ADO,
            HIS_TRAN_PATI_TECH _tranPatiTech
            )
        {
            try
            {
                this.PatientADO = patientADO;
                this.PatyAlterBhyt = patientTypeAlter;
                this.currentTreatment = hisTreatment;
                this.tranpatiReasonADOs = tranpatiReasonADOs.OrderBy(o => o.TRAN_PATI_REASON_CODE).ToList(); ;
                this.tranPatiForms = tranPatiForms;
                this.Mps000011ADO = _Mps000011ADO;
                this._TranPatiTech = _tranPatiTech;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000011PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
            HIS_TREATMENT hisTreatment,
            List<TranPatiReasonADO> tranpatiReasonADOs,
            HIS_TRAN_PATI_FORM tranPatiForms,
            Mps000011ADO _Mps000011ADO,
            HIS_TRAN_PATI_TECH _tranPatiTech,
            List<HIS_MEDI_ORG> mediOrgs
            )
        {
            try
            {
                this.PatientADO = patientADO;
                this.PatyAlterBhyt = patientTypeAlter;
                this.currentTreatment = hisTreatment;
                this.tranpatiReasonADOs = tranpatiReasonADOs.OrderBy(o => o.TRAN_PATI_REASON_CODE).ToList(); ;
                this.tranPatiForms = tranPatiForms;
                this.Mps000011ADO = _Mps000011ADO;
                this._TranPatiTech = _tranPatiTech;
                this._MediOrgs = mediOrgs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000011PDO(
         PatientADO patientADO,
         V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
         HIS_TREATMENT hisTreatment,
         List<TranPatiReasonADO> tranpatiReasonADOs,
         HIS_TRAN_PATI_FORM tranPatiForms,
         Mps000011ADO _Mps000011ADO,
         HIS_TRAN_PATI_TECH _tranPatiTech,
         List<HIS_MEDI_ORG> mediOrgs,
         HIS_SERVICE_REQ serviceReq
         )
        {
            try
            {
                this.PatientADO = patientADO;
                this.PatyAlterBhyt = patientTypeAlter;
                this.currentTreatment = hisTreatment;
                this.tranpatiReasonADOs = tranpatiReasonADOs.OrderBy(o => o.TRAN_PATI_REASON_CODE).ToList(); ;
                this.tranPatiForms = tranPatiForms;
                this.Mps000011ADO = _Mps000011ADO;
                this._TranPatiTech = _tranPatiTech;
                this._MediOrgs = mediOrgs;
                this._ServiceReq = serviceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000011PDO(
          PatientADO patientADO,
          V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
          HIS_TREATMENT hisTreatment,
          List<TranPatiReasonADO> tranpatiReasonADOs,
          HIS_TRAN_PATI_FORM tranPatiForms,
          Mps000011ADO _Mps000011ADO,
          HIS_TRAN_PATI_TECH _tranPatiTech,
          HIS_SERVICE_REQ serviceReq
          )
        {
            try
            {
                this.PatientADO = patientADO;
                this.PatyAlterBhyt = patientTypeAlter;
                this.currentTreatment = hisTreatment;
                this.tranpatiReasonADOs = tranpatiReasonADOs.OrderBy(o => o.TRAN_PATI_REASON_CODE).ToList(); ;
                this.tranPatiForms = tranPatiForms;
                this.Mps000011ADO = _Mps000011ADO;
                this._TranPatiTech = _tranPatiTech;
                this._ServiceReq = serviceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000011PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
            HIS_TREATMENT hisTreatment,
            List<TranPatiReasonADO> tranpatiReasonADOs,
            HIS_TRAN_PATI_FORM tranPatiForms,
            Mps000011ADO _Mps000011ADO,
            HIS_TRAN_PATI_TECH _tranPatiTech,
            HIS_DHST _dhst
            )
        {
            try
            {
                this.PatientADO = patientADO;
                this.PatyAlterBhyt = patientTypeAlter;
                this.currentTreatment = hisTreatment;
                this.tranpatiReasonADOs = tranpatiReasonADOs.OrderBy(o => o.TRAN_PATI_REASON_CODE).ToList(); ;
                this.tranPatiForms = tranPatiForms;
                this.Mps000011ADO = _Mps000011ADO;
                this._TranPatiTech = _tranPatiTech;
                this._Dhst = _dhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000011PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
            HIS_TREATMENT hisTreatment,
            List<TranPatiReasonADO> tranpatiReasonADOs,
            HIS_TRAN_PATI_FORM tranPatiForms,
            Mps000011ADO _Mps000011ADO,
            HIS_TRAN_PATI_TECH _tranPatiTech,
            List<HIS_MEDI_ORG> mediOrgs,
            HIS_DHST _dhst
            )
        {
            try
            {
                this.PatientADO = patientADO;
                this.PatyAlterBhyt = patientTypeAlter;
                this.currentTreatment = hisTreatment;
                this.tranpatiReasonADOs = tranpatiReasonADOs.OrderBy(o => o.TRAN_PATI_REASON_CODE).ToList(); ;
                this.tranPatiForms = tranPatiForms;
                this.Mps000011ADO = _Mps000011ADO;
                this._TranPatiTech = _tranPatiTech;
                this._MediOrgs = mediOrgs;
                this._Dhst = _dhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000011PDO(
         PatientADO patientADO,
         V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
         HIS_TREATMENT hisTreatment,
         List<TranPatiReasonADO> tranpatiReasonADOs,
         HIS_TRAN_PATI_FORM tranPatiForms,
         Mps000011ADO _Mps000011ADO,
         HIS_TRAN_PATI_TECH _tranPatiTech,
         List<HIS_MEDI_ORG> mediOrgs,
         HIS_SERVICE_REQ serviceReq,
         HIS_DHST _dhst
         )
        {
            try
            {
                this.PatientADO = patientADO;
                this.PatyAlterBhyt = patientTypeAlter;
                this.currentTreatment = hisTreatment;
                this.tranpatiReasonADOs = tranpatiReasonADOs.OrderBy(o => o.TRAN_PATI_REASON_CODE).ToList(); ;
                this.tranPatiForms = tranPatiForms;
                this.Mps000011ADO = _Mps000011ADO;
                this._TranPatiTech = _tranPatiTech;
                this._MediOrgs = mediOrgs;
                this._ServiceReq = serviceReq;
                this._Dhst = _dhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000011PDO(
          PatientADO patientADO,
          V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
          HIS_TREATMENT hisTreatment,
          List<TranPatiReasonADO> tranpatiReasonADOs,
          HIS_TRAN_PATI_FORM tranPatiForms,
          Mps000011ADO _Mps000011ADO,
          HIS_TRAN_PATI_TECH _tranPatiTech,
          HIS_SERVICE_REQ serviceReq,
          HIS_DHST _dhst
          )
        {
            try
            {
                this.PatientADO = patientADO;
                this.PatyAlterBhyt = patientTypeAlter;
                this.currentTreatment = hisTreatment;
                this.tranpatiReasonADOs = tranpatiReasonADOs.OrderBy(o => o.TRAN_PATI_REASON_CODE).ToList(); ;
                this.tranPatiForms = tranPatiForms;
                this.Mps000011ADO = _Mps000011ADO;
                this._TranPatiTech = _tranPatiTech;
                this._ServiceReq = serviceReq;
                this._Dhst = _dhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }


    public class TranPatiReasonADO : MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON
    {
        public string checkbox { get; set; }

        public TranPatiReasonADO() { }

        public TranPatiReasonADO(HIS_TRAN_PATI_REASON data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<HIS_TRAN_PATI_REASON>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, item.GetValue(data));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class PatientADO : MOS.EFMODEL.DataModels.V_HIS_PATIENT
    {
        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        //public string CMND_DATE_STR { get; set; }
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
                    //this.CMND_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.CMND_DATE ?? 0);

                    if (this.GENDER_CODE == SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_GENDER__GENDER_CODE__FEMALE))
                    {
                        this.GENDER_MALE = "";
                        this.GENDER_FEMALE = "X";
                    }
                    else
                    {
                        this.GENDER_MALE = "X";
                        this.GENDER_FEMALE = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    static class AgeUtil
    {
        internal static string CalculateFullAge(long ageNumber)
        {
            string tuoi;
            string cboAge;
            try
            {
                DateTime dtNgSinh = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                TimeSpan diff = DateTime.Now - dtNgSinh;
                long tongsogiay = diff.Ticks;
                if (tongsogiay < 0)
                {
                    tuoi = "";
                    cboAge = "Tuổi";
                    return "";
                }
                DateTime newDate = new DateTime(tongsogiay);

                int nam = newDate.Year - 1;
                int thang = newDate.Month - 1;
                int ngay = newDate.Day - 1;
                int gio = newDate.Hour;
                int phut = newDate.Minute;
                int giay = newDate.Second;

                if (nam > 0)
                {
                    tuoi = nam.ToString();
                    cboAge = "Tuổi";
                }
                else
                {
                    if (thang > 0)
                    {
                        tuoi = thang.ToString();
                        cboAge = "Tháng";
                    }
                    else
                    {
                        if (ngay > 0)
                        {
                            tuoi = ngay.ToString();
                            cboAge = "ngày";
                        }
                        else
                        {
                            tuoi = "";
                            cboAge = "Giờ";
                        }
                    }
                }
                return tuoi + " " + cboAge;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }

    public class Mps000011ADO
    {
        public string DEATH_CAUSE_CODE { get; set; }
        public string DEATH_CAUSE_NAME { get; set; }
        public string DEATH_WITHIN_CODE { get; set; }
        public string DEATH_WITHIN_NAME { get; set; }
        public string TRAN_PATI_FORM_CODE { get; set; }
        public string TRAN_PATI_FORM_NAME { get; set; }
        public string TRAN_PATI_REASON_CODE { get; set; }
        public string TRAN_PATI_REASON_NAME { get; set; }

        public string END_DEPARTMENT_CODE { get; set; }
        public string END_DEPARTMENT_NAME { get; set; }
        public string END_ROOM_CODE { get; set; }
        public string END_ROOM_NAME { get; set; }
        public string FEE_LOCK_DEPARTMENT_CODE { get; set; }
        public string FEE_LOCK_DEPARTMENT_NAME { get; set; }
        public string FEE_LOCK_ROOM_CODE { get; set; }
        public string FEE_LOCK_ROOM_NAME { get; set; }
        public string IN_DEPARTMENT_CODE { get; set; }
        public string IN_DEPARTMENT_NAME { get; set; }
        public string IN_ROOM_CODE { get; set; }
        public string IN_ROOM_NAME { get; set; }
        public string TREATMENT_RESULT_NAME { get; set; }
        public string TREATMENT_RESULT_CODE { get; set; }
    }
}
