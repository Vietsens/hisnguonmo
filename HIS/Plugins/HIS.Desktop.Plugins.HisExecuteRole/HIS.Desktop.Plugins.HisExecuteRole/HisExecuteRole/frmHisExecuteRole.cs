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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.HisExecuteRole.Entity;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisExecuteRole.HisExecuteRole
{
    public partial class frmHisExecuteRole : HIS.Desktop.Utility.FormBase
    {

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE currentData;
        List<HIS_EXECUTE_ROLE> listKhoa = new List<HIS_EXECUTE_ROLE>();
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        long RoleID;
        bool IsDebate;

        public frmHisExecuteRole(Inventec.Desktop.Common.Modules.Module moduleData, bool isDebate)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;
                this.IsDebate = isDebate;

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

        private void frmHisExecuteRole_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {

                Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole = new ResourceManager("HIS.Desktop.Plugins.HisExecuteRole.Resources.Lang", typeof(HIS.Desktop.Plugins.HisExecuteRole.HisExecuteRole.frmHisExecuteRole).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.layoutControl4.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.layoutControl7.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.btnSearch.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.STT.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColDepartmentCode.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColDepartmentCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColDepartmentCode.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColDepartmentName.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColDepartmentName.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColDepartmentName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColDepartmentName.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                //this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.btnCancel.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.btnAdd.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                //this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.dnNavigation.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.btnEdit.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.lciDepartmentCode.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.lciDepartmentCode.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.lciDepartmentName.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.lciDepartmentName.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.bar1.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                //this.lcIsSurgMain.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.lcIsSurgMain.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColIsSurgMain.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColIsSurgMain.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColIsSurgMain.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColIsSurgMain.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.gridColIsDisable.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.gridColIsDisable.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.gridColIsDisable.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.gridColIsDisable.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                //this.lcIsTitle.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.lcIsTitle.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                //this.lcIsPosition.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.lcIsPosition.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                //this.lcIsSurgry.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.lcIsSurgry.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                //this.lcIsStock.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.lcIsStock.Text", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColIsTitle.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColIsTitle.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColIsTitle.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColIsTitle.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColIsPosition.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColIsPosition.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColIsPosition.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColIsPosition.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColIsSurgry.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColIsSurgry.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColIsSurgry.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColIsSurgry.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColIsStock.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColIsStock.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());
                this.grdColIsStock.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRole.grdColIsStock.ToolTip", Resources.ResourceLanguageManager.LanguagefrmHisExecuteRole, LanguageManager.GetCulture());

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

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// Gan focus vao control mac dinh
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        private void FillDataToControlsForm()
        {
            try
            {
                //InitComboBranchId();

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FillDataToGridControl()
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// Ham goi api lay du lieu phan trang
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>> apiResult = null;
                HisExecuteRoleFilter filter = new HisExecuteRoleFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                //dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>>(HisRequestUriStore.MOSHIS_EXECUTE_ROLE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null && apiResult.Data.Count > 0)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>)apiResult.Data;
                    if (data != null)
                    {
                        //dnNavigation.DataSource = data;
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }

                gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisExecuteRoleFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
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

                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE pData = (MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage + ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IS_TITLE_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_TITLE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {

                            LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_POSITION_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_POSITION == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {

                            LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_STOCK_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_STOCK == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {

                            LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_SURGRY_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_SURGRY == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {

                            LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_SURG_MAIN_STR")
                    {
                        try
                        {
                            e.Value = pData.IS_SURG_MAIN == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {

                            LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_DISABLE_IN_EKIP_str")
                    {
                        try
                        {
                            e.Value = pData.IS_DISABLE_IN_EKIP == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {

                            LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                    gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE data)
        {
            try
            {
                if (data != null)
                {
                    RoleID = data.ID;
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE data)
        {
            try
            {
                if (data != null)
                {
                    txtExcuteRoleCode.Text = data.EXECUTE_ROLE_CODE;
                    txtExcuteRoleName.Text = data.EXECUTE_ROLE_NAME;
                    checkIsSurgMain.Checked = data.IS_SURG_MAIN == 1 ? true : false;
                    chkTechnician.Checked = data.IS_SUBCLINICAL == 1 ? true : false;
                    chkReadResults.Checked = data.IS_SUBCLINICAL_RESULT == 1 ? true : false;
                    chkAllowSimultaneity.Checked = data.ALLOW_SIMULTANEITY == 1 ? true : false;
                    chkIsTitle.Checked = data.IS_TITLE == 1 ? true : false;
                    chkIsPosition.Checked = data.IS_POSITION == 1 ? true : false;
                    chkIsStock.Checked = data.IS_STOCK == 1 ? true : false;
                    chkIsSurgry.Checked = data.IS_SURGRY == 1 ? true : false;
                    chkSingleInEkip.Checked = data.IS_SINGLE_IN_EKIP == 1 ? true : false;
                    chkDisableEkip.Checked = data.IS_DISABLE_IN_EKIP == 1 ? true : false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Gan focus vao control mac dinh
        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                txtExcuteRoleCode.Text = "";
                txtExcuteRoleName.Text = "";
                chkIsTitle.Checked = false;
                chkIsPosition.Checked = false;
                chkIsSurgry.Checked = false;
                checkIsSurgMain.Checked = false;
                chkTechnician.Checked = false;
                chkReadResults.Checked = false;
                chkAllowSimultaneity.Checked = false;
                chkIsStock.Checked = false;
                chkSingleInEkip.Checked = false;
                chkDisableEkip.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExecuteRoleFilter filter = new HisExecuteRoleFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>>(HisRequestUriStore.MOSHIS_EXECUTE_ROLE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
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

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                    HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_EXECUTE_ROLE_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_EXECUTE_ROLE>();
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                SetFocusEditor();
                txtExcuteRoleCode.Focus();
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

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE updateDTO = new MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>(HisRequestUriStore.MOSHIS_EXECUTE_ROLE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    if (RoleID > 0)
                    {
                        updateDTO.ID = RoleID;
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>(HisRequestUriStore.MOSHIS_EXECUTE_ROLE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToGridControl();
                        }
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<HIS_EXECUTE_ROLE>();
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE currentDTO)
        {
            try
            {
                currentDTO.EXECUTE_ROLE_CODE = txtExcuteRoleCode.Text.Trim();
                currentDTO.EXECUTE_ROLE_NAME = txtExcuteRoleName.Text.Trim();
                currentDTO.IS_TITLE = chkIsTitle.Checked ? (short)1 : (short)0;
                currentDTO.IS_POSITION = chkIsPosition.Checked ? (short)1 : (short)0;
                currentDTO.IS_SURGRY = chkIsSurgry.Checked ? (short)1 : (short)0;
                currentDTO.IS_STOCK = chkIsStock.Checked ? (short)1 : (short)0;
                currentDTO.IS_SURG_MAIN = checkIsSurgMain.Checked ? (short)1 : (short)0;
                currentDTO.ALLOW_SIMULTANEITY = chkAllowSimultaneity.Checked ? (short)1 : (short)0;

                if (chkTechnician.Checked)
                {
                    currentDTO.IS_SUBCLINICAL = 1;
                }
                else if (!chkTechnician.Checked)
                {
                    currentDTO.IS_SUBCLINICAL = null;
                }

                if (chkReadResults.Checked)
                {
                    currentDTO.IS_SUBCLINICAL_RESULT = 1;
                }
                else if (!chkReadResults.Checked)
                {
                    currentDTO.IS_SUBCLINICAL_RESULT = null;
                }

                if (chkSingleInEkip.Checked)
                {
                    currentDTO.IS_SINGLE_IN_EKIP = 1;
                }
                else if (!chkSingleInEkip.Checked)
                {
                    currentDTO.IS_SINGLE_IN_EKIP = null;
                }

                if (chkDisableEkip.Checked)
                {
                    currentDTO.IS_DISABLE_IN_EKIP = 1;
                }
                else if (!chkDisableEkip.Checked)
                {
                    currentDTO.IS_DISABLE_IN_EKIP = null;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtExcuteRoleCode);
                ValidationSingleControl(txtExcuteRoleName);
                if (this.IsDebate)
                {
                    ValidationCheckControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationCheckControl()
        {
            try
            {
                ValidateRule.CheckValidationRule validRule = new ValidateRule.CheckValidationRule();

                validRule.Chk1 = chkIsTitle;
                validRule.Chk2 = chkIsPosition;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(chkIsTitle, validRule);
                dxValidationProviderEditorInfo.SetValidationRule(chkIsPosition, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Focus default
                SetDefaultFocus();

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_EXECUTE_ROLE hisDepertments = new HIS_EXECUTE_ROLE();
            bool notHandler = false;
            try
            {
                HIS_EXECUTE_ROLE dataDepartment = (HIS_EXECUTE_ROLE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonKhoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_EXECUTE_ROLE data1 = new HIS_EXECUTE_ROLE();
                    data1.ID = dataDepartment.ID;
                    WaitingManager.Show();
                    hisDepertments = new BackendAdapter(param).Post<HIS_EXECUTE_ROLE>("api/HisExecuteRole/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisDepertments != null) FillDataToGridControl();
                    BackendDataWorker.Reset<HIS_EXECUTE_ROLE>();
                }
                notHandler = true;
                //DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ phòng", "Thông báo");
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_EXECUTE_ROLE hisDepertments = new HIS_EXECUTE_ROLE();
            bool notHandler = false;
            try
            {
                HIS_EXECUTE_ROLE dataDepartment = (HIS_EXECUTE_ROLE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonBoKhoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_EXECUTE_ROLE data1 = new HIS_EXECUTE_ROLE();
                    data1.ID = dataDepartment.ID;
                    WaitingManager.Show();
                    hisDepertments = new BackendAdapter(param).Post<HIS_EXECUTE_ROLE>("api/HisExecuteRole/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisDepertments != null) FillDataToGridControl();
                    BackendDataWorker.Reset<HIS_EXECUTE_ROLE>();
                }
                notHandler = true;
                MessageManager.Show(this.ParentForm, param, notHandler);
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
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_EXECUTE_ROLE data = (HIS_EXECUTE_ROLE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_TRUE ? btnUnlock : btnLock);
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_TRUE ? btnGEdit : btnGEdit_Disable);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExcuteRoleCode_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtExcuteRoleCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExcuteRoleName.Focus();
                    txtExcuteRoleName.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtExcuteRoleName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //checkIsSurgMain.Focus();
                    chkIsTitle.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void checkIsSurgMain_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkTechnician.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void chkTechnician_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkReadResults.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void chkReadResults_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAllowSimultaneity.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void chkIsTitle_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsPosition.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void chkIsSurgry_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkIsSurgry.Checked)
                        checkIsSurgMain.Focus();
                    else
                        chkIsStock.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void chkIsStock_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                    if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
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

        private void chkIsPosition_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSurgry.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void chkIsSurgry_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                if (chkIsSurgry.Checked)
                {
                    checkIsSurgMain.Enabled = true;
                }
                else
                {
                    checkIsSurgMain.Enabled = false;
                    checkIsSurgMain.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        
    }
}
