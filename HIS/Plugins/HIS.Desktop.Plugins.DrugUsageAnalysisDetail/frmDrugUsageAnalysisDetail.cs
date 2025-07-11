using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DrugUsageAnalysisDetail
{
    public partial class frmDrugUsageAnalysisDetail : HIS.Desktop.Utility.FormBase
    {
        V_HIS_TREATMENT treatment = null;
        HIS_DRUG_USE_ANALYSIS drugUseAnalysis = null;

        Inventec.Desktop.Common.Modules.Module current = null;
        V_HIS_TRACKING trackingData = null;
        Tuple<bool, bool> isAlowEditPharmacistOrDoctor = null;
        HIS.Desktop.Common.DelegateSelectData delegateSelectData = null;
        public frmDrugUsageAnalysisDetail(
            Inventec.Desktop.Common.Modules.Module moduleData, 
            V_HIS_TRACKING tracking,
            Tuple<bool, bool> isAlow, 
            HIS.Desktop.Common.DelegateSelectData delegateRefreshData 
            )
            :base(moduleData)
        {
            InitializeComponent();
            try
            {
                current = moduleData;
                trackingData = tracking;
                isAlowEditPharmacistOrDoctor = isAlow;
                delegateSelectData = delegateRefreshData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmDrugUsageAnalysisDetail_Load(object sender, EventArgs e)
        {
            Inventec.Common.Logging.LogSystem.Info("isAlowEditPharmacist: " + isAlowEditPharmacistOrDoctor.Item1);
            Inventec.Common.Logging.LogSystem.Info("isAlowEditDoctor: " + isAlowEditPharmacistOrDoctor.Item2);
            SetRegion2EnabledState(); // Thiết lập trạng thái Enabled cho vùng 2 và 3
            SetDataCboPHARMACIST(cboPharmacist); // Combobox dược sĩ
            SetDataCboDOCTOR(cboDoctor); // Combobox bác sĩ
            LoadTreatmentById(); // Thông tin bệnh nhân, thông tin điều trị
            LoadDrugUseAnalysis(); // Thông tin can thiệp sử dụng thuốc
            SetDataFormcontrol(); 
        }

        private void SetDataFormcontrol()
        {
            try
            {
                if (treatment != null)
                {
                    lblTreatmentCode.Text = treatment.TREATMENT_CODE;
                    lblPatientName.Text = treatment.TDL_PATIENT_NAME;
                    if (treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        lblDob.Text = treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    else
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB
                            .ToString());
                    lblGender.Text = treatment.TDL_PATIENT_GENDER_NAME;
                }
                if (drugUseAnalysis != null)
                {
                    if (drugUseAnalysis.INTERVENTION_DATE.HasValue)
                    {
                        var dateValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(drugUseAnalysis.INTERVENTION_DATE.Value);
                        if (dateValue.HasValue)
                            dtInterventionDate.DateTime = dateValue.Value;
                    }
                    spinInterventionNumber.Value = drugUseAnalysis.INTERVENTION_NUMBER.HasValue ? drugUseAnalysis.INTERVENTION_NUMBER.Value : 0;
                    txtInterventionDrugs.Text = drugUseAnalysis.INTERVENED_DRUGS;
                    cboPharmacist.EditValue = drugUseAnalysis.PHARMACIST_LOGINNAME;
                    txtPharmacistOpinion.Text = drugUseAnalysis.PHARMACIST_OPINION;
                    cboDoctor.EditValue = drugUseAnalysis.DOCTOR_LOGINNAME;
                    radioGroup1.SelectedIndex = drugUseAnalysis.IS_AGREE.HasValue && drugUseAnalysis.IS_AGREE.Value == 1 ? 0 : 1;
                    txtDecisionReason.Text = drugUseAnalysis.DECISION_REASON;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatmentById()
        {           
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = trackingData.TREATMENT_ID; // treatmentId
                treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetRegion2EnabledState()
        {
            try
            {
                //var employee = BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                //bool isDoctor = employee?.IS_DOCTOR == 1;
                layoutControlGroupVung2.Enabled = isAlowEditPharmacistOrDoctor.Item1;
                layoutControlGroupVung3.Enabled = isAlowEditPharmacistOrDoctor.Item2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDrugUseAnalysis()
        {           
            try
            {
                CommonParam param = new CommonParam();
                HisDrugUseAnalysisFilter drugUseAnalysisFilter = new HisDrugUseAnalysisFilter();
                drugUseAnalysisFilter.TRACKING_ID = trackingData.ID; // trackingId
                drugUseAnalysisFilter.TDL_TREATMENT_ID = trackingData.TREATMENT_ID; // treatmentId 
                drugUseAnalysis = new BackendAdapter(param).Get<List<HIS_DRUG_USE_ANALYSIS>>("api/HisDrugUseAnalysis/Get", ApiConsumers.MosConsumer, drugUseAnalysisFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataCboPHARMACIST(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == 1 && o.IS_DOCTOR != 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 100, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ tên", 150, 2));
                //columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa", 150, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, true, 400);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
                cbo.Properties.ImmediatePopup = true;
                cbo.Properties.PopupFormMinSize = new System.Drawing.Size(400, cbo.Properties.PopupFormSize.Height);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataCboDOCTOR(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == 1 && o.IS_DOCTOR == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 100, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ tên", 150, 2));
                //columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa", 150, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, true, 400);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
                cbo.Properties.ImmediatePopup = true;
                cbo.Properties.PopupFormMinSize = new System.Drawing.Size(400, cbo.Properties.PopupFormSize.Height);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool isUpdate = drugUseAnalysis != null && drugUseAnalysis.ID > 0;
                if (!isUpdate)
                    drugUseAnalysis = new HIS_DRUG_USE_ANALYSIS();

                drugUseAnalysis.TRACKING_ID = trackingData.ID;
                drugUseAnalysis.TDL_TREATMENT_ID = trackingData.TREATMENT_ID;
                if (dtInterventionDate.DateTime != DateTime.MinValue)
                    drugUseAnalysis.INTERVENTION_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInterventionDate.DateTime);
                else
                    drugUseAnalysis.INTERVENTION_DATE = null;

                drugUseAnalysis.INTERVENTION_NUMBER = Convert.ToInt32(spinInterventionNumber.Value);
                drugUseAnalysis.INTERVENED_DRUGS = txtInterventionDrugs.Text.Trim();
                drugUseAnalysis.PHARMACIST_LOGINNAME = cboPharmacist.EditValue?.ToString();
                drugUseAnalysis.PHARMACIST_USERNAME = cboPharmacist.Properties.GetDisplayText(cboPharmacist.EditValue);
                drugUseAnalysis.PHARMACIST_OPINION = txtPharmacistOpinion.Text.Trim();
                drugUseAnalysis.DOCTOR_LOGINNAME = cboDoctor.EditValue?.ToString();
                drugUseAnalysis.DOCTOR_USERNAME = cboDoctor.Properties.GetDisplayText(cboDoctor.EditValue);
                drugUseAnalysis.IS_AGREE = radioGroup1.SelectedIndex == 0 ? (short)1 : (short?)null;
                drugUseAnalysis.DECISION_REASON = txtDecisionReason.Text.Trim();
                Inventec.Common.Logging.LogSystem.Info("drugUseAnalysis: " + drugUseAnalysis);

                HIS_DRUG_USE_ANALYSIS result = null;
                if (isUpdate)
                {
                    result = new BackendAdapter(param).Post<HIS_DRUG_USE_ANALYSIS>("api/HisDrugUseAnalysis/Update", ApiConsumers.MosConsumer, drugUseAnalysis, param);
                }
                else
                {
                    result = new BackendAdapter(param).Post<HIS_DRUG_USE_ANALYSIS>("api/HisDrugUseAnalysis/Create", ApiConsumers.MosConsumer, drugUseAnalysis, param);
                }

                if (result != null)
                {
                    this.DialogResult = DialogResult.OK;
                    if (delegateSelectData != null)
                    {
                        delegateSelectData(result);
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, result != null);
                if (result != null)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDoctor_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDoctor.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPharmacist_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboPharmacist.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPharmacist_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboPharmacist.Properties.Buttons[1].Visible = !string.IsNullOrEmpty(cboPharmacist.Text); 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDoctor_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDoctor.Properties.Buttons[1].Visible = !string.IsNullOrEmpty(cboDoctor.Text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinInterventionNumber_EditValueChanged(object sender, EventArgs e)
        {
           if (spinInterventionNumber.Value < spinInterventionNumber.Properties.MinValue)
            {
                spinInterventionNumber.Value = spinInterventionNumber.Properties.MinValue;
            }
        }
    }
}
