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
using HIS.UC.MediStock;
using HIS.UC.MediStock.ADO;
using Inventec.Common.Adapter;
using MOS.Filter;
using HIS.UC.ListMedicineType;
using HIS.UC.ListMedicineType.ADO;
using HIS.Desktop.Plugins.HisBranchTime.Entity;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisBranchTime.Resources;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraBars;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.MediStockMetyList;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.HisBranchTime
{
    public partial class UCMediStockMetyList : HIS.Desktop.Utility.UserControlBase
    {
        internal List<HIS.UC.ListMedicineType.ListMedicineTypeADO> MetyAdo { get; set; }
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
        long checkMety = 0;
        UCListMedicineTypeProcessor metyProcessor = null;
        UCMediStockProcessor stockProcessor = null;
        UserControl ucMety;
        UserControl ucStock;
        long isChoseStock = 0;
        long isChoseMety = 0;
        bool isCheckAll;
        bool checkRa = false;
        V_HIS_MEDI_STOCK mediStock = new V_HIS_MEDI_STOCK();
        V_HIS_MEDICINE_TYPE medicineType = new V_HIS_MEDICINE_TYPE();
        List<V_HIS_MEDICINE_TYPE> listMety = new List<V_HIS_MEDICINE_TYPE>();
        List<V_HIS_MEDI_STOCK> listStock = new List<V_HIS_MEDI_STOCK>();
        List<HIS_MEDI_STOCK_METY> listMediStockMety = new List<HIS_MEDI_STOCK_METY>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        HIS.UC.MediStock.MediStockADO currentCopyMedistockAdo = null;
        HIS.UC.ListMedicineType.ListMedicineTypeADO CurrentCopyMedicineTypeAdo = null;
        public UCMediStockMetyList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                InitMety();
                InitMediStock();
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
                if (ucMety != null && ucStock != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Bat dau");
                    bool success = false;
                    CommonParam param = new CommonParam();
                    List<HIS.UC.ListMedicineType.ListMedicineTypeADO> medicineMetys = metyProcessor.GetDataGridView(ucMety) as List<HIS.UC.ListMedicineType.ListMedicineTypeADO>;
                    //if (medicineMetys.Exists(o => (o.TRUST_AMOUNT_IN_STOCK_STR > o.ALERT_MAX_IN_STOCK) || (o.ALERT_MAX_IN_STOCK != null && o.TRUST_AMOUNT_IN_STOCK_STR == null)))
                    //{
                    //    Inventec.Common.Logging.LogSystem.Error("Co so thuc te lon hon co so");
                    //    return;
                    //}
                    List<HIS.UC.MediStock.MediStockADO> medicineStocks = stockProcessor.GetDataGridView(ucStock) as List<HIS.UC.MediStock.MediStockADO>;
                    #region ---Kho
                    if (isChoseStock == 1 && medicineMetys != null && medicineMetys.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Kho_Buoc 1");
                        if (medicineMetys != null && medicineMetys.Count > 0)
                        {
                           
                            if (checkRa == true)
                            {
                                Inventec.Common.Logging.LogSystem.Info("Kho_Buoc 2");
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
                                Inventec.Common.Logging.LogSystem.Info("Bat dau cap nhat");
                                if (dataDeletes.Count == 0 && dataCreates.Count == 0 && dataUpdate.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn loại thuốc");
                                    return;
                                }
                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Info("Kho_Update");
                                    var stockMetyUpdates = new List<HIS_MEDI_STOCK_METY>();
                                    foreach (var item in dataUpdate)
                                    {
                                        var mediStockMety = listMediStockMety.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.ID && o.MEDI_STOCK_ID == checkStock);
                                        if (mediStockMety != null)
                                        {
                                            mediStockMety.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK_STR;
                                            mediStockMety.ALERT_MAX_IN_STOCK = item.ALERT_MAX_IN_STOCK;
                                            mediStockMety.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
                                            mediStockMety.IS_PREVENT_EXP = short.Parse(item.IS_PREVENT_EXP == true ? "1" : "0");
                                            mediStockMety.IS_PREVENT_MAX = short.Parse(item.IS_PREVENT_MAX == true ? "1" : "0");
                                            mediStockMety.IS_GOODS_RESTRICT = short.Parse(item.IsGoodsRetrict == true ? "1" : "0");
                                            stockMetyUpdates.Add(mediStockMety);
                                        }
                                    }
                                   
                                    if (stockMetyUpdates != null && stockMetyUpdates.Count > 0)
                                    {
                                        Inventec.Common.Logging.LogSystem.Info("Kho_Du lieu update:" + stockMetyUpdates.Count);
                                        var updateResult = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_METY>>(
                                                   "/api/HisMediStockMety/UpdateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   stockMetyUpdates,
                                                   param);
                                        
                                        if (updateResult != null && updateResult.Count > 0)
                                        {
                                            Inventec.Common.Logging.LogSystem.Info("Kho_Du lieu api update kho tra ve:" + updateResult.Count);
                                            //listMediStockMety.AddRange(updateResult);
                                            success = true;
                                        }
                                    }
                                }
                                if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 15)
                                {
                                    Inventec.Common.Logging.LogSystem.Info("Kho_Du lieu delete:" + dataDeletes.Count);
                                    List<long> deleteIds = listMediStockMety.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.MEDICINE_TYPE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "/api/HisMediStockMety/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteIds,
                                              param);
                                    if (deleteResult)
                                    {
                                        Inventec.Common.Logging.LogSystem.Info("Kho_Du lieu api delete kho tra ve:True");
                                        listMediStockMety = listMediStockMety.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                        
                                        success = true;
                                    }
                                }
                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Info("Kho_Create");
                                    List<HIS_MEDI_STOCK_METY> stockMetyCreates = new List<HIS_MEDI_STOCK_METY>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_MEDI_STOCK_METY stockMety = new HIS_MEDI_STOCK_METY();
                                        stockMety.MEDICINE_TYPE_ID = item.ID;
                                        stockMety.MEDI_STOCK_ID = checkStock;
                                        stockMety.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK_STR;
                                        stockMety.ALERT_MAX_IN_STOCK = item.ALERT_MAX_IN_STOCK;
                                        stockMety.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
                                        stockMety.IS_PREVENT_EXP = short.Parse(item.IS_PREVENT_EXP == true ? "1" : "0");
                                        stockMety.IS_PREVENT_MAX = short.Parse(item.IS_PREVENT_MAX == true ? "1" : "0");
                                        stockMety.IS_GOODS_RESTRICT = short.Parse(item.IsGoodsRetrict == true ? "1" : "0");
                                        stockMetyCreates.Add(stockMety);
                                    }
                                    Inventec.Common.Logging.LogSystem.Info("Kho_Du lieu create:" + stockMetyCreates.Count);
                                    var createResult = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_METY>>(
                                               "/api/HisMediStockMety/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               stockMetyCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        Inventec.Common.Logging.LogSystem.Info("Kho_Du lieu api create tra ve:" + stockMetyCreates.Count);
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
                        Inventec.Common.Logging.LogSystem.Info("Ket thuc buoc 1");
                    }
                    #endregion
                    #region ---Loai thuoc
                    if (isChoseMety == 2 && medicineStocks != null && medicineStocks.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Loai thuoc_Buoc 1");
                        if (checkRa == true)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Loai thuoc_Buoc 2");
                            HIS.UC.ListMedicineType.ListMedicineTypeADO medicineType = medicineMetys.FirstOrDefault(o => o.ID == checkMety);
                            var dataCheckeds = medicineStocks.Where(p => (p.checkMest == true)).ToList();
                            //List xóa
                            var dataDeletes = medicineStocks.Where(o => listMediStockMety.Select(p => p.MEDI_STOCK_ID)
                           .Contains(o.ID) && o.checkMest == false).ToList();
                            //list them
                            var dataCreates = dataCheckeds.Where(o => !listMediStockMety.Select(p => p.MEDI_STOCK_ID)
                                .Contains(o.ID)).ToList();
                            //list update
                            var dataUpdates = dataCheckeds.Where(o => listMediStockMety.Select(p => p.MEDI_STOCK_ID)
                                .Contains(o.ID)).ToList();
                            if (dataDeletes.Count == 0 && dataCreates.Count == 0 && dataUpdates.Count == 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn kho.");
                                return;
                            }
                            if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 5)
                            {
                                Inventec.Common.Logging.LogSystem.Info("Loai thuoc_Du lieu delete:" + dataDeletes.Count);
                                List<long> deleteIds = listMediStockMety.Where(o => dataDeletes.Select(p => p.ID)
                                    .Contains(o.MEDI_STOCK_ID)).Select(o => o.ID).ToList();
                                bool deleteResult = new BackendAdapter(param).Post<bool>(
                                          "/api/HisMediStockMety/DeleteList",
                                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                          deleteIds,
                                          param);
                                if (deleteResult)
                                {
                                    Inventec.Common.Logging.LogSystem.Info("Loai thuoc_Du lieu api delete tra ve:True");
                                    listMediStockMety = listMediStockMety.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                    success = true;
                                }
                            }
                            if (dataUpdates != null && dataUpdates.Count > 0 && medicineType != null)
                            {
                                Inventec.Common.Logging.LogSystem.Info("Loai thuoc_Update:");
                                // List<HIS_MEDI_STOCK_METY> stockMetyUpdates = new List<HIS_MEDI_STOCK_METY>();
                                var stockMetyUpdates = new List<HIS_MEDI_STOCK_METY>();
                                foreach (var item in dataUpdates)
                                {
                                    var metyStock = listMediStockMety.FirstOrDefault(o => o.MEDI_STOCK_ID == item.ID && o.MEDICINE_TYPE_ID == checkMety);
                                    if (metyStock != null)
                                    {
                                        metyStock.ALERT_MIN_IN_STOCK = medicineType.ALERT_MIN_IN_STOCK_STR;
                                        metyStock.ALERT_MAX_IN_STOCK = medicineType.ALERT_MAX_IN_STOCK;
                                        metyStock.EXP_MEDI_STOCK_ID = medicineType.EXP_MEDI_STOCK_ID;
                                        metyStock.IS_PREVENT_EXP = short.Parse(medicineType.IS_PREVENT_EXP == true ? "1" : "0");
                                        metyStock.IS_PREVENT_MAX = short.Parse(medicineType.IS_PREVENT_MAX == true ? "1" : "0");
                                        metyStock.IS_GOODS_RESTRICT = short.Parse(medicineType.IsGoodsRetrict == true ? "1" : "0");
                                        stockMetyUpdates.Add(metyStock);
                                    }
                                }
                                if (stockMetyUpdates != null && stockMetyUpdates.Count > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Info("Loai thuoc_Du lieu Update:" + stockMetyUpdates.Count);
                                    var updateResult = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_METY>>(
                                               "/api/HisMediStockMety/UpdateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               stockMetyUpdates,
                                               param);
                                    if (updateResult != null && updateResult.Count > 0)
                                    {
                                        Inventec.Common.Logging.LogSystem.Info("Loai thuoc_Du lieu api Update tra ve:" + updateResult.Count);
                                        //listMediStockMety.AddRange(updateResult);
                                        success = true;
                                    }
                                }
                            }
                            if (dataCreates != null && dataCreates.Count > 0 && medicineType != null)
                            {
                                Inventec.Common.Logging.LogSystem.Info("Loai thuoc_Create:" + dataCreates.Count);
                                List<HIS_MEDI_STOCK_METY> metyStockCreates = new List<HIS_MEDI_STOCK_METY>();
                                foreach (var item in dataCreates)
                                {
                                    HIS_MEDI_STOCK_METY metyStock = new HIS_MEDI_STOCK_METY();
                                    metyStock.MEDI_STOCK_ID = item.ID;
                                    metyStock.MEDICINE_TYPE_ID = checkMety;
                                    metyStock.ALERT_MIN_IN_STOCK = medicineType.ALERT_MIN_IN_STOCK_STR;
                                    metyStock.ALERT_MAX_IN_STOCK = medicineType.ALERT_MAX_IN_STOCK;
                                    metyStock.EXP_MEDI_STOCK_ID = medicineType.EXP_MEDI_STOCK_ID;
                                    metyStock.IS_PREVENT_EXP = short.Parse(medicineType.IS_PREVENT_EXP == true ? "1" : "0");
                                    metyStock.IS_PREVENT_MAX = short.Parse(medicineType.IS_PREVENT_MAX == true ? "1" : "0");
                                    metyStock.IS_GOODS_RESTRICT = short.Parse(medicineType.IsGoodsRetrict == true ? "1" : "0");
                                    metyStockCreates.Add(metyStock);
                                }
                                Inventec.Common.Logging.LogSystem.Info("Loai thuoc_Du lieu Create:" +metyStockCreates.Count);
                                var createResult = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_METY>>(
                                           "/api/HisMediStockMety/CreateList",
                                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                           metyStockCreates,
                                           param);
                                if (createResult != null && createResult.Count > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Info("Loai thuoc_Du lieu api Create tra ve:" + createResult.Count);
                                    listMediStockMety.AddRange(createResult);
                                    success = true;
                                }
                            }
                            if (success)
                            {
                                BackendDataWorker.Reset<HIS_MEDI_STOCK_METY>();
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
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Loại thuốc");
                        }
                        ////
                        //var mediMetyCheckeds = medicineMetys.Where(p => (p.check1 == true)).ToList();
                        //var mediStockCheckeds = medicineStocks.Where(p => (p.checkMest == true)).ToList();
                        //if (mediMetyCheckeds.Count == 0 && mediStockCheckeds.Count == 0)
                        //{
                        //    param.Messages.Add(String.Format(ResourceMessage.ChuaChonKhoLoaiThuoc));
                        //}
                        //else
                        //{
                        //    if (mediMetyCheckeds.Count == 0 && mediStockCheckeds.Count != 0)
                        //    {
                        //        param.Messages.Add(String.Format(ResourceMessage.ChuaChonLoaiThuoc));
                        //    }
                        //    if (mediMetyCheckeds.Count != 0 && mediStockCheckeds.Count == 0)
                        //    {
                        //        param.Messages.Add(String.Format(ResourceMessage.ChuaChonKho));
                        //    }
                        //}
                        //MessageManager.Show(this.ParentForm, param, success);
                    }
                    #endregion
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
            chkDoNotDisplayLock.Checked = true;
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
                ado.processUpdateTrustAmount = processUpdateTrustAmount;

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
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeColumn.Add(colCheck1);

              /*  ListMedicineTypeColumn colUpdateTrustAmount = new ListMedicineTypeColumn("   ", "updateTrustAmount", 30, true);
                colUpdateTrustAmount.VisibleIndex = 2;
                colUpdateTrustAmount.Caption = " ";
                colUpdateTrustAmount.ToolTip = "Cập nhật Cơ số thực tế";
                colUpdateTrustAmount.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeColumn.Add(colUpdateTrustAmount);*/

                ListMedicineTypeColumn colMaPhong = new ListMedicineTypeColumn("Mã loại thuốc", "MEDICINE_TYPE_CODE", 100, false);
                colMaPhong.VisibleIndex = 3;
                ado.ListMedicineTypeColumn.Add(colMaPhong);

                ListMedicineTypeColumn colTenPhong = new ListMedicineTypeColumn("Tên loại thuốc", "MEDICINE_TYPE_NAME", 200, false);
                colTenPhong.VisibleIndex = 4;
                ado.ListMedicineTypeColumn.Add(colTenPhong);

                ListMedicineTypeColumn colDonVi = new ListMedicineTypeColumn("Đơn vị tính", "SERVICE_UNIT_NAME", 90, false);
                colDonVi.VisibleIndex = 5;
                colDonVi.Visible = false;
                ado.ListMedicineTypeColumn.Add(colDonVi);

                ListMedicineTypeColumn colMaHoaChat = new ListMedicineTypeColumn("Mã hoạt chất", "ACTIVE_INGR_BHYT_CODE", 100, false);
                colMaHoaChat.VisibleIndex = 6;
                colMaHoaChat.Visible = false;
                ado.ListMedicineTypeColumn.Add(colMaHoaChat);

                ListMedicineTypeColumn colTenHoatChat = new ListMedicineTypeColumn("Tên hoạt chất", "ACTIVE_INGR_BHYT_NAME", 200, false);
                colTenHoatChat.VisibleIndex = 7;
                colTenHoatChat.Visible = false;
                ado.ListMedicineTypeColumn.Add(colTenHoatChat);

                ListMedicineTypeColumn colHamLuong = new ListMedicineTypeColumn("Hàm lượng", "CONCENTRA", 100, false);
                colHamLuong.VisibleIndex = 8;
                colHamLuong.Visible = false;
                ado.ListMedicineTypeColumn.Add(colHamLuong);

                ListMedicineTypeColumn colTran = new ListMedicineTypeColumn("Số lượng sàn", "ALERT_MIN_IN_STOCK_STR", 100, true);
                colTran.VisibleIndex = 9;

                ado.ListMedicineTypeColumn.Add(colTran);

                ListMedicineTypeColumn colSan = new ListMedicineTypeColumn("Cơ số", "ALERT_MAX_IN_STOCK", 100, true);
                colSan.VisibleIndex = 10;
                colSan.Visible = false;
                colSan.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeColumn.Add(colSan);

                ListMedicineTypeColumn ColKhoXuat = new ListMedicineTypeColumn("Kho xuất", "EXP_MEDI_STOCK_ID", 100, true);
                ColKhoXuat.VisibleIndex = 11;
                ColKhoXuat.Visible = false;
                ColKhoXuat.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeColumn.Add(ColKhoXuat);

               /* ListMedicineTypeColumn colTrust = new ListMedicineTypeColumn("Cơ số thực tế", "TRUST_AMOUNT_IN_STOCK_STR", 100, true);
                colTrust.ToolTip = "Là cơ số thực tế do bị thay đổi trong trường hợp có các phiếu xuất khác ngoài phiếu xuất cho bệnh nhân (vd: xuất chuyển kho, xuất hao phí, ...) hoặc có các phiếu nhập khác ngoài nhập bù cơ số (vd: nhập đầu kì, nhập thu hồi, ...)";
                colTrust.VisibleIndex = 12;
                colTrust.Visible = false;
                colTrust.AllowEdit = false;
                colTrust.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeColumn.Add(colTrust);*/

                ListMedicineTypeColumn colQuaTran = new ListMedicineTypeColumn("Chặn nhập quá trần", "IS_PREVENT_MAX", 100, true);
                colQuaTran.VisibleIndex = 13;
                colQuaTran.Visible = false;
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeColumn.Add(colQuaTran);

                ListMedicineTypeColumn colKoXuat = new ListMedicineTypeColumn("Không cho xuất", "IS_PREVENT_EXP", 100, true);
                colKoXuat.VisibleIndex = 14;
                colKoXuat.Visible = false;
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeColumn.Add(colKoXuat);

                ListMedicineTypeColumn colGioiHan = new ListMedicineTypeColumn("Thuốc giới hạn", "IsGoodsRetrict", 100, true);
                colGioiHan.VisibleIndex = 15;
                colGioiHan.Visible = false;
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colGioiHan.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMedicineTypeColumn.Add(colGioiHan);

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
        private void FillDataToMety(UCMediStockMetyList _Mety)
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

                //if (cboRoomType.EditValue != null)
                //    RoomFillter.ROOM_TYPE_ID = (long)cboRoomType.EditValue;
                //long isChoseStock = 0;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseStock = (long)cboChoose.EditValue;
                }
                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseStock = (long)cboChoose.EditValue;
                }
                if (chkDoNotDisplayLock.Checked == true)
                {
                    metyFilter.IS_ACTIVE = 1;
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>>(
                    "api/HisMedicineType/GetView",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    metyFilter,
                    param);

                MetyAdo = new List<HIS.UC.ListMedicineType.ListMedicineTypeADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listMety = rs.Data;
                    foreach (var item in listMety)
                    {
                        HIS.UC.ListMedicineType.ListMedicineTypeADO metypeADO = new HIS.UC.ListMedicineType.ListMedicineTypeADO(item);
                        if (isChoseStock == 2)
                        {
                            metypeADO.isKeyChoose = true;
                            //btnCheckAll2.Enabled = false;
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
                            mediStockMety.ALERT_MAX_IN_STOCK = item.ALERT_MAX_IN_STOCK;
                            mediStockMety.ALERT_MIN_IN_STOCK_STR = item.ALERT_MIN_IN_STOCK;
                            mediStockMety.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
                            mediStockMety.IS_PREVENT_EXP = item.IS_PREVENT_EXP == 1 ? true : false;
                            mediStockMety.IS_PREVENT_MAX = item.IS_PREVENT_MAX == 1 ? true : false;
                            mediStockMety.IsGoodsRetrict = item.IS_GOODS_RESTRICT == 1 ? true : false;
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
        private void FillDataToGrid2(UCMediStockMetyList _roomtype)
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
                ucPaging2.Init(FillDataToGridMety, param, numPageSize);
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
        private void InitMediStock()
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

                //MediStockColumn colMaKhoa = new MediStockColumn("Mã khoa", "DEPARTMENT_CODE", 80, false);
                //colMaKhoa.VisibleIndex = 4;
                //ado.ListMediStockColumn.Add(colMaKhoa);

                //MediStockColumn colTenKhoa = new MediStockColumn("Tên khoa", "DEPARTMENT_NAME", 150, false);
                //colTenKhoa.VisibleIndex = 5;
                //ado.ListMediStockColumn.Add(colTenKhoa);


                this.ucStock = (UserControl)stockProcessor.Run(ado);
                if (ucStock != null)
                {
                    this.panelControl1.Controls.Add(this.ucStock);
                    this.ucStock.Dock = DockStyle.Fill;
                }
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
                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseMety = (long)cboChoose.EditValue;
                }
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseMety = (long)cboChoose.EditValue;
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
                        if (isChoseMety == 1)
                        {
                            medistockADO.isKeyChooseMest = true;
                            //btnCheckAll1.Enabled = false;
                        }
                        StockAdo.Add(medistockADO);
                    }
                }
                if (listMediStockMety != null && listMediStockMety.Count > 0)
                {

                    foreach (var itemStock in listMediStockMety)
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
        private void FillDataToGrid1(UCMediStockMetyList _Stock)
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
        private void btn_Radio_Enable_Click1(V_HIS_MEDI_STOCK data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMediStockMetyFilter filter = new HisMediStockMetyFilter();
                filter.MEDI_STOCK_ID = data.ID;
                checkStock = data.ID;
                listMediStockMety = new List<HIS_MEDI_STOCK_METY>();
                listMediStockMety = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK_METY>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEDI_STOCK_METY_GET,
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
                            mediStockMety.ALERT_MAX_IN_STOCK = item.ALERT_MAX_IN_STOCK;
                            mediStockMety.ALERT_MIN_IN_STOCK_STR = item.ALERT_MIN_IN_STOCK;
                            mediStockMety.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
                            mediStockMety.IS_PREVENT_EXP = item.IS_PREVENT_EXP == 1 ? true : false;
                            mediStockMety.IS_PREVENT_MAX = item.IS_PREVENT_MAX == 1 ? true : false;
                            mediStockMety.IsGoodsRetrict = item.IS_GOODS_RESTRICT == 1 ? true : false;
                            mediStockMety.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
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
                // load data to combobox Medi stock
                CommonParam commonpram = new CommonParam();
                MOS.Filter.HisMestRoomFilter mestRoomFilter = new HisMestRoomFilter();
                mestRoomFilter.ROOM_ID = data.ROOM_ID;
                var listMestRoom = new BackendAdapter(param).Get<List<V_HIS_MEST_ROOM>>(
                                 "api/HisMestRoom/GetView",
                                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                  mestRoomFilter,
                                  commonpram);

                metyProcessor.LoaCboMediStock(ucMety, listMestRoom, false);
                ///
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
                MOS.Filter.HisMediStockMetyFilter filter = new HisMediStockMetyFilter();
                filter.MEDICINE_TYPE_ID = data.ID;

                checkMety = data.ID;
                //filter.
                listMediStockMety = new List<HIS_MEDI_STOCK_METY>();
                listMediStockMety = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK_METY>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_MEDI_STOCK_METY_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.MediStock.MediStockADO> dataNew = new List<HIS.UC.MediStock.MediStockADO>();
                dataNew = (from r in listStock select new HIS.UC.MediStock.MediStockADO(r)).ToList();
                if (listMediStockMety != null && listMediStockMety.Count > 0)
                {

                    foreach (var itemStock in listMediStockMety)
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
        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Kho"));
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
                listMediStockMety = new List<HIS_MEDI_STOCK_METY>();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
                if((Inventec.Common.TypeConvert.Parse.ToInt64(cboChoose.EditValue.ToString() ?? ""))==1)
                {
                    metyProcessor.LoaCboMediStock(ucMety, null, false);
                }
                else
                    metyProcessor.LoaCboMediStock(ucMety, null, true);
               
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MediStockMetyList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBranchTime.UCMediStockMetyList).Assembly);

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
                this.chkDoNotDisplayLock.Text = Inventec.Common.Resource.Get.Value("UCMediStockMetyList.chkDoNotDisplayLock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                if (data != null && listMediStockMety != null && listMediStockMety.Count > 0)
                {
                    HIS_MEDI_STOCK_METY mediStockMety = listMediStockMety.FirstOrDefault(o => o.MEDI_STOCK_ID == data.ID);
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
                                    var result = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_METY>>("api/HisMediStockMety/CopyByMety", ApiConsumer.ApiConsumers.MosConsumer, hisMestMetyCopyByMetySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        listMediStockMety = result;
                                        List<HIS.UC.MediStock.MediStockADO> dataNew = new List<HIS.UC.MediStock.MediStockADO>();
                                        dataNew = (from r in listStock select new HIS.UC.MediStock.MediStockADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {

                                            foreach (var itemStock in result)
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

                        case HIS.UC.ListMedicineType.Popup.PopupMenuProcessor.ItemType.UpdateTrustAmount:
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
                if (isChoseMety != 1)
                {
                    MessageManager.Show("Vui lòng chọn kho!");
                    return;
                }
                List<HIS.UC.ListMedicineType.ListMedicineTypeADO> Ados = metyProcessor.GetDataGridView(ucMety) as List<HIS.UC.ListMedicineType.ListMedicineTypeADO>;
                if (Ados == null || Ados.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong load dc danh sach thuoc");
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
                        MessageBox.Show("Chưa chọn thuốc", "Thông báo");
                        return;
                    }
                    sdo.TypeIds = listAdoCheck.Select(o => o.ID).ToList();
                }
                WaitingManager.Show();

                List<HIS_MEDI_STOCK_METY> mediStockMety = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_METY>>("api/HisMediStockMety/SetRealBaseAmount", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                //CloseTreatmentProcessor.TreatmentUnFinish(, param);
                WaitingManager.Hide();
                if (mediStockMety != null && mediStockMety.Count > 0)
                {
                    success = true;
                    foreach (var item in Ados)
                    {
                        HIS_MEDI_STOCK_METY updatedAmountBase = mediStockMety.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.ID && o.MEDI_STOCK_ID == listMstAdoCheck.First().ID);
                        if (updatedAmountBase != null)
                        {
                            item.check1 = true;
                            if (!this.listMediStockMety.Exists(o => o.ID == updatedAmountBase.ID))
                            {
                                this.listMediStockMety.Add(updatedAmountBase);
                            }
                        }
                    }
                    metyProcessor.Reload(ucMety, Ados);
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

        private void MedistockGridView_MouseRightClick(object sender, ItemClickEventArgs e)
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
                                    HisMestMetyCopyByMediStockSDO hisMestMetyCopyByMediStockSDO = new HisMestMetyCopyByMediStockSDO();
                                    hisMestMetyCopyByMediStockSDO.CopyMediStockId = currentCopyMedistockAdo.ID;
                                    hisMestMetyCopyByMediStockSDO.PasteMediStockId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_METY>>("api/HisMediStockMety/CopyByMediStock", ApiConsumer.ApiConsumers.MosConsumer, hisMestMetyCopyByMediStockSDO, param);
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
                                                    mediStockMety.ALERT_MAX_IN_STOCK = item.ALERT_MAX_IN_STOCK;
                                                    mediStockMety.ALERT_MIN_IN_STOCK_STR = item.ALERT_MIN_IN_STOCK;
                                                    mediStockMety.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
                                                    mediStockMety.IS_PREVENT_EXP = item.IS_PREVENT_EXP == 1 ? true : false;
                                                    mediStockMety.IS_PREVENT_MAX = item.IS_PREVENT_MAX == 1 ? true : false;
                                                    mediStockMety.IsGoodsRetrict = item.IS_GOODS_RESTRICT == 1 ? true : false;
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
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_PATY)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_MEDICINE_TYPE)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_ROOM)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_MEDICINE_PATY)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(HIS_MEDI_STOCK_METY)).ToString(), false);
            MessageManager.Show("Xử lý thành công");
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
        {

            BackendDataWorker.Reset<HIS_MEDI_STOCK_METY>();
            BackendDataWorker.Reset<V_HIS_SERVICE>();
            BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
            BackendDataWorker.Reset<V_HIS_MEDICINE_TYPE>();
            BackendDataWorker.Reset<V_HIS_SERVICE_ROOM>();
            BackendDataWorker.Reset<V_HIS_MEDICINE_PATY>();
            BackendDataWorker.Reset<MedicineMaterialTypeComboADO>();
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
                if (this.moduleData != null)
                {
                    CallModule callModule = new CallModule(CallModule.HisImportServiceRoom, moduleData.RoomId, moduleData.RoomTypeId, listArgs);
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
