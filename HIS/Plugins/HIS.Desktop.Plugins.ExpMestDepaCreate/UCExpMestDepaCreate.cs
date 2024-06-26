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
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.HisBloodTypeInStock;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.SDO;
using HIS.Desktop.Plugins.ExpMestDepaCreate.ADO;
using HIS.Desktop.Plugins.ExpMestDepaCreate.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Menu;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Controls.EditorLoader;
namespace HIS.Desktop.Plugins.ExpMestDepaCreate
{
    public partial class UCExpMestDepaCreate : UserControlBase
    {
        MedicineTypeInStockProcessor mediInStockProcessor = null;
        MaterialTypeInStockTreeProcessor mateInStockProcessor = null;
        MaterialTypeInStockTreeProcessor hoaChatInStockProcessor = null;
        HisBloodTypeInStockProcessor mauInStockProcessor = null;

        ExpMestMedicineProcessor expMestMediProcessor = null;
        ExpMestMaterialProcessor expMestMateProcessor = null;
        ExpMestMaterialProcessor expMestHoaChatProcessor = null;

        UserControl ucMediInStock = null;
        UserControl ucMateInStock = null;
        UserControl ucHoaChatInStock = null;
        UserControl ucMau = null;
        UserControl ucExpMestMedi = null;
        UserControl ucExpMestMate = null;
        UserControl ucExpMestHoaChat = null;

        Dictionary<long, MediMateTypeADO> dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();
        MediMateTypeADO currentMediMate = null;

        List<V_HIS_MEST_ROOM> listExpMediStock;

        HisExpMestResultSDO resultSdo = null;
        bool isUpdate = false;

        long roomId;
        long roomTypeId;
        V_HIS_ROOM currentRoom = null;

        int positionHandleControl = -1;

        public UCExpMestDepaCreate(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.roomTypeId = module.RoomTypeId;
                this.roomId = module.RoomId;
                InitMedicineTree();
                InitMaterialTree();
                InitMaterialTree_HC();
                InitBloodTree();
                InitExpMestMateGrid();
                InitExpMestMediGrid();
                InitExpMestMateGrid_HC();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCExpMestDepaCreate(V_HIS_EXP_MEST _expMest)
        {
            InitializeComponent();
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboRespositoryBloodRH()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemGridLookUpEditBloodRH, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRespositoryBloodABO()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemGridLookUpEditBloodABO, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitBloodTree()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCExpMestDepaCreate;
                this.mauInStockProcessor = new HisBloodTypeInStockProcessor();
                HisBloodTypeInStockInitADO ado = new HisBloodTypeInStockInitADO();
                ado.IsShowButtonAdd = false;
                ado.IsShowCheckNode = false;
                ado.IsShowSearchPanel = true;
                ado.IsAutoWidth = true;
                ado.IsShowButtonExpand = false;
                ado.HisBloodTypeInStockClick = bloodInStockTree_Click;
                ado.HisBloodTypeInStockRowEnter = bloodInStockTree_RowEnter;
                ado.HisBloodTypeInStockColumns = new List<HisBloodTypeInStockColumn>();
                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__TXT_KEYWORD__NULL_VALUE", langManager, culture);

                HisBloodTypeInStockColumn colMedicineTypeCode = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_MEDICINE_TYPE_CODE", langManager, culture), "BloodTypeCode", 70, false);
                colMedicineTypeCode.VisibleIndex = 0;
                ado.HisBloodTypeInStockColumns.Add(colMedicineTypeCode);

                HisBloodTypeInStockColumn colMedicineTypeName = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "BloodTypeName", 250, false);
                colMedicineTypeName.VisibleIndex = 1;
                ado.HisBloodTypeInStockColumns.Add(colMedicineTypeName);

                //HisBloodTypeInStockColumn colServiceUnitName = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 70, false);
                //colServiceUnitName.VisibleIndex = 2;
                //ado.HisBloodTypeInStockColumns.Add(colServiceUnitName);

                HisBloodTypeInStockColumn colVolume = new HisBloodTypeInStockColumn("Dung tích (ml)", "Volume", 70, false);
                colVolume.VisibleIndex = 2;
                ado.HisBloodTypeInStockColumns.Add(colVolume);

                HisBloodTypeInStockColumn colAvailableAmount = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "Amount", 100, false);
                colAvailableAmount.VisibleIndex = 3;
                colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
                colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAvailableAmount.Format.FormatString = "#,##0.00";
                ado.HisBloodTypeInStockColumns.Add(colAvailableAmount);



                this.ucMau = (UserControl)this.mauInStockProcessor.Run(ado);
                if (this.ucMau != null)
                {
                    this.xtraTabPageMau.Controls.Add(this.ucMau);
                    this.ucMau.Dock = DockStyle.Fill;
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
                var langManager = Base.ResourceLangManager.LanguageUCExpMestDepaCreate;

                this.mateInStockProcessor = new MaterialTypeInStockTreeProcessor();
                MaterialTypeInStockInitADO ado = new MaterialTypeInStockInitADO();
                ado.IsShowButtonAdd = false;
                ado.IsShowCheckNode = false;
                ado.IsShowSearchPanel = true;
                ado.IsAutoWidth = true;
                ado.MaterialTypeInStockClick = materialInStockTree_Click;
                ado.MaterialTypeInStockRowEnter = materialInStockTree_EnterRow;
                ado.MaterialTypeInStockColumns = new List<MaterialTypeInStockColumn>();
                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__TXT_KEYWORD__NULL_VALUE", langManager, culture);

                MaterialTypeInStockColumn colMaterialTypeCode = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_CODE", langManager, culture), "MaterialTypeCode", 80, false);
                colMaterialTypeCode.VisibleIndex = 0;
                ado.MaterialTypeInStockColumns.Add(colMaterialTypeCode);

                MaterialTypeInStockColumn colMaterialTypeName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MaterialTypeName", 300, false);
                colMaterialTypeName.VisibleIndex = 1;
                ado.MaterialTypeInStockColumns.Add(colMaterialTypeName);

                MaterialTypeInStockColumn colServiceUnitName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 60, false);
                colServiceUnitName.VisibleIndex = 2;
                ado.MaterialTypeInStockColumns.Add(colServiceUnitName);

                MaterialTypeInStockColumn colAvailableAmount = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 110, false);
                colAvailableAmount.VisibleIndex = 3;
                colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
                colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAvailableAmount.Format.FormatString = "#,##0.00";
                ado.MaterialTypeInStockColumns.Add(colAvailableAmount);

                MaterialTypeInStockColumn colNationalName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_NATIONAL_NAME", langManager, culture), "NationalName", 150, false);
                colNationalName.VisibleIndex = 4;
                ado.MaterialTypeInStockColumns.Add(colNationalName);

                MaterialTypeInStockColumn colManufacturerName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_MANUFACTURER_NAME", langManager, culture), "ManufacturerName", 200, false);
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

        private void InitMaterialTree_HC()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCExpMestDepaCreate;

                this.hoaChatInStockProcessor = new MaterialTypeInStockTreeProcessor();
                MaterialTypeInStockInitADO ado = new MaterialTypeInStockInitADO();
                ado.IsShowButtonAdd = false;
                ado.IsShowCheckNode = false;
                ado.IsShowSearchPanel = true;
                ado.IsAutoWidth = true;
                ado.MaterialTypeInStockClick = materialInStockTree_Click_HC;
                ado.MaterialTypeInStockRowEnter = materialInStockTree_EnterRow_HC;
                ado.MaterialTypeInStockColumns = new List<MaterialTypeInStockColumn>();
                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__TXT_KEYWORD__NULL_VALUE", langManager, culture);

                MaterialTypeInStockColumn colMaterialTypeCode = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_CODE", langManager, culture), "MaterialTypeCode", 80, false);
                colMaterialTypeCode.VisibleIndex = 0;
                ado.MaterialTypeInStockColumns.Add(colMaterialTypeCode);

                MaterialTypeInStockColumn colMaterialTypeName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MaterialTypeName", 300, false);
                colMaterialTypeName.VisibleIndex = 1;
                ado.MaterialTypeInStockColumns.Add(colMaterialTypeName);

                MaterialTypeInStockColumn colMaterialConcenTra = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_CONCEN_TRA", langManager, culture), "Concentra", 70, false);
                colMaterialConcenTra.VisibleIndex = 2;
                ado.MaterialTypeInStockColumns.Add(colMaterialConcenTra);

                MaterialTypeInStockColumn colServiceUnitName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 60, false);
                colServiceUnitName.VisibleIndex = 3;
                ado.MaterialTypeInStockColumns.Add(colServiceUnitName);

                MaterialTypeInStockColumn colAvailableAmount = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 110, false);
                colAvailableAmount.VisibleIndex = 4;
                colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
                colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAvailableAmount.Format.FormatString = "#,##0.00";
                ado.MaterialTypeInStockColumns.Add(colAvailableAmount);

                MaterialTypeInStockColumn colNationalName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_NATIONAL_NAME", langManager, culture), "NationalName", 150, false);
                colNationalName.VisibleIndex = 5;
                ado.MaterialTypeInStockColumns.Add(colNationalName);

                MaterialTypeInStockColumn colManufacturerName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MATERIAL_TREE__COLUMN_MANUFACTURER_NAME", langManager, culture), "ManufacturerName", 200, false);
                colManufacturerName.VisibleIndex = 6;
                ado.MaterialTypeInStockColumns.Add(colManufacturerName);

                this.ucHoaChatInStock = (UserControl)this.hoaChatInStockProcessor.Run(ado);
                if (this.ucHoaChatInStock != null)
                {
                    this.xtraTabPageHoaChat.Controls.Add(this.ucHoaChatInStock);
                    this.ucHoaChatInStock.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMedicineTree()
        {
            var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            var langManager = Base.ResourceLangManager.LanguageUCExpMestDepaCreate;
            this.mediInStockProcessor = new MedicineTypeInStockProcessor();
            MedicineTypeInStockInitADO ado = new MedicineTypeInStockInitADO();
            ado.IsShowButtonAdd = false;
            ado.IsShowCheckNode = false;
            ado.IsShowSearchPanel = true;
            ado.IsAutoWidth = true;
            ado.MedicineTypeInStockClick = medicineInStockTree_CLick;
            ado.MedicineTypeInStockRowEnter = medicineInStockTree_RowEnter;
            ado.MedicineTypeInStockColumns = new List<MedicineTypeInStockColumn>();
            ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__TXT_KEYWORD__NULL_VALUE", langManager, culture);

            MedicineTypeInStockColumn colMedicineTypeCode = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_MEDICINE_TYPE_CODE", langManager, culture), "MedicineTypeCode", 70, false);
            colMedicineTypeCode.VisibleIndex = 0;
            ado.MedicineTypeInStockColumns.Add(colMedicineTypeCode);

            MedicineTypeInStockColumn colMedicineTypeName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "MedicineTypeName", 250, false);
            colMedicineTypeName.VisibleIndex = 1;
            ado.MedicineTypeInStockColumns.Add(colMedicineTypeName);

            MedicineTypeInStockColumn colMedicineConcenTra = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_CONCEN_TRA", langManager, culture), "Concentra", 70, false);
            colMedicineConcenTra.VisibleIndex = 2;
            ado.MedicineTypeInStockColumns.Add(colMedicineConcenTra);

            MedicineTypeInStockColumn colServiceUnitName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 50, false);
            colServiceUnitName.VisibleIndex = 3;
            ado.MedicineTypeInStockColumns.Add(colServiceUnitName);

            MedicineTypeInStockColumn colAvailableAmount = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 100, false);
            colAvailableAmount.VisibleIndex = 4;
            colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
            colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            colAvailableAmount.Format.FormatString = "#,##0.00";
            ado.MedicineTypeInStockColumns.Add(colAvailableAmount);

            MedicineTypeInStockColumn colNationalName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_NATIONAL_NAME", langManager, culture), "NationalName", 100, false);
            colNationalName.VisibleIndex = 5;
            ado.MedicineTypeInStockColumns.Add(colNationalName);

            MedicineTypeInStockColumn colManufacturerName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_MANUFACTURER_NAME", langManager, culture), "ManufacturerName", 120, false);
            colManufacturerName.VisibleIndex = 6;
            ado.MedicineTypeInStockColumns.Add(colManufacturerName);

            MedicineTypeInStockColumn colActiveIngrBhytName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_ACTIVE_INGR_BHYT_NAME", langManager, culture), "ActiveIngrBhytName", 120, false);
            colActiveIngrBhytName.VisibleIndex = 7;
            ado.MedicineTypeInStockColumns.Add(colActiveIngrBhytName);

            MedicineTypeInStockColumn colRegisterNumber = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__MEDICINE_TREE__COLUMN_REGISTER_NUMBER", langManager, culture), "RegisterNumber", 70, false);
            colRegisterNumber.VisibleIndex = 8;
            ado.MedicineTypeInStockColumns.Add(colRegisterNumber);

            this.ucMediInStock = (UserControl)this.mediInStockProcessor.Run(ado);
            if (this.ucMediInStock != null)
            {
                this.xtraTabPageMedicine.Controls.Add(this.ucMediInStock);
                this.ucMediInStock.Dock = DockStyle.Fill;
            }
        }

        private void InitExpMestMediGrid()
        {
            try
            {
                //var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //var langManager = Base.ResourceLangManager.LanguageUCExpMestDepaCreate;

                //this.expMestMediProcessor = new ExpMestMedicineProcessor();
                //ExpMestMedicineInitADO ado = new ExpMestMedicineInitADO();
                //ado.ExpMestMedicineGrid_CustomUnboundColumnData = expMestMediGrid__CustomUnboundColumnData;
                //ado.IsShowSearchPanel = false;
                //ado.ListExpMestMedicineColumn = new List<ExpMestMedicineColumn>();

                //ExpMestMedicineColumn colMedicineTypeName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "MEDICINE_TYPE_NAME", 170, false);
                //colMedicineTypeName.VisibleIndex = 0;
                //ado.ListExpMestMedicineColumn.Add(colMedicineTypeName);

                //ExpMestMedicineColumn colServiceUnitName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 40, false);
                //colServiceUnitName.VisibleIndex = 1;
                //ado.ListExpMestMedicineColumn.Add(colServiceUnitName);

                //ExpMestMedicineColumn colAmount = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_AMOUNT", langManager, culture), "AMOUNT", 100, false);
                //colAmount.VisibleIndex = 2;
                //colAmount.Format = new DevExpress.Utils.FormatInfo();
                //colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                //colAmount.Format.FormatString = "#,##0.00";
                //ado.ListExpMestMedicineColumn.Add(colAmount);

                ////ExpMestMedicineColumn colExpiredDate = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_EXPIRED_DATE", langManager, culture), "EXPIRED_DATE_STR", 90, false);
                ////colExpiredDate.VisibleIndex = 3;
                ////colExpiredDate.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ////ado.ListExpMestMedicineColumn.Add(colExpiredDate);

                ////ExpMestMedicineColumn colPackageNumber = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_PACKAGE_NUMBER", langManager, culture), "PACKAGE_NUMBER", 60, false);
                ////colPackageNumber.VisibleIndex = 4;
                ////ado.ListExpMestMedicineColumn.Add(colPackageNumber);

                //ExpMestMedicineColumn colNationalName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_NATIONAL_NAME", langManager, culture), "NATIONAL_NAME", 100, false);
                //colNationalName.VisibleIndex = 5;
                //ado.ListExpMestMedicineColumn.Add(colNationalName);

                //ExpMestMedicineColumn colManufacturerName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MEDICINE__COLUMN_MANUFACTURER_NAME", langManager, culture), "MANUFACTURER_NAME", 100, false);
                //colManufacturerName.VisibleIndex = 6;
                //ado.ListExpMestMedicineColumn.Add(colManufacturerName);

                //this.ucExpMestMedi = (UserControl)this.expMestMediProcessor.Run(ado);
                //if (this.ucExpMestMedi != null)
                //{
                //    this.xtraTabPageExpMestMedi.Controls.Add(this.ucExpMestMedi);
                //    this.ucExpMestMedi.Dock = DockStyle.Fill;
                //}
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
                //var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //var langManager = Base.ResourceLangManager.LanguageUCExpMestDepaCreate;

                //this.expMestMateProcessor = new ExpMestMaterialProcessor();
                //ExpMestMaterialInitADO ado = new ExpMestMaterialInitADO();
                //ado.ExpMestMaterialGrid_CustomUnboundColumnData = expMestMateGrid__CustomUnboundColumnData;
                //ado.IsShowSearchPanel = false;
                //ado.ListExpMestMaterialColumn = new List<ExpMestMaterialColumn>();

                //ExpMestMaterialColumn colMaterialTypeName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MATERIAL_TYPE_NAME", 170, false);
                //colMaterialTypeName.VisibleIndex = 0;
                //ado.ListExpMestMaterialColumn.Add(colMaterialTypeName);

                //ExpMestMaterialColumn colServiceUnitName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 40, false);
                //colServiceUnitName.VisibleIndex = 1;
                //ado.ListExpMestMaterialColumn.Add(colServiceUnitName);

                //ExpMestMaterialColumn colAmount = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_AMOUNT", langManager, culture), "AMOUNT", 100, false);
                //colAmount.VisibleIndex = 2;
                //colAmount.Format = new DevExpress.Utils.FormatInfo();
                //colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                //colAmount.Format.FormatString = "#,##0.00";
                //ado.ListExpMestMaterialColumn.Add(colAmount);

                //// ExpMestMaterialColumn colExpiredDate = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_EXPIRED_DATE", langManager, culture), "EXPIRED_DATE_STR", 90, false);
                //// colExpiredDate.VisibleIndex = 3;
                //// colExpiredDate.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ////ado.ListExpMestMaterialColumn.Add(colExpiredDate);

                ////ExpMestMaterialColumn colPackageNumber = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_PACKAGE_NUMBER", langManager, culture), "PACKAGE_NUMBER", 60, false);
                ////colPackageNumber.VisibleIndex = 4;
                ////ado.ListExpMestMaterialColumn.Add(colPackageNumber);

                //ExpMestMaterialColumn colNationalName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_NATIONAL_NAME", langManager, culture), "NATIONAL_NAME", 100, false);
                //colNationalName.VisibleIndex = 5;
                //ado.ListExpMestMaterialColumn.Add(colNationalName);

                //ExpMestMaterialColumn colManufacturerName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_MANUFACTURER_NAME", langManager, culture), "MANUFACTURER_NAME", 100, false);
                //colManufacturerName.VisibleIndex = 6;
                //ado.ListExpMestMaterialColumn.Add(colManufacturerName);

                //this.ucExpMestMate = (UserControl)this.expMestMateProcessor.Run(ado);
                //if (this.ucExpMestMate != null)
                //{
                //    this.xtraTabPageExpMestMate.Controls.Add(this.ucExpMestMate);
                //    this.ucExpMestMate.Dock = DockStyle.Fill;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitExpMestMateGrid_HC()
        {
            try
            {
                //var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //var langManager = Base.ResourceLangManager.LanguageUCExpMestDepaCreate;

                //this.expMestHoaChatProcessor = new ExpMestMaterialProcessor();
                //ExpMestMaterialInitADO ado = new ExpMestMaterialInitADO();
                //ado.ExpMestMaterialGrid_CustomUnboundColumnData = expMestMateGrid__CustomUnboundColumnData;
                //ado.IsShowSearchPanel = false;
                //ado.ListExpMestMaterialColumn = new List<ExpMestMaterialColumn>();

                //ExpMestMaterialColumn colMaterialTypeName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MATERIAL_TYPE_NAME", 170, false);
                //colMaterialTypeName.VisibleIndex = 0;
                //ado.ListExpMestMaterialColumn.Add(colMaterialTypeName);

                //ExpMestMaterialColumn colServiceUnitName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 40, false);
                //colServiceUnitName.VisibleIndex = 1;
                //ado.ListExpMestMaterialColumn.Add(colServiceUnitName);

                //ExpMestMaterialColumn colAmount = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_AMOUNT", langManager, culture), "AMOUNT", 100, false);
                //colAmount.VisibleIndex = 2;
                //colAmount.Format = new DevExpress.Utils.FormatInfo();
                //colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                //colAmount.Format.FormatString = "#,##0.00";
                //ado.ListExpMestMaterialColumn.Add(colAmount);

                //ExpMestMaterialColumn colNationalName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_NATIONAL_NAME", langManager, culture), "NATIONAL_NAME", 100, false);
                //colNationalName.VisibleIndex = 5;
                //ado.ListExpMestMaterialColumn.Add(colNationalName);

                //ExpMestMaterialColumn colManufacturerName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_EXP_MEST_MATERIAL__COLUMN_MANUFACTURER_NAME", langManager, culture), "MANUFACTURER_NAME", 100, false);
                //colManufacturerName.VisibleIndex = 6;
                //ado.ListExpMestMaterialColumn.Add(colManufacturerName);

                //this.ucExpMestHoaChat = (UserControl)this.expMestHoaChatProcessor.Run(ado);
                //if (this.ucExpMestHoaChat != null)
                //{
                //    this.xtraTabPageExpMestHoaChat.Controls.Add(this.ucExpMestHoaChat);
                //    this.ucExpMestHoaChat.Dock = DockStyle.Fill;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestDepaCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadRoomById();


                if (this.currentRoom != null)
                {
                    ValidControl();
                    LoadDataToCboExpMediStock();
                    LoadDataIsRequiredReason();
                    SetDataGridControlDetail();
                    ResetControlCommon();
                    GenerateMenuPrint();
                    cboExpMediStock.Focus();
                    cboExpMediStock.SelectAll();
                }
                else
                {
                    btnAdd.Enabled = false;
                    btnSave.Enabled = false;
                    btnNew.Enabled = false;
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

        private void LoadRoomById()
        {
            try
            {
                this.currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId && o.ROOM_TYPE_ID == this.roomTypeId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboExpMediStock()
        {
            try
            {
                listExpMediStock = new List<V_HIS_MEST_ROOM>();
                var listCabinet = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => o.IS_CABINET == 1 && o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID && o.IS_ACTIVE == 1).ToList();

                //if (listCabinetId != null && listCabinetId.Count > 0)
                //{
                listExpMediStock = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == currentRoom.ID && o.IS_ACTIVE == 1).ToList();
                //}

                //var listCabinet = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => listCabinetId.Contains(o.MEDI_STOCK_ID) && o.ROOM_ID == currentRoom.ID).ToList();

                if (listCabinet != null && listCabinet.Count > 0)
                {
                    foreach (var item in listCabinet)
                    {
                        if (listExpMediStock != null && listExpMediStock.Count > 0)
                        {
                            if (listExpMediStock.Select(o => o.MEDI_STOCK_ID).Contains(item.ID) != true)
                            {
                                V_HIS_MEST_ROOM mestRoom = new V_HIS_MEST_ROOM();
                                mestRoom.ROOM_ID = item.ROOM_ID;
                                mestRoom.MEDI_STOCK_ID = item.ID;
                                mestRoom.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
                                mestRoom.MEDI_STOCK_CODE = item.MEDI_STOCK_CODE;
                                listExpMediStock.Add(mestRoom);
                            }
                        }
                    }

                }

                if (listExpMediStock != null && listExpMediStock.Count > 0)
                {
                    List<long> _MediStockIds = listExpMediStock.Select(p => p.MEDI_STOCK_ID).ToList();
                    var mediStock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(p => _MediStockIds.Contains(p.ID) && p.IS_ACTIVE == 1).ToList();
                    if (mediStock != null && mediStock.Count > 0)
                    {
                        listExpMediStock = listExpMediStock.Where(p => mediStock.Select(o => o.ID).Contains(p.MEDI_STOCK_ID)).OrderByDescending(o => o.PRIORITY).ToList();
                    }
                    else
                        listExpMediStock = new List<V_HIS_MEST_ROOM>();
                }

                cboExpMediStock.Properties.DataSource = listExpMediStock;
                cboExpMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboExpMediStock.Properties.ValueMember = "MEDI_STOCK_ID";
                cboExpMediStock.Properties.ForceInitialize();
                cboExpMediStock.Properties.Columns.Clear();
                cboExpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 50));
                cboExpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 120));
                cboExpMediStock.Properties.ShowHeader = false;
                cboExpMediStock.Properties.ImmediatePopup = true;
                cboExpMediStock.Properties.DropDownRows = 10;
                cboExpMediStock.Properties.PopupWidth = 170;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataIsRequiredReason()
        {

            try
            {
                cboREASON.Properties.Buttons[1].Visible = false;
                List<long> _MediStockIds = listExpMediStock.Select(p => p.MEDI_STOCK_ID).ToList();
                var reason = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON>().Where(p => p.IS_DEPA == 1 && p.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboREASON, reason, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


        private void ResetControlCommon()
        {
            try
            {
                this.resultSdo = null;
                xtraTabControlMain.SelectedTabPageIndex = 0;
                isUpdate = false;
                dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();
                this.currentMediMate = null;
                cboExpMediStock.Enabled = true;
                btnSave.Enabled = true;
                ddBtnPrint.Enabled = false;
                if (listExpMediStock != null && listExpMediStock.Count > 0)
                {
                    var room = listExpMediStock.OrderByDescending(o => o.PRIORITY).FirstOrDefault();
                    if (room != null)
                    {
                        cboExpMediStock.EditValue = room.MEDI_STOCK_ID;
                        LoadDataToTreeList(room);
                    }
                    else
                        cboExpMediStock.EditValue = null;
                }
                else
                    cboExpMediStock.EditValue = null;
                txtDescription.Text = "";
                spinRemedyCount.EditValue = null;
                cboREASON.EditValue = null;
                if (this.currentRoom != null)
                {
                    txtReqDepartmentName.Text = this.currentRoom.DEPARTMENT_NAME;
                    txtReqRoomName.Text = this.currentRoom.ROOM_NAME;
                    if (!String.IsNullOrEmpty(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()))
                    {
                        txtReqUsername.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    }
                    else
                    {
                        txtReqUsername.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    }
                }
                else
                {
                    txtReqDepartmentName.Text = "";
                    txtReqRoomName.Text = "";
                    txtReqUsername.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataGridControlDetail()
        {
            try
            {
                this.lciRemedyCount.Enabled = !(dicMediMateAdo.Select(o => o.Value).ToList().Exists(o => !o.IsMedicine)) && xtraTabControlMain.SelectedTabPageIndex == 0;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMediMateAdo.Select(o => o.Value).ToList()), dicMediMateAdo.Select(o => o.Value).ToList()));
                gridControlExpMestDetail.BeginUpdate();
                gridControlExpMestDetail.DataSource = dicMediMateAdo.Select(o => o.Value).ToList();
                gridControlExpMestDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToTreeList(V_HIS_MEST_ROOM mestRoom)
        {
            try
            {
                long key = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__TACH_VAT_TU_HOA_CHAT));
                if (key == 1)
                {
                    xtraTabControlMain.TabPages[2].PageVisible = true;
                }
                else
                {
                    xtraTabControlMain.TabPages[2].PageVisible = false;
                }

                List<HisMedicineTypeInStockSDO> listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                List<HisMaterialTypeInStockSDO> listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                List<HisMaterialTypeInStockSDO> listMateTypeInStock_HCs = new List<HisMaterialTypeInStockSDO>();
                List<HisBloodTypeInStockSDO> listBloodTypeInStock = new List<HisBloodTypeInStockSDO>();
                dicMediMateAdo.Clear();
                if (mestRoom != null)
                {
                    CommonParam param = new CommonParam();
                    #region -----Thuoc ----
                    //MOS.Filter.HisMetyAvailableViewFilter metyFilter = new HisMetyAvailableViewFilter();
                    //metyFilter.MEDI_STOCK_ID = mestRoom.MEDI_STOCK_ID;
                    //metyFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //metyFilter.IS_AVAILABLE = true;
                    //var _MetyAvailables = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_METY_AVAILABLE>>("api/HisMetyAvailable/GetView", ApiConsumers.MosConsumer, metyFilter, param);
                    //if (_MetyAvailables != null && _MetyAvailables.Count > 0)
                    //{
                    //    var _medicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    //    foreach (var item in _MetyAvailables)
                    //    {
                    //        if (item.AMOUNT <= 0)
                    //        {
                    //            continue;
                    //        }
                    //        HisMedicineTypeInStockSDO ado = new HisMedicineTypeInStockSDO();
                    //        ado.Id = item.MEDICINE_TYPE_ID;
                    //        ado.IsActive = item.IS_ACTIVE;
                    //        ado.AvailableAmount = item.AMOUNT;
                    //        ado.MedicineTypeCode = item.MEDICINE_TYPE_CODE;
                    //        ado.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    //        ado.NationalName = item.NATIONAL_NAME;
                    //        ado.ActiveIngrBhytName = item.ACTIVE_INGR_BHYT_NAME;
                    //        ado.RegisterNumber = item.REGISTER_NUMBER;
                    //        ado.ServiceUnitName = item.SERVICE_UNIT_NAME;
                    //        ado.ServiceId = item.MEDICINE_TYPE_ID;

                    //        var data = _medicineTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                    //        if (data != null)
                    //        {
                    //            ado.ManufacturerName = data.MANUFACTURER_NAME;
                    //        }
                    //        listMediTypeInStock.Add(ado);

                    //    }
                    //}
                    #endregion

                    #region ------Vat Tu ----
                    //MOS.Filter.HisMatyAvailableViewFilter matyFilter = new HisMatyAvailableViewFilter();
                    //matyFilter.MEDI_STOCK_ID = mestRoom.MEDI_STOCK_ID;
                    //matyFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //matyFilter.IS_AVAILABLE = true;
                    //var _MatyAvailables = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_MATY_AVAILABLE>>("api/HisMatyAvailable/GetView", ApiConsumers.MosConsumer, matyFilter, param);
                    //if (_MatyAvailables != null && _MatyAvailables.Count > 0)
                    //{
                    //    var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    //    foreach (var item in _MatyAvailables)
                    //    {
                    //        if (item.AMOUNT <= 0)
                    //        {
                    //            continue;
                    //        }
                    //        HisMaterialTypeInStockSDO ado = new HisMaterialTypeInStockSDO();
                    //        ado.Id = item.MATERIAL_TYPE_ID;
                    //        ado.IsActive = item.IS_ACTIVE;
                    //        ado.AvailableAmount = item.AMOUNT;
                    //        ado.MaterialTypeCode = item.MATERIAL_TYPE_CODE;
                    //        ado.MaterialTypeName = item.MATERIAL_TYPE_NAME;
                    //        ado.NationalName = item.NATIONAL_NAME;
                    //        ado.ServiceUnitName = item.SERVICE_UNIT_NAME;
                    //        ado.ServiceId = item.MATERIAL_TYPE_ID;

                    //        var data = _materialTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                    //        if (data != null)
                    //        {
                    //            ado.ManufacturerName = data.MANUFACTURER_NAME;
                    //        }
                    //        if (key == 1 && data != null && data.IS_CHEMICAL_SUBSTANCE == 1)
                    //        {
                    //            listMateTypeInStock_HCs.Add(ado);
                    //        }
                    //        else
                    //            listMateTypeInStock.Add(ado);
                    //    }
                    //}
                    #endregion

                    #region ---- Code Cu ----
                    #region thuốc
                    HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                    mediFilter.MEDI_STOCK_ID = mestRoom.MEDI_STOCK_ID;
                    mediFilter.IS_LEAF = true;
                    mediFilter.IS_AVAILABLE = true;
                    mediFilter.IS_ACTIVE = true;
                    listMediTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, null);

                    if (listMediTypeInStock != null && listMediTypeInStock.Count > 0)
                    {
                        var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == mestRoom.MEDI_STOCK_ID);
                        if (mediStock != null && mediStock.IS_BUSINESS == 1)
                        {
                            var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                            if (dataMediTypes != null && dataMediTypes.Count > 0)
                            {
                                listMediTypeInStock = listMediTypeInStock.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                            }
                        }
                        else
                        {
                            var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                            if (dataMediTypes != null && dataMediTypes.Count > 0)
                            {
                                listMediTypeInStock = listMediTypeInStock.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                            }
                        }
                    }

                    var metyDepa = BackendDataWorker.Get<HIS_MEST_METY_DEPA>();
                    if (metyDepa != null && metyDepa.Count > 0)
                    {
                        metyDepa = metyDepa.Where(o => o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID && (!o.MEDI_STOCK_ID.HasValue || o.MEDI_STOCK_ID == mestRoom.MEDI_STOCK_ID) && o.IS_JUST_PRESCRIPTION != 1).ToList();
                    }

                    if (metyDepa != null && metyDepa.Count > 0 && listMediTypeInStock != null && listMediTypeInStock.Count > 0)
                    {
                        listMediTypeInStock = listMediTypeInStock.Where(o => !metyDepa.Select(s => s.MEDICINE_TYPE_ID).Contains(o.Id)).ToList();
                    }
                    #endregion
                    #region vật
                    HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                    mateFilter.MEDI_STOCK_ID = mestRoom.MEDI_STOCK_ID;
                    mateFilter.IS_LEAF = true;
                    mateFilter.IS_AVAILABLE = true;
                    mateFilter.IS_ACTIVE = true;
                    var dataMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mateFilter, null);

                    if (dataMaterials != null && dataMaterials.Count > 0)
                    {
                        var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == mestRoom.MEDI_STOCK_ID);
                        if (mediStock != null && mediStock.IS_BUSINESS == 1)
                        {
                            var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                            if (dataMateTypes != null && dataMateTypes.Count > 0)
                            {
                                dataMaterials = dataMaterials.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                            }
                        }
                        else
                        {
                            var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                            if (dataMateTypes != null && dataMateTypes.Count > 0)
                            {
                                dataMaterials = dataMaterials.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                dataMaterials = new List<HisMaterialTypeInStockSDO>();
                            }
                        }

                        var matyDepa = BackendDataWorker.Get<HIS_MEST_MATY_DEPA>();
                        if (matyDepa != null && matyDepa.Count > 0)
                        {
                            matyDepa = matyDepa.Where(o => o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID && (!o.MEDI_STOCK_ID.HasValue || o.MEDI_STOCK_ID == mestRoom.MEDI_STOCK_ID) && o.IS_JUST_PRESCRIPTION != 1).ToList();
                        }

                        if (matyDepa != null && matyDepa.Count > 0 && dataMaterials != null && dataMaterials.Count > 0)
                        {
                            dataMaterials = dataMaterials.Where(o => !matyDepa.Select(s => s.MATERIAL_TYPE_ID).Contains(o.Id)).ToList();
                        }
                    }
                    #endregion
                    #region máu

                    //MOS.Filter.HisBloodViewFilter filter = new MOS.Filter.HisBloodViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //filter.BLOOD_TYPE_IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //filter.MEDI_STOCK_ID = mestRoom.MEDI_STOCK_ID;
                    //List<V_HIS_BLOOD> lstBloods = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BLOOD>>("/api/HisBlood/GetView", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);

                    //HisBloodTypeFilter hbt = new HisBloodTypeFilter();
                    //hbt.IS_ACTIVE = 1;
                    //hbt.IDs = lstBloods.Select(o => o.BLOOD_TYPE_ID).ToList();
                    //List<HIS_BLOOD_TYPE> lstBloodsType = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BLOOD_TYPE>>("/api/HisBloodType/Get", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);

                    //lstBloodsType = lstBloodsType.Where(o => o.IS_LEAF == (short?)1).ToList();

                    Inventec.Common.Logging.LogSystem.Error("mestRoom.MEDI_STOCK_ID_____" + mestRoom.MEDI_STOCK_ID);
                    HisBloodTypeStockViewFilter blfilter = new HisBloodTypeStockViewFilter();
                    blfilter.MEDI_STOCK_ID = mestRoom.MEDI_STOCK_ID;
                    blfilter.IS_LEAF = true;
                    //                   blfilter.IS_AVAILABLE = true;
                    blfilter.IS_ACTIVE = 1;
                    listBloodTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisBloodTypeInStockSDO>>("api/HisBloodType/GetInStockBloodType", ApiConsumers.MosConsumer, blfilter, null);

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listBloodTypeInStock), listBloodTypeInStock));

                    if (listBloodTypeInStock != null && listBloodTypeInStock.Count > 0)
                    {
                        //var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == mestRoom.MEDI_STOCK_ID);
                        //if (mediStock != null && mediStock.IS_BUSINESS == 1)
                        //{
                        var dataBloodTypes = BackendDataWorker.Get<HIS_BLOOD_TYPE>().Where(p => p.IS_LEAF == 1).ToList();
                        if (dataBloodTypes != null && dataBloodTypes.Count > 0)
                        {
                            listBloodTypeInStock = listBloodTypeInStock.Where(p => dataBloodTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();

                        }
                        else
                        {
                            listBloodTypeInStock = new List<HisBloodTypeInStockSDO>();
                        }
                        //}
                        //else
                        //{
                        //    var dataBloodTypes = BackendDataWorker.Get<HIS_BLOOD_TYPE>().Where(p => p.IS_LEAF != 1).ToList();
                        //    if (dataBloodTypes != null && dataBloodTypes.Count > 0)
                        //    {
                        //        listBloodTypeInStock = listBloodTypeInStock.Where(p => dataBloodTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                        //    }
                        //    else
                        //    {
                        //        listBloodTypeInStock = new List<HisBloodTypeInStockSDO>();
                        //    }
                        //}
                    }
                    #endregion
                    #region hc
                    listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                    listMateTypeInStock_HCs = new List<HisMaterialTypeInStockSDO>();
                    if (dataMaterials != null && dataMaterials.Count > 0)
                    {
                        foreach (var item in dataMaterials)
                        {
                            var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.Id);

                            if (key == 1 && data != null && data.IS_CHEMICAL_SUBSTANCE == 1)
                            {
                                listMateTypeInStock_HCs.Add(item);
                            }
                            else
                                listMateTypeInStock.Add(item);
                        }
                    }
                    #endregion
                    #endregion
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listBloodTypeInStock), listBloodTypeInStock));
                this.mediInStockProcessor.Reload(this.ucMediInStock, listMediTypeInStock);
                this.mateInStockProcessor.Reload(this.ucMateInStock, listMateTypeInStock);
                this.hoaChatInStockProcessor.Reload(this.ucHoaChatInStock, listMateTypeInStock_HCs);
                this.mauInStockProcessor.Reload(this.ucMau, listBloodTypeInStock);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFocusMediOrMateStock()
        {
            try
            {

                if (xtraTabControlMain.SelectedTabPageIndex == 0)
                {
                    this.mediInStockProcessor.FocusKeyword(this.ucMediInStock);
                    this.lciRemedyCount.Enabled = !(dicMediMateAdo.Select(o => o.Value).ToList().Exists(o => !o.IsMedicine));
                }
                else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                {
                    this.mateInStockProcessor.FocusKeyword(this.ucMateInStock);
                    this.lciRemedyCount.Enabled = false;
                    this.spinRemedyCount.EditValue = null;

                }
                else
                {
                    this.mauInStockProcessor.FocusKeyword(this.ucMau);
                    this.lciRemedyCount.Enabled = false;
                    this.spinRemedyCount.EditValue = null;

                }
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
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__PRINT_MENU__ITEM_IN_PHIEU_XUAT_SU_DUNG", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuXuatSuDung)));
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__PRINT_MENU__ITEM_IN_PHIEU_XUAT_SU_DUNG_THEO_DIEU_KIEN", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuXuatSuDungTheoDieuKien)));

                ddBtnPrint.DropDownControl = menu;
                ddBtnPrint.Enabled = false;
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
                ValidControlReqDepartment();
                ValidControlReqRoom();
                ValidControlReqUsername();
                ValidControlExpAmount();

                long keyCFG_IsRequiredReason = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ExpMestDepaCreate_IsRequiredReason));
                string keyCFG_MOS_IsReasonRequired = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__MOS__EXP_MEST__IS_REASON_REQUIRED) ?? "";
                if (keyCFG_IsRequiredReason == 1 || keyCFG_MOS_IsReasonRequired.Trim() == "1")
                {
                    ValidControlIsRequiredReason();
                    lcIsRequiredReason.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    lcIsRequiredReason.AppearanceItemCaption.ForeColor = Color.Black;
                }
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
                mediStockRule.cboExpMediStock = cboExpMediStock;
                dxValidationProvider1.SetValidationRule(cboExpMediStock, mediStockRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlIsRequiredReason()
        {
            try
            {
                IsRequiredReasonValidationRule IsRequiredReason = new IsRequiredReasonValidationRule();
                IsRequiredReason.cbo = cboREASON;
                dxValidationProvider1.SetValidationRule(cboREASON, IsRequiredReason);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidControlReqDepartment()
        {
            try
            {
                ReqDepartmentValidationRule departmentRule = new ReqDepartmentValidationRule();
                departmentRule.txtReqDepartment = txtReqDepartmentName;
                dxValidationProvider1.SetValidationRule(txtReqDepartmentName, departmentRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlReqRoom()
        {
            try
            {
                ReqRoomValidationRule roomRule = new ReqRoomValidationRule();
                roomRule.txtReqRoom = txtReqRoomName;
                dxValidationProvider1.SetValidationRule(txtReqRoomName, roomRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlReqUsername()
        {
            try
            {
                ReqUsernameValidationRule usernameRule = new ReqUsernameValidationRule();
                usernameRule.txtReqUsername = txtReqUsername;
                dxValidationProvider1.SetValidationRule(txtReqUsername, usernameRule);
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
                amountRule.spinExpAmount = spinExpAmount;
                dxValidationProvider2.SetValidationRule(spinExpAmount, amountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        private void dxValidationProvider2_ValidationFailed(object sender, ValidationFailedEventArgs e)
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
                //Button
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__BTN_ADD", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__BTN_NEW", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__BTN_SAVE", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__BTN_PRINT", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);

                //Layout
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__LAYOUT_DESCRIPTION", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.layoutExpAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__LAYOUT_EXP_AMOUNT", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.layoutExpMestMediStock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__LAYOUT_EXP_MEST_MEDI_STOCK", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.layoutNote.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__LAYOUT_NOTE", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.layoutReqDepartment.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__LAYOUT_REQ_DEPARTMENT", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.layoutReqRoom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__LAYOUT_REQ_ROOM", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.layoutReqUsername.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__LAYOUT_REQ_USERNAME", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);

                //GridControl Detail
                this.gridColumn_ExpMestDetail_ExpAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_CONTROL__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.gridColumn_ExpMestDetail_ManufacturerName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_CONTROL__COLUMN_MANUFACTURER_NAME", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.gridColumn_ExpMestDetail_MediMateTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_CONTROL__COLUMN_MEDI_MATE_TYPE_NAME", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.gridColumn_ExpMestDetail_NationalName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_CONTROL__COLUMN_NATIONAL_NAME", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.gridColumn_ExpMestDetail_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__GRID_CONTROL__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);

                //Xtra Tab
                //this.xtraTabPageExpMestMate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__XTRA_TAB_MATERIAL", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                //this.xtraTabPageExpMestMedi.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__XTRA_TAB_MEDICINE", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__XTRA_TAB_MATERIAL", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__XTRA_TAB_MEDICINE", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);

                //Repository Button
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_DEPA_CREATE__REPOSITORY_BTN_DELETE", Base.ResourceLangManager.LanguageUCExpMestDepaCreate, cul);
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

        private void gridViewExpMestDetail_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);
                bool IsMedicine = (bool)vw.GetRowCellValue(e.RowHandle, "IsMedicine");
                bool IsHoaChat = (bool)vw.GetRowCellValue(e.RowHandle, "IsHoaChat");
                bool IsBlood = (bool)vw.GetRowCellValue(e.RowHandle, "IsBlood");
                if (IsMedicine && !IsBlood)
                {
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else if (IsHoaChat && !IsBlood)
                {
                    e.Appearance.ForeColor = Color.DarkGreen;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else if (IsBlood)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                {
                    e.Appearance.ForeColor = Color.Blue;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData)
                {
                    var data = (MediMateTypeADO)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                    {
                        if (data != null)
                        {
                            if (e.Column.FieldName == "VOLUME_str")
                            {
                                try
                                {
                                    if (data.VOLUME > 0)
                                    {
                                        e.Value = data.VOLUME;
                                    }
                                    else
                                    {
                                        e.Value = "";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                            else if (e.Column.FieldName == "BLOOD_ID_ABO_str")
                            {
                                if (data.IsBlood)
                                {
                                    e.Value = data.BLOOD_ID_ABO;
                                }
                                else
                                {
                                    e.Value = null;
                                }
                            }
                            else if (e.Column.FieldName == "BLOOD_ID_RH_str")
                            {
                                if (data.IsBlood)
                                {
                                    e.Value = data.BLOOD_ID_RH;
                                }
                                else
                                {
                                    e.Value = null;
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    MediMateTypeADO data = (MediMateTypeADO)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "BLOOD_ID_ABO_str")
                    {
                        InitComboRespositoryBloodABO();
                        Inventec.Common.Logging.LogSession.Warn("BLOOD_ID_ABO_str______________");
                        e.RepositoryItem = (data.IsBlood ? repositoryItemGridLookUpEditBloodABO : null);
                    }

                    if (e.Column.FieldName == "BLOOD_ID_RH_str")
                    {
                        InitComboRespositoryBloodRH();
                        Inventec.Common.Logging.LogSession.Warn("BLOOD_ID_RH_str______________");
                        e.RepositoryItem = (data.IsBlood ? repositoryItemGridLookUpEditBloodRH : null);

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboREASON_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboREASON.EditValue = null;
                    cboREASON.Properties.Buttons[1].Visible = false;
                    cboREASON.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboREASON_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (cboREASON.EditValue != null)
                {
                    cboREASON.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboREASON.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void spinRemedyCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboREASON.Focus();
                    cboREASON.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinRemedyCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!(e.KeyChar != '-'))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
