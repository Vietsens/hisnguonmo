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
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleEdit.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleEdit.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.ExpMestSaleEdit
{
    public partial class UCExpMestSaleEdit : UserControl
    {
        MedicineTypeInStockProcessor mediInStockProcessor = null;
        MaterialTypeInStockTreeProcessor mateInStockProcessor = null;
        ExpMestMedicineProcessor expMestMediProcessor = null;
        ExpMestMaterialProcessor expMestMateProcessor = null;

        UserControl ucMediInStock = null;
        UserControl ucMateInStock = null;
        UserControl ucExpMestMedi = null;
        UserControl ucExpMestMate = null;

        List<HisMedicineTypeInStockSDO> listMediTypeInStock;
        List<HisMaterialTypeInStockSDO> listMateTypeInStock;
        Dictionary<long, HisMedicineTypeInStockSDO> dicMedicineTypeStock = new Dictionary<long, HisMedicineTypeInStockSDO>();
        Dictionary<long, HisMaterialTypeInStockSDO> dicMaterialTypeStock = new Dictionary<long, HisMaterialTypeInStockSDO>();

        Dictionary<long, MediMateTypeADO> dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();
        MediMateTypeADO currentMediMate = null;
        HIS_PATIENT_TYPE patientType = null;
        V_HIS_PRESCRIPTION prescription = null;

        HisSaleExpMestResultSDO resultSdo = null;
        bool isUpdate = false;
        decimal medicineAmount = 0;
        decimal materialAmount = 0;

        V_HIS_MEDI_STOCK mediStock = null;
        long roomId;
        long roomTypeId;
        Inventec.Desktop.Common.Modules.Module currentModule;

        long? expMestIdForEdit = null;

        int positionHandleControl = -1;

        public UCExpMestSaleEdit(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.roomTypeId = currentModule.RoomTypeId;
                    this.roomId = currentModule.RoomId;
                }
                InitMedicineTree();
                InitMaterialTree();
                InitExpMestMateGrid();
                InitExpMestMediGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCExpMestSaleEdit(Inventec.Desktop.Common.Modules.Module module, long expMestId)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.roomTypeId = currentModule.RoomTypeId;
                    this.roomId = currentModule.RoomId;
                }
                expMestIdForEdit = expMestId;
                InitMedicineTree();
                InitMaterialTree();
                InitExpMestMateGrid();
                InitExpMestMediGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMedicineTree()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCExpMestSaleEdit;
                this.mediInStockProcessor = new MedicineTypeInStockProcessor();
                MedicineTypeInStockInitADO ado = new MedicineTypeInStockInitADO();
                ado.IsShowButtonAdd = false;
                ado.IsShowCheckNode = false;
                ado.IsShowSearchPanel = true;
                ado.IsAutoWidth = true;
                ado.MedicineTypeInStockClick = medicineInStockTree_CLick;
                ado.MedicineTypeInStockRowEnter = medicineInStockTree_RowEnter;
                ado.MedicineTypeInStockColumns = new List<MedicineTypeInStockColumn>();
                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__TXT_KEYWORD__NULL_VALUE", langManager, culture);

                MedicineTypeInStockColumn colMedicineTypeCode = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MEDICINE_TREE__COLUMN_MEDICINE_TYPE_CODE", langManager, culture), "MedicineTypeCode", 100, false);
                colMedicineTypeCode.VisibleIndex = 0;
                ado.MedicineTypeInStockColumns.Add(colMedicineTypeCode);

                MedicineTypeInStockColumn colMedicineTypeName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MEDICINE_TREE__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "MedicineTypeName", 250, false);
                colMedicineTypeName.VisibleIndex = 1;
                ado.MedicineTypeInStockColumns.Add(colMedicineTypeName);

                MedicineTypeInStockColumn colServiceUnitName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MEDICINE_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 50, false);
                colServiceUnitName.VisibleIndex = 2;
                ado.MedicineTypeInStockColumns.Add(colServiceUnitName);

                MedicineTypeInStockColumn colAvailableAmount = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MEDICINE_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 100, false);
                colAvailableAmount.VisibleIndex = 3;
                colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
                colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAvailableAmount.Format.FormatString = "#,##0.00";
                ado.MedicineTypeInStockColumns.Add(colAvailableAmount);

                MedicineTypeInStockColumn colNationalName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MEDICINE_TREE__COLUMN_NATIONAL_NAME", langManager, culture), "NationalName", 100, false);
                colNationalName.VisibleIndex = 4;
                ado.MedicineTypeInStockColumns.Add(colNationalName);

                MedicineTypeInStockColumn colManufacturerName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MEDICINE_TREE__COLUMN_MANUFACTURER_NAME", langManager, culture), "ManufacturerName", 120, false);
                colManufacturerName.VisibleIndex = 5;
                ado.MedicineTypeInStockColumns.Add(colManufacturerName);

                MedicineTypeInStockColumn colActiveIngrBhytName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MEDICINE_TREE__COLUMN_ACTIVE_INGR_BHYT_NAME", langManager, culture), "ActiveIngrBhytName", 120, false);
                colActiveIngrBhytName.VisibleIndex = 6;
                ado.MedicineTypeInStockColumns.Add(colActiveIngrBhytName);

                MedicineTypeInStockColumn colRegisterNumber = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MEDICINE_TREE__COLUMN_REGISTER_NUMBER", langManager, culture), "RegisterNumber", 70, false);
                colRegisterNumber.VisibleIndex = 7;
                ado.MedicineTypeInStockColumns.Add(colRegisterNumber);

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

        private void InitMaterialTree()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCExpMestSaleEdit;

                this.mateInStockProcessor = new MaterialTypeInStockTreeProcessor();
                MaterialTypeInStockInitADO ado = new MaterialTypeInStockInitADO();
                ado.IsShowButtonAdd = false;
                ado.IsShowCheckNode = false;
                ado.IsShowSearchPanel = true;
                ado.IsAutoWidth = true;
                ado.MaterialTypeInStockClick = materialInStockTree_Click;
                ado.MaterialTypeInStockRowEnter = materialInStockTree_EnterRow;
                ado.MaterialTypeInStockColumns = new List<MaterialTypeInStockColumn>();
                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__TXT_KEYWORD__NULL_VALUE", langManager, culture);

                MaterialTypeInStockColumn colMaterialTypeCode = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_CODE", langManager, culture), "MaterialTypeCode", 100, false);
                colMaterialTypeCode.VisibleIndex = 0;
                ado.MaterialTypeInStockColumns.Add(colMaterialTypeCode);

                MaterialTypeInStockColumn colMaterialTypeName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MaterialTypeName", 300, false);
                colMaterialTypeName.VisibleIndex = 1;
                ado.MaterialTypeInStockColumns.Add(colMaterialTypeName);

                MaterialTypeInStockColumn colServiceUnitName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MATERIAL_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 60, false);
                colServiceUnitName.VisibleIndex = 2;
                ado.MaterialTypeInStockColumns.Add(colServiceUnitName);

                MaterialTypeInStockColumn colAvailableAmount = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MATERIAL_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 110, false);
                colAvailableAmount.VisibleIndex = 3;
                colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
                colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAvailableAmount.Format.FormatString = "#,##0.00";
                ado.MaterialTypeInStockColumns.Add(colAvailableAmount);

                MaterialTypeInStockColumn colNationalName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MATERIAL_TREE__COLUMN_NATIONAL_NAME", langManager, culture), "NationalName", 120, false);
                colNationalName.VisibleIndex = 4;
                ado.MaterialTypeInStockColumns.Add(colNationalName);

                MaterialTypeInStockColumn colManufacturerName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__MATERIAL_TREE__COLUMN_MANUFACTURER_NAME", langManager, culture), "ManufacturerName", 150, false);
                colManufacturerName.VisibleIndex = 5;
                ado.MaterialTypeInStockColumns.Add(colManufacturerName);

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

        private void InitExpMestMediGrid()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCExpMestSaleEdit;

                this.expMestMediProcessor = new ExpMestMedicineProcessor();
                ExpMestMedicineInitADO ado = new ExpMestMedicineInitADO();
                ado.ExpMestMedicineGrid_CustomUnboundColumnData = expMestMediGrid__CustomUnboundColumnData;
                ado.IsShowSearchPanel = false;
                ado.ListExpMestMedicineColumn = new List<ExpMestMedicineColumn>();

                ExpMestMedicineColumn colMedicineTypeName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "MEDICINE_TYPE_NAME", 170, false);
                colMedicineTypeName.VisibleIndex = 0;
                ado.ListExpMestMedicineColumn.Add(colMedicineTypeName);

                ExpMestMedicineColumn colServiceUnitName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 40, false);
                colServiceUnitName.VisibleIndex = 1;
                ado.ListExpMestMedicineColumn.Add(colServiceUnitName);

                ExpMestMedicineColumn colAmount = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_AMOUNT", langManager, culture), "AMOUNT", 100, false);
                colAmount.VisibleIndex = 2;
                colAmount.Format = new DevExpress.Utils.FormatInfo();
                colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAmount.Format.FormatString = "#,##0.00";
                ado.ListExpMestMedicineColumn.Add(colAmount);

                ExpMestMedicineColumn colExpiredDate = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_EXPIRED_DATE", langManager, culture), "EXPIRED_DATE_STR", 90, false);
                colExpiredDate.VisibleIndex = 3;
                colExpiredDate.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExpMestMedicineColumn.Add(colExpiredDate);

                ExpMestMedicineColumn colPackageNumber = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_PACKAGE_NUMBER", langManager, culture), "PACKAGE_NUMBER", 60, false);
                colPackageNumber.VisibleIndex = 4;
                ado.ListExpMestMedicineColumn.Add(colPackageNumber);

                ExpMestMedicineColumn colNationalName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_NATIONAL_NAME", langManager, culture), "NATIONAL_NAME", 100, false);
                colNationalName.VisibleIndex = 5;
                ado.ListExpMestMedicineColumn.Add(colNationalName);

                ExpMestMedicineColumn colManufacturerName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_MANUFACTURER_NAME", langManager, culture), "MANUFACTURER_NAME", 100, false);
                colManufacturerName.VisibleIndex = 6;
                ado.ListExpMestMedicineColumn.Add(colManufacturerName);

                this.ucExpMestMedi = (UserControl)this.expMestMediProcessor.Run(ado);
                if (this.ucExpMestMedi != null)
                {
                    this.xtraTabPageExpMestMedi.Controls.Add(this.ucExpMestMedi);
                    this.ucExpMestMedi.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitExpMestMateGrid()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCExpMestSaleEdit;

                this.expMestMateProcessor = new ExpMestMaterialProcessor();
                ExpMestMaterialInitADO ado = new ExpMestMaterialInitADO();
                ado.ExpMestMaterialGrid_CustomUnboundColumnData = expMestMateGrid__CustomUnboundColumnData;
                ado.IsShowSearchPanel = false;
                ado.ListExpMestMaterialColumn = new List<ExpMestMaterialColumn>();

                ExpMestMaterialColumn colMaterialTypeName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MATERIAL_TYPE_NAME", 170, false);
                colMaterialTypeName.VisibleIndex = 0;
                ado.ListExpMestMaterialColumn.Add(colMaterialTypeName);

                ExpMestMaterialColumn colServiceUnitName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 40, false);
                colServiceUnitName.VisibleIndex = 1;
                ado.ListExpMestMaterialColumn.Add(colServiceUnitName);

                ExpMestMaterialColumn colAmount = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_AMOUNT", langManager, culture), "AMOUNT", 100, false);
                colAmount.VisibleIndex = 2;
                colAmount.Format = new DevExpress.Utils.FormatInfo();
                colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAmount.Format.FormatString = "#,##0.00";
                ado.ListExpMestMaterialColumn.Add(colAmount);

                ExpMestMaterialColumn colExpiredDate = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_EXPIRED_DATE", langManager, culture), "EXPIRED_DATE_STR", 90, false);
                colExpiredDate.VisibleIndex = 3;
                colExpiredDate.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExpMestMaterialColumn.Add(colExpiredDate);

                ExpMestMaterialColumn colPackageNumber = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_PACKAGE_NUMBER", langManager, culture), "PACKAGE_NUMBER", 60, false);
                colPackageNumber.VisibleIndex = 4;
                ado.ListExpMestMaterialColumn.Add(colPackageNumber);

                ExpMestMaterialColumn colNationalName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_NATIONAL_NAME", langManager, culture), "NATIONAL_NAME", 100, false);
                colNationalName.VisibleIndex = 5;
                ado.ListExpMestMaterialColumn.Add(colNationalName);

                ExpMestMaterialColumn colManufacturerName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_MANUFACTURER_NAME", langManager, culture), "MANUFACTURER_NAME", 100, false);
                colManufacturerName.VisibleIndex = 6;
                ado.ListExpMestMaterialColumn.Add(colManufacturerName);

                this.ucExpMestMate = (UserControl)this.expMestMateProcessor.Run(ado);
                if (this.ucExpMestMate != null)
                {
                    this.xtraTabPageExpMestMate.Controls.Add(this.ucExpMestMate);
                    this.ucExpMestMate.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestSaleEdit_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadMediStockByRoomId();
                if (this.mediStock != null)
                {
                    LoadKeyUCLanguage();
                    ValidControl();
                    FillDataToTreeMediMate();
                    LoadDataToCboPatientType();
                    LoadDataToCboSampleForm();
                    LoadDataToCboGender();
                    ResetValueControlCommon();
                    ResetValueControlDetail();
                    SetEnableControlPriceByCheckBox();

                    if (ExpMestSaleEditCFG.patientTypeIdByCode > 0)
                    {
                        cboPatientType.EditValue = ExpMestSaleEditCFG.patientTypeIdByCode;
                        patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == ExpMestSaleEditCFG.patientTypeIdByCode);
                    }

                    InitResultSdoByExpMestId();
                    GenerateMenuPrint();
                    SetFocusTreeMediOrMate();
                }
                else
                {
                    btnAdd.Enabled = false;
                    btnSave.Enabled = false;
                    ddBtnPrint.Enabled = false;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboGender()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboGender, BackendDataWorker.Get<HIS_GENDER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitResultSdoByExpMestId()
        {
            try
            {
                this.resultSdo = null;
                if (this.expMestIdForEdit.HasValue && this.expMestIdForEdit.Value > 0)
                {
                    HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    expMestFilter.ID = this.expMestIdForEdit.Value;
                    var listExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, null);
                    if (listExpMest == null || listExpMest.Count != 1)
                    {
                        throw new Exception("Khong lay duoc expMest theo id: " + expMestIdForEdit.Value);
                    }
                    var expMest = listExpMest.FirstOrDefault();

                    HisSaleExpMestFilter saleExpMestFilter = new HisSaleExpMestFilter();
                    saleExpMestFilter.EXP_MEST_ID = expMest.ID;
                    var listSaleExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SALE_EXP_MEST>>("api/HisSaleExpMest/Get", ApiConsumers.MosConsumer, saleExpMestFilter, null);
                    if (listSaleExpMest == null || listSaleExpMest.Count != 1)
                    {
                        throw new Exception("Khong lay du saleExpMest theo expMestId: " + expMest.ID);
                    }
                    var saleExpMest = listSaleExpMest.FirstOrDefault();

                    HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_ID = expMest.ID;
                    var listExpMestMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, null);

                    HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.EXP_MEST_ID = expMest.ID;
                    var listExpMestMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, null);

                    resultSdo = new HisSaleExpMestResultSDO();
                    resultSdo.ExpMaterials = listExpMestMaterial;
                    resultSdo.ExpMedicines = listExpMestMedicine;
                    resultSdo.ExpMest = expMest;
                    resultSdo.SaleExpMest = saleExpMest;
                    cboPatientType.EditValue = saleExpMest.PATIENT_TYPE_ID;
                    txtDescription.Text = expMest.DESCRIPTION;
                    txtVirPatientName.Text = saleExpMest.CLIENT_NAME;
                    cboGender.EditValue = saleExpMest.CLIENT_GENDER_ID;
                    dtDob.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(saleExpMest.CLIENT_DOB ?? 0);
                    txtAddress.Text = saleExpMest.CLIENT_ADDRESS;

                    ProcessFillDataBySuccess();
                    FillDataToGridExpMest();
                    FillDataGridExpMestDetail();
                    SetTotalPrice();
                    //SetEnableControlPriceByCheckBox();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMediStockByRoomId()
        {
            try
            {
                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_TYPE_ID == this.roomTypeId && o.ROOM_ID == this.roomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboPatientType()
        {
            try
            {
                cboPatientType.Properties.DataSource = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ValueMember = "ID";
                cboPatientType.Properties.ForceInitialize();
                cboPatientType.Properties.Columns.Clear();
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_CODE", "", 50));
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_NAME", "", 120));
                cboPatientType.Properties.ShowHeader = false;
                cboPatientType.Properties.ImmediatePopup = true;
                cboPatientType.Properties.DropDownRows = 10;
                cboPatientType.Properties.PopupWidth = 170;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboSampleForm()
        {
            try
            {
                cboSampleForm.Properties.DataSource = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>();
                cboSampleForm.Properties.DisplayMember = "EXP_MEST_TEMPLATE_NAME";
                cboSampleForm.Properties.ValueMember = "ID";
                cboSampleForm.Properties.ForceInitialize();
                cboSampleForm.Properties.Columns.Clear();
                cboSampleForm.Properties.Columns.Add(new LookUpColumnInfo("EXP_MEST_TEMPLATE_CODE", "", 50));
                cboSampleForm.Properties.Columns.Add(new LookUpColumnInfo("EXP_MEST_TEMPLATE_NAME", "", 200));
                cboSampleForm.Properties.ShowHeader = false;
                cboSampleForm.Properties.ImmediatePopup = true;
                cboSampleForm.Properties.DropDownRows = 10;
                cboSampleForm.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTreeMediMate()
        {
            try
            {
                dicMaterialTypeStock.Clear();
                dicMedicineTypeStock.Clear();
                listMateTypeInStock = null;
                listMediTypeInStock = null;
                if (mediStock != null)
                {
                    HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                    mediFilter.MEDI_STOCK_ID = mediStock.ID;
                    mediFilter.IS_LEAF = true;
                    listMediTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, null);

                    HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                    mateFilter.MEDI_STOCK_ID = mediStock.ID;
                    mateFilter.IS_LEAF = true;
                    listMateTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mateFilter, null);
                }

                if (listMediTypeInStock != null && listMediTypeInStock.Count > 0)
                {
                    listMediTypeInStock = listMediTypeInStock.Where(o => o.AvailableAmount > 0).ToList();
                    foreach (var item in listMediTypeInStock)
                    {
                        dicMedicineTypeStock[item.Id] = item;
                    }
                }

                if (listMateTypeInStock != null && listMateTypeInStock.Count > 0)
                {
                    listMateTypeInStock = listMateTypeInStock.Where(o => o.AvailableAmount > 0).ToList();
                    foreach (var item in listMateTypeInStock)
                    {
                        dicMaterialTypeStock[item.Id] = item;
                    }
                }

                this.mediInStockProcessor.Reload(this.ucMediInStock, listMediTypeInStock);
                this.mateInStockProcessor.Reload(this.ucMateInStock, listMateTypeInStock);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetValueControlCommon()
        {
            try
            {
                if (this.mediStock != null)
                {
                    txtExpMediStock.Text = this.mediStock.MEDI_STOCK_NAME;
                }
                else
                {
                    txtExpMediStock.Text = "";
                }
                dicMediMateAdo.Clear();
                this.prescription = null;
                this.currentMediMate = null;
                this.resultSdo = null;
                this.isUpdate = false;
                this.SetTotalPrice();
                txtPrescriptionCode.Text = "";
                txtVirPatientName.Text = "";
                txtVirPatientName.Enabled = true;
                txtAddress.Enabled = true;
                cboGender.Enabled = true;
                dtDob.Enabled = true;
                cboPatientType.EditValue = ExpMestSaleEditCFG.patientTypeIdByCode;
                checkIsVisitor.Checked = false;
                txtAddress.Text = "";
                cboGender.EditValue = null;
                dtDob.EditValue = null;     
                txtSampleForm.Text = "";
                cboSampleForm.EditValue = null;
                txtDescription.Text = "";
                ddBtnPrint.Enabled = false;
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
                gridControlExpMestDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpMestDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetValueControlDetail()
        {
            try
            {
                this.currentMediMate = null;
                spinAmount.Value = 0;
                spinExpPrice.Value = 0;
                spinExpVatRatio.Value = 0;
                spinDiscount.Value = 0;
                txtTutorial.Text = "";
                txtNote.Text = "";
                btnAdd.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableControlPriceByCheckBox()
        {
            try
            {
                if (checkImpExpPrice.Checked)
                {
                    spinExpPrice.Enabled = true;
                    spinExpPrice.Value = 0;
                    spinExpVatRatio.Enabled = true;
                    spinExpVatRatio.Value = 0;
                    spinDiscount.Enabled = true;
                    spinDiscount.Value = 0;
                }
                else
                {
                    spinExpPrice.Enabled = false;
                    spinExpPrice.Value = 0;
                    spinExpVatRatio.Enabled = false;
                    spinExpVatRatio.Value = 0;
                    spinDiscount.Enabled = false;
                    spinDiscount.Value = 0;
                }
                if (this.currentMediMate != null && this.patientType != null)
                {
                    var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_PATY>>(HisRequestUriStore.HIS_SERVICE_PATY_GET_APPLIED_VIEW, ApiConsumers.MosConsumer, null, null, "serviceId", this.currentMediMate.SERVICE_ID, "treatmentTime", null);
                    if (listServicePaty != null)
                    {
                        var paty = listServicePaty.FirstOrDefault(o => o.PATIENT_TYPE_ID == this.patientType.ID);
                        if (paty != null)
                        {
                            spinExpPrice.Value = paty.PRICE;
                            spinExpVatRatio.Value = paty.VAT_RATIO * 100;
                        }
                    }
                }
                else if (this.currentMediMate != null && cboPatientType.EditValue != null)
                {
                    var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_PATY>>(HisRequestUriStore.HIS_SERVICE_PATY_GET_APPLIED_VIEW, ApiConsumers.MosConsumer, null, null, "serviceId", this.currentMediMate.SERVICE_ID, "treatmentTime", null);
                    if (listServicePaty != null)
                    {
                        var paty = listServicePaty.FirstOrDefault(o => o.PATIENT_TYPE_ID == (long)cboPatientType.EditValue);
                        if (paty != null)
                        {
                            spinExpPrice.Value = paty.PRICE;
                            spinExpVatRatio.Value = paty.VAT_RATIO * 100;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFocusTreeMediOrMate()
        {
            try
            {
                if (xtraTabControlMain.SelectedTabPageIndex == 0)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridExpMest()
        {
            try
            {
                List<V_HIS_EXP_MEST_MEDICINE> listMedicine = null;
                List<V_HIS_EXP_MEST_MATERIAL> listMaterial = null;
                if (this.resultSdo != null)
                {
                    listMedicine = this.resultSdo.ExpMedicines;
                    listMaterial = this.resultSdo.ExpMaterials;
                }
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    listMedicine = listMedicine.OrderBy(o => o.ID).ToList();
                }
                if (listMaterial != null && listMaterial.Count > 0)
                {
                    listMaterial = listMaterial.OrderBy(o => o.ID).ToList();
                }
                this.expMestMediProcessor.Reload(this.ucExpMestMedi, listMedicine);
                this.expMestMateProcessor.Reload(this.ucExpMestMate, listMaterial);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GenerateMenuPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__PRINT_MENU__ITEM_IN_PHIEU_XUAT_BAN", Base.ResourceLangManager.LanguageUCExpMestSaleEdit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInPhieuXuatBan)));
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__PRINT_MENU__ITEM_IN_HUONG_DAN_SU_DUNG", Base.ResourceLangManager.LanguageUCExpMestSaleEdit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInHuongDanSuDung)));

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTotalPrice()
        {
            try
            {
                decimal totalPrice = 0;
                if (dicMediMateAdo.Count > 0)
                {
                    //totalPrice = dicMediMateAdo.Select(s => s.Value).ToList().Sum(o => o.ADVISORY_TOTAL_PRICE ?? 0);
                    totalPrice = dicMediMateAdo.Select(s => s.Value).ToList().Sum(o => o.EXP_AMOUNT * o.ADVISORY_PRICE ?? 0);
                }
                lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0);
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
                ValidControlExpMediStock();
                ValidControlPatientType();
                ValidControlPatyPrice();
                ValidControlExpAmount();
                ValidControlExpPrice();
                ValidControlExpVatRatio();
                ValidControlDiscount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpMediStock()
        {
            try
            {
                ExpMediStockValidationRule mediStockRule = new ExpMediStockValidationRule();
                mediStockRule.txtExpMediStockName = txtExpMediStock;
                dxValidationProvider1.SetValidationRule(txtExpMediStock, mediStockRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlPatientType()
        {
            try
            {
                PatientTypeValidationRule patientTypeRule = new PatientTypeValidationRule();
                patientTypeRule.cboPatientType = cboPatientType;
                dxValidationProvider1.SetValidationRule(cboPatientType, patientTypeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlPatyPrice()
        {
            try
            {
                PatientTypePriceValidationRule patyPriceRule = new PatientTypePriceValidationRule();
                patyPriceRule.checkImpExpPrice = checkImpExpPrice;
                patyPriceRule.cboPatientType = cboPatientType;
                dxValidationProvider2.SetValidationRule(cboPatientType, patyPriceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpAmount()
        {
            try
            {
                ExpAmountValidationRule amountRule = new ExpAmountValidationRule();
                amountRule.spinAmount = spinAmount;
                dxValidationProvider2.SetValidationRule(spinAmount, amountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpPrice()
        {
            try
            {
                ExpPriceValidationRule priceRule = new ExpPriceValidationRule();
                priceRule.checkImpExpPrice = checkImpExpPrice;
                priceRule.spinExpPrice = spinExpPrice;
                dxValidationProvider2.SetValidationRule(spinExpPrice, priceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpVatRatio()
        {
            try
            {
                ExpVatRatioValidationRule vatRatioRule = new ExpVatRatioValidationRule();
                vatRatioRule.checkImpExpPrice = checkImpExpPrice;
                vatRatioRule.spinExpVatRatio = spinExpVatRatio;
                dxValidationProvider2.SetValidationRule(spinExpVatRatio, vatRatioRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDiscount()
        {
            try
            {
                DiscountValidationRule discountRule = new DiscountValidationRule();
                discountRule.spinDiscount = spinDiscount;
                discountRule.checkImpExpPrice = checkImpExpPrice;
                discountRule.spinAmount = spinAmount;
                discountRule.spinExpPrice = spinExpPrice;
                dxValidationProvider2.SetValidationRule(spinDiscount, discountRule);
            }
            catch (Exception ex)
            {
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

        private void LoadKeyUCLanguage()
        {
            try
            {
                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCExpMestSaleEdit;
                //Button
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__BTN_ADD", langManager, cul);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__BTN_SAVE", langManager, cul);
                this.btnSavePrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__BTN_SAVE_PRINT", langManager, cul);
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__BTN_PRINT", langManager, cul);

                //Layout
                this.checkImpExpPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_CHECK_IMP_EXP_PRICE", langManager, cul);
                this.checkIsVisitor.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_CHECK_VISITOR", langManager, cul);
                this.layoutAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_EXP_AMOUNT", langManager, cul);
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_DESCRIPTION", langManager, cul);
                this.layoutDiscount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_DISCOUNT", langManager, cul);
                this.layoutExpMediStock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_EXP_MEDI_STOCK", langManager, cul);
                this.layoutExpPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_EXP_PRICE", langManager, cul);
                this.layoutExpVatRatio.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_EXP_VAT_RATIO", langManager, cul);
                this.layoutNote.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_NOTE", langManager, cul);
                this.layoutPatient.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_PATIENT", langManager, cul);
                this.layoutPatientType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_PATIENT_TYPE", langManager, cul);
                this.layoutPrescriptionCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_PRESCRIPTION_CODE", langManager, cul);
                this.layoutSampleForm.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_EXP_MEST_TEMPLATE", langManager, cul);
                this.layoutTutorial.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__LAYOUT_TUTORIAL", langManager, cul);

                //GridControl Detail
                this.gridColumn_ExpMestDetail_ExpAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_CONTROL__COLUMN_AMOUNT", langManager, cul);
                this.gridColumn_ExpMestDetail_ManufacturerName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_CONTROL__COLUMN_MANUFACTURER_NAME", langManager, cul);
                this.gridColumn_ExpMestDetail_MediMateTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_CONTROL__COLUMN_MEDI_MATE_TYPE_NAME", langManager, cul);
                this.gridColumn_ExpMestDetail_NationalName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_CONTROL__COLUMN_NATIONAL_NAME", langManager, cul);
                this.gridColumn_ExpMestDetail_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__GRID_CONTROL__COLUMN_SERVICE_UNIT_NAME", langManager, cul);

                //Xtra Tab
                this.xtraTabPageExpMestMate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__XTRA_TAB_MATERIAL", langManager, cul);
                this.xtraTabPageExpMestMedi.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__XTRA_TAB_MEDICINE", langManager, cul);
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__XTRA_TAB_MATERIAL", langManager, cul);
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__XTRA_TAB_MEDICINE", langManager, cul);

                //Repository Button
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__REPOSITORY_BTN_DELETE", langManager, cul);

                //NUll Value
                this.txtPrescriptionCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__TXT_PRESCRIPTION_CODE__NULL_VALUE", langManager, cul);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        public void BtnSavePrint()
        {
            try
            {
                gridViewExpMestDetail.PostEditor();
                btnSavePrint_Click(null, null);
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
                if (txtPrescriptionCode.Enabled)
                {
                    txtPrescriptionCode.Focus();
                    txtPrescriptionCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
