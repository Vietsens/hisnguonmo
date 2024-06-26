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
using MOS.EFMODEL.DataModels;
using HIS.UC.ListMedicineType;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using HIS.UC.ListAntigen;
using HIS.UC.ListAntigen.ADO;
using Inventec.Common.Adapter;
using MOS.Filter;
using HIS.UC.ListMedicineType.ADO;
using HIS.Desktop.Plugins.AntigenMety.Entity;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.AntigenMety.Resources;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraBars;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData.ADO;

namespace HIS.Desktop.Plugins.AntigenMety
{
    public partial class UCAntigenMetyList : UserControl
    {
        internal List<HIS.UC.ListMedicineType.ListMedicineTypeADO> MetyAdo { get; set; }
        internal List<HIS.UC.ListAntigen.ListAntigenADO> StockAdo { get; set; }
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        long checkStock = 0;
        long checkMety = 0;
        HIS.UC.ListMedicineType.UCListMedicineTypeProcessor metyProcessor = null;
        UCListAntigenProcessor stockProcessor = null;
        UserControl ucMety;
        UserControl ucAntigen;
        long isChoseStock = 0;
        long isChoseMety = 0;
        bool isCheckAll;
        bool checkRa = false;
        HIS_ANTIGEN mediStock = new HIS_ANTIGEN();
        V_HIS_MEDICINE_TYPE medicineType = new V_HIS_MEDICINE_TYPE();
        List<V_HIS_MEDICINE_TYPE> listMety = new List<V_HIS_MEDICINE_TYPE>();
        List<HIS_ANTIGEN> listStock = new List<HIS_ANTIGEN>();
        List<HIS_ANTIGEN_METY> listMediStockMety = new List<HIS_ANTIGEN_METY>();

        HIS.UC.ListAntigen.ListAntigenADO currentCopyListAntigenADO = null;
        HIS.UC.ListMedicineType.ListMedicineTypeADO CurrentCopyMedicineTypeAdo = null;

        public UCAntigenMetyList()
        {
            InitializeComponent();
            try
            {
                InitMety();
                InitAntigen();
                LoadComboStatus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (ucMety != null && ucAntigen != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    List<HIS.UC.ListMedicineType.ListMedicineTypeADO> medicineMetys = metyProcessor.GetDataGridView(ucMety) as List<HIS.UC.ListMedicineType.ListMedicineTypeADO>;
                    List<HIS.UC.ListAntigen.ListAntigenADO> medicineStocks = stockProcessor.GetDataGridView(ucAntigen) as List<HIS.UC.ListAntigen.ListAntigenADO>;
                    if (isChoseStock == 1 && medicineMetys != null && medicineMetys.Count > 0)
                    {
                        if (medicineMetys != null && medicineMetys.Count > 0)
                        {
                            if (checkRa == true)
                            {
                                var dataCheckeds = medicineMetys.Where(p => (p.check1 == true)).ToList();

                                //List xóa
                                var dataDeletes = medicineMetys.Where(o => listMediStockMety.Select(p => p.MEDICINE_TYPE_ID)
                               .Contains(o.ID) && o.check1 == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !listMediStockMety.Select(p => p.MEDICINE_TYPE_ID)
                                    .Contains(o.ID)).ToList();

                                // List update
                                var dataUpdate = dataCheckeds.Where(o => listMediStockMety.Select(p => p.MEDICINE_TYPE_ID)
                                    .Contains(o.ID)).ToList();


                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    var stockMetyUpdates = new List<HIS_ANTIGEN_METY>();
                                    foreach (var item in dataUpdate)
                                    {
                                        var mediStockMety = listMediStockMety.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.ID && o.ANTIGEN_ID == checkStock);
                                        if (mediStockMety != null)
                                        {
                                            stockMetyUpdates.Add(mediStockMety);
                                        }
                                    }
                                    if (stockMetyUpdates != null && stockMetyUpdates.Count > 0)
                                    {
                                        var updateResult = new BackendAdapter(param).Post<List<HIS_ANTIGEN_METY>>(
                                                   "/api/HisAntigenMety/UpdateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   stockMetyUpdates,
                                                   param);
                                        if (updateResult != null && updateResult.Count > 0)
                                        {
                                            //listMediStockMety.AddRange(updateResult);
                                            success = true;
                                        }
                                    }
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 15)
                                {
                                    List<long> deleteIds = listMediStockMety.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.MEDICINE_TYPE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "/api/HisAntigenMety/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteIds,
                                              param);
                                    if (deleteResult)
                                    {
                                        listMediStockMety = listMediStockMety.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                        success = true;
                                    }
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_ANTIGEN_METY> stockMetyCreates = new List<HIS_ANTIGEN_METY>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_ANTIGEN_METY stockMety = new HIS_ANTIGEN_METY();
                                        stockMety.MEDICINE_TYPE_ID = item.ID;
                                        stockMety.ANTIGEN_ID = checkStock;
                                        stockMetyCreates.Add(stockMety);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_ANTIGEN_METY>>(
                                               "/api/HisAntigenMety/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               stockMetyCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        listMediStockMety.AddRange(createResult);
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
                                medicineMetys = medicineMetys.OrderByDescending(p => p.check1).ToList();
                                metyProcessor.Reload(ucMety, medicineMetys);

                            }
                            else
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Kho");
                            }
                        }
                    }

                    if (isChoseMety == 2 && medicineStocks != null && medicineStocks.Count > 0)
                    {
                        if (checkRa == true)
                        {
                            HIS.UC.ListMedicineType.ListMedicineTypeADO medicineType = medicineMetys.FirstOrDefault(o => o.ID == checkMety);

                            var dataCheckeds = medicineStocks.Where(p => (p.check1 == true)).ToList();

                            //List xóa
                            var dataDeletes = medicineStocks.Where(o => listMediStockMety.Select(p => p.ANTIGEN_ID)
                           .Contains(o.ID) && o.check1 == false).ToList();

                            //list them
                            var dataCreates = dataCheckeds.Where(o => !listMediStockMety.Select(p => p.ANTIGEN_ID)
                                .Contains(o.ID)).ToList();

                            //list update
                            var dataUpdates = dataCheckeds.Where(o => listMediStockMety.Select(p => p.ANTIGEN_ID)
                                .Contains(o.ID)).ToList();


                            if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 5)
                            {
                                List<long> deleteIds = listMediStockMety.Where(o => dataDeletes.Select(p => p.ID)
                                    .Contains(o.ANTIGEN_ID)).Select(o => o.ID).ToList();
                                bool deleteResult = new BackendAdapter(param).Post<bool>(
                                          "/api/HisAntigenMety/DeleteList",
                                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                          deleteIds,
                                          param);
                                if (deleteResult)
                                {
                                    listMediStockMety = listMediStockMety.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                    success = true;
                                }
                            }

                            if (dataUpdates != null && dataUpdates.Count > 0 && medicineType != null)
                            {
                                // List<HIS_ANTIGEN_METY> stockMetyUpdates = new List<HIS_ANTIGEN_METY>();
                                var stockMetyUpdates = new List<HIS_ANTIGEN_METY>();
                                foreach (var item in dataUpdates)
                                {
                                    var metyStock = listMediStockMety.FirstOrDefault(o => o.ANTIGEN_ID == item.ID && o.MEDICINE_TYPE_ID == checkMety);
                                    if (metyStock != null)
                                    {
                                        stockMetyUpdates.Add(metyStock);
                                    }
                                }
                                if (stockMetyUpdates != null && stockMetyUpdates.Count > 0)
                                {
                                    var updateResult = new BackendAdapter(param).Post<List<HIS_ANTIGEN_METY>>(
                                               "/api/HisAntigenMety/UpdateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               stockMetyUpdates,
                                               param);
                                    if (updateResult != null && updateResult.Count > 0)
                                    {
                                        //listMediStockMety.AddRange(updateResult);
                                        success = true;
                                    }
                                }
                            }

                            if (dataCreates != null && dataCreates.Count > 0 && medicineType != null)
                            {
                                List<HIS_ANTIGEN_METY> metyStockCreates = new List<HIS_ANTIGEN_METY>();
                                foreach (var item in dataCreates)
                                {
                                    HIS_ANTIGEN_METY metyStock = new HIS_ANTIGEN_METY();
                                    metyStock.ANTIGEN_ID = item.ID;
                                    metyStock.MEDICINE_TYPE_ID = checkMety;
                                    metyStockCreates.Add(metyStock);
                                }

                                var createResult = new BackendAdapter(param).Post<List<HIS_ANTIGEN_METY>>(
                                           "/api/HisAntigenMety/CreateList",
                                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                           metyStockCreates,
                                           param);
                                if (createResult != null && createResult.Count > 0)
                                {
                                    listMediStockMety.AddRange(createResult);
                                    success = true;

                                }
                            }
                            if (success)
                            {
                                BackendDataWorker.Reset<HIS_ANTIGEN_METY>();
                            }
                            WaitingManager.Hide();
                            #region Show message
                            MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            SessionManager.ProcessTokenLost(param);
                            #endregion
                            medicineStocks = medicineStocks.OrderByDescending(p => p.check1).ToList();
                            stockProcessor.Reload(ucAntigen, medicineStocks);
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Loại thuốc");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCMediStockMetyList_Load(object sender, EventArgs e)
        {
            FillDataToGrid1(this);
            FillDataToGrid2(this);
            SetCaptionByLanguageKey();
        }

        private void InitMety()
        {
            try
            {
                metyProcessor = new UCListMedicineTypeProcessor();
                ListMedicineTypeInitADO ado = new ListMedicineTypeInitADO();
                ado.ListMedicineTypeColumn = new List<UC.ListMedicineType.ListMedicineTypeColumn>();
                ado.gridViewMety_MouseDownMety = gridViewMety_MouseDownMety;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.GridView_MouseRightClick = GridViewMedicineType_MouseRightClick;

                ListMedicineTypeColumn colRadio1 = new ListMedicineTypeColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeColumn.Add(colRadio1);

                ListMedicineTypeColumn colCheck1 = new ListMedicineTypeColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = imgMety.Images[1];
                colCheck1.Caption = "Chọn tất cả";
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeColumn.Add(colCheck1);

                ListMedicineTypeColumn colMaPhong = new ListMedicineTypeColumn("Mã vắc xin", "MEDICINE_TYPE_CODE", 100, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListMedicineTypeColumn.Add(colMaPhong);

                ListMedicineTypeColumn colTenPhong = new ListMedicineTypeColumn("Tên vắc xin", "MEDICINE_TYPE_NAME", 200, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListMedicineTypeColumn.Add(colTenPhong);

                //ListMedicineTypeColumn colTran = new ListMedicineTypeColumn("Số lượng sàn", "ALERT_MIN_IN_STOCK_STR", 100, true);
                //colTran.VisibleIndex = 4;

                //ado.ListMedicineTypeColumn.Add(colTran);

                //ListMedicineTypeColumn colSan = new ListMedicineTypeColumn("Số lượng trần", "ALERT_MAX_IN_STOCK", 100, true);
                //colSan.VisibleIndex = 5;
                //colSan.Visible = false;
                //colSan.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListMedicineTypeColumn.Add(colSan);

                //ListMedicineTypeColumn colQuaTran = new ListMedicineTypeColumn("Chặn nhập quá trần", "IS_PREVENT_MAX", 100, true);
                //colQuaTran.VisibleIndex = 6;
                //colQuaTran.Visible = false;
                ////colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListMedicineTypeColumn.Add(colQuaTran);

                //ListMedicineTypeColumn colKoXuat = new ListMedicineTypeColumn("Không cho xuất", "IS_PREVENT_EXP", 100, true);
                //colKoXuat.VisibleIndex = 7;
                //colKoXuat.Visible = false;
                ////colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListMedicineTypeColumn.Add(colKoXuat);

                //ListMedicineTypeColumn colGioiHan = new ListMedicineTypeColumn("Thuốc giới hạn", "IsGoodsRetrict", 100, true);
                //colGioiHan.VisibleIndex = 8;
                //colGioiHan.Visible = false;
                ////colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //colGioiHan.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListMedicineTypeColumn.Add(colGioiHan);

                this.ucMety = (UserControl)metyProcessor.Run(ado);
                if (ucMety != null)
                {
                    this.panelControl2.Controls.Add(this.ucMety);
                    this.ucMety.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }        

        private void FillDataToMety(UCAntigenMetyList _Mety)
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
                FillDataToGridMety(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridMety, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMety(object data)
        {
            try
            {
                WaitingManager.Show();
                listMety = new List<V_HIS_MEDICINE_TYPE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisMedicineTypeViewFilter metyFilter = new MOS.Filter.HisMedicineTypeViewFilter();
                metyFilter.ORDER_FIELD = "MEDICINE_TYPE_NAME";
                metyFilter.ORDER_DIRECTION = "ASC";
                metyFilter.KEY_WORD = txtKeyword2.Text;
                //metyFilter.IS_VACCINE = 1;//TODO   
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseStock = (long)cboChoose.EditValue;
                }
                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseStock = (long)cboChoose.EditValue;
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>>(
                    "api/HisMedicineType/GetView",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    metyFilter,
                    param);

                MetyAdo = new List<HIS.UC.ListMedicineType.ListMedicineTypeADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listMety = rs.Data.Where(o => o.IS_VACCINE == 1).ToList();//TODO bỏ sau khi filter có đk lọc
                    foreach (var item in listMety)
                    {
                        HIS.UC.ListMedicineType.ListMedicineTypeADO metypeADO = new HIS.UC.ListMedicineType.ListMedicineTypeADO(item);
                        if (isChoseStock == 2)
                        {
                            metypeADO.isKeyChoose = true;
                        }
                        MetyAdo.Add(metypeADO);
                    }
                }
                if (listMediStockMety != null && listMediStockMety.Count > 0)
                {
                    foreach (var item in listMediStockMety)
                    {
                        var mediStockMety = MetyAdo.FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                        if (mediStockMety != null)
                        {
                            mediStockMety.check1 = true;
                        }
                    }
                }
                MetyAdo = MetyAdo.OrderByDescending(p => p.check1).ToList();
                if (ucMety != null)
                {
                    metyProcessor.Reload(ucMety, MetyAdo);
                }
                rowCount = (data == null ? 0 : MetyAdo.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2(UCAntigenMetyList _roomtype)
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
                FillDataToGridMety(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridMety, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch2.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitAntigen()
        {
            try
            {
                stockProcessor = new UCListAntigenProcessor();
                ListAntigenInitADO ado = new ListAntigenInitADO();
                ado.ListAntigenColumn = new List<ListAntigenColumn>();
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click1;
                ado.gridViewMety_MouseDownMety = gridViewStock_MouseDownStock;
                ado.ListDepositReqGrid_RowCellClick = Grid_RowCellClick;
                ado.GridView_MouseRightClick = MedistockGridView_MouseRightClick;

                ListAntigenColumn colRadio2 = new ListAntigenColumn("   ", "radio1", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAntigenColumn.Add(colRadio2);

                ListAntigenColumn colCheck2 = new ListAntigenColumn("   ", "check1", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imgStock.Images[1];
                colCheck2.Caption = "Chọn tất cả";
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAntigenColumn.Add(colCheck2);

                ListAntigenColumn colMaPhong = new ListAntigenColumn("Mã kháng nguyên", "ANTIGEN_CODE", 80, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListAntigenColumn.Add(colMaPhong);

                ListAntigenColumn colTenPhong = new ListAntigenColumn("Tên kháng nguyên", "ANTIGEN_NAME", 150, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListAntigenColumn.Add(colTenPhong);


                this.ucAntigen = (UserControl)stockProcessor.Run(ado);
                if (ucAntigen != null)
                {
                    this.panelControl1.Controls.Add(this.ucAntigen);
                    this.ucAntigen.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToStock(UCAntigenMetyList _Stock)
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
                ucPaging1.Init(FillDataToGridStock, param);
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
                listStock = new List<HIS_ANTIGEN>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisAntigenFilter stockFilter = new MOS.Filter.HisAntigenFilter();
                stockFilter.ORDER_FIELD = "ANTIGEN_NAME";
                stockFilter.ORDER_DIRECTION = "ASC";
                stockFilter.KEY_WORD = txtKeyword1.Text;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseMety = (long)cboChoose.EditValue;
                }
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseMety = (long)cboChoose.EditValue;
                }
                var rs = new BackendAdapter(param).GetRO<List<HIS_ANTIGEN>>(
                    "api/HisAntigen/Get",
                    ApiConsumers.MosConsumer,
                    stockFilter,
                    param);

                StockAdo = new List<HIS.UC.ListAntigen.ListAntigenADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listStock = rs.Data;
                    foreach (var item in listStock)
                    {                       
                        HIS.UC.ListAntigen.ListAntigenADO ListAntigenADO = new HIS.UC.ListAntigen.ListAntigenADO(item);
                        if (isChoseMety == 1)
                        {
                            ListAntigenADO.isKeyChoose = true;
                        }
                        StockAdo.Add(ListAntigenADO);
                    }
                }
                if (listMediStockMety != null && listMediStockMety.Count > 0)
                {

                    foreach (var itemStock in listMediStockMety)
                    {
                        var check = StockAdo.FirstOrDefault(o => o.ID == itemStock.ANTIGEN_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                StockAdo = StockAdo.OrderByDescending(p => p.check1).ToList();
                if (ucAntigen != null)
                {
                    stockProcessor.Reload(ucAntigen, StockAdo);
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

        private void FillDataToGrid1(UCAntigenMetyList _Stock)
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
                ucPaging1.Init(FillDataToGridStock, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click1(HIS_ANTIGEN data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisAntigenMetyFilter filter = new HisAntigenMetyFilter();
                filter.MEDI_STOCK_ID = data.ID;
                checkStock = data.ID;
                listMediStockMety = new List<HIS_ANTIGEN_METY>();
                listMediStockMety = new BackendAdapter(param).Get<List<HIS_ANTIGEN_METY>>(
                                RequestUriStore.HIS_ANTIGEN_METY_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.ListMedicineType.ListMedicineTypeADO> dataNew = new List<HIS.UC.ListMedicineType.ListMedicineTypeADO>();
                dataNew = (from r in listMety select new HIS.UC.ListMedicineType.ListMedicineTypeADO(r)).ToList();
                if (listMediStockMety != null && listMediStockMety.Count > 0)
                {
                    foreach (var item in listMediStockMety)
                    {
                        var mediStockMety = dataNew.FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                        if (mediStockMety != null)
                        {
                            mediStockMety.check1 = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    if (ucMety != null)
                    {
                        metyProcessor.Reload(ucMety, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid2(this);
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

        private void btn_Radio_Enable_Click(V_HIS_MEDICINE_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisAntigenMetyFilter filter = new HisAntigenMetyFilter();
                filter.MEDICINE_TYPE_ID = data.ID;

                checkMety = data.ID;
                //filter.
                listMediStockMety = new List<HIS_ANTIGEN_METY>();
                listMediStockMety = new BackendAdapter(param).Get<List<HIS_ANTIGEN_METY>>(
                                RequestUriStore.HIS_ANTIGEN_METY_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.ListAntigen.ListAntigenADO> dataNew = new List<HIS.UC.ListAntigen.ListAntigenADO>();
                dataNew = (from r in listStock select new HIS.UC.ListAntigen.ListAntigenADO(r)).ToList();
                if (listMediStockMety != null && listMediStockMety.Count > 0)
                {

                    foreach (var itemStock in listMediStockMety)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemStock.ANTIGEN_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    if (ucAntigen != null)
                    {
                        stockProcessor.Reload(ucAntigen, dataNew);
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

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Vắc xin"));
                status.Add(new Status(2, "Loại thuốc"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                cboChoose.EditValue = status[0].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboChoose_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                checkRa = false;
                isChoseStock = 0;
                isChoseMety = 0;
                listMediStockMety = new List<HIS_ANTIGEN_METY>();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCheckAll1_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCheckAll2_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Save()
        {
            try
            {
                btnSave.Focus();
                simpleButton1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMety_MouseDownMety(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseMety == 2)
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
                            var lstCheckAll = MetyAdo;
                            List<HIS.UC.ListMedicineType.ListMedicineTypeADO> lstChecks = new List<HIS.UC.ListMedicineType.ListMedicineTypeADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var roomCheckedNum = MetyAdo.Where(o => o.check1 == true).Count();
                                var roomNum = MetyAdo.Count();
                                if ((roomCheckedNum > 0 && roomCheckedNum < roomNum) || roomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imgMety.Images[0];
                                }

                                if (roomCheckedNum == roomNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imgMety.Images[1];
                                }
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID > 0)
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
                                        if (item.ID > 0)
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

                                metyProcessor.Reload(ucMety, lstChecks);
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
                        if (hi.Column.FieldName == "check1")
                        {
                            var lstCheckAll = StockAdo;
                            List<HIS.UC.ListAntigen.ListAntigenADO> lstChecks = new List<HIS.UC.ListAntigen.ListAntigenADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var roomCheckedNum = StockAdo.Where(o => o.check1 == true).Count();
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
                                        if (item.ID > 0)
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
                                        if (item.ID > 0)
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

                                stockProcessor.Reload(ucAntigen, lstChecks);
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

        private void btnSearch1_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid1(this);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AntigenMety.Resources.Lang", typeof(HIS.Desktop.Plugins.AntigenMety.UCAntigenMetyList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch1.Text = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.btnSearch1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch2.Text = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.btnSearch2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Grid_RowCellClick(HIS_ANTIGEN data)
        {
            try
            {
                if (data != null && listMediStockMety != null && listMediStockMety.Count > 0)
                {
                    HIS_ANTIGEN_METY mediStockMety = listMediStockMety.FirstOrDefault(o => o.ANTIGEN_ID == data.ID);
                    if (mediStockMety != null)
                    {
                        metyProcessor.ReloadRow(ucMety, mediStockMety);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridViewMedicineType_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.ListMedicineType.ListMedicineTypeADO)
                {
                    var type = (HIS.UC.ListMedicineType.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.ListMedicineType.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseMety != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn thuốc!");
                                    break;
                                }
                                this.CurrentCopyMedicineTypeAdo = (HIS.UC.ListMedicineType.ListMedicineTypeADO)sender;
                                break;
                            }
                        case HIS.UC.ListMedicineType.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.ListMedicineType.ListMedicineTypeADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.CurrentCopyMedicineTypeAdo == null && isChoseMety != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.CurrentCopyMedicineTypeAdo != null && currentPaste != null && isChoseMety == 2)
                                {
                                    if (this.CurrentCopyMedicineTypeAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisMestMetyCopyByMetySDO hisMestMetyCopyByMetySDO = new HisMestMetyCopyByMetySDO();
                                    hisMestMetyCopyByMetySDO.CopyMedicineTypeId = this.CurrentCopyMedicineTypeAdo.ID;
                                    hisMestMetyCopyByMetySDO.PasteMedicineTypeId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_ANTIGEN_METY>>("api/HisAntigenMety/CopyByMety", ApiConsumer.ApiConsumers.MosConsumer, hisMestMetyCopyByMetySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        listMediStockMety = result;
                                        List<HIS.UC.ListAntigen.ListAntigenADO> dataNew = new List<HIS.UC.ListAntigen.ListAntigenADO>();
                                        dataNew = (from r in listStock select new HIS.UC.ListAntigen.ListAntigenADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {

                                            foreach (var itemStock in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemStock.ANTIGEN_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                                            if (ucAntigen != null)
                                            {
                                                stockProcessor.Reload(ucAntigen, dataNew);
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

        private void MedistockGridView_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.ListAntigen.ListAntigenADO)
                {
                    var type = (HIS.UC.ListAntigen.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.ListAntigen.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseStock != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn kho!");
                                    break;
                                }
                                this.currentCopyListAntigenADO = (HIS.UC.ListAntigen.ListAntigenADO)sender;
                                break;
                            }
                        case HIS.UC.ListAntigen.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.ListAntigen.ListAntigenADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyListAntigenADO == null && isChoseStock != 1)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyListAntigenADO != null && currentPaste != null && isChoseStock == 1)
                                {
                                    if (this.currentCopyListAntigenADO.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisMestMetyCopyByMediStockSDO hisMestMetyCopyByMediStockSDO = new HisMestMetyCopyByMediStockSDO();
                                    hisMestMetyCopyByMediStockSDO.CopyMediStockId = currentCopyListAntigenADO.ID;
                                    hisMestMetyCopyByMediStockSDO.PasteMediStockId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_ANTIGEN_METY>>("api/HisAntigenMety/CopyByMediStock", ApiConsumer.ApiConsumers.MosConsumer, hisMestMetyCopyByMediStockSDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        listMediStockMety = result;
                                        List<HIS.UC.ListMedicineType.ListMedicineTypeADO> dataNew = new List<HIS.UC.ListMedicineType.ListMedicineTypeADO>();
                                        dataNew = (from r in listMety select new HIS.UC.ListMedicineType.ListMedicineTypeADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var item in result)
                                            {
                                                var mediStockMety = dataNew.FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                                                if (mediStockMety != null)
                                                {
                                                    mediStockMety.check1 = true;
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                                            if (ucMety != null)
                                            {
                                                metyProcessor.Reload(ucMety, dataNew);
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

        private void simpleButton2_Click(object sender, EventArgs e)
        {          
            BackendDataWorker.CacheMonitorSyncExecute((typeof(HIS_ANTIGEN_METY)).ToString(), false);
            MessageManager.Show("Xử lý thành công");
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            BackendDataWorker.Reset<HIS_ANTIGEN_METY>();
            MessageManager.Show("Xử lý thành công");
        }
    }
}
