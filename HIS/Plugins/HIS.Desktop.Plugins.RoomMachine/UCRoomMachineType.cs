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
using HIS.Desktop.Plugins.RoomMachine.entity;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.RoomMachine.entity;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.Data;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Controls.Session;

namespace HIS.Desktop.Plugins.RoomMachine
{
    public partial class UCRoomMachine : UserControl
    {
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        long RoomID = 0;
        long MachineID = 0;
        List<MachineADO> ListMachineADO = new List<MachineADO>();
        List<RoomADO> ListRoomADO = new List<RoomADO>();
        public UCRoomMachine()
        {
            InitializeComponent();
        }

        public void FindShortcut1()
        {
            try
            {
                btnFind1_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut2()
        {
            try
            {
                btnFind2_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveShortcut()
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCRoomMachine_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                grclCheck11.Image = imgStock.Images[1];
                grclCheck11.ImageAlignment = StringAlignment.Center;
                grclcheck21.Image = imgStock.Images[1];
                grclcheck21.ImageAlignment = StringAlignment.Center;
                cboChoose.EditValue = "Máy";
                LoadStatus();
                InitCombo();
                FilldataToGridMachine();
                FilldataToGridRoom();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCombo()
        {
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("ROOM_TYPE_CODE", "", 50, 1));
            columnInfos.Add(new ColumnInfo("ROOM_TYPE_NAME", "", 100, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_TYPE_NAME", "ID", columnInfos, false, 150);
            ControlEditorLoader.Load(cboRoomType, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE>(), controlEditorADO);
            cboRoomType.EditValue = 1;
        }

        private void LoadStatus()
        {
            if (cboChoose.EditValue == "Máy")
            {
                grclCheck12.OptionsColumn.AllowEdit = true;
                grclcheck21.OptionsColumn.AllowEdit = true;
                grclCheck11.OptionsColumn.AllowEdit = false;
                grclcheck22.OptionsColumn.AllowEdit = false;
                check21.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

            }
            else
            {
                grclCheck12.OptionsColumn.AllowEdit = false;
                grclcheck21.OptionsColumn.AllowEdit = false;
                grclCheck11.OptionsColumn.AllowEdit = true;
                grclcheck22.OptionsColumn.AllowEdit = true;
                check11.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            }
        }

        private void FilldataToGridRoom()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging2(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPaging2, param, numPageSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging2(object param)
        {

            try
            {
                start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>> apiResult = null;
                HisRoomFilter filter = new HisRoomFilter();
                SetFilterNavBar2(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                gridView2.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>("api/HisRoom/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_ROOM>)apiResult.Data;

                    if (data != null)
                    {
                        ListRoomADO = new List<entity.RoomADO>();
                        foreach (var item in data)
                        {
                            RoomADO x = new RoomADO();
                            x.ROOM_NAME = item.ROOM_NAME;
                            x.ROOM_CODE = item.ROOM_CODE;
                            x.ROOM_TYPE_NAME = item.ROOM_TYPE_NAME;
                            x.ID = item.ID;
                            x.check1 = false;
                            x.check2 = false;
                            ListRoomADO.Add(x);
                        }
                        gridView2.GridControl.DataSource = ListRoomADO.OrderByDescending(o => o.check1);
                        rowCount = (ListRoomADO == null ? 0 : ListRoomADO.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                        if (MachineID != 0)
                        {
                            RefeshGridRoom();
                            rowCount = (ListRoomADO == null ? 0 : ListRoomADO.Count);
                            dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        }
                    }
                }

                gridView2.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar2(ref HisRoomFilter filter)
        {
            filter.KEY_WORD = txtKeyword2.Text.Trim();
            filter.ROOM_TYPE_ID = Convert.ToInt32(cboRoomType.EditValue);
        }

        private void FilldataToGridMachine()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging1(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging1.Init(LoadPaging1, param, numPageSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging1(object param)
        {

            try
            {
                start1 = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start1, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_MACHINE>> apiResult = null;
                HisMachineFilter filter = new HisMachineFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_MACHINE>>("api/HisMachine/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_MACHINE>)apiResult.Data;

                    if (data != null)
                    {
                        ListMachineADO = new List<entity.MachineADO>();
                        foreach (var item in data)
                        {
                            MachineADO x = new MachineADO();
                            x.MACHINE_NAME = item.MACHINE_NAME;
                            x.MACHINE_CODE = item.MACHINE_CODE;
                            x.ID = item.ID;
                            x.check1 = false;
                            x.check2 = false;

                            ListMachineADO.Add(x);
                        }

                        gridView1.GridControl.DataSource = ListMachineADO.OrderByDescending(o => o.check1);
                        rowCount1 = (ListMachineADO == null ? 0 : ListMachineADO.Count);
                        dataTotal1 = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                        if (RoomID != 0)
                        {
                            RefeshGridMachine();
                        }
                    }
                }
                gridView1.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisMachineFilter filter)
        {
            filter.KEY_WORD = txtKeyword1.Text.Trim();
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FilldataToGridRoom();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FilldataToGridMachine();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                grclCheck11.Image = imgStock.Images[1];
                grclcheck21.Image = imgPatient.Images[1];
                MachineID = 0;
                RoomID = 0;
                LoadStatus();
                FilldataToGridRoom();
                FilldataToGridMachine();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridMachine();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridRoom();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Boolean success = false;
                List<HIS_ROOM_MACHINE> ListCreate = new List<HIS_ROOM_MACHINE>();
                List<long> ListDelete = new List<long>();
                CommonParam param = new CommonParam();
                if (RoomID == 0 && MachineID == 0)
                {
                    if (cboChoose.EditValue == "Phòng")
                    {
                        MessageBox.Show("chưa chọn phòng");
                        WaitingManager.Hide();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("chưa chọn máy");
                        WaitingManager.Hide();
                        return;
                    }
                }
                if (RoomID != 0)
                {
                    HisRoomMachineFilter filter = new HisRoomMachineFilter();
                    CommonParam paramCommon = new CommonParam();
                    List<HIS_ROOM_MACHINE> RoomMachine = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_ROOM_MACHINE>>("api/HisRoomMachine/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    List<long> ListMachineOld = new List<long>();
                    foreach (var item in RoomMachine)
                    {
                        if (item.ROOM_ID == RoomID)
                        {
                            ListMachineOld.Add(item.MACHINE_ID);
                        }
                    }
                    foreach (var item in ListMachineADO)
                    {
                        if (ListMachineOld.Contains(item.ID) && !item.check1)
                        {
                            var x = from item1 in RoomMachine
                                    where item1.MACHINE_ID == item.ID && item1.ROOM_ID == RoomID
                                    select item1;
                            HIS_ROOM_MACHINE o = x.FirstOrDefault();
                            long ID = o.ID;
                            ListDelete.Add(ID);
                        }
                        if ((!ListMachineOld.Contains(item.ID) && item.check1) || (ListMachineOld.Count == 0 && item.check1))
                        {
                            HIS_ROOM_MACHINE patientype = new HIS_ROOM_MACHINE();
                            patientype.ROOM_ID = RoomID;
                            patientype.MACHINE_ID = item.ID;
                            ListCreate.Add(patientype);
                        }
                    }
                    if (ListCreate.Count != null && ListCreate.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ROOM_MACHINE>("api/HisRoomMachine/CreateList", ApiConsumers.MosConsumer, ListCreate, param);
                        success = true;

                    }

                    else if (ListDelete.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<bool>("api/HisRoomMachine/DeleteList", ApiConsumers.MosConsumer, ListDelete, param);
                        if (resultData)
                        {
                            success = true;
                        }
                    }
                    else if (ListCreate.Count == 0 && ListDelete.Count == 0)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Chưa có thay đổi", "thông báo");

                        return;
                    }
                    if (success)
                    {
                        RefeshGridRoom();
                    }
                }
                else
                {
                    HisRoomMachineFilter filter = new HisRoomMachineFilter();
                    CommonParam paramCommon = new CommonParam();
                    List<HIS_ROOM_MACHINE> RoomMachine = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_ROOM_MACHINE>>("api/HisRoomMachine/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    List<long> ListRoomIDOld = new List<long>();
                    foreach (var item in RoomMachine)
                    {
                        if (item.MACHINE_ID == MachineID)
                        {
                            ListRoomIDOld.Add(item.ROOM_ID);
                        }
                    }
                    foreach (var item in ListRoomADO)
                    {
                        if (ListRoomIDOld.Contains(item.ID) && !item.check1)
                        {
                            var x = from item1 in RoomMachine
                                    where item1.MACHINE_ID == MachineID && item1.ROOM_ID == item.ID
                                    select item1;
                            HIS_ROOM_MACHINE o = x.FirstOrDefault();
                            long ID = o.ID;
                            ListDelete.Add(ID);
                        }
                        if ((!ListRoomIDOld.Contains(item.ID) && item.check1) || (ListRoomIDOld.Count == 0 && item.check1))
                        {
                            HIS_ROOM_MACHINE patientype = new HIS_ROOM_MACHINE();
                            patientype.ROOM_ID = item.ID;
                            patientype.MACHINE_ID = MachineID;
                            ListCreate.Add(patientype);
                        }
                    }
                    if (ListCreate.Count != null && ListCreate.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ROOM_MACHINE>("api/HisRoomMachine/CreateList", ApiConsumers.MosConsumer, ListCreate, param);
                        success = true;

                    }

                    else if (ListDelete.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<bool>("api/HisRoomMachine/DeleteList", ApiConsumers.MosConsumer, ListDelete, param);
                        if (resultData)
                        {
                            success = true;
                        }
                    }
                    else if (ListCreate.Count == 0 && ListDelete.Count == 0)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Chưa có thay đổi", "thông báo");

                        return;
                    }
                    if (success)
                    {
                        BackendDataWorker.Reset<HIS_ROOM_MACHINE>();
                        RefeshGridMachine();
                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                MachineADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (MachineADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "check11")
                    {
                        if (cboChoose.EditValue != "Máy")
                        {
                            e.RepositoryItem = check11;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheckEdit1;
                        }
                    }
                    else if (e.Column.FieldName == "check12")
                    {
                        e.RepositoryItem = check12;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MachineADO pData = (MachineADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "check12")
                    {
                        e.Value = pData.check2;
                    }
                    else if (e.Column.FieldName == "check11")
                    {
                        e.Value = pData.check1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    RoomADO pData = (RoomADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start1; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "check22")
                    {
                        e.Value = pData.check2;
                    }
                    else if (e.Column.FieldName == "check21")
                    {
                        e.Value = pData.check1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void check12_Click(object sender, EventArgs e)
        {
            var row = (MachineADO)gridView1.GetFocusedRow();
            if (row.check2 == true)
            {
                row.check2 = false;
                MachineID = 0;
                FilldataToGridRoom();
                return;
            }
            foreach (var item in ListMachineADO)
            {
                item.check2 = false;
                if (item.ID == row.ID)
                {
                    MachineID = row.ID;
                    item.check2 = true;
                }
            }
            gridControl1.DataSource = ListMachineADO;
            gridControl1.RefreshDataSource();
            RefeshGridRoom();
        }

        private void RefeshGridRoom()
        {
            List<long> ListRoomId = new List<long>();
            HisRoomMachineFilter filter = new HisRoomMachineFilter();
            CommonParam paramCommon = new CommonParam();
            List<HIS_ROOM_MACHINE> RoomMachine = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_ROOM_MACHINE>>("api/HisRoomMachine/Get", ApiConsumers.MosConsumer, filter, paramCommon);
            if (RoomMachine == null||RoomMachine.Count == 0)
            {
                foreach (var item in ListRoomADO)
                {
                    item.check1 = false;
                }
                return;
            }
            foreach (var item in RoomMachine)
            {
                if (item.MACHINE_ID == MachineID)
                {
                    ListRoomId.Add(item.ROOM_ID);
                }
            }
            foreach (var item in ListRoomADO)
            {
                item.check1 = false;
                foreach (var item1 in ListRoomId)
                {
                    if (item.ID == item1)
                    {
                        item.check1 = true;
                    }
                }
            }
            var data = ListRoomADO.OrderByDescending(o => o.check1);
            gridControl2.DataSource = null;
            gridControl2.DataSource = data;
            gridControl2.RefreshDataSource();
        }

        private void check21_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (RoomADO)gridView2.GetFocusedRow();
                if (row.check1)
                {
                    row.check1 = false;
                }
                else
                {
                    row.check1 = true;
                }

                foreach (var item in ListRoomADO)
                {
                    if (row.ID == item.ID)
                    {
                        item.check1 = row.check1;
                    }
                }
                //    gridControl2.DataSource = ListRoomADO;
                //    gridControl2.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void check22_Click(object sender, EventArgs e)
        {
            var row = (RoomADO)gridView2.GetFocusedRow();
            if (row.check2 == true)
            {
                row.check2 = false;
                RoomID = 0;
                FilldataToGridMachine();
                return;
            }
            foreach (var item in ListRoomADO)
            {
                item.check2 = false;
                if (item.ID == row.ID)
                {
                    RoomID = row.ID;
                    item.check2 = true;
                }
            }
            gridControl2.DataSource = ListRoomADO;
            gridControl2.RefreshDataSource();
            RefeshGridMachine();
        }

        private void RefeshGridMachine()
        {
            List<long> ListMachineId = new List<long>(); HisRoomMachineFilter filter = new HisRoomMachineFilter();
            CommonParam paramCommon = new CommonParam();
            List<HIS_ROOM_MACHINE> RoomMachine = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_ROOM_MACHINE>>("api/HisRoomMachine/Get", ApiConsumers.MosConsumer, filter, paramCommon);
            if (RoomMachine == null||RoomMachine.Count ==0)
            {
                foreach (var item in ListMachineADO)
                {
                    item.check1 = false;
                }
                return;
            }
            foreach (var item in RoomMachine)
            {
                if (item.ROOM_ID == RoomID)
                {
                    ListMachineId.Add(item.MACHINE_ID);
                }
            }
            foreach (var item in ListMachineADO)
            {
                item.check1 = false;
                foreach (var item1 in ListMachineId)
                {
                    if (item.ID == item1)
                    {
                        item.check1 = true;
                    }
                }
            }
            //var data = 
            // gridControl1.DataSource = null;
            gridControl1.DataSource = ListMachineADO.OrderByDescending(o => o.check1);
            gridControl1.RefreshDataSource();
        }

        private void check11_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MachineADO)gridView1.GetFocusedRow();
                if (row.check1)
                {
                    row.check1 = false;
                }
                else
                {
                    row.check1 = true;
                }

                foreach (var item in ListMachineADO)
                {
                    if (row.ID == item.ID)
                    {
                        item.check1 = row.check1;
                    }
                }
                //    gridControl1.DataSource = ListMachineADO;
                //    gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl2_Click(object sender, EventArgs e)
        {

        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (cboChoose.EditValue == "Máy")
                {
                    return;
                }
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check11")
                        {
                            if (grclCheck11.Image == imgStock.Images[1])
                            {
                                grclCheck11.Image = imgStock.Images[0];
                                foreach (var item in ListMachineADO)
                                {
                                    item.check1 = true;
                                }
                            }
                            else
                            {
                                grclCheck11.Image = imgStock.Images[1];
                                foreach (var item in ListMachineADO)
                                {
                                    item.check1 = false;
                                }
                            }
                            gridControl1.RefreshDataSource();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "check21")
                    {
                        if (cboChoose.EditValue == "Máy")
                        {
                            e.RepositoryItem = check21;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheckEdit2;
                        }
                    }
                    else if (e.Column.FieldName == "check22")
                    {
                        e.RepositoryItem = check22;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (cboChoose.EditValue == "Phòng")
                {
                    return;
                }
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check21")
                        {
                            if (grclcheck21.Image == imgPatient.Images[1])
                            {
                                grclcheck21.Image = imgPatient.Images[0];
                                foreach (var item in ListRoomADO)
                                {
                                    item.check1 = true;
                                }
                            }
                            else
                            {
                                grclcheck21.Image = imgPatient.Images[1];
                                foreach (var item in ListRoomADO)
                                {
                                    item.check1 = false;
                                }
                            }
                            gridControl2.RefreshDataSource();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridMachine();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridRoom();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
