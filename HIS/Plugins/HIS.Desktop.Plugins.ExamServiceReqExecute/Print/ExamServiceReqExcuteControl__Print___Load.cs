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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
//using MPS.Old.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private void LoadPatient()
        {
            try
            {
                if (this.patient == null || this.patient.ID == 0)
                {
                    CommonParam param = new CommonParam();
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = treatment.PATIENT_ID;
                    var patients = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param);

                    this.patient = patients.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDHST()
        {
            try
            {
                if (this.HisServiceReqView.DHST_ID.HasValue)
                {
                    CommonParam param = new CommonParam();
                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.ID = this.HisServiceReqView.DHST_ID;
                    dhst = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceReqView()
        {
            CommonParam param = new CommonParam();
            HisServiceReqViewFilter examServiceReqFilter = new HisServiceReqViewFilter();
            examServiceReqFilter.ID = this.HisServiceReqView.ID;
            this.HisServiceReqView = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, examServiceReqFilter, param).FirstOrDefault();
        }

        private void LoadPatientTypeAlter()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                filterPatienTypeAlter.TREATMENT_ID = treatment.ID;
                patientTypeAlter = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filterPatienTypeAlter, param).OrderByDescending(o => o.LOG_TIME).FirstOrDefault();

                var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == (treatment.IN_TREATMENT_TYPE_ID ?? 0));
                if (treatmentType != null && patientTypeAlter != null)
                {
                    patientTypeAlter.TREATMENT_TYPE_ID = treatmentType.ID;
                    patientTypeAlter.TREATMENT_TYPE_CODE = treatmentType.TREATMENT_TYPE_CODE;
                    patientTypeAlter.TREATMENT_TYPE_NAME = treatmentType.TREATMENT_TYPE_NAME;
                }
                //HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDepartmentTran()
        {
            try
            {
                CommonParam param = new CommonParam();
                //Lấy thông tin chuyển khoa
                HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                departmentTranFilter.TREATMENT_ID = treatmentId;
                departmentTrans = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departmentTranFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void LoadTranPati()
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        // lấy thông tin chuyển viện
        //        MOS.Filter.HisTranPatiViewFilter hisTranPatiFilter = new HisTranPatiViewFilter();
        //        hisTranPatiFilter.TREATMENT_ID = treatmentId;

        //        tranPatie = new BackendAdapter(param)
        //            .Get<List<MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI>>("api/HisTranPati/GetView", ApiConsumers.MosConsumer, hisTranPatiFilter, param).FirstOrDefault();

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void LoadExpMest()
        {
            try
            {
                CommonParam param = new CommonParam();
                //thuoc
                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.TDL_TREATMENT_ID = treatmentId;

                expMests = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServ()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.TDL_TREATMENT_ID = treatment.ID;
                sereServMedis = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, param).Where(o => o.MEDICINE_ID != null).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServ8()
        {
            try
            {
                if (SereServ8s == null || SereServ8s.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    HisSereServFilter sereServFilter = new HisSereServFilter();
                    sereServFilter.TREATMENT_ID = this.HisServiceReqView.TREATMENT_ID;
                    SereServ8s = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, sereServFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private List<V_HIS_TREATMENT_FEE> LoadTreatmentFee()
        {
            List<V_HIS_TREATMENT_FEE> treatmentFees = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter treatmentFeeFilter = new HisTreatmentFeeViewFilter();
                treatmentFeeFilter.ID = this.treatment.ID;
                treatmentFees = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, treatmentFeeFilter, param);
            }
            catch (Exception ex)
            {
                treatmentFees = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatmentFees;
        }

        private V_HIS_TREATMENT_FEE_4 GetTreatmentFee4()
        {
            V_HIS_TREATMENT_FEE_4 treatmentFee = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeView4Filter treatmentFeeFilter = new HisTreatmentFeeView4Filter();
                treatmentFeeFilter.ID = this.HisServiceReqView.TREATMENT_ID;
                treatmentFee = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE_4>>("api/HisTreatment/GetFeeView4", ApiConsumers.MosConsumer, treatmentFeeFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                treatmentFee = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatmentFee;
        }

        private V_HIS_TREATMENT LoadTreatmentView()
        {
            V_HIS_TREATMENT treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = this.HisServiceReqView.TREATMENT_ID;
                treatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return treatment;
        }

        private void CheckBordereauType(ref bool isPhoiBHYT, ref bool isPhoiVienPhi)
        {
            try
            {
                if (SereServ8s != null && SereServ8s.Count > 0)
                {
                    long patientTypeIdBHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                    var sereServ_BHYT = SereServ8s.Where(o =>
                        o.PATIENT_TYPE_ID == patientTypeIdBHYT).ToList();

                    if (sereServ_BHYT != null && sereServ_BHYT.Count > 0)
                    {
                        isPhoiBHYT = true;
                    }

                    var sereServ_VienPhi = SereServ8s.Where(
                        o => (o.PATIENT_TYPE_ID != patientTypeIdBHYT)
                            || (o.PATIENT_TYPE_ID == patientTypeIdBHYT
                            && ((o.HEIN_LIMIT_PRICE != null && o.HEIN_LIMIT_PRICE > 0) ? o.HEIN_LIMIT_PRICE * o.AMOUNT : o.VIR_PRICE_NO_ADD_PRICE * o.AMOUNT - o.VIR_TOTAL_HEIN_PRICE) > 0)).ToList();
                    if (sereServ_VienPhi != null && sereServ_VienPhi.Count > 0)
                    {
                        isPhoiVienPhi = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private Dictionary<string, List<HIS_SERE_SERV>> GroupSereServByPatyAlterBhyt(List<HIS_SERE_SERV> listSereServ)
        {
            Dictionary<string, List<HIS_SERE_SERV>> dicSereServ = new Dictionary<string, List<HIS_SERE_SERV>>();
            try
            {

                foreach (HIS_SERE_SERV s in listSereServ)
                {
                    if (s.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT && s.JSON_PATIENT_TYPE_ALTER != null)
                    {
                        HIS_PATIENT_TYPE_ALTER patyAlter = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(s.JSON_PATIENT_TYPE_ALTER);
                        if (patyAlter != null)
                        {
                            string key = this.ToString(patyAlter);
                            List<HIS_SERE_SERV> list;
                            if (dicSereServ.ContainsKey(key))
                            {
                                list = dicSereServ[key];
                            }
                            else
                            {
                                list = new List<HIS_SERE_SERV>();
                                dicSereServ.Add(key, list);
                            }
                            list.Add(s);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dicSereServ = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dicSereServ;
        }

        private string ToString(HIS_PATIENT_TYPE_ALTER patyAlter)
        {
            if (patyAlter != null)
            {
                return NVL(patyAlter.HEIN_CARD_NUMBER) + "|"
                    + NVL(patyAlter.HEIN_MEDI_ORG_CODE) + "|"
                    + NVL(patyAlter.LEVEL_CODE) + "|"
                    + NVL(patyAlter.RIGHT_ROUTE_CODE) + "|"
                    + NVL(patyAlter.RIGHT_ROUTE_TYPE_CODE) + "|"
                    + NVL(patyAlter.JOIN_5_YEAR) + "|"
                    + NVL(patyAlter.PAID_6_MONTH) + "|"
                    + NVL(patyAlter.LIVE_AREA_CODE) + "|"
                    + NVL(patyAlter.HNCODE);
            }
            return null;
        }

        private string NVL(string s)
        {
            return !string.IsNullOrWhiteSpace(s) ? s : "";
        }

        public string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
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


    }
}
