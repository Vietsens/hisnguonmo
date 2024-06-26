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
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.Filter;
using SAR.Desktop.Plugins.SarFormType;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using SAR.EFMODEL.DataModels;
using DevExpress.XtraEditors.ViewInfo;
using SAR.Desktop.Plugins.SarFormType.Validtion;

namespace SAR.Desktop.Plugins.SarFormType
{
    public partial class frmSarFormType : HIS.Desktop.Utility.FormBase
    {
        public frmSarFormType(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            SetIcon();
            SetCaptionByLanguageKey();
        }

        #region global
        Inventec.Desktop.Common.Modules.Module moduleData;
        SAR.EFMODEL.DataModels.SAR_FORM_TYPE currentData;
        SAR.EFMODEL.DataModels.SAR_FORM_TYPE resultData;
        List<SAR_FORM_TYPE> listEmployee;
        int positionHandle = -1;
        int ActionType = -1;
        int rowCount;
        int dataTotal;
        DelegateSelectData delegateSelect = null;
        internal long id;
        int startPage;
        int limit;
        #endregion

        #region private first
        private void FillDataToGridControl()
        {
            WaitingManager.Show();

            int numPageSize = 0;
            if (ucPaging.pagingGrid != null)
            {
                numPageSize = ucPaging.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
            }

            LoadPaging(new CommonParam(0, numPageSize));

            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            //ucPaging.Init(loadPaging, param);
            ucPaging.Init(LoadPaging, param, numPageSize, gridControl1);

            WaitingManager.Hide();
        }

        private void SaveProcess()
        {
            if (!btnEdit.Enabled && !btnAdd.Enabled)
                return;

            positionHandle = -1;
            if (!dxValidationProvider.Validate())
                return;

            CommonParam param = new CommonParam();
            bool success = false;
            SAR.EFMODEL.DataModels.SAR_FORM_TYPE updateDTO = new SAR.EFMODEL.DataModels.SAR_FORM_TYPE();

            WaitingManager.Show();

            if (this.currentData != null && this.currentData.ID > 0)
            {
                updateDTO = currentData;
            }

            updateDTO.FORM_TYPE_CODE = txtFormTypeCode.Text.Trim();
            updateDTO.FORM_TYPE_NAME = txtFormTypeName.Text.Trim();

            if (ActionType == GlobalVariables.ActionAdd)
            {
                updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_FORM_TYPE>
                  (SAR.Desktop.Plugins.SarFormType.SarRequestUriStore.SAR_FORMTYPE_CREATE, ApiConsumers.SarConsumer,
                  updateDTO, param);
            }
            else if (updateDTO != null)
            {
                resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_FORM_TYPE>
                  (SAR.Desktop.Plugins.SarFormType.SarRequestUriStore.SAR_FORMTYPE_UPDATE, ApiConsumers.SarConsumer,
                  updateDTO, param);
                //if (resultData != null)
                //{
                //  success = true;
                //  FillDataToGridControl();
                //}
            }
            if (resultData != null)
            {
                success = true;
                FillDataToGridControl();
                ResetFormData();
                LoadEmployee();
            }
            else
            {

            }

            WaitingManager.Hide();
            MessageManager.Show(this, param, success);
            SessionManager.ProcessTokenLost(param);
        }

        private void ChangedDataRow()
        {
            currentData = new SAR.EFMODEL.DataModels.SAR_FORM_TYPE();
            currentData = (SAR.EFMODEL.DataModels.SAR_FORM_TYPE)gridviewFormList.GetFocusedRow();

            if (currentData != null)
            {
                txtFormTypeName.Text = currentData.FORM_TYPE_NAME;
                txtFormTypeCode.Text = currentData.FORM_TYPE_CODE;
                this.ActionType = GlobalVariables.ActionEdit;
                EnableControlChanged(this.ActionType);

                btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                  (dxValidationProvider, dxErrorProvider);
            }
        }

        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        }

        private void SetDefaultValue()
        {
            txtSearch.Text = "";
            LoadEmployee();
        }

        private void LoadEmployee()
        {
            try
            {
                
                CommonParam param = new CommonParam();
                SAR.Filter.SarFormTypeFilter filter = new SAR.Filter.SarFormTypeFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtSearch.Text;

                this.listEmployee = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<SAR_FORM_TYPE>>
                  (SAR.Desktop.Plugins.SarFormType.SarRequestUriStore.SAR_FORMTYPE_GET, ApiConsumers.SarConsumer, filter, param);
                gridControl1.BeginUpdate();
                gridControl1.DataSource = this.listEmployee;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditInfo.IsInitialized) return;
                lcEditInfo.BeginUpdate();

                foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditInfo.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                    {
                        DevExpress.XtraEditors.BaseEdit formatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        formatFrm.ResetText();
                        formatFrm.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                lcEditInfo.EndUpdate();
            }
        }

        private void ValidateForm()
        {
            ValidMaxlength(txtFormTypeCode, 200);
            ValidMaxlength(txtFormTypeName, 100);
            WaitingManager.Hide();
        }

        private void InitComboLoginName()
        {
            CommonParam param = new CommonParam();
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("SAR.Desktop.Plugins.SarFormType.Resources.Lang", typeof(SAR.Desktop.Plugins.SarFormType.frmSarFormType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSarFormType.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditInfo.Text = Inventec.Common.Resource.Get.Value("frmSarFormType.lcEditInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmSarFormType.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barSearch.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.barSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barEdit.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.barEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barAdd.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.barAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barRefresh.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.barRefresh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmSarFormType.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmSarFormType.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmSarFormType.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFormTypeName.Text = Inventec.Common.Resource.Get.Value("frmSarFormType.lciFormTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFormTypeCode.Text = Inventec.Common.Resource.Get.Value("frmSarFormType.lciFormTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmSarFormType.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDelete.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.gridColumnDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLock.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.gridColumnLock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnFormTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.gridColumnFormTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnFormTypeName.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.gridColumnFormTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.gridColumnCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifierTime.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.gridColumnModifierTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreator.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.gridColumnCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifier.Caption = Inventec.Common.Resource.Get.Value("frmSarFormType.gridColumnModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmSarFormType.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon
                  (System.IO.Path.Combine
                  (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region private second
        private void LoadPaging(object param)
        {
            startPage = ((CommonParam)param).Start ?? 0;
            limit = ((CommonParam)param).Limit ?? 0;

            CommonParam paramCommon = new CommonParam(startPage, limit);

            SAR.Filter.SarFormTypeFilter filterSearch = new SAR.Filter.SarFormTypeFilter();
            filterSearch.KEY_WORD = txtSearch.Text.Trim();
            filterSearch.ORDER_FIELD = "MODIFY_TIME";
            filterSearch.ORDER_DIRECTION = "DESC";

            Inventec.Core.ApiResultObject<List<SAR.EFMODEL.DataModels.SAR_FORM_TYPE>> apiResult = null;
            apiResult = new BackendAdapter(paramCommon).GetRO<List<SAR.EFMODEL.DataModels.SAR_FORM_TYPE>>
              (SAR.Desktop.Plugins.SarFormType.SarRequestUriStore.SAR_FORMTYPE_GET, ApiConsumers.SarConsumer, filterSearch, paramCommon);
            if (apiResult != null)
            {
                var data = (List<SAR.EFMODEL.DataModels.SAR_FORM_TYPE>)apiResult.Data;
                gridviewFormList.GridControl.DataSource = data;
                rowCount = (data == null ? 0 : data.Count);
                dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            ValidateNotNull validRule = new ValidateNotNull();
            validRule.textEdit = control;
            validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProvider.SetValidationRule(control, validRule);
        }
        #endregion

        #region event
        private void frmEmployee_Load(object sender, EventArgs e)
        {
            FillDataToGridControl();
            InitComboLoginName();
            SetDefaultValue();
            this.ActionType = GlobalVariables.ActionAdd;
            EnableControlChanged(this.ActionType);
            ValidateForm();
            //txtFormTypeName.Properties.Mask.EditMask = ".{20}";
        }

        void ValidMaxlength(DevExpress.XtraEditors.TextEdit control, int maxLength)
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = control;
                validateMaxLength.maxLength = maxLength;
                dxValidationProvider.SetValidationRule(control, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                ChangedDataRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    SAR.EFMODEL.DataModels.SAR_FORM_TYPE data = (SAR.EFMODEL.DataModels.SAR_FORM_TYPE)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnUnlock : btnLock;
                    if (e.Column.FieldName == "DELETE")
                        e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDelete : btnUndelete;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {

                SAR.EFMODEL.DataModels.SAR_FORM_TYPE data = (SAR.EFMODEL.DataModels.SAR_FORM_TYPE)gridviewFormList.GetRow(e.ListSourceRowIndex);
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                if (e.Column.FieldName == "STATUS")
                    e.Value = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? "đang hoạt động" : "đã khóa";

                if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(createTime));
                }
                if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    string mobdifyTime = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(mobdifyTime));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                SAR.EFMODEL.DataModels.SAR_FORM_TYPE data = (SAR.EFMODEL.DataModels.SAR_FORM_TYPE)gridviewFormList.GetRow(e.RowHandle);
                if (e.Column.FieldName == "STATUS")
                    e.Appearance.ForeColor = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                SAR.EFMODEL.DataModels.SAR_FORM_TYPE resultLock = new SAR.EFMODEL.DataModels.SAR_FORM_TYPE();
                bool notHandler = false;

                SAR.EFMODEL.DataModels.SAR_FORM_TYPE currentLock = (SAR.EFMODEL.DataModels.SAR_FORM_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //resultLock.ID = currentLock.ID;
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_FORM_TYPE>
                      (SAR.Desktop.Plugins.SarFormType.SarRequestUriStore.SAR_FORMTYPE_CHANGELOCK, ApiConsumers.SarConsumer, currentLock, param);

                    if (resultLock != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                SAR.EFMODEL.DataModels.SAR_FORM_TYPE resultUnlock = new SAR.EFMODEL.DataModels.SAR_FORM_TYPE();
                bool notHandler = false;

                SAR.EFMODEL.DataModels.SAR_FORM_TYPE currentUnlock = (SAR.EFMODEL.DataModels.SAR_FORM_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //resultUnlock.ID = currentUnlock.ID;
                    WaitingManager.Show();
                    resultUnlock = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_FORM_TYPE>(SAR.Desktop.Plugins.SarFormType.SarRequestUriStore.SAR_FORMTYPE_CHANGELOCK, ApiConsumers.SarConsumer,
                      currentUnlock, param);

                    if (resultUnlock != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                SAR.EFMODEL.DataModels.SAR_FORM_TYPE currentDelete = (SAR.EFMODEL.DataModels.SAR_FORM_TYPE)gridviewFormList.GetFocusedRow();
                if (currentDelete != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage
                      (HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        success = new BackendAdapter(param).Post<bool>
                        (SAR.Desktop.Plugins.SarFormType.SarRequestUriStore.SAR_FORMTYPE_DELETE, ApiConsumers.SarConsumer, currentDelete.ID, param);
                        if (success)
                            FillDataToGridControl();
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                this.currentData = new SAR_FORM_TYPE();
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider, dxErrorProvider);
                ResetFormData();
                LoadEmployee();
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFormTypeName.Focus();
                    txtFormTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDipLoma_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkAdmin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkDoctor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    if (this.ActionType == GlobalVariables.ActionEdit)
                        btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbLoginName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtFormTypeName.Focus();
                txtFormTypeName.SelectAll();
            }
            else
            {
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> obj = new List<object>();
                CallModule callModule = new CallModule(CallModule.HisImportEmployee, 0, 0, obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFormTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFormTypeName.Focus();
                    txtFormTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFormTypeName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
