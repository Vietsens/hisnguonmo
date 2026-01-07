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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.WebApiClient;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
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


namespace HIS.Desktop.Plugins.HisCustomerSource.HisCustomerSource
{
    public partial class frmHisCustomerSource : HIS.Desktop.Utility.FormBase
    {
        #region ---Decalre---
        Module Currentmodule;
        RefeshReference refeshReference;
        int ActionType = -1;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        HIS_CUSTOMER_SOURCE currentData;
        #endregion
        public frmHisCustomerSource(Module module)
            : this(null, null)
        {

        }
        public frmHisCustomerSource(Module module, RefeshReference reference)
            : base(module)
        {
            InitializeComponent();
            this.refeshReference = reference;
            this.Currentmodule = module;
            try
            {
                if (this.Currentmodule != null && !String.IsNullOrEmpty(this.Currentmodule.text))
                {
                    this.Text = this.Currentmodule.text;
                }
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.AddBarManager(this.barManager1);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmHisCustomerSource_Load(object sender, EventArgs e)
        {
            try
            {
                Validate();
                SetDataDefaut();
                EnableControlChange(this.ActionType);
                LoadDataToGridControl();
                SetCapitionByLanguageKey();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #region ---PreviewKeyDown---
        private void txtCustomerSourceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCustomerSourceName.Focus();
                    txtCustomerSourceName.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtCustomerSourceName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtFromTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
  
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtToTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                        btnAdd.Focus();
                    else if (btnEdit.Enabled)
                        btnEdit.Focus();
                    else
                        btnCancel.Focus();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---Validate---
        private void Validate()
        {
            try
            {
                ValidateMaxlength(txtCustomerSourceCode, true, 10);
                ValidateMaxlength(txtCustomerSourceName, true, 500);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void ValidateMaxlength(DevExpress.XtraEditors.BaseEdit control, bool IsRequired, int maxlength)
        {
            try
            {
                ControlMaxLengthValidationRule valie = new ControlMaxLengthValidationRule();
                valie.editor = control;
                valie.maxLength = maxlength;
                valie.IsRequired = IsRequired;
                valie.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                valie.ErrorText = "Nhập quá ký tự cho phép (" + maxlength + ")";
                dxValidationProvider1.SetValidationRule(control, valie);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---SetData---
        private void SetCapitionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisCustomerSource.Resources.Lang", typeof(HIS.Desktop.Plugins.HisCustomerSource.HisCustomerSource.frmHisCustomerSource).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnCancel.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.bbtnCancel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.F2.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.F2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcInfor.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.lcInfor.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcCustomerSourceCode.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.lcCustomerSourceCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcCustomerSourceName.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.lcCustomerSourceName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLock.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColLock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLock.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColLock.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelete.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCustomerSourceCode.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColCustomerSourceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCustomerSourceName.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColCustomerSourceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFromTo.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColFromTo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColToTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColToTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsActive.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColIsActive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSource.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private String Setlanguage(string KeyCaption)
        {
            string keycaption = "";
            try
            {
                keycaption = Inventec.Common.Resource.Get.Value("HisCustomerSource." + KeyCaption, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                keycaption = "";
                LogSystem.Warn(ex);
            }
            return keycaption;
        }
        private void SetDataDefaut()
        {
            try
            {
                txtCustomerSourceCode.Text = "";
                txtCustomerSourceName.Text = "";
                this.ActionType = GlobalVariables.ActionAdd;

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void EnableControlChange(int action)
        {
            try
            {
                this.btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
                this.btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        //Load data to gridcontrol
        private void LoadDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPaging, param, numPageSize, this.GridControlCustomerSource);
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
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<HIS_CUSTOMER_SOURCE>> apiResuilt = null;
                HisCustomerSourceFilter filter = new HisCustomerSourceFilter();
                filter.KEY_WORD = txtSearch.Text.Trim();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                GridViewCustomerSource.BeginUpdate();
                GridControlCustomerSource.DataSource = null;
                apiResuilt = new BackendAdapter(paramCommon).GetRO<List<HIS_CUSTOMER_SOURCE>>(HisRequestUriStore.CustomerSource_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResuilt != null)
                {
                    var data = apiResuilt.Data;
                    if (data != null && data.Count > 0)
                    {
                        GridControlCustomerSource.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResuilt.Param == null ? 0 : apiResuilt.Param.Count ?? 0);
                    }
                }
                GridViewCustomerSource.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        //
        // Update data
        private void ProcessorSave()
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                HIS_CUSTOMER_SOURCE UpdateDTO = new HIS_CUSTOMER_SOURCE();
                UpDataDTOFromDataForm(ref UpdateDTO);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var Result = new BackendAdapter(param).Post<HIS_CUSTOMER_SOURCE>(HisRequestUriStore.CustomerSource_Create, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (Result != null)
                    {
                        BackendDataWorker.Reset<HIS_CUSTOMER_SOURCE>();
                        success = true;
                        LoadDataToGridControl();
                        btnCancel_Click(null, null);
                    }
                }
                else
                {
                    if (this.currentData != null)
                    {
                        UpdateDTO.ID = this.currentData.ID;
                        var Resutl = new BackendAdapter(param).Post<HIS_CUSTOMER_SOURCE>(HisRequestUriStore.CustomerSource_UPDATE, ApiConsumers.MosConsumer, UpdateDTO, param);
                        if (Resutl != null)
                        {
                            BackendDataWorker.Reset<HIS_CUSTOMER_SOURCE>();
                            success = true;
                            LoadDataToGridControl();

                        }
                    }
                }
                WaitingManager.Hide();
                #region ---Thong bao---
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        private void UpDataDTOFromDataForm(ref HIS_CUSTOMER_SOURCE data)
        {
            try
            {
                data.CUSTOMER_SOURCE_CODE = txtCustomerSourceCode.Text;
                data.CUSTOMER_SOURCE_NAME = txtCustomerSourceName.Text;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        //
        //Set data defaut to control 
        private void RestFormData()
        {
            try
            {
                if (!lcInfor.IsInitialized)
                    return;
                lcInfor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcInfor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtCustomerSourceCode.Focus();
                            txtCustomerSourceCode.SelectAll();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcInfor.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedataRow(HIS_CUSTOMER_SOURCE data)
        {
            try
            {
                if (data != null)
                {
                    FillDatatoControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChange(this.ActionType);
                    this.btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void FillDatatoControl(HIS_CUSTOMER_SOURCE data)
        {
            try
            {
                if (data != null)
                {
                    txtCustomerSourceCode.Text = data.CUSTOMER_SOURCE_CODE;
                    txtCustomerSourceName.Text = data.CUSTOMER_SOURCE_NAME;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---Even Button---
        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnEdit.Enabled && this.ActionType == GlobalVariables.ActionEdit)
                    btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnAdd.Enabled && this.ActionType == GlobalVariables.ActionAdd)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void F2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtCustomerSourceCode.Focus();
                txtCustomerSourceCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProcessorSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProcessorSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChange(this.ActionType);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                RestFormData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region ---Even GridControl---
        private void GridViewCustomerSource_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_CUSTOMER_SOURCE datarow = (HIS_CUSTOMER_SOURCE)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (datarow != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + this.startPage;
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = (datarow.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa");
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(datarow.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(datarow.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "LOCK")
                        {
                            e.Value = datarow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? "Mở" : "Khóa";
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void GridViewCustomerSource_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_CUSTOMER_SOURCE datarow = view.GetRow(e.RowHandle) as HIS_CUSTOMER_SOURCE;
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (datarow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnLock : btnUnLock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (datarow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDelete : btnVisibleDetele);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void GridViewCustomerSource_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views
                    .Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_CUSTOMER_SOURCE dataRow = (HIS_CUSTOMER_SOURCE)GridViewCustomerSource.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        e.Appearance.ForeColor = (dataRow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Color.Green : Color.Red);
                    }
                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void GridViewCustomerSource_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_CUSTOMER_SOURCE datarow = (HIS_CUSTOMER_SOURCE)GridViewCustomerSource.GetFocusedRow();
                if (datarow != null)
                {
                    this.currentData = datarow;
                    ChangedataRow(datarow);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        #endregion
        #region ---btn Lock and Delete
        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_CUSTOMER_SOURCE datarow = (HIS_CUSTOMER_SOURCE)GridViewCustomerSource.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var Result = new BackendAdapter(param).Post<HIS_CUSTOMER_SOURCE>(HisRequestUriStore.CustomerSource_CHANGELOCK, ApiConsumers.MosConsumer, datarow.ID, param);
                        if (Result != null)
                        {
                            LoadDataToGridControl();
                            success = true;
                            btnCancel_Click(null, null);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_CUSTOMER_SOURCE datarow = (HIS_CUSTOMER_SOURCE)GridViewCustomerSource.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var Result = new BackendAdapter(param).Post<HIS_CUSTOMER_SOURCE>(HisRequestUriStore.CustomerSource_CHANGELOCK, ApiConsumers.MosConsumer, datarow.ID, param);
                        if (Result != null)
                        {
                            LoadDataToGridControl();
                            success = true;
                            btnCancel_Click(null, null);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();

                HIS_CUSTOMER_SOURCE datarow = (HIS_CUSTOMER_SOURCE)GridViewCustomerSource.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        bool success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.CustomerSource_DELETE, ApiConsumers.MosConsumer, datarow.ID, param);
                        if (success)
                        {
                            LoadDataToGridControl();
                            btnCancel_Click(null, null);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    LoadDataToGridControl();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
    }
}
