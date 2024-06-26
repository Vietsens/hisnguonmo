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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TYT.EFMODEL.DataModels;

namespace TYT.Desktop.Plugins.TYTFetusBorn
{
    public partial class frmFetusBorn : HIS.Desktop.Utility.FormBase
    {
        private int positionHandle = -1;
        private string LoginName;
        private bool IsAdmin;

        internal Inventec.Desktop.Common.Modules.Module currentModule;

        V_HIS_PATIENT patient { get; set; }

        TYT_FETUS_BORN feTusBorn { get; set; }

        TYT_FETUS_BORN UpdateData { get; set; }

        int action = 0;

        public frmFetusBorn()
        {
            InitializeComponent();
        }

        public frmFetusBorn(Inventec.Desktop.Common.Modules.Module _currentModule, TYT.EFMODEL.DataModels.TYT_FETUS_BORN __FETUS_EXAM)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this.feTusBorn = __FETUS_EXAM;
            this.UpdateData = __FETUS_EXAM;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }
            this.action = 2;
        }

        public frmFetusBorn(Inventec.Desktop.Common.Modules.Module _currentModule, V_HIS_PATIENT _patient)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this.patient = _patient;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }

            this.action = 1;
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frm_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                this.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim();
                this.IsAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(LoginName);

                SetDefault();

                FillDataToControl(this.feTusBorn);

                CheckPatient();

                ProcessDataGird();

                EnableControlChanged(this.action);

                ValidControls();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckPatient()
        {
            try
            {
                if (this.patient == null && this.feTusBorn != null)
                {
                    MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                    filter.PATIENT_CODE__EXACT = this.feTusBorn.PATIENT_CODE;
                    var apiResult = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        this.patient = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataGird()
        {
            try
            {
                List<TYT_FETUS_BORN> listFetusBorn = GetListFetusBorn();

                gridControl1.BeginUpdate();
                gridControl1.DataSource = null;
                gridControl1.DataSource = listFetusBorn;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<TYT_FETUS_BORN> GetListFetusBorn()
        {
            List<TYT_FETUS_BORN> result = null;
            try
            {
                if (this.patient != null)
                {
                    TYT.Filter.TytFetusBornFilter filter = new Filter.TytFetusBornFilter();
                    filter.PATIENT_CODE__EXACT = this.patient.PATIENT_CODE;
                    result = new BackendAdapter(new CommonParam()).Get<List<TYT_FETUS_BORN>>("api/TytFetusBorn/Get", ApiConsumers.TytConsumer, filter, null);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetDefault()
        {
            try
            {
                action = GlobalVariables.ActionAdd;
                chkIsFetusManage.Focus();
                chkIsFetusManage.BackColor = Color.Gray;
                chkIsFetusManage.CheckState = CheckState.Unchecked;
                chkIsUVFull.CheckState = CheckState.Unchecked;
                spinParaNormalCount.EditValue = null;
                spinParaPrematurelyCount.EditValue = null;
                spinParaMiscarriageCount.EditValue = null;
                spinParaChildCount.EditValue = null;
                chkHIVBefore.CheckState = CheckState.Unchecked;
                chkHIVBorn.CheckState = CheckState.Unchecked;
                chkCheckCase33.CheckState = CheckState.Unchecked;
                chkCheckCase43.CheckState = CheckState.Unchecked;
                chkIsChildDeath.CheckState = CheckState.Unchecked;
                chkCheckCaseHaveNotYet.CheckState = CheckState.Unchecked;
                txtFirstWeekExam.Text = "";
                chkIsDeath.CheckState = CheckState.Unchecked;
                txtObstetricComplication.Text = "";
                rdoFemale.CheckState = CheckState.Unchecked;
                rdoMale.CheckState = CheckState.Unchecked;
                spinChildWeight.EditValue = null;
                txtNote.Text = "";
                txtExam742.Text = "";
                txtBornMethod.Text = "";
                txtMidWifeName.Text = "";
                txtChildStatus.Text = "";
                txtBornPlace.Text = "";
                chkIsBreastFeedingFirstHour.CheckState = CheckState.Unchecked;
                chkIsK1.CheckState = CheckState.Unchecked;

                this.dxValidationProvider.RemoveControlError(txtNote);
                this.dxValidationProvider.RemoveControlError(txtExam742);
                this.dxValidationProvider.RemoveControlError(txtBornMethod);
                this.dxValidationProvider.RemoveControlError(txtMidWifeName);
                this.dxValidationProvider.RemoveControlError(txtChildStatus);
                this.dxValidationProvider.RemoveControlError(txtBornPlace);
                this.dxValidationProvider.RemoveControlError(txtFirstWeekExam);
                this.dxValidationProvider.RemoveControlError(txtObstetricComplication);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl(TYT_FETUS_BORN feTusBorn)
        {
            try
            {
                if (feTusBorn != null)
                {
                    action = GlobalVariables.ActionEdit;
                    chkIsFetusManage.CheckState = feTusBorn.IS_FETUS_MANAGE == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkIsUVFull.CheckState = feTusBorn.IS_UV_FULL == 1 ? CheckState.Checked : CheckState.Unchecked;
                    spinParaNormalCount.EditValue = feTusBorn.PARA_NORMAL_COUNT;
                    spinParaPrematurelyCount.EditValue = feTusBorn.PARA_PREMATURELY_COUNT;
                    spinParaMiscarriageCount.EditValue = feTusBorn.PARA_MISCARRIAGE_COUNT;
                    spinParaChildCount.EditValue = feTusBorn.PARA_CHILD_COUNT;
                    chkHIVBefore.CheckState = feTusBorn.IS_HIV_BEFORE == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkHIVBorn.CheckState = feTusBorn.IS_HIV_BORN == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkCheckCase33.CheckState = feTusBorn.CHECK_CASE == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkCheckCase43.CheckState = feTusBorn.CHECK_CASE == 2 ? CheckState.Checked : CheckState.Unchecked;
                    chkCheckCaseHaveNotYet.CheckState = feTusBorn.CHECK_CASE == null ? CheckState.Checked : CheckState.Unchecked;
                    txtFirstWeekExam.Text = feTusBorn.FIRST_WEEK_EXAM;
                    chkIsDeath.CheckState = feTusBorn.IS_DEATH == 1 ? CheckState.Checked : CheckState.Unchecked;
                    txtObstetricComplication.Text = feTusBorn.OBSTETRIC_COMPLICATION;
                    rdoFemale.CheckState = feTusBorn.CHILD_GENDER_ID == 1 ? CheckState.Checked : CheckState.Unchecked;
                    rdoMale.CheckState = feTusBorn.CHILD_GENDER_ID == 2 ? CheckState.Checked : CheckState.Unchecked;
                    chkIsChildDeath.CheckState = feTusBorn.IS_CHILD_DEATH == 1 ? CheckState.Checked : CheckState.Unchecked;
                    spinChildWeight.EditValue = feTusBorn.CHILD_WEIGHT;
                    txtNote.Text = feTusBorn.NOTE;
                    txtBornMethod.Text = feTusBorn.BORN_METHOD;
                    txtMidWifeName.Text = feTusBorn.MIDWIFE_NAME;
                    txtChildStatus.Text = feTusBorn.CHILD_STATUS;
                    txtBornPlace.Text = feTusBorn.BORN_PLACE;
                    txtExam742.Text = feTusBorn.EXAM_742;
                    chkIsBreastFeedingFirstHour.CheckState = feTusBorn.IS_BREASTFEEDING_FIRST_HOUR == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkIsK1.CheckState = feTusBorn.IS_K1 == 1 ? CheckState.Checked : CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                btnSave.Focus();
                if (!btnSave.Enabled) return;

                this.positionHandle = -1;
                if (!dxValidationProvider.Validate()) return;

                success = SaveProcess(GlobalVariables.ActionEdit, ref param);
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool SaveProcess(int action, ref CommonParam param)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                TYT_FETUS_BORN senData = new TYT_FETUS_BORN();
                if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    if (this.UpdateData == null) return false;
                    Inventec.Common.Mapper.DataObjectMapper.Map<TYT_FETUS_BORN>(senData, this.UpdateData);
                }
                else
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<TYT_FETUS_BORN>(senData, this.patient);
                    senData.PERSON_ADDRESS = this.patient.VIR_ADDRESS;
                    senData.ID = 0;
                    senData.BHYT_NUMBER = this.patient.TDL_HEIN_CARD_NUMBER;
                    senData.VIR_PERSON_NAME = this.patient.VIR_PATIENT_NAME;
                }
                ProcessDataForSave(ref senData);

                TYT_FETUS_BORN _result = new TYT_FETUS_BORN();
                if (this.action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    _result = new BackendAdapter(param).Post<TYT_FETUS_BORN>("api/TytFetusBorn/Create", ApiConsumers.TytConsumer, senData, param);
                }
                else
                {
                    _result = new BackendAdapter(param).Post<TYT_FETUS_BORN>("api/TytFetusBorn/Update", ApiConsumers.TytConsumer, senData, param);
                }

                WaitingManager.Hide();
                if (_result != null && _result.ID > 0)
                {
                    result = true;
                    //this.feTusBorn = _result;
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ProcessDataGird();
                    EnableControlChanged(this.action);
                }
            }
            catch (Exception ex)
            {
                result = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessDataForSave(ref TYT_FETUS_BORN result)
        {
            try
            {
                if (result == null) result = new TYT_FETUS_BORN();

                if (chkIsFetusManage.CheckState == CheckState.Checked)
                {
                    result.IS_FETUS_MANAGE = 1;
                }
                else
                {
                    result.IS_FETUS_MANAGE = null;
                }

                if (chkIsUVFull.CheckState == CheckState.Checked)
                {
                    result.IS_UV_FULL = 1;
                }
                else
                {
                    result.IS_UV_FULL = null;
                }

                if (spinParaNormalCount.EditValue != null)
                {
                    result.PARA_NORMAL_COUNT = (long)spinParaNormalCount.Value;
                }
                else
                {
                    result.PARA_NORMAL_COUNT = null;
                }

                if (spinParaPrematurelyCount.EditValue != null)
                {
                    result.PARA_PREMATURELY_COUNT = (long)spinParaPrematurelyCount.Value;
                }
                else
                {
                    result.PARA_PREMATURELY_COUNT = null;
                }

                if (spinParaMiscarriageCount.EditValue != null)
                {
                    result.PARA_MISCARRIAGE_COUNT = (long)spinParaMiscarriageCount.Value;
                }
                else
                {
                    result.PARA_MISCARRIAGE_COUNT = null;
                }

                if (spinParaChildCount.EditValue != null)
                {
                    result.PARA_CHILD_COUNT = (long)spinParaChildCount.Value;
                }
                else
                {
                    result.PARA_CHILD_COUNT = null;
                }

                if (chkHIVBefore.CheckState == CheckState.Checked)
                {
                    result.IS_HIV_BEFORE = 1;
                }
                else
                {
                    result.IS_HIV_BEFORE = null;
                }

                if (chkHIVBorn.CheckState == CheckState.Checked)
                {
                    result.IS_HIV_BORN = 1;
                }
                else
                {
                    result.IS_HIV_BORN = null;
                }

                if (chkIsChildDeath.CheckState == CheckState.Checked)
                {
                    result.IS_CHILD_DEATH = 1;
                }
                else
                {
                    result.IS_CHILD_DEATH = null;
                }

                if (chkCheckCase33.CheckState == CheckState.Checked)
                {
                    result.CHECK_CASE = 1;
                }
                else if (chkCheckCase43.CheckState == CheckState.Checked)
                {
                    result.CHECK_CASE = 2;
                }
                else
                {
                    result.CHECK_CASE = null;
                }

                result.FIRST_WEEK_EXAM = txtFirstWeekExam.Text;

                if (chkIsDeath.CheckState == CheckState.Checked)
                {
                    result.IS_DEATH = 1;
                }
                else
                {
                    result.IS_DEATH = null;
                }

                result.OBSTETRIC_COMPLICATION = txtObstetricComplication.Text;

                if (rdoFemale.CheckState == CheckState.Checked)
                {
                    result.CHILD_GENDER_ID = 1;
                }
                else if (rdoMale.CheckState == CheckState.Checked)
                {
                    result.CHILD_GENDER_ID = 2;
                }
                else
                {
                    result.CHILD_GENDER_ID = null;
                }

                if (spinChildWeight.EditValue != null)
                {
                    result.CHILD_WEIGHT = (long)spinChildWeight.Value;
                }

                result.NOTE = txtNote.Text;
                result.EXAM_742 = txtExam742.Text;
                result.BORN_METHOD = txtBornMethod.Text;
                result.MIDWIFE_NAME = txtMidWifeName.Text;
                result.CHILD_STATUS = txtChildStatus.Text;
                result.BORN_PLACE = txtBornPlace.Text;

                if (chkIsBreastFeedingFirstHour.CheckState == CheckState.Checked)
                {
                    result.IS_BREASTFEEDING_FIRST_HOUR = 1;
                }
                else
                {
                    result.IS_BREASTFEEDING_FIRST_HOUR = null;
                }

                if (chkIsK1.CheckState == CheckState.Checked)
                {
                    result.IS_K1 = 1;
                }
                else
                {
                    result.IS_K1 = null;
                }

                result.BRANCH_CODE = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).BranchCode;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonI__Refesh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTrongLuong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinParaPrematurelyCount.Focus();
                    spinParaPrematurelyCount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinParaPrematurelyCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinParaMiscarriageCount.Focus();
                    spinParaMiscarriageCount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinParaMiscarriageCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinParaChildCount.Focus();
                    spinParaChildCount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinParaChildCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHIVBefore.Focus();
                    chkHIVBefore.Focus();
                    chkHIVBefore.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsFetusManage_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsUVFull.Focus();
                    chkIsUVFull.SelectAll();
                    chkIsUVFull.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsUVFull_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinParaNormalCount.Focus();
                    spinParaNormalCount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHIVBefore_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHIVBorn.Focus();
                    chkHIVBorn.Focus();
                    chkHIVBorn.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHIVBorn_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCheckCase33.Focus();
                    chkCheckCase33.Focus();
                    chkCheckCase33.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCheckCase33_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCheckCase43.Focus();
                    chkCheckCase43.SelectAll();
                    chkCheckCase43.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCheckCase43_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCheckCaseHaveNotYet.Focus();
                    chkCheckCaseHaveNotYet.Focus();
                    chkCheckCaseHaveNotYet.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCheckCaseHaveNotYet_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFirstWeekExam.Focus();
                    txtFirstWeekExam.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFirstWeekExam_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExam742.Focus();
                    txtExam742.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExam742_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsDeath.Focus();
                    chkIsDeath.SelectAll();
                    chkIsDeath.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsDeath_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtObstetricComplication.Focus();
                    txtObstetricComplication.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtObstetricComplication_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoFemale.Focus();
                    rdoFemale.SelectAll();
                    rdoFemale.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoFemale_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoMale.Focus();
                    rdoMale.SelectAll();
                    rdoMale.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoMale_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinChildWeight.Focus();
                    spinChildWeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinChildWeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBornMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMidWifeName.Focus();
                    txtMidWifeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMidWifeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBornPlace.Focus();
                    txtBornPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtChildStatus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsBreastFeedingFirstHour.Focus();
                    chkIsBreastFeedingFirstHour.SelectAll();
                    chkIsBreastFeedingFirstHour.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBornPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtChildStatus.Focus();
                    txtChildStatus.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsBreastFeedingFirstHour_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsK1.Focus();
                    chkIsK1.SelectAll();
                    chkIsK1.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsK1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void chkIsChildDeath_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    btnSave.Select();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsK1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsChildDeath.Focus();
                    chkIsChildDeath.SelectAll();
                    chkIsChildDeath.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsFetusManage_Leave(object sender, EventArgs e)
        {
            chkIsFetusManage.BackColor = Color.Transparent;
        }

        private void chkIsUVFull_Leave(object sender, EventArgs e)
        {
            chkIsUVFull.BackColor = Color.Transparent;
        }

        private void chkHIVBefore_Leave(object sender, EventArgs e)
        {
            chkHIVBefore.BackColor = Color.Transparent;
        }

        private void chkHIVBorn_Leave(object sender, EventArgs e)
        {
            chkHIVBorn.BackColor = Color.Transparent;
        }

        private void chkCheckCase33_Leave(object sender, EventArgs e)
        {
            try
            {
                chkCheckCase33.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkCheckCase43_Leave(object sender, EventArgs e)
        {
            try
            {
                chkCheckCase43.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkCheckCaseHaveNotYet_Leave(object sender, EventArgs e)
        {
            chkCheckCaseHaveNotYet.BackColor = Color.Transparent;
        }

        private void chkIsDeath_Leave(object sender, EventArgs e)
        {
            chkIsDeath.BackColor = Color.Transparent;
        }

        private void rdoFemale_Leave(object sender, EventArgs e)
        {
            rdoFemale.BackColor = Color.Transparent;
        }

        private void rdoMale_Leave(object sender, EventArgs e)
        {
            rdoMale.BackColor = Color.Transparent;
        }

        private void chkIsBreastFeedingFirstHour_Leave(object sender, EventArgs e)
        {
            chkIsBreastFeedingFirstHour.BackColor = Color.Transparent;
        }

        private void chkIsK1_Leave(object sender, EventArgs e)
        {
            chkIsK1.BackColor = Color.Transparent;
        }

        private void chkIsChildDeath_Leave(object sender, EventArgs e)
        {
            chkIsChildDeath.BackColor = Color.Transparent;
        }

        private void chkCheckCase33_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCheckCase33.CheckState == CheckState.Checked)
                {
                    chkCheckCase43.CheckState = CheckState.Unchecked;
                    chkCheckCaseHaveNotYet.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCheckCase43_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCheckCase43.CheckState == CheckState.Checked)
                {
                    chkCheckCase33.CheckState = CheckState.Unchecked;
                    chkCheckCaseHaveNotYet.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCheckCaseHaveNotYet_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCheckCaseHaveNotYet.CheckState == CheckState.Checked)
                {
                    chkCheckCase33.CheckState = CheckState.Unchecked;
                    chkCheckCase43.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoFemale_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoFemale.CheckState == CheckState.Checked)
                {
                    rdoMale.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoMale_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoMale.CheckState == CheckState.Checked)
                {
                    rdoFemale.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (TYT_FETUS_BORN)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "BREASTFEEDING_FIRST_HOUR")
                        {
                            if (data.IS_BREASTFEEDING_FIRST_HOUR.HasValue && data.IS_BREASTFEEDING_FIRST_HOUR.Value == 1)
                                e.Value = true;
                            else
                                e.Value = false;
                        }
                        else if (e.Column.FieldName == "CHILD_DEATH")
                        {
                            if (data.IS_CHILD_DEATH.HasValue && data.IS_CHILD_DEATH.Value == 1)
                                e.Value = true;
                            else
                                e.Value = false;
                        }
                        else if (e.Column.FieldName == "DEATH")
                        {
                            if (data.IS_DEATH.HasValue && data.IS_DEATH.Value == 1)
                                e.Value = true;
                            else
                                e.Value = false;
                        }
                        else if (e.Column.FieldName == "FETUS_MANAGE")
                        {
                            if (data.IS_FETUS_MANAGE.HasValue && data.IS_FETUS_MANAGE.Value == 1)
                                e.Value = true;
                            else
                                e.Value = false;
                        }
                        else if (e.Column.FieldName == "HIV_BEFORE")
                        {
                            if (data.IS_HIV_BEFORE.HasValue && data.IS_HIV_BEFORE.Value == 1)
                                e.Value = true;
                            else
                                e.Value = false;
                        }
                        else if (e.Column.FieldName == "HIV_BORN")
                        {
                            if (data.IS_HIV_BORN.HasValue && data.IS_HIV_BORN.Value == 1)
                                e.Value = true;
                            else
                                e.Value = false;
                        }
                        else if (e.Column.FieldName == "K1")
                        {
                            if (data.IS_K1.HasValue && data.IS_K1.Value == 1)
                                e.Value = true;
                            else
                                e.Value = false;
                        }
                        else if (e.Column.FieldName == "UV_FULL")
                        {
                            if (data.IS_UV_FULL.HasValue && data.IS_UV_FULL.Value == 1)
                                e.Value = true;
                            else
                                e.Value = false;
                        }
                        else if (e.Column.FieldName == "CHILD_GENDER")
                        {
                            if (data.CHILD_GENDER_ID.HasValue && data.CHILD_GENDER_ID.Value == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                                e.Value = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciChildGender__Male.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            else
                                e.Value = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciChildGender__Female.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        }
                        else if (e.Column.FieldName == "CHECK_CASE_STR")
                        {
                            if (data.CHECK_CASE.HasValue)
                            {
                                if (data.CHECK_CASE.Value == 1)
                                {
                                    string t = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciCheckCase33.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                    if (!String.IsNullOrWhiteSpace(t))
                                    {
                                        e.Value = t.Replace(':', ' ');
                                    }
                                }
                                else if (data.CHECK_CASE.Value == 2)
                                {
                                    string t = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciCheckCase43.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                    if (!String.IsNullOrWhiteSpace(t))
                                    {
                                        e.Value = t.Replace(':', ' ');
                                    }
                                }
                                else
                                {
                                    string t = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciCheckCaseHaveNotYet.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                    if (!String.IsNullOrWhiteSpace(t))
                                    {
                                        e.Value = t.Replace(':', ' ');
                                    }
                                }
                            }
                            else
                            {
                                string t = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciCheckCaseHaveNotYet.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                if (!String.IsNullOrWhiteSpace(t))
                                {
                                    e.Value = t.Replace(':', ' ');
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var row = (TYT_FETUS_BORN)gridView1.GetFocusedRow();
                if (row != null)
                {
                    this.UpdateData = row;
                    FillDataToControl(row);

                    EnableControlChanged(this.action);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                BtnAdd.Focus();
                if (!BtnAdd.Enabled) return;

                this.positionHandle = -1;
                if (!dxValidationProvider.Validate()) return;

                success = SaveProcess(GlobalVariables.ActionAdd, ref param);
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.SetDefault();

                EnableControlChanged(this.action);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnSave.Enabled = (action == GlobalVariables.ActionEdit);
                BtnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControls()
        {
            try
            {
                Validation.TextEditValidationRule note = new Validation.TextEditValidationRule();
                note.maxLenght = 100;
                note.txtEdit = txtNote;
                note.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtNote, note);

                Validation.TextEditValidationRule bornMethod = new Validation.TextEditValidationRule();
                bornMethod.maxLenght = 100;
                bornMethod.txtEdit = txtBornMethod;
                bornMethod.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtBornMethod, bornMethod);

                Validation.TextEditValidationRule bornPlace = new Validation.TextEditValidationRule();
                bornPlace.maxLenght = 100;
                bornPlace.txtEdit = txtBornPlace;
                bornPlace.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtBornPlace, bornPlace);

                Validation.TextEditValidationRule childStatus = new Validation.TextEditValidationRule();
                childStatus.maxLenght = 100;
                childStatus.txtEdit = txtChildStatus;
                childStatus.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtChildStatus, childStatus);

                Validation.TextEditValidationRule exam742 = new Validation.TextEditValidationRule();
                exam742.maxLenght = 100;
                exam742.txtEdit = txtExam742;
                exam742.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtExam742, exam742);

                Validation.TextEditValidationRule firstWeekExam = new Validation.TextEditValidationRule();
                firstWeekExam.maxLenght = 100;
                firstWeekExam.txtEdit = txtFirstWeekExam;
                firstWeekExam.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtFirstWeekExam, firstWeekExam);

                Validation.TextEditValidationRule midWifeName = new Validation.TextEditValidationRule();
                midWifeName.maxLenght = 100;
                midWifeName.txtEdit = txtMidWifeName;
                midWifeName.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtMidWifeName, midWifeName);

                Validation.TextEditValidationRule obstetricComplication = new Validation.TextEditValidationRule();
                obstetricComplication.maxLenght = 100;
                obstetricComplication.txtEdit = txtObstetricComplication;
                obstetricComplication.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtObstetricComplication, obstetricComplication);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (TYT_FETUS_BORN)gridView1.GetFocusedRow();
                if (row != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        var success = new BackendAdapter(param).Post<bool>("api/TytFetusBorn/Delete", ApiConsumers.TytConsumer, row.ID, param);
                        if (success)
                        {
                            ProcessDataGird();
                        }

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string loginname = (View.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();

                    if (e.Column.FieldName == "Delete")
                    {
                        if (this.IsAdmin || LoginName.ToLower() == loginname.ToLower())// || loginname.ToLower() != this.LoginName.ToLower())
                        {
                            e.RepositoryItem = repositoryItemButtonDelete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonDeleteDisable;
                        }
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
