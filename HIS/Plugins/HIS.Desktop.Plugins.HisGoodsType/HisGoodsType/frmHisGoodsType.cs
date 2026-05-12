/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisGoodsType
{
    public partial class frmHisGoodsType : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_GOODS_TYPE currentData;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        RefeshReference delegateRefresh;
        bool isResetting = false;
        #endregion

        #region Construct
        public frmHisGoodsType(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
            : base(_module)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = _module;
                this.delegateRefresh = _delegateRefresh;
                gridControlFormList.ToolTipController = toolTipControllerGrid;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmHisGoodsType(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;
                SetIcon();
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
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Form Load
        private void frmHisGoodsType_Load(object sender, EventArgs e)
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

        public void MeShow()
        {
            try
            {
                ConfigSpinNumOrderAllowNull();
                SetDefaultValue();
                EnableControlChanged(this.ActionType);
                FillDataToGridControl();
                SetCaptionByLanguageKey();
                InitTabIndex();
                ValidateForm();
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ConfigSpinNumOrderAllowNull()
        {
            try
            {
                spNumOrder.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                spNumOrder.Properties.IsFloatValue = false;
                spNumOrder.Properties.Mask.EditMask = "N0";
                spNumOrder.Properties.NullValuePromptShowForEmptyValue = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Caption / Language
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.HisGoodsType.Resources.Lang",
                    typeof(HIS.Desktop.Plugins.HisGoodsType.frmHisGoodsType).Assembly);

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisGoodsType.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisGoodsType.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.grdColCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisGoodsType.grdColCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.grdColName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisGoodsType.grdColName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumOrder.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.grdColNumOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumOrder.ToolTip = Inventec.Common.Resource.Get.Value("frmHisGoodsType.grdColNumOrder.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisGoodsType.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisGoodsType.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisGoodsType.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisGoodsType.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGoodsTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisGoodsType.lciGoodsTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGoodsTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisGoodsType.lciGoodsTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisGoodsType.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisGoodsType.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisGoodsType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
        #endregion

        #region Default / Focus / TabIndex
        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                ResetFormData();
                EnableControlChanged(this.ActionType);
                txtKeyword.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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

        private void SetFocusEditor()
        {
            try
            {
                txtGoodsTypeCode.Focus();
                txtGoodsTypeCode.SelectAll();
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
                dicOrderTabIndexControl.Add("txtGoodsTypeCode", 0);
                dicOrderTabIndexControl.Add("txtGoodsTypeName", 1);
                dicOrderTabIndexControl.Add("spNumOrder", 2);

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
                            if (be != null && itemOrderTab.Key.Contains(be.Name))
                            {
                                be.TabIndex = itemOrderTab.Value;
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
        #endregion

        #region Grid Load / Paging
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

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_GOODS_TYPE>> apiResult = null;
                HisGoodsTypeFilter filter = new HisGoodsTypeFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "NUM_ORDER";
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_GOODS_TYPE>>(
                    HisRequestUriStore.HIS_GOODS_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_GOODS_TYPE>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewFormList.EndUpdate();

                SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisGoodsTypeFilter filter)
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
        #endregion

        #region Grid Events
        private void gridviewFormList_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var pData = (MOS.EFMODEL.DataModels.HIS_GOODS_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.Value = Inventec.Common.Resource.Get.Value(
                                "frmHisGoodsType.HoatDong",
                                Resources.ResourceLanguageManager.LanguageResource,
                                LanguageManager.GetCulture());
                        }
                        else
                        {
                            e.Value = Inventec.Common.Resource.Get.Value(
                                "frmHisGoodsType.TamKhoa",
                                Resources.ResourceLanguageManager.LanguageResource,
                                LanguageManager.GetCulture());
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }
                }
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
                    var data = (MOS.EFMODEL.DataModels.HIS_GOODS_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
            if (e.RowHandle >= 0)
            {
                var data = (MOS.EFMODEL.DataModels.HIS_GOODS_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void gridviewFormList_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_GOODS_TYPE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_GOODS_TYPE)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentData = rowData;
                        ChangedDataRow(rowData);
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
                if (isResetting) return;

                var dataList = gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.HIS_GOODS_TYPE>;
                if (dataList == null || dnNavigation.Position < 0 || dnNavigation.Position >= dataList.Count) return;

                this.currentData = dataList[dnNavigation.Position];
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
        #endregion

        #region Editor (Form Right Side)
        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_GOODS_TYPE data)
        {
            try
            {
                if (data == null) return;
                FillDataToEditorControl(data);
                this.ActionType = GlobalVariables.ActionEdit;
                EnableControlChanged(this.ActionType);

                btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_GOODS_TYPE data)
        {
            try
            {
                if (data == null) return;
                txtGoodsTypeCode.Text = data.GOODS_TYPE_CODE;
                txtGoodsTypeName.Text = data.GOODS_TYPE_NAME;
                spNumOrder.EditValue = data.NUM_ORDER;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            BaseEdit fomatFrm = lci.Control as BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                    txtGoodsTypeCode.Focus();
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_GOODS_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisGoodsTypeFilter filter = new HisGoodsTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_GOODS_TYPE>>(
                    HisRequestUriStore.HIS_GOODS_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Search
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_GOODS_TYPE)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentData = rowData;
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Save / Refresh / Cancel
        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                isResetting = true;
                try
                {
                    this.currentData = null;
                    SetDefaultValue();
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                    FillDataToGridControl();
                }
                finally
                {
                    isResetting = false;
                }
                SetFocusEditor();
            }
            catch (Exception ex)
            {
                isResetting = false;
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
                if (!btnEdit.Enabled && !btnAdd.Enabled) return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate()) return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_GOODS_TYPE updateDTO = new MOS.EFMODEL.DataModels.HIS_GOODS_TYPE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_GOODS_TYPE>(
                        HisRequestUriStore.HIS_GOODS_TYPE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                        if (this.delegateRefresh != null) this.delegateRefresh();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_GOODS_TYPE>(
                        HisRequestUriStore.HIS_GOODS_TYPE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        if (this.delegateRefresh != null) this.delegateRefresh();
                    }
                }

                if (success) SetFocusEditor();
                WaitingManager.Hide();

                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(
                    "SaveProcess that bai." +
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => currentData), currentData),
                    ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_GOODS_TYPE currentDTO)
        {
            try
            {
                currentDTO.GOODS_TYPE_CODE = txtGoodsTypeCode.Text.Trim();
                currentDTO.GOODS_TYPE_NAME = txtGoodsTypeName.Text.Trim();
                if (spNumOrder.EditValue != null && !string.IsNullOrWhiteSpace(spNumOrder.Text))
                {
                    currentDTO.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(spNumOrder.Value.ToString());
                }
                else
                {
                    currentDTO.NUM_ORDER = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Lock / Unlock / Delete
        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_GOODS_TYPE)gridviewFormList.GetFocusedRow();
                if (rowData == null) return;

                if (XtraMessageBox.Show(
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                CommonParam param = new CommonParam();
                bool success = false;

                WaitingManager.Show();
                success = new BackendAdapter(param).Post<bool>(
                    HisRequestUriStore.HIS_GOODS_TYPE_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                WaitingManager.Hide();

                if (success)
                {
                    FillDataToGridControl();
                    var firstRow = ((List<MOS.EFMODEL.DataModels.HIS_GOODS_TYPE>)gridControlFormList.DataSource);
                    currentData = firstRow != null ? firstRow.FirstOrDefault() : null;
                    if (this.delegateRefresh != null) this.delegateRefresh();
                }
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ChangeLockProcess(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong);
        }

        private void btnGunLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ChangeLockProcess(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong);
        }

        private void ChangeLockProcess(LibraryMessage.Message.Enum confirmMsg)
        {
            CommonParam param = new CommonParam();
            try
            {
                var data = (MOS.EFMODEL.DataModels.HIS_GOODS_TYPE)gridviewFormList.GetFocusedRow();
                if (data == null) return;

                if (XtraMessageBox.Show(
                        MessageUtil.GetMessage(confirmMsg),
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                WaitingManager.Show();
                var success = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_GOODS_TYPE>(
                    HisRequestUriStore.HIS_GOODS_TYPE_CHANGE_LOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();

                if (success != null)
                {
                    FillDataToGridControl();
                    if (this.delegateRefresh != null) this.delegateRefresh();
                }
                MessageManager.Show(this, param, success != null);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationMaxlength(txtGoodsTypeCode, 20, true);
                ValidationMaxlength(txtGoodsTypeName, 200, true);
                ValidationSpinNumOrder();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxlength(BaseEdit control, int maxLength, bool isRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validRule = new ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.maxLength = maxLength;
                validRule.IsRequired = isRequired;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSpinNumOrder()
        {
            try
            {
                ValidateSpinNumOrder validRule = new ValidateSpinNumOrder();
                validRule.spin = spNumOrder;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(spNumOrder, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null) return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null) return;

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
        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut bar
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try { btnSearch_Click(null, null); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
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
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try { btnRefesh_Click(null, null); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try { txtKeyword.Focus(); txtKeyword.SelectAll(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion

        #region Editor key navigation
        private void spNumOrder_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
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
        #endregion
    }
}
