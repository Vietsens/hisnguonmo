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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Plugins.LisDeliveryNoteList.ADO;
using Inventec.Common.Controls.EditorLoader;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using HIS.Desktop.Plugins.LisDeliveryNoteList.Utils;
using DevExpress.Data;
namespace HIS.Desktop.Plugins.LisDeliveryNoteList.Run
{
    public partial class UCLisDeliveryNoteList : UserControl
    {
        #region Derlare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        List<StatusAdo> statusSelecteds;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        string roomCode { get; set; }
        int lastRowHandle = -1;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        #endregion

        public UCLisDeliveryNoteList(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCLisDeliveryNoteList_Load(object sender, EventArgs e)
        {
            try
            {
                SetLanguage();
                InitCombo();
                SetDefaultValue();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetLanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.LisDeliveryNoteList.Resources.Lang", typeof(HIS.Desktop.Plugins.LisDeliveryNoteList.Run.UCLisDeliveryNoteList).Assembly);

                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKey.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("txtKey.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtDeliveryNoteCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("txtDeliveryNoteCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("layoutControlItem3.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                dteCreateFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartDay() ?? 0));
                dteCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                dteReceivingFrom.EditValue = null;
                dteReceivingTo.EditValue = null;
                cboSampleRoom.EditValue = null;
                txtDeliveryNoteCode.Text = "";
                txtKey.Text = "";

                GridCheckMarksSelection gridCheckMark = cboStatusDe.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboStatusDe.Properties.View);
                    cboStatusDe.EditValue = null;
                    cboStatusDe.Focus();
                    gridCheckMark.SelectAll(cboStatusDe.Properties.DataSource);
                }
                cboStatusDe.Properties.Buttons[1].Visible = (statusSelecteds != null && statusSelecteds.Count > 0);
                cboSampleRoom.Properties.Buttons[1].Visible = false;
                cboStatusDe.Focus();
                txtDeliveryNoteCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboStatusCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboStatusDe.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboStatusDe.Properties.Tag = gridCheck;
                cboStatusDe.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboStatusDe.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboStatusDe.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Event_Check(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                statusSelecteds = new List<StatusAdo>();
                if (gridCheckMark != null)
                {
                    foreach (StatusAdo er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.Name);
                            this.statusSelecteds.Add(er);
                        }
                    }
                }
                this.cboStatusDe.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboStatus()
        {
            try
            {
                cboStatusDe.Properties.DataSource = GetStt();
                cboStatusDe.Properties.DisplayMember = "Name";
                cboStatusDe.Properties.ValueMember = "id";
                DevExpress.XtraGrid.Columns.GridColumn column = cboStatusDe.Properties.View.Columns.AddField("Name");
                column.VisibleIndex = 1;
                column.Width = 150;
                column.Caption = "Tất cả";
                cboStatusDe.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboStatusDe.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<StatusAdo> GetStt()
        {
            List<StatusAdo> list = new List<StatusAdo>();
            try
            {
                list.Add(new StatusAdo("Tạo mới", 1));
                list.Add(new StatusAdo("Đang nhận", 2));
                list.Add(new StatusAdo("Đã nhận", 3));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
            return list;
        }

        private void InitComboSampleRoom()
        {
            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SAMPLE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SAMPLE_ROOM_CODE", "Mã", 30, 1));
                columnInfos.Add(new ColumnInfo("SAMPLE_ROOM_NAME", "Tên", 120, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SAMPLE_ROOM_NAME", "SAMPLE_ROOM_CODE", columnInfos, true, 150);
                ControlEditorLoader.Load(cboSampleRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo()
        {
            try
            {
                InitComboStatusCheck();
                InitComboStatus();
                InitComboSampleRoom();
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
                roomCode = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId).ROOM_CODE;

                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadGridData(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_LIS_DELIVERY_NOTE>> apiResult = null;
                LisDeliveryNoteViewFilter filter = new LisDeliveryNoteViewFilter();
                SetFilter(ref filter);
                gridView1.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_LIS_DELIVERY_NOTE>>("api/LisDeliveryNote/GetView", ApiConsumers.LisConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl1.DataSource = data;
                    }
                    else
                    {
                        gridControl1.DataSource = null;

                    }
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
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

        private void SetFilter(ref LIS.Filter.LisDeliveryNoteViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!string.IsNullOrEmpty(txtDeliveryNoteCode.Text))
                {
                    //    string code = txtDeliveryNoteCode.Text.Trim();
                    //    if (code.Length < 10 && checkDigit(code))
                    //    {
                    //        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    //        txtDeliveryNoteCode.Text = code;
                    //    }
                    filter.DELIVERY_NOTE_CODE__EXACT = txtDeliveryNoteCode.Text;
                }
                filter.RECEIVE_ROOM_CODE__OR__SEND_ROOM_CODE__EXACT = roomCode;
                filter.KEY_WORD = txtKey.Text.Trim();
                if (dteReceivingFrom.EditValue != null && dteReceivingFrom.DateTime != DateTime.MinValue)
                    filter.RECEIVING_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteReceivingFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dteReceivingTo.EditValue != null && dteReceivingTo.DateTime != DateTime.MinValue)
                    filter.RECEIVING_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteReceivingTo.EditValue).ToString("yyyyMMddHHmm") + "59");
                if (dteCreateFrom.EditValue != null && dteCreateFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteCreateFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dteCreateTo.EditValue != null && dteCreateTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteCreateTo.EditValue).ToString("yyyyMMddHHmm") + "59");
                if (cboSampleRoom.EditValue != null)
                {
                    filter.SEND_ROOM_CODE__EXACT = cboSampleRoom.EditValue.ToString();
                }
                if (this.statusSelecteds != null && this.statusSelecteds.Count() > 0)
                {
                    filter.DELIVERY_NOTE_STATUSES = this.statusSelecteds.Select(o => o.id).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleRoomList_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.statusSelecteds != null && this.statusSelecteds.Count > 0)
                {
                    foreach (var item in this.statusSelecteds)
                    {
                        roomName += item.Name + ", ";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_LIS_DELIVERY_NOTE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging.pagingGrid.CurrentPage - 1) * (ucPaging.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "STATUS")
                    {
                        if (data.DELIVERY_NOTE_STATUS == 1)
                        {
                            e.Value = imageList1.Images[0];
                        }
                        else if (data.DELIVERY_NOTE_STATUS == 2)
                        {
                            e.Value = imageList1.Images[1];
                        }
                        else if (data.DELIVERY_NOTE_STATUS == 3)
                        {
                            e.Value = imageList1.Images[2];
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                    else if (e.Column.FieldName == "RECEIVING_TIME_str")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.RECEIVING_TIME ?? 0).Substring(0, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.RECEIVING_TIME ?? 0).Length - 3);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_str")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }

                }
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
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_LIS_DELIVERY_NOTE data = (V_LIS_DELIVERY_NOTE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "EDIT")
                    {
                        if (data.DELIVERY_NOTE_STATUS == 1 && data.SEND_ROOM_CODE == roomCode)
                            e.RepositoryItem = btnEditEditEnable;
                        else
                            e.RepositoryItem = btnEditEditDisable;

                    }

                    if (e.Column.FieldName == "DELETE")
                    {
                        if (data.DELIVERY_NOTE_STATUS == 1 && data.SEND_ROOM_CODE == roomCode)
                            e.RepositoryItem = btnEditDeleteEnable;
                        else
                            e.RepositoryItem = btnEditDeleteDisable;


                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl1)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl1.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "STATUS")
                            {
                                long? isConfirm = null;
                                if (!String.IsNullOrWhiteSpace((view.GetRowCellValue(lastRowHandle, "DELIVERY_NOTE_STATUS") ?? "").ToString()))
                                {
                                    isConfirm = Convert.ToInt64((view.GetRowCellValue(lastRowHandle, "DELIVERY_NOTE_STATUS") ?? "").ToString());
                                }
                                if (isConfirm == 1)
                                {
                                    text = "Tạo mới";
                                }
                                else if (isConfirm == 2)
                                {
                                    text = "Đang nhận";
                                }
                                else if (isConfirm == 3)
                                {
                                    text = "Đã nhận";
                                }
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

        private void btnEditDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_LIS_DELIVERY_NOTE)gridView1.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
                if (row != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có chắc muốn xóa phiếu giao nhận bệnh phẩm không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        WaitingManager.Show();
                        if (row != null)
                        {
                            var rs = new BackendAdapter(param).Post<bool>("api/LisDeliveryNote/Delete", ApiConsumers.LisConsumer, row.ID, param);
                            if (rs)
                            {
                                FillDataToGrid();
                                WaitingManager.Hide();
                            }
                            WaitingManager.Hide();
                            MessageManager.Show(this.ParentForm, param, rs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEditEditEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_LIS_DELIVERY_NOTE)gridView1.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.LisDeliveryNoteCreateUpdate").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.LisDeliveryNoteCreateUpdate'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        LIS_DELIVERY_NOTE dt = new LIS_DELIVERY_NOTE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_DELIVERY_NOTE>(dt, row);
                        listArgs.Add(dt);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)FillDataToGrid);
                        WaitingManager.Hide();
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        Inventec.Common.Logging.LogSystem.Debug("INPUT MODULE:___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dt), dt));
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleData.ExtensionInfo.Code + row.ID, "Tạo/Sửa phiếu giao nhận bệnh phẩm", (System.Windows.Forms.UserControl)extenceInstance, currentModule);

                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEditView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_LIS_DELIVERY_NOTE)gridView1.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    CallModule callModule = new CallModule("HIS.Desktop.Plugins.LisDeliveryNoteDetail", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                    WaitingManager.Hide();
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
                FillDataToGrid();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void txtDeliveryNoteCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteCreateFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dteCreateFrom.EditValue != null && dteCreateFrom.DateTime != DateTime.MinValue)
                    dteCreateFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(
                 Convert.ToDateTime(dteCreateFrom.EditValue).ToString("yyyyMMdd0000") + "00"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteCreateTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dteCreateTo.EditValue != null && dteCreateTo.DateTime != DateTime.MinValue)
                    dteCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(
                 Convert.ToDateTime(dteCreateTo.EditValue).ToString("yyyyMMdd2359") + "59"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteReceivingFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dteReceivingFrom.EditValue != null && dteReceivingFrom.DateTime != DateTime.MinValue)
                    dteReceivingFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(
                 Convert.ToDateTime(dteReceivingFrom.EditValue).ToString("yyyyMMdd0000") + "00"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteReceivingTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dteReceivingTo.EditValue != null && dteCreateTo.DateTime != DateTime.MinValue)
                    dteReceivingTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(
                 Convert.ToDateTime(dteReceivingTo.EditValue).ToString("yyyyMMdd2359") + "59"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatusDe_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboStatusDe.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboStatusDe.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboStatusDe.Properties.View);
                    }
                    statusSelecteds = null;
                    cboStatusDe.Properties.Buttons[1].Visible = (statusSelecteds != null && statusSelecteds.Count > 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboSampleRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboSampleRoom.EditValue = null;
                    cboSampleRoom.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSampleRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboSampleRoom.EditValue != null)
                {
                    cboSampleRoom.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatusDe_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboStatusDe.EditValue != null)
                {
                    this.cboStatusDe.Properties.Buttons[1].Visible = (statusSelecteds != null && statusSelecteds.Count > 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
