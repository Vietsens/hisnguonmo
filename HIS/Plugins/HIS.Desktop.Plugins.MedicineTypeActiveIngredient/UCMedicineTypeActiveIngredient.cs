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
using HIS.Desktop.Plugins.MedicineTypeActiveIngredient.entity;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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
using HIS.UC.ActiveIngredent;
using HIS.UC.ActiveIngredent.ADO;
using HIS.UC.MedicineTypeForGradient;
using HIS.UC.MedicineTypeForGradient.ADO;

namespace HIS.Desktop.Plugins.MedicineTypeActiveIngredient
{
    public partial class UCMedicineTypeActiveIngredient : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_ROOM_TYPE> roomType { get; set; }
        ActiveIngredentProcessor RoomProcessor;
        HIS.UC.MedicineTypeForGradient.UCMedicineTypeForGradientProcessor MediStockProcessor;
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
        internal List<HIS.UC.ActiveIngredent.ActiveIngredentADO> lstRoomADOs { get; set; }
        internal List<HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO> lstMediStockADOs { get; set; }
        List<HIS_ACTIVE_INGREDIENT> listRoom;
        List<V_HIS_MEDICINE_TYPE> listMediStock;
        long roomIdCheckByRoom = 0;
        long mediStockIdCheck = 0;
        long isChooseRoom;
        long isChooseMediStock;
        bool isCheckAll;
        List<V_HIS_MEDICINE_TYPE_ACIN> mestRooms { get; set; }
        List<V_HIS_MEDICINE_TYPE_ACIN> mestRoomMediStocks { get; set; }

        HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO currentCopyMedistockAdo;
        HIS.UC.ActiveIngredent.ActiveIngredentADO currentCopyRoomAccountAdo;

        public UCMedicineTypeActiveIngredient(Inventec.Desktop.Common.Modules.Module _moduleData)
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
                status.Add(new Status(1, "Hoạt chất"));
                status.Add(new Status(2, "Loại thuốc"));

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

        private void FillDataToGridMediStock(UCMedicineTypeActiveIngredient uCMestExportRoom)
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
                listMediStock = new List<V_HIS_MEDICINE_TYPE>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisMedicineTypeViewFilter MediStockFilter = new HisMedicineTypeViewFilter();
                MediStockFilter.KEY_WORD = txtKeyword1.Text;
                MediStockFilter.ORDER_DIRECTION = "ASC";
                MediStockFilter.ORDER_FIELD = "MEDICINE_TYPE_CODE";
                MediStockFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                if ((long)cboChoose.EditValue == 2)
                {
                    isChooseMediStock = (long)cboChoose.EditValue;
                }

                var mest = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEDICINE_TYPE_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    MediStockFilter,
                    param);

                lstMediStockADOs = new List<MedicineTypeForGradientADO>();
                if (mest != null && mest.Data.Count > 0)
                {
                    listMediStock = mest.Data;
                    foreach (var item in listMediStock)
                    {
                        MedicineTypeForGradientADO MedicineTypeForGradientADO = new MedicineTypeForGradientADO(item);
                        if (isChooseMediStock == 2)
                        {
                            MedicineTypeForGradientADO.isKeyChooseMest = true;
                        }
                        lstMediStockADOs.Add(MedicineTypeForGradientADO);
                    }
                }

                if (mestRooms != null && mestRooms.Count > 0)
                {
                    foreach (var itemUsername in mestRooms)
                    {
                        var check = lstMediStockADOs.FirstOrDefault(o => o.ID == itemUsername.MEDICINE_TYPE_ID);
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

        private void FillDataToGridRoom(UCMedicineTypeActiveIngredient uCMestExportRoom)
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
                listRoom = new List<HIS_ACTIVE_INGREDIENT>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisActiveIngredientFilter RoomFilter = new HisActiveIngredientFilter();
                RoomFilter.ORDER_FIELD = "ACTIVE_INGREDIENT_CODE";
                RoomFilter.ORDER_DIRECTION = "ASC";
                RoomFilter.KEY_WORD = txtKeyword2.Text;
                RoomFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChooseRoom = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT>>(
                    "api/HisActiveIngredient/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    RoomFilter,
                    param);

                lstRoomADOs = new List<ActiveIngredentADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listRoom = rs.Data;
                    foreach (var item in listRoom)
                    {
                        ActiveIngredentADO ActiveIngredentADO = new ActiveIngredentADO(item);
                        if (isChooseRoom == 1)
                        {
                            ActiveIngredentADO.isKeyChoose = true;
                        }
                        lstRoomADOs.Add(ActiveIngredentADO);
                    }
                }

                if (mestRoomMediStocks != null && mestRoomMediStocks.Count > 0)
                {
                    foreach (var itemUsername in mestRoomMediStocks)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ACTIVE_INGREDIENT_ID);
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
                MediStockProcessor = new UCMedicineTypeForGradientProcessor();
                MedicineTypeForGradientInitADO ado = new MedicineTypeForGradientInitADO();
                ado.ListMedicineTypeForGradientColumn = new List<MedicineTypeForGradientColumn>();
                ado.gridViewMedicineType_MouseDownMest = gridViewMediStock_MouseDownMest;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                //ado.GridView_MouseRightClick = MedistockGridView_MouseRightClick;

                MedicineTypeForGradientColumn colRadio2 = new MedicineTypeForGradientColumn("   ", "radioMest", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeForGradientColumn.Add(colRadio2);

                MedicineTypeForGradientColumn colCheck2 = new MedicineTypeForGradientColumn("   ", "checkMest", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollectionMediStock.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeForGradientColumn.Add(colCheck2);

                MedicineTypeForGradientColumn colMaKhoXuat = new MedicineTypeForGradientColumn("Mã loại thuốc", "MEDICINE_TYPE_CODE", 60, false);
                colMaKhoXuat.VisibleIndex = 2;
                ado.ListMedicineTypeForGradientColumn.Add(colMaKhoXuat);

                MedicineTypeForGradientColumn colTenKhoXuat = new MedicineTypeForGradientColumn("Tên loại thuốc", "MEDICINE_TYPE_NAME", 100, false);
                colTenKhoXuat.VisibleIndex = 3;
                ado.ListMedicineTypeForGradientColumn.Add(colTenKhoXuat);

                //MedicineTypeForGradientColumn colNguoiTao = new MedicineTypeForGradientColumn("Người tạo", "CREATOR", 100, false);
                //colNguoiTao.VisibleIndex = 4;
                //ado.ListMedicineTypeForGradientColumn.Add(colNguoiTao);

                //MedicineTypeForGradientColumn colNguoiSua = new MedicineTypeForGradientColumn("Người sửa", "MODIFIER", 100, false);
                //colNguoiSua.VisibleIndex = 5;
                //ado.ListMedicineTypeForGradientColumn.Add(colNguoiSua);

                this.ucGridControlMediStock = (UserControl)MediStockProcessor.Run(ado);

                if (ucGridControlMediStock != null)
                {
                    this.panelControlMedicineType.Controls.Add(this.ucGridControlMediStock);
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
                            List<HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO> lstChecks = new List<HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO>();

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

        private void btn_Radio_Enable_Click1(V_HIS_MEDICINE_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineTypeAcinViewFilter mestRoomFilter = new HisMedicineTypeAcinViewFilter();
                mestRoomFilter.MEDICINE_TYPE_ID = data.ID;
                mediStockIdCheck = data.ID;

                mestRoomMediStocks = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE_ACIN>>(
                    "api/HisMedicineTypeAcin/GetView",
                   ApiConsumers.MosConsumer,
                   mestRoomFilter,
                   param);
                List<HIS.UC.ActiveIngredent.ActiveIngredentADO> dataNew = new List<HIS.UC.ActiveIngredent.ActiveIngredentADO>();

                dataNew = (from r in listRoom select new ActiveIngredentADO(r)).ToList();
                if (mestRoomMediStocks != null && mestRoomMediStocks.Count > 0)
                {
                    foreach (var itemUsername in mestRoomMediStocks)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.ACTIVE_INGREDIENT_ID);
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
                RoomProcessor = new HIS.UC.ActiveIngredent.ActiveIngredentProcessor();
                ActiveIngredentInitADO ado = new ActiveIngredentInitADO();
                ado.ListActiveIngredentColumn = new List<ActiveIngredentColumn>();
                ado.ActiveIngredentGrid_MouseDown = gridViewRoom_MouseDownRoom;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                //ado.rooom_MouseRightClick = GridViewRooom_MouseRightClick;

                ActiveIngredentColumn colRadio1 = new ActiveIngredentColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListActiveIngredentColumn.Add(colRadio1);

                ActiveIngredentColumn colCheck1 = new ActiveIngredentColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionRoom.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListActiveIngredentColumn.Add(colCheck1);

                ActiveIngredentColumn colMaPhong = new ActiveIngredentColumn("Mã hoạt chất", "ACTIVE_INGREDIENT_CODE", 60, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListActiveIngredentColumn.Add(colMaPhong);

                ActiveIngredentColumn colTenPhong = new ActiveIngredentColumn("Tên hoạt chất", "ACTIVE_INGREDIENT_NAME", 100, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListActiveIngredentColumn.Add(colTenPhong);

                this.ucGridControlRoom = (UserControl)RoomProcessor.Run(ado);
                if (ucGridControlRoom != null)
                {
                    this.panelControlActiveIngredient.Controls.Add(this.ucGridControlRoom);
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
                            List<HIS.UC.ActiveIngredent.ActiveIngredentADO> lstChecks = new List<HIS.UC.ActiveIngredent.ActiveIngredentADO>();

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

        private void btn_Radio_Enable_Click(HIS_ACTIVE_INGREDIENT data, ActiveIngredentADO ado)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineTypeAcinViewFilter mestRoomFilter = new HisMedicineTypeAcinViewFilter();
                mestRoomFilter.ACTIVE_INGREDIENT_ID = data.ID;// TODO
                roomIdCheckByRoom = data.ID;

                mestRooms = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE_ACIN>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEDICINE_TYPE_ACIN_GETVIEW,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                mestRoomFilter,
                                param);
                List<HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO> dataNew = new List<HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO>();
                dataNew = (from r in listMediStock select new MedicineTypeForGradientADO(r)).ToList();
                if (mestRooms != null && mestRooms.Count > 0)
                {
                    foreach (var itemUsername in mestRooms)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.MEDICINE_TYPE_ID);
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
                            if (mediStock is List<HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO>)
                            {
                                var data = (List<HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO>)mediStock;

                                if (data != null && data.Count > 0)
                                {
                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisMedicineTypeAcinViewFilter filter = new HisMedicineTypeAcinViewFilter();
                                    filter.ACTIVE_INGREDIENT_ID = roomIdCheckByRoom;// nambg

                                    //var mestRooms = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE_ACIN>>(
                                    //   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEST_ROOM_GETVIEW,
                                    //   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    //   filter,
                                    //   param);

                                    var listMediStock = this.mestRooms.Select(p => p.MEDICINE_TYPE_ID).ToList();

                                    var dataCheckeds = data.Where(p => p.checkMest == true).ToList();

                                    //List xoa

                                    var dataDeletes = data.Where(o => this.mestRooms.Select(p => p.MEDICINE_TYPE_ID)
                                        .Contains(o.ID) && o.checkMest == false).ToList();

                                    //list them
                                    var dataCreates = dataCheckeds.Where(o => !this.mestRooms.Select(p => p.MEDICINE_TYPE_ID)
                                        .Contains(o.ID)).ToList();

                                    if (dataDeletes != null && dataDeletes.Count > 0)
                                    {
                                        List<long> deleteIds = this.mestRooms.Where(o => dataDeletes.Select(p => p.ID)
                                            .Contains(o.MEDICINE_TYPE_ID)).Select(o => o.ID).ToList();
                                        WaitingManager.Show();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "api/HisMedicineTypeAcin/DeleteList",
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
                                        List<V_HIS_MEDICINE_TYPE_ACIN> MestRoomCreates = new List<V_HIS_MEDICINE_TYPE_ACIN>();
                                        foreach (var item in dataCreates)
                                        {
                                            V_HIS_MEDICINE_TYPE_ACIN mestRoom = new V_HIS_MEDICINE_TYPE_ACIN();
                                            mestRoom.MEDICINE_TYPE_ID = item.ID;
                                            mestRoom.ACTIVE_INGREDIENT_ID = roomIdCheckByRoom;
                                            MestRoomCreates.Add(mestRoom);
                                        }
                                        WaitingManager.Show();
                                        var createResult = new BackendAdapter(param).Post<List<V_HIS_MEDICINE_TYPE_ACIN>>(
                                                   "api/HisMedicineTypeAcin/CreateList",
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
                                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn hoạt chất", "Thông báo");
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
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn loại thuốc", "Thông báo");
                            return;
                        }
                    }
                    if (isChooseMediStock == 2)
                    {
                        if (mediStockIdCheck > 0)
                        {
                            if (room is List<HIS.UC.ActiveIngredent.ActiveIngredentADO>)
                            {
                                var data = (List<HIS.UC.ActiveIngredent.ActiveIngredentADO>)room;

                                if (data != null && data.Count > 0)
                                {
                                    //bool success = false;
                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisMedicineTypeAcinViewFilter filter = new HisMedicineTypeAcinViewFilter();
                                    filter.MEDICINE_TYPE_ID = mediStockIdCheck;
                                    //var mestRooms = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE_ACIN>>(
                                    //   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEST_ROOM_GETVIEW,
                                    //   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    //   filter,
                                    //   param);

                                    var listRoomID = mestRoomMediStocks.Select(p => p.ACTIVE_INGREDIENT_ID).ToList();

                                    var dataChecked = data.Where(p => p.check1 == true).ToList();

                                    //List xoa

                                    var dataDelete = data.Where(o => mestRoomMediStocks.Select(p => p.ACTIVE_INGREDIENT_ID)
                                        .Contains(o.ID) && o.check1 == false).ToList();

                                    //list them
                                    var dataCreate = dataChecked.Where(o => !mestRoomMediStocks.Select(p => p.ACTIVE_INGREDIENT_ID)
                                        .Contains(o.ID)).ToList();

                                    if (dataDelete != null && dataDelete.Count > 0)
                                    {
                                        List<long> deleteId = mestRoomMediStocks.Where(o => dataDelete.Select(p => p.ID)
                                            .Contains(o.ACTIVE_INGREDIENT_ID)).Select(o => o.ID).ToList();
                                        WaitingManager.Show();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "api/HisMedicineTypeAcin/DeleteList",
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
                                        List<V_HIS_MEDICINE_TYPE_ACIN> mestRoomCreate = new List<V_HIS_MEDICINE_TYPE_ACIN>();
                                        foreach (var item in dataCreate)
                                        {
                                            V_HIS_MEDICINE_TYPE_ACIN mestRoom = new V_HIS_MEDICINE_TYPE_ACIN();
                                            mestRoom.ACTIVE_INGREDIENT_ID = item.ID;
                                            mestRoom.MEDICINE_TYPE_ID = mediStockIdCheck;
                                            mestRoomCreate.Add(mestRoom);
                                        }
                                        WaitingManager.Show();
                                        var createResult = new BackendAdapter(param).Post<List<V_HIS_MEDICINE_TYPE_ACIN>>(
                                                   "api/HisMedicineTypeAcin/CreateList",
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
                                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn loại thuốc", "Thông báo");
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
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn hoạt chất", "Thông báo");
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
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO)
                {
                    //var type = (HIS.UC.MedicineType.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    //switch (type)
                    //{
                    //    case HIS.UC.MediStock.Popup.PopupMenuProcessor.ItemType.Copy:
                    //        {
                    //            if (isChooseMediStock != 2)
                    //            {
                    //                MessageManager.Show("Vui lòng chọn kho!");
                    //                break;
                    //            }
                    //            this.currentCopyMedistockAdo = (HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO)sender;
                    //            break;
                    //        }
                    //    case HIS.UC.MediStock.Popup.PopupMenuProcessor.ItemType.Paste:
                    //        {
                    //            var currentPaste = (HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO)sender;
                    //            bool success = false;
                    //            CommonParam param = new CommonParam();
                    //            if (this.currentCopyMedistockAdo == null && isChooseMediStock != 2)
                    //            {
                    //                MessageManager.Show("Vui lòng copy!");
                    //                break;
                    //            }
                    //            if (this.currentCopyMedistockAdo != null && currentPaste != null && isChooseMediStock == 2)
                    //            {
                    //                if (this.currentCopyMedistockAdo.ID == currentPaste.ID)
                    //                {
                    //                    MessageManager.Show("Trùng dữ liệu copy và paste");
                    //                    break;
                    //                }
                    //                HisMestRoomCopyByMediStockSDO hisMestMetyCopyByMediStockSDO = new HisMestRoomCopyByMediStockSDO();
                    //                hisMestMetyCopyByMediStockSDO.CopyMediStockId = currentCopyMedistockAdo.ID;
                    //                hisMestMetyCopyByMediStockSDO.PasteMediStockId = currentPaste.ID;
                    //                var result = new BackendAdapter(param).Post<List<HIS_MEST_ROOM>>("api/HisMestRoom/CopyByMediStock", ApiConsumer.ApiConsumers.MosConsumer, hisMestMetyCopyByMediStockSDO, param);
                    //                if (result != null)
                    //                {
                    //                    success = true;
                    //                    List<HIS.UC.ActiveIngredent.ActiveIngredentADO> dataNew = new List<HIS.UC.ActiveIngredent.ActiveIngredentADO>();

                    //                    dataNew = (from r in listRoom select new ActiveIngredentADO(r)).ToList();
                    //                    if (result != null && result.Count > 0)
                    //                    {
                    //                        foreach (var itemUsername in result)
                    //                        {
                    //                            var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.ACTIVE_INGREDIENT_ID);
                    //                            if (check != null)
                    //                            {
                    //                                check.check1 = true;
                    //                            }
                    //                        }
                    //                    }

                    //                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    //                    if (ucGridControlRoom != null)
                    //                    {
                    //                        RoomProcessor.Reload(ucGridControlRoom, dataNew);
                    //                    }
                    //                }
                    //            }
                    //            MessageManager.Show(this.ParentForm, param, success);
                    //            break;
                    //        }
                    //    default:
                    //        break;
                    //}
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
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.ActiveIngredent.ActiveIngredentADO)
                {
                    //var type = (HIS.UC.ActiveIngredent.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    //switch (type)
                    //{
                    //    case HIS.UC.Room.Popup.PopupMenuProcessor.ItemType.CopyPhongSangPhong:
                    //        {
                    //            if (isChooseRoom != 1)
                    //            {
                    //                MessageManager.Show("Vui lòng chọn phòng!");
                    //                break;
                    //            }
                    //            this.currentCopyRoomAccountAdo = (HIS.UC.ActiveIngredent.ActiveIngredentADO)sender;
                    //            break;
                    //        }
                    //    case HIS.UC.Room.Popup.PopupMenuProcessor.ItemType.PastePhongSangPhong:
                    //        {
                    //            var currentPasteRoom = (HIS.UC.ActiveIngredent.ActiveIngredentADO)sender;
                    //            bool success = false;
                    //            CommonParam param = new CommonParam();
                    //            if (isChooseRoom != 1)
                    //            {
                    //                MessageManager.Show("Vui lòng chọn phòng!");
                    //                break;
                    //            }
                    //            if (this.currentCopyRoomAccountAdo == null)
                    //            {
                    //                MessageManager.Show("Vui lòng copy!");
                    //                break;
                    //            }
                    //            if (this.currentCopyRoomAccountAdo != null && currentPasteRoom != null && isChooseRoom == 1)
                    //            {
                    //                if (this.currentCopyRoomAccountAdo.ID == currentPasteRoom.ID)
                    //                {
                    //                    MessageManager.Show("Trùng dữ liệu copy và paste");
                    //                    break;
                    //                }
                    //                HisMestRoomCopyByRoomSDO hisUserRoomCopyByRoomSDO = new HisMestRoomCopyByRoomSDO();
                    //                hisUserRoomCopyByRoomSDO.CopyRoomId = currentCopyRoomAccountAdo.ID;
                    //                hisUserRoomCopyByRoomSDO.PasteRoomId = currentPasteRoom.ID;
                    //                var result = new BackendAdapter(param).Post<List<HIS_MEST_ROOM>>("api/HisMestRoom/CopyByRoom", ApiConsumer.ApiConsumers.MosConsumer, hisUserRoomCopyByRoomSDO, param);
                    //                if (result != null)
                    //                {
                    //                    success = true;
                    //                    List<HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO> dataNew = new List<HIS.UC.MedicineTypeForGradient.MedicineTypeForGradientADO>();
                    //                    dataNew = (from r in listMediStock select new MedicineTypeForGradientADO(r)).ToList();
                    //                    if (result != null && result.Count > 0)
                    //                    {
                    //                        foreach (var itemUsername in result)
                    //                        {
                    //                            var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.MEDICINE_TYPE_ID);
                    //                            if (check != null)
                    //                            {
                    //                                check.checkMest = true;
                    //                            }
                    //                        }
                    //                    }

                    //                    dataNew = dataNew.OrderByDescending(p => p.checkMest).ToList();
                    //                    if (ucGridControlMediStock != null)
                    //                    {
                    //                        MediStockProcessor.Reload(ucGridControlMediStock, dataNew);
                    //                    }
                    //                }
                    //            }
                    //            MessageManager.Show(this.ParentForm, param, success);
                    //            break;
                    //        }
                    //    default:
                    //        break;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
