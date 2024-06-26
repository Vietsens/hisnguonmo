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
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.HoreHandover.Validation;
using HIS.Desktop.Plugins.HoreHandover.ADO;
using AutoMapper;
using MOS.SDO;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigSystem;

namespace HIS.Desktop.Plugins.HoreHandover
{
    public partial class UCHoreHandover : UserControlBase
    {
        private int positionHandleControl = -1;

        private int horeStart = 0;
        private int horeLimit = 0;
        private int horeRowCount = 0;
        private int horeDataTotal = 0;

        private long? hisHoreHandoverId = null;
        private HIS_HORE_HANDOVER horeHandover = null;

        private List<HisHoldReturnADO> listAdd = new List<HisHoldReturnADO>();
        private List<HIS_HORE_HOHA> includes = new List<HIS_HORE_HOHA>();

        private HisHoreHandoverResultSDO resultSDO = null;


        public UCHoreHandover(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        public UCHoreHandover(Inventec.Desktop.Common.Modules.Module module, long horeHandoverId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.hisHoreHandoverId = horeHandoverId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHoreHandover_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.listAdd = new List<HisHoldReturnADO>();
                this.includes = new List<HIS_HORE_HOHA>();
                this.LoadHoreHandover();
                this.LoadHoreHohaUpdate();
                this.ValidControl();
                this.InitComboReceiveRoom();
                this.SetDefaultValueControl();
                this.FillDataToGridHoldReturn();
                this.RefreshDataGridHandover();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadHoreHandover()
        {
            try
            {
                if (this.hisHoreHandoverId.HasValue)
                {
                    HisHoreHandoverFilter filter = new HisHoreHandoverFilter();
                    filter.ID = this.hisHoreHandoverId.Value;
                    List<HIS_HORE_HANDOVER> holdHandovers = new BackendAdapter(new CommonParam()).Get<List<HIS_HORE_HANDOVER>>("api/HisHoreHandover/Get", ApiConsumers.MosConsumer, filter, null);
                    if (holdHandovers != null && holdHandovers.Count == 1)
                    {
                        this.horeHandover = holdHandovers[0];
                    }
                    else
                    {
                        LogSystem.Error("Khong lay duoc HIS_HORE_HANDOVER theo id: " + this.hisHoreHandoverId.HasValue + ". " + LogUtil.TraceData("holdHandovers", holdHandovers));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadHoreHohaUpdate()
        {
            try
            {
                if (this.resultSDO != null)
                {
                    Mapper.CreateMap<V_HIS_HORE_HOHA, HIS_HORE_HOHA>();
                    foreach (var item in this.resultSDO.HoreHohas)
                    {
                        var ado = listAdd != null ? listAdd.FirstOrDefault(o => o.ID == item.HOLD_RETURN_ID) : null;
                        if (ado != null)
                        {
                            ado.HoreHoha = Mapper.Map<HIS_HORE_HOHA>(item);
                        }
                    }
                }
                else if (this.horeHandover != null)
                {
                    HisHoreHohaFilter hHFilter = new HisHoreHohaFilter();
                    hHFilter.HORE_HANDOVER_ID = this.horeHandover.ID;
                    List<HIS_HORE_HOHA> horeHohas = new BackendAdapter(new CommonParam()).Get<List<HIS_HORE_HOHA>>("api/HisHoreHoha/Get", ApiConsumers.MosConsumer, hHFilter, null);

                    if (horeHohas != null && horeHohas.Count > 0)
                    {
                        HisHoldReturnViewFilter hRFilter = new HisHoldReturnViewFilter();
                        hRFilter.IS_HANDOVERING = true;
                        hRFilter.RESPONSIBLE_ROOM_ID = this.currentModuleBase.RoomId;
                        List<V_HIS_HOLD_RETURN> holdReturns = new BackendAdapter(new CommonParam()).Get<List<V_HIS_HOLD_RETURN>>("api/HisHoldReturn/GetView", ApiConsumers.MosConsumer, hRFilter, null);
                        Mapper.CreateMap<V_HIS_HOLD_RETURN, HisHoldReturnADO>();
                        foreach (var item in horeHohas)
                        {
                            var hR = holdReturns != null ? holdReturns.FirstOrDefault(o => o.ID == item.HOLD_RETURN_ID) : null;
                            if (hR != null)
                            {
                                HisHoldReturnADO ado = Mapper.Map<HisHoldReturnADO>(hR);
                                ado.HoreHoha = item;
                                listAdd.Add(ado);
                            }
                        }
                    }

                    this.RefreshDataGridHandover();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                this.ValidReceiveRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidReceiveRoom()
        {
            try
            {
                ReceiveRoomValidationRule rule = new ReceiveRoomValidationRule();
                rule.txtReceiveRoomCode = txtReceiveRoomCode;
                rule.cboReceiveRoom = cboReceiveRoom;
                dxValidationProvider1.SetValidationRule(txtReceiveRoomCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboReceiveRoom()
        {
            try
            {
                List<V_HIS_ROOM> listRoom = null;
                if (this.horeHandover != null)
                {
                    listRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && (o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__KHO || o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN || o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TD)
                        && o.ID != this.horeHandover.SEND_ROOM_ID).ToList();
                }
                else
                {
                    listRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && (o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__KHO || o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN || o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TD)
                        && o.ID != this.currentModuleBase.RoomId).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboReceiveRoom, listRoom, controlEditorADO);
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
                SetDefaultHoldReturnControl();
                SetDefaultHandoverControl();
                SetDefaultComboReceiveRoom();
                btnSave.Enabled = true;
                btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultHandoverControl()
        {
            try
            {
                txtHandoverPatientCode.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultComboReceiveRoom()
        {
            try
            {
                if (this.horeHandover != null)
                {
                    cboReceiveRoom.EditValue = this.horeHandover.RECEIVE_ROOM_ID;
                }
                else
                {
                    cboReceiveRoom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultHoldReturnControl()
        {
            try
            {
                txtHoreKeyword.Text = "";
                txtHorePatientCode.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridHoldReturn()
        {
            try
            {
                int pagingSize = ucPagingHoldReturn.pagingGrid != null ? ucPagingHoldReturn.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridHoldReturnPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = this.horeRowCount;
                param.Count = this.horeDataTotal;
                ucPagingHoldReturn.Init(GridHoldReturnPaging, param, pagingSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridHoldReturnPaging(object param)
        {
            try
            {
                this.horeStart = ((CommonParam)param).Start ?? 0;
                this.horeLimit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(this.horeStart, this.horeLimit);
                List<V_HIS_HOLD_RETURN> listData = new List<V_HIS_HOLD_RETURN>();
                HisHoldReturnViewFilter filter = new HisHoldReturnViewFilter();

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "HOLD_TIME";
                filter.RESPONSIBLE_ROOM_ID = this.currentModuleBase.RoomId;
                if (this.includes != null && this.includes.Count > 0)
                {
                    filter.IS_NOT_HANDOVERING_OR_IDs = includes.Select(s => s.HOLD_RETURN_ID).ToList();
                }
                else
                {
                    filter.IS_HANDOVERING = false;
                }
                filter.HAS_RETURN_TIME = false;
                if (!String.IsNullOrWhiteSpace(txtHorePatientCode.Text))
                {
                    string code = txtHorePatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    }
                    txtHorePatientCode.Text = code;
                    filter.PATIENT_CODE__EXACT = code;
                }
                else
                {
                    filter.KEY_WORD = txtHoreKeyword.Text;
                }

                ApiResultObject<List<V_HIS_HOLD_RETURN>> rs = new BackendAdapter(paramCommon).GetRO<List<V_HIS_HOLD_RETURN>>("api/HisHoldReturn/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                if (rs != null)
                {
                    if (rs.Data != null)
                    {
                        listData = rs.Data;
                    }
                    this.horeRowCount = (listData == null ? 0 : listData.Count);
                    this.horeDataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }

                gridControlHoldReturn.BeginUpdate();
                gridControlHoldReturn.DataSource = listData;
                gridControlHoldReturn.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHorePatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnHoreFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHoreKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnHoreFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHoreFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnHoreFind.Enabled) return;
                WaitingManager.Show();
                this.FillDataToGridHoldReturn();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHoreRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnHoreRefresh.Enabled) return;
                WaitingManager.Show();
                this.SetDefaultHoldReturnControl();
                this.FillDataToGridHoldReturn();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHoldReturn_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_HOLD_RETURN data = (V_HIS_HOLD_RETURN)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + this.horeStart;
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            if (data.IS_HAS_NOT_DAY_DOB.HasValue && data.IS_HAS_NOT_DAY_DOB.Value == 1)
                            {
                                e.Value = data.DOB.ToString().Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                            }
                        }
                        else if (e.Column.FieldName == "HOLD_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.HOLD_TIME);
                        }
                        else if (e.Column.FieldName == "HOLD_USER")
                        {
                            e.Value = (data.HOLD_LOGINNAME ?? "") + " - " + (data.HOLD_USERNAME ?? "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHoldReturn_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "ADD") return;
                V_HIS_HOLD_RETURN data = (V_HIS_HOLD_RETURN)gridViewHoldReturn.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "ADD")
                    {
                        if (listAdd != null && listAdd.Any(a => a.ID == data.ID))
                        {
                            e.RepositoryItem = repositoryItemButton__Add__Disable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Add;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHoldReturn_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                V_HIS_HOLD_RETURN data = (V_HIS_HOLD_RETURN)gridViewHoldReturn.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (listAdd != null && listAdd.Any(a => a.ID == data.ID))
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                V_HIS_HOLD_RETURN data = (V_HIS_HOLD_RETURN)gridViewHoldReturn.GetFocusedRow();
                if (data != null && !listAdd.Any(a => a.ID == data.ID))
                {
                    Mapper.CreateMap<V_HIS_HOLD_RETURN, HisHoldReturnADO>();
                    HisHoldReturnADO row = Mapper.Map<HisHoldReturnADO>(data);
                    var hh = includes != null ? includes.FirstOrDefault(o => o.HOLD_RETURN_ID == row.ID) : null;
                    if (hh != null)
                    {
                        row.HoreHoha = hh;
                        includes.Remove(hh);
                    }
                    btnPrint.Enabled = false;
                    listAdd.Add(row);
                    this.RefreshDataGridHandover();
                }
                gridControlHoldReturn.RefreshDataSource();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHandoverPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.FillDataAndAddToHandover();
                    txtHandoverPatientCode.Text = "";
                    txtHandoverPatientCode.Focus();
                    txtHandoverPatientCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtReceiveRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        cboReceiveRoom.EditValue = null;
                        cboReceiveRoom.Focus();
                        cboReceiveRoom.ShowPopup();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ROOM_CODE.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.ROOM_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboReceiveRoom.EditValue = searchResult[0].ID;
                            this.txtReceiveRoomCode.Text = searchResult[0].ROOM_CODE;
                        }
                        else
                        {
                            cboReceiveRoom.EditValue = null;
                            cboReceiveRoom.Focus();
                            cboReceiveRoom.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReceiveRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

        }

        private void cboReceiveRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtReceiveRoomCode.Text = "";
                this.cboReceiveRoom.Properties.Buttons[1].Visible = false;
                if (this.cboReceiveRoom.EditValue != null)
                {
                    this.cboReceiveRoom.Properties.Buttons[1].Visible = true;
                    V_HIS_ROOM data = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == Convert.ToInt64(cboReceiveRoom.EditValue));
                    if (data != null)
                    {
                        this.txtReceiveRoomCode.Text = data.ROOM_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboReceiveRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboReceiveRoom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReceiveRoom_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void gridViewHoreHandover_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisHoldReturnADO data = (HisHoldReturnADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            if (data.IS_HAS_NOT_DAY_DOB.HasValue && data.IS_HAS_NOT_DAY_DOB.Value == 1)
                            {
                                e.Value = data.DOB.ToString().Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                            }
                        }
                        else if (e.Column.FieldName == "HOLD_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.HOLD_TIME);
                        }
                        else if (e.Column.FieldName == "HOLD_USER")
                        {
                            e.Value = (data.HOLD_LOGINNAME ?? "") + " - " + (data.HOLD_USERNAME ?? "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Remove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisHoldReturnADO row = (HisHoldReturnADO)gridViewHoreHandover.GetFocusedRow();
                if (row != null && listAdd.Any(a => a.ID == row.ID))
                {
                    listAdd.Remove(row);
                    if (row.HoreHoha != null)
                    {
                        this.includes.Add(row.HoreHoha);
                    }
                    btnPrint.Enabled = false;
                    this.RefreshDataGridHandover();
                    this.gridControlHoldReturn.RefreshDataSource();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataAndAddToHandover()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtHandoverPatientCode.Text))
                {
                    WaitingManager.Show();
                    HisHoldReturnViewFilter filter = new HisHoldReturnViewFilter();
                    filter.RESPONSIBLE_ROOM_ID = this.currentModuleBase.RoomId;
                    if (this.includes != null && this.includes.Count > 0)
                    {
                        filter.IS_NOT_HANDOVERING_OR_IDs = includes.Select(s => s.HOLD_RETURN_ID).ToList();
                    }
                    else
                    {
                        filter.IS_HANDOVERING = false;
                    }
                    filter.HAS_RETURN_TIME = false;
                    string code = txtHandoverPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    }
                    txtHandoverPatientCode.Text = code;
                    filter.PATIENT_CODE__EXACT = code;

                    List<V_HIS_HOLD_RETURN> rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_HOLD_RETURN>>("api/HisHoldReturn/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    if (rs != null && rs.Count == 1)
                    {
                        var add = rs.FirstOrDefault();
                        if (listAdd == null || !listAdd.Any(a => a.ID == add.ID))
                        {
                            Mapper.CreateMap<V_HIS_HOLD_RETURN, HisHoldReturnADO>();
                            HisHoldReturnADO row = Mapper.Map<HisHoldReturnADO>(add);
                            var hh = includes != null ? includes.FirstOrDefault(o => o.HOLD_RETURN_ID == row.ID) : null;
                            if (hh != null)
                            {
                                row.HoreHoha = hh;
                                includes.Remove(hh);
                            }
                            listAdd.Add(row);
                            btnPrint.Enabled = false;
                        }
                        this.RefreshDataGridHandover();
                        this.gridControlHoldReturn.RefreshDataSource();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        WaitingManager.Hide();
                        XtraMessageBox.Show(String.Format("Không tim được phiếu giữ giấy tờ của bệnh nhân có mã {0}. Vui lòng kiểm tra lại.", code), "Thống báo", DevExpress.Utils.DefaultBoolean.True);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshDataGridHandover()
        {
            try
            {
                gridControlHoreHandover.BeginUpdate();
                gridControlHoreHandover.DataSource = listAdd;
                gridControlHoreHandover.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate()) return;
                if (listAdd == null || listAdd.Count <= 0)
                {
                    XtraMessageBox.Show("Bạn chưa chọn giấy tờ bàn giao", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HisHoreHandoverCreateSDO sdo = new HisHoreHandoverCreateSDO();
                sdo.WorkingRoomId = this.currentModuleBase.RoomId;
                sdo.ReceiveRoomId = Convert.ToInt64(cboReceiveRoom.EditValue);
                if (this.resultSDO != null)
                {
                    sdo.Id = this.resultSDO.HoreHandover.ID;
                }
                else if (this.horeHandover != null)
                {
                    sdo.Id = this.horeHandover.ID;
                }
                sdo.HisHoldReturnIds = listAdd.Select(s => s.ID).ToList();
                HisHoreHandoverResultSDO rs = null;
                if (sdo.Id.HasValue && sdo.Id.Value > 0)
                {
                    rs = new BackendAdapter(param).Post<HisHoreHandoverResultSDO>("api/HisHoreHandover/Update", ApiConsumers.MosConsumer, sdo, param);
                }
                else
                {
                    rs = new BackendAdapter(param).Post<HisHoreHandoverResultSDO>("api/HisHoreHandover/Create", ApiConsumers.MosConsumer, sdo, param);
                }

                if (rs != null)
                {
                    success = true;
                    this.resultSDO = rs;
                    this.LoadHoreHohaUpdate();
                    btnPrint.Enabled = true;
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultSDO == null) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000326", delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultSDO == null) return false;
                MPS.Processor.Mps000326.PDO.Mps000326PDO rdo = new MPS.Processor.Mps000326.PDO.Mps000326PDO(this.resultSDO.HoreHandover, this.resultSDO.HoreHohas);

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled) return;
                WaitingManager.Show();
                this.listAdd = new List<HisHoldReturnADO>();
                this.includes = new List<HIS_HORE_HOHA>();
                this.horeHandover = null;
                this.hisHoreHandoverId = null;
                this.resultSDO = null;
                this.SetDefaultValueControl();
                this.FillDataAndAddToHandover();
                this.RefreshDataGridHandover();
                dxValidationProvider1.RemoveControlError(txtReceiveRoomCode);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FIND()
        {
            try
            {
                btnHoreFind_Click(null, null);
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
                btnHoreRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SAVE()
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void PRINT()
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void NEW()
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FOCUS()
        {
            try
            {
                txtHandoverPatientCode.Focus();
                txtHandoverPatientCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
