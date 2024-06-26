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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.InformationAllowGoHome.Sda;
using HIS.Desktop.Plugins.InformationAllowGoHome.UCCauseOfDeath;
using HIS.Desktop.Plugins.InformationAllowGoHome.UCCauseOfDeath.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InformationAllowGoHome
{
    public partial class frmInformationAllowGoHome : FormBase
    {
        Inventec.Desktop.Common.Modules.Module module;
        UCCauseOfDeathProcessor causeOfDeathProcessor { get; set; }
        UserControl ucCauseOfDeath { get; set; }
        CauseOfDeathADO causeOfDeathAdo { get; set; }
        Action<CauseOfDeathADO> causeReult { get; set; }

        long treatmentId;
        HIS_TREATMENT currentHisTreatment { get; set; }
        bool isSave { get; set; }
        public frmInformationAllowGoHome(Inventec.Desktop.Common.Modules.Module module, long _TreatmentId, bool _IsSave)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.treatmentId = _TreatmentId;
                this.isSave = _IsSave;
                this.module = module;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void frmInformationAllowGoHome_Load(object sender, EventArgs e)
        {
            try
            {
                GetTreatment(this.treatmentId);
                InitUcControl();
                SetEnableControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetEnableControl()
        {
            try
            {
                btnSave.Enabled = isSave;
                btnPrint.Enabled = causeOfDeathAdo.SevereIllNessInfo != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitUcControl()
        {
            try
            {
                causeOfDeathAdo = new CauseOfDeathADO();
                causeOfDeathAdo.Treatment = currentHisTreatment;
                var severeIllnessInfo = GetSevereIllnessInfo(this.treatmentId);
                if (severeIllnessInfo != null)
                {
                    causeOfDeathAdo.SevereIllNessInfo = severeIllnessInfo;
                    causeOfDeathAdo.ListEventsCausesDeath = GetListEventsCausesDeath(severeIllnessInfo.ID);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("CauseOfDeathADO__Input___:", causeOfDeathAdo));
                causeOfDeathProcessor = new UCCauseOfDeathProcessor();
                ucCauseOfDeath = (UserControl)causeOfDeathProcessor.Run(causeOfDeathAdo);
                causeOfDeathProcessor.SetValue(ucCauseOfDeath, causeOfDeathAdo);
                ucCauseOfDeath.Dock = DockStyle.Fill;
                panelControl.Controls.Clear();
                panelControl.Controls.Add(this.ucCauseOfDeath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private HIS_SEVERE_ILLNESS_INFO GetSevereIllnessInfo(long _TreatmentId)
        {
            HIS_SEVERE_ILLNESS_INFO rs = null;
            try
            {
                HisSevereIllnessInfoFilter ft = new HisSevereIllnessInfoFilter();
                ft.TREATMENT_ID = _TreatmentId;
                ft.IS_DEATH = false;
                rs = new BackendAdapter(new CommonParam()).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, ft, null).FirstOrDefault();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private void GetTreatment(long _TreatmentId)
        {
            try
            {
                HisTreatmentFileFilter ft = new HisTreatmentFileFilter();
                ft.ID = _TreatmentId;
                currentHisTreatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, ft, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
        private List<HIS_EVENTS_CAUSES_DEATH> GetListEventsCausesDeath(long _SsIllnessInfoId)
        {
            List<HIS_EVENTS_CAUSES_DEATH> rs = null;
            try
            {
                HisEventsCausesDeathFilter ft = new HisEventsCausesDeathFilter();
                ft.SEVERE_ILLNESS_INFO_ID = _SsIllnessInfoId;
                rs = new BackendAdapter(new CommonParam()).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, ft, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;
                CauseOfDeathADO cause = causeOfDeathProcessor.GetValue(this.ucCauseOfDeath) as CauseOfDeathADO;
                CallApiSevereIllnessInfo(cause);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallApiSevereIllnessInfo(CauseOfDeathADO cause)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                SevereIllnessInfoSDO sdo = new SevereIllnessInfoSDO();
                sdo.SevereIllnessInfo = cause.SevereIllNessInfo;
                sdo.SevereIllnessInfo.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.module.RoomId).DepartmentId;
                sdo.EventsCausesDeaths = cause.ListEventsCausesDeath;
                var dt = new BackendAdapter(param)
                   .Post<HisServiceReqExamUpdateResultSDO>("api/HisSevereIllnessInfo/CreateOrUpdate", ApiConsumers.MosConsumer, sdo, param);
                if (dt != null)
                {
                    success = true;
                    btnPrint.Enabled = true;
                    string message = string.Format("Lưu thông tin xin ra viện. TREATMENT_CODE: {0}", currentHisTreatment.TREATMENT_CODE);
                    string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    SdaEventLogCreate eventlog = new SdaEventLogCreate();
                    eventlog.Create(login, null, true, message);
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintMps000484();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void PrintMps000484()
        {
            try
            {
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate("Mps000484", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000484":
                        ProcessPrintMps000484(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private void ProcessPrintMps000484(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps000484");
                if (this.treatmentId > 0)
                {
                    WaitingManager.Show();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = treatmentId;
                    HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                        .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    HIS_SEVERE_ILLNESS_INFO SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    List<HIS_EVENTS_CAUSES_DEATH> lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    HIS_DEPARTMENT_TRAN departmentTran = new HIS_DEPARTMENT_TRAN();
                    HIS_DEPARTMENT department = new HIS_DEPARTMENT();
                    HIS_PATIENT_TYPE_ALTER patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    CommonParam param = new CommonParam();
                    HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                    filter.TREATMENT_ID = treatmentId;
                    var dtSevere = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dtSevere != null && dtSevere.Count > 0)
                    {
                        SevereIllnessInfo = dtSevere.FirstOrDefault(o => o.IS_DEATH == 0);
                        if (SevereIllnessInfo != null)
                        {
                            MOS.Filter.HisDepartmentTranFilter filterDepartmentTran = new HisDepartmentTranFilter();
                            filterDepartmentTran.TREATMENT_ID = this.treatmentId;
                            filterDepartmentTran.DEPARTMENT_ID = SevereIllnessInfo.DEPARTMENT_ID;
                            var datas = new BackendAdapter(null).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filterDepartmentTran, null);
                            if (datas != null && datas.Count > 0)
                                departmentTran = datas.Last();
                            MOS.Filter.HisDepartmentFilter filterDepartment = new HisDepartmentFilter();
                            filterDepartment.ID = SevereIllnessInfo.DEPARTMENT_ID;
                            var datasDepatment = new BackendAdapter(null).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filterDepartment, null);
                            if (datasDepatment != null && datasDepatment.Count > 0)
                                department = datasDepatment.First();
                            HisEventsCausesDeathFilter filterChild = new HisEventsCausesDeathFilter();
                            filterChild.SEVERE_ILLNESS_INFO_ID = SevereIllnessInfo.ID;
                            lstEvents = new BackendAdapter(param).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, filterChild, param);
                        }
                    }
                    HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                    patientTypeAlterFilter.TREATMENT_ID = treatmentId;
                    var patientTypeAlterData = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                    if (patientTypeAlterData != null && patientTypeAlterData.Count > 0)
                    {
                        patientTypeAlter = patientTypeAlterData[0];
                    }
                    HIS_PATIENT patient = GetPatientByID(treatment.PATIENT_ID);
                    HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    if (SevereIllnessInfo == null)
                        SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    if (lstEvents == null)
                        lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    if (patientTypeAlter == null)
                        patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    if (departmentTran == null)
                        departmentTran = new HIS_DEPARTMENT_TRAN();
                    if (department == null)
                        department = new HIS_DEPARTMENT();

                    WaitingManager.Hide();
                    MPS.Processor.Mps000484.PDO.Mps000484PDO pdo = new MPS.Processor.Mps000484.PDO.Mps000484PDO
                        (
                        SevereIllnessInfo,
                        lstEvents,
                        treatment,
                        patientTypeAlter,
                        patient,
                        departmentTran,
                        department,
                        branch,
                        BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == 1).OrderBy(o => o.ICD_CODE).ToList(),
                        BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().ToList(),
                        BackendDataWorker.Get<HIS_TREATMENT_RESULT>().ToList()
                        );
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment != null ? treatment.TREATMENT_CODE : "", printTypeCode);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });

                    //if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    //{
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    //}
                    //else
                    //{
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                    //}
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private HIS_PATIENT GetPatientByID(long id)
        {
            HIS_PATIENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = id;
                result = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
