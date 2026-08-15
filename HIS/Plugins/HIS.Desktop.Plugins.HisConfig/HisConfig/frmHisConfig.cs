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
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utility;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Utilities.Extensions;
using System.Text;
using DevExpress.XtraEditors.Repository;

namespace HIS.Desktop.Plugins.HisConfig
{
    public partial class frmHisConfig : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_CONFIG currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        string workingModulelink = "";
        List<string> roleUser { get; set; }
        #endregion

        #region Construct
        public frmHisConfig(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

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

        public frmHisConfig(Inventec.Desktop.Common.Modules.Module moduleData, string _workingModuleLink)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;
                this.workingModulelink = _workingModuleLink;

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

        #region Private method
        private void frmHisConfig_Load(object sender, EventArgs e)
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

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisConfig.Resources.Lang", typeof(HIS.Desktop.Plugins.HisConfig.frmHisConfig).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisConfig.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisConfig.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisConfig.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisConfig.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisConfig.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmHisConfig.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisConfig.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                dxValidationProviderEditorInfo.RemoveControlError(txtKey);
                ResetFormData();
                EnableControlChanged(this.ActionType);
                txtKeyword.Text = "";
                txtModuleLinks.Text = this.workingModulelink;
                try
                {
                    GridCheckMarksSelection gridCheckMarkConfigGroupSearch = cboConfigGroupSearch.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkConfigGroupSearch.ClearSelection(cboConfigGroupSearch.Properties.View);
                    cboConfigGroupSearch.Text = "";
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                chkRefreshConfig.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
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

        private void InitTabIndex()
        {
            try
            {
                dicOrderTabIndexControl.Add("txtKey", 0);
                dicOrderTabIndexControl.Add("txtValue", 1);
                dicOrderTabIndexControl.Add("txtDefaultValue", 2);
                dicOrderTabIndexControl.Add("txtDescription", 3);
                //dicOrderTabIndexControl.Add("spMaxCapacity", 2);


                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                InitcboBranch();
                InitComboConfigGroup();
                InitComboConfigGroupSearch();
                //
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboConfigGroupSearch()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboConfigGroupSearch.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboConfigGroupSearch);
                cboConfigGroupSearch.Properties.Tag = gridCheck;
                cboConfigGroupSearch.Properties.View.OptionsSelection.MultiSelect = true;

                cboConfigGroupSearch.Properties.DataSource = BackendDataWorker.Get<HIS_CONFIG_GROUP>().ToList();
                cboConfigGroupSearch.Properties.DisplayMember = "CONFIG_GROUP_NAME";
                cboConfigGroupSearch.Properties.ValueMember = "CONFIG_GROUP_CODE";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboConfigGroupSearch.Properties.View.Columns.AddField("CONFIG_GROUP_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboConfigGroupSearch.Properties.View.Columns.AddField("CONFIG_GROUP_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "";

                cboConfigGroupSearch.Properties.PopupFormWidth = 200;
                cboConfigGroupSearch.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboConfigGroupSearch.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboConfigGroupSearch.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboConfigGroupSearch.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboConfigGroup()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboConfigGroup.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboConfigGroup);
                cboConfigGroup.Properties.Tag = gridCheck;
                cboConfigGroup.Properties.View.OptionsSelection.MultiSelect = true;

                cboConfigGroup.Properties.DataSource = BackendDataWorker.Get<HIS_CONFIG_GROUP>().ToList();
                cboConfigGroup.Properties.DisplayMember = "CONFIG_GROUP_NAME";
                cboConfigGroup.Properties.ValueMember = "CONFIG_GROUP_CODE";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboConfigGroup.Properties.View.Columns.AddField("CONFIG_GROUP_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboConfigGroup.Properties.View.Columns.AddField("CONFIG_GROUP_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "";

                cboConfigGroup.Properties.PopupFormWidth = 200;
                cboConfigGroup.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboConfigGroup.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboConfigGroup.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboConfigGroup.Properties.View);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboConfigGroup(object sender, EventArgs e)
        {
            try
            {
                string name = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    foreach (HIS_CONFIG_GROUP er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        name += er.CONFIG_GROUP_NAME + ",";
                    }
                    cboConfigGroup.Text = name;
                    cboConfigGroup.ToolTip = name;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboConfigGroupSearch(object sender, EventArgs e)
        {
            try
            {
                string name = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    foreach (HIS_CONFIG_GROUP er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        name += er.CONFIG_GROUP_NAME + ",";
                    }
                    cboConfigGroupSearch.Text = name;
                    cboConfigGroupSearch.ToolTip = name;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitcboBranch()
        {
            var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>();
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("BRANCH_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(cboBranch, data, controlEditorADO);
        }
        #region Init combo

        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {

                WaitingManager.Show();
                BackendDataWorker.Reset<V_HIS_CONFIG>();
                List<V_HIS_CONFIG> dataSource = BackendDataWorker.Get<V_HIS_CONFIG>().Where(o => CheckConfig(o, roleUser)).ToList();
                if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                {
                    string keyword = txtKeyword.Text.Trim().ToLower();
                    dataSource = dataSource
                        .Where(o => (!String.IsNullOrWhiteSpace(o.KEY) && o.KEY.ToLower().Contains(keyword))
                        || (!String.IsNullOrWhiteSpace(o.VALUE) && o.VALUE.ToLower().Contains(keyword))
                        || (!String.IsNullOrWhiteSpace(o.DESCRIPTION) && o.DESCRIPTION.ToLower().Contains(keyword))
                        || (!String.IsNullOrWhiteSpace(o.DEFAULT_VALUE) && o.DEFAULT_VALUE.ToLower().Contains(keyword))
                        || (!String.IsNullOrWhiteSpace(o.CONFIG_CODE) && o.CONFIG_CODE.ToLower().Contains(keyword))
                        ).OrderByDescending(o => o.MODIFY_TIME).ToList();
                }

                if (!String.IsNullOrWhiteSpace(txtModuleLinks.Text) && dataSource != null && dataSource.Count() > 0)
                {
                    dataSource = dataSource
                       .Where(o => !String.IsNullOrWhiteSpace(o.MODULE_LINKS)
                           && o.MODULE_LINKS.ToLower().Contains(txtModuleLinks.Text.Trim().ToLower())
                       ).OrderByDescending(o => o.MODIFY_TIME).ToList();
                }

                GridCheckMarksSelection gridCheckMarkConfigGroup = cboConfigGroupSearch.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckMarkConfigGroup != null && gridCheckMarkConfigGroup.SelectedCount > 0 && dataSource != null && dataSource.Count() > 0)
                {
                    List<string> codes = new List<string>();
                    foreach (HIS_CONFIG_GROUP rv in gridCheckMarkConfigGroup.Selection)
                    {
                        if (rv != null && !codes.Contains(rv.CONFIG_GROUP_CODE))
                            codes.Add(rv.CONFIG_GROUP_CODE);
                    }
                    dataSource = dataSource.Where(o => !string.IsNullOrEmpty(o.CONFIG_GROUP_CODES) && o.CONFIG_GROUP_CODES.Split(';').ToList().Exists(p => codes.Contains(p))).OrderByDescending(o => o.MODIFY_TIME).ToList();
                }

                dnNavigation.DataSource = dataSource != null ? dataSource.OrderByDescending(o => o.MODIFY_TIME).ToList() : dataSource;
                gridviewFormList.BeginUpdate();
                gridviewFormList.GridControl.DataSource = null;
                gridviewFormList.GridControl.DataSource = dataSource != null ? dataSource.OrderByDescending(o => o.MODIFY_TIME).ToList() : dataSource;
                gridviewFormList.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        bool CheckConfig(V_HIS_CONFIG cfg, List<string> roles)
        {
            bool rs = false;
            try
            {
                if (string.IsNullOrEmpty(cfg.ROLE_CODES))
                    rs = true;
                else
                {
                    if (roles != null && roles.Count() > 0 && roles.Exists(o => (";" + cfg.ROLE_CODES + ";").Contains((";" + o + ";"))))
                        rs = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return rs;
            }
            return rs;
        }

        private void SetFilterNavBar(ref HisConfigViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.NULL__OR__BRANCH_ID = BranchDataWorker.GetCurrentBranchId();
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_CONFIG)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_CONFIG)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.V_HIS_CONFIG pData = (MOS.EFMODEL.DataModels.V_HIS_CONFIG)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == 1)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    //else if (e.Column.FieldName == "CONFIG_GROUP_NAME_DISPLAY")
                    //{
                    //    try
                    //    {
                    //        if (!string.IsNullOrEmpty(pData.CONFIG_GROUP_NAMES))
                    //        {

                    //        }
                    //        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error(ex);
                    //    }
                    //}
                }

                gridControlFormList.RefreshDataSource();
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
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_CONFIG)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_CONFIG)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentData = rowData;
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

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.V_HIS_CONFIG)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.V_HIS_CONFIG>)[dnNavigation.Position];
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_CONFIG data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    cboBranch.ReadOnly = !(data.KEY == "MOS.HIS_TREATMENT.AUTO_FINISH_SERVICE_REQ.SERVICE_CODE");

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_CONFIG data)
        {
            try
            {
                if (data != null)
                {
                    txtConfigCode.Text = data.CONFIG_CODE;
                    cboBranch.EditValue = data.BRANCH_ID;
                    txtKey.Text = data.KEY;
                    txtDescription.Text = data.DESCRIPTION;
                    txtValue.Text = data.VALUE;
                    txtDefaultValue.Text = data.DEFAULT_VALUE;

                    GridCheckMarksSelection gridCheckMarkConfigGroup = cboConfigGroup.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkConfigGroup.ClearSelection(cboConfigGroup.Properties.View);
                    if (!String.IsNullOrWhiteSpace(data.CONFIG_GROUP_CODES) && cboConfigGroup.Properties.Tag != null)
                    {
                        ProcessSelectBusiness(data.CONFIG_GROUP_CODES, gridCheckMarkConfigGroup);
                    }
                    else
                    {
                        cboConfigGroup.EditValue = null;
                        GridCheckMarksSelection gridCheckMarkBusinessCodes = cboConfigGroup.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMarkBusinessCodes.ClearSelection(cboConfigGroup.Properties.View);
                    }
                    //spMaxCapacity.EditValue = data.MAX_CAPACITY;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectBusiness(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_CONFIG_GROUP> ds = cboConfigGroup.Properties.DataSource as List<HIS_CONFIG_GROUP>;
                string[] arrays = p.Split(';');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_CONFIG_GROUP> selects = new List<HIS_CONFIG_GROUP>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.CONFIG_GROUP_CODE == item) : null;
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

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
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
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    try
                    {
                        GridCheckMarksSelection gridCheckMarkConfigGroup = cboConfigGroup.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMarkConfigGroup.ClearSelection(cboConfigGroup.Properties.View);
                        cboConfigGroup.Text = "";
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }

                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            //txtKey.Focus();
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_CONFIG currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisConfigFilter filter = new HisConfigFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(new CommonParam()).Get<List<HIS_CONFIG>>("api/HisConfig/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault(o => o.ID == currentId);
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
                //btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
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

        #region Button handler
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
                //FillDataToGridControl();
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
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_CONFIG)gridviewFormList.GetFocusedRow();
                HisConfigViewFilter filter = new HisConfigViewFilter();
                filter.ID = rowData.ID;
                var data = new BackendAdapter(param).Get<List<HIS_CONFIG>>(HisRequestUriStore.MOSHIS_CONFIG_GET_VIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                if (rowData != null)
                {
                    bool success = false;
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_CONFIG_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                    if (success)
                    {
                        BackendDataWorker.Reset<V_HIS_CONFIG>();
                        FillDataToGridControl();
                        HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                        currentData = ((List<V_HIS_CONFIG>)gridControlFormList.DataSource).FirstOrDefault();
                    }
                    MessageManager.Show(this, param, success);
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
                SetDefaultValue();
                ResetFormData();
                SetFocusEditor();
                chkRefreshConfig.Checked = true;
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
                if (!btnEdit.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_CONFIG updateDTO = new MOS.EFMODEL.DataModels.HIS_CONFIG();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CONFIG>(HisRequestUriStore.MOSHIS_CONFIG_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        BackendDataWorker.Reset<V_HIS_CONFIG>();
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                        if (chkRefreshConfig.Checked)
                            HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CONFIG>(HisRequestUriStore.MOSHIS_CONFIG_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        BackendDataWorker.Reset<V_HIS_CONFIG>();
                        success = true;
                        FillDataToGridControl();
                        if (chkRefreshConfig.Checked)
                            HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                    }
                }

                if (success)
                {
                    SetFocusEditor();

                    // update lai ShowDecimalOption
                    if (this.currentData.KEY == "HIS.Desktop.Plugins.ShowDecimalOption")
                    {
                        string showDecimal = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ShowDecimalOption");
                        Inventec.Common.Number.Convert.isShowDecimalOption = showDecimal == "1";
                    }

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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_CONFIG data)
        {
            try
            {
                //if (data == null)
                //    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_CONFIG) is null");
                //var rowData = (MOS.EFMODEL.DataModels.HIS_CONFIG)gridviewFormList.GetFocusedRow();
                //if (rowData != null)
                //{
                //    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_CONFIG>(rowData, data);
                //    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_CONFIG currentDTO)
        {
            try
            {
                if (cboBranch.EditValue != null)
                {
                    currentDTO.BRANCH_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBranch.EditValue.ToString());
                }
                else
                {
                    currentDTO.BRANCH_ID = null;
                }
                currentDTO.KEY = txtKey.Text.Trim();
                currentDTO.DESCRIPTION = txtDescription.Text.Trim();
                currentDTO.DEFAULT_VALUE = txtDefaultValue.Text.Trim();
                currentDTO.VALUE = txtValue.Text.Trim();

                GridCheckMarksSelection gridCheckMarkConfigGroup = cboConfigGroup.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckMarkConfigGroup != null && gridCheckMarkConfigGroup.SelectedCount > 0)
                {
                    List<string> codes = new List<string>();
                    foreach (HIS_CONFIG_GROUP rv in gridCheckMarkConfigGroup.Selection)
                    {
                        if (rv != null && !codes.Contains(rv.CONFIG_GROUP_CODE))
                            codes.Add(rv.CONFIG_GROUP_CODE);
                    }

                    currentDTO.CONFIG_GROUP_CODES = String.Join(";", codes);
                }
                else
                    currentDTO.CONFIG_GROUP_CODES = null;
                //currentDTO.MAX_CAPACITY = (long)spMaxCapacity.Value;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtKey);
                //ValidationSingleControl(txtDescription);
                //ValidationSingleControl1();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl1()
        {
            try
            {
                //ValidatespMax validate = new ValidatespMax();
                //validate.spMax = spMaxCapacity;
                //validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                //validate.ErrorType = ErrorType.Warning;
                //this.dxValidationProviderEditorInfo.SetValidationRule(spMaxCapacity, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
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
        #endregion

        #region Tooltip
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
        #endregion

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                txtDefaultValue.Properties.ReadOnly = true;
                txtKey.Properties.ReadOnly = true;
                txtDescription.Properties.ReadOnly = true;

                //Gan gia tri mac dinh
                SetDefaultValue();

                // Get dataSource
                GetDataSourceConfig();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDataSourceConfig()
        {
            try
            {
                var token = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                if (token != null && token.RoleDatas != null && token.RoleDatas.Count > 0)
                {
                    this.roleUser = token.RoleDatas.Select(o => o.RoleCode).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut
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
                //if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                //btnAdd_Click(null, null);
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
        #endregion

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            CommonParam param = new CommonParam();
            HIS_CONFIG success = new HIS_CONFIG();
            bool rs = false;
            //bool notHandler = false;
            try
            {

                V_HIS_CONFIG data = (V_HIS_CONFIG)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_CONFIG data1 = new HIS_CONFIG();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_CONFIG>(HisRequestUriStore.MOSHIS_CONFIG_CHANGE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_CONFIG>();
                        FillDataToGridControl();
                        HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                        rs = true;
                    }
                    MessageManager.Show(this, param, rs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void btnGunLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_CONFIG success = new HIS_CONFIG();
            bool rs = false;
            //bool notHandler = false;
            try
            {

                V_HIS_CONFIG data = (V_HIS_CONFIG)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_CONFIG data1 = new HIS_CONFIG();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_CONFIG>(HisRequestUriStore.MOSHIS_CONFIG_CHANGE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_CONFIG>();
                        FillDataToGridControl();
                        HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                        rs = true;
                    }
                    MessageManager.Show(this, param, rs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_CONFIG data = (V_HIS_CONFIG)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGunLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : repositoryItemButtonEdit1);

                    }
                    if (e.Column.FieldName == "Reset")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Btn_Reset_Enable : Btn_Reset_Disable);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spMaxCapacity_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        //  bt.Focus();
                    }
                    else
                        btnEdit.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                V_HIS_CONFIG data = (V_HIS_CONFIG)gridviewFormList.GetRow(e.RowHandle);
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void gridControlFormList_Click(object sender, EventArgs e)
        {

        }

        private void txtKey_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtValue.Focus();
                    txtValue.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtValue_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDefaultValue.Focus();
                    txtDefaultValue.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDefaultValue_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    if (this.ActionType == GlobalVariables.ActionAdd)
                //    {
                //        btnAdd.Focus();
                //    }
                //    else
                //        btnEdit.Focus();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_Reset_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_CONFIG success = new HIS_CONFIG();
            bool rs = false;
            try
            {

                V_HIS_CONFIG data = (V_HIS_CONFIG)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show("Bạn có muốn reset dữ liệu không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_CONFIG dataReset = new HIS_CONFIG();
                    LoadCurrent(data.ID, ref dataReset);
                    dataReset.VALUE = dataReset.DEFAULT_VALUE;

                    WaitingManager.Show();

                    string mosmasterAddressUri = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.MASTER_ADDRESS");
                    if (!String.IsNullOrEmpty(mosmasterAddressUri))
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mosmasterAddressUri), mosmasterAddressUri));
                        Inventec.Common.WebApiClient.ApiConsumer mosConsumer = new Inventec.Common.WebApiClient.ApiConsumer(mosmasterAddressUri, GlobalVariables.APPLICATION_CODE);
                        mosConsumer.SetTokenCode(ApiConsumers.MosConsumer.GetTokenCode());
                        success = new BackendAdapter(param).Post<HIS_CONFIG>(HisRequestUriStore.MOSHIS_CONFIG_UPDATE, mosConsumer, dataReset, param);
                    }
                    else
                    {
                        success = new BackendAdapter(param).Post<HIS_CONFIG>(HisRequestUriStore.MOSHIS_CONFIG_UPDATE, ApiConsumers.MosConsumer, dataReset, param);
                    }

                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_CONFIG>();
                        FillDataToGridControl();
                        Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.Refresh();
                        rs = true;
                    }
                    MessageManager.Show(this, param, rs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                List<HIS_CONFIG> listData = new List<HIS_CONFIG>();

                var listDataSource = (List<V_HIS_CONFIG>)gridControlFormList.DataSource;
                if (listDataSource != null && listDataSource.Count > 0)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_CONFIG, HIS_CONFIG>();
                    listData = AutoMapper.Mapper.Map<List<HIS_CONFIG>>(listDataSource);
                    listData.ForEach(o => o.VALUE = o.DEFAULT_VALUE);
                }

                CommonParam param = new CommonParam();
                bool rs = false;
                try
                {
                    if (MessageBox.Show("Bạn có muốn reset dữ liệu không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        List<HIS_CONFIG> success = null;

                        string mosmasterAddressUri = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.MASTER_ADDRESS");
                        if (!String.IsNullOrEmpty(mosmasterAddressUri))
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mosmasterAddressUri), mosmasterAddressUri));
                            Inventec.Common.WebApiClient.ApiConsumer mosConsumer = new Inventec.Common.WebApiClient.ApiConsumer(mosmasterAddressUri, GlobalVariables.APPLICATION_CODE);
                            mosConsumer.SetTokenCode(ApiConsumers.MosConsumer.GetTokenCode());
                            success = new BackendAdapter(param).Post<List<HIS_CONFIG>>(HisRequestUriStore.MOSHIS_CONFIG_UPDATE_LIST, mosConsumer, listData, param);
                        }
                        else
                        {
                            success = new BackendAdapter(param).Post<List<HIS_CONFIG>>(HisRequestUriStore.MOSHIS_CONFIG_UPDATE_LIST, ApiConsumers.MosConsumer, listData, param);
                        }

                        WaitingManager.Hide();
                        if (success != null)
                        {
                            FillDataToGridControl();
                            Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.Refresh();
                            rs = true;
                        }
                        MessageManager.Show(this, param, rs);
                    }
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboBranch_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                cboBranch.EditValue = null;
            }
        }

        private void txtKey_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtKey.Text) && txtKey.Text == "MOS.HIS_TREATMENT.AUTO_FINISH_SERVICE_REQ.SERVICE_CODE")
                {
                    cboBranch.ReadOnly = false;
                }
                else
                {
                    cboBranch.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisImportConfig").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisImportConfig'");

                List<object> listArgs = new List<object>();
                listArgs.Add(this.moduleData);

                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData != null ? this.moduleData.RoomId : 0, this.moduleData != null ? this.moduleData.RoomTypeId : 0), listArgs);
                if (extenceInstance == null) throw new NullReferenceException("extenceInstance is null");

                WaitingManager.Hide();
                ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            CreateExport();
        }

        private void bbtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnImport_Click(null, null);
        }

        private void bbtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnExport_Click(null, null);
        }

        private void CreateExport()
        {
            try
            {
                List<string> expCode = new List<string>();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "EXPORT_HIS_Config.xlsx");

                //chọn đường dẫn
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không tìm thấy file", templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    MOS.Filter.HisConfigViewFilter filter = new HisConfigViewFilter();
                    var HisConfigs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_CONFIG>>("api/HisConfig/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                    ProcessData(HisConfigs, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Xuất file thành công");

                                if (MessageBox.Show("Bạn có muốn mở file?",
                                    "Thông báo", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<V_HIS_CONFIG> data, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "ExportResult", data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }

        private void txtModuleLinks_KeyUp(object sender, KeyEventArgs e)
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_CONFIG)gridviewFormList.GetFocusedRow();
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

        private void btnGTutorial_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_CONFIG)gridviewFormList.GetFocusedRow();

                string key = rowData.KEY;
                string domain = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_CRM;
                string url = string.Format("{0}ords/f?p=104:5:::::P5_CODE:{1}", domain, key);
                Inventec.Common.Logging.LogSystem.Debug("Open brower - url__:" + url);
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(domain))
                {
                    try
                    {
                        WaitingManager.Show();
                        System.Diagnostics.Process.Start(url);
                        WaitingManager.Hide();
                    }
                    catch (Exception ex)
                    {
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConfigGroup_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dienDieuTri = "";
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_CONFIG_GROUP rv in gridCheckMark.Selection)
                {
                    dienDieuTri += rv.CONFIG_GROUP_NAME + ", ";
                }
                e.DisplayText = dienDieuTri;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboConfigGroup_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboConfigGroup.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboConfigGroup.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboConfigGroup.Properties.View);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboConfigGroupSearch_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dienDieuTri = "";
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_CONFIG_GROUP rv in gridCheckMark.Selection)
                {
                    dienDieuTri += rv.CONFIG_GROUP_NAME + ", ";
                }
                e.DisplayText = dienDieuTri;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboConfigGroupSearch_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboConfigGroupSearch.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboConfigGroupSearch.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboConfigGroupSearch.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
