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
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils;

namespace HIS.Desktop.Plugins.PrepareList
{
    public partial class UCPrepareList : HIS.Desktop.Utility.UserControlBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private string LoggingName = "";
        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private HIS_MEDI_STOCK Medistock;

        public UCPrepareList()
        {
            InitializeComponent();
        }

        public UCPrepareList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.Medistock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == moduleData.RoomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCPrepareList_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGrid();

                TxtKeyword.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.BarCreateTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__BAR_CREATE_TIME");
                this.BtnApproval.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__BTN_APPROVAL");
                this.BtnRefresh.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__BTN_REFRESH");
                this.BtnSearch.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__BTN_SEARCH");
                this.Gc_Approval.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_APPROVAL");
                this.Gc_ApprovalName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_APPROVAL_NAME");
                this.Gc_ApprovalTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_APPROVAL_TIME");
                this.Gc_CreateTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_CREATE_TIME");
                this.Gc_Creator.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_CREATOR");
                this.Gc_Delete.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_DELETE");
                this.Gc_Description.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_DESCRIPTION");
                this.Gc_DisApproval.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_DISAPPROVAL");
                this.Gc_Edit.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_EDIT");
                this.Gc_FromTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_FROM_TIME");
                this.Gc_GenderName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_GENDER_NAME");
                this.Gc_HeinCardNumber.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_HEIN_CAR_BUMBER");
                this.Gc_Modifier.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_MODIFIER");
                this.Gc_ModifyTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_MODIFY_TIME");
                this.Gc_PatientAddress.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_PATIENT_ADDRESS");
                this.Gc_PatientCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_PATIENT_CODE");
                this.Gc_PatientDob.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_PATIENT_DOB");
                this.Gc_PatientName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_PATIENT_NAME");
                this.Gc_PrepareCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_PREPARE_CODE");
                this.Gc_ReqName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_REQ_NAME");
                this.Gc_Stt.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_STT");
                this.Gc_ToTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_TO_TIME");
                this.Gc_TreatmentCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__TREATMENT_CODE");
                this.Gc_View.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__GC_VIEW");
                this.LciTimeFrom.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__LCI_TIME_FROM");
                this.LciTimeTo.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__LCI_TIME_TO");
                this.repositoryItemBtnApproval.Buttons[0].ToolTip = this.repositoryItemBtnApprovalDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__RP_BTN_APPROVAL");
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = this.repositoryItemBtnDeleteDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__RP_BTN_DELETE");
                this.repositoryItemBtnDisapproval.Buttons[0].ToolTip = this.repositoryItemBtnDisapprovalDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__RP_BTN_DISAPPROVAL");
                this.repositoryItemBtnEdit.Buttons[0].ToolTip = this.repositoryItemBtnEditDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__RP_BTN_EDIT");
                this.repositoryItemBtnView.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__RP_BTN_VIEW");
                this.TxtKeyword.Properties.NullValuePrompt = GetLanguageControl("IVT_LANGUAGE_KEY__UC_PREPARE_LIST__TXT_KEYWORD__NULL_VALUE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void SetDefaultValueControl()
        {
            try
            {
                DtTimeFrom.DateTime = DateTime.Now;
                DtTimeTo.DateTime = DateTime.Now;
                TxtKeyword.Text = "";
                TxtKeyword.Focus();
                TxtKeyword.SelectAll();
                if (this.Medistock == null)
                {
                    LciBtnApproval.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    gridViewPrepare.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                    this.Gc_Approval.VisibleIndex = -1;
                    this.Gc_DisApproval.VisibleIndex = -1;
                }
                else
                {
                    LciBtnApproval.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    gridViewPrepare.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
                }

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
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControlPrepare);
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
                ApiResultObject<List<V_HIS_PREPARE>> apiResult = null;
                HisPrepareViewFilter filter = new HisPrepareViewFilter();
                SetFilter(ref filter);

                gridViewPrepare.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<V_HIS_PREPARE>>
                    ("api/HisPrepare/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlPrepare.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlPrepare.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControlPrepare.DataSource = null;
                }
                gridViewPrepare.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewPrepare.EndUpdate();
            }
        }

        private void SetFilter(ref HisPrepareViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                if (DtTimeFrom.EditValue != null && DtTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(DtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                if (DtTimeTo.EditValue != null && DtTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(DtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");

                if (!String.IsNullOrEmpty(TxtKeyword.Text))
                {
                    filter.KEY_WORD = TxtKeyword.Text.Trim();
                }

                if (Medistock == null)//khong phai kho
                {
                    filter.CREATOR = LoggingName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPrepare_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_PREPARE data = (V_HIS_PREPARE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "PATIENT_DOB")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TO_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TO_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "FROM_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.FROM_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_NAME")
                        {
                            e.Value = DisplayName(data.APPROVAL_LOGINNAME, data.APPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "REQ_NAME")
                        {
                            e.Value = DisplayName(data.REQ_LOGINNAME, data.REQ_USERNAME);
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

        private void gridViewPrepare_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string approvalLoginname = (View.GetRowCellValue(e.RowHandle, "APPROVAL_LOGINNAME") ?? "").ToString();
                    string reqLoginname = (View.GetRowCellValue(e.RowHandle, "REQ_LOGINNAME") ?? "").ToString();
                    string creator = (View.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();

                    if (e.Column.FieldName == "Edit")
                    {
                        if (creator == LoggingName && String.IsNullOrWhiteSpace(approvalLoginname))
                        {
                            e.RepositoryItem = repositoryItemBtnEdit;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnEditDisable;
                        }
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        if (creator == LoggingName && String.IsNullOrWhiteSpace(approvalLoginname))
                        {
                            e.RepositoryItem = repositoryItemBtnDelete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnDeleteDisable;
                        }
                    }
                    else if (e.Column.FieldName == "Approval")
                    {
                        if (String.IsNullOrWhiteSpace(approvalLoginname) && this.Medistock != null)
                        {
                            e.RepositoryItem = repositoryItemBtnApproval;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnApprovalDisable;
                        }
                    }
                    else if (e.Column.FieldName == "DisApproval")
                    {
                        if (!String.IsNullOrWhiteSpace(approvalLoginname) && LoggingName == approvalLoginname)
                        {
                            e.RepositoryItem = repositoryItemBtnDisapproval;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnDisapprovalDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
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

        private void BtnRefresh_Click(object sender, EventArgs e)
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

        private void BtnApproval_Click(object sender, EventArgs e)
        {
            try
            {
                BtnApproval.Focus();
                if (!BtnApproval.Enabled || LciBtnApproval.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always) return;

                List<V_HIS_PREPARE> listApproval = new List<V_HIS_PREPARE>();
                for (int i = 0; i < gridViewPrepare.GetSelectedRows().Count(); i++)
                {
                    listApproval.Add((V_HIS_PREPARE)gridViewPrepare.GetRow(gridViewPrepare.GetSelectedRows()[i]));
                }

                if (listApproval.Count > 0)
                {
                    List<string> errorApproval = new List<string>();
                    foreach (var item in listApproval)
                    {
                        if (item.APPROVAL_TIME.HasValue || !String.IsNullOrWhiteSpace(item.APPROVAL_LOGINNAME))
                        {
                            errorApproval.Add(item.PREPARE_CODE);
                        }
                    }

                    if (errorApproval.Count <= 0)
                    {
                        HisPrepareApproveListSDO sdo = new HisPrepareApproveListSDO();
                        sdo.Ids = listApproval.Select(s => s.ID).Distinct().ToList();
                        sdo.ReqRoomId = this.moduleData.RoomId;

                        //goi api duyet
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<List<HIS_PREPARE>>
                            ("api/HisPrepare/ApproveList", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null && apiresult.Count > 0)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceLanguageManager.CacPhieuDaDuyet, string.Join(",", errorApproval)));
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.BanChuaChonDuLieuDuyet);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //goi module xem
                var row = (V_HIS_PREPARE)gridViewPrepare.GetFocusedRow();
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.ID);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisPrepareDetail", moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_PREPARE)gridViewPrepare.GetFocusedRow();
                if (row != null)
                {
                    if (row.CREATOR == LoggingName || String.IsNullOrWhiteSpace(row.APPROVAL_LOGINNAME))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(
                           Resources.ResourceLanguageManager.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                           Resources.ResourceLanguageManager.ThongBao,
                           MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            WaitingManager.Show();
                            CommonParam param = new CommonParam();
                            bool success = false;
                            var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                (param).Post<bool>
                                ("api/HisPrepare/Delete", ApiConsumer.ApiConsumers.MosConsumer, row.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            if (apiresult)
                            {
                                success = true;
                                FillDataToGrid();
                            }
                            WaitingManager.Hide();
                            #region Show message
                            MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //goi module duyet
                var row = (V_HIS_PREPARE)gridViewPrepare.GetFocusedRow();
                if (row != null)
                {
                    if (String.IsNullOrWhiteSpace(row.APPROVAL_LOGINNAME) && this.Medistock != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.ID);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)Search);

                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisPrepareApprove", moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDisapproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //huy duyet
                var row = (V_HIS_PREPARE)gridViewPrepare.GetFocusedRow();
                if (row != null)
                {
                    if (!String.IsNullOrWhiteSpace(row.APPROVAL_LOGINNAME) && this.Medistock != null && LoggingName == row.APPROVAL_LOGINNAME)
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        HisPrepareSDO sdo = new HisPrepareSDO();
                        sdo.Id = row.ID;
                        sdo.ReqRoomId = this.moduleData.RoomId;

                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<HIS_PREPARE>
                            ("api/HisPrepare/Unapprove", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //goi module sua
                var row = (V_HIS_PREPARE)gridViewPrepare.GetFocusedRow();
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS_PREPARE prepare = new HIS_PREPARE();
                    prepare.ID = row.ID;
                    listArgs.Add(prepare);
                    listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)Search);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Prepare", moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Search()
        {
            try
            {
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Refreshs()
        {
            try
            {
                BtnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPrepare_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridHitInfo hitInfo = gridViewPrepare.CalcHitInfo(e.Location);
                if (hitInfo.InRowCell)
                {
                    gridViewPrepare.FocusedColumn = hitInfo.Column;
                    gridViewPrepare.FocusedRowHandle = hitInfo.RowHandle;
                    gridViewPrepare.ShowEditor();

                    if (hitInfo.Column.Name == Gc_Approval.Name)
                    {
                        repositoryItemBtnApproval_ButtonClick(null, null);
                    }
                    else if (hitInfo.Column.Name == Gc_Delete.Name)
                    {
                        repositoryItemBtnDelete_ButtonClick(null, null);
                    }
                    else if (hitInfo.Column.Name == Gc_DisApproval.Name)
                    {
                        repositoryItemBtnDisapproval_ButtonClick(null, null);
                    }
                    else if (hitInfo.Column.Name == Gc_Edit.Name)
                    {
                        repositoryItemBtnEdit_ButtonClick(null, null);
                    }
                    else if (hitInfo.Column.Name == Gc_View.Name)
                    {
                        repositoryItemBtnView_ButtonClick(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
