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
using HIS.UC.ListMaterialType;
using HIS.UC.MediStock;
using MOS.EFMODEL.DataModels;
using HIS.UC.MediStock.ADO;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.MediStockMatyList.Entity;
using DevExpress.XtraGrid.Views.Layout.Modes;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.UC.ListMaterialType.ADO;
using MOS.Filter;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.MediStockMatyList.Resources;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraBars;
using MOS.SDO;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.MediStockMatyList
{
    public partial class UCMediStockMatyList : HIS.Desktop.Utility.UserControlBase
    {
        internal List<HIS.UC.ListMaterialType.ListMaterialTypeADO> MatyAdo { get; set; }
        internal List<HIS.UC.MediStock.MediStockADO> StockAdo { get; set; }
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        long checkStock = 0;
        long checkMaty = 0;
        UCListMaterialTypeProcessor matyProcessor = null;
        UCMediStockProcessor stockProcessor = null;
        UserControl ucMaty;
        UserControl ucStock;
        long isChoseStock = 0;
        long isChoseMaty = 0;
        bool isCheckAll;
        bool checkRa = false;
        List<V_HIS_MATERIAL_TYPE> listMaty = new List<V_HIS_MATERIAL_TYPE>();
        List<V_HIS_MEDI_STOCK> listStock = new List<V_HIS_MEDI_STOCK>();
        List<HIS_MEDI_STOCK_MATY> listMediStockMaty = new List<HIS_MEDI_STOCK_MATY>();
        internal Inventec.Desktop.Common.Modules.Module currentModule;

        HIS.UC.ListMaterialType.ListMaterialTypeADO currentCopyMaterialTypeAdo;
        HIS.UC.MediStock.MediStockADO currentCopyMedistockAdo;

        public UCMediStockMatyList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            InitStock();
            InitMaty();
        }

        private void UCMediStockMatyList_Load(object sender, EventArgs e)
        {
            FillDataToGrid1(this);
            FillDataToGrid2(this);
            LoadComboStatus();
            SetCaptionByLanguageKey();
            chkNotDisplayBlock.Checked = true;
        }
        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Kho"));
                status.Add(new Status(2, "Loại vật tư"));

                List<Inventec.Common.Controls.EditorLoader.ColumnInfo> columnInfos = new List<Inventec.Common.Controls.EditorLoader.ColumnInfo>();
                columnInfos.Add(new Inventec.Common.Controls.EditorLoader.ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboStatus, status, controlEditorADO);
                cboStatus.EditValue = status[0].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitStock()
        {
            try
            {
                stockProcessor = new UCMediStockProcessor();
                MediStockInitADO ado = new MediStockInitADO();
                ado.ListMediStockColumn = new List<UC.MediStock.MediStockColumn>();
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.gridViewMediStock_MouseDownMest = gridViewStock_MouseDownStock;
                ado.ListMediStockGrid_RowCellClick = Grid_RowCellClick;
                ado.GridView_MouseRightClick = MedistockGridView_MouseRightClick;

                MediStockColumn colRadio2 = new MediStockColumn("   ", "radioMest", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMediStockColumn.Add(colRadio2);

                MediStockColumn colCheck2 = new MediStockColumn("   ", "checkMest", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imgStock.Images[1];
                colCheck2.Caption = "Chọn tất cả";
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMediStockColumn.Add(colCheck2);

                MediStockColumn colMaPhong = new MediStockColumn("Mã kho", "MEDI_STOCK_CODE", 80, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListMediStockColumn.Add(colMaPhong);

                MediStockColumn colTenPhong = new MediStockColumn("Tên kho", "MEDI_STOCK_NAME", 150, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListMediStockColumn.Add(colTenPhong);



                this.ucStock = (UserControl)stockProcessor.Run(ado);
                if (ucStock != null)
                {
                    this.pnlStock.Controls.Add(this.ucStock);
                    this.ucStock.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillDataToGrid1(UCMediStockMatyList _Stock)
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridStock(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridStock, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridStock(object data)
        {
            try
            {
                WaitingManager.Show();
                listStock = new List<V_HIS_MEDI_STOCK>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisMediStockViewFilter stockFilter = new MOS.Filter.HisMediStockViewFilter();
                stockFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                stockFilter.ORDER_FIELD = "MEDI_STOCK";
                stockFilter.ORDER_DIRECTION = "ASC";
                stockFilter.KEY_WORD = txtKeyword1.Text;

                //if (cboRoomType.EditValue != null)
                //    RoomFillter.ROOM_TYPE_ID = (long)cboRoomType.EditValue;
                //long isChoseMety = 0;
                if ((long)cboStatus.EditValue == 1)
                {
                    isChoseMaty = (long)cboStatus.EditValue;
                }
                if ((long)cboStatus.EditValue == 2)
                {
                    isChoseMaty = (long)cboStatus.EditValue;
                }
                var rs = new BackendAdapter(param).GetRO<List<V_HIS_MEDI_STOCK>>(
                    "api/HisMediStock/GetView",
                    ApiConsumers.MosConsumer,
                    stockFilter,
                    param);

                StockAdo = new List<HIS.UC.MediStock.MediStockADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_HIS_ROOM> dataByRoom = new List<V_HIS_ROOM>();
                    listStock = rs.Data;
                    foreach (var item in listStock)
                    {
                        //HIS.UC.RoomTypeList.RoomTypeListADO RoomtypeADO = new HIS.UC.RoomTypeList.RoomTypeListADO(item);
                        HIS.UC.MediStock.MediStockADO medistockADO = new HIS.UC.MediStock.MediStockADO(item);
                        if (isChoseMaty == 1)
                        {
                            medistockADO.isKeyChooseMest = true;
                            //btnCheckAll1.Enabled = false;
                        }
                        StockAdo.Add(medistockADO);
                    }
                }
                if (listMediStockMaty != null && listMediStockMaty.Count > 0)
                {

                    foreach (var itemStock in listMediStockMaty)
                    {
                        var check = StockAdo.FirstOrDefault(o => o.ID == itemStock.MEDI_STOCK_ID);
                        if (check != null)
                        {
                            check.checkMest = true;
                        }
                    }
                }

                StockAdo = StockAdo.OrderByDescending(p => p.checkMest).ToList();
                if (ucStock != null)
                {
                    stockProcessor.Reload(ucStock, StockAdo);
                }
                rowCount = (data == null ? 0 : StockAdo.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitMaty()
        {
            try
            {
                matyProcessor = new UCListMaterialTypeProcessor();
                ListMaterialTypeInitADO ado = new ListMaterialTypeInitADO();
                ado.ListMaterialTypeColumn = new List<UC.ListMaterialType.ListMaterialTypeColumn>();
                ado.gridViewMaty_MouseDownMaty = gridViewMaty_MouseDownMaty;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.gridView_MouseRightClick = MatyGridView_MouseRightClick;
                ado.processUpdateTrustAmount = processUpdateTrustAmount;

                ListMaterialTypeColumn colRadio1 = new ListMaterialTypeColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialTypeColumn.Add(colRadio1);

                ListMaterialTypeColumn colCheck1 = new ListMaterialTypeColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = imgMaty.Images[1];
                colCheck1.Caption = "Chọn tất cả";
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialTypeColumn.Add(colCheck1);

               /* ListMaterialTypeColumn colUpdateTrustAmount = new ListMaterialTypeColumn("   ", "updateTrustAmount", 30, true);
                colUpdateTrustAmount.VisibleIndex = 2;
                colUpdateTrustAmount.Caption = " ";
                colUpdateTrustAmount.ToolTip = "Cập nhật Cơ số thực tế";
                colUpdateTrustAmount.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialTypeColumn.Add(colUpdateTrustAmount);*/

                ListMaterialTypeColumn colMaPhong = new ListMaterialTypeColumn("Mã loại vật tư", "MATERIAL_TYPE_CODE", 100, false);
                colMaPhong.VisibleIndex = 3;
                ado.ListMaterialTypeColumn.Add(colMaPhong);

                ListMaterialTypeColumn colTenPhong = new ListMaterialTypeColumn("Tên loại vật tư", "MATERIAL_TYPE_NAME", 200, true);
                colTenPhong.VisibleIndex = 4;
                ado.ListMaterialTypeColumn.Add(colTenPhong);

                ListMaterialTypeColumn colDonViTinh = new ListMaterialTypeColumn("Đơn vị tính", "SERVICE_UNIT_NAME", 90, false);
                colDonViTinh.VisibleIndex = 5;
                ado.ListMaterialTypeColumn.Add(colDonViTinh);

                ListMaterialTypeColumn colTran = new ListMaterialTypeColumn("Số lượng sàn", "ALERT_MIN_IN_STOCK_STR", 100, true);
                colTran.VisibleIndex = 6;
                colTran.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialTypeColumn.Add(colTran);

               ListMaterialTypeColumn colSan = new ListMaterialTypeColumn("Cơ số", "ALERT_MAX_IN_STOCK_STR", 100, true);
                colSan.VisibleIndex = 7;
                colSan.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialTypeColumn.Add(colSan);

                ListMaterialTypeColumn colMestStock = new ListMaterialTypeColumn("Kho xuất", "EXP_MEDI_STOCK_ID", 100, true);
                colMestStock.VisibleIndex = 8;
                colMestStock.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialTypeColumn.Add(colMestStock);

             /*   ListMaterialTypeColumn colTrust = new ListMaterialTypeColumn("Cơ số thực tế", "TRUST_AMOUNT_IN_STOCK_STR", 100, true);
                colTrust.ToolTip = "Là cơ số thực tế do bị thay đổi trong trường hợp có các phiếu xuất khác ngoài phiếu xuất cho bệnh nhân (vd: xuất chuyển kho, xuất hao phí, ...) hoặc có các phiếu nhập khác ngoài nhập bù cơ số (vd: nhập đầu kì, nhập thu hồi, ...)";
                colTrust.VisibleIndex = 9;
                colTrust.AllowEdit = false;
                colTrust.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialTypeColumn.Add(colTrust);*/

                ListMaterialTypeColumn colQuaTran = new ListMaterialTypeColumn("Chặn nhập quá trần", "IS_PREVENT_MAX", 100, true);
                colQuaTran.VisibleIndex = 10;
                colQuaTran.Visible = false;
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialTypeColumn.Add(colQuaTran);

                ListMaterialTypeColumn colKoXuat = new ListMaterialTypeColumn("Vật tư giới hạn", "IS_GOODS_RESTRICT", 100, true);
                colKoXuat.VisibleIndex = 11;
                colKoXuat.Visible = false;
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMaterialTypeColumn.Add(colKoXuat);

                this.ucMaty = (UserControl)matyProcessor.Run(ado);
                if (ucMaty != null)
                {
                    this.pnlMaty.Controls.Add(this.ucMaty);
                    this.ucMaty.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillDataToGrid2(UCMediStockMatyList _Material)
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridMaty(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridMaty, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToMaty(UCMediStockMatyList _Mety)
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridMaty(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridMaty, param);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridMaty(object data)
        {
            try
            {
                WaitingManager.Show();
                listMaty = new List<V_HIS_MATERIAL_TYPE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisMaterialTypeViewFilter metyFilter = new MOS.Filter.HisMaterialTypeViewFilter();
                metyFilter.ORDER_FIELD = "MATERIAL_TYPE_NAME";
                metyFilter.ORDER_DIRECTION = "ASC";
                metyFilter.KEY_WORD = txtKeyword2.Text;

                //if (cboRoomType.EditValue != null)
                //    RoomFillter.ROOM_TYPE_ID = (long)cboRoomType.EditValue;
                //long isChoseStock = 0;
                if ((long)cboStatus.EditValue == 2)
                {
                    isChoseStock = (long)cboStatus.EditValue;
                }
                if ((long)cboStatus.EditValue == 1)
                {
                    isChoseStock = (long)cboStatus.EditValue;
                }
                if (chkNotDisplayBlock.Checked == true)
                {
                    metyFilter.IS_ACTIVE = 1;
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<V_HIS_MATERIAL_TYPE>>(
                    "api/HisMaterialType/GetView",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    metyFilter,
                    param);

                MatyAdo = new List<HIS.UC.ListMaterialType.ListMaterialTypeADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listMaty = rs.Data;
                    foreach (var item in listMaty)
                    {
                        HIS.UC.ListMaterialType.ListMaterialTypeADO matypeADO = new HIS.UC.ListMaterialType.ListMaterialTypeADO(item);
                        if (isChoseStock == 2)
                        {
                            matypeADO.isKeyChoose = true;
                            //btnCheckAll2.Enabled = false;
                        }
                        MatyAdo.Add(matypeADO);
                    }
                }
                if (listMediStockMaty != null && listMediStockMaty.Count > 0)
                {
                    foreach (var item in listMediStockMaty)
                    {
                        var mediStockMety = MatyAdo.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                        if (mediStockMety != null)
                        {
                            mediStockMety.check1 = true;
                            mediStockMety.ALERT_MAX_IN_STOCK_STR = item.ALERT_MAX_IN_STOCK;
                            mediStockMety.ALERT_MIN_IN_STOCK_STR = item.ALERT_MIN_IN_STOCK;
                            mediStockMety.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
                            mediStockMety.IS_GOODS_RESTRICT = item.IS_GOODS_RESTRICT == 1 ? true : false;
                            mediStockMety.IS_PREVENT_MAX = item.IS_PREVENT_MAX == 1 ? true : false;
                        }
                    }
                }
                MatyAdo = MatyAdo.OrderByDescending(p => p.check1).ToList();
                if (ucMaty != null)
                {
                    matyProcessor.Reload(ucMaty, MatyAdo);
                }
                rowCount = (data == null ? 0 : MatyAdo.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
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
                HisMediStockMatyFilter filter = new HisMediStockMatyFilter();
                filter.MEDI_STOCK_ID = data.ID;
                checkStock = data.ID;
                listMediStockMaty = new List<HIS_MEDI_STOCK_MATY>();
                listMediStockMaty = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK_MATY>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEDI_STOCK_MATY_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.ListMaterialType.ListMaterialTypeADO> dataNew = new List<HIS.UC.ListMaterialType.ListMaterialTypeADO>();
                dataNew = (from r in listMaty select new HIS.UC.ListMaterialType.ListMaterialTypeADO(r)).ToList();
                if (listMediStockMaty != null && listMediStockMaty.Count > 0)
                {
                    foreach (var item in listMediStockMaty)
                    {
                        var mediStockMety = dataNew.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                        if (mediStockMety != null)
                        {
                            mediStockMety.check1 = true;
                            mediStockMety.ALERT_MAX_IN_STOCK_STR = item.ALERT_MAX_IN_STOCK;
                            mediStockMety.ALERT_MIN_IN_STOCK_STR = item.ALERT_MIN_IN_STOCK;
                            mediStockMety.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
                            mediStockMety.IS_GOODS_RESTRICT = item.IS_GOODS_RESTRICT == 1 ? true : false;
                            mediStockMety.IS_PREVENT_MAX = item.IS_PREVENT_MAX == 1 ? true : false;
                        }
                    }
                  
                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    if (ucMaty != null)
                    {
                        matyProcessor.Reload(ucMaty, dataNew);
                       
                    }
                }
                else
                {
                    FillDataToGrid2(this);
                }
                BackendDataWorker.Reset<V_HIS_MEST_ROOM>();
                var listMestRoom = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(p => p.ROOM_ID == data.ROOM_ID).ToList();
                if (ucMaty != null)
                matyProcessor.LoaCboMediStock(ucMaty, listMestRoom, false);
                checkRa = true;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btn_Radio_Enable_Click(V_HIS_MATERIAL_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisMediStockMatyFilter filter = new HisMediStockMatyFilter();
                filter.MATERIAL_TYPE_ID = data.ID;
                checkMaty = data.ID;
                //filter.
                listMediStockMaty = new List<HIS_MEDI_STOCK_MATY>();
                listMediStockMaty = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK_MATY>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEDI_STOCK_MATY_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.MediStock.MediStockADO> dataNew = new List<HIS.UC.MediStock.MediStockADO>();
                dataNew = (from r in listStock select new HIS.UC.MediStock.MediStockADO(r)).ToList();
                if (listMediStockMaty != null && listMediStockMaty.Count > 0)
                {

                    foreach (var itemStock in listMediStockMaty)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemStock.MEDI_STOCK_ID);
                        if (check != null)
                        {
                            check.checkMest = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.checkMest).ToList();
                    if (ucStock != null)
                    {
                        stockProcessor.Reload(ucStock, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid1(this);
                }
                checkRa = true;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewMaty_MouseDownMaty(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseMaty == 2)
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
                            var lstCheckAll = MatyAdo;
                            List<HIS.UC.ListMaterialType.ListMaterialTypeADO> lstChecks = new List<HIS.UC.ListMaterialType.ListMaterialTypeADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var roomCheckedNum = MatyAdo.Where(o => o.check1 == true).Count();
                                var roomNum = MatyAdo.Count();
                                if ((roomCheckedNum > 0 && roomCheckedNum < roomNum) || roomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imgMaty.Images[0];
                                }

                                if (roomCheckedNum == roomNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imgMaty.Images[1];
                                }
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

                                matyProcessor.Reload(ucMaty, lstChecks);
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
        private void gridViewStock_MouseDownStock(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseStock == 1)
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
                            var lstCheckAll = StockAdo;
                            List<HIS.UC.MediStock.MediStockADO> lstChecks = new List<HIS.UC.MediStock.MediStockADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var roomCheckedNum = StockAdo.Where(o => o.checkMest == true).Count();
                                var roomNum = StockAdo.Count();
                                if ((roomCheckedNum > 0 && roomCheckedNum < roomNum) || roomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imgStock.Images[0];
                                }

                                if (roomCheckedNum == roomNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imgStock.Images[1];
                                }
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

                                stockProcessor.Reload(ucStock, lstChecks);
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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MediStockMatyList.Resources.Lang", typeof(HIS.Desktop.Plugins.MediStockMatyList.UCMediStockMatyList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.cboStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch1.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.btnSearch1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch2.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.btnSearch2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotDisplayBlock.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.chkNotDisplayBlock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatus_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                checkRa = false;
                isChoseStock = 0;
                isChoseMaty = 0;
                listMediStockMaty = new List<HIS_MEDI_STOCK_MATY>();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
                if ((Inventec.Common.TypeConvert.Parse.ToInt64(cboStatus.EditValue.ToString() ?? "")) == 1)
                {
                    matyProcessor.LoaCboMediStock(ucMaty, null, false);
                }
                else
                    matyProcessor.LoaCboMediStock(ucMaty, null, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch1_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid1(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch2_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ucMaty != null && ucStock != null)
                {
                    bool success = false;

                    CommonParam param = new CommonParam();
                    List<HIS.UC.ListMaterialType.ListMaterialTypeADO> medicineMatys = matyProcessor.GetDataGridView(ucMaty) as List<HIS.UC.ListMaterialType.ListMaterialTypeADO>;
                   // if (medicineMatys.Exists(o => (o.TRUST_AMOUNT_IN_STOCK_STR > o.ALERT_MAX_IN_STOCK_STR) || (o.ALERT_MAX_IN_STOCK_STR != null && o.TRUST_AMOUNT_IN_STOCK_STR == null)))
                     //   return;
                    List<HIS.UC.MediStock.MediStockADO> medicineStocks = stockProcessor.GetDataGridView(ucStock) as List<HIS.UC.MediStock.MediStockADO>;
                    #region ---Kho
                    if (isChoseStock == 1 && medicineMatys != null && medicineMatys.Count > 0)
                    {
                        if (checkRa == true)
                        {
                            if (medicineMatys != null && medicineMatys.Count > 0)
                            {
                                //Check List
                                var dataCheckeds = medicineMatys.Where(p => (p.check1 == true)).ToList();

                                //List xóa
                                var dataDeletes = medicineMatys.Where(o => listMediStockMaty.Select(p => p.MATERIAL_TYPE_ID)
                               .Contains(o.ID) && o.check1 == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !listMediStockMaty.Select(p => p.MATERIAL_TYPE_ID)
                                    .Contains(o.ID)).ToList();

                                // List update
                                var dataUpdate = dataCheckeds.Where(o => listMediStockMaty.Select(p => p.MATERIAL_TYPE_ID)
                                    .Contains(o.ID)).ToList();
                                if (dataDeletes.Count == 0 && dataCreates.Count == 0 && dataUpdate.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn loại vật tư");
                                    return;
                                }
                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    // List<HIS_MEDI_STOCK_MATY> stockMetyUpdates = new List<HIS_MEDI_STOCK_MATY>();
                                    var stockMatyUpdates = new List<HIS_MEDI_STOCK_MATY>();
                                    foreach (var item in dataUpdate)
                                    {
                                        var mediStockMaty = listMediStockMaty.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.ID && o.MEDI_STOCK_ID == checkStock);
                                        if (mediStockMaty != null)
                                        {
                                            mediStockMaty.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK_STR;
                                            mediStockMaty.ALERT_MAX_IN_STOCK = item.ALERT_MAX_IN_STOCK_STR;
                                            mediStockMaty.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;

                                            //mediStockMaty.TRUST_AMOUNT_IN_STOCK_STR = 2004;
                                            mediStockMaty.IS_GOODS_RESTRICT = short.Parse(item.IS_GOODS_RESTRICT == true ? "1" : "0");
                                            mediStockMaty.IS_PREVENT_MAX = short.Parse(item.IS_PREVENT_MAX == true ? "1" : "0");
                                            stockMatyUpdates.Add(mediStockMaty);
                                        }
                                    }
                                    if (stockMatyUpdates != null && stockMatyUpdates.Count > 0)
                                    {
                                        var updateResult = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_MATY>>(
                                                   "/api/HisMediStockMaty/UpdateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   stockMatyUpdates,
                                                   param);
                                        if (updateResult != null && updateResult.Count > 0)
                                        {
                                            listMediStockMaty.AddRange(updateResult);
                                            success = true;
                                        }
                                    }
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 15)
                                {
                                    List<long> deleteIds = listMediStockMaty.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.MATERIAL_TYPE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "/api/HisMediStockMaty/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteIds,
                                              param);
                                    if (deleteResult)
                                    {
                                        listMediStockMaty = listMediStockMaty.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                       
                                        success = true;
                                    }
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_MEDI_STOCK_MATY> stockMetyCreates = new List<HIS_MEDI_STOCK_MATY>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_MEDI_STOCK_MATY stockMety = new HIS_MEDI_STOCK_MATY();
                                        stockMety.MATERIAL_TYPE_ID = item.ID;
                                        stockMety.MEDI_STOCK_ID = checkStock;
                                        stockMety.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK_STR;
                                        stockMety.ALERT_MAX_IN_STOCK = item.ALERT_MAX_IN_STOCK_STR;
                                        stockMety.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
                                        stockMety.IS_GOODS_RESTRICT = short.Parse(item.IS_GOODS_RESTRICT == true ? "1" : "0");
                                        stockMety.IS_PREVENT_MAX = short.Parse(item.IS_PREVENT_MAX == true ? "1" : "0");
                                        stockMetyCreates.Add(stockMety);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_MATY>>(
                                               "/api/HisMediStockMaty/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               stockMetyCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        listMediStockMaty.AddRange(createResult);
                                        success = true;
                                    }
                                }
                                WaitingManager.Hide();
                                #region Show message
                                MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                SessionManager.ProcessTokenLost(param);
                                #endregion
                                medicineMatys = medicineMatys.OrderByDescending(p => p.check1).ToList();
                                matyProcessor.Reload(ucMaty, medicineMatys);
                            }
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Kho");
                        }
                    }
                    #endregion
                    #region ---Loại vật tư
                    if (isChoseMaty == 2 && medicineStocks != null && medicineStocks.Count > 0)
                    {
                        if (checkRa == true)
                        {
                            HIS.UC.ListMaterialType.ListMaterialTypeADO medicineType = medicineMatys.FirstOrDefault(o => o.ID == checkMaty);
                            //Check List
                            var dataCheckeds = medicineStocks.Where(p => (p.checkMest == true)).ToList();

                            //List xóa
                            var dataDeletes = medicineStocks.Where(o => listMediStockMaty.Select(p => p.MEDI_STOCK_ID)
                           .Contains(o.ID) && o.checkMest == false).ToList();

                            //list them
                            var dataCreates = dataCheckeds.Where(o => !listMediStockMaty.Select(p => p.MEDI_STOCK_ID)
                                .Contains(o.ID)).ToList();

                            //list update
                            var dataUpdates = dataCheckeds.Where(o => listMediStockMaty.Select(p => p.MEDI_STOCK_ID)
                                .Contains(o.ID)).ToList();
                            if (dataDeletes.Count == 0 && dataCreates.Count == 0 && dataUpdates.Count == 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn kho");
                                return;
                            }
                            if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 5)
                            {
                                List<long> deleteIds = listMediStockMaty.Where(o => dataDeletes.Select(p => p.ID)
                                    .Contains(o.MEDI_STOCK_ID)).Select(o => o.ID).ToList();
                                bool deleteResult = new BackendAdapter(param).Post<bool>(
                                          "/api/HisMediStockMaty/DeleteList",
                                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                          deleteIds,
                                          param);
                                if (deleteResult)
                                {
                                    listMediStockMaty = listMediStockMaty.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                    success = true;
                                }

                            }

                            if (dataUpdates != null && dataUpdates.Count > 0 && medicineType != null)
                            {
                                // List<HIS_MEDI_STOCK_MATY> stockMetyUpdates = new List<HIS_MEDI_STOCK_MATY>();
                                var stockMetyUpdates = new List<HIS_MEDI_STOCK_MATY>();
                                foreach (var item in dataUpdates)
                                {
                                    var metyStock = listMediStockMaty.FirstOrDefault(o => o.MEDI_STOCK_ID == item.ID && o.MATERIAL_TYPE_ID == checkMaty);
                                    if (metyStock != null)
                                    {
                                        metyStock.ALERT_MIN_IN_STOCK = medicineType.ALERT_MIN_IN_STOCK_STR;
                                        metyStock.ALERT_MAX_IN_STOCK = medicineType.ALERT_MAX_IN_STOCK_STR;
                                        metyStock.EXP_MEDI_STOCK_ID = medicineType.EXP_MEDI_STOCK_ID;
                                        metyStock.IS_GOODS_RESTRICT = short.Parse(medicineType.IS_GOODS_RESTRICT == true ? "1" : "0");
                                        metyStock.IS_PREVENT_MAX = short.Parse(medicineType.IS_PREVENT_MAX == true ? "1" : "0");
                                        stockMetyUpdates.Add(metyStock);
                                    }
                                }
                                if (stockMetyUpdates != null && stockMetyUpdates.Count > 0)
                                {
                                    var updateResult = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_MATY>>(
                                               "/api/HisMediStockMaty/UpdateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               stockMetyUpdates,
                                               param);
                                    if (updateResult != null && updateResult.Count > 0)
                                    {
                                        //listMediStockMaty.AddRange(updateResult);
                                        success = true;
                                    }
                                }
                            }

                            if (dataCreates != null && dataCreates.Count > 0 && medicineType != null)
                            {
                                List<HIS_MEDI_STOCK_MATY> matyStockCreates = new List<HIS_MEDI_STOCK_MATY>();
                                foreach (var item in dataCreates)
                                {
                                    HIS_MEDI_STOCK_MATY matyStock = new HIS_MEDI_STOCK_MATY();
                                    matyStock.MEDI_STOCK_ID = item.ID;
                                    matyStock.MATERIAL_TYPE_ID = checkMaty;
                                    matyStock.ALERT_MIN_IN_STOCK = medicineType.ALERT_MIN_IN_STOCK_STR;
                                    matyStock.ALERT_MAX_IN_STOCK = medicineType.ALERT_MAX_IN_STOCK_STR;
                                    matyStock.EXP_MEDI_STOCK_ID = medicineType.EXP_MEDI_STOCK_ID;
                                    matyStock.IS_GOODS_RESTRICT = short.Parse(medicineType.IS_GOODS_RESTRICT == true ? "1" : "0");
                                    matyStock.IS_PREVENT_MAX = short.Parse(medicineType.IS_PREVENT_MAX == true ? "1" : "0");
                                    matyStockCreates.Add(matyStock);
                                }

                                var createResult = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_MATY>>(
                                           "/api/HisMediStockMaty/CreateList",
                                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                           matyStockCreates,
                                           param);
                                if (createResult != null && createResult.Count > 0)
                                {
                                    listMediStockMaty.AddRange(createResult);
                                    success = true;
                                }

                            }
                            if (success)
                            {
                                BackendDataWorker.Reset<HIS_MEDI_STOCK_MATY>();
                            }
                            WaitingManager.Hide();
                            #region Show message
                            MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            SessionManager.ProcessTokenLost(param);
                            #endregion
                            medicineStocks = medicineStocks.OrderByDescending(p => p.checkMest).ToList();
                            stockProcessor.Reload(ucStock, medicineStocks);
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Loại vật tư");
                        }
                    }
                    #endregion
                    //var mediMetyCheckeds = medicineMatys.Where(p => (p.check1 == true)).ToList();
                    //var mediStockCheckeds = medicineStocks.Where(p => (p.checkMest == true)).ToList();

                    //if (mediMetyCheckeds.Count == 0 && mediStockCheckeds.Count == 0)
                    //{
                    //    param.Messages.Add(String.Format(ResourceMessage.ChuaChonKhoLoaiThuoc));
                    //}
                    //if (mediMetyCheckeds.Count == 0 && mediStockCheckeds.Count != 0)
                    //{
                    //    param.Messages.Add(String.Format(ResourceMessage.ChuaChonLoaiVatTu));
                    //} if (mediMetyCheckeds.Count != 0 && mediStockCheckeds.Count == 0)
                    //{
                    //    param.Messages.Add(String.Format(ResourceMessage.ChuaChonKho));
                    //}
                    //MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void Save()
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

        private void txtKeyword1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch1_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch2_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void Grid_RowCellClick(V_HIS_MEDI_STOCK data)
        {
            try
            {
                if (data != null && listMediStockMaty != null && listMediStockMaty.Count > 0)
                {
                    HIS_MEDI_STOCK_MATY mediStockMaty = listMediStockMaty.FirstOrDefault(o => o.MEDI_STOCK_ID == data.ID);
                    if (mediStockMaty != null)
                    {
                        matyProcessor.ReloadRow(ucMaty, mediStockMaty);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                                if (isChoseStock != 1)
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
                                if (this.currentCopyMedistockAdo == null && isChoseStock != 1)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyMedistockAdo != null && currentPaste != null && isChoseStock == 1)
                                {
                                    if (this.currentCopyMedistockAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisMestMatyCopyByMediStockSDO hisMestMetyCopyByMediStockSDO = new HisMestMatyCopyByMediStockSDO();
                                    hisMestMetyCopyByMediStockSDO.CopyMediStockId = currentCopyMedistockAdo.ID;
                                    hisMestMetyCopyByMediStockSDO.PasteMediStockId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_MATY>>("api/HisMediStockMaty/CopyByMediStock", ApiConsumer.ApiConsumers.MosConsumer, hisMestMetyCopyByMediStockSDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        listMediStockMaty = result;
                                        List<HIS.UC.ListMaterialType.ListMaterialTypeADO> dataNew = new List<HIS.UC.ListMaterialType.ListMaterialTypeADO>();
                                        dataNew = (from r in listMaty select new HIS.UC.ListMaterialType.ListMaterialTypeADO(r)).ToList();
                                        if (listMediStockMaty != null && listMediStockMaty.Count > 0)
                                        {

                                            foreach (var item in listMediStockMaty)
                                            {
                                                var mediStockMety = dataNew.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                                                if (mediStockMety != null)
                                                {
                                                    mediStockMety.check1 = true;
                                                    mediStockMety.ALERT_MAX_IN_STOCK_STR = item.ALERT_MAX_IN_STOCK;
                                                    mediStockMety.ALERT_MIN_IN_STOCK_STR = item.ALERT_MIN_IN_STOCK;
                                                    mediStockMety.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
                                                    mediStockMety.IS_GOODS_RESTRICT = item.IS_GOODS_RESTRICT == 1 ? true : false;
                                                    mediStockMety.IS_PREVENT_MAX = item.IS_PREVENT_MAX == 1 ? true : false;
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                                            if (ucMaty != null)
                                            {
                                                matyProcessor.Reload(ucMaty, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid2(this);
                                        }
                                        checkRa = true;
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

        private void MatyGridView_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.ListMaterialType.ListMaterialTypeADO)
                {
                    var type = (HIS.UC.ListMaterialType.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.ListMaterialType.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseMaty != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn vật tư!");
                                    break;
                                }
                                this.currentCopyMaterialTypeAdo = (HIS.UC.ListMaterialType.ListMaterialTypeADO)sender;
                                break;
                            }
                        case HIS.UC.ListMaterialType.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.ListMaterialType.ListMaterialTypeADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyMaterialTypeAdo == null && isChoseMaty != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyMaterialTypeAdo != null && currentPaste != null && isChoseMaty == 2)
                                {
                                    if (this.currentCopyMaterialTypeAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisMestMatyCopyByMatySDO hisMestMatyCopyByMatySDO = new HisMestMatyCopyByMatySDO();
                                    hisMestMatyCopyByMatySDO.CopyMaterialTypeId = this.currentCopyMaterialTypeAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteMaterialTypeId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_MATY>>("api/HisMediStockMaty/CopyByMaty", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        listMediStockMaty = result;
                                        List<HIS.UC.MediStock.MediStockADO> dataNew = new List<HIS.UC.MediStock.MediStockADO>();
                                        dataNew = (from r in listStock select new HIS.UC.MediStock.MediStockADO(r)).ToList();
                                        if (listMediStockMaty != null && listMediStockMaty.Count > 0)
                                        {

                                            foreach (var itemStock in listMediStockMaty)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemStock.MEDI_STOCK_ID);
                                                if (check != null)
                                                {
                                                    check.checkMest = true;
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.checkMest).ToList();
                                            if (ucStock != null)
                                            {
                                                stockProcessor.Reload(ucStock, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid1(this);
                                        }
                                        checkRa = true;
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        case HIS.UC.ListMaterialType.Popup.PopupMenuProcessor.ItemType.UpdateTrustAmount:
                            {
                                this.processUpdateTrustAmount(null);
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



        private void processUpdateTrustAmount(long? TypeId)
        {
            bool success = false;
            try
            {
                if (isChoseMaty != 1)
                {
                    MessageManager.Show("Vui lòng chọn kho!");
                    return;
                }
                List<HIS.UC.ListMaterialType.ListMaterialTypeADO> Ados = matyProcessor.GetDataGridView(ucMaty) as List<HIS.UC.ListMaterialType.ListMaterialTypeADO>;
                if (Ados == null || Ados.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong load dc danh sach vat tu");
                    return;
                }
                List<HIS.UC.MediStock.MediStockADO> MstAdos = stockProcessor.GetDataGridView(ucStock) as List<HIS.UC.MediStock.MediStockADO>;

                var listMstAdoCheck = MstAdos.Where(o => o.radioMest == true).ToList();
                if (listMstAdoCheck == null || listMstAdoCheck.Count == 0)
                {
                    MessageBox.Show("Chưa chọn kho", "Thông báo");
                    return;
                }


                if (listMstAdoCheck.First().ID == 0)
                {
                    MessageBox.Show("Chưa chọn kho", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                HisMediStockSetRealBaseAmountSDO sdo = new HisMediStockSetRealBaseAmountSDO();
                sdo.MediStockId = listMstAdoCheck.First().ID;
                if (TypeId != null && TypeId > 0)
                {
                    sdo.TypeIds = new List<long>() { TypeId.Value };
                }
                else
                {
                    var listAdoCheck = Ados.Where(o => o.check1 == true).ToList();
                    if (listAdoCheck == null || listAdoCheck.Count == 0)
                    {
                        MessageBox.Show("Chưa chọn vật tư", "Thông báo");
                        return;
                    }
                    sdo.TypeIds = listAdoCheck.Select(o => o.ID).ToList();
                }
                WaitingManager.Show();

                List<HIS_MEDI_STOCK_MATY> mediStockMety = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_MATY>>("api/HisMediStockMaty/SetRealBaseAmount", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                //CloseTreatmentProcessor.TreatmentUnFinish(, param);
                WaitingManager.Hide();
                if (mediStockMety != null && mediStockMety.Count > 0)
                {
                    success = true;
                    foreach (var item in Ados)
                    {
                        HIS_MEDI_STOCK_MATY updatedAmountBase = mediStockMety.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.ID && o.MEDI_STOCK_ID == listMstAdoCheck.First().ID);
                        if (updatedAmountBase != null)
                        {
                            item.check1 = true;
                            if (!this.listMediStockMaty.Exists(o => o.ID == updatedAmountBase.ID))
                            {
                                this.listMediStockMaty.Add(updatedAmountBase);
                            }
                        }

                    }
                    matyProcessor.Reload(ucMaty, Ados);
                }

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


       /* private bool MatyGridView_ValidatingEditor(object sender, out string errorText)
        {
            bool result = true;
            errorText = "";
            try
            {
                ColumnView view = sender as ColumnView;
                HIS.UC.ListMaterialType.ListMaterialTypeADO currentMaty = (HIS.UC.ListMaterialType.ListMaterialTypeADO)sender;
                if (view.FocusedColumn.FieldName == "ALERT_MAX_IN_STOCK_STR")
                {
                    if (!currentMaty.TRUST_AMOUNT_IN_STOCK_STR.HasValue)
                    {
                        result = false;
                        errorText = "Bạn phải thực hiện cập nhật \"Cơ số thực tế\" trước khi nhập \"Cơ số\"";
                    }
                    else if (currentMaty.ALERT_MAX_IN_STOCK_STR.HasValue && currentMaty.ALERT_MAX_IN_STOCK_STR < currentMaty.TRUST_AMOUNT_IN_STOCK_STR)
                    {
                        result = false;
                        errorText = "Không cho phép nhập \"Cơ số\" nhỏ hơn \"Cơ số thực tế\"";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
                result = true;
            }
            return result;
        }
        */
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            BackendDataWorker.Reset<HIS_MEDI_STOCK_MATY>();
            BackendDataWorker.Reset<V_HIS_SERVICE>();
            BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
            BackendDataWorker.Reset<V_HIS_MATERIAL_TYPE>();
            BackendDataWorker.Reset<V_HIS_SERVICE_ROOM>();
            BackendDataWorker.Reset<V_HIS_MATERIAL_PATY>();
            MessageManager.Show("Xử lý thành công");
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            BackendDataWorker.CacheMonitorSyncExecute((typeof(HIS_MEDI_STOCK_MATY)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_PATY)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_MATERIAL_TYPE)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_ROOM)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_MATERIAL_PATY)).ToString(), false);
            MessageManager.Show("Xử lý thành công");
        }
        public void refreshForm()
        {
            btnSearch2_Click(null, null);
        }
        public void ImportShortcut()
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)refreshForm);
                if (this.currentModule != null)
                {
                    CallModule callModule = new CallModule(CallModule.HisImportServiceRoom, currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisImportServiceRoom, 0, 0, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
