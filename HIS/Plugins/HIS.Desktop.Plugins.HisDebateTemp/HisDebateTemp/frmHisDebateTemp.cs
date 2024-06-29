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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisDebateTemp.Validate;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisDebateTemp.Resources;
using System.Resources;
using Inventec.Common.Resource;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisDebateTemp.HisDebateTemp
{
    public partial class frmHisDebateTemp : FormBase
    {
        #region Declare avairable
        Module module;
        HIS_DEBATE HisDebate;
        HIS_DEBATE_TEMP HisDebateTemp;
        int RowCount = 0;
        int Startpage = 0;
        int dataTotal = 0;
        int ActionType = -1;
        HIS_DEBATE_TEMP currentdata;
        MOS.SDO.WorkPlaceSDO WorkPlaceSDO;
        #endregion

        public frmHisDebateTemp(Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.module = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this.ActionType = GlobalVariables.ActionAdd;
                }
                catch (Exception ex)
                {

                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        public frmHisDebateTemp(Module moduleData, HIS_DEBATE_TEMP HisDebateTemp)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.module = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this.ActionType = GlobalVariables.ActionAdd;
                    if (HisDebateTemp != null)
                    {
                        this.HisDebateTemp = HisDebateTemp;

                    }
                }
                catch (Exception ex)
                {

                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        public frmHisDebateTemp(Module moduleData, HIS_DEBATE HisDebate)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.module = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this.ActionType = GlobalVariables.ActionAdd;
                    if (HisDebateTemp != null)
                    {
                        this.HisDebate = HisDebate;

                    }
                }
                catch (Exception ex)
                {

                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void frmHisDebateTemp_Load(object sender, EventArgs e)
        {
            try
            {
                WorkPlaceSDO = WorkPlace.WorkPlaceSDO.Where(o => o.RoomId == module.RoomId && o.RoomTypeId == module.RoomTypeId).FirstOrDefault();
                MeShow();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        #region ---Button Click
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessors();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessors();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                SetDataDefault();
                EnableControlChanged(this.ActionType);
                RestFromData();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                txtDebateTempCode.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnRestFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtDebateTempCode.Focus();
                txtDebateTempCode.SelectAll();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnLOCK_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var datarow = (HIS_DEBATE_TEMP)gridViewDebateTemp.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    if (datarow != null)
                    {
                        var data = new BackendAdapter(param).Post<HIS_DEBATE_TEMP>(HisRequestUriStore.HisDebateTemp_ChangeLock, ApiConsumers.MosConsumer, datarow.ID, null);
                        if (data != null)
                        {
                            BackendDataWorker.Reset<HIS_DEBATE_TEMP>();
                            FillDataToGridControl();
                            success = true;
                            var check = BackendDataWorker.Get<HIS_DEBATE_TEMP>().FirstOrDefault(p => p.ID == data.ID && p.IS_ACTIVE != 1).ToString();
                            if (check != null)
                            {
                                btnEdit.Enabled = false;
                            }
                        }
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void btnUNLOCK_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var datarow = (HIS_DEBATE_TEMP)gridViewDebateTemp.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    if (datarow != null)
                    {
                        var data = new BackendAdapter(param).Post<HIS_DEBATE_TEMP>(HisRequestUriStore.HisDebateTemp_ChangeLock, ApiConsumers.MosConsumer, datarow.ID, null);
                        if (data != null)
                        {
                            BackendDataWorker.Reset<HIS_DEBATE_TEMP>();
                            FillDataToGridControl();
                            success = true;
                            var check = BackendDataWorker.Get<HIS_DEBATE_TEMP>().FirstOrDefault(p => p.ID == data.ID && p.IS_ACTIVE == 1).ToString();
                            if (check != null)
                            {
                                btnEdit.Enabled = false;
                            }
                        }
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var datarow = (HIS_DEBATE_TEMP)gridViewDebateTemp.GetFocusedRow();
                if (MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    if (datarow != null)
                    {
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HisDebateTemp_Delete, ApiConsumers.MosConsumer, datarow.ID, null);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_DEBATE_TEMP>();
                            FillDataToGridControl();
                            btnCancel_Click(null, null);
                        }
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        #endregion

        #region ---Set Data
        private void MeShow()
        {
            try
            {
                SetDataDefault();
                EnableControlChanged(this.ActionType);
                Validate();
                FillDataToGridControl();
                SetCapitonByLanguesKey();
                if (HisDebateTemp != null)
                {
                    FillDataEditorControl(HisDebateTemp);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetDataDefault()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtDebateTempCode.Text = "";
                txtDebateTempName.Text = "";
                txtPathologicalHistory.Text = "";
                txtHospitalizationState.Text = "";
                txtBeforeDiagnostic.Text = "";
                txtTreatmentTracking.Text = "";
                txtDiagnostic.Text = "";
                txtTreatmentMethod.Text = "";
                txtCareMethod.Text = "";
                txtConclusion.Text = "";
                txtDiscussion.Text = "";
                ChkIsPublic1.Checked = false;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int actionType)
        {
            try
            {
                btnAdd.Enabled = (actionType == GlobalVariables.ActionAdd);
                btnEdit.Enabled = (actionType == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pagingSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = RowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, pagingSize, this.gridControlDebateTemp);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                Startpage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramcommon = new CommonParam(Startpage, limit);
                ApiResultObject<List<HIS_DEBATE_TEMP>> apiResult = null;
                HisDebateTempFilter filter = new HisDebateTempFilter();

                SetFilter(ref filter);
                gridControlDebateTemp.DataSource = null;
                gridViewDebateTemp.BeginUpdate();
                apiResult = new BackendAdapter(paramcommon).GetRO<List<HIS_DEBATE_TEMP>>(HisRequestUriStore.HisDebateTemp_Get, ApiConsumers.MosConsumer, filter, paramcommon);
                if (apiResult != null)
                {
                    var data = (List<HIS_DEBATE_TEMP>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlDebateTemp.DataSource = data;
                        RowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewDebateTemp.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisDebateTempFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.KEY_WORD__CODE_OR_NAME = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ChangDataRow(HIS_DEBATE_TEMP dataRow)
        {
            try
            {
                if (dataRow != null)
                {
                    FillDataEditorControl(dataRow);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    if (dataRow != null)
                    {
                        btnEdit.Enabled = (dataRow.IS_ACTIVE == 1);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void FillDataEditorControl(HIS_DEBATE_TEMP dataRow)
        {
            try
            {
                if (dataRow != null)
                {

                    if (dataRow.DEBATE_TEMP_CODE != null)
                    {
                        txtDebateTempCode.Text = dataRow.DEBATE_TEMP_CODE;
                    }
                    else
                        txtDebateTempCode.Text = "";
                    if (dataRow.DEBATE_TEMP_NAME != null)
                    {
                        txtDebateTempName.Text = dataRow.DEBATE_TEMP_NAME;
                    }
                    else
                        txtDebateTempName.Text = "";
                    if (dataRow.IS_PUBLIC == 1)
                    {
                        ChkIsPublic1.CheckState = CheckState.Checked;
                    }
                    else
                        ChkIsPublic1.CheckState = CheckState.Unchecked;

                    if (dataRow.PATHOLOGICAL_HISTORY != null)
                    {
                        txtPathologicalHistory.Text = dataRow.PATHOLOGICAL_HISTORY;
                    }
                    else
                        txtPathologicalHistory.Text = "";

                    if (dataRow.HOSPITALIZATION_STATE != null)
                    {
                        txtHospitalizationState.Text = dataRow.HOSPITALIZATION_STATE;
                    }
                    else
                        txtHospitalizationState.Text = "";

                    if (dataRow.BEFORE_DIAGNOSTIC != null)
                    {
                        txtBeforeDiagnostic.Text = dataRow.BEFORE_DIAGNOSTIC;
                    }
                    else
                        txtBeforeDiagnostic.Text = "";
                    if (dataRow.TREATMENT_TRACKING != null)
                    {
                        txtTreatmentTracking.Text = dataRow.TREATMENT_TRACKING;
                    }
                    else
                        txtTreatmentTracking.Text = "";
                    if (dataRow.DIAGNOSTIC != null)
                    {
                        txtDiagnostic.Text = dataRow.DIAGNOSTIC;
                    }
                    else
                        txtDiagnostic.Text = "";

                    if (dataRow.TREATMENT_METHOD != null)
                    {
                        txtTreatmentMethod.Text = dataRow.TREATMENT_METHOD;
                    }
                    else
                        txtTreatmentMethod.Text = "";
                    if (dataRow.CARE_METHOD != null)
                    {
                        txtCareMethod.Text = dataRow.CARE_METHOD;
                    }
                    else
                        txtCareMethod.Text = "";

                    if (dataRow.CONCLUSION != null)
                    {
                        txtConclusion.Text = dataRow.CONCLUSION;
                    }
                    else

                        txtConclusion.Text = "";

                    if (dataRow.DISCUSSION != null)
                    {
                        txtDiscussion.Text = dataRow.DISCUSSION;
                    }
                    else

                        txtDiscussion.Text = "";
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private bool CheckCoincidence()
        {
            bool success = true;
            string DebateCode = txtDebateTempCode.Text.Trim();
            try
            {
                var data = BackendDataWorker.Get<HIS_DEBATE_TEMP>();
                foreach (var dt in data)
                {
                    if (dt.DEBATE_TEMP_CODE == DebateCode)
                    {
                        success = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
            return success;
        }

        private void UpdateDataFromForm(HIS_DEBATE_TEMP data)
        {
            try
            {
                data.DEBATE_TEMP_CODE = txtDebateTempCode.Text.Trim();
                data.IS_PUBLIC = (short)(ChkIsPublic1.Checked ? 1 : 0);
                data.DEBATE_TEMP_NAME = txtDebateTempName.Text.Trim();
                data.PATHOLOGICAL_HISTORY = txtPathologicalHistory.Text.Trim();
                data.HOSPITALIZATION_STATE = txtHospitalizationState.Text.Trim();
                data.BEFORE_DIAGNOSTIC = txtBeforeDiagnostic.Text.Trim();
                data.TREATMENT_TRACKING = txtTreatmentTracking.Text.Trim();
                data.DIAGNOSTIC = txtDiagnostic.Text.Trim();
                data.TREATMENT_METHOD = txtTreatmentMethod.Text.Trim();
                data.CARE_METHOD = txtCareMethod.Text.Trim();
                data.CONCLUSION = txtConclusion.Text.Trim();
                data.DISCUSSION = txtDiscussion.Text.Trim();
                data.DEPARTMENT_ID = WorkPlaceSDO.DepartmentId;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SaveProcessors()
        {
            try
            {
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HIS_DEBATE_TEMP UpdateDTO = new HIS_DEBATE_TEMP();
                UpdateDataFromForm(UpdateDTO);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    if (CheckCoincidence())
                    {
                        if (HisDebateTemp != null && HisDebateTemp.HIS_DEBATE_USER != null && HisDebateTemp.HIS_DEBATE_USER.Count > 0)
                        {
                            UpdateDTO.HIS_DEBATE_USER = HisDebateTemp.HIS_DEBATE_USER;
                        }
                        var apiResult = new BackendAdapter(param).Post<HIS_DEBATE_TEMP>(HisRequestUriStore.HisDebateTemp_Create, ApiConsumers.MosConsumer, UpdateDTO, param);
                        if (apiResult != null)
                        {
                            BackendDataWorker.Reset<HIS_DEBATE_TEMP>();
                            FillDataToGridControl();
                            success = true;
                            btnCancel_Click(null, null);
                        }

                    }
                    else
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Xử lý thất bại. Mã biên bản hội chuẩn mẫu đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {
                    if (currentdata != null)
                    {
                        UpdateDTO.ID = currentdata.ID;
                        var apiResult = new BackendAdapter(param).Post<HIS_DEBATE_TEMP>(HisRequestUriStore.HisDebateTemp_Update, ApiConsumers.MosConsumer, UpdateDTO, param);
                        if (apiResult != null)
                        {
                            BackendDataWorker.Reset<HIS_DEBATE_TEMP>();
                            FillDataToGridControl();
                            success = true;
                        }
                    }

                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void RestFromData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized)
                    return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && ChkIsPublic1.Text != "Công khai" && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtDebateTempCode.Focus();
                            txtDebateTempCode.SelectAll();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
                ChkIsPublic1.Checked = false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCapitonByLanguesKey()
        {
            try
            {
                if (this.module != null && !String.IsNullOrEmpty(this.module.text))
                {
                    this.Text = module.text;
                }
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisDebateTemp.Resources.Lang", typeof(HIS.Desktop.Plugins.HisDebateTemp.HisDebateTemp.frmHisDebateTemp).Assembly);
                this.btnAdd.Text = Get.Value("frmHisDebateTemp.btnAdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Get.Value("frmHisDebateTemp.btnEdit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Get.Value("frmHisDebateTemp.btnCancel.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Get.Value("frmHisDebateTemp.btnSearch.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcHisDebateTempCode.Text = Get.Value("frmHisDebateTemp.lcHisDebateTempCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcDebateTempName.Text = Get.Value("frmHisDebateTemp.lcDebateTempName.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcPathologicalHistory.Text = Get.Value("frmHisDebateTemp.lcPathologicalHistory.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHospitalizationState.Text = Get.Value("frmHisDebateTemp.lciHospitalizationState.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBeforeDiagnostic.Text = Get.Value("frmHisDebateTemp.lciBeforeDiagnostic.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcTreatmentTracking.Text = Get.Value("frmHisDebateTemp.lcTreatmentTracking.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDiagnostic.Text = Get.Value("frmHisDebateTemp.lciDiagnostic.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentMethod.Text = Get.Value("frmHisDebateTemp.lciTreatmentMethod.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCareMethod.Text = Get.Value("frmHisDebateTemp.lciCareMethod.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConclusion.Text = Get.Value("frmHisDebateTemp.lciConclusion.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDiscussion.Text = Get.Value("frmHisDebateTemp.lciDiscussion.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Get.Value("frmHisDebateTemp.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Get.Value("frmHisDebateTemp.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDebateTempCode.Caption = Get.Value("frmHisDebateTemp.grdColDebateTempCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDebateTempCode.ToolTip = Get.Value("frmHisDebateTemp.grdColDebateTempCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDebateTempName.Caption = Get.Value("frmHisDebateTemp.grdColDebateTempName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDebateTempName.ToolTip = Get.Value("frmHisDebateTemp.grdColDebateTempName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Get.Value("frmHisDebateTemp.grdColCreateTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Get.Value("frmHisDebateTemp.grdColCreateTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCreator.Caption = Get.Value("frmHisDebateTemp.grdCreator.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCreator.ToolTip = Get.Value("frmHisDebateTemp.grdCreator.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Get.Value("frmHisDebateTemp.grdColModifyTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Get.Value("frmHisDebateTemp.grdColModifyTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Get.Value("frmHisDebateTemp.grdColModifier.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Get.Value("frmHisDebateTemp.grdColModifier.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---Validate form
        private void Validate()
        {
            try
            {
                ValidateMaxLangth(txtDebateTempCode, 20);
                ValidateMaxLangth(txtDebateTempName, 200);
                ValidateMaxLangthAllowNull(txtPathologicalHistory, 2000);
                ValidateMaxLangthAllowNull(txtHospitalizationState, 2000);
                ValidateMaxLangthAllowNull(txtBeforeDiagnostic, 2000);
                ValidateMaxLangthAllowNull(txtTreatmentTracking, 2000);
                ValidateMaxLangthAllowNull(txtDiagnostic, 2000);
                ValidateMaxLangthAllowNull(txtTreatmentMethod, 2000);
                ValidateMaxLangthAllowNull(txtCareMethod, 2000);
                ValidateMaxLangthAllowNull(txtConclusion, 2000);
                ValidateMaxLangthAllowNull(txtDiscussion, 2000);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateMaxLangth(DevExpress.XtraEditors.TextEdit txtControl, int MaxLangth)
        {
            try
            {
                ValidateMaxLeng validate = new ValidateMaxLeng();
                validate.MaxLength = MaxLangth;
                validate.txtControl = txtControl;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtControl, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateMaxLangthAllowNull(DevExpress.XtraEditors.TextEdit txtcontrol, int maxlangth)
        {
            try
            {
                ValidateMaxLengAllowNull validate = new ValidateMaxLengAllowNull();
                validate.Maxlangth = maxlangth;
                validate.txtcontrol = txtcontrol;
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtcontrol, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---Even GridView
        private void gridViewDebateTemp_Click(object sender, EventArgs e)
        {
            try
            {
                var dataRow = (HIS_DEBATE_TEMP)gridViewDebateTemp.GetFocusedRow();
                if (dataRow != null)
                {
                    currentdata = dataRow;
                    ChangDataRow(dataRow);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewDebateTemp_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_DEBATE_TEMP DataRow = (HIS_DEBATE_TEMP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (DataRow != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + Startpage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(DataRow.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(DataRow.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IS_PUBLIC_STR")
                        {
                            e.Value = (DataRow.IS_PUBLIC == 1 ? true : false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewDebateTemp_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                var datarow = (HIS_DEBATE_TEMP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (datarow != null)
                {
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (datarow.IS_ACTIVE == 0 ? btnEnableDelete : btnDelete);
                    }
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (datarow.IS_ACTIVE == 0 ? btnUNLOCK : btnLOCK);
                    }
                }
            }

        }
        #endregion

        #region ---Even TextEdit
        private void txtDebateTempCode_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ChkIsPublic1.Focus();
                    ChkIsPublic1.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtDebateTempName_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPathologicalHistory.Focus();
                    txtPathologicalHistory.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtPathologicalHistory_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHospitalizationState.Focus();
                    txtHospitalizationState.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtHospitalizationState_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBeforeDiagnostic.Focus();
                    txtBeforeDiagnostic.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtBeforeDiagnostic_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatmentTracking.Focus();
                    txtTreatmentTracking.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtTreatmentTracking_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiagnostic.Focus();
                    txtDiagnostic.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtDiagnostic_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatmentMethod.Focus();
                    txtTreatmentMethod.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtTreatmentMethod_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCareMethod.Focus();
                    txtCareMethod.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtCareMethod_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtConclusion.Focus();
                    txtConclusion.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtConclusion_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiscussion.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChkIsPublic1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDebateTempName.Focus();
                    txtDebateTempName.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtDiscussion_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                    else
                    {
                        btnCancel.Focus();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
    }
}
