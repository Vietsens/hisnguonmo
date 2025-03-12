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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using SAR.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.Utilities;
using HIS.Desktop.Utilities.Extensions;
using SAR.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using System.Text;
using SAR.Desktop.Plugins.SarPrintType.ADO;
using System.Web.Script.Serialization;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraEditors.Controls;

namespace SAR.Desktop.Plugins.SarPrintType
{
    public partial class frmSarPrintType : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        SAR.EFMODEL.DataModels.SAR_PRINT_TYPE currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<EMR_BUSINESS> icd;
        List<EMR_BUSINESS> icdSeleteds;
        string[] icdNew;
        #endregion

        #region Construct
        public frmSarPrintType(Inventec.Desktop.Common.Modules.Module moduleData)
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
        #endregion

        #region Private method
        private void frmSarPrintType_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("SAR.Desktop.Plugins.SarPrintType.Resources.Lang", typeof(SAR.Desktop.Plugins.SarPrintType.frmSarPrintType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.chkDoNotAllowReprint.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.chkDoNotAllowReprint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDoNotAllowPrint.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.chkDoNotAllowPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDigitalSign.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.chkDigitalSign.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsSingleCopy.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.chkIsSingleCopy.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkky.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.chkky.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKhongGopNut.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.chkKhongGopNut.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHistory.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.chkHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmSarPrintType.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColCode.ToolTip = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColName.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColName.ToolTip = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmSarPrintType.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSarPrintType.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBedTypeCode.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.lciBedTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.barManager1.= Inventec.Common.Resource.Get.Value("frmSarPrintType.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmSarPrintType.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSarPrintType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                ResetFormData();
                EnableControlChanged(this.ActionType);

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
                dicOrderTabIndexControl.Add("txtProgramCode", 0);
                dicOrderTabIndexControl.Add("txtProgramName", 1);
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
                string isUseSignEmr = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.HIS.DESKTOP.IS_USE_SIGN_EMR");
                if (isUseSignEmr == "1")
                {
                    layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    //Check EMR
                    var data = BackendDataWorker.Get<EMR_BUSINESS>();
                    if (data != null && data.Count > 0)
                    {
                        InitCheck(CboICD, SelectionGrid__BUSINESS_NAME);
                        var dataBuss = data.Any(o => o.IS_ACTIVE == 1) ? data.Where(o => o.IS_ACTIVE == 1).ToList() : null;
                        InitCombo(CboICD, dataBuss, "BUSINESS_NAME", "ID");
                    }

                    //Fill data into datasource combo docuemnt type
                    InitComboDocumentType();
                    InitEmrDocumentGroup();

                }
                else
                {
                    layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboDocumentType()
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = BackendDataWorker.Get<EMR_DOCUMENT_TYPE>();
                var dataBuss = data.Any(o => o.IS_ACTIVE == 1) ? data.Where(o => o.IS_ACTIVE == 1).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_TYPE_NAME", "DOCUMENT_TYPE_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboEmrDocumentType, dataBuss, controlEditorADO);
                cboEmrDocumentType.Properties.ImmediatePopup = true;
                cboEmrDocumentType.Properties.AllowNullInput = DefaultBoolean.True;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitEmrDocumentGroup()
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = BackendDataWorker.Get<EMR_DOCUMENT_GROUP>();
                var dataBuss = data.Any(o => o.IS_ACTIVE == 1) ? data.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_LEAF == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_GROUP_NAME", "DOCUMENT_GROUP_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboEmrDocumentGroup, dataBuss, controlEditorADO);
                cboEmrDocumentGroup.Properties.ImmediatePopup = true;
                cboEmrDocumentGroup.Properties.AllowNullInput = DefaultBoolean.True;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

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
                Inventec.Core.ApiResultObject<List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>> apiResult = null;
                SarPrintTypeFilter filter = new SarPrintTypeFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>>(HisRequestUriStore.SARSAR_PRINT_TYPE_GET, ApiConsumers.SarConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                if (apiResult != null)
                {
                    var data = (List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
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

        private void SetFilterNavBar(ref SarPrintTypeFilter filter)
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
                    var rowData = (SAR.EFMODEL.DataModels.SAR_PRINT_TYPE)gridviewFormList.GetFocusedRow();
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
                    SAR.EFMODEL.DataModels.SAR_PRINT_TYPE pData = (SAR.EFMODEL.DataModels.SAR_PRINT_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    else if (e.Column.FieldName == "Tudong")
                    {
                        try
                        {
                            e.Value = pData.IS_AUTO_CHOOSE_BUSINESS == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "Khongchophepinlai")
                    {
                        try
                        {
                            e.Value = pData.DO_NOT_ALLOW_REPRINT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "Kydientu")
                    {
                        try
                        {
                            e.Value = pData.IS_DIGITAL_SIGN == 1 ? true : false;
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
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.CREATE_TIME);
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

                    if (e.Column.FieldName == "BatBuocNhapLyDoKhiInLai")
                    {
                         e.Value = pData.IS_PRINT_EXCEPTION_REASON  == 1 ? true : false;
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
                var rowData = (SAR.EFMODEL.DataModels.SAR_PRINT_TYPE)gridviewFormList.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowData), rowData));
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                    SetValueICD(this.CboICD, this.icdSeleteds, BackendDataWorker.Get<EMR_BUSINESS>().Where(o => o.IS_ACTIVE == 1).ToList());

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
                    var rowData = (SAR.EFMODEL.DataModels.SAR_PRINT_TYPE)gridviewFormList.GetFocusedRow();
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

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (SAR.EFMODEL.DataModels.SAR_PRINT_TYPE)(gridControlFormList.DataSource as List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>)[dnNavigation.Position];
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

        private void ChangedDataRow(SAR.EFMODEL.DataModels.SAR_PRINT_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

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

        private void FillDataToEditorControl(SAR.EFMODEL.DataModels.SAR_PRINT_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    txtCode.Text = data.PRINT_TYPE_CODE;
                    txtName.Text = data.PRINT_TYPE_NAME;
                    chkKhongGopNut.Checked = data.IS_NO_GROUP == 1 ? true : false;
                    chkky.Checked = data.IS_AUTO_CHOOSE_BUSINESS == 1 ? true : false;
                    chkDigitalSign.Checked = data.IS_DIGITAL_SIGN == 1 ? true : false;
                    chkIsSingleCopy.Checked = data.IS_SINGLE_COPY == 1 ? true : false;
                    this.chkHistory.Checked = data.IS_PRINT_LOG == 1 ? true : false;
                    this.chkHistory.Enabled = data.PRINT_LOG_ENABLE == 1 ? true : false;
                    txtFileMau.Text = data.FILE_PATTERN;
                    cboEmrDocumentType.EditValue = data.EMR_DOCUMENT_TYPE_CODE;
                    cboEmrDocumentGroup.EditValue = data.EMR_DOCUMENT_GROUP_CODE;
                    txtReprintExceptionAccount.EditValue = data.REPRINT_EXCEPTION_LOGINNAME;
                    txtDoNotAllowPrintAccount.EditValue = data.PRINT_EXCEPTION_LOGINNAME;
                    chkDoNotAllowReprint.Checked = data.DO_NOT_ALLOW_REPRINT == 1 ? true : false;
                    chkDoNotAllowPrint.Checked = data.DO_NOT_ALLOW_PRINT == 1 ? true : false;
                    chkIS_PRINT_EXCEPTION_REASON.Checked = data.IS_PRINT_EXCEPTION_REASON == 1 ? true : false;
                    filldatatocboICD(data);
                    memoMappingEMR.Text = data.EMR_COLUMN_MAPPING;
                    memoDisablePrintByKeyCFG.Text = data.DISABLE_PRINT_BY_KEY_CFG;
                    memoGenSignatureEnable.Text = data.GEN_SIGNATURE_BY_KEY_CFG;
                    memoNumCopyByKeyCFG.Text = data.NUM_COPY_BY_KEY_CFG;
                    memoSetSignature.Text = data.GEN_SIGNER_BY_KEY_CFG;
                    //btnPopUpGenSignatureEnable.Enabled = data.GEN_SIGNATURE_ENABLE == 1 ? true : false;


                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.GEN_SIGNATURE_ENABLE), data.GEN_SIGNATURE_ENABLE));
                    if (data.GEN_SIGNATURE_ENABLE != null)
                    {
                        if (data.GEN_SIGNATURE_ENABLE == 1)
                        {
                            btnPopUpGenSignatureEnable.Enabled = true;
                        }
                        else
                        {
                            btnPopUpGenSignatureEnable.Enabled = false;
                        }
                    }
                    else
                    {
                        btnPopUpGenSignatureEnable.Enabled = false;
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

        private void
            ResetFormData()
        {
            try
            {
                icdSeleteds = new List<EMR_BUSINESS>();
                CboICD.Text = "";
                SetValueICD(this.CboICD, this.icdSeleteds, BackendDataWorker.Get<EMR_BUSINESS>());

                txtKeyword.Text = "";
                txtCode.Text = "";
                txtName.Text = "";
                txtFileMau.Text = "";
                cboEmrDocumentType.EditValue = null;
                cboEmrDocumentGroup.EditValue = null;
                chkKhongGopNut.Checked = false;
                chkHistory.Checked = false;
                chkHistory.Enabled = true;
                chkky.Checked = false;
                chkDigitalSign.Checked = false;
                chkIsSingleCopy.Checked = false;
                chkDoNotAllowReprint.Enabled = false;
                chkDoNotAllowReprint.Checked = false;
                chkDoNotAllowPrint.Enabled = false;
                chkDoNotAllowPrint.Checked = false;
                txtReprintExceptionAccount.Enabled = false;
                txtReprintExceptionAccount.Text = "";
                txtDoNotAllowPrintAccount.Enabled = false;
                txtDoNotAllowPrintAccount.Text = "";
                memoDisablePrintByKeyCFG.Text = "";
                chkIS_PRINT_EXCEPTION_REASON.Checked = false;
                memoMappingEMR.Text = "";
                memoGenSignatureEnable.Text = "";
                memoNumCopyByKeyCFG.Text = "";
                memoSetSignature.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref SAR.EFMODEL.DataModels.SAR_PRINT_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                SarPrintTypeFilter filter = new SarPrintTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>>(HisRequestUriStore.SARSAR_PRINT_TYPE_GET, ApiConsumers.SarConsumer, filter, param).FirstOrDefault();
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
                var rowData = (SAR.EFMODEL.DataModels.SAR_PRINT_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SarPrintTypeFilter filter = new SarPrintTypeFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<SAR_PRINT_TYPE>>(HisRequestUriStore.SARSAR_PRINT_TYPE_GET, ApiConsumers.SarConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.SARSAR_PRINT_TYPE_DELETE, ApiConsumers.SarConsumer, data, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            currentData = ((List<SAR_PRINT_TYPE>)gridControlFormList.DataSource).FirstOrDefault();
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
                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE updateDTO = new SAR.EFMODEL.DataModels.SAR_PRINT_TYPE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>(HisRequestUriStore.SARSAR_PRINT_TYPE_CREATE, ApiConsumers.SarConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>(HisRequestUriStore.SARSAR_PRINT_TYPE_UPDATE, ApiConsumers.SarConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();

                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                    MPS.ProcessorBase.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
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

        private void UpdateDTOFromDataForm(ref SAR.EFMODEL.DataModels.SAR_PRINT_TYPE currentDTO)
        {
            try
            {
                currentDTO.PRINT_TYPE_CODE = txtCode.Text.Trim();
                currentDTO.PRINT_TYPE_NAME = txtName.Text.Trim();
                currentDTO.FILE_PATTERN = txtFileMau.Text.Trim();
                currentDTO.IS_NO_GROUP = chkKhongGopNut.Checked ? (short?)1 : null;
                currentDTO.IS_AUTO_CHOOSE_BUSINESS = chkky.Checked ? (short?)1 : (short)0;
                currentDTO.IS_DIGITAL_SIGN = chkDigitalSign.Checked ? (short?)1 : (short)0;
                currentDTO.IS_SINGLE_COPY = chkIsSingleCopy.Checked ? (short?)1 : (short)0;
                currentDTO.IS_PRINT_LOG = chkHistory.Checked ? (short?)1 : null;
                currentDTO.IS_PRINT_EXCEPTION_REASON = chkIS_PRINT_EXCEPTION_REASON.Checked ? (short?)1 : null;
                if (chkHistory.Checked)
                {
                    currentDTO.IS_PRINT_LOG = 1;
                    if (chkDoNotAllowReprint.Checked)
                    {
                        currentDTO.DO_NOT_ALLOW_REPRINT = 1;
                        if (!string.IsNullOrEmpty(txtReprintExceptionAccount.Text.Trim()))
                        {
                            currentDTO.REPRINT_EXCEPTION_LOGINNAME = txtReprintExceptionAccount.Text.Trim();
                        }
                        else
                        {
                            currentDTO.REPRINT_EXCEPTION_LOGINNAME = "";
                        }
                    }
                    else
                    {
                        currentDTO.DO_NOT_ALLOW_REPRINT = null;
                        currentDTO.REPRINT_EXCEPTION_LOGINNAME = "";
                    }

                    if (chkDoNotAllowPrint.Checked)
                    {
                        currentDTO.DO_NOT_ALLOW_PRINT = 1;
                        if (!string.IsNullOrEmpty(txtDoNotAllowPrintAccount.Text.Trim()))
                        {
                            currentDTO.PRINT_EXCEPTION_LOGINNAME = txtDoNotAllowPrintAccount.Text.Trim();
                        }
                        else
                        {
                            currentDTO.PRINT_EXCEPTION_LOGINNAME = "";
                        }
                    }
                    else
                    {
                        currentDTO.DO_NOT_ALLOW_PRINT = null;
                        currentDTO.PRINT_EXCEPTION_LOGINNAME = "";
                    }

                }
                else
                {
                    currentDTO.IS_PRINT_LOG = null;
                    currentDTO.DO_NOT_ALLOW_REPRINT = null;
                    currentDTO.REPRINT_EXCEPTION_LOGINNAME = "";
                    currentDTO.DO_NOT_ALLOW_PRINT = null;
                    currentDTO.PRINT_EXCEPTION_LOGINNAME = "";
                }

                List<string> ICDds = icdSeleteds.Select(o => o.BUSINESS_CODE).ToList();
                currentDTO.BUSINESS_CODES = string.Join(";", ICDds);

                if (cboEmrDocumentType.EditValue != null)
                    currentDTO.EMR_DOCUMENT_TYPE_CODE = cboEmrDocumentType.EditValue.ToString();
                else
                    currentDTO.EMR_DOCUMENT_TYPE_CODE = null;
                if (cboEmrDocumentGroup.EditValue != null)
                    currentDTO.EMR_DOCUMENT_GROUP_CODE = cboEmrDocumentGroup.EditValue.ToString();
                else
                    currentDTO.EMR_DOCUMENT_GROUP_CODE = null;
                if (CboICD.EditValue != null)
                    currentDTO.BUSINESS_CODES = CboICD.EditValue.ToString();
                currentDTO.EMR_COLUMN_MAPPING = memoMappingEMR.Text;
                currentDTO.DISABLE_PRINT_BY_KEY_CFG = memoDisablePrintByKeyCFG.Text;
                currentDTO.GEN_SIGNATURE_BY_KEY_CFG = memoGenSignatureEnable.Text;
                currentDTO.NUM_COPY_BY_KEY_CFG = memoNumCopyByKeyCFG.Text;
                currentDTO.GEN_SIGNER_BY_KEY_CFG = memoSetSignature.Text;
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
                ValidationSingleControl(txtCode);
                ValidationSingleControl(txtName);
                ValidationSingleControl(txtFileMau);
                ValidationMaxlength(txtDoNotAllowPrintAccount, 4000);
                ValidationMaxlength(txtReprintExceptionAccount, 4000);
                ValidationMaxlength(memoMappingEMR, 4000);
                ValidationMaxlength(memoDisablePrintByKeyCFG, 4000);
                ValidationMaxlength(memoGenSignatureEnable, 4000);
                ValidationMaxlength(memoNumCopyByKeyCFG, 4000);
                ValidationMaxlength(memoSetSignature, 4000);
                //ValidationSingleControl1();

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
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxlength(BaseEdit control, int? maxLength)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validRule = new ControlMaxLengthValidationRule();
                validRule.maxLength = maxLength;
                validRule.editor = control;
                validRule.ErrorText = "Dữ liệu vượt quá độ dài cho phép";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
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
                //Gan gia tri mac dinh
                SetDefaultValue();
                //Load ngon ngu label control
                SetCaptionByLanguageKey();
                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Load du lieu
                FillDataToGridControl();

                FillDataToControlsForm();


                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();

                // Default control
                chkIS_PRINT_EXCEPTION_REASON.Enabled = false;
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
            SAR_PRINT_TYPE success = new SAR_PRINT_TYPE();
            //bool notHandler = false;
            try
            {

                SAR_PRINT_TYPE data = (SAR_PRINT_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SAR_PRINT_TYPE data1 = new SAR_PRINT_TYPE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR_PRINT_TYPE>(HisRequestUriStore.SARSAR_PRINT_TYPE_CHANGE_LOCK, ApiConsumers.SarConsumer, data1, param);
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
            SAR_PRINT_TYPE success = new SAR_PRINT_TYPE();
            //bool notHandler = false;
            try
            {

                SAR_PRINT_TYPE data = (SAR_PRINT_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SAR_PRINT_TYPE data1 = new SAR_PRINT_TYPE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR_PRINT_TYPE>(HisRequestUriStore.SARSAR_PRINT_TYPE_CHANGE_LOCK, ApiConsumers.SarConsumer, data1, param);
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
                    SAR_PRINT_TYPE data = (SAR_PRINT_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGunLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : repositoryItemButtonEdit1);

                    }
                    if (e.Column.FieldName == "Tudong")
                    {
                        e.RepositoryItem = chktudong;
                    }
                    if (e.Column.FieldName == "KhongGopNut")
                    {
                        if (data.IS_NO_GROUP == 1)
                            e.RepositoryItem = Btn_KhongGopNut_Enable;
                    }
                    if (e.Column.FieldName == "Kydientu")
                    {
                        e.RepositoryItem = chkkydientu;
                    }
                    if (e.Column.FieldName == "IS_PRINT_LOG")
                    {
                        if (data.IS_PRINT_LOG == 1)
                            e.RepositoryItem = Btn_CoLuuLichSuIn_Enable;
                    }
                    if (e.Column.FieldName == "Khongchophepinlai")
                    {
                        e.RepositoryItem = chkkhongchophepinlai;
                    }
                    if (e.Column.FieldName == "BatBuocNhapLyDoKhiInLai")
                    {
                        e.RepositoryItem = repositoryItemCheckLyDoInLai;
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
                SAR_PRINT_TYPE data = (SAR_PRINT_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void txtName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtFileMau.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFileMau_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboEmrDocumentType.Focus();
                    cboEmrDocumentType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHistory_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkDoNotAllowReprint.Enabled)
                    {
                        chkIS_PRINT_EXCEPTION_REASON.Focus();
                        chkIS_PRINT_EXCEPTION_REASON.SelectAll();
                    }
                    else if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else btnEdit.Focus();
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

        private void cboEmrDocumentType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboEmrDocumentGroup.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = " Tất cả ";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);

                    ////
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void CboICD_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (EMR_BUSINESS rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0)
                    {
                        sb.Append(" ; ");
                    }
                    sb.Append(rv.BUSINESS_NAME.ToString());

                }
                e.DisplayText = sb.ToString();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void SelectionGrid__BUSINESS_NAME(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<EMR_BUSINESS> sgSelectedNews = new List<EMR_BUSINESS>();
                    foreach (EMR.EFMODEL.DataModels.EMR_BUSINESS rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0)
                            {
                                sb.Append(";");
                            }
                            sb.Append(rv.BUSINESS_NAME.ToString());
                            sgSelectedNews.Add(rv);

                        }

                    }
                    this.icdSeleteds = new List<EMR_BUSINESS>();
                    this.icdSeleteds.AddRange(sgSelectedNews);

                }
                this.CboICD.Text = sb.ToString();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void SetValueICD(GridLookUpEdit grdLookUpEdit, List<EMR_BUSINESS> listSelect, List<EMR_BUSINESS> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    //EmrBusinessFilter filter = new EmrBusinessFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;


                    grdLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.BUSINESS_CODE == p.BUSINESS_CODE)).ToList();
                    GridCheckMarksSelection gridCheckMark = grdLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.Selection.Clear();
                        gridCheckMark.Selection.AddRange(selectFilter);
                    }

                }
                grdLookUpEdit.Text = null;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void filldatatocboICD(SAR_PRINT_TYPE data)
        {
            try
            {
                if (data.BUSINESS_CODES != null)
                {

                    icdSeleteds = new List<EMR_BUSINESS>();
                    CboICD.Text = "";
                    SetValueICD(this.CboICD, this.icdSeleteds, BackendDataWorker.Get<EMR.EFMODEL.DataModels.EMR_BUSINESS>().Where(o => o.IS_ACTIVE == 1).ToList());
                    string icdstring = data.BUSINESS_CODES;
                    icdNew = icdstring.Split(';');
                    if (icdNew.Count() == 1)
                    {
                        icdSeleteds = BackendDataWorker.Get<EMR_BUSINESS>().Where(o => o.BUSINESS_CODE == (data.BUSINESS_CODES)).ToList();
                        CboICD.Text = BackendDataWorker.Get<EMR_BUSINESS>().FirstOrDefault(o => o.BUSINESS_CODE == data.BUSINESS_CODES).BUSINESS_NAME;
                    }
                    else
                    {
                        string cboIcdText = "";
                        for (int i = 0; i < icdNew.Count(); i++)
                        {
                            //int m = int.Parse(icdNew[i]);
                            string m = (icdNew[i]);
                            List<EMR_BUSINESS> ICDLoad = new List<EMR_BUSINESS>();
                            ICDLoad = BackendDataWorker.Get<EMR_BUSINESS>().Where(o => o.BUSINESS_CODE == m).ToList();
                            if (cboIcdText.Length > 0)
                                cboIcdText = cboIcdText + ";" + BackendDataWorker.Get<EMR_BUSINESS>().FirstOrDefault(o => o.BUSINESS_CODE == (data.BUSINESS_CODES)).BUSINESS_NAME;
                            foreach (EMR_BUSINESS a in ICDLoad)
                            {
                                icdSeleteds.Add(a);
                            }
                        }

                        CboICD.Text = cboIcdText;
                    }


                }
                else
                {
                    icdSeleteds = new List<EMR_BUSINESS>();
                    CboICD.Text = "";
                    SetValueICD(this.CboICD, this.icdSeleteds, BackendDataWorker.Get<EMR.EFMODEL.DataModels.EMR_BUSINESS>());

                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void CboICD_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    EMR.EFMODEL.DataModels.EMR_BUSINESS gt = this.icd.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(CboICD.EditValue.ToString()));
                    if (gt != null)
                    {
                        chkKhongGopNut.Focus();
                    }
                }
                else chkKhongGopNut.Focus();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

        }

        private void chkDoNotAllowReprint_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkDigitalSign.Focus();
                    chkDigitalSign.SelectAll();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkHistory_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (chkHistory.CheckState == CheckState.Checked)
                {
                    chkDoNotAllowReprint.Enabled = true;
                    chkDoNotAllowPrint.Enabled = true;
                    chkIS_PRINT_EXCEPTION_REASON.Enabled = true;

                }
                else
                {
                    chkDoNotAllowReprint.Enabled = false;
                    chkDoNotAllowReprint.Checked = false;
                    chkDoNotAllowPrint.Enabled = false;
                    chkDoNotAllowPrint.Checked = false;
                    chkIS_PRINT_EXCEPTION_REASON.Enabled = false;
                    chkIS_PRINT_EXCEPTION_REASON.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkDoNotAllowReprint_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDoNotAllowReprint.Checked)
            {
                txtReprintExceptionAccount.Enabled = true;
                chkDoNotAllowReprint.Enabled = true;
            }
            else
            {
                txtReprintExceptionAccount.Enabled = false;
                txtReprintExceptionAccount.EditValue = "";
            }
        }

        private void txtAccount_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1 && chkDoNotAllowReprint.Checked)
                {
                    WaitingManager.Show();
                    //CallModule callModule = new CallModule(CallModule.SecondaryIcd, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    frmAcsUser frm = new frmAcsUser(GetReprintLoginNames, txtReprintExceptionAccount.Text);
                    frm.ShowDialog();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetReprintLoginNames(string delegateLoginNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateLoginNames))
                {
                    txtReprintExceptionAccount.Text = delegateLoginNames;
                }
                else
                {
                    txtReprintExceptionAccount.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDoNotAllowLoginNames(string delegateLoginNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateLoginNames))
                {
                    txtDoNotAllowPrintAccount.Text = delegateLoginNames;
                }
                else
                {
                    txtDoNotAllowPrintAccount.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkDigitalSign_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSingleCopy.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsSingleCopy_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHistory.Focus();
                    chkHistory.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDoNotAllowPrint_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDoNotAllowPrint.Checked)
            {
                txtDoNotAllowPrintAccount.Enabled = true;
                chkDoNotAllowPrint.Enabled = true;
            }
            else
            {
                txtDoNotAllowPrintAccount.Enabled = false;
                txtDoNotAllowPrintAccount.EditValue = "";
            }
        }

        private void txtDoNotAllowPrintAccount_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1 && chkDoNotAllowPrint.Checked)
                {
                    WaitingManager.Show();
                    //CallModule callModule = new CallModule(CallModule.SecondaryIcd, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    frmAcsUser frm = new frmAcsUser(GetDoNotAllowLoginNames, txtDoNotAllowPrintAccount.Text);
                    frm.ShowDialog();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region các memo json
        ////các hàm của memo MappingEMR
        private void btnPopupMappingEMR_Click(object sender, EventArgs e)
        {
            try
            {
                //var rowData = (SAR.EFMODEL.DataModels.SAR_PRINT_TYPE)gridviewFormList.GetFocusedRow();
                //if (rowData != null)
                //{

                //if (!String.IsNullOrWhiteSpace(rowData.EMR_COLUMN_MAPPING))
                if (!String.IsNullOrEmpty(memoMappingEMR.Text))
                {

                    //List<ColumnMappingADO> ados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ColumnMappingADO>>(rowData.EMR_COLUMN_MAPPING);
                    List<ColumnMappingADO> ados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ColumnMappingADO>>(memoMappingEMR.Text);

                    if (ados == null || ados.Count == 0)
                    {
                        ColumnMappingADO mapping = new ColumnMappingADO();
                        mapping.Edit = 0;
                        ados.Add(mapping);
                    }
                    else if (ados.Count == 1)
                    {
                        foreach (var item in ados)
                        {
                            item.Edit = 0;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ados.Count; i++)
                        {
                            if (i == 0)
                            {
                                ados[0].Edit = 0;
                            }
                            else
                            {
                                ados[i].Edit = 1;
                            }
                        }
                    }
                    gridControlMappingEMR.DataSource = ados;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ados), ados));
                }
                else
                {
                    List<ColumnMappingADO> ados = new List<ColumnMappingADO>();

                    ColumnMappingADO mapping = new ColumnMappingADO();
                    mapping.Edit = 0;
                    ados.Add(mapping);
                    gridControlMappingEMR.DataSource = ados;
                }
                //}
                Rectangle buttonPosition = new Rectangle(btnPopupMappingEMR.Bounds.X, btnPopupMappingEMR.Bounds.Y, btnPopupMappingEMR.Bounds.Width, btnPopupMappingEMR.Bounds.Height);
                popupControlContainerMappingEMR.ShowPopup(new Point(buttonPosition.X + 480, buttonPosition.Bottom + btnPopupMappingEMR.Bounds.Height - 10));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMappingEMR_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnAddAndDelete")
                {
                    ColumnMappingADO data = (ColumnMappingADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    //int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewMappingEMR.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (data.Edit == 0)
                    {
                        e.RepositoryItem = repositoryBtnAdd;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryBtnDelete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryBtnAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                List<ColumnMappingADO> mappingEmrAdoTemps = new List<ColumnMappingADO>();
                var mappingEmrs = gridControlMappingEMR.DataSource as List<ColumnMappingADO>;
                ColumnMappingADO mappingEmrAdoTemp = new ColumnMappingADO();
                mappingEmrAdoTemp.Edit = 1;
                mappingEmrs.Add(mappingEmrAdoTemp);
                gridControlMappingEMR.DataSource = null;
                gridControlMappingEMR.DataSource = mappingEmrs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var mappingEMRs = gridControlMappingEMR.DataSource as List<ColumnMappingADO>;
                var mappingEMR = (ColumnMappingADO)gridViewMappingEMR.GetFocusedRow();
                if (mappingEMR != null)
                {
                    if (mappingEMRs.Count > 0)
                    {
                        mappingEMRs.Remove(mappingEMR);
                        gridControlMappingEMR.DataSource = null;
                        gridControlMappingEMR.DataSource = mappingEMRs;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnMappingEMR_Click(object sender, EventArgs e)
        {
            try
            {
                List<ColumnMappingADO> listObject = gridControlMappingEMR.DataSource as List<ColumnMappingADO>;
                var listObjectTemps = new List<Object>();
                foreach (var item in listObject)
                {
                    if (!String.IsNullOrEmpty(item.EmrColumn) || !String.IsNullOrEmpty(item.Key))
                    {
                        var listObjectTemp = new { EmrColumn = item.EmrColumn, Key = item.Key };
                        listObjectTemps.Add(listObjectTemp);
                    }
                }
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(listObjectTemps);
                if (jsonString == "[]")
                {
                    memoMappingEMR.Text = null;
                }
                else {
                    memoMappingEMR.Text = jsonString;
                }
               
                PopupContainerBarControl control = popupControlContainerMappingEMR.Parent as PopupContainerBarControl;
                control.ClosePopup();
                //popupControlContainerMappingEMR.Parent.clos
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //các hàm của memo DisablePrintByKey
        private void gridViewDisablePrintByKey_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "DELETE")
                {
                    DisablePrintByKeyCFGADO data = (DisablePrintByKeyCFGADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    //int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewMappingEMR.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (data.Edit == 0)
                    {
                        e.RepositoryItem = resBtnAddDisablePrint;
                    }
                    else
                    {
                        e.RepositoryItem = resBtnDeleteDisablePrint;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPopUpDisablePrintByKeyCFG_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(memoDisablePrintByKeyCFG.Text))
                {

                    //List<ColumnMappingADO> ados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ColumnMappingADO>>(rowData.EMR_COLUMN_MAPPING);
                    List<DisablePrintByKeyCFGADO> ados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DisablePrintByKeyCFGADO>>(memoDisablePrintByKeyCFG.Text);
                    if (ados == null || ados.Count == 0)
                    {
                        DisablePrintByKeyCFGADO mapping = new DisablePrintByKeyCFGADO();
                        mapping.Edit = 0;
                        ados.Add(mapping);
                    }
                    else if (ados.Count == 1)
                    {
                        foreach (var item in ados)
                        {
                            item.Edit = 0;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ados.Count; i++)
                        {
                            if (i == 0)
                            {
                                ados[0].Edit = 0;
                            }
                            else
                            {
                                ados[i].Edit = 1;
                            }
                        }
                    }
                    gridControlDisablePrintByKey.DataSource = ados;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ados), ados));
                }
                else
                {
                    List<DisablePrintByKeyCFGADO> ados = new List<DisablePrintByKeyCFGADO>();

                    DisablePrintByKeyCFGADO mapping = new DisablePrintByKeyCFGADO();
                    mapping.Edit = 0;
                    ados.Add(mapping);
                    gridControlDisablePrintByKey.DataSource = ados;
                }
                Rectangle buttonPosition = new Rectangle(btnPopUpDisablePrintByKeyCFG.Bounds.X, btnPopUpDisablePrintByKeyCFG.Bounds.Y, btnPopUpDisablePrintByKeyCFG.Bounds.Width, btnPopUpDisablePrintByKeyCFG.Bounds.Height);
                popupContainerDisablePrintByKeyCFG.ShowPopup(new Point(buttonPosition.X + 480, buttonPosition.Bottom + btnPopUpDisablePrintByKeyCFG.Bounds.Height - 10));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void resBtnAddDisablePrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                List<DisablePrintByKeyCFGADO> disablePrintAdoTemps = new List<DisablePrintByKeyCFGADO>();
                var disablePrints = gridControlDisablePrintByKey.DataSource as List<DisablePrintByKeyCFGADO>;
                DisablePrintByKeyCFGADO disablePrintAdoTemp = new DisablePrintByKeyCFGADO();
                disablePrintAdoTemp.Edit = 1;
                disablePrints.Add(disablePrintAdoTemp);
                gridControlDisablePrintByKey.DataSource = null;
                gridControlDisablePrintByKey.DataSource = disablePrints;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void resBtnDeleteDisablePrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var disablePrints = gridControlDisablePrintByKey.DataSource as List<DisablePrintByKeyCFGADO>;
                var disablePrint = (DisablePrintByKeyCFGADO)gridViewDisablePrintByKey.GetFocusedRow();
                if (disablePrint != null)
                {
                    if (disablePrints.Count > 0)
                    {
                        disablePrints.Remove(disablePrint);
                        gridControlDisablePrintByKey.DataSource = null;
                        gridControlDisablePrintByKey.DataSource = disablePrints;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDisablePrintByKeyCFG_Click(object sender, EventArgs e)
        {
            try
            {
                List<DisablePrintByKeyCFGADO> listObject = gridControlDisablePrintByKey.DataSource as List<DisablePrintByKeyCFGADO>;
                var listObjectTemps = new List<Object>();
                foreach (var item in listObject)
                {
                    if (!String.IsNullOrEmpty(item.Value) || !String.IsNullOrEmpty(item.Key) || !String.IsNullOrEmpty(item.Message))
                    {
                        var listObjectTemp = new { Key = item.Key, Value = item.Value, Message = item.Message };
                        listObjectTemps.Add(listObjectTemp);
                    }
                }
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(listObjectTemps);
                if (jsonString == "[]")
                {
                    memoDisablePrintByKeyCFG.Text = null;
                }
                else
                {
                    memoDisablePrintByKeyCFG.Text = jsonString;
                }
                
                PopupContainerBarControl control = popupContainerDisablePrintByKeyCFG.Parent as PopupContainerBarControl;
                control.ClosePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //các hàm của memo Gensignature
        private void btnPopUpGenSignatureEnable_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(memoGenSignatureEnable.Text))
                {
                    List<GenSignatureByKeyCFGADO> ados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GenSignatureByKeyCFGADO>>(memoGenSignatureEnable.Text);
                    if (ados == null || ados.Count == 0)
                    {
                        GenSignatureByKeyCFGADO mapping = new GenSignatureByKeyCFGADO();
                        mapping.Edit = 0;
                        ados.Add(mapping);
                    }
                    else if (ados.Count == 1)
                    {
                        foreach (var item in ados)
                        {
                            item.Edit = 0;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ados.Count; i++)
                        {
                            if (i == 0)
                            {
                                ados[0].Edit = 0;
                            }
                            else
                            {
                                ados[i].Edit = 1;
                            }
                        }
                    }
                    gridControlGenSignatureByKeyCFG.DataSource = ados;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ados), ados));
                }
                else
                {
                    List<GenSignatureByKeyCFGADO> ados = new List<GenSignatureByKeyCFGADO>();
                    GenSignatureByKeyCFGADO mapping = new GenSignatureByKeyCFGADO();
                    mapping.Edit = 0;
                    ados.Add(mapping);
                    gridControlGenSignatureByKeyCFG.DataSource = ados;
                }
                Rectangle buttonPosition = new Rectangle(btnPopUpGenSignatureEnable.Bounds.X, btnPopUpGenSignatureEnable.Bounds.Y, btnPopUpGenSignatureEnable.Bounds.Width, btnPopUpGenSignatureEnable.Bounds.Height);
                popupContainerGenSignatureByKeyCFG.ShowPopup(new Point(buttonPosition.X + 480, buttonPosition.Bottom + btnPopUpGenSignatureEnable.Bounds.Height - 10));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewGenSignatureByKeyCFG_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnAddAndDelete")
                {
                    GenSignatureByKeyCFGADO data = (GenSignatureByKeyCFGADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    //int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewMappingEMR.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (data.Edit == 0)
                    {
                        e.RepositoryItem = repositoryButtonAddGenSignature;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryButtonDeleteGenSignature;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryButtonAddGenSignature_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                List<GenSignatureByKeyCFGADO> genSignatureByKeyAdoTemps = new List<GenSignatureByKeyCFGADO>();
                var genSignatureByKeys = gridControlGenSignatureByKeyCFG.DataSource as List<GenSignatureByKeyCFGADO>;
                GenSignatureByKeyCFGADO genSignatureByKeyAdoTemp = new GenSignatureByKeyCFGADO();
                genSignatureByKeyAdoTemp.Edit = 1;
                genSignatureByKeys.Add(genSignatureByKeyAdoTemp);
                gridControlGenSignatureByKeyCFG.DataSource = null;
                gridControlGenSignatureByKeyCFG.DataSource = genSignatureByKeys;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => genSignatureByKeys), genSignatureByKeys));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryButtonDeleteGenSignature_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var genSignatureByKeyCFGs = gridControlGenSignatureByKeyCFG.DataSource as List<GenSignatureByKeyCFGADO>;
                var genSignatureByKeyCFG = (GenSignatureByKeyCFGADO)gridViewGenSignatureByKeyCFG.GetFocusedRow();
                if (genSignatureByKeyCFG != null)
                {
                    if (genSignatureByKeyCFGs.Count > 0)
                    {
                        genSignatureByKeyCFGs.Remove(genSignatureByKeyCFG);
                        gridControlGenSignatureByKeyCFG.DataSource = null;
                        gridControlGenSignatureByKeyCFG.DataSource = genSignatureByKeyCFGs;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGenSignatureByKeyCFG_Click(object sender, EventArgs e)
        {
            try
            {
                List<GenSignatureByKeyCFGADO> listObject = gridControlGenSignatureByKeyCFG.DataSource as List<GenSignatureByKeyCFGADO>;
                var listObjectTemps = new List<Object>();
                foreach (var item in listObject)
                {
                    if (!String.IsNullOrEmpty(item.LoginnameKey) || !String.IsNullOrEmpty(item.SignatureKey))
                    {
                        var listObjectTemp = new { LoginnameKey = item.LoginnameKey, SignatureKey = item.SignatureKey };
                        listObjectTemps.Add(listObjectTemp);
                    }
                }
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(listObjectTemps);
                if (jsonString == "[]")
                {
                    memoGenSignatureEnable.Text = null;
                }
                else
                {
                    memoGenSignatureEnable.Text = jsonString;
                }
                
                PopupContainerBarControl control = popupContainerGenSignatureByKeyCFG.Parent as PopupContainerBarControl;
                control.ClosePopup();
                //popupControlContainerMappingEMR.Parent.clos
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewGenSignatureByKeyCFG_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var row = (GenSignatureByKeyCFGADO)gridViewGenSignatureByKeyCFG.GetFocusedRow();
                if (e.Column.FieldName == "LoginnameKey")
                {
                    //string cellValue = view.GetRowCellValue(e.RowHandle, view.Columns["LoginnameKey"]).ToString() + "_SIGNATURE";
                    //view.SetRowCellValue(e.RowHandle, view.Columns["SignatureKey"], cellValue);
                    row.SignatureKey = row.LoginnameKey + "_SIGNATURE";
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cellValue), cellValue));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        private void cboEmrDocumentGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongGopNut.Focus();
                    chkKhongGopNut.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKhongGopNut_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkky.Focus();
                    chkky.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmrDocumentType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEmrDocumentType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmrDocumentGroup_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEmrDocumentGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkky_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkDigitalSign.Focus();
                    chkDigitalSign.SelectAll();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIS_PRINT_EXCEPTION_REASON_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkDoNotAllowReprint.Enabled)
                    {
                        chkDoNotAllowReprint.Focus();
                        chkDoNotAllowReprint.SelectAll();
                    }
                    else if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void btnPopUpNumCopyByKeyCFG_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(memoNumCopyByKeyCFG.Text))
                {
                    List<NumCopyByKeyCFGADO> ados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NumCopyByKeyCFGADO>>(memoNumCopyByKeyCFG.Text);
                    if (ados == null || ados.Count == 0)
                    {
                        NumCopyByKeyCFGADO mapping = new NumCopyByKeyCFGADO();
                        mapping.Edit = 0;
                        ados.Add(mapping);
                    }
                    else if (ados.Count == 1)
                    {
                        foreach (var item in ados)
                        {
                            item.Edit = 0;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ados.Count; i++)
                        {
                            if (i == 0)
                            {
                                ados[0].Edit = 0;
                            }
                            else
                            {
                                ados[i].Edit = 1;
                            }
                        }
                    }
                    gridControlNumCopyByKeyCFG.DataSource = ados;
                }
                else
                {
                    List<NumCopyByKeyCFGADO> ados = new List<NumCopyByKeyCFGADO>();
                    NumCopyByKeyCFGADO mapping = new NumCopyByKeyCFGADO();
                    mapping.Edit = 0;
                    ados.Add(mapping);
                    gridControlNumCopyByKeyCFG.DataSource = ados;
                }
                Rectangle buttonPosition = new Rectangle(btnPopUpNumCopyByKeyCFG.Bounds.X, btnPopUpNumCopyByKeyCFG.Bounds.Y, btnPopUpNumCopyByKeyCFG.Bounds.Width, btnPopUpNumCopyByKeyCFG.Bounds.Height);
                popupControlContainerNumCopyByKeyCFG.ShowPopup(new Point(buttonPosition.X + 480, buttonPosition.Bottom + popupControlContainerNumCopyByKeyCFG.Bounds.Height - 10));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewNumCopyByKeyCFG_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "DELETE")
                {
                    NumCopyByKeyCFGADO data = (NumCopyByKeyCFGADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data.Edit == 0)
                    {
                        e.RepositoryItem = resBtnAddNumCopyByKeyCFG;
                    }
                    else
                    {
                        e.RepositoryItem = resBtnDeleteNumCopyByKeyCFG;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void resBtnAddNumCopyByKeyCFG_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var numCopys = gridControlNumCopyByKeyCFG.DataSource as List<NumCopyByKeyCFGADO>;
                NumCopyByKeyCFGADO numCopyAdoTemp = new NumCopyByKeyCFGADO();
                numCopyAdoTemp.Edit = 1;
                numCopys.Add(numCopyAdoTemp);
                gridControlNumCopyByKeyCFG.DataSource = null;
                gridControlNumCopyByKeyCFG.DataSource = numCopys;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void resBtnDeleteNumCopyByKeyCFG_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var numCopys = gridControlNumCopyByKeyCFG.DataSource as List<NumCopyByKeyCFGADO>;
                var numCopy = (NumCopyByKeyCFGADO)gridViewNumCopyByKeyCFG.GetFocusedRow();
                if (numCopy != null)
                {
                    if (numCopys.Count > 0)
                    {
                        numCopys.Remove(numCopy);
                        gridControlNumCopyByKeyCFG.DataSource = null;
                        gridControlNumCopyByKeyCFG.DataSource = numCopys;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNumCopyByKeyCFG_Click(object sender, EventArgs e)
        {
            try
            {
                List<NumCopyByKeyCFGADO> listObject = gridControlNumCopyByKeyCFG.DataSource as List<NumCopyByKeyCFGADO>;
                var listObjectTemps = new List<Object>();
                foreach (var item in listObject)
                {
                    if (!String.IsNullOrEmpty(item.Value) || !String.IsNullOrEmpty(item.Key) || !String.IsNullOrEmpty(item.NumCopy))
                    {
                        var listObjectTemp = new { Key = item.Key.Trim(), Value = item.Value.Trim(), NumCopy = item.NumCopy.Trim() };
                        listObjectTemps.Add(listObjectTemp);
                    }
                }
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(listObjectTemps);
                if (jsonString == "[]")
                {
                    memoNumCopyByKeyCFG.Text = null;
                }
                else
                {
                    memoNumCopyByKeyCFG.Text = jsonString;
                }

                PopupContainerBarControl control = popupControlContainerNumCopyByKeyCFG.Parent as PopupContainerBarControl;
                control.ClosePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPopUpSetSignature_ChangeUICues(object sender, UICuesEventArgs e)
        {

        }

        private void btnPopUpSetSignature_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(memoSetSignature.Text))
                {
                    List<SetSignatureByKeyCFGADO> ados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SetSignatureByKeyCFGADO>>(memoSetSignature.Text);
                    if (ados == null || ados.Count == 0)
                    {
                        SetSignatureByKeyCFGADO SetSig = new SetSignatureByKeyCFGADO();
                        SetSig.Edit = 0;
                        SetSig.Num = 1;
                        ados.Add(SetSig);
                    }
                    else if (ados.Count == 1)
                    {
                        foreach (var item in ados)
                        {
                            item.Edit = 0;
                        }
                    }
                    else
                    {
                        ados[0].Edit = 0;
                        ados[0].Num = 1;

                        // Các hàng tiếp theo Num tăng dần
                        for (int i = 1; i < ados.Count; i++)
                        {
                            ados[i].Edit = 1;
                            ados[i].Num = ados[i - 1].Num + 1;
                        }
                    }

                    gridControlSetSignature.DataSource = ados.OrderBy(o => o.Edit).ToList();
                }
                else
                {
                    List<SetSignatureByKeyCFGADO> ados = new List<SetSignatureByKeyCFGADO>();
                    SetSignatureByKeyCFGADO SetSig = new SetSignatureByKeyCFGADO();
                    SetSig.Edit = 0;
                    SetSig.Num = 1;
                    ados.Add(SetSig);
                    gridControlSetSignature.DataSource = ados;
                }
                Rectangle buttonPosition = new Rectangle(btnPopUpSetSignature.Bounds.X, btnPopUpSetSignature.Bounds.Y, btnPopUpSetSignature.Bounds.Width, btnPopUpSetSignature.Bounds.Height);
                popupControlContainerSetSignature.ShowPopup(new Point(buttonPosition.X + 480, buttonPosition.Bottom + popupControlContainerSetSignature.Bounds.Height - 10));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repoBtnAddSetSignature_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                List<SetSignatureByKeyCFGADO> SetSigAdoTemps = new List<SetSignatureByKeyCFGADO>();
                var SetSig = gridControlSetSignature.DataSource as List<SetSignatureByKeyCFGADO>;
                SetSignatureByKeyCFGADO SetSigAdoTemp = new SetSignatureByKeyCFGADO();
                long? MaxNum = SetSig.Any() ? SetSig.Max(x => x.Num) : 0;
                SetSigAdoTemp.Num = MaxNum + 1;
                SetSigAdoTemp.Edit = 1;
                SetSig.Add(SetSigAdoTemp);

                gridControlSetSignature.DataSource = null;
                gridControlSetSignature.DataSource = SetSig;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repoBtnDeleteSetSignature_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var SetSigs = gridControlSetSignature.DataSource as List<SetSignatureByKeyCFGADO>;
                var SetSig = (SetSignatureByKeyCFGADO)gridViewSetSignature.GetFocusedRow();
                if (SetSig != null)
                {
                    if (SetSigs.Count > 0)
                    {
                        SetSigs.Remove(SetSig);
                        for (int i = 0; i < SetSigs.Count; i++)
                        {
                            SetSigs[i].Num = i + 1; // Gán Num từ 1 đến n
                        }
                        gridControlSetSignature.DataSource = null;
                        gridControlSetSignature.DataSource = SetSigs;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSetSignature_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

        }

        private void gridViewSetSignature_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "AddAndDelete")
                {
                    SetSignatureByKeyCFGADO data = (SetSignatureByKeyCFGADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data.Edit == 0)
                    {
                        e.RepositoryItem = repoBtnAddSetSignature;
                    }
                    else
                    {
                        e.RepositoryItem = repoBtnDeleteSetSignature;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSetSignature_Click(object sender, EventArgs e)
        {
            try
            {
                List<SetSignatureByKeyCFGADO> listObject = gridControlSetSignature.DataSource as List<SetSignatureByKeyCFGADO>;
                var listObjectTemps = new List<Object>();
                foreach (var item in listObject)
                {
                    if (!String.IsNullOrEmpty(item.Key) && item.Num != null)
                    {
                        var invalidRows = listObject.Where(x => string.IsNullOrEmpty(x.Key) || x.Num == null).ToList();

                        if (invalidRows.Count > 0)
                        {
                            XtraMessageBox.Show("Có hàng chứa dữ liệu không hợp lệ: Key hoặc Num bị null/trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var CountKey = listObject.GroupBy(x => x.Key.Trim())
                                      .Where(g => g.Count() > 1)
                                      .Select(g => g.Key)
                                      .ToList();

                        var CountNum = listObject
                                            .GroupBy(x => x.Num)
                                            .Where(g => g.Count() > 1 && g.Key != null)
                                            .Select(g => g.Key)
                                            .ToList();

                        if (CountNum.Count > 0)
                        {
                            XtraMessageBox.Show("Có thứ tự bị trùng: " + string.Join(", ", CountNum), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (CountKey.Count > 0)
                        {
                            XtraMessageBox.Show("Có Key bị trùng: " + string.Join(", ", CountKey), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (true)
                        {

                        }

                        var listObjectTemp = new { Key = item.Key.Trim(), Num = item.Num };
                        listObjectTemps.Add(listObjectTemp);

                    }
                    else if(String.IsNullOrEmpty(item.Key) && item.Num == null)
                    {
                        continue;
                    }
                    else
                    {
                        XtraMessageBox.Show("Hàng có dữ liệu không hợp lệ: Key hoặc Num đang bị null!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }     
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(listObjectTemps);
                if (jsonString == "[]")
                {
                    memoSetSignature.Text = null;
                }
                else
                {
                    memoSetSignature.Text = jsonString;
                }

                PopupContainerBarControl control = popupControlContainerSetSignature.Parent as PopupContainerBarControl;
                control.ClosePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
