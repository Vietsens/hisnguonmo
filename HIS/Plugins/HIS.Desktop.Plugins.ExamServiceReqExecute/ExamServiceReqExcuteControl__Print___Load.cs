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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
//using MPS.Old.Config;
using MPS.Processor.Mps000007.PDO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : System.Windows.Forms.UserControl
    {
        private void LoadPatient()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatment.PATIENT_ID;
                patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();
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
                CommonParam param = new CommonParam();
                HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.ID = this.HisServiceReqView.DHST_ID;
                dhst = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientTypeAlter()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                filterPatienTypeAlter.TREATMENT_ID = treatment.ID;
                patientTypeAlter = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filterPatienTypeAlter, param).OrderByDescending(o=>o.LOG_TIME).FirstOrDefault();
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
                    HisSereServView8Filter sereServFilter = new HisSereServView8Filter();
                    sereServFilter.TDL_TREATMENT_ID = this.HisServiceReqView.TREATMENT_ID;
                    SereServ8s = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_8>>("api/HisSereServ/GetView8", ApiConsumers.MosConsumer, sereServFilter, param);
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
                HisTreatmentFeeViewFilter treatmentFeeFilter = new HisTreatmentFeeViewFilter();
                treatmentFeeFilter.ID = this.HisServiceReqView.TREATMENT_ID;
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

        private V_HIS_TREATMENT LoadTreatmentView()
        {
            V_HIS_TREATMENT treatment = null;
            try
            {
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
                            && (o.HEIN_LIMIT_PRICE != null ? o.HEIN_LIMIT_PRICE * o.AMOUNT : o.VIR_PRICE_NO_ADD_PRICE * o.AMOUNT - o.VIR_TOTAL_HEIN_PRICE) > 0)).ToList();
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

        private Dictionary<string, List<V_HIS_SERE_SERV_8>> GroupSereServByPatyAlterBhyt(List<V_HIS_SERE_SERV_8> listSereServ)
        {
            Dictionary<string, List<V_HIS_SERE_SERV_8>> dicSereServ = new Dictionary<string, List<V_HIS_SERE_SERV_8>>();
            try
            {

                foreach (V_HIS_SERE_SERV_8 s in listSereServ)
                {
                    if (s.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT && s.JSON_PATIENT_TYPE_ALTER != null)
                    {
                        HIS_PATIENT_TYPE_ALTER patyAlter = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(s.JSON_PATIENT_TYPE_ALTER);
                        if (patyAlter != null)
                        {
                            string key = this.ToString(patyAlter);
                            List<V_HIS_SERE_SERV_8> list;
                            if (dicSereServ.ContainsKey(key))
                            {
                                list = dicSereServ[key];
                            }
                            else
                            {
                                list = new List<V_HIS_SERE_SERV_8>();
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

    }
}
