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
using HIS.Desktop.LocalStorage.BackendData;
using HTC.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using Inventec.Core;
using HTC.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.RevenueList.Base;
using HIS.Desktop.Plugins.RevenueList.Update;
using HIS.Desktop.LibraryMessage;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.RevenueList
{
    public partial class UCRevenueList : HIS.Desktop.Utility.UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int start;
        int limit;
        Inventec.Desktop.Common.Modules.Module moduleData;

        public UCRevenueList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = _moduleData;
                ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCRevenueList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyUCLanguage();
                LoadDataToComboPeriod();
                LoadDataToCboExeDepart();
                LoadDataToCboReqDepart();
                SetDefaultControl();
                FillDataToGridControl();
                txtKeyword.Focus();
                txtKeyword.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                dtCreateTimeFrom.EditValue = null;
                dtCreateTimeTo.EditValue = null;
                dtRevenueTimeFrom.EditValue = null;
                dtRevenueTimeTo.EditValue = null;
                dtInTimeFrom.EditValue = null;
                dtInTimeTo.EditValue = null;
                dtOutTimeFrom.EditValue = null;
                dtOutTimeTo.EditValue = null;
                cboExeDepartment.EditValue = null;
                cboReqDepartment.EditValue = null;
                txtKeyword.Text = "";
                cboPeriod.EditValue = null;
                SetDefaultPeriod();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultPeriod()
        {
            try
            {
                var data = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__TRUE);
                if (data == null)
                {
                    data = BackendDataWorker.Get<HTC_PERIOD>().OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                }
                if (data != null)
                {
                    cboPeriod.EditValue = data.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboPeriod()
        {
            try
            {
                cboPeriod.Properties.DataSource = BackendDataWorker.Get<HTC_PERIOD>().OrderByDescending(o => o.CREATE_TIME).ToList();
                cboPeriod.Properties.DisplayMember = "PERIOD_NAME";
                cboPeriod.Properties.ValueMember = "ID";
                cboPeriod.Properties.ForceInitialize();
                cboPeriod.Properties.Columns.Clear();
                cboPeriod.Properties.Columns.Add(new LookUpColumnInfo("PERIOD_CODE", "Mã", 50));
                cboPeriod.Properties.Columns.Add(new LookUpColumnInfo("PERIOD_NAME", "Tên", 140));
                cboPeriod.Properties.ShowHeader = true;
                cboPeriod.Properties.ImmediatePopup = true;
                cboPeriod.Properties.DropDownRows = 10;
                cboPeriod.Properties.PopupWidth = 190;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboReqDepart()
        {
            try
            {
                cboReqDepartment.Properties.DataSource = BackendDataWorker.Get<HIS_DEPARTMENT>().OrderBy(o => o.NUM_ORDER).ToList();
                cboReqDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboReqDepartment.Properties.ValueMember = "ID";
                cboReqDepartment.Properties.ForceInitialize();
                cboReqDepartment.Properties.Columns.Clear();
                cboReqDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "Mã", 50));
                cboReqDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "Tên", 120));
                cboReqDepartment.Properties.ShowHeader = true;
                cboReqDepartment.Properties.ImmediatePopup = true;
                cboReqDepartment.Properties.DropDownRows = 10;
                cboReqDepartment.Properties.PopupWidth = 170;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboExeDepart()
        {
            try
            {
                cboExeDepartment.Properties.DataSource = BackendDataWorker.Get<HIS_DEPARTMENT>().OrderBy(o => o.NUM_ORDER).ToList();
                cboExeDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboExeDepartment.Properties.ValueMember = "ID";
                cboExeDepartment.Properties.ForceInitialize();
                cboExeDepartment.Properties.Columns.Clear();
                cboExeDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "Mã", 50));
                cboExeDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "Tên", 120));
                cboExeDepartment.Properties.ShowHeader = true;
                cboExeDepartment.Properties.ImmediatePopup = true;
                cboExeDepartment.Properties.DropDownRows = 10;
                cboExeDepartment.Properties.PopupWidth = 170;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                FillDataToGridControlRevenue(new CommonParam(0, (int)100));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridControlRevenue, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControlRevenue(object param)
        {
            try
            {
                gridControlRevenueList.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HtcRevenueFilter filter = new HtcRevenueFilter();
                if (!String.IsNullOrEmpty(txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                }
                if (cboPeriod.EditValue != null)
                {
                    filter.PERIOD_ID = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPeriod.EditValue)).ID;
                }

                if (cboReqDepartment.EditValue != null)
                {
                    var reqDepart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboReqDepartment.EditValue));
                    if (reqDepart != null)
                    {
                        filter.REQUEST_DEPARTMENT_CODE_EXACT = reqDepart.DEPARTMENT_CODE;
                    }
                }

                if (cboExeDepartment.EditValue != null)
                {
                    var exeDepart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExeDepartment.EditValue));
                    if (exeDepart != null)
                    {
                        filter.EXECUTE_DEPARTMENT_CODE_EXACT = exeDepart.DEPARTMENT_CODE;
                    }
                }

                if (dtRevenueTimeFrom.EditValue != null && dtRevenueTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.REVENUE_TIME_FROM = Convert.ToInt64(dtRevenueTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtRevenueTimeTo.EditValue != null && dtRevenueTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.REVENUE_TIME_TO = Convert.ToInt64(dtRevenueTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }
                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_TO = Convert.ToInt64(dtCreateTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                if (dtInTimeFrom.EditValue != null && dtInTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.IN_TIME_FROM = Convert.ToInt64(dtInTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtInTimeTo.EditValue != null && dtInTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.IN_TIME_TO = Convert.ToInt64(dtInTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                if (dtOutTimeFrom.EditValue != null && dtOutTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.OUT_TIME_FROM = Convert.ToInt64(dtOutTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtOutTimeTo.EditValue != null && dtOutTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.OUT_TIME_TO = Convert.ToInt64(dtOutTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<HTC_REVENUE>>(HtcRequestUriStore.HTC_REVENUE__GET, ApiConsumers.HtcConsumer, filter, paramCommon);
                if (result != null)
                {
                    var listData = (List<HTC_REVENUE>)result.Data;
                    rowCount = (listData == null ? 0 : listData.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    gridControlRevenueList.BeginUpdate();
                    gridControlRevenueList.DataSource = listData;
                    gridControlRevenueList.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPeriod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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

        private void dtRevenueTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtRevenueTimeTo.Focus();
                    dtRevenueTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtRevenueTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtCreateTimeTo.Focus();
                    dtCreateTimeTo.ShowPopup();
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

        private void cboReqDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboReqDepartment.EditValue != null)
                {
                    cboReqDepartment.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboReqDepartment.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReqDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboReqDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExeDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExeDepartment.EditValue != null)
                {
                    cboExeDepartment.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboExeDepartment.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExeDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExeDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRevenueList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                try
                {
                    if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                    {
                        var data = (HTC_REVENUE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data != null)
                        {
                            if (e.Column.FieldName == "STT")
                            {
                                try
                                {
                                    e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                            else if (e.Column.FieldName == "REVENUE_TIME_STR")
                            {
                                try
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.REVENUE_TIME);
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                            else if (e.Column.FieldName == "DOB_STR")
                            {
                                try
                                {
                                    e.Value = data.DOB.HasValue ? (data.DOB.Value.ToString().Substring(0, 4)) : "";
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                            else if (e.Column.FieldName == "IN_TIME_STR")
                            {
                                try
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.IN_TIME ?? 0);
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                            else if (e.Column.FieldName == "OUT_TIME_STR")
                            {
                                try
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.OUT_TIME ?? 0);
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
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
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
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultControl();
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HTC_REVENUE)gridViewRevenueList.GetFocusedRow();
                if (data != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HtcRevenue/Delete", ApiConsumers.HtcConsumer, data.ID, param);
                    if (success)
                    {
                        FillDataToGridControlRevenue(new CommonParam(start, limit));
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HTC_REVENUE)gridViewRevenueList.GetFocusedRow();
                if (data != null)
                {
                    frmUpdateRevenue frm = new frmUpdateRevenue(data, this.moduleData);
                    frm.ShowDialog();
                    FillDataToGridControlRevenue(new CommonParam(start, limit));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeyUCLanguage()
        {
            try
            {
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__BTN_FIND", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__BTN_REFRESH", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //gridControlRevenue
                this.gridColumn_RevenueList_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_CreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_CREATE_TIME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_Creator.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_CREATOR", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_ExeDepartmentName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_EXECUTE_DEPARTMENT_NAME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_ExeRoomName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_EXECUTE_ROOM_NAME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_HeinPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_HEIN_PRICE", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_Modifier.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_MODIFIER", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_MODIFY_TIME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_PatientTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_PATIENT_TYPE_NAME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_Price.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_PRICE", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_ReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_REQUEST_DEPARTMENT_NAME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_ReqRoomName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_REQUEST_ROOM_NAME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_RevenueCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_REVENUE_CODE", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_RevenueTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_REVENUE_TIME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_ServiceName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_ServiceTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_SERVICE_TYPE_NAME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_VirTotalHeinPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_VIR_TOTAL_HEIN_PRICE", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_VirTotalPatientPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_VIR_TOTAL_PATIENT_PRICE", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_RevenueList_VirTotalPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__GRID_CONTROL__COLUMN_VIR_TOTAL_PRICE", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__NAV_BAR_CONTROL__CREATE_TIME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.navBarGroupPeriod.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__NAV_BAR_CONTROL__PERIOD", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.navBarGroupRevenueTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__NAV_BAR_CONTROL__REVENUE_TIME", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.layoutRevenueTimeFrom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__LAYOUT_TIME_FROM", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutRevenueTimeTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__LAYOUT_TIME_TO", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.layoutCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__LAYOUT_TIME_FROM", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_REVENUE_LIST__LAYOUT_TIME_TO", Base.ResourceLangManager.LanguageUCRevenueList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnFind()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnRefresh()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeleteRevenue_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnDeleteRevenue.Enabled)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RevenueDeleteByPeriod").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RevenueDeleteByPeriod'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToGridControlRevenue(new CommonParam(start, limit));
                }
                else
                {
                    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
