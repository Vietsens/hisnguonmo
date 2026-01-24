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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
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


namespace HIS.Desktop.Plugins.HisCustomerSourceDetail.HisCustomerSourceDetail
{
    public partial class frmHisCustomerSourceDetailDetail : HIS.Desktop.Utility.FormBase
    {
        #region ---Decalre---
        Module Currentmodule;
        RefeshReference refeshReference;
        int ActionType = -1;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        HIS_CUSTOMER_SOURCE_DT currentData;
        List<HIS_CUSTOMER_SOURCE> listCustomer;

        #endregion
        public frmHisCustomerSourceDetailDetail(Module module)
            : this(null, null)
        {

        }
        public frmHisCustomerSourceDetailDetail(Module module, RefeshReference reference)
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

        private void frmHisCustomerSourceDetail_Load(object sender, EventArgs e)
        {
            try
            {
                Validate();
                SetDataDefaut();
                EnableControlChange(this.ActionType);
                LoadDataToGridControl();
                SetCapitionByLanguageKey();
                InitComboCustomer();
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
                    txtDoctorName.Focus();
                    txtDoctorName.SelectAll();
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
        #endregion
        #region ---Validate---
        private void Validate()
        {
            try
            {
                ValidateMaxlength(txtDoctorCode, true, 50);
                ValidateMaxlength(txtDoctorName, true, 100);
                ValidateRequired(txtCustomerCode);
                ValidateRequired(cboCustomer);
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
        private void ValidateRequired(DevExpress.XtraEditors.BaseEdit control)
        {
            try
            {
                DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule requiredRule = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
                requiredRule.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
                requiredRule.ErrorText = "Dữ liệu này là bắt buộc";
                requiredRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, requiredRule);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisCustomerSourceDetail.Resources.Lang", typeof(HIS.Desktop.Plugins.HisCustomerSourceDetail.HisCustomerSourceDetail.frmHisCustomerSourceDetailDetail).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnCancel.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.bbtnCancel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.F2.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.F2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcInfor.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.lcInfor.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcCustomerSourceCode.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.lcCustomerSourceCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcCustomerSourceName.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.lcCustomerSourceName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLock.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColLock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLock.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColLock.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelete.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCustomerSourceCode.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColCustomerSourceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCustomerSourceName.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColCustomerSourceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFromTo.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColFromTo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColToTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColToTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsActive.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColIsActive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisCustomerSourceDetail.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                keycaption = Inventec.Common.Resource.Get.Value("HisCustomerSourceDetail." + KeyCaption, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                txtDoctorCode.Text = "";
                txtDoctorName.Text = "";
                txtCustomerCode.Text = "";
                cboCustomer.EditValue = null;
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
                ApiResultObject<List<HIS_CUSTOMER_SOURCE_DT>> apiResuilt = null;
                HisCustomerSourceDtFilter filter = new HisCustomerSourceDtFilter();
                filter.KEY_WORD = txtSearch.Text.Trim();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                GridViewCustomerSource.BeginUpdate();
                GridControlCustomerSource.DataSource = null;
                Inventec.Common.Logging.LogSystem.Info("filter.Data: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                apiResuilt = new BackendAdapter(paramCommon).GetRO<List<HIS_CUSTOMER_SOURCE_DT>>
                    (HisRequestUriStore.CustomerSource_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResuilt != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("apiResuilt.Data: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResuilt.Data), apiResuilt.Data));
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
                HIS_CUSTOMER_SOURCE_DT UpdateDTO = new HIS_CUSTOMER_SOURCE_DT();
                UpDataDTOFromDataForm(ref UpdateDTO);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var Result = new BackendAdapter(param).Post<HIS_CUSTOMER_SOURCE_DT>(HisRequestUriStore.CustomerSource_Create, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (Result != null)
                    {
                        BackendDataWorker.Reset<HIS_CUSTOMER_SOURCE_DT>();
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
                        var Resutl = new BackendAdapter(param).Post<HIS_CUSTOMER_SOURCE_DT>(HisRequestUriStore.CustomerSource_UPDATE, ApiConsumers.MosConsumer, UpdateDTO, param);
                        if (Resutl != null)
                        {
                            BackendDataWorker.Reset<HIS_CUSTOMER_SOURCE_DT>();
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
        private void UpDataDTOFromDataForm(ref HIS_CUSTOMER_SOURCE_DT data)
        {
            try
            {
                data.LOGINNAME  = txtDoctorCode.Text;
                data.USERNAME = txtDoctorName.Text;
                if (cboCustomer.EditValue != null)
                {
                    var customer = listCustomer.FirstOrDefault(o => o.CUSTOMER_SOURCE_CODE == cboCustomer.EditValue.ToString());

                    if (customer != null)
                    {
                        data.CUSTOMER_SOURCE_ID = customer.ID;
                    }
                    else
                    {
                        data.CUSTOMER_SOURCE_ID = null;
                    }
                }
                else
                {
                    data.CUSTOMER_SOURCE_ID = null;
                }
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
                            txtDoctorCode.Focus();
                            txtDoctorCode.SelectAll();
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

        private void ChangedataRow(HIS_CUSTOMER_SOURCE_DT data)
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
        private void FillDatatoControl(HIS_CUSTOMER_SOURCE_DT data)
        {
            try
            {
                if (data != null)
                {
                    txtDoctorCode.Text = data.LOGINNAME;
                    txtDoctorName.Text = data.USERNAME;

                    if (data.CUSTOMER_SOURCE_ID != null)
                    {
                        var customer = listCustomer.FirstOrDefault(o => o.ID == data.CUSTOMER_SOURCE_ID);

                        if (customer != null)
                        {
                            cboCustomer.EditValue = customer.CUSTOMER_SOURCE_CODE;
                            txtCustomerCode.Text = customer.CUSTOMER_SOURCE_CODE;
                        }
                        else
                        {
                            cboCustomer.EditValue = null;
                            txtCustomerCode.Text = "";
                        }
                    }
                    else
                    {
                        cboCustomer.EditValue = null;
                        txtCustomerCode.Text = "";
                    }
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
                txtDoctorCode.Focus();
                txtDoctorCode.SelectAll();
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
                    HIS_CUSTOMER_SOURCE_DT datarow = (HIS_CUSTOMER_SOURCE_DT)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "CUSTOMER_SOURCE_NAME")
                        {
                            // Kiểm tra listCustomer và kết quả tìm kiếm có null không
                            if (listCustomer != null)
                            {
                                var cus = listCustomer.FirstOrDefault(o => o.ID == datarow.CUSTOMER_SOURCE_ID);
                                e.Value = (cus != null) ? cus.CUSTOMER_SOURCE_NAME : "Không xác định";
                            }
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
                    HIS_CUSTOMER_SOURCE_DT datarow = view.GetRow(e.RowHandle) as HIS_CUSTOMER_SOURCE_DT;
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
                    HIS_CUSTOMER_SOURCE_DT dataRow = (HIS_CUSTOMER_SOURCE_DT)GridViewCustomerSource.GetRow(e.RowHandle);
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
                HIS_CUSTOMER_SOURCE_DT datarow = (HIS_CUSTOMER_SOURCE_DT)GridViewCustomerSource.GetFocusedRow();
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
                HIS_CUSTOMER_SOURCE_DT datarow = (HIS_CUSTOMER_SOURCE_DT)GridViewCustomerSource.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var Result = new BackendAdapter(param).Post<HIS_CUSTOMER_SOURCE_DT>(HisRequestUriStore.CustomerSource_CHANGELOCK, ApiConsumers.MosConsumer, datarow.ID, param);
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
                HIS_CUSTOMER_SOURCE_DT datarow = (HIS_CUSTOMER_SOURCE_DT)GridViewCustomerSource.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var Result = new BackendAdapter(param).Post<HIS_CUSTOMER_SOURCE_DT>(HisRequestUriStore.CustomerSource_CHANGELOCK, ApiConsumers.MosConsumer, datarow.ID, param);
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

                HIS_CUSTOMER_SOURCE_DT datarow = (HIS_CUSTOMER_SOURCE_DT)GridViewCustomerSource.GetFocusedRow();
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

        private void cboCustomer_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCustomer.EditValue != null)
                    {
                        var data = listCustomer.SingleOrDefault(o => o.CUSTOMER_SOURCE_CODE == cboCustomer.EditValue.ToString());
                        if (data != null)
                        {
                            txtCustomerCode.Text = data.CUSTOMER_SOURCE_CODE;
                            txtCustomerCode.Focus();
                            txtCustomerCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCustomer_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCustomer.Properties.Buttons[1].Visible = true;
                    cboCustomer.EditValue = null;
                    txtCustomerCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCustomerCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtCustomerCode.Text))
                    {
                        cboCustomer.EditValue = null;
                        cboCustomer.Focus();
                        cboCustomer.ShowPopup();
                    }
                    else
                    {
                        var searchItem = listCustomer.FirstOrDefault(o => o.CUSTOMER_SOURCE_CODE.ToUpper() == txtCustomerCode.Text.Trim().ToUpper());

                        if (searchItem != null)
                        {
                            txtCustomerCode.Text = searchItem.CUSTOMER_SOURCE_CODE;
                            cboCustomer.EditValue = searchItem.CUSTOMER_SOURCE_CODE;
                        }
                        else
                        {
                            cboCustomer.Focus();
                            cboCustomer.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboCustomer()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_CUSTOMER_SOURCE>().Where(o => o.IS_ACTIVE == 1).ToList();
                listCustomer = new List<HIS_CUSTOMER_SOURCE>();
                listCustomer = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CUSTOMER_SOURCE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("CUSTOMER_SOURCE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CUSTOMER_SOURCE_NAME", "CUSTOMER_SOURCE_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCustomer, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
