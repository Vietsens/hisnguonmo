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
using SAR.EFMODEL.DataModels;
using SAR.Filter;
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

namespace SAR.Desktop.Plugins.SarReportType
{
    public partial class frmSarReportType : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        SAR.EFMODEL.DataModels.SAR_REPORT_TYPE currentData;
        List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE_GROUP> reportTypeGroups;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;
        #endregion

        #region Construct
        public frmSarReportType(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;
                this.Text = moduleData.text;

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
        private void frmSarReportType_Load(object sender, EventArgs e)
        {
            try
            {
                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();
                LoadComboStatusHour();

                //Load du lieu
                FillDataToGridControl();

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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("SAR.Desktop.Plugins.SarReportType.Resources.Lang", typeof(SAR.Desktop.Plugins.SarReportType.frmSarReportType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TabNormal.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.TabNormal.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboReportTypeGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSarReportType.cboReportTypeGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciCode.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.LciCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciName.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.LciName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciReportGroup.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.LciReportGroup.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciMota.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.LciMota.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TabTkb.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.TabTkb.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboTkbGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSarReportType.CboTkbGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciTkbCode.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.LciTkbCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciTkbName.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.LciTkbName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciTkbGroupType.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.LciTkbGroupType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciTbkDescription.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.LciTbkDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciTkbSql.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.LciTkbSql.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefesh.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.btnRefesh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmSarReportType.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.grdColCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.grdColName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmSarReportType.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmSarReportType.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSarReportType.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.barButtonItem4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem5.Caption = Inventec.Common.Resource.Get.Value("frmSarReportType.barButtonItem5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton1.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.simpleButton1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.textEdit1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSarReportType.textEdit1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton2.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.simpleButton2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton3.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.simpleButton3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton4.Text = Inventec.Common.Resource.Get.Value("frmSarReportType.simpleButton4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                this.currentData = new SAR_REPORT_TYPE();
                this.ActionType = GlobalVariables.ActionAdd;
                ResetFormData();
                EnableControlChanged(this.ActionType);
                //txtKeyword.Text = "";
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

        private void FillDataToControlsForm()
        {
            try
            {
                CommonParam param = new CommonParam();
                SarReportTypeGroupFilter filter = new SarReportTypeGroupFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                reportTypeGroups = new BackendAdapter(param).Get<List<SAR_REPORT_TYPE_GROUP>>("api/SarReportTypeGroup/Get", ApiConsumers.SarConsumer, filter, null);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("REPORT_TYPE_GROUP_CODE", "", 100, 1, true));
                columnInfos.Add(new ColumnInfo("REPORT_TYPE_GROUP_NAME", "", 400, 1, true));

                ControlEditorADO controlEditorADO = new ControlEditorADO("REPORT_TYPE_GROUP_NAME", "ID", columnInfos, false, 500);
                ControlEditorLoader.Load(cboReportTypeGroup, reportTypeGroups, controlEditorADO);
                ControlEditorLoader.Load(CboTkbGroup, reportTypeGroups, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, pageSize, this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>> apiResult = null;
                SarReportTypeFilter filter = new SarReportTypeFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>>(HisRequestUriStore.SARSAR_REPORT_TYPE_GET, ApiConsumers.SarConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlFormList.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlFormList.DataSource = null;
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
                gridviewFormList.EndUpdate();
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref SarReportTypeFilter filter)
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
                    var rowData = (SAR.EFMODEL.DataModels.SAR_REPORT_TYPE)gridviewFormList.GetFocusedRow();
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
                    SAR.EFMODEL.DataModels.SAR_REPORT_TYPE pData = (SAR.EFMODEL.DataModels.SAR_REPORT_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    else if (e.Column.FieldName == "REPORT_TYPE_GROUP_NAME" && pData.REPORT_TYPE_GROUP_ID.HasValue)
                    {
                        e.Value = (this.reportTypeGroups.FirstOrDefault(o => o.ID == pData.REPORT_TYPE_GROUP_ID) ?? new SAR_REPORT_TYPE_GROUP()).REPORT_TYPE_GROUP_NAME;
                    }
                    else if (e.Column.FieldName == "Importan")
                    {
                        try
                        {
                            if (pData.IS_IMPORTANCE.HasValue && pData.IS_IMPORTANCE == 1)
                                e.Value = true;
                            else
                                e.Value = false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "HOUR_FROM_STR")
                    {
                        try
                        {
                            if (!String.IsNullOrWhiteSpace(pData.HOUR_FROM))
                            {
                                DateTime h = new DateTime(1, 1, 1, int.Parse(pData.HOUR_FROM.Substring(0, 2)), int.Parse(pData.HOUR_FROM.Substring(2, 2)), 0);
                                e.Value = h.ToString("HH:mm tt");
                            }
                            else
                                e.Value = null;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "HOUR_TO_STR")
                    {
                        try
                        {
                            if (!String.IsNullOrWhiteSpace(pData.HOUR_TO))
                            {
                                DateTime h = new DateTime(1, 1, 1, int.Parse(pData.HOUR_TO.Substring(0, 2)), int.Parse(pData.HOUR_TO.Substring(2, 2)), 0);
                                e.Value = h.ToString("HH:mm tt");
                            }
                            else
                                e.Value = null;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
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
                var rowData = (SAR.EFMODEL.DataModels.SAR_REPORT_TYPE)gridviewFormList.GetFocusedRow();
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
                    var rowData = (SAR.EFMODEL.DataModels.SAR_REPORT_TYPE)gridviewFormList.GetFocusedRow();
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

        private void ChangedDataRow(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderTkb, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    ResetFormData();
                    if (data.REPORT_TYPE_CODE.Contains("TKB"))
                    {
                        xtraTabControl.SelectedTabPage = TabTkb;
                        TxtTkbCode.Text = data.REPORT_TYPE_CODE.Substring(3);
                        TxtTkbName.Text = data.REPORT_TYPE_NAME;
                        TxtTkbDescription.Text = data.DESCRIPTION;
                        CboTkbGroup.EditValue = data.REPORT_TYPE_GROUP_ID;
                        if (data.SQL != null)
                        {
                            TxtTkbSql.Text = System.Text.Encoding.UTF8.GetString(data.SQL);
                        }

                        TabNormal.PageEnabled = false;
                        ChkTkbImportan.Checked = data.IS_IMPORTANCE.HasValue && data.IS_IMPORTANCE.Value == (short)1;
                        CboTkbHourFrom.EditValue = data.HOUR_FROM;
                        CboTkbHourTo.EditValue = data.HOUR_TO;
                    }
                    else
                    {
                        xtraTabControl.SelectedTabPage = TabNormal;
                        txtCode.Text = data.REPORT_TYPE_CODE;
                        txtCode.Enabled = false;
                        txtName.Text = data.REPORT_TYPE_NAME;
                        txtMoTa.Text = data.DESCRIPTION;
                        cboReportTypeGroup.EditValue = data.REPORT_TYPE_GROUP_ID;
                        TabTkb.PageEnabled = false;
                        ChkImportan.Checked = data.IS_IMPORTANCE.HasValue && data.IS_IMPORTANCE.Value == (short)1;
                        CboHourFrom.EditValue = data.HOUR_FROM;
                        CboHourTo.EditValue = data.HOUR_TO;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                if (xtraTabControl.SelectedTabPage == TabNormal)
                {
                    txtCode.Focus();
                    txtCode.SelectAll();
                }
                else if (xtraTabControl.SelectedTabPage == TabTkb)
                {
                    TxtTkbCode.Focus();
                    TxtTkbCode.SelectAll();
                }
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
                txtCode.Text = "";
                txtMoTa.Text = "";
                txtName.Text = "";
                TxtTkbCode.Text = "";
                TxtTkbDescription.Text = "";
                TxtTkbName.Text = "";
                TxtTkbSql.Text = "";
                cboReportTypeGroup.EditValue = null;
                CboTkbGroup.EditValue = null;
                TabTkb.PageEnabled = true;
                TabNormal.PageEnabled = true;
                ChkImportan.Checked = false;
                ChkTkbImportan.Checked = false;
                txtCode.Enabled = true;
                TxtTkbCode.Enabled = true;
                CboHourFrom.EditValue = null;
                CboHourTo.EditValue = null;
                CboTkbHourFrom.EditValue = null;
                CboTkbHourTo.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref SAR.EFMODEL.DataModels.SAR_REPORT_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                SarReportTypeFilter filter = new SarReportTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>>(HisRequestUriStore.SARSAR_REPORT_TYPE_GET, ApiConsumers.SarConsumer, filter, param).FirstOrDefault();
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
                if (xtraTabControl.SelectedTabPage == TabTkb)
                {
                    TxtTkbCode.Focus();
                }
                else if (xtraTabControl.SelectedTabPage == TabNormal)
                {
                    txtCode.Focus();
                }

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderTkb, dxErrorProvider);
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
                var rowData = (SAR.EFMODEL.DataModels.SAR_REPORT_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SarReportTypeFilter filter = new SarReportTypeFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<SAR_REPORT_TYPE>>(HisRequestUriStore.SARSAR_REPORT_TYPE_GET, ApiConsumers.SarConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.SARSAR_REPORT_TYPE_DELETE, ApiConsumers.SarConsumer, data, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            //currentData = ((List<SAR_REPORT_TYPE>)gridControlFormList.DataSource).FirstOrDefault();
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
                btnAdd.Focus();
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;

                if (xtraTabControl.SelectedTabPage == TabNormal && !dxValidationProviderEditorInfo.Validate())
                    return;

                if (xtraTabControl.SelectedTabPage == TabTkb && !dxValidationProviderTkb.Validate())
                    return;

                if (xtraTabControl.SelectedTabPage == TabNormal && txtCode.Text.Trim().ToUpper().StartsWith("TKB"))
                {
                    if (this.currentData == null || this.currentData.ID == 0)
                    {
                        MessageBox.Show("Mã tự khai báo cần thiết lập Câu truy vấn. ");
                        xtraTabControl.SelectedTabPage = TabTkb;
                        TxtTkbCode.Text = txtCode.Text.Trim().ToUpper().Substring(3);
                        TxtTkbName.Text = txtName.Text;
                        CboTkbGroup.EditValue = cboReportTypeGroup.EditValue;
                        ChkTkbImportan.Checked = ChkImportan.Checked;
                        TxtTkbDescription.Text = txtMoTa.Text;
                        CboTkbHourFrom.EditValue = CboHourFrom.EditValue;
                        CboTkbHourTo.EditValue = CboHourTo.EditValue;

                        txtName.Text = null;
                        txtCode.Text = null;
                        cboReportTypeGroup.EditValue = null;
                        ChkImportan.Checked = false;
                        txtMoTa.Text = null;
                        CboHourFrom.EditValue = null;
                        CboHourTo.EditValue = null;

                        TxtTkbSql.Focus();
                        TxtTkbSql.SelectAll();
                        TabNormal.PageEnabled = false;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Không cho phép sửa sang Mã tự khai báo.");
                        return;
                    }
                }

                WaitingManager.Show();
                SAR.EFMODEL.DataModels.SAR_REPORT_TYPE updateDTO = new SAR.EFMODEL.DataModels.SAR_REPORT_TYPE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>(HisRequestUriStore.SARSAR_REPORT_TYPE_CREATE, ApiConsumers.SarConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>(HisRequestUriStore.SARSAR_REPORT_TYPE_UPDATE, ApiConsumers.SarConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
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

        private void UpdateRowDataAfterEdit(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE) is null");
                var rowData = (SAR.EFMODEL.DataModels.SAR_REPORT_TYPE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref SAR.EFMODEL.DataModels.SAR_REPORT_TYPE currentDTO)
        {
            try
            {
                if (xtraTabControl.SelectedTabPage == TabNormal)
                {
                    currentDTO.REPORT_TYPE_CODE = txtCode.Text.Trim();
                    currentDTO.REPORT_TYPE_NAME = txtName.Text.Trim();
                    currentDTO.DESCRIPTION = txtMoTa.Text.Trim();
                    if (cboReportTypeGroup.EditValue != null)
                    {
                        currentDTO.REPORT_TYPE_GROUP_ID = (long)cboReportTypeGroup.EditValue;
                    }

                    if (ChkImportan.Checked)
                    {
                        currentDTO.IS_IMPORTANCE = 1;
                    }
                    else
                    {
                        currentDTO.IS_IMPORTANCE = null;
                    }

                    if (CboHourFrom.EditValue != null)
                        currentDTO.HOUR_FROM = CboHourFrom.EditValue.ToString();
                    else
                        currentDTO.HOUR_FROM = null;

                    if (CboHourTo.EditValue != null)
                        currentDTO.HOUR_TO = CboHourTo.EditValue.ToString();
                    else
                        currentDTO.HOUR_TO = null;
                }
                else if (xtraTabControl.SelectedTabPage == TabTkb)
                {
                    currentDTO.REPORT_TYPE_CODE = TxtTkbCodePrefix.Text.Trim() + TxtTkbCode.Text.Trim();
                    currentDTO.REPORT_TYPE_NAME = TxtTkbName.Text.Trim();
                    currentDTO.DESCRIPTION = TxtTkbDescription.Text.Trim();
                    currentDTO.SQL = System.Text.Encoding.UTF8.GetBytes(TxtTkbSql.Text.Trim());
                    if (CboTkbGroup.EditValue != null)
                    {
                        currentDTO.REPORT_TYPE_GROUP_ID = (long)CboTkbGroup.EditValue;
                    }

                    if (ChkTkbImportan.Checked)
                    {
                        currentDTO.IS_IMPORTANCE = 1;
                    }
                    else
                    {
                        currentDTO.IS_IMPORTANCE = null;
                    }

                    if (CboTkbHourFrom.EditValue != null)
                        currentDTO.HOUR_FROM = CboTkbHourFrom.EditValue.ToString();
                    else
                        currentDTO.HOUR_FROM = null;

                    if (CboTkbHourTo.EditValue != null)
                        currentDTO.HOUR_TO = CboTkbHourTo.EditValue.ToString();
                    else
                        currentDTO.HOUR_TO = null;
                }
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
                ValidationMaxlength(dxValidationProviderEditorInfo, txtCode, 10);
                ValidationMaxlength(dxValidationProviderEditorInfo, txtName, 100);
                ValidationHourFromTo(dxValidationProviderEditorInfo, CboHourFrom, CboHourTo);

                ValidationMaxlength(dxValidationProviderTkb, TxtTkbCode, 10);
                ValidationMaxlength(dxValidationProviderTkb, TxtTkbName, 100);
                ValidationSingleControl(dxValidationProviderTkb, TxtTkbSql);
                ValidationHourFromTo(dxValidationProviderTkb, CboTkbHourFrom, CboTkbHourTo);
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

        private void ValidationSingleControl(DXValidationProvider dxValidation, BaseEdit control)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidation.SetValidationRule(control, validRule);
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
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
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
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxlength(DXValidationProvider dxValidation, BaseEdit control, int? maxLength)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validRule = new ControlMaxLengthValidationRule();
                validRule.maxLength = maxLength;
                validRule.IsRequired = true;
                validRule.editor = control;
                validRule.ErrorText = "Dữ liệu vượt quá độ dài cho phép";
                validRule.ErrorType = ErrorType.Warning;
                dxValidation.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationHourFromTo(DXValidationProvider dxValidation, GridLookUpEdit hourFrom, GridLookUpEdit hourTo)
        {
            try
            {
                Validation.ControlTwoHourValidationRule validRule = new Validation.ControlTwoHourValidationRule();
                validRule.HourFrom = hourFrom;
                validRule.HourTo = hourTo;
                validRule.ErrorText = "Thời gian từ đến không hợp lệ";
                validRule.ErrorType = ErrorType.Warning;
                dxValidation.SetValidationRule(hourFrom, validRule);
                dxValidation.SetValidationRule(hourTo, validRule);
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
                if (e.Info == null && e.SelectedControl == gridControlFormList)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlFormList.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            SAR_REPORT_TYPE dataRow = (SAR_REPORT_TYPE)((IList)((BaseView)gridviewFormList).DataSource)[info.RowHandle];
                            string text = "";
                            if (info.Column.FieldName == "Importan")
                                text = "Quan trọng";

                            lastInfo = new DevExpress.Utils.ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
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
        #endregion

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            SAR_REPORT_TYPE success = new SAR_REPORT_TYPE();
            //bool notHandler = false;
            try
            {
                SAR_REPORT_TYPE data = (SAR_REPORT_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SAR_REPORT_TYPE data1 = new SAR_REPORT_TYPE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR_REPORT_TYPE>(HisRequestUriStore.SARSAR_REPORT_TYPE_CHANGE_LOCK, ApiConsumers.SarConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControl();
                    }
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
            SAR_REPORT_TYPE success = new SAR_REPORT_TYPE();
            //bool notHandler = false;
            try
            {
                SAR_REPORT_TYPE data = (SAR_REPORT_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SAR_REPORT_TYPE data1 = new SAR_REPORT_TYPE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR_REPORT_TYPE>(HisRequestUriStore.SARSAR_REPORT_TYPE_CHANGE_LOCK, ApiConsumers.SarConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControl();
                    }
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
                    SAR_REPORT_TYPE data = (SAR_REPORT_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGunLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : repositoryItemButtonEdit1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                SAR_REPORT_TYPE data = (SAR_REPORT_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
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
                    cboReportTypeGroup.Focus();
                    cboReportTypeGroup.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMoTa_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else
                        btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboReportTypeGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ChkImportan.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TxtTkbCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtTkbName.Focus();
                    TxtTkbName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTkbName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboTkbGroup.Focus();
                    CboTkbGroup.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTkbGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ChkTkbImportan.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTkbDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.Enter)
                {
                    TxtTkbSql.Focus();
                    TxtTkbSql.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTkbGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboTkbGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReportTypeGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboReportTypeGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTkbSql_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab || (e.Control && e.KeyCode == Keys.Enter))
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else
                        btnEdit.Focus();

                    e.IsInputKey = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMoTa_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab || (e.Control && e.KeyCode == Keys.Enter))
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else
                        btnEdit.Focus();

                    e.IsInputKey = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Res_CheckEdit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var chk = sender as CheckEdit;
                var row = (SAR_REPORT_TYPE)gridviewFormList.GetFocusedRow();
                if (row != null && chk != null)
                {
                    if (chk.Checked)
                        row.IS_IMPORTANCE = 1;
                    else
                        row.IS_IMPORTANCE = null;

                    CommonParam param = new CommonParam();
                    bool success = false;

                    var resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>(HisRequestUriStore.SARSAR_REPORT_TYPE_UPDATE, ApiConsumers.SarConsumer, row, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }

                    WaitingManager.Hide();

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkImportan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboHourFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkTkbImportan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboTkbHourFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTkbCode_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void TxtTkbCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTkbCode_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(TxtTkbCode.Text.Trim()))
                {
                    string code = TxtTkbCode.Text.Trim();
                    code = string.Format("{0:00000}", Convert.ToInt64(code));
                    TxtTkbCode.Text = code;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboHourFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboHourTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboHourTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMoTa.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTkbHourFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboTkbHourTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTkbHourTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtTkbDescription.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTkbHourFrom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboTkbHourFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTkbHourTo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboTkbHourTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboHourFrom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboHourFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboHourTo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboHourTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboStatusHour()
        {
            try
            {
                //Load dộng thời gian từ hàm cs hỗ trợ
                //dùng cú phspd truy vấn linq để khởi  tạo dữ liệu
                List<StatusHour> status = new List<StatusHour>();
                status.Add(new StatusHour("0000", "12:00 AM")); status.Add(new StatusHour("0015", "12:15 AM"));
                status.Add(new StatusHour("0030", "12:30 AM")); status.Add(new StatusHour("0045", "12:45 AM"));
                status.Add(new StatusHour("0100", "1:00 AM")); status.Add(new StatusHour("0115", "1:15 AM"));
                status.Add(new StatusHour("0130", "1:30 AM")); status.Add(new StatusHour("0145", "1:45 AM"));
                status.Add(new StatusHour("0200", "2:00 AM")); status.Add(new StatusHour("0215", "2:15 AM"));
                status.Add(new StatusHour("0230", "2:30 AM")); status.Add(new StatusHour("0245", "2:45 AM"));
                status.Add(new StatusHour("0300", "3:00 AM")); status.Add(new StatusHour("0315", "3:15 AM"));
                status.Add(new StatusHour("0330", "3:30 AM")); status.Add(new StatusHour("0345", "3:45 AM"));
                status.Add(new StatusHour("0400", "4:00 AM")); status.Add(new StatusHour("0415", "4:15 AM"));
                status.Add(new StatusHour("0430", "4:30 AM")); status.Add(new StatusHour("0445", "4:45 AM"));
                status.Add(new StatusHour("0500", "5:00 AM")); status.Add(new StatusHour("0515", "5:15 AM"));
                status.Add(new StatusHour("0530", "5:30 AM")); status.Add(new StatusHour("0545", "5:45 AM"));
                status.Add(new StatusHour("0600", "6:00 AM")); status.Add(new StatusHour("0615", "6:15 AM"));
                status.Add(new StatusHour("0630", "6:30 AM")); status.Add(new StatusHour("0645", "6:45 AM"));
                status.Add(new StatusHour("0700", "7:00 AM")); status.Add(new StatusHour("0715", "7:15 AM"));
                status.Add(new StatusHour("0730", "7:30 AM")); status.Add(new StatusHour("0745", "7:45 AM"));
                status.Add(new StatusHour("0800", "8:00 AM")); status.Add(new StatusHour("0815", "8:15 AM"));
                status.Add(new StatusHour("0830", "8:30 AM")); status.Add(new StatusHour("0845", "8:45 AM"));
                status.Add(new StatusHour("0900", "9:00 AM")); status.Add(new StatusHour("0915", "9:15 AM"));
                status.Add(new StatusHour("0930", "9:30 AM")); status.Add(new StatusHour("0945", "9:45 AM"));
                status.Add(new StatusHour("1000", "10:00 AM")); status.Add(new StatusHour("1015", "10:15 AM"));
                status.Add(new StatusHour("1030", "10:30 AM")); status.Add(new StatusHour("1045", "10:45 AM"));
                status.Add(new StatusHour("1100", "11:00 AM")); status.Add(new StatusHour("1115", "11:15 AM"));
                status.Add(new StatusHour("1130", "11:30 AM")); status.Add(new StatusHour("1145", "11:45 AM"));
                status.Add(new StatusHour("1200", "12:00 PM")); status.Add(new StatusHour("1215", "12:15 PM"));
                status.Add(new StatusHour("1230", "12:30 PM")); status.Add(new StatusHour("1245", "12:45 PM"));
                status.Add(new StatusHour("1300", "1:00 PM")); status.Add(new StatusHour("1315", "1:15 PM"));
                status.Add(new StatusHour("1330", "1:30 PM")); status.Add(new StatusHour("1345", "1:45 PM"));
                status.Add(new StatusHour("1400", "2:00 PM")); status.Add(new StatusHour("1415", "2:15 PM"));
                status.Add(new StatusHour("1430", "2:30 PM")); status.Add(new StatusHour("1445", "2:45 PM"));
                status.Add(new StatusHour("1500", "3:00 PM")); status.Add(new StatusHour("1515", "3:15 PM"));
                status.Add(new StatusHour("1530", "3:30 PM")); status.Add(new StatusHour("1545", "3:45 PM"));
                status.Add(new StatusHour("1600", "4:00 PM")); status.Add(new StatusHour("1615", "4:15 PM"));
                status.Add(new StatusHour("1630", "4:30 PM")); status.Add(new StatusHour("1645", "4:45 PM"));
                status.Add(new StatusHour("1700", "5:00 PM")); status.Add(new StatusHour("1715", "5:15 PM"));
                status.Add(new StatusHour("1730", "5:30 PM")); status.Add(new StatusHour("1745", "5:45 PM"));
                status.Add(new StatusHour("1800", "6:00 PM")); status.Add(new StatusHour("1815", "6:15 PM"));
                status.Add(new StatusHour("1830", "6:30 PM")); status.Add(new StatusHour("1845", "6:45 PM"));
                status.Add(new StatusHour("1900", "7:00 PM")); status.Add(new StatusHour("1915", "7:15 PM"));
                status.Add(new StatusHour("1930", "7:30 PM")); status.Add(new StatusHour("1945", "7:45 PM"));
                status.Add(new StatusHour("2000", "8:00 PM")); status.Add(new StatusHour("2015", "8:15 PM"));
                status.Add(new StatusHour("2030", "8:30 PM")); status.Add(new StatusHour("2045", "8:45 PM"));
                status.Add(new StatusHour("2100", "9:00 PM")); status.Add(new StatusHour("2115", "9:15 PM"));
                status.Add(new StatusHour("2130", "9:30 PM")); status.Add(new StatusHour("2145", "9:45 PM"));
                status.Add(new StatusHour("2200", "10:00 PM")); status.Add(new StatusHour("2215", "10:15 PM"));
                status.Add(new StatusHour("2230", "10:30 PM")); status.Add(new StatusHour("2245", "10:45 PM"));
                status.Add(new StatusHour("2300", "11:00 PM")); status.Add(new StatusHour("2315", "11:15 PM"));
                status.Add(new StatusHour("2330", "11:30 PM")); status.Add(new StatusHour("2345", "11:45 PM"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "statusCode", columnInfos, false, 350);
                ControlEditorLoader.Load(CboHourFrom, status, controlEditorADO);
                ControlEditorLoader.Load(CboHourTo, status, controlEditorADO);
                ControlEditorLoader.Load(CboTkbHourFrom, status, controlEditorADO);
                ControlEditorLoader.Load(CboTkbHourTo, status, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
