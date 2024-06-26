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
using System.Windows.Forms;
using Inventec.Desktop.Common.Modules;
using Inventec.UC.Paging;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using ACS.EFMODEL.DataModels;
using MOS.Filter;
using ACS.Filter;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;

namespace HIS.Desktop.Plugins.HisConfigGroup
{
    public partial class frmHisConfigGroup : HIS.Desktop.Utility.FormBase
    {

        #region ---Decalre
        PagingGrid pagingGrid;
        Module moduleData;
        int ActionType = -1;
        int startPage = 0;
        int dataTotal = 0;
        int rowCount = 0;

        HIS_CONFIG_GROUP CurrentData;
        List<ACS_ROLE> listAcsRoles { get; set; }

        #endregion
        #region Construct
        public frmHisConfigGroup(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        private void frmHisConfigGroup_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultData();
                EnableControlChanged(this.ActionType);
                SetCaptionByLanguageKey();
                IntCombocboVaitro();
                FillDataToGridControl();
                ValidateText(txtMa, 20);
                ValidateText(txtTen, 100);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #region ---Set data
        private void SetDefaultData()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtMa.Text = "";
                txtTen.Text = "";
                txtSearch.Text = "";
                txtSearch.Focus();
                try
                {
                    GridCheckMarksSelection gridCheckRole = cboVaitro.Properties.Tag as GridCheckMarksSelection;
                    gridCheckRole.ClearSelection(cboVaitro.Properties.View);
                    cboVaitro.Text = "";
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
        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = moduleData.text;
                }
            }
            catch (Exception ex)
            {

                LogAction.Warn(ex);
            }
        }

        private void ChangeDataRow(HIS_CONFIG_GROUP datarow)
        {
            try
            {
                if (datarow != null)
                {
                    FillDataEditorControl(datarow);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    if (datarow != null)
                    {
                        btnEdit.Enabled = (datarow.IS_ACTIVE == 1);
                    }

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);

                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void FillDataEditorControl(HIS_CONFIG_GROUP datarow)
        {
            try
            {

                txtMa.Text = datarow.CONFIG_GROUP_CODE;
                txtTen.Text = datarow.CONFIG_GROUP_NAME;
                if (cboVaitro.Properties.Tag == null)
                {
                    IntCombocboVaitro();
                }
                GridCheckMarksSelection gridCheckMark = cboVaitro.Properties.Tag as GridCheckMarksSelection;
                if (!String.IsNullOrWhiteSpace(datarow.ROLE_CODES) && cboVaitro.Properties.Tag != null)
                {
                    ProcessSelectBusiness(datarow.ROLE_CODES, gridCheckMark, cboVaitro);
                }
                else if (gridCheckMark != null)
                {
                    //GridCheckMarksSelection gridCheckMarkBusinessCodes = cboVaitro.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.ClearSelection(cboVaitro.Properties.View);
                    //cboVaitro.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectBusiness(string p, GridCheckMarksSelection gridCheckMark, GridLookUpEdit cbo)
        {
            try
            {
                List<ACS_ROLE> ds = cbo.Properties.DataSource as List<ACS_ROLE>;
                string[] arrays = p.Split(';');
                if (arrays != null && arrays.Length > 0)
                {
                    List<ACS_ROLE> selects = new List<ACS_ROLE>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ROLE_CODE == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UpdateDataFromform(HIS_CONFIG_GROUP updateDTO)
        {
            try
            {
                updateDTO.CONFIG_GROUP_CODE = txtMa.Text.Trim();
                updateDTO.CONFIG_GROUP_NAME = txtTen.Text.Trim();

                GridCheckMarksSelection gridCheckRole = cboVaitro.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckRole != null && gridCheckRole.SelectedCount > 0)
                {
                    List<string> codes = new List<string>();
                    foreach (ACS_ROLE rv in gridCheckRole.Selection)
                    {
                        if (rv != null && !codes.Contains(rv.ROLE_CODE.ToString()))
                            codes.Add(rv.ROLE_CODE.ToString());
                    }

                    updateDTO.ROLE_CODES = String.Join(";", codes);
                }
                else
                    updateDTO.ROLE_CODES = null;

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtMa.Focus();
                            txtMa.SelectAll();
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
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateText(DevExpress.XtraEditors.TextEdit textcontrol, int maxlangth)
        {
            try
            {
                ValidateMaxLength vali = new ValidateMaxLength();
                vali.txtEdit = textcontrol;
                vali.Maxlength = maxlangth;
                vali.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                vali.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textcontrol, vali);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void SaveProcessor()
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
                HIS_CONFIG_GROUP UpdateDTO = new HIS_CONFIG_GROUP();
                if (this.ActionType == GlobalVariables.ActionEdit && this.CurrentData != null && this.CurrentData.ID > 0)
                {
                    LoadCurrent(this.CurrentData.ID, ref UpdateDTO);
                }

                UpdateDataFromform(UpdateDTO);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var ResultData = new BackendAdapter(param).Post<HIS_CONFIG_GROUP>(HisConfigGroupUriStore.HisConfigGroup_Create, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (ResultData != null)
                    {
                        BackendDataWorker.Reset<HIS_CONFIG_GROUP>();
                        FillDataToGridControl();
                        success = true;
                        RestFromData();
                        try
                        {
                            GridCheckMarksSelection gridCheckRole = cboVaitro.Properties.Tag as GridCheckMarksSelection;
                            gridCheckRole.ClearSelection(cboVaitro.Properties.View);
                            cboVaitro.Text = "";
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Warn(ex);
                        }
                    }
                }
                else
                {
                    var ResultData = new BackendAdapter(param).Post<HIS_CONFIG_GROUP>(HisConfigGroupUriStore.HisConfigGroup_Update, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (ResultData != null)
                    {
                        BackendDataWorker.Reset<HIS_CONFIG_GROUP>();
                        FillDataToGridControl();
                        success = true;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadCurrent(long currentId, ref HIS_CONFIG_GROUP UpdateDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisConfigGroupFilter filter = new HisConfigGroupFilter();
                filter.ID = currentId;
                UpdateDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CONFIG_GROUP>>(HisConfigGroupUriStore.HisConfigGroup_Get, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void IntCombocboVaitro()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboVaitro.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__Vaitro);
                cboVaitro.Properties.Tag = gridCheck;
                cboVaitro.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                AcsRoleFilter filter = new AcsRoleFilter();
                filter.IS_ACTIVE = 1;
                listAcsRoles = new List<ACS_ROLE>();
                listAcsRoles = new BackendAdapter(param).Get<List<ACS_ROLE>>("api/AcsRole/Get", ApiConsumers.AcsConsumer, filter, null).ToList();

                cboVaitro.Properties.DataSource = listAcsRoles;
                cboVaitro.Properties.DisplayMember = "ROLE_NAME";
                cboVaitro.Properties.ValueMember = "ROLE_CODE";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboVaitro.Properties.View.Columns.AddField("ROLE_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboVaitro.Properties.View.Columns.AddField("ROLE_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "";

                cboVaitro.Properties.PopupFormWidth = 300;
                cboVaitro.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboVaitro.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboVaitro.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboVaitro.Properties.View);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Vaitro(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    foreach (ACS_ROLE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.ROLE_NAME + ",";
                    }
                    cboVaitro.Text = typeName;
                    cboVaitro.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region ---Load data to gridcontrol
        private void FillDataToGridControl()
        {

            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging1.pagingGrid != null)
                {

                    pagingSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pagingSize, this.gcConfigGroup);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        #endregion
        private void LoadPaging(object commonParam)
        {
            try
            {

                startPage = ((CommonParam)commonParam).Start ?? 0;
                int limit = ((CommonParam)commonParam).Limit ?? 0;
                CommonParam paramcommon = new CommonParam(startPage, limit);
                ApiResultObject<List<HIS_CONFIG_GROUP>> apiResult = null;
                HisConfigGroupFilter filter = new HisConfigGroupFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                // SetFilter(ref filter);
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.KEY_WORD = txtSearch.Text.Trim();
                }
                gcConfigGroup.DataSource = null;
                gvConfigGroup.BeginUpdate();
                apiResult = new BackendAdapter(paramcommon).GetRO<List<HIS_CONFIG_GROUP>>(HisConfigGroupUriStore.HisConfigGroup_Get, ApiConsumers.MosConsumer, filter, paramcommon);

                if (apiResult != null)
                {
                    var data = (List<HIS_CONFIG_GROUP>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gcConfigGroup.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }

                }

                gvConfigGroup.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetFilter(ref HisConfigGroupFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }


        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);

        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_CONFIG_GROUP data)
        {
            try
            {
                if (data != null)
                {
                    txtMa.Text = data.CONFIG_GROUP_CODE;
                    txtTen.Text = data.CONFIG_GROUP_NAME;
                    cboVaitro.Text = data.ROLE_CODES;
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    //btnEdit.Enabled = (this.currentData.IS_BASE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gvConfigGroup_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (HIS_CONFIG_GROUP)gvConfigGroup.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ROLE_STR")
                    {
                        if (data.IS_ACTIVE == 0)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gvConfigGroup_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_CONFIG_GROUP DataRow = (HIS_CONFIG_GROUP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (DataRow != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(DataRow.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(DataRow.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "ROLE_STR")
                        {
                            if (!string.IsNullOrEmpty(DataRow.ROLE_CODES))
                            {
                                e.Value = string.Join(", ", listAcsRoles.Where(o => (";" + DataRow.ROLE_CODES + ";").Contains(";" + o.ROLE_CODE + ";")).Select(o => o.ROLE_NAME));
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
        private void gvConfigGroup_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var dataRow = (HIS_CONFIG_GROUP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "DELETE")
                        {
                            e.RepositoryItem = (dataRow.IS_ACTIVE != 1 ? btnGEnable : btnGDelete);
                        }
                        if (e.Column.FieldName == "LOCK")
                        {
                            e.RepositoryItem = (dataRow.IS_ACTIVE == 1 ? btnGUnlock : btnGLock);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void gvConfigGroup_Click(object sender, EventArgs e)
        {
            try
            {
                var datarow = (HIS_CONFIG_GROUP)gvConfigGroup.GetFocusedRow();
                if (datarow != null)
                {
                    this.CurrentData = datarow;
                    ChangeDataRow(datarow);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
                
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                RestFromData();
                try
                {
                    GridCheckMarksSelection gridCheckRole = cboVaitro.Properties.Tag as GridCheckMarksSelection;
                    gridCheckRole.ClearSelection(cboVaitro.Properties.View);
                    cboVaitro.Text = "";
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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

                LogSystem.Error(ex);
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

                LogSystem.Error(ex);
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

                LogSystem.Error(ex);
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

                LogSystem.Error(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtMa_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMa.Focus();
                    txtTen.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_CONFIG_GROUP)gvConfigGroup.GetFocusedRow();
                bool success = false;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        HIS_CONFIG_GROUP Result = new BackendAdapter(param).Post<HIS_CONFIG_GROUP>(HisConfigGroupUriStore.HisConfigGroup_Changelock, ApiConsumer.ApiConsumers.MosConsumer, rowData.ID, param);
                        if (Result != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            btnEdit.Enabled = false;
                        }
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnGUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_CONFIG_GROUP)gvConfigGroup.GetFocusedRow();
                bool success = false;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        HIS_CONFIG_GROUP Result = new BackendAdapter(param).Post<HIS_CONFIG_GROUP>(HisConfigGroupUriStore.HisConfigGroup_Changelock, ApiConsumer.ApiConsumers.MosConsumer, rowData.ID, param);
                        if (Result != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            btnEdit.Enabled = true;
                        }
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_CONFIG_GROUP)gvConfigGroup.GetFocusedRow();
                bool success = false;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        success = new BackendAdapter(param).Post<bool>(HisConfigGroupUriStore.HisConfigGroup_Delete, ApiConsumer.ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            btnReset_Click(null, null);
                        }
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtMa_KeyDown(object sender, KeyEventArgs e)
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
                        btnReset.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void gcConfigGroup_DoubleClick_1(object sender, EventArgs e)
        {

        }

        private void cboVaitro_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;

                if (gridCheckMark == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("gridCheckMark is null");
                    return; 
                }
                foreach (ACS_ROLE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.ROLE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
        #endregion
