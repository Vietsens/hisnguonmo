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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.LocalData;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisSaleExpMestList
{
    public partial class UCHisSaleExpMestList : UserControl
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        int lastRowHandle = -1;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        private string LoggingName = "";
        #endregion

        #region Construct
        public UCHisSaleExpMestList()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                gridControlSaleExpMestList.ToolTipController = this.toolTipController;
                //SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisSaleExpMestList(long roomId, long roomTypeId)
            : this()
        {
            try
            {
                medistock = Base.GlobalStore.ListMediStock.FirstOrDefault(o => o.ROOM_ID == roomId && o.ROOM_TYPE_ID == roomTypeId);
                this.roomId = roomId;
                this.roomTypeId = roomTypeId;
                //SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisPrescriptionList_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                //LoadKeysFromlanguage();

                FillDataNavStatus();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGrid();

                txtExpCode.Focus();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //filter
                this.txtExpCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__TXT_EXP_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__NAV_BAR_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__CREATE_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__CREATE_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.navBarGroupStatus.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__NAV_BAR_STATUS",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);

                //gridView
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnApprovalName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__APPROVAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnApprovalTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__APPROVAL_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnBill.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__CREATE_BILL",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnExpLoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__EXP_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnExpMestCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__EXP_MEST_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnExpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__EXP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnMediStockName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__MEDI_STOCK_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__MIDIFIER",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnReqDepartmentCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__REQ_DEPARTMENT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__REQ_DEPARTMENT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnReqName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__REQ_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnVirPatientName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__VIR_PATIENT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnPatientCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__PATIENT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.gridColumnTreatmentCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__TREATMENT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__TXT_TREATMENT_CODE_NULL_VALUE_PROMPT",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataNavStatus()
        {
            try
            {
                navBarControlFilter.BeginUpdate();
                int d = 0;
                foreach (var item in Base.GlobalStore.HisExpMestStts)
                {
                    navBarGroupStatus.GroupClientHeight += 25;
                    DevExpress.XtraEditors.CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit();
                    layoutControlStatus.Controls.Add(checkEdit);
                    checkEdit.Location = new System.Drawing.Point(57, 2 + (d * 23));
                    checkEdit.Name = item.ID.ToString();
                    checkEdit.Properties.Caption = item.EXP_MEST_STT_NAME;
                    checkEdit.Size = new System.Drawing.Size(134, 19);
                    checkEdit.StyleController = this.layoutControlStatus;
                    checkEdit.TabIndex = 4 + d;
                    checkEdit.EnterMoveNextControl = false;
                    d++;
                }
                navBarControlFilter.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtKeyWord.Text = "";
                txtExpCode.Text = "";
                dtCreateTimeFrom.DateTime = DateTime.Now;
                dtCreateTimeTo.DateTime = DateTime.Now;
                dtExpTimeFrom.EditValue = null;
                dtExpTimeTo.EditValue = null;
                txtExpCode.Focus();
                SetDefaultStatus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultStatus()
        {
            try
            {
                if (layoutControlStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlStatus.Controls.Count; i++)
                    {
                        if (layoutControlStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            checkEdit.Checked = false;
                        }
                    }
                }
                navBarGroupStatus.Expanded = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                GridPaging(new CommonParam(0, (int)ConfigApplications.NumPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, (int)ConfigApplications.NumPageSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SALE_EXP_MEST>> apiResult = null;
                MOS.Filter.HisSaleExpMestViewFilter filter = new MOS.Filter.HisSaleExpMestViewFilter();
                SetFilter(ref filter);
                gridViewSaleExpMestList.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SALE_EXP_MEST>>
                    (ApiConsumer.HisRequestUriStore.HIS_SALE_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlSaleExpMestList.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlSaleExpMestList.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewSaleExpMestList.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewSaleExpMestList.EndUpdate();
            }
        }

        private void SetFilter(ref MOS.Filter.HisSaleExpMestViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                if (roomId == 0)
                {
                    filter.CREATOR = LoggingName;
                }
                else
                {
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                }

                if (!String.IsNullOrEmpty(txtExpCode.Text))
                {
                    string code = txtExpCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpCode.Text = code;
                    }
                    filter.EXP_MEST_CODE__EXACT = code;
                }

                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue)
                    filter.EXP_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtExpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue)
                    filter.EXP_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtExpTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                SetFilterStatus(ref filter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetFilterStatus(ref MOS.Filter.HisSaleExpMestViewFilter filter)
        {
            try
            {
                if (layoutControlStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlStatus.Controls.Count; i++)
                    {
                        if (layoutControlStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            if (checkEdit.Checked)
                            {
                                if (filter.EXP_MEST_STT_IDs == null)
                                    filter.EXP_MEST_STT_IDs = new List<long>();
                                filter.EXP_MEST_STT_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string DisplayName(string loginname, string username)
        {
            string value = "";
            try
            {
                if (String.IsNullOrEmpty(loginname) && String.IsNullOrEmpty(username))
                {
                    value = "";
                }
                else if (loginname != "" && username == "")
                {
                    value = loginname;
                }
                else if (loginname == "" && username != "")
                {
                    value = username;
                }
                else if (loginname != "" && username != "")
                {
                    value = string.Format("{0} - {1}", loginname, username);
                }
                return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return value;
            }
        }

        private void txtExpCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtCreateTimeFrom.EditValue != null)
                    {
                        dtCreateTimeTo.Focus();
                        dtCreateTimeTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
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
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlSaleExpMestList)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlSaleExpMestList.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "EXP_MEST_STT_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "EXP_MEST_STT_ICON")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
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

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    long billId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSaleExpMestList.GetRowCellValue(e.RowHandle, "BILL_ID") ?? "").ToString());
                    long statusIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSaleExpMestList.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSaleExpMestList.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    long expMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSaleExpMestList.GetRowCellValue(e.RowHandle, "EXP_MEST_TYPE_ID") ?? "").ToString());
                    string creator = (gridViewSaleExpMestList.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    if (e.Column.FieldName == "EDIT_DISPLAY") // sửa
                    {
                        if ((creator == LoggingName) && (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT || statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED || statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST))
                        {
                            e.RepositoryItem = ButtonEditEnable;
                        }
                        else
                            e.RepositoryItem = ButtonEditDisable;
                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY") //hủy
                    {
                        if ((creator == LoggingName) && (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT || statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED || statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST))
                        {
                            e.RepositoryItem = ButtonDiscardEnable;
                        }
                        else
                            e.RepositoryItem = ButtonDiscardDisable;
                    }
                    else if (e.Column.FieldName == "APPROVAL_DISPLAY") //duyet
                    {
                        if (medistock != null && (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED || statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST))
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonApprovalEnable;
                            else
                                e.RepositoryItem = ButtonApprovalDisable;
                        }
                        else
                            e.RepositoryItem = ButtonApprovalDisable;
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")// Không duyệt
                    {
                        if (medistock != null && (statusIdCheckForButtonEdit != Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT && statusIdCheckForButtonEdit != Base.HisExpMestSttCFG.EXP_MEST_STT_ID__EXPORTED && statusIdCheckForButtonEdit != Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED))
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonDisApprovalEnable;
                            else
                                e.RepositoryItem = ButtonDisApprovalDisable;
                        }
                        else
                            e.RepositoryItem = ButtonDisApprovalDisable;
                    }
                    else if (e.Column.FieldName == "EXPORT_DISPLAY")// thực xuất
                    {
                        if (medistock != null && statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__APPROVED)
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonExportEnable;
                            else
                                e.RepositoryItem = ButtonExportDisable;
                        }
                        else
                            e.RepositoryItem = ButtonExportDisable;
                    }
                    else if (e.Column.FieldName == "CANCEL_EXPORT_DISPLAY")//hủy thực xuất
                    {
                        if (medistock != null && statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__EXPORTED)
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonCancelExport;
                            else
                                e.RepositoryItem = ButtonCancelExportDisable;
                        }
                        else
                            e.RepositoryItem = ButtonCancelExportDisable;
                    }
                    else if (e.Column.FieldName == "MOBA_IMP_MEST_CREATE")// Tạo yêu cầu nhập thu hồi
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__EXPORTED &&
                            expMestTypeId != Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__AGGR)
                        {
                            if (expMestTypeId == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__PRES)
                            {
                                decimal BloodTypeCount = (decimal)(gridViewSaleExpMestList.GetRowCellValue(e.RowHandle, "BLOOD_TYPE_COUNT") ?? 0);
                                if (BloodTypeCount == 0)
                                {
                                    e.RepositoryItem = ButtonEditMobaImpMestCreateEnable;
                                }
                                else
                                    e.RepositoryItem = ButtonEditMobaImpMestCreateDisable;
                            }
                            else
                                e.RepositoryItem = ButtonEditMobaImpMestCreateEnable;
                        }
                        else
                            e.RepositoryItem = ButtonEditMobaImpMestCreateDisable;
                    }
                    else if (e.Column.FieldName == "CREATE_BILL")
                    {
                        if (statusIdCheckForButtonEdit != Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED && billId == 0)
                        {
                            e.RepositoryItem = ButtonEditCreateBillEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditCreateBillDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SALE_EXP_MEST data = (MOS.EFMODEL.DataModels.V_HIS_SALE_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "EXP_MEST_STT_ICON")// trạng thái
                        {
                            if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT) //tam thoi
                            {
                                e.Value = imageListStatus.Images[0];
                            }
                            else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST) //yeu cau
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED) // tu choi duyet
                            {
                                e.Value = imageListStatus.Images[2];
                            }
                            else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__APPROVED) // duyet
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__EXPORTED) // da xuat
                            {
                                e.Value = imageListStatus.Images[4];
                            }
                            else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__AGGREGATE) //Tong hop
                            {
                                e.Value = imageListStatus.Images[5];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "EXP_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXP_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_LOGINNAME_DISPLAY")
                        {
                            string APPROVAL_LOGINNAME = data.APPROVAL_LOGINNAME;
                            string APPROVAL_USERNAME = data.APPROVAL_USERNAME;
                            e.Value = DisplayName(APPROVAL_LOGINNAME, APPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "EXP_LOGINNAME_DISPLAY")
                        {
                            string IMP_LOGINNAME = data.EXP_LOGINNAME;
                            string IMP_USERNAME = data.EXP_USERNAME;
                            e.Value = DisplayName(IMP_LOGINNAME, IMP_USERNAME);
                        }
                        else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
                        {
                            string Req_loginName = data.REQ_LOGINNAME;
                            string Req_UserName = data.REQ_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Public method
        public void Search()
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

        public void Refreshs()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusCode()
        {
            try
            {
                txtExpCode.Focus();
                txtExpCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void FocusTreatmentCode()
        {
            try
            {
                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void ButtonEditMobaImpMestCreateEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var ExpMestData = (MOS.EFMODEL.DataModels.V_HIS_SALE_EXP_MEST)gridViewSaleExpMestList.GetFocusedRow();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MobaImpMestCreate").FirstOrDefault();
                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.MobaImpMestCreate");
                    MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                }
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData.EXP_MEST_ID);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).ShowDialog();
                    FillDataApterClose(ExpMestData);
                }
                else
                {
                    MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterClose(MOS.EFMODEL.DataModels.V_HIS_SALE_EXP_MEST ExpMestData)
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

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisSaleExpMestList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisSaleExpMestList.UCHisSaleExpMestList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnVIewDetail.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnVIewDetail.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDiscard.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnDiscard.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApproval.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnApproval.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDisApproval.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnDisApproval.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnExport.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnExport.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCancelExport.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnCancelExport.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMobaImpMestCreate.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnMobaImpMestCreate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grridColumnExpMestStt.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.grridColumnExpMestStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit.NullText = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.repositoryItemPictureEdit.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMediStockName.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnVirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnVirPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnReqName.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnReqName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalName.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnApprovalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnApprovalTime.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnApprovalTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnExpLoginName.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnExpLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnExpTime.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnExpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnReqDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnReqDepartmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnReqDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreator.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifier.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.gridColumnModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtExpCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.txtExpCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControlFilter.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.navBarControlFilter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.navBarGroupCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.lciCreateTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.lciCreateTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlStatus.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.layoutControlStatus.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupStatus.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.navBarGroupStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.navBarGroupExpTime.Caption = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.navBarGroupExpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.lciCreateTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisSaleExpMestList.lciCreateTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



    }
}
