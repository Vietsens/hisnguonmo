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
using System.Windows.Forms;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.Plugins.AllocateLiquExpMestCreate.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.UC.HisMedicineInStock;
using HIS.UC.HisMedicineInStock.ADO;
using System.Linq;
using HIS.Desktop.LibraryMessage;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.UC.HisMaterialInStockSDO;
using HIS.UC.HisMaterialInStockSDO.ADO;
using HIS.Desktop.Plugins.AllocateLiquExpMestCreate.Resources;

namespace HIS.Desktop.Plugins.AllocateLiquExpMestCreate
{
    public partial class UCAllocateLiquExpMestCreate : UserControl
    {
        #region declare
        MedicineInStockProcessor mediInStockProcessor = null;
        MaterialInStockProcessor mateInStockProcessor = null;
        ExpMestMedicineProcessor expMestMediProcessor = null;
        ExpMestMaterialProcessor expMestMateProcessor = null;

        UserControl ucMediInStock = null;
        UserControl ucMateInStock = null;
        UserControl ucExpMestMedicineRequest = null;
        UserControl ucExpMestMaterialRequest = null;

        List<MOS.SDO.HisMedicineInStockSDO> medicineInStockSDOs;
        List<MOS.SDO.HisMaterialInStockSDO> materialInStockSDOs;

        MOS.SDO.HisMaterialInStockSDO hisMaterialInStockSDO = null;
        MOS.SDO.HisMedicineInStockSDO hisMedicineInStockSDO = null;

        List<MediMateInStockADO> mediMateInStockADOProcess;// danh sach thuoc/vat tu xu ly

        MediMateInStockADO currentMediMate = null;
        HisLiquExpMestResultSDO resultSdo = null;

        bool isUpdate = false;
        int ActionType = 0;

        V_HIS_MEDI_STOCK mediStock = null;
        long roomId;
        long roomTypeId;
        List<V_HIS_MEDI_STOCK> listExpMediStock;
        int positionHandleControl = -1;
        #endregion

        #region contructor
        public UCAllocateLiquExpMestCreate(long roomtypeid, long roomid)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.roomTypeId = roomtypeid;
                this.roomId = roomid;
                LoadDataToCboMedistock();
                InitExpMestMateGrid();
                InitExpMestMediGrid();
                InitMedicineGrid();
                InitMaterialGrid();
                mediMateInStockADOProcess = new List<MediMateInStockADO>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestSaleCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                listExpMediStock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().ToList();
                SetCaptionByLanguageKey();
                ValidControl();
                ResetAllControl();
                this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionView;
                EnableControl(this.ActionType);
                SetDefaultMedistock();
                txtExpMedistock.Focus();
                txtExpMedistock.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region private function

        /// <summary>
        /// Set mặc định kho
        /// </summary>
        private void SetDefaultMedistock()
        {
            try
            {
                if (listExpMediStock == null || listExpMediStock.Count <= 0)
                {
                    return;
                }
                var medistock = listExpMediStock.FirstOrDefault(o => o.ROOM_ID == this.roomId);
                if (medistock != null)
                {
                    cboMedistock.EditValue = medistock.ID;
                    txtExpMedistock.Text = medistock.MEDI_STOCK_NAME;
                    this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    EnableControl(this.ActionType);
                }
                else
                {
                    cboMedistock.EditValue = null;
                    txtExpMedistock.Text = "";
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Load dữ liệu vào commbo medistock
        /// </summary>
        private void LoadDataToCboMedistock()
        {
            try
            {
                cboMedistock.Properties.DataSource = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                cboMedistock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMedistock.Properties.ValueMember = "ID";
                cboMedistock.Properties.ForceInitialize();
                cboMedistock.Properties.Columns.Clear();
                cboMedistock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 50));
                cboMedistock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 120));
                cboMedistock.Properties.ShowHeader = false;
                cboMedistock.Properties.ImmediatePopup = true;
                cboMedistock.Properties.DropDownRows = 10;
                cboMedistock.Properties.PopupWidth = 170;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// load thuốc trong kho
        /// </summary>
        /// <param name="txtKeyword"></param>
        private void LoadMedicineInStock(string txtKeyword)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineStockViewFilter hisMedicineStockViewFilter = new HisMedicineStockViewFilter();
                hisMedicineStockViewFilter.MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMedistock.EditValue ?? "").ToString());
                hisMedicineStockViewFilter.IS_LEAF = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_LEAF__TRUE;
                hisMedicineStockViewFilter.KEY_WORD = txtKeyword;
                medicineInStockSDOs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.SDO.HisMedicineInStockSDO>>(HIS.Desktop.Plugins.AllocateLiquExpMestCreate.HisRequestUriStore.MOSHIS_MEDICINE_GET_IN_STOCK_MEDICINE, ApiConsumer.ApiConsumers.MosConsumer, hisMedicineStockViewFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// load vật tư trong kho
        /// </summary>
        /// <param name="txtKeyword"></param>
        private void LoadMaterialInStock(string txtKeyword)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMaterialStockViewFilter hisMaterialStockViewFilter = new HisMaterialStockViewFilter();
                hisMaterialStockViewFilter.MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMedistock.EditValue ?? "").ToString());
                hisMaterialStockViewFilter.IS_LEAF = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_LEAF__TRUE;
                hisMaterialStockViewFilter.KEY_WORD = txtKeyword;
                materialInStockSDOs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.SDO.HisMaterialInStockSDO>>(HIS.Desktop.Plugins.AllocateLiquExpMestCreate.HisRequestUriStore.MOSHIS_MATERIAL_GET_IN_STOCK_MATERIAL, ApiConsumer.ApiConsumers.MosConsumer, hisMaterialStockViewFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusKeyWord()
        {
            try
            {
                if (xtraTabControlMain.SelectedTabPageIndex == 0)//thuoc
                {
                    this.mediInStockProcessor.FocusKeyword(this.ucMediInStock);
                }
                else
                {
                    this.mateInStockProcessor.FocusKeyword(this.ucMateInStock);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Khởi tạo 
        /// </summary>
        private void InitMedicineGrid()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCAllocateExpeExpMestCreate;
                this.mediInStockProcessor = new MedicineInStockProcessor();
                HisMedicineInStockInitADO ado = new HisMedicineInStockInitADO();
                ado.hisMedicineInStockSDOs = this.medicineInStockSDOs;
                ado.MedicineInStockColumns = new List<MedicineInStockColumn>();
                ado.gridView_Click = gridViewMedicine_Click;
                ado.txtKeyword_KeyDown = txtkeyWordMedicine_keyDown;
                ado.gridView_KeyDown = gridViewMedicine_KeyDown;
                ado.MedicineInStockGrid_CustomUnboundColumnData = MedicineInStockGrid_CustomUnboundColumnData;
                //Mã thuốc
                MedicineInStockColumn colMaThuoc = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", langManager, culture), "MEDICINE_TYPE_CODE", 60, false, true);
                colMaThuoc.VisibleIndex = 0;
                ado.MedicineInStockColumns.Add(colMaThuoc);

                //Tên thuốc
                MedicineInStockColumn colTenThuoc = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "MEDICINE_TYPE_NAME", 100, false, true);
                colTenThuoc.VisibleIndex = 1;
                ado.MedicineInStockColumns.Add(colTenThuoc);

                //Khả dụng
                MedicineInStockColumn colKhaDung = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MEDICINE_IN_STOCK__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 100, false, true);
                colKhaDung.Format = new DevExpress.Utils.FormatInfo();
                colKhaDung.Format.FormatString = "#,##0.00";
                colKhaDung.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colKhaDung.VisibleIndex = 2;
                ado.MedicineInStockColumns.Add(colKhaDung);

                //Tồn kho
                MedicineInStockColumn colTonKho = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT", langManager, culture), "TotalAmount", 100, false, true);
                colTonKho.VisibleIndex = 3;
                colTonKho.Format = new DevExpress.Utils.FormatInfo();
                colTonKho.Format.FormatString = "#,##0.00";
                colTonKho.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MedicineInStockColumns.Add(colTonKho);

                //Hạn sử dụng
                MedicineInStockColumn colHanSuDung = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MEDICINE_IN_STOCK__COLUMN_EXPIRED_DATE", langManager, culture), "EXPIRED_DATE_STR", 100, false, true);
                colHanSuDung.VisibleIndex = 4;
                colHanSuDung.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.MedicineInStockColumns.Add(colHanSuDung);

                //Đơn vị tính
                MedicineInStockColumn colLoaiPhong = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MEDICINE_IN_STOCK__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 100, false, true);
                colLoaiPhong.VisibleIndex = 5;
                ado.MedicineInStockColumns.Add(colLoaiPhong);

                //Nhà cung cấp
                MedicineInStockColumn colNCC = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MEDICINE_IN_STOCK__COLUMN_SUPPLIER_NAME", langManager, culture), "SUPPLIER_NAME", 100, false, true);
                colNCC.VisibleIndex = 6;
                ado.MedicineInStockColumns.Add(colNCC);

                //Gói thầu
                MedicineInStockColumn colGoiThau = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MEDICINE_IN_STOCK__COLUMN_PACKAGE_NUMBER", langManager, culture), "PACKAGE_NUMBER", 100, false, true);
                colGoiThau.VisibleIndex = 7;
                ado.MedicineInStockColumns.Add(colGoiThau);

                //Số lô
                MedicineInStockColumn colSolo = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MEDICINE_IN_STOCK__COLUMN_BID_NUMBER", langManager, culture), "BID_NUMBER", 100, false, true);
                colSolo.VisibleIndex = 8;
                ado.MedicineInStockColumns.Add(colSolo);


                this.ucMediInStock = (UserControl)this.mediInStockProcessor.Run(ado);
                if (this.ucMediInStock != null)
                {
                    this.xtraTabPageMedicine.Controls.Add(this.ucMediInStock);
                    this.ucMediInStock.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineInStockGrid_CustomUnboundColumnData(HisMedicineInStockSDO data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "EXPIRED_DATE_STR")
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)(data.EXPIRED_DATE ?? 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_KeyDown(object sender, KeyEventArgs e, HisMedicineInStockSDO data)
        {
            try
            {
                this.hisMedicineInStockSDO = data;
                if (data == null)
                {
                    currentMediMate = null;
                    return;
                }
                currentMediMate = new MediMateInStockADO(data);
                if (e.KeyCode == Keys.Enter)
                {
                    FocusSpinAmountControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtkeyWordMedicine_keyDown(string keyWordText)
        {
            try
            {
                LoadMedicineInStock(keyWordText);
                mediInStockProcessor.Reload(this.ucMediInStock, this.medicineInStockSDOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusSpinAmountControl()
        {
            try
            {
                spinAmount.Value = 0;
                spinAmount.Focus();
                spinAmount.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        /// khởi tạo grid expMest medicine
        /// </summary>
        private void InitExpMestMediGrid()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCAllocateExpeExpMestCreate;

                this.expMestMediProcessor = new ExpMestMedicineProcessor();
                ExpMestMedicineInitADO ado = new ExpMestMedicineInitADO();
                ado.ExpMestMedicineGrid_CustomUnboundColumnData = expMestMediGrid__CustomUnboundColumnData;
                ado.IsShowSearchPanel = false;
                ado.ListExpMestMedicineColumn = new List<ExpMestMedicineColumn>();

                ExpMestMedicineColumn colMedicineTypeName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "MEDICINE_TYPE_NAME", 170, false);
                colMedicineTypeName.VisibleIndex = 0;
                ado.ListExpMestMedicineColumn.Add(colMedicineTypeName);

                ExpMestMedicineColumn colServiceUnitName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 40, false);
                colServiceUnitName.VisibleIndex = 1;
                ado.ListExpMestMedicineColumn.Add(colServiceUnitName);

                ExpMestMedicineColumn colAmount = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_AMOUNT", langManager, culture), "AMOUNT", 100, false);
                colAmount.VisibleIndex = 2;
                colAmount.Format = new DevExpress.Utils.FormatInfo();
                colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAmount.Format.FormatString = "#,##0.00";
                ado.ListExpMestMedicineColumn.Add(colAmount);

                ExpMestMedicineColumn colExpiredDate = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_EXPIRED_DATE", langManager, culture), "EXPIRED_DATE_STR", 90, false);
                colExpiredDate.VisibleIndex = 3;
                colExpiredDate.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExpMestMedicineColumn.Add(colExpiredDate);

                ExpMestMedicineColumn colPackageNumber = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_PACKAGE_NUMBER", langManager, culture), "PACKAGE_NUMBER", 60, false);
                colPackageNumber.VisibleIndex = 4;
                ado.ListExpMestMedicineColumn.Add(colPackageNumber);

                ExpMestMedicineColumn colNationalName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_NATIONAL_NAME", langManager, culture), "NATIONAL_NAME", 100, false);
                colNationalName.VisibleIndex = 5;
                ado.ListExpMestMedicineColumn.Add(colNationalName);

                ExpMestMedicineColumn colManufacturerName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_MANUFACTURER_NAME", langManager, culture), "MANUFACTURER_NAME", 100, false);
                colManufacturerName.VisibleIndex = 6;
                ado.ListExpMestMedicineColumn.Add(colManufacturerName);

                this.ucExpMestMedicineRequest = (UserControl)this.expMestMediProcessor.Run(ado);
                //this.ucExpMestMedicineExecute = (UserControl)this.expMestMediProcessor.Run(ado);
                if (this.ucExpMestMedicineRequest != null)
                {
                    this.xtraTabPageRequestMedicineResult.Controls.Add(this.ucExpMestMedicineRequest);
                    this.ucExpMestMedicineRequest.Dock = DockStyle.Fill;
                }
                //if (this.ucExpMestMedicineExecute != null)
                //{
                //    this.xtraTabPageExportMedicineResult.Controls.Add(this.ucExpMestMedicineExecute);
                //    this.ucExpMestMedicineExecute.Dock = DockStyle.Fill;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// khởi tạo grid material 
        /// </summary>
        private void InitMaterialGrid()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCAllocateExpeExpMestCreate;

                this.mateInStockProcessor = new MaterialInStockProcessor();
                HisMaterialInStockInitADO ado = new HisMaterialInStockInitADO();
                ado.hisMaterialInStockSDOs = this.materialInStockSDOs;
                ado.gridView_Click = gridviewMaterial_click;
                ado.txtKeyword_KeyDown = txtKeyword_KeyDown;
                ado.gridView_KeyDown = gridViewMaterial_KeyDown;
                ado.MaterialInStockGrid_CustomUnboundColumnData = materialInStockGrid_CustomUnboundColumnData;
                ado.MaterialInStockColumns = new List<MaterialInStockColumn>();

                //Mã vật tư
                MaterialInStockColumn colMaVatTu = new MaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MATERIAL_IN_STOCK__COLUMN_MATERIAL_TYPE_CODE", langManager, culture), "MATERIAL_TYPE_CODE", 60, false, true);
                colMaVatTu.VisibleIndex = 0;
                ado.MaterialInStockColumns.Add(colMaVatTu);

                //Tên vật tư
                MaterialInStockColumn colTenVatTu = new MaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MATERIAL_IN_STOCK__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MATERIAL_TYPE_NAME", 100, false, true);
                colTenVatTu.VisibleIndex = 1;
                ado.MaterialInStockColumns.Add(colTenVatTu);

                //Khả dụng
                MaterialInStockColumn colKhaDung = new MaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MATERIAL_IN_STOCK__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 100, false, true);
                colKhaDung.Format = new DevExpress.Utils.FormatInfo();
                colKhaDung.Format.FormatString = "#,##0.00";
                colKhaDung.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colKhaDung.VisibleIndex = 2;
                ado.MaterialInStockColumns.Add(colKhaDung);

                //Tồn kho
                MaterialInStockColumn colTonKho = new MaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MATERIAL_IN_STOCK__COLUMN_TOTAL_AMOUNT", langManager, culture), "TotalAmount", 100, false, true);
                colTonKho.Format = new DevExpress.Utils.FormatInfo();
                colTonKho.Format.FormatString = "#,##0.00";
                colTonKho.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colTonKho.VisibleIndex = 3;
                ado.MaterialInStockColumns.Add(colTonKho);

                //Đơn vị tính
                MaterialInStockColumn colDvt = new MaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MATERIAL_IN_STOCK__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 100, false, true);
                colDvt.VisibleIndex = 4;
                ado.MaterialInStockColumns.Add(colDvt);

                //Nhà cung cấp
                MaterialInStockColumn colNCC = new MaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MATERIAL_IN_STOCK__COLUMN_SUPPLIER_NAME", langManager, culture), "SUPPLIER_NAME", 100, false, true);
                colNCC.VisibleIndex = 5;
                ado.MaterialInStockColumns.Add(colNCC);

                //Gói thầu
                MaterialInStockColumn colGoiThau = new MaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MATERIAL_IN_STOCK__COLUMN_BID_NUMBER", langManager, culture), "BID_NUMBER", 100, false, true);
                colGoiThau.VisibleIndex = 6;
                ado.MaterialInStockColumns.Add(colGoiThau);

                MaterialInStockColumn colSoLo = new MaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_MATERIAL_IN_STOCK__COLUMN_PACKAGE_NUMBER", langManager, culture), "PACKAGE_NUMBER", 100, false, true);
                colSoLo.VisibleIndex = 7;
                ado.MaterialInStockColumns.Add(colSoLo);

                this.ucMateInStock = (UserControl)this.mateInStockProcessor.Run(ado);
                if (this.ucMateInStock != null)
                {
                    this.xtraTabPageMaterial.Controls.Add(this.ucMateInStock);
                    this.ucMateInStock.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialInStockGrid_CustomUnboundColumnData(HisMaterialInStockSDO data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void gridViewMaterial_KeyDown(object sender, KeyEventArgs e, HisMaterialInStockSDO data)
        {
            try
            {
                this.hisMaterialInStockSDO = data;
                if (data == null)
                {
                    currentMediMate = null;
                    return;
                }
                currentMediMate = new MediMateInStockADO(data);
                if (e.KeyCode == Keys.Enter)
                {
                    FocusSpinAmountControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_KeyDown(string txtKeywordValue)
        {
            try
            {
                LoadMaterialInStock(txtKeywordValue);
                mateInStockProcessor.Reload(this.ucMateInStock, this.materialInStockSDOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// khởi tạo grid expMest material
        /// </summary>
        private void InitExpMestMateGrid()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCAllocateExpeExpMestCreate;

                this.expMestMateProcessor = new ExpMestMaterialProcessor();
                ExpMestMaterialInitADO ado = new ExpMestMaterialInitADO();
                ado.ExpMestMaterialGrid_CustomUnboundColumnData = expMestMateGrid__CustomUnboundColumnData;
                ado.IsShowSearchPanel = false;
                ado.ListExpMestMaterialColumn = new List<ExpMestMaterialColumn>();

                ExpMestMaterialColumn colMaterialTypeName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MATERIAL_TYPE_NAME", 170, false);
                colMaterialTypeName.VisibleIndex = 0;
                ado.ListExpMestMaterialColumn.Add(colMaterialTypeName);

                ExpMestMaterialColumn colServiceUnitName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 40, false);
                colServiceUnitName.VisibleIndex = 1;
                ado.ListExpMestMaterialColumn.Add(colServiceUnitName);

                ExpMestMaterialColumn colAmount = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_AMOUNT", langManager, culture), "AMOUNT", 100, false);
                colAmount.VisibleIndex = 2;
                colAmount.Format = new DevExpress.Utils.FormatInfo();
                colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAmount.Format.FormatString = "#,##0.00";
                ado.ListExpMestMaterialColumn.Add(colAmount);

                ExpMestMaterialColumn colExpiredDate = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_EXPIRED_DATE", langManager, culture), "EXPIRED_DATE_STR", 90, false);
                colExpiredDate.VisibleIndex = 3;
                colExpiredDate.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExpMestMaterialColumn.Add(colExpiredDate);

                ExpMestMaterialColumn colPackageNumber = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_PACKAGE_NUMBER", langManager, culture), "PACKAGE_NUMBER", 60, false);
                colPackageNumber.VisibleIndex = 4;
                ado.ListExpMestMaterialColumn.Add(colPackageNumber);

                ExpMestMaterialColumn colNationalName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_NATIONAL_NAME", langManager, culture), "NATIONAL_NAME", 100, false);
                colNationalName.VisibleIndex = 5;
                ado.ListExpMestMaterialColumn.Add(colNationalName);

                ExpMestMaterialColumn colManufacturerName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ALLOCATE_EXPE_EXP_MEST_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_MANUFACTURER_NAME", langManager, culture), "MANUFACTURER_NAME", 100, false);
                colManufacturerName.VisibleIndex = 6;
                ado.ListExpMestMaterialColumn.Add(colManufacturerName);

                this.ucExpMestMaterialRequest = (UserControl)this.expMestMateProcessor.Run(ado);
                if (this.ucExpMestMaterialRequest != null)
                {
                    this.xtraTabPageRequestMaterialResult.Controls.Add(this.ucExpMestMaterialRequest);
                    this.ucExpMestMaterialRequest.Dock = DockStyle.Fill;
                }
                //this.ucExpMestMaterialExecute = (UserControl)this.expMestMateProcessor.Run(ado);
                //if (this.ucExpMestMaterialExecute != null)
                //{
                //    this.xtraTabPageExportMaterialResult.Controls.Add(this.ucExpMestMaterialExecute);
                //    this.ucExpMestMaterialExecute.Dock = DockStyle.Fill;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        /// enable/disable control
        /// </summary>
        /// <param name="actionType"></param>
        private void EnableControl(int actionType)
        {
            try
            {
                btnAdd.Enabled = (actionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd || actionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                btnSave.Enabled = (actionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd || actionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                ddBtnPrint.Enabled = (actionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionView);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataGridExpMestDetail()
        {
            try
            {
                gridControlExpMestDetail.BeginUpdate();
                //gridControlExpMestDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpMestDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// reset control
        /// </summary>
        private void ResetValueControlDetail()
        {
            try
            {
                this.currentMediMate = null;
                spinAmount.Value = 0;
                btnAdd.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetLeftBottomControl()
        {
            try
            {
                spinAmount.Value = 0;
                spinVat.Value = 0;
                spinVat.Text = "";
                spinExpPrice.Value = 0;
                spinExpPrice.Text = "";
                this.currentMediMate = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetLeftControl()
        {
            try
            {
                txtExpMedistock.Text = "";
                cboMedistock.EditValue = null;
                txtDescription.Text = "";
                cboMedistock.EditValue = null;
                medicineInStockSDOs = null;
                materialInStockSDOs = null;
                this.mediInStockProcessor.Reload(this.ucMediInStock, medicineInStockSDOs);
                this.mateInStockProcessor.Reload(this.ucMateInStock, materialInStockSDOs);
                ResetLeftBottomControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetRightControl()
        {
            try
            {
                resultSdo = null;
                mediMateInStockADOProcess = null;
                gridControlExpMestDetail.BeginUpdate();
                gridControlExpMestDetail.DataSource = mediMateInStockADOProcess;
                gridControlExpMestDetail.EndUpdate();
                this.expMestMateProcessor.Reload(this.ucExpMestMaterialRequest, null);
                this.expMestMediProcessor.Reload(this.ucExpMestMedicineRequest, null);
                //this.expMestMateProcessor.Reload(this.ucExpMestMaterialExecute, null);
                //this.expMestMediProcessor.Reload(this.ucExpMestMedicineExecute, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetAllControl()
        {
            try
            {
                ResetLeftControl();
                ResetRightControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///  kiểm tra tồn tại medicine/ material
        /// </summary>
        /// <param name="mediMaty"></param>
        /// <returns></returns>
        bool CheckExistMediMaty(object mediMaty)
        {
            try
            {
                bool result = false;
                if (this.mediMateInStockADOProcess == null)
                    return false;

                if (mediMaty is MOS.SDO.HisMedicineInStockSDO)
                {
                    var medicineInStock = (HisMedicineInStockSDO)mediMaty;
                    var checkMedicine = this.mediMateInStockADOProcess.FirstOrDefault(o => o.MEDI_MATE_TYPE_ID == medicineInStock.MEDICINE_TYPE_ID);
                    if (checkMedicine != null)
                    {
                        result = true;
                    }
                }
                if (mediMaty is MOS.SDO.HisMaterialInStockSDO)
                {
                    var materialInStock = (MOS.SDO.HisMaterialInStockSDO)mediMaty;
                    var checkMaterial = this.mediMateInStockADOProcess.FirstOrDefault(o => o.MEDI_MATE_TYPE_ID == materialInStock.MATERIAL_TYPE_ID);
                    if (checkMaterial != null)
                    {
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadDataGridControlProcess()
        {
            try
            {
                WaitingManager.Show();
                LoadMedicineInStock("");
                LoadMaterialInStock("");
                mediInStockProcessor.Reload(this.ucMediInStock, this.medicineInStockSDOs);
                mateInStockProcessor.Reload(this.ucMateInStock, this.materialInStockSDOs);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToGridControlProcess()
        {
            try
            {
                gridControlExpMestDetail.BeginUpdate();
                gridControlExpMestDetail.DataSource = this.mediMateInStockADOProcess;
                gridControlExpMestDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcess()
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                positionHandleControl = -1;
                if (!dxValidationProviderRight.Validate() || this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionView || gridControlExpMestDetail.DataSource == null)
                    return;

                WaitingManager.Show();
                MOS.SDO.HisLiquExpMestSDO liquExpMestSdo = new HisLiquExpMestSDO();
                liquExpMestSdo.LiquExpMest = new HIS_LIQU_EXP_MEST();
                liquExpMestSdo.ExpMaterials = new List<HisMaterialAmountWithPriceSDO>();
                liquExpMestSdo.ExpMedicines = new List<HisMedicineAmountWithPriceSDO>();
                liquExpMestSdo.ExpMest = new HIS_EXP_MEST();
                liquExpMestSdo.ExpMest.REQ_ROOM_ID = roomId;

                List<MediMateInStockADO> mediMatyInGridcontrol = (List<MediMateInStockADO>)gridControlExpMestDetail.DataSource;
                foreach (var item in mediMatyInGridcontrol)
                {
                    if (item.IsMedicine)
                    {
                        HisMedicineAmountWithPriceSDO hisMedicineAmountSDO = new HisMedicineAmountWithPriceSDO();
                        hisMedicineAmountSDO.Amount = item.EXP_AMOUNT;
                        hisMedicineAmountSDO.MedicineId = item.MEDI_MATE_ID;
                        hisMedicineAmountSDO.Price = item.ExpPrice;
                        hisMedicineAmountSDO.VatRatio = item.ExpVatRatio / 100;
                        liquExpMestSdo.ExpMedicines.Add(hisMedicineAmountSDO);
                    }
                    else
                    {
                        HisMaterialAmountWithPriceSDO hisMaterialAmountSDO = new HisMaterialAmountWithPriceSDO();
                        hisMaterialAmountSDO.Amount = item.EXP_AMOUNT;
                        hisMaterialAmountSDO.MaterialId = item.MEDI_MATE_ID;
                        hisMaterialAmountSDO.Price = item.ExpPrice;
                        hisMaterialAmountSDO.VatRatio = item.ExpVatRatio / 100;
                        liquExpMestSdo.ExpMaterials.Add(hisMaterialAmountSDO);
                    }
                }
                liquExpMestSdo.ExpMest.DESCRIPTION = txtDescription.Text.Trim();
                if (cboMedistock.EditValue != null)
                {
                    liquExpMestSdo.ExpMest.MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMedistock.EditValue ?? "").ToString());
                }

                resultSdo = new BackendAdapter(param).Post<MOS.SDO.HisLiquExpMestResultSDO>(HIS.Desktop.Plugins.AllocateLiquExpMestCreate.HisRequestUriStore.MOSHIS_LIQU_EXP_MEST_CREATE, ApiConsumers.MosConsumer, liquExpMestSdo, param);
                if (resultSdo != null)
                {
                    success = true;
                    SetDataToGridControlResult(resultSdo.ExpMaterials, resultSdo.ExpMedicines);
                    this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionView;
                    EnableControl(this.ActionType);
                    //ResetLeftControl();
                    InitMenuToButtonPrint();
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToGridControlResult(List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines)
        {
            try
            {
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineRequests = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineExecutes = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialRequests = new List<V_HIS_EXP_MEST_MATERIAL>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialExecutes = new List<V_HIS_EXP_MEST_MATERIAL>();
                if (expMestMedicines != null && expMestMedicines.Count > 0)
                {
                    // sắp xếp theo thứ tự tắng dần của id
                    expMestMedicines = expMestMedicines.OrderBy(p => p.ID).ToList();
                    expMestMedicineRequests = expMestMedicines.Where(p => p.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                    expMestMedicineExecutes = expMestMedicines.Where(p => p.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                }
                if (expMestMaterials != null)
                {
                    // sắp xếp theo thứ tự tắng dàn của id
                    expMestMaterials = expMestMaterials.OrderBy(o => o.ID).ToList();
                    expMestMaterialRequests = expMestMaterials.Where(p => p.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                    expMestMaterialExecutes = expMestMaterials.Where(p => p.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                }

                this.expMestMediProcessor.Reload(this.ucExpMestMedicineRequest, expMestMedicineRequests);
                //this.expMestMediProcessor.Reload(this.ucExpMestMedicineExecute, expMestMedicineExecutes);
                this.expMestMateProcessor.Reload(this.ucExpMestMaterialRequest, expMestMaterialRequests);
                //this.expMestMateProcessor.Reload(this.ucExpMestMaterialExecute, expMestMaterialExecutes);

                //if (expMestMaterialExecutes != null && expMestMaterialExecutes.Count > 0)
                //{
                //    xtraTabControlExpMestResult.SelectedTabPageIndex = 3;
                //}
                //else if (expMestMedicineExecutes != null && expMestMedicineExecutes.Count > 0)
                //{
                //    xtraTabControlExpMestResult.SelectedTabPageIndex = 2;
                //}
                //else if (expMestMaterialRequests != null && expMestMaterialRequests.Count > 0)
                //{
                //    xtraTabControlExpMestResult.SelectedTabPageIndex = 1;
                //}
                //else
                //{
                //    xtraTabControlExpMestResult.SelectedTabPageIndex = 0;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region event handler
        public void gridViewMedicine_Click(MOS.SDO.HisMedicineInStockSDO data)
        {
            try
            {
                this.hisMedicineInStockSDO = data;
                currentMediMate = new MediMateInStockADO(data);
                FocusSpinAmountControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceGroup_MouseDownDelegate(object sender, MouseEventArgs e)
        {
        }

        private void btn_Radio_Service_Group_Enable_Click(HisMedicineInStockSDO data)
        {
        }

        private void gridviewMaterial_click(HisMaterialInStockSDO data)
        {
            try
            {
                this.hisMaterialInStockSDO = data;
                currentMediMate = new MediMateInStockADO(data);
                FocusSpinAmountControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Service_Group_Enable_Click(HisMaterialInStockSDO data)
        {
        }

        private void expMestMediGrid__CustomUnboundColumnData(V_HIS_EXP_MEST_MEDICINE data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "EXPIRED_DATE_STR")
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void expMestMateGrid__CustomUnboundColumnData(V_HIS_EXP_MEST_MATERIAL data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "EXPIRED_DATE_STR")
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedistock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    ReloadDataGridControlProcess();
                    this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    EnableControl(this.ActionType);
                    ResetRightControl();
                    ResetLeftBottomControl();
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!dxValidationProviderControlLeft.Validate() || this.currentMediMate == null)
                    return;
                if (spinAmount.Value > this.currentMediMate.AvailableAmount)
                {
                    WaitingManager.Hide();
                    MessageManager.Show(ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho);
                    {
                        FocusKeyWord();
                        return;
                    }
                }
                if (xtraTabControlMain.SelectedTabPageIndex == 0)//tab thuoc
                {
                    var medicineInstock = new MediMateInStockADO(this.hisMedicineInStockSDO);
                    if (CheckExistMediMaty(hisMedicineInStockSDO))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThuocVatTuDaCoTrongDanhSachXuatBanCoMuonThayTheThuoc, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            return;
                        }
                        else
                        {
                            this.mediMateInStockADOProcess.RemoveAll(o => o.MEDI_MATE_TYPE_CODE == medicineInstock.MEDI_MATE_TYPE_CODE && o.IsMedicine);
                        }
                    }
                    if (mediMateInStockADOProcess == null)
                    {
                        mediMateInStockADOProcess = new List<MediMateInStockADO>();
                    }
                    if (mediMateInStockADOProcess.Count == 0)
                    {
                        medicineInstock.ID_ROW = 0;
                    }
                    else
                    {
                        var idRowMax = mediMateInStockADOProcess.Max(o => o.ID_ROW);
                        medicineInstock.ID_ROW = idRowMax + 1;
                    }

                    medicineInstock.EXP_AMOUNT = spinAmount.Value;
                    if (string.IsNullOrEmpty(spinExpPrice.Text))
                    {
                        medicineInstock.ExpPrice = null;
                    }
                    else
                    {
                        medicineInstock.ExpPrice = spinExpPrice.Value;
                    }
                    if (string.IsNullOrEmpty(spinVat.Text))
                    {
                        medicineInstock.ExpVatRatio = null;
                    }
                    else
                    {
                        medicineInstock.ExpVatRatio = spinVat.Value;
                    }
                    this.mediMateInStockADOProcess.Add(medicineInstock);
                }
                else
                {
                    var materialInStock = new MediMateInStockADO(this.hisMaterialInStockSDO);
                    if (CheckExistMediMaty(this.hisMaterialInStockSDO))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThuocVatTuDaCoTrongDanhSachXuatBanCoMuonThayTheThuoc, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            return;
                        }
                        else
                        {
                            this.mediMateInStockADOProcess.RemoveAll(o => o.MEDI_MATE_TYPE_CODE == materialInStock.MEDI_MATE_TYPE_CODE && !o.IsMedicine);
                        }
                    }
                    if (mediMateInStockADOProcess == null)
                    {
                        mediMateInStockADOProcess = new List<MediMateInStockADO>();
                    }
                    if (mediMateInStockADOProcess.Count == 0)
                    {
                        materialInStock.ID_ROW = 0;
                    }
                    else
                    {
                        var idRowMax = mediMateInStockADOProcess.Max(o => o.ID_ROW);
                        materialInStock.ID_ROW = idRowMax + 1;
                    }

                    materialInStock.EXP_AMOUNT = spinAmount.Value;
                    if (string.IsNullOrEmpty(spinExpPrice.Text))
                    {
                        materialInStock.ExpPrice = null;
                    }
                    else
                    {
                        materialInStock.ExpPrice = spinExpPrice.Value;
                    }
                    if (string.IsNullOrEmpty(spinVat.Text))
                    {
                        materialInStock.ExpVatRatio = null;
                    }
                    else
                    {
                        materialInStock.ExpVatRatio = spinVat.Value;
                    }
                    this.mediMateInStockADOProcess.Add(materialInStock);
                }
                ResetLeftBottomControl();
                FocusKeyWord();
                gridControlExpMestDetail.BeginUpdate();
                gridControlExpMestDetail.DataSource = this.mediMateInStockADOProcess;
                gridControlExpMestDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var mediMatyFocus = (MediMateInStockADO)gridViewExpMestDetail.GetFocusedRow();
                if (mediMatyFocus != null)
                {
                    this.mediMateInStockADOProcess.RemoveAll(o => o.ID_ROW == mediMatyFocus.ID_ROW);
                }
                SetDataToGridControlProcess();
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
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedistock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    mediStock = null;
                    if (!String.IsNullOrEmpty(cboMedistock.Text))
                    {
                        string key = cboMedistock.Text.ToLower();
                        var listData = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboMedistock.EditValue = listData.First().ID;
                            ReloadDataGridControlProcess();
                            mediStock = listData.First();
                            this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            EnableControl(this.ActionType);
                            ResetRightControl();
                            ResetLeftBottomControl();
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboMedistock.Focus();
                        cboMedistock.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (xtraTabControlMain.SelectedTabPageIndex == 0)//thuoc
                    {
                        this.mediInStockProcessor.FocusKeyword(this.ucMediInStock);
                    }
                    else
                    {
                        this.mateInStockProcessor.FocusKeyword(this.ucMateInStock);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                //ResetLeftControl();
                ResetRightControl();
                this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                EnableControl(this.ActionType);
                SetDefaultMedistock();
                ReloadDataGridControlProcess();
                ResetLeftBottomControl();
                FocusPresCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMedistock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtExpMedistock.Text))
                    {
                        string key = txtExpMedistock.Text.ToLower();
                        var listData = listExpMediStock.Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboMedistock.EditValue = listData.First().ID;
                            this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            EnableControl(this.ActionType);
                            ResetRightControl();
                            ReloadDataGridControlProcess();
                            ResetLeftBottomControl();
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboMedistock.Focus();
                        cboMedistock.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMedistock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    cboMedistock.Focus();
                    cboMedistock.ShowPopup();
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMedistock.EditValue = null;
                    ReloadDataGridControlProcess();
                    ResetLeftBottomControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedistock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                V_HIS_MEDI_STOCK mestRoom = null;
                if (cboMedistock.EditValue != null)
                {
                    mestRoom = listExpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboMedistock.EditValue));
                }
                if (mestRoom != null)
                {
                    txtExpMedistock.Properties.Buttons[1].Visible = true;
                    txtExpMedistock.Text = mestRoom.MEDI_STOCK_NAME;
                    this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    EnableControl(this.ActionType);
                    ResetRightControl();
                    ReloadDataGridControlProcess();
                }
                else
                {
                    txtExpMedistock.Properties.Buttons[1].Visible = false;
                    txtExpMedistock.Text = "";
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinAmount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinExpPrice.Focus();
                    spinExpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinVat.Focus();
                    spinVat.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinVat_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region public fuction
        public void BtnAdd()
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnSave()
        {
            try
            {
                gridViewExpMestDetail.PostEditor();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnNew()
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

        public void FocusPresCode()
        {
            try
            {
                if (txtExpMedistock.Enabled)
                {
                    txtExpMedistock.Focus();
                    txtExpMedistock.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

    }
}
