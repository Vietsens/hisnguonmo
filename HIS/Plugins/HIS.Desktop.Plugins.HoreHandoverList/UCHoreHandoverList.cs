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
using HIS.Desktop.Utility;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.ConfigSystem;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.HoreHandoverList
{
    public partial class UCHoreHandoverList : UserControlBase
    {
        private int start = 0;
        private int limit = 0;
        private int rowCount = 0;
        private int dataTotal = 0;

        private int lastRowHandle = -1;
        private GridColumn lastColumn = null;
        private ToolTipControlInfo lastInfo = null;

        public UCHoreHandoverList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void UCHoreHandoverList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                gridControlHoreHandover.ToolTipController = this.toolTipController1;
                this.SetDefaultControl();
                this.FillDataToGrid();
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
                txtHoreHandoverCode.Text = "";
                txtKeyword.Text = "";
                dtCreateTimeFrom.DateTime = DateTime.Now;
                dtCreateTimeTo.DateTime = DateTime.Now;
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
                int pagingSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(GridPaging, param, pagingSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                this.start = ((CommonParam)param).Start ?? 0;
                this.limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(this.start, this.limit);
                List<V_HIS_HORE_HANDOVER> listData = new List<V_HIS_HORE_HANDOVER>();
                HisHoreHandoverViewFilter filter = new HisHoreHandoverViewFilter();

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.SEND_OR_RECEIVE_ROOM_ID = this.currentModuleBase.RoomId;
                if (!String.IsNullOrWhiteSpace(txtHoreHandoverCode.Text))
                {
                    string code = txtHoreHandoverCode.Text.Trim();
                    if (code.Length < 8)
                    {
                        code = string.Format("{0:00000000}", Convert.ToInt64(code));
                    }
                    txtHoreHandoverCode.Text = code;
                    filter.HORE_HANDOVER_CODE__EXACT = code;
                }
                else
                {
                    filter.KEY_WORD = txtKeyword.Text;
                    if (checkReceive.Checked && !checkRequest.Checked)
                    {
                        filter.HORE_HANDOVER_STT_ID = IMSys.DbConfig.HIS_RS.HIS_HORE_HANDOVER_STT.ID__RECEIVE;
                    }
                    else if (checkRequest.Checked && !checkReceive.Checked)
                    {
                        filter.HORE_HANDOVER_STT_ID = IMSys.DbConfig.HIS_RS.HIS_HORE_HANDOVER_STT.ID__REQUEST;
                    }
                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_TIME_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_TIME_TO = Convert.ToInt64(dtCreateTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                    }
                }

                ApiResultObject<List<V_HIS_HORE_HANDOVER>> rs = new BackendAdapter(paramCommon).GetRO<List<V_HIS_HORE_HANDOVER>>("api/HisHoreHandover/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                if (rs != null)
                {
                    if (rs.Data != null)
                    {
                        listData = rs.Data;
                    }
                    this.rowCount = (listData == null ? 0 : listData.Count);
                    this.dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }

                gridControlHoreHandover.BeginUpdate();
                gridControlHoreHandover.DataSource = listData;
                gridControlHoreHandover.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
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
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHoreHandoverCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
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
                if (!btnFind.Enabled) return;
                WaitingManager.Show();
                FillDataToGrid();
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
                if (!btnRefresh.Enabled) return;
                WaitingManager.Show();
                this.SetDefaultControl();
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHoreHandover_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_HORE_HANDOVER data = (V_HIS_HORE_HANDOVER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + this.start;
                        }
                        else if (e.Column.FieldName == "IMAGE_STATUS")
                        {
                            if (data.HORE_HANDOVER_STT_ID == IMSys.DbConfig.HIS_RS.HIS_HORE_HANDOVER_STT.ID__REQUEST)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else
                            {
                                e.Value = imageListIcon.Images[3];
                            }
                        }
                        else if (e.Column.FieldName == "SEND_USER")
                        {
                            e.Value = (data.SEND_LOGINNAME ?? "") + " - " + (data.SEND_USERNAME ?? "");
                        }
                        else if (e.Column.FieldName == "RECEIVE_USER")
                        {
                            if (!String.IsNullOrWhiteSpace(data.RECEIVE_LOGINNAME))
                            {
                                e.Value = (data.RECEIVE_LOGINNAME ?? "") + " - " + (data.RECEIVE_USERNAME ?? "");
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHoreHandover_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                V_HIS_HORE_HANDOVER row = (V_HIS_HORE_HANDOVER)gridViewHoreHandover.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (row.HORE_HANDOVER_STT_ID == IMSys.DbConfig.HIS_RS.HIS_HORE_HANDOVER_STT.ID__REQUEST
                            && row.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                            && row.SEND_ROOM_ID == this.currentModuleBase.RoomId)
                        {
                            e.RepositoryItem = repositoryItemButtonDelete_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonDelete__Disable;
                        }
                    }
                    else if (e.Column.FieldName == "EDIT")
                    {
                        if (row.HORE_HANDOVER_STT_ID == IMSys.DbConfig.HIS_RS.HIS_HORE_HANDOVER_STT.ID__REQUEST
                            && row.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                            && row.SEND_ROOM_ID == this.currentModuleBase.RoomId)
                        {
                            e.RepositoryItem = repositoryItemButtonEdit__Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEdit__Disable;
                        }
                    }
                    else if (e.Column.FieldName == "RECEIVE")
                    {
                        if (row.RECEIVE_ROOM_ID == this.currentModuleBase.RoomId)
                        {
                            if (row.HORE_HANDOVER_STT_ID == IMSys.DbConfig.HIS_RS.HIS_HORE_HANDOVER_STT.ID__REQUEST)
                            {
                                e.RepositoryItem = repositoryItemButtonReceive__Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonUnreceive__Enable;
                            }
                        }
                        else
                        {
                            if (row.HORE_HANDOVER_STT_ID == IMSys.DbConfig.HIS_RS.HIS_HORE_HANDOVER_STT.ID__REQUEST)
                            {
                                e.RepositoryItem = repositoryItemButtonReceive__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonUnreceive__Disable;
                            }
                        }
                    }
                    else if (e.Column.FieldName == "PRINT")
                    {
                        e.RepositoryItem = repositoryItemButtonPrint;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_HORE_HANDOVER row = (V_HIS_HORE_HANDOVER)gridViewHoreHandover.GetFocusedRow();
                if (row != null)
                {
                    var yes = XtraMessageBox.Show("Bạn có muốn xóa phiếu bàn giao hay không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.True);
                    if (yes != DialogResult.Yes)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HisHoreHandoverSDO sdo = new HisHoreHandoverSDO();
                    sdo.Id = row.ID;
                    sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                    success = new BackendAdapter(param).Post<bool>("api/HisHoreHandover/Delete", ApiConsumers.MosConsumer, sdo, param);
                    if (success)
                    {
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit__Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_HORE_HANDOVER row = (V_HIS_HORE_HANDOVER)gridViewHoreHandover.GetFocusedRow();
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HoreHandover").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HoreHandover");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                        FillDataToGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonReceive__Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_HORE_HANDOVER row = (V_HIS_HORE_HANDOVER)gridViewHoreHandover.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HisHoreHandoverSDO sdo = new HisHoreHandoverSDO();
                    sdo.Id = row.ID;
                    sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                    var rs = new BackendAdapter(param).Post<HisHoreHandoverResultSDO>("api/HisHoreHandover/Receive", ApiConsumers.MosConsumer, sdo, param);
                    if (rs != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonUnreceive__Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_HORE_HANDOVER row = (V_HIS_HORE_HANDOVER)gridViewHoreHandover.GetFocusedRow();
                if (row != null)
                {
                    var yes = XtraMessageBox.Show("Bạn có muốn hủy tiếp nhận phiếu bàn giao hay không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.True);
                    if (yes != DialogResult.Yes)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HisHoreHandoverSDO sdo = new HisHoreHandoverSDO();
                    sdo.Id = row.ID;
                    sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                    var rs = new BackendAdapter(param).Post<HisHoreHandoverResultSDO>("api/HisHoreHandover/Unreceive", ApiConsumers.MosConsumer, sdo, param);
                    if (rs != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlHoreHandover)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlHoreHandover.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "IMAGE_STATUS")
                            {
                                //text = (view.GetRowCellValue(lastRowHandle, "SAMPLE_STT_NAME") ?? "").ToString();
                                var busyCount = ((V_HIS_HORE_HANDOVER)view.GetRow(lastRowHandle)).HORE_HANDOVER_STT_ID;
                                if (busyCount == IMSys.DbConfig.HIS_RS.HIS_HORE_HANDOVER_STT.ID__REQUEST)
                                {
                                    text = "Yêu cầu";
                                }
                                else if (busyCount == IMSys.DbConfig.HIS_RS.HIS_HORE_HANDOVER_STT.ID__RECEIVE)
                                {
                                    text = "Đã tiếp nhận";
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FIND()
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

        public void REFRESH()
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

        private void repositoryItemButtonPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_HORE_HANDOVER row = (V_HIS_HORE_HANDOVER)gridViewHoreHandover.GetFocusedRow();
                if (row != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate("Mps000326", delegatePrintTemplate);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                V_HIS_HORE_HANDOVER row = (V_HIS_HORE_HANDOVER)gridViewHoreHandover.GetFocusedRow();
                if (row != null)
                {
                    HisHoreHohaViewFilter filter = new HisHoreHohaViewFilter();
                    filter.HORE_HANDOVER_ID = row.ID;
                    List<V_HIS_HORE_HOHA> hisHoreHohas = new BackendAdapter(new CommonParam()).Get<List<V_HIS_HORE_HOHA>>("api/HisHoreHoha/GetView", ApiConsumers.MosConsumer, filter, null);

                    MPS.Processor.Mps000326.PDO.Mps000326PDO rdo = new MPS.Processor.Mps000326.PDO.Mps000326PDO(row, hisHoreHohas);

                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

    }
}
