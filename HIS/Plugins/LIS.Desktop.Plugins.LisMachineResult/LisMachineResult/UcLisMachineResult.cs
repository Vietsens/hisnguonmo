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
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using LIS.SDO;
using LIS.Filter;
using LIS.Desktop.Plugins.LisMachineResult;
using LIS.Desktop.Plugins.LisMachineResult.ADO;
using LIS.EFMODEL.DataModels;
using LIS.Desktop.Plugins.LisMachineResult.Resources;
using DevExpress.XtraEditors.Controls;
using System.Text;
using HIS.Desktop.Utilities.Extensions;

namespace LIS.Desktop.Plugins.LisMachineResult.LisMachineResult
{
    public partial class UcLisMachineResult : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.Desktop.Common.Modules.Module module;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private int positionHandle;
        V_LIS_MACHINE_RESULT currentMachineResult;
        List<LIS_MACHINE> lisMachineSelecteds;
        public UcLisMachineResult(Inventec.Desktop.Common.Modules.Module module)
        {
            this.module = module;
            InitializeComponent();
        }

        private void UcLisMachineResult_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitComboLisMachine();
                SetDefaultValueLeft();
                LoadDataGridLeft();
                ValidateBarcodeEdit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UcLisMachineResult
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("LIS.Desktop.Plugins.LisMachineResult.Resources.Lang", typeof(LIS.Desktop.Plugins.LisMachineResult.LisMachineResult.UcLisMachineResult).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.ToolTip = Inventec.Common.Resource.Get.Value("UcLisMachineResult.btnSave.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblServiceReqCode.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.lblServiceReqCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblMachine.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.lblMachine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblTime.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.lblTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UcLisMachineResult.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UcLisMachineResult.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtBarCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UcLisMachineResult.txtBarCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UcLisMachineResult.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateBarcodeEdit()
        {
            try
            {
                ValidateEmptyOrNull valid = new ValidateEmptyOrNull();
                valid.txt = txtBarcodeEdit;
                dxValidationProvider1.SetValidationRule(txtBarcodeEdit, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueRight(V_LIS_MACHINE_RESULT data)
        {
            try
            {
                dxValidationProvider1.RemoveControlError(txtBarcodeEdit);
                if (data == null)
                {
                    lblMachine.Text = lblServiceReqCode.Text = lblTime.Text = null;
                    txtBarcodeEdit.Text = null;
                    dteEdit.EditValue = null;
                    gridControl2.DataSource = null;
                    btnEdit.Enabled = btnSave.Enabled = false;

                }
                else
                {
                    btnEdit.Enabled = btnSave.Enabled = true;
                    lblTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    lblMachine.Text = data.MACHINE_NAME ?? data.MACHINE_CODE;
                    lblServiceReqCode.Text = data.SERVICE_REQ_CODE;
                    txtBarcodeEdit.Text = data.BARCODE;
                    dteEdit.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.CREATE_TIME ?? 0) ?? DateTime.Now;
                    LoadDataGridRight(data);
                    gridView2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void InitComboLisMachine()
        {
            InitCheck(cboLisMachine, SelectionGrid__cboLisMachine);
            InitCombo(cboLisMachine, BackendDataWorker.Get<LIS_MACHINE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "MACHINE_NAME", "MACHINE_CODE");
        }
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField(ValueMember);
                col1.VisibleIndex = 1;
                col1.Width = 100;
                col1.Caption = "ALL";

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 2;
                col2.Width = 250;
                col2.Caption = "Tất cả";

                cbo.Properties.PopupFormWidth = 350;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
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
        private void SelectionGrid__cboLisMachine(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<LIS_MACHINE> sgSelectedNews = new List<LIS_MACHINE>();
                    foreach (LIS_MACHINE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.MACHINE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.lisMachineSelecteds = new List<LIS_MACHINE>();
                    this.lisMachineSelecteds.AddRange(sgSelectedNews);
                }

                this.cboLisMachine.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDataGridRight(V_LIS_MACHINE_RESULT data)
        {
            try
            {
                List<MachineIndexResultADO> lstADO = new List<MachineIndexResultADO>();
                CommonParam param = new CommonParam();
                LIS.Filter.LisMachineIndexResultViewFilter filter = new LisMachineIndexResultViewFilter();
                filter.MACHINE_RESULT_ID = data.ID;
                var dataList = new BackendAdapter(param).Get<List<V_LIS_MACHINE_INDEX_RESULT>>("api/LisMachineIndexResult/GetView", ApiConsumers.LisConsumer, filter, param);
                if (dataList != null && dataList.Count > 0)
                {
                    var dataIndex = BackendDataWorker.Get<V_LIS_TEST_INDEX_MAP>().Where(o => dataList.Exists(p => p.MACHINE_INDEX_CODE == o.MACHINE_INDEX_CODE));
                    foreach (var item in dataList)
                    {
                        MachineIndexResultADO ado = new MachineIndexResultADO(item);
                        var lstIndex = dataIndex.Where(p => p.MACHINE_INDEX_CODE == item.MACHINE_INDEX_CODE);
                        if (dataIndex != null && dataIndex.Count() > 0 && lstIndex.Count() > 0 && lstIndex.Count() == 1)
                        {
                            ado.TEST_INDEX_CODE = lstIndex.FirstOrDefault().TEST_INDEX_CODE;
                            ado.TEST_INDEX_NAME = lstIndex.FirstOrDefault().TEST_INDEX_NAME;
                        }
                        lstADO.Add(ado);
                    }
                }
                gridControl2.DataSource = lstADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGridLeft()
        {
            try
            {
                WaitingManager.Show();

                currentMachineResult = null;
                SetDefaultValueRight(currentMachineResult);
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, numPageSize, this.gridControl1);
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
                Inventec.Core.ApiResultObject<List<LIS.EFMODEL.DataModels.V_LIS_MACHINE_RESULT>> apiResult = null;
                LisMachineResultViewFilter filter = new LisMachineResultViewFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<LIS.EFMODEL.DataModels.V_LIS_MACHINE_RESULT>>("api/LisMachineResult/GetView", ApiConsumers.LisConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("apiResult", apiResult));
                if (apiResult != null)
                {
                    var data = (List<LIS.EFMODEL.DataModels.V_LIS_MACHINE_RESULT>)apiResult.Data;
                    if (data != null)
                    {
                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetFilterNavBar(ref LisMachineResultViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
                if (dteFrom.EditValue != null && dteFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Int64.Parse(dteFrom.DateTime.ToString("yyyyMMdd000000"));
                if (dteTo.EditValue != null && dteTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Int64.Parse(dteTo.DateTime.ToString("yyyyMMdd235959"));
                switch (cboStatus.SelectedIndex)
                {
                    case 1:
                        filter.IS_SAVED = true;
                        break;
                    case 2:
                        filter.IS_SAVED = false;
                        break;
                    default:
                        break;
                }
                if (this.module != null)
                {
                    var currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.module.RoomId);
                    filter.EXECUTE_ROOM_CODE__EXACT_OR_NULL = currentRoom != null ? currentRoom.ROOM_CODE : null;
                }
                if (lisMachineSelecteds != null && lisMachineSelecteds.Count > 0)
                {
                    filter.MACHINE_CODES = lisMachineSelecteds.Select(o => o.MACHINE_CODE).ToList();
                }
                filter.BARCODE__EXACT = txtBarCode.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }


        private void SetDefaultValueLeft()
        {
            try
            {
                dteFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.Now;
                dteTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.Now;
                cboStatus.SelectedIndex = 0;
                txtBarCode.Text = null;
                txtSearch.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                currentMachineResult = (V_LIS_MACHINE_RESULT)gridView1.GetFocusedRow();
                SetDefaultValueRight(currentMachineResult);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                string gateCode = (gridView1.GetRowCellValue(e.RowHandle, "IS_SAVED") ?? "").ToString();
                if (e.Column.FieldName == "IsSave")
                {
                    if (!string.IsNullOrEmpty(gateCode) && gateCode == "1")
                    {
                        e.RepositoryItem = repV;
                    }
                    else
                    {
                        e.RepositoryItem = repX;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    LIS.EFMODEL.DataModels.V_LIS_MACHINE_RESULT pData = (LIS.EFMODEL.DataModels.V_LIS_MACHINE_RESULT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "MACHINE")
                    {
                        try
                        {
                            e.Value = pData.MACHINE_NAME ?? pData.MACHINE_CODE;
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                string gateCode = (gridView2.GetRowCellValue(e.RowHandle, "IS_SAVED") ?? "").ToString();
                if (e.Column.FieldName == "IsSave")
                {
                    if (!string.IsNullOrEmpty(gateCode) && gateCode == "1")
                    {
                        e.RepositoryItem = repV2;
                    }
                    else
                    {
                        e.RepositoryItem = repX2;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MachineIndexResultADO pData = (MachineIndexResultADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
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
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MachineResultBarcodeSDO sdo = new MachineResultBarcodeSDO();
                sdo.Barcode = txtBarcodeEdit.Text.Trim();
                sdo.MachineResultId = currentMachineResult.ID;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                var data = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_MACHINE_RESULT>("api/LisMachineResult/UpdateBarcode", ApiConsumers.LisConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (data != null)
                {
                    LoadDataGridLeft();
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, data != null);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowSelected = gridView2.GetSelectedRows();
                if (rowSelected == null || rowSelected.Count() == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show
                            ("Bạn phải chọn ít nhất 1 chỉ số máy để lưu lại kết quả", "Thông báo");
                    return;
                }

                WaitingManager.Show();
                List<long> indexResults = new List<long>();
                foreach (var i in rowSelected)
                {
                    var row = (MachineIndexResultADO)gridView2.GetRow(i);
                    indexResults.Add(row.ID);
                }
                MachineResultSDO sdo = new MachineResultSDO();
                if (dteEdit.EditValue != null && dteEdit.DateTime != DateTime.MinValue)
                    sdo.BarcodeTime = Int64.Parse(dteEdit.DateTime.ToString("yyyyMMdd000000"));
                sdo.MachineResultId = currentMachineResult.ID;
                sdo.IndexResultsIds = indexResults;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                var data = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/LisMachineResult/ReturnResult", ApiConsumers.LisConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (data)
                {
                    LoadDataGridLeft();
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, data);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataGridLeft();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void FindShortCut()
        {
            btnSearch_Click(null, null);
        }
        public void SaveShortCut()
        {
            if (btnSave.Enabled)
                btnSave_Click(null, null);
        }
        public void EditShortCut()
        {
            if (btnEdit.Enabled)
                btnEdit_Click(null, null);
        }

        private void txtBarCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(txtBarCode.Text))
                    {
                        txtSearch.Focus();
                        txtSearch.SelectAll();
                    }
                    else
                    {
                        this.btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    this.btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLisMachine_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (LIS_MACHINE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.MACHINE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
