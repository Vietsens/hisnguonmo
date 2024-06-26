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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.Bhyt.HeinTreatmentType;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Print
{
    public class PrintGlobalStore
    {
        public static PatientADO GetPatientById(long patientId)
        {
            PatientADO currentPatientADO = new PatientADO();
            MOS.EFMODEL.DataModels.HIS_PATIENT patient = new HIS_PATIENT();
            try
            {
                MOS.Filter.HisPatientFilter patientFilter = new MOS.Filter.HisPatientFilter();
                patientFilter.ID = patientId;
                CommonParam param = new CommonParam();
                var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GET, ApiConsumers.MosConsumer, patientFilter, param);
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();

                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.ADO.PatientADO>(currentPatientADO, patient);
                    //currentPatientADO.CMND_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientADO.CMND_DATE ?? 0);
                    currentPatientADO.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientADO.DOB);
                    currentPatientADO.AGE = AgeHelper.CalculateAgeFromYear(currentPatientADO.DOB);
                    if (currentPatientADO != null && currentPatientADO.DOB > 0)
                    {
                        currentPatientADO.DOB_YEAR = currentPatientADO.DOB.ToString().Substring(0, 4);
                    }

                    if (currentPatientADO.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        currentPatientADO.GENDER_MALE = "";
                        currentPatientADO.GENDER_FEMALE = "X";
                    }
                    else
                    {
                        currentPatientADO.GENDER_MALE = "X";
                        currentPatientADO.GENDER_FEMALE = "";
                    }

                    var gender = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == currentPatientADO.GENDER_ID);
                    if (gender != null)
                    {
                        currentPatientADO.GENDER_CODE = gender.GENDER_CODE;
                        currentPatientADO.GENDER_NAME = gender.GENDER_NAME;
                    }

                    var military = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MILITARY_RANK>().FirstOrDefault(o => o.ID == currentPatientADO.MILITARY_RANK_ID);
                    if (military != null)
                    {
                        currentPatientADO.MILITARY_RANK_CODE = military.MILITARY_RANK_CODE;
                        currentPatientADO.MILITARY_RANK_NAME = military.MILITARY_RANK_NAME;
                        currentPatientADO.NUM_ORDER = military.NUM_ORDER;
                    }

                    var work = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == currentPatientADO.WORK_PLACE_ID);
                    if (work != null)
                    {
                        currentPatientADO.WORK_PLACE_ADDRESS = work.ADDRESS;
                        currentPatientADO.WORK_PLACE_CODE = work.WORK_PLACE_CODE;
                        currentPatientADO.WORK_PLACE_NAME = work.WORK_PLACE_NAME;
                    }

                    //var bornType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BORN_TYPE>().FirstOrDefault(o => o.ID == currentPatientADO.BORN_TYPE_ID);
                    //if (bornType != null)
                    //{
                    //    currentPatientADO.BORN_TYPE_CODE = bornType.BORN_TYPE_CODE;
                    //    currentPatientADO.BORN_TYPE_NAME = bornType.BORN_TYPE_NAME;
                    //}

                    var career = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.ID == currentPatientADO.CAREER_ID);
                    if (career != null)
                    {
                        currentPatientADO.CAREER_CODE = career.CAREER_CODE;
                        currentPatientADO.CAREER_NAME = career.CAREER_NAME;
                    }

                    //var bloodAbo = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == currentPatientADO.BLOOD_ABO_ID);
                    //if (bloodAbo != null)
                    //{
                    //    currentPatientADO.BLOOD_ABO_CODE = bloodAbo.BLOOD_ABO_CODE;
                    //}

                    //var bloodRh = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == currentPatientADO.BLOOD_RH_ID);
                    //if (bloodRh != null)
                    //{
                    //    currentPatientADO.BLOOD_RH_CODE = bloodRh.BLOOD_RH_CODE;
                    //}
                }
            }
            catch (Exception ex)
            {
                currentPatientADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentPatientADO;
        }

        public static PatientADO GetPatientADOForPrintByIPatient(HIS_PATIENT patient)
        {
            PatientADO currentPatientADO = new PatientADO();
            try
            {
                if (patient == null) throw new ArgumentNullException("patient");

                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.ADO.PatientADO>(currentPatientADO, patient);
                //currentPatientADO.CMND_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientADO.CMND_DATE ?? 0);
                currentPatientADO.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientADO.DOB);
                currentPatientADO.AGE = AgeHelper.CalculateAgeFromYear(currentPatientADO.DOB);
                if (currentPatientADO != null && currentPatientADO.DOB > 0)
                {
                    currentPatientADO.DOB_YEAR = currentPatientADO.DOB.ToString().Substring(0, 4);
                }

                if (currentPatientADO.GENDER_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Inventec.Common.LocalStorage.SdaConfig.ConfigKeys.DBCODE__HIS_RS__HIS_GENDER__GENDER_CODE__FEMALE))
                {
                    currentPatientADO.GENDER_MALE = "";
                    currentPatientADO.GENDER_FEMALE = "X";
                }
                else
                {
                    currentPatientADO.GENDER_MALE = "X";
                    currentPatientADO.GENDER_FEMALE = "";
                }

                var gender = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == currentPatientADO.GENDER_ID);
                if (gender != null)
                {
                    currentPatientADO.GENDER_CODE = gender.GENDER_CODE;
                    currentPatientADO.GENDER_NAME = gender.GENDER_NAME;
                }

                var military = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MILITARY_RANK>().FirstOrDefault(o => o.ID == currentPatientADO.MILITARY_RANK_ID);
                if (military != null)
                {
                    currentPatientADO.MILITARY_RANK_CODE = military.MILITARY_RANK_CODE;
                    currentPatientADO.MILITARY_RANK_NAME = military.MILITARY_RANK_NAME;
                    currentPatientADO.NUM_ORDER = military.NUM_ORDER;
                }

                var work = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == currentPatientADO.WORK_PLACE_ID);
                if (work != null)
                {
                    currentPatientADO.WORK_PLACE_ADDRESS = work.ADDRESS;
                    currentPatientADO.WORK_PLACE_CODE = work.WORK_PLACE_CODE;
                    currentPatientADO.WORK_PLACE_NAME = work.WORK_PLACE_NAME;
                }

                //var bornType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BORN_TYPE>().FirstOrDefault(o => o.ID == currentPatientADO.BORN_TYPE_ID);
                //if (bornType != null)
                //{
                //    currentPatientADO.BORN_TYPE_CODE = bornType.BORN_TYPE_CODE;
                //    currentPatientADO.BORN_TYPE_NAME = bornType.BORN_TYPE_NAME;
                //}

                var career = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.ID == currentPatientADO.CAREER_ID);
                if (career != null)
                {
                    currentPatientADO.CAREER_CODE = career.CAREER_CODE;
                    currentPatientADO.CAREER_NAME = career.CAREER_NAME;
                }

                //var bloodAbo = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == currentPatientADO.BLOOD_ABO_ID);
                //if (bloodAbo != null)
                //{
                //    currentPatientADO.BLOOD_ABO_CODE = bloodAbo.BLOOD_ABO_CODE;
                //}

                //var bloodRh = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == currentPatientADO.BLOOD_RH_ID);
                //if (bloodRh != null)
                //{
                //    currentPatientADO.BLOOD_RH_CODE = bloodRh.BLOOD_RH_CODE;
                //}
            }
            catch (Exception ex)
            {
                currentPatientADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentPatientADO;
        }

        public static PatientADO getPatient(long treatmentId)
        {
            PatientADO currentPatientADO = new PatientADO();
            MOS.EFMODEL.DataModels.V_HIS_PATIENT patient = new V_HIS_PATIENT();
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentHisTreatment = new MOS.EFMODEL.DataModels.V_HIS_TREATMENT();
            try
            {
                MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                hisTreatmentFilter.ID = treatmentId;

                CommonParam param = new CommonParam();
                var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                if (treatments != null && treatments.Count > 0)
                {
                    currentHisTreatment = treatments.FirstOrDefault();

                    MOS.Filter.HisPatientViewFilter patientViewFilter = new MOS.Filter.HisPatientViewFilter();
                    patientViewFilter.ID = currentHisTreatment.PATIENT_ID;
                    patient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientViewFilter, param).SingleOrDefault();

                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_PATIENT, PatientADO>();
                    currentPatientADO = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_PATIENT, PatientADO>(patient);
                    //currentPatientADO.CMND_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientADO.CMND_DATE ?? 0);
                    currentPatientADO.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientADO.DOB);
                    currentPatientADO.AGE = AgeHelper.CalculateAgeFromYear(currentPatientADO.DOB);
                    if (currentPatientADO != null && currentPatientADO.DOB > 0)
                    {
                        currentPatientADO.DOB_YEAR = currentPatientADO.DOB.ToString().Substring(0, 4);
                    }
                    if (currentPatientADO.GENDER_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Inventec.Common.LocalStorage.SdaConfig.ConfigKeys.DBCODE__HIS_RS__HIS_GENDER__GENDER_CODE__FEMALE))
                    {
                        currentPatientADO.GENDER_MALE = "";
                        currentPatientADO.GENDER_FEMALE = "X";
                    }
                    else
                    {
                        currentPatientADO.GENDER_MALE = "X";
                        currentPatientADO.GENDER_FEMALE = "";
                    }
                }
            }
            catch (Exception ex)
            {
                currentPatientADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentPatientADO;
        }

        public static string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;

        }

        public static void LoadCurrentPatientTypeAlter(long treatmentId, long intructionTime, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                //CommonParam param = new CommonParam();
                //HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                //filter.TreatmentId = treatmentId;
                //if (intructionTime > 0)
                //    filter.InstructionTime = intructionTime;
                //else
                //    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                //hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                filter.TREATMENT_ID = treatmentId;
                var patyAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                hisPatientTypeAlter = patyAlter.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Tính số ngày điều trị
        public static long? GetToTalDayTreatments(long treatmentId, List<V_HIS_DEPARTMENT_TRAN> departmentTrans, long? treatmentEndTypeId, long? treatmentResultId)
        {
            long? soNgayDieuTri = 0;
            Config.HisConfigCFG.LoadConfig();
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                filterPatienTypeAlter.TREATMENT_ID = treatmentId;
                List<V_HIS_PATIENT_TYPE_ALTER> patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_VIEW, ApiConsumers.MosConsumer, filterPatienTypeAlter, param);
                var patientType = GetFirstTreat(patientTypeAlter);
                if (patientType != null)
                {
                    if (departmentTrans == null || departmentTrans.Count == 0)
                    {
                        departmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETHOSPITALINOUT, ApiConsumers.MosConsumer, treatmentId, param);
                    }
                    if (departmentTrans != null && departmentTrans.Count > 0 && departmentTrans[departmentTrans.Count - 1].DEPARTMENT_IN_TIME != null)

                        if (patientType.PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
                        {
                            soNgayDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(patientType.LOG_TIME, departmentTrans[departmentTrans.Count - 1].DEPARTMENT_IN_TIME ?? 0, treatmentEndTypeId, treatmentResultId, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                        }
                        else
                        {
                            soNgayDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(patientType.LOG_TIME, departmentTrans[departmentTrans.Count - 1].DEPARTMENT_IN_TIME ?? 0, treatmentEndTypeId, treatmentResultId, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                        }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                soNgayDieuTri = 0;
            }
            return soNgayDieuTri;
        }

        private static V_HIS_PATIENT_TYPE_ALTER GetFirstTreat(List<V_HIS_PATIENT_TYPE_ALTER> patientTypeAlters)
        {
            return patientTypeAlters.Where(o => !string.IsNullOrWhiteSpace(o.HEIN_TREATMENT_TYPE_CODE) && o.HEIN_TREATMENT_TYPE_CODE.Equals(HeinTreatmentTypeCode.TREAT)).OrderBy(o => o.LOG_TIME).FirstOrDefault();
        }

        public static TreatmentADO getTreatment(long treatmentId)
        {
            CommonParam param = new CommonParam();
            TreatmentADO treatmentADO = new TreatmentADO();
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentHisTreatment = new MOS.EFMODEL.DataModels.V_HIS_TREATMENT();
            try
            {
                MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                hisTreatmentFilter.ID = treatmentId;
                var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                if (treatments != null && treatments.Count > 0)
                {
                    currentHisTreatment = treatments.FirstOrDefault();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TREATMENT, TreatmentADO>();
                    treatmentADO = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_TREATMENT, TreatmentADO>(currentHisTreatment);
                }
            }
            catch (Exception ex)
            {
                treatmentADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatmentADO;
        }

        // lay thong tin chuyen khoa
        public static V_HIS_DEPARTMENT_TRAN getDepartmentTran(long treatmentId)
        {
            V_HIS_DEPARTMENT_TRAN departmentTranADO = new V_HIS_DEPARTMENT_TRAN();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDepartmentTranViewFilter hisDepartmentTranFilter = new HisDepartmentTranViewFilter();
                hisDepartmentTranFilter.TREATMENT_ID = treatmentId;
                hisDepartmentTranFilter.ORDER_FIELD = "DESC";
                var departmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW, ApiConsumers.MosConsumer, hisDepartmentTranFilter, param);
                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    departmentTranADO = departmentTrans.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return departmentTranADO;
        }

        public static HIS_DHST getDhst(long treatmentId)
        {
            HIS_DHST dhst = new HIS_DHST();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDhstFilter hisDhstFilter = new HisDhstFilter();
                hisDhstFilter.TREATMENT_ID = treatmentId;
                dhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, hisDhstFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                dhst = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dhst;
        }
    }
}
