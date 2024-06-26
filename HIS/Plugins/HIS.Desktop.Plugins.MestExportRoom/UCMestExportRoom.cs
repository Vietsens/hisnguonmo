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
using HIS.Desktop.Plugins.MestExportRoom.entity;
using HIS.UC.Room;
using HIS.UC.Room.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.UC.MediStock;
using HIS.UC.MediStock.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using MOS.SDO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid;

namespace HIS.Desktop.Plugins.MestExportRoom
{
    public partial class UCMestExportRoom : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_ROOM_TYPE> roomType { get; set; }
        UCRoomProcessor RoomProcessor;
        UCMediStockProcessor MediStockProcessor;
        UserControl ucGridControlRoom;
        UserControl ucGridControlMediStock;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.Room.RoomAccountADO> lstRoomADOs { get; set; }
        internal List<HIS.UC.MediStock.MediStockADO> lstMediStockADOs { get; set; }
        List<V_HIS_ROOM> listRoom;
        List<V_HIS_MEDI_STOCK> listMediStock;
        long roomIdCheckByRoom = 0;
        long mediStockIdCheck = 0;
        long isChooseRoom;
        long isChooseMediStock;
        bool isCheckAll;
        List<V_HIS_MEST_ROOM> mestRooms { get; set; }
        List<V_HIS_MEST_ROOM> mestRoomMediStocks { get; set; }

        HIS.UC.MediStock.MediStockADO currentCopyMedistockAdo;
        HIS.UC.Room.RoomAccountADO currentCopyRoomAccountAdo;

        public UCMestExportRoom(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
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

        private void UCMestExportRoom_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadDataToCombo();
                InitUCgridRoom();
                InitUCgridMediStock();
                FillDataToGridRoom(this);
                FillDataToGridMediStock(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisRoomTypeFilter RoomTypeFilter = new HisRoomTypeFilter();
                roomType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ROOM_TYPE>>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ROOM_TYPE_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    RoomTypeFilter,
                    param);
                LoadDataToComboRoomType(cboRoomType, roomType);
                LoadComboStatus();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Phòng"));
                status.Add(new Status(2, "Kho xuất"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                cboChoose.EditValue = status[1].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboRoomType(DevExpress.XtraEditors.GridLookUpEdit cboRoomType, List<HIS_ROOM_TYPE> roomType)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_TYPE_NAME", "", 400, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_TYPE_NAME", "ID", columnInfos, false, 500);
                Inventec.Common.Controls.EditorLoader.ControlEditorLoader.Load(cboRoomType, roomType, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMediStock(UCMestExportRoom uCMestExportRoom)
        {
            try
            {
                mediStockIdCheck = 0;
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridMediStockPage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging1.Init(FillDataToGridMediStockPage, param, numPageSize, (GridControl)this.MediStockProcessor.GetGridControl(this.ucGridControlMediStock));
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMediStockPage(object data)
        {
            try
            {
                WaitingManager.Show();
                listMediStock = new List<V_HIS_MEDI_STOCK>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisMediStockFilter MediStockFilter = new HisMediStockFilter();
                MediStockFilter.KEY_WORD = txtKeyword1.Text;
                MediStockFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                MediStockFilter.ORDER_DIRECTION = "ASC";
                MediStockFilter.ORDER_FIELD = "MEDI_STOCK_CODE";

                if ((long)cboChoose.EditValue == 2)
                {
                    isChooseMediStock = (long)cboChoose.EditValue;
                }

                var mest = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEDI_STOCK_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    MediStockFilter,
                    param);

                lstMediStockADOs = new List<MediStockADO>();
                if (mest != null && mest.Data.Count > 0)
                {
                    listMediStock = mest.Data;
                    foreach (var item in listMediStock)
                    {
                        MediStockADO MediStockADO = new MediStockADO(item);
                        if (isChooseMediStock == 2)
                        {
                            MediStockADO.isKeyChooseMest = true;
                        }
                        lstMediStockADOs.Add(MediStockADO);
                    }
                }

                if (mestRooms != null && mestRooms.Count > 0)
                {
                    foreach (var itemUsername in mestRooms)
                    {
                        var check = lstMediStockADOs.FirstOrDefault(o => o.ID == itemUsername.MEDI_STOCK_ID);
                        if (check != null)
                        {
                            check.checkMest = true;
                        }
                    }
                }
                lstMediStockADOs = lstMediStockADOs.OrderByDescending(p => p.checkMest).ToList();

                if (ucGridControlMediStock != null)
                {
                    MediStockProcessor.Reload(ucGridControlMediStock, lstMediStockADOs);
                }
                rowCount1 = (data == null ? 0 : lstMediStockADOs.Count);
                dataTotal1 = (mest.Param == null ? 0 : mest.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridRoom(UCMestExportRoom uCMestExportRoom)
        {
            try
            {
                roomIdCheckByRoom = 0;
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridRoomPage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridRoomPage, param, numPageSize, (GridControl)RoomProcessor.GetGridControl(this.ucGridControlRoom));
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridRoomPage(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoom = new List<V_HIS_ROOM>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisRoomFilter RoomFilter = new HisRoomFilter();
                RoomFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                RoomFilter.ORDER_FIELD = "DEPARTMENT_NAME";
                RoomFilter.ORDER_DIRECTION = "ASC";
                RoomFilter.KEY_WORD = txtKeyword2.Text;
                if (cboRoomType.EditValue != null)
                    RoomFilter.ROOM_TYPE_ID = (long)cboRoomType.EditValue;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChooseRoom = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ROOM_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    RoomFilter,
                    param);

                lstRoomADOs = new List<RoomAccountADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listRoom = rs.Data;
                    foreach (var item in listRoom)
                    {
                        RoomAccountADO RoomAccountADO = new RoomAccountADO(item);
                        if (isChooseRoom == 1)
                        {
                            RoomAccountADO.isKeyChoose = true;
                        }
                        lstRoomADOs.Add(RoomAccountADO);
                    }
                }

                if (mestRoomMediStocks != null && mestRoomMediStocks.Count > 0)
                {
                    foreach (var itemUsername in mestRoomMediStocks)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).ToList();

                if (ucGridControlRoom != null)
                {
                    RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                }
                rowCount = (data == null ? 0 : lstRoomADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCgridMediStock()
        {
            try
            {
                MediStockProcessor = new UCMediStockProcessor();
                MediStockInitADO ado = new MediStockInitADO();
                ado.ListMediStockColumn = new List<MediStockColumn>();
                ado.gridViewMediStock_MouseDownMest = gridViewMediStock_MouseDownMest;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.GridView_MouseRightClick = MedistockGridView_MouseRightClick;

                MediStockColumn colRadio2 = new MediStockColumn("   ", "radioMest", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMediStockColumn.Add(colRadio2);

                MediStockColumn colCheck2 = new MediStockColumn("   ", "checkMest", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollectionMediStock.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMediStockColumn.Add(colCheck2);

                MediStockColumn colMaKhoXuat = new MediStockColumn("Mã kho xuất", "MEDI_STOCK_CODE", 60, false);
                colMaKhoXuat.VisibleIndex = 2;
                ado.ListMediStockColumn.Add(colMaKhoXuat);

                MediStockColumn colTenKhoXuat = new MediStockColumn("Tên kho xuất", "MEDI_STOCK_NAME", 100, false);
                colTenKhoXuat.VisibleIndex = 3;
                ado.ListMediStockColumn.Add(colTenKhoXuat);

                MediStockColumn colNguoiTao = new MediStockColumn("Người tạo", "CREATOR", 100, false);
                colNguoiTao.VisibleIndex = 4;
                ado.ListMediStockColumn.Add(colNguoiTao);

                MediStockColumn colNguoiSua = new MediStockColumn("Người sửa", "MODIFIER", 100, false);
                colNguoiSua.VisibleIndex = 5;
                ado.ListMediStockColumn.Add(colNguoiSua);

                this.ucGridControlMediStock = (UserControl)MediStockProcessor.Run(ado);

                if (ucGridControlMediStock != null)
                {
                    this.panelControlMest.Controls.Add(this.ucGridControlMediStock);
                    this.ucGridControlMediStock.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStock_MouseDownMest(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChooseMediStock == 2)
                {
                    return;
                }
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "checkMest")
                        {
                            var lstCheckAll = lstMediStockADOs;
                            List<HIS.UC.MediStock.MediStockADO> lstChecks = new List<HIS.UC.MediStock.MediStockADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkMest = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkMest = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                //ReloadData
                                MediStockProcessor.Reload(ucGridControlMediStock, lstChecks);
                                //??

                            }
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click1(V_HIS_MEDI_STOCK data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMestRoomFilter mestRoomFilter = new HisMestRoomFilter();
                mestRoomFilter.MEDI_STOCK_ID = data.ID;
                mediStockIdCheck = data.ID;

                mestRoomMediStocks = new BackendAdapter(param).Get<List<V_HIS_MEST_ROOM>>(
                    "api/HisMestRoom/GetView",
                   ApiConsumers.MosConsumer,
                   mestRoomFilter,
                   param);
                List<HIS.UC.Room.RoomAccountADO> dataNew = new List<HIS.UC.Room.RoomAccountADO>();

                dataNew = (from r in listRoom select new RoomAccountADO(r)).ToList();
                if (mestRoomMediStocks != null && mestRoomMediStocks.Count > 0)
                {
                    foreach (var itemUsername in mestRoomMediStocks)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlRoom != null)
                {
                    RoomProcessor.Reload(ucGridControlRoom, dataNew);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCgridRoom()
        {
            try
            {
                RoomProcessor = new UCRoomProcessor();
                RoomInitADO ado = new RoomInitADO();
                ado.ListRoomColumn = new List<RoomColumn>();
                ado.gridViewRoom_MouseDownRoom = gridViewRoom_MouseDownRoom;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.rooom_MouseRightClick = GridViewRooom_MouseRightClick;

                RoomColumn colRadio1 = new RoomColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colRadio1);

                RoomColumn colCheck1 = new RoomColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionRoom.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colCheck1);

                RoomColumn colMaPhong = new RoomColumn("Mã phòng", "ROOM_CODE", 60, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListRoomColumn.Add(colMaPhong);

                RoomColumn colTenPhong = new RoomColumn("Tên phòng", "ROOM_NAME", 100, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListRoomColumn.Add(colTenPhong);

                RoomColumn colLoaiPhong = new RoomColumn("Loại phòng", "ROOM_TYPE_NAME", 100, false);
                colLoaiPhong.VisibleIndex = 4;
                ado.ListRoomColumn.Add(colLoaiPhong);

                RoomColumn colKhoa = new RoomColumn("Khoa", "DEPARTMENT_NAME", 100, false);
                colKhoa.VisibleIndex = 5;
                ado.ListRoomColumn.Add(colKhoa);

                this.ucGridControlRoom = (UserControl)RoomProcessor.Run(ado);
                if (ucGridControlRoom != null)
                {
                    this.panelControlRoom.Controls.Add(this.ucGridControlRoom);
                    this.ucGridControlRoom.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoom_MouseDownRoom(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChooseRoom == 1)
                {
                    return;
                }

                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check1")
                        {
                            var lstCheckAll = lstRoomADOs;
                            List<HIS.UC.Room.RoomAccountADO> lstChecks = new List<HIS.UC.Room.RoomAccountADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                RoomProcessor.Reload(ucGridControlRoom, lstChecks);
                                //??

                            }
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click(V_HIS_ROOM data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMestRoomFilter mestRoomFilter = new HisMestRoomFilter();
                mestRoomFilter.ROOM_ID = data.ID;
                roomIdCheckByRoom = data.ID;

                mestRooms = new BackendAdapter(param).Get<List<V_HIS_MEST_ROOM>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEST_ROOM_GETVIEW,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                mestRoomFilter,
                                param);
                List<HIS.UC.MediStock.MediStockADO> dataNew = new List<HIS.UC.MediStock.MediStockADO>();
                dataNew = (from r in listMediStock select new MediStockADO(r)).ToList();
                if (mestRooms != null && mestRooms.Count > 0)
                {
                    foreach (var itemUsername in mestRooms)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.MEDI_STOCK_ID);
                        if (check != null)
                        {
                            check.checkMest = true;
                        }
                    }
                }

                dataNew = dataNew.OrderByDescending(p => p.checkMest).ToList();
                if (ucGridControlMediStock != null)
                {
                    MediStockProcessor.Reload(ucGridControlMediStock, dataNew);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridRoom(this);
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
                FillDataToGridMediStock(this);
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

                isChooseMediStock = 0;
                isChooseRoom = 0;
                roomIdCheckByRoom = 0;
                mediStockIdCheck = 0;
                mestRoomMediStocks = null;
                mestRooms = null;
                FillDataToGridMediStock(this);
                FillDataToGridRoom(this);
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
                    FillDataToGridMediStock(this);
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
                    FillDataToGridRoom(this);
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
                if (ucGridControlMediStock != null && ucGridControlRoom != null)
                {
                    object mediStock = MediStockProcessor.GetDataGridView(ucGridControlMediStock);
                    object room = RoomProcessor.GetDataGridView(ucGridControlRoom);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChooseRoom == 1)
                    {
                        if (roomIdCheckByRoom > 0)
                        {
                            if (mediStock is List<HIS.UC.MediStock.MediStockADO>)
                            {
                                var data = (List<HIS.UC.MediStock.MediStockADO>)mediStock;

                                if (data != null && data.Count > 0)
                                {
                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisMestRoomFilter filter = new HisMestRoomFilter();
                                    filter.ROOM_ID = roomIdCheckByRoom;

                                    //var mestRooms = new BackendAdapter(param).Get<List<V_HIS_MEST_ROOM>>(
                                    //   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEST_ROOM_GETVIEW,
                                    //   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    //   filter,
                                    //   param);

                                    var listMediStock = this.mestRooms.Select(p => p.MEDI_STOCK_ID).ToList();

                                    var dataCheckeds = data.Where(p => p.checkMest == true).ToList();

                                    //List xoa

                                    var dataDeletes = data.Where(o => this.mestRooms.Select(p => p.MEDI_STOCK_ID)
                                        .Contains(o.ID) && o.checkMest == false).ToList();

                                    //list them
                                    var dataCreates = dataCheckeds.Where(o => !this.mestRooms.Select(p => p.MEDI_STOCK_ID)
                                        .Contains(o.ID)).ToList();

                                    if (dataDeletes != null && dataDeletes.Count > 0)
                                    {
                                        List<long> deleteIds = this.mestRooms.Where(o => dataDeletes.Select(p => p.ID)
                                            .Contains(o.MEDI_STOCK_ID)).Select(o => o.ID).ToList();
                                        WaitingManager.Show();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "api/HisMestRoom/DeleteList",
                                                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                  deleteIds,
                                                  param);
                                        WaitingManager.Hide();
                                        if (deleteResult)
                                        {
                                            success = true;
                                            this.mestRooms = this.mestRooms.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                        }
                                    }

                                    if (dataCreates != null && dataCreates.Count > 0)
                                    {
                                        List<V_HIS_MEST_ROOM> MestRoomCreates = new List<V_HIS_MEST_ROOM>();
                                        foreach (var item in dataCreates)
                                        {
                                            V_HIS_MEST_ROOM mestRoom = new V_HIS_MEST_ROOM();
                                            mestRoom.MEDI_STOCK_ID = item.ID;
                                            mestRoom.ROOM_ID = roomIdCheckByRoom;
                                            MestRoomCreates.Add(mestRoom);
                                        }
                                        WaitingManager.Show();
                                        var createResult = new BackendAdapter(param).Post<List<V_HIS_MEST_ROOM>>(
                                                   "api/HisMestRoom/CreateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   MestRoomCreates,
                                                   param);
                                        WaitingManager.Hide();
                                        if (createResult != null && createResult.Count > 0)
                                        {
                                            success = true;
                                            this.mestRooms.AddRange(createResult);
                                        }
                                    }

                                    if ((dataDeletes == null && dataCreates == null) || (dataDeletes.Count == 0 && dataCreates.Count == 0))
                                    {

                                        if (dataCheckeds != null && dataCheckeds.Count > 0)
                                        {
                                            success = true;
                                        }
                                        else
                                        {
                                            WaitingManager.Hide();
                                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn kho", "Thông báo");
                                            return;
                                        }
                                    }

                                    data = data.OrderByDescending(p => p.checkMest).ToList();
                                    if (ucGridControlMediStock != null)
                                    {
                                        MediStockProcessor.Reload(ucGridControlMediStock, data);
                                    }
                                }
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng", "Thông báo");
                            return;
                        }
                    }
                    if (isChooseMediStock == 2)
                    {
                        if (mediStockIdCheck > 0)
                        {
                            if (room is List<HIS.UC.Room.RoomAccountADO>)
                            {
                                var data = (List<HIS.UC.Room.RoomAccountADO>)room;

                                if (data != null && data.Count > 0)
                                {
                                    //bool success = false;
                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisMestRoomFilter filter = new HisMestRoomFilter();
                                    filter.MEDI_STOCK_ID = mediStockIdCheck;
                                    //var mestRooms = new BackendAdapter(param).Get<List<V_HIS_MEST_ROOM>>(
                                    //   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEST_ROOM_GETVIEW,
                                    //   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    //   filter,
                                    //   param);

                                    var listRoomID = mestRoomMediStocks.Select(p => p.ROOM_ID).ToList();

                                    var dataChecked = data.Where(p => p.check1 == true).ToList();

                                    //List xoa

                                    var dataDelete = data.Where(o => mestRoomMediStocks.Select(p => p.ROOM_ID)
                                        .Contains(o.ID) && o.check1 == false).ToList();

                                    //list them
                                    var dataCreate = dataChecked.Where(o => !mestRoomMediStocks.Select(p => p.ROOM_ID)
                                        .Contains(o.ID)).ToList();

                                    if (dataDelete != null && dataDelete.Count > 0)
                                    {
                                        List<long> deleteId = mestRoomMediStocks.Where(o => dataDelete.Select(p => p.ID)
                                            .Contains(o.ROOM_ID)).Select(o => o.ID).ToList();
                                        WaitingManager.Show();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "api/HisMestRoom/DeleteList",
                                                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                  deleteId,
                                                  param);
                                        WaitingManager.Hide();
                                        if (deleteResult)
                                        {
                                            success = true;
                                            mestRoomMediStocks = mestRoomMediStocks.Where(o => !deleteId.Contains(o.ID)).ToList();
                                        }
                                    }

                                    if (dataCreate != null && dataCreate.Count > 0)
                                    {
                                        List<V_HIS_MEST_ROOM> mestRoomCreate = new List<V_HIS_MEST_ROOM>();
                                        foreach (var item in dataCreate)
                                        {
                                            V_HIS_MEST_ROOM mestRoom = new V_HIS_MEST_ROOM();
                                            mestRoom.ROOM_ID = item.ID;
                                            mestRoom.MEDI_STOCK_ID = mediStockIdCheck;
                                            mestRoomCreate.Add(mestRoom);
                                        }
                                        WaitingManager.Show();
                                        var createResult = new BackendAdapter(param).Post<List<V_HIS_MEST_ROOM>>(
                                                   "api/HisMestRoom/CreateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   mestRoomCreate,
                                                   param);
                                        WaitingManager.Hide();
                                        if (createResult != null && createResult.Count > 0)
                                        {
                                            success = true;
                                            mestRoomMediStocks.AddRange(createResult);
                                        }
                                    }

                                    if ((dataDelete == null && dataCreate == null) || (dataDelete.Count == 0 && dataCreate.Count == 0))
                                    {

                                        if (dataChecked != null && dataChecked.Count > 0)
                                        {
                                            success = true;
                                        }
                                        else
                                        {
                                            WaitingManager.Hide();
                                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng", "Thông báo");
                                            return;
                                        }
                                    }

                                    data = data.OrderByDescending(p => p.check1).ToList();
                                    if (ucGridControlRoom != null)
                                    {
                                        RoomProcessor.Reload(ucGridControlRoom, data);
                                    }
                                }
                                //else
                                //{
                                //    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                                //}
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn kho", "Thông báo");
                            return;
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedistockGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.MediStock.MediStockADO)
                {
                    var type = (HIS.UC.MediStock.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.MediStock.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChooseMediStock != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn kho!");
                                    break;
                                }
                                this.currentCopyMedistockAdo = (HIS.UC.MediStock.MediStockADO)sender;
                                break;
                            }
                        case HIS.UC.MediStock.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.MediStock.MediStockADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyMedistockAdo == null && isChooseMediStock != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyMedistockAdo != null && currentPaste != null && isChooseMediStock == 2)
                                {
                                    if (this.currentCopyMedistockAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisMestRoomCopyByMediStockSDO hisMestMetyCopyByMediStockSDO = new HisMestRoomCopyByMediStockSDO();
                                    hisMestMetyCopyByMediStockSDO.CopyMediStockId = currentCopyMedistockAdo.ID;
                                    hisMestMetyCopyByMediStockSDO.PasteMediStockId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_MEST_ROOM>>("api/HisMestRoom/CopyByMediStock", ApiConsumer.ApiConsumers.MosConsumer, hisMestMetyCopyByMediStockSDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.Room.RoomAccountADO> dataNew = new List<HIS.UC.Room.RoomAccountADO>();

                                        dataNew = (from r in listRoom select new RoomAccountADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var itemUsername in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                }
                                            }
                                        }

                                        dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                                        if (ucGridControlRoom != null)
                                        {
                                            RoomProcessor.Reload(ucGridControlRoom, dataNew);
                                        }
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewRooom_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.Room.RoomAccountADO)
                {
                    var type = (HIS.UC.Room.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.Room.Popup.PopupMenuProcessor.ItemType.CopyPhongSangPhong:
                            {
                                if (isChooseRoom != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn phòng!");
                                    break;
                                }
                                this.currentCopyRoomAccountAdo = (HIS.UC.Room.RoomAccountADO)sender;
                                break;
                            }
                        case HIS.UC.Room.Popup.PopupMenuProcessor.ItemType.PastePhongSangPhong:
                            {
                                var currentPasteRoom = (HIS.UC.Room.RoomAccountADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (isChooseRoom != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn phòng!");
                                    break;
                                }
                                if (this.currentCopyRoomAccountAdo == null)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyRoomAccountAdo != null && currentPasteRoom != null && isChooseRoom == 1)
                                {
                                    if (this.currentCopyRoomAccountAdo.ID == currentPasteRoom.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisMestRoomCopyByRoomSDO hisUserRoomCopyByRoomSDO = new HisMestRoomCopyByRoomSDO();
                                    hisUserRoomCopyByRoomSDO.CopyRoomId = currentCopyRoomAccountAdo.ID;
                                    hisUserRoomCopyByRoomSDO.PasteRoomId = currentPasteRoom.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_MEST_ROOM>>("api/HisMestRoom/CopyByRoom", ApiConsumer.ApiConsumers.MosConsumer, hisUserRoomCopyByRoomSDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.MediStock.MediStockADO> dataNew = new List<HIS.UC.MediStock.MediStockADO>();
                                        dataNew = (from r in listMediStock select new MediStockADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var itemUsername in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.MEDI_STOCK_ID);
                                                if (check != null)
                                                {
                                                    check.checkMest = true;
                                                }
                                            }
                                        }

                                        dataNew = dataNew.OrderByDescending(p => p.checkMest).ToList();
                                        if (ucGridControlMediStock != null)
                                        {
                                            MediStockProcessor.Reload(ucGridControlMediStock, dataNew);
                                        }
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        default:
                            break;
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
