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
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.DepartmentExpeMaty.DepartmentExpeMaty
{
    public partial class frmDepartmentExpeMaty : HIS.Desktop.Utility.FormBase
    {

        Module Currentmodule;
        RefeshReference refeshReference;
        int ActionType = -1;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        V_HIS_DEPARTMENT_EXPE_MATY currentData;

        public frmDepartmentExpeMaty(Module module)
            : this(null, null)
        {

        }
        public frmDepartmentExpeMaty(Module module, RefeshReference reference)
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

        private void frmDepartmentExpeMaty_Load(object sender, EventArgs e)
        {
            try
            {
                InitCombo(cboMaty);
                InitCombo(cboDepartment);
                InitCombo(cboStock);
                //
                InitSpin(spinMaxExpend);
                //
                Validate();
                EnableControlChange(this.ActionType);
                LoadDataToGridControl();
                SetCapitionByLanguageKey();
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void Validate()
        {
            try
            {
                ValidateMaxlength(txtMaty, true, 0);
                ValidateMaxlength(cboMaty, true, 0);
                ValidateMaxlength(txtDepartment, true, 0);
                ValidateMaxlength(cboDepartment, true, 0);
                ValidateMaxlength(spinMaxExpend, true, 0);
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
                //valie.maxLength = maxlength;
                valie.IsRequired = IsRequired;
                valie.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //valie.ErrorText = "Nhập quá ký tự cho phép (" + maxlength + ")";
                dxValidationProvider1.SetValidationRule(control, valie);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetCapitionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DepartmentExpeMaty.Resources.Lang", typeof(HIS.Desktop.Plugins.DepartmentExpeMaty.DepartmentExpeMaty.frmDepartmentExpeMaty).Assembly);
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnCancel.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.bbtnCancel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.F2.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.F2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcInfor.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.lcInfor.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStock.Properties.NullText = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.cboStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMaty.Properties.NullText = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.cboMaty.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcCustomerSourceName.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.lcCustomerSourceName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcCustomerSourceCode.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.lcCustomerSourceCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.layoutControlItem15.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLock.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColLock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLock.ToolTip = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColLock.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelete.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCustomerSourceCode.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColCustomerSourceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCustomerSourceName.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColCustomerSourceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFromTo.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColFromTo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColToTime.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColToTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsActive.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColIsActive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmDepartmentExpeMaty.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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
                ucPaging2.Init(LoadPaging, param, numPageSize, this.grdHisDepartmentExpeMaty);
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
                ApiResultObject<List<V_HIS_DEPARTMENT_EXPE_MATY>> apiResuilt = null;
                HisDepartmentExpeMatyFilter filter = new HisDepartmentExpeMatyFilter();
                filter.KEY_WORD = txtSearch.Text.Trim();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                grvViewHisDepartmentExpeMaty.BeginUpdate();
                grdHisDepartmentExpeMaty.DataSource = null;
                apiResuilt = new BackendAdapter(paramCommon).GetRO<List<V_HIS_DEPARTMENT_EXPE_MATY>>(HisRequestUriStore.CustomerSource_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResuilt != null)
                {
                    var data = apiResuilt.Data;
                    if (data != null && data.Count > 0)
                    {
                        grdHisDepartmentExpeMaty.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResuilt.Param == null ? 0 : apiResuilt.Param.Count ?? 0);
                    }
                }
                grvViewHisDepartmentExpeMaty.EndUpdate();
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
                var dataSource = grdHisDepartmentExpeMaty.DataSource as List<V_HIS_DEPARTMENT_EXPE_MATY>;
                if (dataSource != null)
                {
                    if (dataSource.Any(a => a.ID != (this.ActionType != GlobalVariables.ActionAdd && this.currentData != null ? this.currentData.ID : 0)
                    && a.MATERIAL_TYPE_ID == ((long)(cboMaty.EditValue ?? 0))
                    && a.DEPARTMENT_ID == ((long)(cboDepartment.EditValue ?? 0))
                    && (cboStock.EditValue == null ? !a.MEDI_STOCK_ID.HasValue : a.MEDI_STOCK_ID.HasValue && a.MEDI_STOCK_ID.Value == ((long)(cboStock.EditValue ?? 0)))
                    ))
                    {
                        XtraMessageBox.Show("Dữ liệu đã tồn tại");
                        return;
                    }
                }
                WaitingManager.Show();

                HIS_DEPARTMENT_EXPE_MATY UpdateDTO = new HIS_DEPARTMENT_EXPE_MATY();
                UpDataDTOFromDataForm(ref UpdateDTO);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var Result = new BackendAdapter(param).Post<HIS_DEPARTMENT_EXPE_MATY>(HisRequestUriStore.CustomerSource_Create, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (Result != null)
                    {
                        BackendDataWorker.Reset<HIS_DEPARTMENT_EXPE_MATY>();
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
                        var Resutl = new BackendAdapter(param).Post<HIS_DEPARTMENT_EXPE_MATY>(HisRequestUriStore.CustomerSource_UPDATE, ApiConsumers.MosConsumer, UpdateDTO, param);
                        if (Resutl != null)
                        {
                            BackendDataWorker.Reset<HIS_DEPARTMENT_EXPE_MATY>();
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
        private void UpDataDTOFromDataForm(ref HIS_DEPARTMENT_EXPE_MATY data)
        {
            try
            {
                if (this.ActionType != GlobalVariables.ActionAdd)
                {
                    V_HIS_DEPARTMENT_EXPE_MATY datarow = (V_HIS_DEPARTMENT_EXPE_MATY)grvViewHisDepartmentExpeMaty.GetFocusedRow();
                    if (datarow != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DEPARTMENT_EXPE_MATY>(data, datarow);
                    }
                }
                data.MATERIAL_TYPE_ID = cboMaty.EditValue == null ? 0 : (long)cboMaty.EditValue;
                data.DEPARTMENT_ID = cboDepartment.EditValue == null ? 0 : (long)cboDepartment.EditValue;
                data.MEDI_STOCK_ID = cboStock.EditValue == null ? (long?)null : (long)cboStock.EditValue;
                data.MAX_EXPEND = spinMaxExpend.EditValue == null ? (long?)null : (long)spinMaxExpend.Value;

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
                            txtMaty.Focus();
                            txtMaty.SelectAll();
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

        private void ChangedataRow(V_HIS_DEPARTMENT_EXPE_MATY data)
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
        private void FillDatatoControl(V_HIS_DEPARTMENT_EXPE_MATY data)
        {
            try
            {
                var selected = cboMaty.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>;
                txtMaty.Text = selected?.FirstOrDefault(o => o.ID == data?.MATERIAL_TYPE_ID)?.MATERIAL_TYPE_CODE;
                cboMaty.EditValue = data.MATERIAL_TYPE_ID;
                var selectedDepartment = cboDepartment.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>;
                txtDepartment.Text = selectedDepartment?.FirstOrDefault(o => o.ID == data.DEPARTMENT_ID)?.DEPARTMENT_CODE;
                cboDepartment.EditValue = data.DEPARTMENT_ID;
                cboStock.EditValue = data.MEDI_STOCK_ID;
                spinMaxExpend.EditValue = data?.MAX_EXPEND;
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
                txtMaty.Focus();
                txtMaty.SelectAll();
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

        private void GridViewCustomerSource_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    V_HIS_DEPARTMENT_EXPE_MATY datarow = (V_HIS_DEPARTMENT_EXPE_MATY)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    V_HIS_DEPARTMENT_EXPE_MATY datarow = view.GetRow(e.RowHandle) as V_HIS_DEPARTMENT_EXPE_MATY;
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
                    V_HIS_DEPARTMENT_EXPE_MATY dataRow = (V_HIS_DEPARTMENT_EXPE_MATY)grvViewHisDepartmentExpeMaty.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        e.Appearance.ForeColor = (dataRow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Color.Green : Color.Red);
                    }
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void GridViewCustomerSource_Click(object sender, EventArgs e)
        {
            try
            {
                V_HIS_DEPARTMENT_EXPE_MATY datarow = (V_HIS_DEPARTMENT_EXPE_MATY)grvViewHisDepartmentExpeMaty.GetFocusedRow();
                if (datarow != null)
                {
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                    this.currentData = datarow;
                    ChangedataRow(datarow);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }


        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                V_HIS_DEPARTMENT_EXPE_MATY datarow = (V_HIS_DEPARTMENT_EXPE_MATY)grvViewHisDepartmentExpeMaty.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var Result = new BackendAdapter(param).Post<HIS_DEPARTMENT_EXPE_MATY>(HisRequestUriStore.CustomerSource_CHANGELOCK, ApiConsumers.MosConsumer, datarow.ID, param);
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
                V_HIS_DEPARTMENT_EXPE_MATY datarow = (V_HIS_DEPARTMENT_EXPE_MATY)grvViewHisDepartmentExpeMaty.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var Result = new BackendAdapter(param).Post<HIS_DEPARTMENT_EXPE_MATY>(HisRequestUriStore.CustomerSource_CHANGELOCK, ApiConsumers.MosConsumer, datarow.ID, param);
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

                V_HIS_DEPARTMENT_EXPE_MATY datarow = (V_HIS_DEPARTMENT_EXPE_MATY)grvViewHisDepartmentExpeMaty.GetFocusedRow();
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


        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }


    }
}
