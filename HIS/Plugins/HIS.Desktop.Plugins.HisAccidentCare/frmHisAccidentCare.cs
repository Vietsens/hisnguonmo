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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAccidentCare
{
    public partial class frmHisAccidentCare : HIS.Desktop.Utility.FormBase
    {
        #region contruction
        public frmHisAccidentCare(Inventec.Desktop.Common.Modules.Module moduleData)
		:base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            SetIcon();
            SetCaptionByLanguageKey();
        }
        #endregion

        #region global
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.HIS_ACCIDENT_CARE currentData;
        MOS.EFMODEL.DataModels.HIS_ACCIDENT_CARE resultData;
        int positionHandle = -1;
        int ActionType = -1;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
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
            ucPaging.Init(LoadPaging, param, numPageSize);
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
            HIS_ACCIDENT_CARE updateDTO = new HIS_ACCIDENT_CARE();

            WaitingManager.Show();

            if (this.currentData != null && this.currentData.ID > 0)
            {
                HisAccidentCareFilter filter = new HisAccidentCareFilter();
                filter.ID = currentData.ID;
                updateDTO = new BackendAdapter(param).Get<List<HIS_ACCIDENT_CARE>>
                    (HisRequestUriStore.HIS_ACCIDENT_CARE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }

            updateDTO.ACCIDENT_CARE_CODE = txtCode.Text.Trim();
            updateDTO.ACCIDENT_CARE_NAME = txtName.Text.Trim();

            if (ActionType == GlobalVariables.ActionAdd)
            {
                updateDTO.IS_ACTIVE = IS_ACTIVE_TRUE;
                resultData = new BackendAdapter(param).Post<HIS_ACCIDENT_CARE>
                (HisRequestUriStore.HIS_ACCIDENT_CARE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                    ResetFormData();
                }
            }
            else if (updateDTO != null)
            {
                resultData = new BackendAdapter(param).Post<HIS_ACCIDENT_CARE>
                    (HisRequestUriStore.HIS_ACCIDENT_CARE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                }
            }

            WaitingManager.Hide();
            MessageManager.Show(this, param, success);
            SessionManager.ProcessTokenLost(param);
        }

        private void ChangedDataRow()
        {
            currentData = new HIS_ACCIDENT_CARE();
            currentData = (HIS_ACCIDENT_CARE)gridviewFormList.GetFocusedRow();
            if (currentData != null)
            {
                txtCode.Text = currentData.ACCIDENT_CARE_CODE;
                txtName.Text = currentData.ACCIDENT_CARE_NAME;
                this.ActionType = GlobalVariables.ActionEdit;
                EnableControlChanged(this.ActionType);

                btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                    (dxValidationProvider, dxErrorProvider);
            }
            txtCode.Focus();
        }

        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        }

        private void SetDefaultValue()
        {
            txtCode.Focus();
            txtCode.SelectAll();
            txtSearch.Text = "";
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
                        txtCode.Focus();
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
            ValidationSingleControl(txtCode);
            ValidationSingleControl(txtName);
            WaitingManager.Hide();
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {


                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisAccidentCare.Resources.Lang", typeof(HIS.Desktop.Plugins.HisAccidentCare.frmHisAccidentCare).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditInfo.Text = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.lcEditInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.barSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.barEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.barAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barRefresh.Caption = Inventec.Common.Resource.Get.Value("frmHisAccidentCare.barRefresh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                    (LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
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

            CommonParam paramCommon = new CommonParam();

            MOS.Filter.HisAccidentCareFilter filterSearch = new HisAccidentCareFilter();
            filterSearch.KEY_WORD = txtSearch.Text.Trim();
            filterSearch.ORDER_DIRECTION = "DESC";
            filterSearch.ORDER_FIELD = "MODIFY_TIME";

            Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_ACCIDENT_CARE>> apiResult = null;
            apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_ACCIDENT_CARE>>
                (HisRequestUriStore.HIS_ACCIDENT_CARE_GET, ApiConsumers.MosConsumer, filterSearch, paramCommon);
            if (apiResult != null)
            {
                var data = (List<MOS.EFMODEL.DataModels.HIS_ACCIDENT_CARE>)apiResult.Data;
                gridviewFormList.GridControl.DataSource = data;
                rowCount = (data == null ? 0 : data.Count);
                dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            ControlEditValidationRule validRule = new ControlEditValidationRule();
            validRule.editor = control;
            validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProvider.SetValidationRule(control, validRule);
        }
        #endregion

        #region event
        private void frmHisAccidentCare_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
                SetDefaultValue();
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                ValidateForm();
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
                    HIS_ACCIDENT_CARE data = (HIS_ACCIDENT_CARE)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = data.IS_ACTIVE == IS_ACTIVE_TRUE ? btnLock : btnUnlock;
                    if (e.Column.FieldName == "DELETE")
                        e.RepositoryItem = data.IS_ACTIVE == IS_ACTIVE_TRUE ? btnDelete : btnUndelete;
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
                HIS_ACCIDENT_CARE data = (HIS_ACCIDENT_CARE)gridviewFormList.GetRow(e.ListSourceRowIndex);
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                if (e.Column.FieldName == "STATUS")
                    e.Value = data.IS_ACTIVE == IS_ACTIVE_TRUE ? "đang hoạt động" : "đã khóa";
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
                HIS_ACCIDENT_CARE data = (HIS_ACCIDENT_CARE)gridviewFormList.GetRow(e.RowHandle);
                if (e.Column.FieldName == "STATUS")
                    e.Appearance.ForeColor = data.IS_ACTIVE == IS_ACTIVE_TRUE ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region action
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
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider, dxErrorProvider);
                ResetFormData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_ACCIDENT_CARE resultLock = new HIS_ACCIDENT_CARE();
                bool notHandler = false;

                HIS_ACCIDENT_CARE currentLock = (HIS_ACCIDENT_CARE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    resultLock.ID = currentLock.ID;
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<HIS_ACCIDENT_CARE>
                        (HisRequestUriStore.HIS_ACCIDENT_CARE_CHANGELOCK, ApiConsumers.MosConsumer, resultLock, param);
                    WaitingManager.Hide();
                }

                notHandler = true;
                MessageManager.Show(this.ParentForm, param, notHandler);
                FillDataToGridControl();
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
                HIS_ACCIDENT_CARE resultUnlock = new HIS_ACCIDENT_CARE();
                bool notHandler = false;

                HIS_ACCIDENT_CARE currentUnlock = (HIS_ACCIDENT_CARE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    resultUnlock.ID = currentUnlock.ID;
                    WaitingManager.Show();
                    resultUnlock = new BackendAdapter(param).Post<HIS_ACCIDENT_CARE>(HisRequestUriStore.HIS_ACCIDENT_CARE_CHANGELOCK, ApiConsumers.MosConsumer,
                        resultUnlock, param);
                    WaitingManager.Hide();
                }
                notHandler = true;
                MessageManager.Show(this.ParentForm, param, notHandler);
                FillDataToGridControl();
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
                HIS_ACCIDENT_CARE currentDelete = (HIS_ACCIDENT_CARE)gridviewFormList.GetFocusedRow();
                if (currentData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage
                        (LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                        "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        success = new BackendAdapter(param).Post<bool>
                        (HisRequestUriStore.HIS_ACCIDENT_CARE_DELETE, ApiConsumers.MosConsumer, currentDelete.ID, param);
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

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
                if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    currentData = (HIS_ACCIDENT_CARE)gridviewFormList.GetFocusedRow();
                    ChangedDataRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtName.Focus();
                    txtName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
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
        #endregion

        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar > 127)
            {
                e.Handled = true;
            }
        }
       
    }
}
