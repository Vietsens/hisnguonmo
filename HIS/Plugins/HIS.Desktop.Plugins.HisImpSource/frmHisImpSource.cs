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
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.HisImpSource
{
    public partial class frmHisImpSource : HIS.Desktop.Utility.FormBase
    {
        #region contructor
        public frmHisImpSource(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;
                SetIcon();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region global
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.HIS_IMP_SOURCE currentData;
        MOS.EFMODEL.DataModels.HIS_IMP_SOURCE resultData;
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

        #region action
        private void frmHisImpSource_Load(object sender, EventArgs e)
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
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
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
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
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

        #region event
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
                    HIS_IMP_SOURCE data = (HIS_IMP_SOURCE)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = data.IS_ACTIVE == IS_ACTIVE_TRUE ? btnUnlock : btnLock;
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
            HIS_IMP_SOURCE data = (HIS_IMP_SOURCE)gridviewFormList.GetRow(e.ListSourceRowIndex);
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //HIS_IMP_SOURCE data = (HIS_IMP_SOURCE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
            if (e.Column.FieldName == "STT")
                e.Value = e.ListSourceRowIndex + 1 + startPage;
            if (e.Column.FieldName == "STATUS")
                e.Value = data.IS_ACTIVE == IS_ACTIVE_TRUE ? "Hoạt động" : "Tạm khóa";
            if (e.Column.FieldName == "IS_DEFAULT_STR")
            {
                e.Value = data.IS_DEFAULT == IS_ACTIVE_TRUE ? true : false;
            }
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

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                HIS_IMP_SOURCE data = (HIS_IMP_SOURCE)gridviewFormList.GetRow(e.RowHandle);
                if (e.Column.FieldName == "STATUS")
                    e.Appearance.ForeColor = data.IS_ACTIVE == IS_ACTIVE_TRUE ? Color.Green : Color.Red;
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
                HIS_IMP_SOURCE resultLock = new HIS_IMP_SOURCE();
                bool notHandler = false;

                HIS_IMP_SOURCE currentLock = (HIS_IMP_SOURCE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<HIS_IMP_SOURCE>
                        (HisRequestUriStore.HIS_IMP_SOURCE_CHANGELOCK, ApiConsumers.MosConsumer, currentLock, param);
                    if (resultLock!=null)
                    {
                        
                         notHandler = true;
                         BackendDataWorker.Reset<HIS_IMP_SOURCE>();
                         FillDataToGridControl();
                         btnRefresh_Click(null, null);
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
                HIS_IMP_SOURCE resultUnlock = new HIS_IMP_SOURCE();
                bool notHandler = false;

                HIS_IMP_SOURCE currentUnlock = (HIS_IMP_SOURCE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    resultUnlock = new BackendAdapter(param).Post<HIS_IMP_SOURCE>(HisRequestUriStore.HIS_IMP_SOURCE_CHANGELOCK, ApiConsumers.MosConsumer,
                        currentUnlock, param);
                    if (resultUnlock != null)
                    {
                       
                        notHandler = true;
                        BackendDataWorker.Reset<HIS_IMP_SOURCE>();
                        FillDataToGridControl();
                        btnRefresh_Click(null, null);
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
                HIS_IMP_SOURCE currentDelete = (HIS_IMP_SOURCE)gridviewFormList.GetFocusedRow();
                if (currentDelete != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage
                        (LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                        "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        success = new BackendAdapter(param).Post<bool>
                        (HisRequestUriStore.HIS_IMP_SOURCE_DELETE, ApiConsumers.MosConsumer, currentDelete.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_IMP_SOURCE>();
                            FillDataToGridControl();
                        }
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
                    currentData = (HIS_IMP_SOURCE)gridviewFormList.GetFocusedRow();
                    ChangedDataRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsDefault.Focus();
                    chkIsDefault.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsDefault_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd)
                    btnAdd.Focus();
                if (this.ActionType == GlobalVariables.ActionEdit)
                    btnEdit.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar > 127)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region private first
        private void FillDataToGridControl()
        {
            try
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

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
            HIS_IMP_SOURCE updateDTO = new HIS_IMP_SOURCE();

            WaitingManager.Show();

            if (this.currentData != null && this.currentData.ID > 0)
            {
                CommonParam filterParam = new CommonParam();
                HisImpSourceFilter filter = new HisImpSourceFilter();
                filter.ID = currentData.ID;
                updateDTO = new BackendAdapter(filterParam).Get<List<HIS_IMP_SOURCE>>
                    (HisRequestUriStore.HIS_IMP_SOURCE_GET, ApiConsumers.MosConsumer, filter, filterParam).FirstOrDefault();
            }

            updateDTO.IMP_SOURCE_CODE = txtCode.Text.Trim();
            updateDTO.IMP_SOURCE_NAME = txtName.Text.Trim();
            if (chkIsDefault.Checked)
                updateDTO.IS_DEFAULT = 1;
            else
                updateDTO.IS_DEFAULT = null;
            if (ActionType == GlobalVariables.ActionAdd)
            {
                updateDTO.IS_ACTIVE = IS_ACTIVE_TRUE;
                resultData = new BackendAdapter(param).Post<HIS_IMP_SOURCE>
                    (HisRequestUriStore.HIS_IMP_SOURCE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                    ResetFormData();
                }
            }
            else if (updateDTO != null)
            {
                resultData = new BackendAdapter(param).Post<HIS_IMP_SOURCE>
                    (HisRequestUriStore.HIS_IMP_SOURCE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                }
            }
            if (success)
            {
                BackendDataWorker.Reset<HIS_IMP_SOURCE>();
            }
            WaitingManager.Hide();
            MessageManager.Show(this, param, success);
            txtCode.Focus();
            txtCode.SelectAll();
            SessionManager.ProcessTokenLost(param);
        }

        private void ChangedDataRow()
        {
            currentData = new HIS_IMP_SOURCE();
            currentData = (HIS_IMP_SOURCE)gridviewFormList.GetFocusedRow();
            if (currentData != null)
            {
                txtCode.Text = currentData.IMP_SOURCE_CODE;
                txtName.Text = currentData.IMP_SOURCE_NAME;
                if (currentData.IS_DEFAULT == 1)
                    chkIsDefault.Checked = true;
                else
                    chkIsDefault.Checked = false;
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisImpSource.Resources.Lang", typeof(HIS.Desktop.Plugins.HisImpSource.frmHisImpSource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditInfo.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.lcEditInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcIsDefault.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.lcIsDefault.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdIsDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.grdIsDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdIsDefault.ToolTip = Inventec.Common.Resource.Get.Value("frmHisImpSource.grdIsDefault.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.barSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.barEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.barAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barRefresh.Caption = Inventec.Common.Resource.Get.Value("frmHisImpSource.barRefresh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisImpSource.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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

            CommonParam paramCommon = new CommonParam(startPage, limit);

            MOS.Filter.HisImpSourceFilter filterSearch = new HisImpSourceFilter();
            filterSearch.KEY_WORD = txtSearch.Text.Trim();
            filterSearch.ORDER_DIRECTION = "DESC";
            filterSearch.ORDER_FIELD = "MODIFY_TIME";

            Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_IMP_SOURCE>> apiResult = null;
            apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_IMP_SOURCE>>
                (HisRequestUriStore.HIS_IMP_SOURCE_GET, ApiConsumers.MosConsumer, filterSearch, paramCommon);
            if (apiResult != null)
            {
                var data = (List<MOS.EFMODEL.DataModels.HIS_IMP_SOURCE>)apiResult.Data;
                if (data != null)
                {
                    gridviewFormList.GridControl.DataSource = data;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
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


    }
}
