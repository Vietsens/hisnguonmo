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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.BidUpdate.ADO;
using HIS.Desktop.Plugins.BidUpdate.Base;
using HIS.Desktop.Plugins.BidUpdate.Config;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BidUpdate
{
    public partial class frmBidUpdate : FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module Module;
        const string timer1 = "timer1";
        internal long bid_id;
        private double idRow = -1;
        internal MOS.EFMODEL.DataModels.HIS_BID bidModel = null;
        internal MOS.EFMODEL.DataModels.HIS_BID bidPrint = null;
        internal List<ADO.MedicineTypeADO> ListMedicineTypeAdoProcess;
        internal ADO.BloodTypeADO bloodType;
        internal ADO.MedicineTypeADO medicineType;
        internal ADO.MaterialTypeADO materialType;
        internal HIS_BID currenthisBid = null;
        RefeshReference refeshData;
        UC_LoadEdit ucMedicine;
        UC_LoadEdit ucMaterial;
        UC_LoadEdit ucBlood;

        private V_HIS_MEDI_STOCK currentStock = null;
        private bool isInit = true;
        private bool hasBusiness = false;
        private bool hasNotBusiness = false;

        HIS.UC.MedicineType.MedicineTypeProcessor medicineTypeProcessor;
        HIS.UC.MaterialType.MaterialTypeTreeProcessor materialTypeProcessor;
        HIS.UC.BloodType.BloodTypeProcessor bloodTypeProcessor;
        UserControl ucMedicineType;
        UserControl ucMaterialType;
        UserControl ucBloodType;
        bool tabMaterial = true;
        bool tabBlood = true;
        bool checkLoad = false;

        internal int ActionType = 0;
        System.Globalization.CultureInfo cultureLang;
        int positionHandleLeft = -1, positionHandleRight = -1;

        List<MOS.EFMODEL.DataModels.HIS_BID_TYPE> bidTypes;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO__Create;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO__Update;

        List<BidFormADO> lstBidForm = new List<BidFormADO>();
        List<HIS_SUPPLIER> lstSupplier = new List<HIS_SUPPLIER>();
        List<SDA_NATIONAL> lstNational = new List<SDA_NATIONAL>();
        List<HIS_MANUFACTURER> lstManufacture = new List<HIS_MANUFACTURER>();
        List<HIS_MEDICINE_USE_FORM> lstMediUseForm = new List<HIS_MEDICINE_USE_FORM>();
        List<V_HIS_MEDICINE_TYPE> listHisMedicineType = new List<V_HIS_MEDICINE_TYPE>();
        List<V_HIS_MATERIAL_TYPE> listHisMaterialType = new List<V_HIS_MATERIAL_TYPE>();
        List<V_HIS_BLOOD_TYPE> listHisBloodType = new List<V_HIS_BLOOD_TYPE>();
        List<V_HIS_BID_MATERIAL_TYPE> bidMaty = new List<V_HIS_BID_MATERIAL_TYPE>();
        #endregion

        #region Construct
        public frmBidUpdate(Inventec.Desktop.Common.Modules.Module _Module, long _bid_id, RefeshReference _refeshData)
            : base(_Module)
        {
            InitializeComponent();
            try
            {
                this.Module = _Module;
                this.bid_id = _bid_id;
                this.refeshData = _refeshData;
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO__Create = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK_CREATE);
                this.currentControlStateRDO__Update = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK_UPDATE);
                InitializeMedicineType();
                InitializeMaterialType();
                InitializeBloodType();
                SetCaptionByLanguageKey();
                this.Text = _Module.text;
                LoadMedicine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        #region Initialize
        private void InitializeMedicineType()
        {
            try
            {
                medicineTypeProcessor = new UC.MedicineType.MedicineTypeProcessor();
                HIS.UC.MedicineType.MedicineTypeInitADO ado = new UC.MedicineType.MedicineTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.MedicineTypeClick = MedicineType_Click;
                ado.MedicineTypeRowEnter = MedicineType_RowEnter;
                ado.MedicineTypeColumns = new List<HIS.UC.MedicineType.MedicineTypeColumn>();

                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__KEY_WORD_NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang);

                //Column MedicineTypeCode
                HIS.UC.MedicineType.MedicineTypeColumn GcMedicineTypeCode = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "MEDICINE_TYPE_CODE", 80, false);
                GcMedicineTypeCode.VisibleIndex = 0;
                ado.MedicineTypeColumns.Add(GcMedicineTypeCode);

                //Column MedicineTypeName
                HIS.UC.MedicineType.MedicineTypeColumn GcMedicineTypeName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "MEDICINE_TYPE_NAME", 150, false);
                GcMedicineTypeName.VisibleIndex = 1;
                ado.MedicineTypeColumns.Add(GcMedicineTypeName);

                //Column ServiceUnitName
                HIS.UC.MedicineType.MedicineTypeColumn GcServiceUnitName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "SERVICE_UNIT_NAME_STR", 80, false);
                GcServiceUnitName.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                GcServiceUnitName.VisibleIndex = 2;
                ado.MedicineTypeColumns.Add(GcServiceUnitName);

                //Column ManufacturerName
                HIS.UC.MedicineType.MedicineTypeColumn GcManufacturerName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 3;
                ado.MedicineTypeColumns.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.MedicineType.MedicineTypeColumn GcNationalName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 4;
                ado.MedicineTypeColumns.Add(GcNationalName);

                //Column UseFromName
                HIS.UC.MedicineType.MedicineTypeColumn GcUseFromName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MEDICINE__GC_MEDICINE_USE_FROM_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "MEDICINE_USE_FORM_NAME", 80, false);
                GcUseFromName.VisibleIndex = 5;
                ado.MedicineTypeColumns.Add(GcUseFromName);

                //Column AvtiveIngrBhytName
                HIS.UC.MedicineType.MedicineTypeColumn GcAvtiveIngrBhytName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "ACTIVE_INGR_BHYT_NAME", 100, false);
                GcAvtiveIngrBhytName.VisibleIndex = 6;
                ado.MedicineTypeColumns.Add(GcAvtiveIngrBhytName);

                ado.MedicineType_CustomUnboundColumnData = MedicineType_CustomUnboundColumnData;

                this.ucMedicineType = (UserControl)medicineTypeProcessor.Run(ado);
                if (this.ucMedicineType != null)
                {
                    this.pnlThuoc.Controls.Add(this.ucMedicineType);
                    this.ucMedicineType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "SERVICE_UNIT_NAME_STR")
                {
                    V_HIS_MEDICINE_TYPE currentRow = e.Row as V_HIS_MEDICINE_TYPE;
                    if (currentRow == null) return;
                    if (currentRow.IMP_UNIT_ID.HasValue)
                        e.Value = currentRow.IMP_UNIT_NAME;
                    else
                        e.Value = currentRow.SERVICE_UNIT_NAME;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineType_Click(UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                //ValidBidPackage(false);
                this.medicineType = new ADO.MedicineTypeADO();
                if (data != null)
                {
                    ADO.MedicineTypeADO medicine = new ADO.MedicineTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicine, data);

                    this.ActionType = GlobalVariables.ActionAdd;
                    VisibleButton(this.ActionType);
                    this.medicineType = medicine;
                    this.medicineType.Type = Base.GlobalConfig.THUOC;
                    spinImpMoreRatio.Enabled = true;
                    SetValueForAdd();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineType_RowEnter(UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                MedicineType_Click(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitializeMaterialType()
        {
            try
            {
                materialTypeProcessor = new UC.MaterialType.MaterialTypeTreeProcessor();
                HIS.UC.MaterialType.MaterialTypeInitADO ado = new UC.MaterialType.MaterialTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.MaterialTypeClick = MaterialType_Click;
                ado.MaterialTypeRowEnter = MaterialType_RowEnter;
                ado.chkGroupByMap_CheckChanged = chkGroupByMap_CheckChanged;
                ado.MaterialTypeColumns = new List<HIS.UC.MaterialType.MaterialTypeColumn>();

                if (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO__Create)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_GROUP_BY_MAP)
                        {
                            ado.IsCheckGroupByMap = item.VALUE == "1";
                        }
                    }
                }
                else if (this.currentControlStateRDO__Update != null && this.currentControlStateRDO__Update.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO__Update)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_GROUP_BY_MAP)
                        {
                            ado.IsCheckGroupByMap = item.VALUE == "1";
                        }
                    }
                }

                ado.KeyFieldName = "KeyField";
                ado.ParentFieldName = "ParentField";

                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__KEY_WORD_NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang);

                //Column MaterialTypeCode
                HIS.UC.MaterialType.MaterialTypeColumn GcMaterialTypeCode = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MATERIAL__GC_MATERIAL_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "MATERIAL_TYPE_CODE", 80, false);
                GcMaterialTypeCode.VisibleIndex = 0;
                ado.MaterialTypeColumns.Add(GcMaterialTypeCode);

                //Column MaterialTypeName
                HIS.UC.MaterialType.MaterialTypeColumn GcMaterialTypeName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MATERIAL__GC_MATERIAL_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "MATERIAL_TYPE_NAME", 150, false);
                GcMaterialTypeName.VisibleIndex = 1;
                ado.MaterialTypeColumns.Add(GcMaterialTypeName);

                //Column ServiceUnitName
                HIS.UC.MaterialType.MaterialTypeColumn GcServiceUnitName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "SERVICE_UNIT_NAME_STR", 80, false);
                GcServiceUnitName.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                GcServiceUnitName.VisibleIndex = 2;
                ado.MaterialTypeColumns.Add(GcServiceUnitName);

                //Column ManufacturerName
                HIS.UC.MaterialType.MaterialTypeColumn GcManufacturerName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 3;
                ado.MaterialTypeColumns.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.MaterialType.MaterialTypeColumn GcNationalName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 4;
                ado.MaterialTypeColumns.Add(GcNationalName);

                ado.MaterialType_CustomUnboundColumnData = MaterialType_CustomUnboundColumnData;

                this.ucMaterialType = (UserControl)materialTypeProcessor.Run(ado);
                if (this.ucMaterialType != null)
                {
                    this.pnlVatTu.Controls.Add(this.ucMaterialType);
                    this.ucMaterialType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkGroupByMap_CheckChanged(CheckState checkState)
        {
            try
            {
                if (this.isInit) return;
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAdd = (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0) ? this.currentControlStateRDO__Create.Where(o => o.KEY == ControlStateConstant.CHECK_GROUP_BY_MAP && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_CREATE).FirstOrDefault() : null;

                HIS.Desktop.Library.CacheClient.ControlStateRDO csUpdate = (this.currentControlStateRDO__Update != null && this.currentControlStateRDO__Update.Count > 0) ? this.currentControlStateRDO__Update.Where(o => o.KEY == ControlStateConstant.CHECK_GROUP_BY_MAP && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_UPDATE).FirstOrDefault() : null;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAdd), csAdd));
                if (csAdd != null)
                {
                    csAdd.VALUE = ((checkState == CheckState.Checked) ? "1" : "");
                }
                else
                {
                    csAdd = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAdd.KEY = ControlStateConstant.CHECK_GROUP_BY_MAP;
                    csAdd.VALUE = ((checkState == CheckState.Checked) ? "1" : "");
                    csAdd.MODULE_LINK = ControlStateConstant.MODULE_LINK_CREATE;
                    if (this.currentControlStateRDO__Create == null)
                        this.currentControlStateRDO__Create = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Create.Add(csAdd);

                }

                if (csUpdate != null)
                {
                    csUpdate.VALUE = ((checkState == CheckState.Checked) ? "1" : "");
                }
                else
                {
                    csUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csUpdate.KEY = ControlStateConstant.CHECK_GROUP_BY_MAP;
                    csUpdate.VALUE = ((checkState == CheckState.Checked) ? "1" : "");
                    csUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK_UPDATE;
                    if (this.currentControlStateRDO__Update == null)
                        this.currentControlStateRDO__Update = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Update.Add(csUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO__Create);
                this.controlStateWorker.SetData(this.currentControlStateRDO__Update);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "SERVICE_UNIT_NAME_STR")
                {
                    V_HIS_MATERIAL_TYPE currentRow = e.Row as V_HIS_MATERIAL_TYPE;
                    if (currentRow == null) return;
                    if (currentRow.IMP_UNIT_ID.HasValue)
                        e.Value = currentRow.IMP_UNIT_NAME;
                    else
                        e.Value = currentRow.SERVICE_UNIT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialType_Click(UC.MaterialType.ADO.MaterialTypeADO data)
        {
            try
            {
                //ValidBidPackage(true);
                this.materialType = new ADO.MaterialTypeADO();
                if (data != null)
                {
                    ADO.MaterialTypeADO material = new ADO.MaterialTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MaterialTypeADO>(material, data);

                    this.ActionType = GlobalVariables.ActionAdd;
                    VisibleButton(this.ActionType);
                    this.materialType = material;
                    this.materialType.Type = Base.GlobalConfig.VATTU;
                    spinImpMoreRatio.Enabled = true;
                    SetValueForAdd();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialType_RowEnter(UC.MaterialType.ADO.MaterialTypeADO data)
        {
            try
            {
                MaterialType_Click(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitializeBloodType()
        {
            try
            {
                bloodTypeProcessor = new UC.BloodType.BloodTypeProcessor();
                HIS.UC.BloodType.BloodTypeInitADO ado = new UC.BloodType.BloodTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.BloodTypeClick = BloodType_Click;
                ado.BloodTypeRowEnter = BloodType_RowEnter;
                ado.BloodTypeColumns = new List<UC.BloodType.BloodTypeColumn>();

                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__KEY_WORD_NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang);

                //Column BloodTypeCode
                HIS.UC.BloodType.BloodTypeColumn GcBloodTypeCode = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_BLOOD__GC_BLOOD_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "BLOOD_TYPE_CODE", 80, false);
                GcBloodTypeCode.VisibleIndex = 0;
                ado.BloodTypeColumns.Add(GcBloodTypeCode);

                //Column BloodTypeName
                HIS.UC.BloodType.BloodTypeColumn GcBloodTypeName = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_BLOOD__GC_BLOOD_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "BLOOD_TYPE_NAME", 150, false);
                GcBloodTypeName.VisibleIndex = 1;
                ado.BloodTypeColumns.Add(GcBloodTypeName);

                //Column ServiceUnitName
                HIS.UC.BloodType.BloodTypeColumn GcServiceUnitName = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "SERVICE_UNIT_NAME", 80, false);
                GcServiceUnitName.VisibleIndex = 2;
                ado.BloodTypeColumns.Add(GcServiceUnitName);

                //column volume
                HIS.UC.BloodType.BloodTypeColumn GcVolume = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_BLOOD_GC_VOLUME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "VOLUME", 60, false);
                GcVolume.VisibleIndex = 3;
                ado.BloodTypeColumns.Add(GcVolume);

                //Column ManufacturerName
                HIS.UC.BloodType.BloodTypeColumn GcManufacturerName = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 4;
                ado.BloodTypeColumns.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.BloodType.BloodTypeColumn GcNationalName = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    cultureLang), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 5;
                ado.BloodTypeColumns.Add(GcNationalName);

                this.ucBloodType = (UserControl)bloodTypeProcessor.Run(ado);
                if (this.ucBloodType != null)
                {
                    this.pnlMau.Controls.Add(this.ucBloodType);
                    this.ucBloodType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BloodType_Click(UC.BloodType.ADO.BloodTypeADO data)
        {
            try
            {
                //ValidBidPackage(false);
                this.bloodType = new ADO.BloodTypeADO();
                if (data != null)
                {
                    ADO.BloodTypeADO blood = new ADO.BloodTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.BloodTypeADO>(blood, data);

                    this.ActionType = GlobalVariables.ActionAdd;
                    VisibleButton(this.ActionType);
                    this.bloodType = blood;
                    this.bloodType.Type = Base.GlobalConfig.MAU;
                    spinImpMoreRatio.Enabled = false;
                    SetValueForAdd();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BloodType_RowEnter(UC.BloodType.ADO.BloodTypeADO data)
        {
            try
            {
                BloodType_Click(data);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BidUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.BidUpdate.frmBidUpdate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSupplier.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBidUpdate.cboSupplier.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnDiscard.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.BtnDiscard.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUpdate.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.btnUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtBidName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmBidUpdate.txtBidName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBidType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBidUpdate.cboBidType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBidName.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.lciBidName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBidNumber.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.lciBidNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem2.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.emptySpaceItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBidType.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.lciBidType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFromTime.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.lciBidFromTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciToTime.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.lciBidToTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmBidUpdate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.txtBidGroupCode.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.txtBidGroupCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtBidPackageCode.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.txtBidPackageCode.Text ", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnImport.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.BtnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.LciExpiredDate.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.LciExpiredDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciRegisterNumber.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.lciRegisterNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConcentra.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.lciConcentra.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNational.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.lciNational.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciManufacturer.Text = Inventec.Common.Resource.Get.Value("frmBidUpdate.lciManufacturer.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmBidUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;
                checkLoad = true;
                HisConfigCFG.LoadConfig();
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                //txtBidNumOrder.EditValue = null;
                InitControlState();
                InitGridEdit();
                SetDefaultValueControl();
                LoadCurrenBid();
                //Dangth
                LoaddatatoCboDosageForm();
                TaskAll();
                //GetData();
                //LoadDataToCboSupplier();
                //LoadDataToCboBidType();
                //LoadDataToCboNational();
                //LoadDataToCboManufacture();
                //LoadDataToCboMediUseForm();
                //FillDataRight(this.currenthisBid);
                CreateThreadLoadCurrentData();
                LoadMediStock();
                //FillDataToGrid();
                ValidControlLeft();
                ValidControlRight();
                RegisterTimer(this.Module.ModuleLink, timer1, 500, timer1_Tick);
                StartTimer(this.Module.ModuleLink, timer1);
                Inventec.Common.Logging.LogAction.Info(this.Module.ModuleLink + ": [StartTimer - Load data tab loai thuoc]");
                this.isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoaddatatoCboDosageForm()
        {
            try
            {
                List<HIS_DOSAGE_FORM> data = BackendDataWorker.Get<HIS_DOSAGE_FORM>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOSAGE_FORM_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("DOSAGE_FORM_NAME", "Tên", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOSAGE_FORM_NAME", "ID", columnInfos, false, 350);
                //Load data vao combobox
                ControlEditorLoader.Load(cboDosageForm, data, controlEditorADO);
                cboDosageForm.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMediStock()
        {
            try
            {
                if (this.currentStock != null && this.currentStock.IS_BUSINESS == (short)1)
                {
                    chkIsOnlyShowByBusiness.Text = Resources.ResourceMessage.ChiHienThiThuocVatTuKD;
                    chkIsOnlyShowByBusiness.ToolTip = Resources.ResourceMessage.ChiHienThiThuocVatTuKinhDoanh;
                    if (this.hasNotBusiness)
                    {
                        chkIsOnlyShowByBusiness.Checked = false;
                    }
                    else
                    {
                        chkIsOnlyShowByBusiness.Checked = true;
                    }
                }
                else
                {
                    chkIsOnlyShowByBusiness.Text = Resources.ResourceMessage.ChiHienThiThuocVatTuKhongKD;
                    chkIsOnlyShowByBusiness.ToolTip = Resources.ResourceMessage.ChiHienThiThuocVatTuKhongKinhDoanh;
                    if (this.hasBusiness)
                    {
                        chkIsOnlyShowByBusiness.Checked = false;
                    }
                    else
                    {
                        chkIsOnlyShowByBusiness.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitGridEdit()
        {
            try
            {
                ADO.BidEditADO medicine = new ADO.BidEditADO();
                medicine.grid_Click = Grid_Click;
                medicine.delete_ButtonClick = Delete_ButtonClick;
                medicine.TYPE = GlobalConfig.THUOC;

                ucMedicine = new UC_LoadEdit(medicine, Module, bid_id);
                ucMedicine.Dock = DockStyle.Fill;

                ADO.BidEditADO material = new ADO.BidEditADO();
                material.grid_Click = Grid_Click;
                material.delete_ButtonClick = Delete_ButtonClick;
                material.TYPE = GlobalConfig.VATTU;

                ucMaterial = new UC_LoadEdit(material, Module, bid_id);
                ucMaterial.Dock = DockStyle.Fill;

                ADO.BidEditADO blood = new ADO.BidEditADO();
                blood.grid_Click = Grid_Click;
                blood.delete_ButtonClick = Delete_ButtonClick;
                blood.TYPE = GlobalConfig.MAU;

                ucBlood = new UC_LoadEdit(blood, Module, bid_id);
                ucBlood.Dock = DockStyle.Fill;

                this.panelControl1.Controls.Add(ucMedicine);
                this.panelControl2.Controls.Add(ucMaterial);
                this.panelControl3.Controls.Add(ucBlood);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitControlState()
        {
            try
            {
                if (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO__Create)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD)
                        {
                            chkClearToAdd.Checked = item.VALUE == "1";
                        }
                    }
                }
                else if (currentControlStateRDO__Update != null && currentControlStateRDO__Update.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO__Update)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD)
                        {
                            chkClearToAdd.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Delete_ButtonClick(ADO.MedicineTypeADO data)
        {
            try
            {
                if (data.AllowDelete)
                {
                    idRow = data.IdRow;
                    foreach (var item in this.ListMedicineTypeAdoProcess)
                    {
                        if (idRow == item.IdRow)
                        {
                            this.ListMedicineTypeAdoProcess.RemoveAll(o => o.IdRow == idRow);
                            idRow = -1;
                            break;
                        }
                    }
                    UpdateGrid();
                    SetDefaultValueControlLeft();
                    ResetLeftControl();
                }
                else
                {
                    string errorMessage = "";
                    if (data.Type == Base.GlobalConfig.THUOC)
                    {
                        errorMessage = String.Format(Resources.ResourceMessage.CanhBaoThuoc, data.MEDICINE_TYPE_NAME);
                    }
                    else if (data.Type == Base.GlobalConfig.VATTU)
                    {
                        errorMessage = String.Format(Resources.ResourceMessage.CanhBaoVatTu, data.MEDICINE_TYPE_NAME);
                    }
                    else if (data.Type == Base.GlobalConfig.MAU)
                    {
                        errorMessage = String.Format(Resources.ResourceMessage.CanhBaoMau, data.MEDICINE_TYPE_NAME);
                    }
                    errorMessage += Resources.ResourceMessage.ThuocVatTuDaCoYeuCauNhapKhongChoPhepXoa;
                    XtraMessageBox.Show(errorMessage);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void Grid_Click(ADO.MedicineTypeADO data)
        {
            try
            {

                ResetLeftControl();
                this.ActionType = GlobalVariables.ActionEdit;
                VisibleButton(this.ActionType);
                this.medicineType = data;
                if (data.Type == Base.GlobalConfig.THUOC)
                {
                    var dosage = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DOSAGE_FORM>().Where(o => o.DOSAGE_FORM_NAME == this.medicineType.DOSAGE_FORM).ToList();

                    ValidCboDosageForm();
                    //ValidBidPackage(false);
                    xtraTabControl1.SelectedTabPageIndex = 0;
                    this.medicineType = data;
                    //this.medicineType.SERVICE_UNIT_NAME = data.IMP_UNIT_ID.HasValue ? data.IMP_UNIT_NAME : data.SERVICE_UNIT_NAME;

                    txtTenBHYT.Text = this.medicineType.HEIN_SERVICE_BHYT_NAME;
                    txtPackingType.Text = this.medicineType.PACKING_TYPE_NAME;
                    txtActiveBhyt.Text = this.medicineType.ACTIVE_INGR_BHYT_NAME;
                    cboMediUseForm.EditValue = this.medicineType.MEDICINE_USE_FORM_ID;
                    if (dosage != null && dosage.Count() > 0)
                    {
                        cboDosageForm.EditValue = dosage[0].ID;
                    }
                    else
                    {
                        cboDosageForm.EditValue = null;
                    }
                    //cboDosageForm.EditValue = this.medicineType.DOSAGE_FORM;
                    txtNOTE.Text = this.medicineType.NOTE;

                    cboDosageForm.Enabled = true;
                    EnableLeftControl(true);
                    txtMaTT.Enabled = false;
                    txtMaDT.Enabled = false;
                    txtTenTT.Enabled = false;
                    ValidMaxlengthDosageForm();
                    lciDosageForm.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    cboInformationBid.SelectedIndex = -1;
                    lciTenBHYT.Size = lciMaTT.Size;
                }
                else if (data.Type == Base.GlobalConfig.VATTU)
                {
                    dxValidationProviderLeft.SetValidationRule(cboDosageForm, null);
                    //ValidBidPackage(true);
                    xtraTabControl1.SelectedTabPageIndex = 1;
                    this.materialType = new ADO.MaterialTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MaterialTypeADO>(this.materialType, data);
                    this.materialType.MATERIAL_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                    this.materialType.MATERIAL_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                    this.materialType.SERVICE_UNIT_NAME = data.IMP_UNIT_ID.HasValue ? data.IMP_UNIT_NAME : data.SERVICE_UNIT_NAME;

                    txtTenTT.Text = this.materialType.BID_MATERIAL_TYPE_NAME ?? "";
                    txtMaTT.Text = this.materialType.BID_MATERIAL_TYPE_CODE ?? "";
                    txtMaDT.Text = this.medicineType.JOIN_BID_MATERIAL_TYPE_CODE;
                    txtNOTE.Text = this.materialType.NOTE;
                    txtTenBHYT.Text = this.materialType.HEIN_SERVICE_BHYT_NAME;
                    txtPackingType.Text = this.materialType.PACKING_TYPE_NAME;
                    EnableLeftControl(true);
                    txtRegisterNumber.Enabled = false;
                    txtPackingType.Enabled = false;
                    txtTenBHYT.Enabled = false;
                    cboDosageForm.Enabled = false;
                    txtActiveBhyt.Enabled = false;
                    cboMediUseForm.Enabled = false;
                    layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem26.Size = lciMaDT.Size;
                    lciTenBHYT.Size = lciMaTT.Size;
                    cboInformationBid.SelectedIndex = (int)(data.INFORMATION_BID ?? 0) - 1;
                }
                else if (data.Type == Base.GlobalConfig.MAU)
                {

                    dxValidationProviderLeft.SetValidationRule(cboDosageForm, null);
                    //ValidBidPackage(false);
                    xtraTabControl1.SelectedTabPageIndex = 2;
                    this.bloodType = new ADO.BloodTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.BloodTypeADO>(this.bloodType, data);
                    this.bloodType.BLOOD_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                    this.bloodType.BLOOD_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                    this.bloodType.ADJUST_AMOUNT = null;
                    EnableLeftControl(false);
                    layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    cboDosageForm.Enabled = false;
                    cboInformationBid.SelectedIndex = -1;
                    lciTenBHYT.Size = lciMaTT.Size;
                }
                var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.ID == data.SUPPLIER_ID);
                if (supplier != null)
                {
                    txtSupplierCode.Text = supplier.SUPPLIER_CODE;
                    cboSupplier.EditValue = supplier.ID;
                    cboSupplier.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    txtSupplierCode.Text = "";
                    cboSupplier.EditValue = null;
                    cboSupplier.Properties.Buttons[1].Visible = false;
                }
                spinAmount.EditValue = data.AMOUNT;
                spinImpPrice.EditValue = data.IMP_PRICE;
                spinImpVat.EditValue = data.IMP_VAT_RATIO * 100;
                spinImpMoreRatio.EditValue = data.ImpMoreRatio;
                txtBidNumOrder.Text = data.BID_NUM_ORDER;
                if (!string.IsNullOrEmpty(data.BID_GROUP_CODE))
                {
                    txtBidGroupCode.Text = data.BID_GROUP_CODE;
                }
                txtBidPackageCode.Text = data.BID_PACKAGE_CODE;

                var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_NAME == data.NATIONAL_NAME).ToList();
                txtRegisterNumber.Text = data.REGISTER_NUMBER;
                if (national != null && national.Count() > 0)
                {
                    txtNationalMainText.Text = national[0].NATIONAL_NAME;
                    cboNational.EditValue = national[0].ID;
                    txtNationalMainText.Visible = false;
                    cboNational.Visible = true;
                    chkEditNational.CheckState = CheckState.Unchecked;
                }
                else
                {
                    txtNationalMainText.Text = data.NATIONAL_NAME;
                    cboNational.EditValue = null;
                    txtNationalMainText.Visible = true;
                    cboNational.Visible = false;
                    chkEditNational.CheckState = CheckState.Checked;
                }
                cboManufacture.EditValue = data.MANUFACTURER_ID;
                txtManufacture.Text = data.MANUFACTURER_CODE;
                txtConcentra.Text = data.CONCENTRA;
                txtRegisterNumber.Text = data.REGISTER_NUMBER;

                if (data.EXPIRED_DATE.HasValue)
                {
                    DtExpiredDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXPIRED_DATE.Value);
                }
                if (data.DAY_LIFESPAN.HasValue)
                {
                    spinDayLifeSpan.EditValue = data.DAY_LIFESPAN.Value;
                }
                else
                {
                    spinDayLifeSpan.EditValue = null;
                }
                if (data.MONTH_LIFESPAN.HasValue)
                {
                    spinMonthLifeSpan.EditValue = data.MONTH_LIFESPAN.Value;
                }
                else
                {
                    spinMonthLifeSpan.EditValue = null;
                }
                if (data.HOUR_LIFESPAN.HasValue)
                {
                    spinHourLifeSpan.EditValue = data.HOUR_LIFESPAN.Value;
                }
                else
                {
                    spinHourLifeSpan.EditValue = null;
                }

                spinAmount.Focus();
                spinAmount.SelectAll();
                dxValidationProviderLeft.RemoveControlError(spinImpPrice);
                dxValidationProviderLeft.RemoveControlError(spinAmount);
                dxValidationProviderLeft.RemoveControlError(spinImpVat);
                dxValidationProviderLeft.RemoveControlError(txtBidNumOrder);
                dxValidationProviderLeft.RemoveControlError(cboSupplier);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataToCboNational()
        {
            try
            {
                cboNational.Properties.DataSource = lstNational;
                cboNational.Properties.DisplayMember = "NATIONAL_NAME";
                cboNational.Properties.ValueMember = "ID";
                cboNational.Properties.TextEditStyle = TextEditStyles.Standard;
                cboNational.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboNational.Properties.ImmediatePopup = true;
                cboNational.ForceInitialize();
                cboNational.Properties.View.Columns.Clear();
                cboNational.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = cboNational.Properties.View.Columns.AddField("NATIONAL_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                GridColumn aColumnName = cboNational.Properties.View.Columns.AddField("NATIONAL_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboManufacture()
        {
            try
            {
                cboManufacture.Properties.DataSource = lstManufacture;
                cboManufacture.Properties.DisplayMember = "MANUFACTURER_NAME";
                cboManufacture.Properties.ValueMember = "ID";
                cboManufacture.Properties.TextEditStyle = TextEditStyles.Standard;
                cboManufacture.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboManufacture.Properties.ImmediatePopup = true;
                cboManufacture.ForceInitialize();
                cboManufacture.Properties.View.Columns.Clear();
                cboManufacture.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = cboManufacture.Properties.View.Columns.AddField("MANUFACTURER_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                GridColumn aColumnName = cboManufacture.Properties.View.Columns.AddField("MANUFACTURER_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboMediUseForm()
        {
            try
            {
                cboMediUseForm.Properties.DataSource = lstMediUseForm;
                cboMediUseForm.Properties.DisplayMember = "MEDICINE_USE_FORM_NAME";
                cboMediUseForm.Properties.ValueMember = "ID";
                cboMediUseForm.Properties.TextEditStyle = TextEditStyles.Standard;
                cboMediUseForm.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboMediUseForm.Properties.ImmediatePopup = true;
                cboMediUseForm.ForceInitialize();
                cboMediUseForm.Properties.View.Columns.Clear();
                cboMediUseForm.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = cboMediUseForm.Properties.View.Columns.AddField("MEDICINE_USE_FORM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 70;

                GridColumn aColumnName = cboMediUseForm.Properties.View.Columns.AddField("MEDICINE_USE_FORM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDataToCboBidForm()
        {
            try
            {
                cboBidForm.Properties.DataSource = lstBidForm;
                cboBidForm.Properties.DisplayMember = "NAME";
                cboBidForm.Properties.ValueMember = "ID";
                cboBidForm.Properties.TextEditStyle = TextEditStyles.Standard;
                cboBidForm.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboBidForm.Properties.ImmediatePopup = true;
                cboBidForm.ForceInitialize();
                cboBidForm.Properties.View.Columns.Clear();
                cboBidForm.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = cboBidForm.Properties.View.Columns.AddField("CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                GridColumn aColumnName = cboBidForm.Properties.View.Columns.AddField("NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboSupplier()
        {
            try
            {
                cboSupplier.Properties.DataSource = lstSupplier;
                cboSupplier.Properties.DisplayMember = "SUPPLIER_NAME";
                cboSupplier.Properties.ValueMember = "ID";
                cboSupplier.Properties.TextEditStyle = TextEditStyles.Standard;
                cboSupplier.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboSupplier.Properties.ImmediatePopup = true;
                cboSupplier.ForceInitialize();
                cboSupplier.Properties.View.Columns.Clear();
                cboSupplier.Properties.PopupFormSize = new Size(600, 300);

                GridColumn aColumnCode = cboSupplier.Properties.View.Columns.AddField("SUPPLIER_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                GridColumn aColumnName = cboSupplier.Properties.View.Columns.AddField("SUPPLIER_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboBidType()
        {
            try
            {
                cboBidType.Properties.DataSource = bidTypes;
                cboBidType.Properties.DisplayMember = "BID_TYPE_NAME";
                cboBidType.Properties.ValueMember = "ID";
                cboBidType.Properties.TextEditStyle = TextEditStyles.Standard;
                cboBidType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboBidType.Properties.ImmediatePopup = true;
                cboBidType.ForceInitialize();
                cboBidType.Properties.View.Columns.Clear();
                //cboBidType.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = cboBidType.Properties.View.Columns.AddField("BID_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                GridColumn aColumnName = cboBidType.Properties.View.Columns.AddField("BID_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
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
                xtraTabControl1.SelectedTabPageIndex = 0;
                txtSupplierCode.Text = "";
                cboSupplier.EditValue = null;
                txtBidName.Text = "";
                txtBID.Text = "";
                cboBidForm.EditValue = null;
                txtBidNumber.Text = "";
                txtBidYear.Text = "";
                txtBidApthauCode.Text = "";
                dtFromTime.EditValue = null;
                dtToTime.EditValue = null;
                txtNationalMainText.Visible = false;
                cboNational.Visible = true;
                chkEditNational.CheckState = CheckState.Unchecked;
                cboBidType.EditValue = (long)1;
                ListMedicineTypeAdoProcess = new List<ADO.MedicineTypeADO>();
                this.ActionType = GlobalVariables.ActionAdd;
                ucBlood.Reload(null);
                ucMaterial.Reload(null);
                ucMedicine.Reload(null);
                cboSupplier.Properties.Buttons[1].Visible = false;
                xtraTabControl1.SelectedTabPageIndex = 0;
                dxValidationProviderRight.RemoveControlError(txtBidName);
                dxValidationProviderRight.RemoveControlError(txtBidNumber);
                dxValidationProviderRight.RemoveControlError(txtBID);
                SetDefaultValueControlLeft();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControlLeft()
        {
            try
            {
                spinAmount.Value = 0;
                spinImpVat.Value = 0;
                spinImpMoreRatio.Value = 0;
                spinImpPrice.Value = 0;
                txtBidNumOrder.Text = "";
                txtBidGroupCode.Text = "";
                txtBidPackageCode.Text = "";
                txtSupplierCode.Text = "";
                cboSupplier.Text = "";
                cboSupplier.EditValue = null;
                cboNational.Visible = true;
                txtNationalMainText.Visible = false;
                chkEditNational.CheckState = CheckState.Unchecked;
                cboSupplier.Properties.Buttons[1].Visible = false;
                spinMonthLifeSpan.EditValue = null;
                spinDayLifeSpan.EditValue = null;
                spinHourLifeSpan.EditValue = null;

                dxValidationProviderLeft.RemoveControlError(spinImpPrice);
                dxValidationProviderLeft.RemoveControlError(spinAmount);
                dxValidationProviderLeft.RemoveControlError(spinImpVat);
                dxValidationProviderLeft.RemoveControlError(txtBidNumOrder);
                dxValidationProviderLeft.RemoveControlError(cboSupplier);
                //trang thai nut
                EnableButton(this.ActionType);
                VisibleButton(this.ActionType);
                ResetLeftControl();
                //FocusTab();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataRight(HIS_BID hisBid)
        {
            if (currenthisBid != null)
            {
                cboBidForm.EditValue = hisBid.BID_FORM_ID;
                txtBidName.Text = hisBid.BID_NAME;
                txtBID.Text = hisBid.BID_EXTRA_CODE;
                txtBidNumber.Text = hisBid.BID_NUMBER;
                cboBidType.EditValue = hisBid.BID_TYPE_ID;
                txtBidYear.Text = hisBid.BID_YEAR;
                txtBidApthauCode.Text = hisBid.BID_APTHAU_CODE;
                if (hisBid.VALID_FROM_TIME.HasValue)
                {
                    dtFromTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisBid.VALID_FROM_TIME ?? 0) ?? DateTime.Now;
                }
                else
                {
                    dtFromTime.EditValue = null;
                }

                if (hisBid.VALID_TO_TIME.HasValue)
                {
                    dtToTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisBid.VALID_TO_TIME ?? 0) ?? DateTime.Now;
                }
                else
                {
                    dtToTime.EditValue = null;
                }
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (chkIsOnlyShowByBusiness.Checked)
                {
                    if (this.currentStock != null && this.currentStock.IS_BUSINESS == (short)1)
                    {
                        listHisMedicineType = listHisMedicineType != null ? listHisMedicineType.Where(o => o.IS_BUSINESS == (short)1).ToList() : null;
                    }
                    else
                    {
                        listHisMedicineType = listHisMedicineType != null ? listHisMedicineType.Where(o => o.IS_BUSINESS != (short)1).ToList() : null;
                    }
                }
                this.medicineTypeProcessor.Reload(this.ucMedicineType, listHisMedicineType);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadCurrentData()
        {
            Thread ThreadMedicine = new Thread(FillMedicine);
            Thread ThreadMaterial = new Thread(FillMaterialType);
            Thread ThreadBlood = new Thread(FillBloodType);
            try
            {
                ThreadMedicine.Start();
                ThreadMaterial.Start();
                ThreadBlood.Start();

                ThreadMedicine.Join();
                ThreadMaterial.Join();
                ThreadBlood.Join();
                UpdateGrid();
            }
            catch (Exception ex)
            {
                ThreadMedicine.Abort();
                ThreadMaterial.Abort();
                ThreadBlood.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillMedicine()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBidMedicineTypeViewFilter filter = new HisBidMedicineTypeViewFilter();
                filter.BID_ID = bid_id;
                var medicineType = new BackendAdapter(param)
                    .Get<List<V_HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).ToList();
                if (medicineType != null && medicineType.Count > 0)
                {
                    List<V_HIS_IMP_MEST_MEDICINE> impMests = new List<V_HIS_IMP_MEST_MEDICINE>();
                    int skip = 0;
                    var medicineTypeIds = medicineType.Select(s => s.MEDICINE_TYPE_ID).ToList();
                    while (medicineTypeIds.Count - skip > 0)
                    {
                        var listIds = medicineTypeIds.Skip(skip).Take(100).ToList();
                        skip += 100;
                        HisImpMestMedicineViewFilter impFilter = new HisImpMestMedicineViewFilter();
                        impFilter.CREATE_TIME_FROM = currenthisBid.CREATE_TIME;
                        impFilter.CREATE_TIME_TO = Inventec.Common.DateTime.Get.Now();
                        impFilter.MEDICINE_TYPE_IDs = listIds;
                        var impMestsTemp = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumers.MosConsumer, impFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (impMestsTemp != null && impMestsTemp.Count > 0)
                        {
                            impMests.AddRange(impMestsTemp);
                        }
                    }
                    Dictionary<long, List<V_HIS_EXP_MEST_MEDICINE>> dicExpMest = new Dictionary<long, List<V_HIS_EXP_MEST_MEDICINE>>();
                    ProcessDataManuExpMest(bid_id, medicineType.Select(o => o.SUPPLIER_ID).Distinct().ToList(), ref dicExpMest);

                    foreach (var item in medicineType)
                    {
                        List<V_HIS_IMP_MEST_MEDICINE> impMedicines = impMests != null && impMests.Count > 0 ? (impMests.Where(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID && o.SUPPLIER_ID == item.SUPPLIER_ID && o.BID_ID == bid_id && o.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC).ToList()) : null;

                        List<V_HIS_EXP_MEST_MEDICINE> expMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                        if (dicExpMest.ContainsKey(item.MEDICINE_TYPE_ID))
                        {
                            expMedicine = dicExpMest[item.MEDICINE_TYPE_ID].Where(o => o.SUPPLIER_ID == item.SUPPLIER_ID && o.BID_ID == bid_id).ToList();
                        }

                        ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                        AutoMapper.Mapper.CreateMap<V_HIS_BID_MEDICINE_TYPE, ADO.MedicineTypeADO>();
                        ado = AutoMapper.Mapper.Map<V_HIS_BID_MEDICINE_TYPE, ADO.MedicineTypeADO>(item);
                        ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                        ado.ImpMoreRatio = item.IMP_MORE_RATIO * 100;
                        ado.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                        ado.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                        ado.IdRow = setIdRow(ListMedicineTypeAdoProcess);
                        ado.Type = Base.GlobalConfig.THUOC;
                        ado.ID = item.MEDICINE_TYPE_ID;
                        var typeAdo = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.ID == ado.ID).FirstOrDefault();
                        if (typeAdo != null)
                        {
                            ado.SERVICE_UNIT_NAME = typeAdo.IMP_UNIT_ID.HasValue ? typeAdo.IMP_UNIT_NAME : typeAdo.SERVICE_UNIT_NAME;
                            if (typeAdo.IS_BUSINESS == (short)1)
                                this.hasBusiness = true;
                            else
                                this.hasNotBusiness = true;
                        }
                        ado.BID_MEDI_MATY_BLO_ID = item.ID;
                        ado.AllowDelete = (impMedicines == null || impMedicines.Count <= 0);
                        var impAmount = impMedicines != null && impMedicines.Count > 0 ? impMedicines.Sum(o => o.AMOUNT) : 0;
                        var expAmount = expMedicine != null && expMedicine.Count > 0 ? expMedicine.Sum(o => o.AMOUNT) : 0;
                        ado.Min_AMOUNT = impAmount - expAmount;
                        ado.EXPIRED_DATE = item.EXPIRED_DATE;
                        ListMedicineTypeAdoProcess.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataManuExpMest(long bidId, List<long> listSupplierId, ref Dictionary<long, List<V_HIS_EXP_MEST_MEDICINE>> dicExpMest)
        {
            try
            {
                if (listSupplierId != null && listSupplierId.Count > 0)
                {
                    HisExpMestMedicineViewFilter expFilter = new HisExpMestMedicineViewFilter();
                    expFilter.SUPPLIER_IDs = listSupplierId;
                    expFilter.BID_ID = bidId;
                    expFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC;
                    expFilter.CREATE_TIME_FROM = currenthisBid.CREATE_TIME;
                    expFilter.CREATE_TIME_TO = Inventec.Common.DateTime.Get.Now();
                    var expMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expFilter, null);
                    if (expMests != null && expMests.Count > 0)
                    {
                        foreach (var item in expMests)
                        {
                            if (!dicExpMest.ContainsKey(item.MEDICINE_TYPE_ID))
                                dicExpMest[item.MEDICINE_TYPE_ID] = new List<V_HIS_EXP_MEST_MEDICINE>();
                            dicExpMest[item.MEDICINE_TYPE_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillMaterialType()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBidMaterialTypeViewFilter filter = new HisBidMaterialTypeViewFilter();
                filter.BID_ID = bid_id;

                var materialType = new BackendAdapter(param)
                    .Get<List<V_HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).ToList();
                LogSystem.Debug(LogUtil.TraceData("BidMaterialType", materialType));
                if (materialType != null && materialType.Count > 0)
                {
                    List<V_HIS_IMP_MEST_MATERIAL> impMests = new List<V_HIS_IMP_MEST_MATERIAL>();
                    int skip = 0;
                    var materialTypeIds = materialType.Where(o => o.MATERIAL_TYPE_ID.HasValue).Select(s => s.MATERIAL_TYPE_ID.Value).ToList();
                    while (materialTypeIds.Count - skip > 0)
                    {
                        var listIds = materialTypeIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        HisImpMestMaterialViewFilter impFilter = new HisImpMestMaterialViewFilter();
                        impFilter.CREATE_TIME_FROM = currenthisBid.CREATE_TIME;
                        impFilter.CREATE_TIME_TO = Inventec.Common.DateTime.Get.Now();
                        impFilter.MATERIAL_TYPE_IDs = listIds;
                        var impMestsTemp = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (impMestsTemp != null && impMestsTemp.Count > 0)
                        {
                            impMests.AddRange(impMestsTemp);
                        }
                    }

                    Dictionary<long, List<V_HIS_EXP_MEST_MATERIAL>> dicExpMest = new Dictionary<long, List<V_HIS_EXP_MEST_MATERIAL>>();
                    ProcessDataManuExpMest(bid_id, materialType.Select(o => o.SUPPLIER_ID).Distinct().ToList(), ref dicExpMest);

                    foreach (var item in materialType)
                    {
                        List<V_HIS_IMP_MEST_MATERIAL> impMaterials = impMests != null && impMests.Count > 0 ? (impMests.Where(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID && o.SUPPLIER_ID == item.SUPPLIER_ID && o.BID_ID == bid_id && o.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC).ToList()) : null;
                        List<V_HIS_EXP_MEST_MATERIAL> expMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();
                        if (item.MATERIAL_TYPE_ID.HasValue && dicExpMest.ContainsKey(item.MATERIAL_TYPE_ID.Value))
                        {
                            expMaterial = dicExpMest[item.MATERIAL_TYPE_ID.Value].Where(o => o.SUPPLIER_ID == item.SUPPLIER_ID && o.BID_ID == bid_id).ToList();
                        }

                        ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                        AutoMapper.Mapper.CreateMap<V_HIS_BID_MATERIAL_TYPE, ADO.MedicineTypeADO>();
                        ado = AutoMapper.Mapper.Map<V_HIS_BID_MATERIAL_TYPE, ADO.MedicineTypeADO>(item);
                        ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        ado.INFORMATION_BID = item.INFORMATION_BID;
                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                        ado.ImpMoreRatio = item.IMP_MORE_RATIO * 100;
                        ado.IdRow = setIdRow(ListMedicineTypeAdoProcess);
                        ado.Type = Base.GlobalConfig.VATTU;
                        if (item.MATERIAL_TYPE_ID.HasValue)
                        {
                            ado.ID = item.MATERIAL_TYPE_ID.Value;
                            ado.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                            ado.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        }
                        else
                        {
                            ado.ID = item.MATERIAL_TYPE_MAP_ID.Value;
                            ado.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_MAP_CODE;
                            ado.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_MAP_NAME;
                            ado.IsMaterialTypeMap = true;
                        }
                        ado.BID_MEDI_MATY_BLO_ID = item.ID;
                        ado.BID_ID = item.BID_ID;
                        ado.AllowDelete = (impMaterials == null || impMaterials.Count <= 0);
                        var impAmount = impMaterials != null && impMaterials.Count > 0 ? impMaterials.Sum(o => o.AMOUNT) : 0;
                        var expAmount = expMaterial != null && expMaterial.Count > 0 ? expMaterial.Sum(o => o.AMOUNT) : 0;
                        ado.Min_AMOUNT = impAmount - expAmount;
                        ado.EXPIRED_DATE = item.EXPIRED_DATE;
                        if (item.MATERIAL_TYPE_ID.HasValue)
                        {
                            var typeAdo = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.ID == ado.ID).FirstOrDefault();
                            if (typeAdo != null)
                            {
                                if (typeAdo.IS_BUSINESS == (short)1)
                                    this.hasBusiness = true;
                                else
                                    this.hasNotBusiness = true;
                            }
                        }
                        ListMedicineTypeAdoProcess.Add(ado);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListMedicineTypeAdoProcess), ListMedicineTypeAdoProcess));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataManuExpMest(long bidId, List<long> listSupplierId, ref Dictionary<long, List<V_HIS_EXP_MEST_MATERIAL>> dicExpMest)
        {
            try
            {
                if (listSupplierId != null && listSupplierId.Count > 0)
                {
                    HisExpMestMaterialViewFilter expFilter = new HisExpMestMaterialViewFilter();
                    expFilter.SUPPLIER_IDs = listSupplierId;
                    expFilter.BID_ID = bidId;
                    expFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC;
                    expFilter.CREATE_TIME_FROM = currenthisBid.CREATE_TIME;
                    expFilter.CREATE_TIME_TO = Inventec.Common.DateTime.Get.Now();
                    var expMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expFilter, null);
                    if (expMests != null && expMests.Count > 0)
                    {
                        foreach (var item in expMests)
                        {
                            if (!dicExpMest.ContainsKey(item.MATERIAL_TYPE_ID))
                                dicExpMest[item.MATERIAL_TYPE_ID] = new List<V_HIS_EXP_MEST_MATERIAL>();
                            dicExpMest[item.MATERIAL_TYPE_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillBloodType()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBidBloodTypeFilter filter = new HisBidBloodTypeFilter();
                filter.BID_ID = bid_id;
                var bloodType = new BackendAdapter(param)
                    .Get<List<V_HIS_BID_BLOOD_TYPE>>("api/HisBidBloodType/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).ToList();
                if (bloodType != null && bloodType.Count > 0)
                {
                    HisImpMestBloodViewFilter impFilter = new HisImpMestBloodViewFilter();
                    impFilter.CREATE_TIME_FROM = currenthisBid.CREATE_TIME;
                    impFilter.CREATE_TIME_TO = Inventec.Common.DateTime.Get.Now();
                    //impFilter.BLOOD_TYPE_IDs = bloodType.Select(s => s.BLOOD_TYPE_ID).ToList();
                    var impMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, impFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    Dictionary<long, List<V_HIS_EXP_MEST_BLOOD>> dicExpMest = new Dictionary<long, List<V_HIS_EXP_MEST_BLOOD>>();
                    ProcessDataManuExpMest(bid_id, bloodType.Select(o => o.SUPPLIER_ID).Distinct().ToList(), ref dicExpMest);

                    foreach (var item in bloodType)
                    {
                        List<V_HIS_IMP_MEST_BLOOD> impBloods = impMests != null && impMests.Count > 0 ? (impMests.Where(o => o.BLOOD_TYPE_ID == item.BLOOD_TYPE_ID && o.SUPPLIER_ID == item.SUPPLIER_ID && o.BID_ID == bid_id && o.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC).ToList()) : null;
                        List<V_HIS_EXP_MEST_BLOOD> expBloods = new List<V_HIS_EXP_MEST_BLOOD>();
                        if (dicExpMest.ContainsKey(item.BLOOD_TYPE_ID))
                        {
                            expBloods = dicExpMest[item.BLOOD_TYPE_ID].Where(o => o.SUPPLIER_ID == item.SUPPLIER_ID && o.BID_ID == bid_id).ToList();
                        }

                        ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                        AutoMapper.Mapper.CreateMap<V_HIS_BID_BLOOD_TYPE, ADO.MedicineTypeADO>();
                        ado = AutoMapper.Mapper.Map<V_HIS_BID_BLOOD_TYPE, ADO.MedicineTypeADO>(item);
                        ado.AMOUNT = item.VOLUME;
                        ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                        ado.MEDICINE_TYPE_CODE = item.BLOOD_TYPE_CODE;
                        ado.MEDICINE_TYPE_NAME = item.BLOOD_TYPE_NAME;
                        ado.ADJUST_AMOUNT = null;
                        ado.IdRow = setIdRow(ListMedicineTypeAdoProcess);
                        ado.Type = Base.GlobalConfig.MAU;
                        ado.ID = item.BLOOD_TYPE_ID;
                        ado.BID_MEDI_MATY_BLO_ID = item.ID;
                        ado.BID_ID = item.BID_ID;
                        ado.AllowDelete = (impBloods == null || impBloods.Count <= 0);
                        var impAmount = impBloods != null && impBloods.Count > 0 ? impBloods.Sum(o => o.VOLUME) : 0;
                        var expAmount = expBloods != null && expBloods.Count > 0 ? expBloods.Sum(o => o.VOLUME) : 0;
                        ado.Min_AMOUNT = impAmount - expAmount;
                        ListMedicineTypeAdoProcess.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataManuExpMest(long bidId, List<long> listSupplierId, ref Dictionary<long, List<V_HIS_EXP_MEST_BLOOD>> dicExpMest)
        {
            try
            {
                if (listSupplierId != null && listSupplierId.Count > 0)
                {
                    HisExpMestBloodViewFilter expFilter = new HisExpMestBloodViewFilter();
                    expFilter.SUPPLIER_IDs = listSupplierId;
                    expFilter.BID_ID = bidId;
                    expFilter.CREATE_TIME_FROM = currenthisBid.CREATE_TIME;
                    expFilter.CREATE_TIME_TO = Inventec.Common.DateTime.Get.Now();
                    var expMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, expFilter, null);
                    if (expMests != null && expMests.Count > 0)
                    {
                        expMests = expMests.Where(o => o.IS_EXPORT == 1 && o.IS_TH != 1).ToList();
                        foreach (var item in expMests)
                        {
                            if (!dicExpMest.ContainsKey(item.BLOOD_TYPE_ID))
                                dicExpMest[item.BLOOD_TYPE_ID] = new List<V_HIS_EXP_MEST_BLOOD>();
                            dicExpMest[item.BLOOD_TYPE_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateGrid()
        {
            if (ListMedicineTypeAdoProcess != null)
            {
                var medi = ListMedicineTypeAdoProcess.Where(o => o.Type == Base.GlobalConfig.THUOC).ToList();
                var mate = ListMedicineTypeAdoProcess.Where(o => o.Type == Base.GlobalConfig.VATTU).ToList();
                var blo = ListMedicineTypeAdoProcess.Where(o => o.Type == Base.GlobalConfig.MAU).ToList();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medi), medi));

                ucBlood.Reload(blo);
                ucMaterial.Reload(mate);
                ucMedicine.Reload(medi);
                btnSave.Enabled = true;
                if (checkLoad)
                {
                    if (medi != null)
                    {
                        xtraTabControlEditBid.SelectedTabPage = xtraTabPageMedicine;
                    }
                    else if (mate != null)
                    {
                        xtraTabControlEditBid.SelectedTabPage = xtraTabPageMaterial;
                    }
                    else if (blo != null)
                    {
                        xtraTabControlEditBid.SelectedTabPage = xtraTabPageBlood;
                    }
                }
                checkLoad = false;

            }
        }
        #endregion

        #region event
        private void EnableButton(int action)
        {
            try
            {
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                btnUpdate.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                btnSave.Enabled = checkLoad ? false : (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisibleButton(int action)
        {
            try
            {
                DevExpress.XtraLayout.Utils.LayoutVisibility always = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                DevExpress.XtraLayout.Utils.LayoutVisibility never = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciBtnAdd.Visibility = (action == GlobalVariables.ActionAdd ? always : never);
                lciBtnUpdate.Visibility = (action == GlobalVariables.ActionEdit ? always : never);
                lciBtnDiscard.Visibility = (action == GlobalVariables.ActionEdit ? always : never);
                Size root = Root.Size;
                if (action == GlobalVariables.ActionAdd)
                {
                    emptySpaceItem1.Size = new Size(root.Width - 110, 26);
                    lciBtnAdd.Size = new Size(110, 26);
                }
                else
                {
                    emptySpaceItem1.Size = new Size(root.Width - 220, 26);
                    lciBtnUpdate.Size = new Size(110, 26);
                    lciBtnDiscard.Size = new Size(110, 26);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValueForAdd()
        {
            try
            {
                spinAmount.Value = 0;
                spinImpVat.Value = 0;
                spinImpMoreRatio.Value = 0;
                spinImpPrice.Value = 0;
                txtBidNumOrder.Text = "";
                txtNOTE.Text = null;
                if (chkClearToAdd.Checked)
                {
                    txtBidPackageCode.Text = "";
                    txtBidGroupCode.Text = "";
                    cboSupplier.EditValue = null;
                    txtSupplierCode.Text = "";
                }
                cboInformationBid.SelectedIndex = -1;
                // thiet lap mac dinh
                if (xtraTabControl1.SelectedTabPageIndex == 0 && this.medicineType != null && !String.IsNullOrEmpty(this.medicineType.MEDICINE_TYPE_CODE))
                {
                    var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_NAME == this.medicineType.NATIONAL_NAME).ToList();
                    var dosage = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DOSAGE_FORM>().Where(o => o.DOSAGE_FORM_NAME == this.medicineType.DOSAGE_FORM).ToList();  
                    txtRegisterNumber.Text = this.medicineType.REGISTER_NUMBER;
                    if (national != null && national.Count() > 0)
                    {
                        txtNationalMainText.Text = national[0].NATIONAL_NAME;
                        cboNational.EditValue = national[0].ID;
                        txtNationalMainText.Visible = false;
                        cboNational.Visible = true;
                        chkEditNational.CheckState = CheckState.Unchecked;
                    }
                    else
                    {
                        txtNationalMainText.Text = this.medicineType.NATIONAL_NAME;
                        cboNational.EditValue = null;
                        txtNationalMainText.Visible = true;
                        cboNational.Visible = false;
                        chkEditNational.CheckState = CheckState.Checked;
                    }

                    cboManufacture.EditValue = this.medicineType.MANUFACTURER_ID;
                    txtManufacture.Text = this.medicineType.MANUFACTURER_CODE;
                    txtConcentra.Text = this.medicineType.CONCENTRA;

                    txtTenBHYT.Text = HisConfigCFG.IsSet__BHYT ? this.medicineType.HEIN_SERVICE_BHYT_NAME : "";
                    txtPackingType.Text = HisConfigCFG.IsSet__BHYT ? this.medicineType.PACKING_TYPE_NAME : "";
                    txtActiveBhyt.Text = HisConfigCFG.IsSet__BHYT ? this.medicineType.ACTIVE_INGR_BHYT_NAME : "";
                    cboMediUseForm.EditValue = HisConfigCFG.IsSet__BHYT ? this.medicineType.MEDICINE_USE_FORM_ID : null;
                    if (dosage != null && dosage.Count() > 0)
                    {
                        cboDosageForm.EditValue = dosage[0].ID;
                    }
                    else
                    {
                        cboDosageForm.EditValue = null;
                    }
                    //cboDosageForm.EditValue = HisConfigCFG.IsSet__BHYT ? this.medicineType.DOSAGE_FORM : "";

                    if (this.medicineType.DAY_LIFESPAN.HasValue)
                    {
                        spinDayLifeSpan.EditValue = this.medicineType.DAY_LIFESPAN.Value;
                    }
                    else
                    {
                        spinDayLifeSpan.EditValue = null;
                    }
                    if (this.medicineType.MONTH_LIFESPAN.HasValue)
                    {
                        spinMonthLifeSpan.EditValue = this.medicineType.MONTH_LIFESPAN.Value;
                    }
                    else
                    {
                        spinMonthLifeSpan.EditValue = null;
                    }
                    if (this.medicineType.HOUR_LIFESPAN.HasValue)
                    {
                        spinHourLifeSpan.EditValue = this.medicineType.HOUR_LIFESPAN.Value;
                    }
                    else
                    {
                        spinHourLifeSpan.EditValue = null;
                    }
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1 && this.materialType != null && !String.IsNullOrEmpty(this.materialType.MATERIAL_TYPE_CODE))
                {
                    var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_NAME == this.materialType.NATIONAL_NAME).ToList();
                    txtRegisterNumber.Text = "";
                    txtRegisterNumber.Enabled = false;
                    if (national != null && national.Count() > 0)
                    {
                        txtNationalMainText.Text = national[0].NATIONAL_NAME;
                        cboNational.EditValue = national[0].ID;
                        txtNationalMainText.Visible = false;
                        cboNational.Visible = true;
                        chkEditNational.CheckState = CheckState.Unchecked;
                    }
                    else
                    {
                        txtNationalMainText.Text = this.materialType.NATIONAL_NAME;
                        cboNational.EditValue = null;
                        txtNationalMainText.Visible = true;
                        cboNational.Visible = false;
                        chkEditNational.CheckState = CheckState.Checked;
                    }

                    cboManufacture.EditValue = this.materialType.MANUFACTURER_ID;
                    txtManufacture.Text = this.materialType.MANUFACTURER_CODE;
                    txtConcentra.Text = this.materialType.CONCENTRA;

                    MOS.Filter.HisBidMaterialTypeViewFilter bidMatyFilter = new MOS.Filter.HisBidMaterialTypeViewFilter();
                    bidMatyFilter.MATERIAL_TYPE_ID = materialType.ID;
                    bidMaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_BID_MATERIAL_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_BID_MATERIAL_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bidMatyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (bidMaty != null && bidMaty.Count > 0)
                    {
                        this.materialType.BID_MATERIAL_TYPE_NAME = bidMaty[0].BID_MATERIAL_TYPE_NAME;
                        this.materialType.BID_MATERIAL_TYPE_CODE = bidMaty[0].BID_MATERIAL_TYPE_CODE;
                    }
                    txtTenTT.Text = this.materialType.BID_MATERIAL_TYPE_NAME ?? "";
                    txtMaTT.Text = this.materialType.BID_MATERIAL_TYPE_CODE ?? "";

                    if (this.materialType.DAY_LIFESPAN.HasValue)
                    {
                        spinDayLifeSpan.EditValue = this.materialType.DAY_LIFESPAN.Value;
                    }
                    else
                    {
                        spinDayLifeSpan.EditValue = null;
                    }
                    if (this.materialType.MONTH_LIFESPAN.HasValue)
                    {
                        spinMonthLifeSpan.EditValue = this.materialType.MONTH_LIFESPAN.Value;
                    }
                    else
                    {
                        spinMonthLifeSpan.EditValue = null;
                    }
                    if (this.materialType.HOUR_LIFESPAN.HasValue)
                    {
                        spinHourLifeSpan.EditValue = this.materialType.HOUR_LIFESPAN.Value;
                    }
                    else
                    {
                        spinHourLifeSpan.EditValue = null;
                    }
                    cboInformationBid.SelectedIndex = 0;
                }
                else
                {
                    EnableLeftControl(false);
                    ResetLeftControl();
                }
                spinAmount.Focus();
                spinAmount.SelectAll();
                dxValidationProviderLeft.RemoveControlError(spinImpPrice);
                dxValidationProviderLeft.RemoveControlError(spinAmount);
                dxValidationProviderLeft.RemoveControlError(spinImpVat);
                dxValidationProviderLeft.RemoveControlError(txtBidNumOrder);
                dxValidationProviderLeft.RemoveControlError(cboSupplier);
                dxValidationProviderLeft.RemoveControlError(txtBidPackageCode);
                dxValidationProviderLeft.RemoveControlError(txtBidGroupCode);
                dxValidationProviderLeft.RemoveControlError(txtTenBHYT);
                dxValidationProviderLeft.RemoveControlError(txtTenTT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addMedicine()
        {
            try
            {
                this.medicineType.AllowDelete = true;
                this.medicineType.IMP_PRICE = spinImpPrice.Value;
                this.medicineType.BID_NUM_ORDER = txtBidNumOrder.Text;
                this.medicineType.IMP_VAT_RATIO = spinImpVat.Value / 100;
                this.medicineType.ImpVatRatio = spinImpVat.Value;
                this.medicineType.ImpMoreRatio = spinImpMoreRatio.Value;
                this.medicineType.AMOUNT = spinAmount.Value;
                this.medicineType.BID_GROUP_CODE = txtBidGroupCode.Text;
                this.medicineType.BID_PACKAGE_CODE = txtBidPackageCode.Text;
                this.medicineType.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                if (cboSupplier.EditValue != null)
                {
                    this.medicineType.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier.EditValue.ToString());
                    this.medicineType.SUPPLIER_NAME = cboSupplier.Text.Trim();
                }
                else
                    this.medicineType.SUPPLIER_ID = null;

                if (DtExpiredDate.EditValue != null)
                {
                    if (DtExpiredDate.DateTime < DateTime.Today)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.HanSuDungKhongDuocNhoHonNgayHienTai, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    }
                    this.medicineType.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DtExpiredDate.DateTime);
                }
                else
                {
                    this.medicineType.EXPIRED_DATE = null;
                }
                var rs = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.ID == medicineType.ID).FirstOrDefault();
                if (rs != null)
                    this.medicineType.SERVICE_UNIT_NAME = rs.IMP_UNIT_ID.HasValue ? rs.IMP_UNIT_NAME : rs.SERVICE_UNIT_NAME;
                this.medicineType.NATIONAL_NAME = txtNationalMainText.Text.Trim();

                if (cboManufacture.EditValue != null)
                {
                    this.medicineType.MANUFACTURER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboManufacture.EditValue.ToString());
                    this.medicineType.MANUFACTURER_NAME = cboManufacture.Text.Trim();
                }
                else
                {
                    this.medicineType.MANUFACTURER_ID = null;
                    this.medicineType.MANUFACTURER_NAME = "";
                }

                this.medicineType.CONCENTRA = txtConcentra.Text.Trim();
                this.medicineType.REGISTER_NUMBER = txtRegisterNumber.Text.Trim();
                this.medicineType.HEIN_SERVICE_BHYT_NAME = txtTenBHYT.Text.Trim();
                this.medicineType.PACKING_TYPE_NAME = txtPackingType.Text.Trim();
                this.medicineType.ACTIVE_INGR_BHYT_NAME = txtActiveBhyt.Text.Trim();
                this.medicineType.DOSAGE_FORM = cboDosageForm.Text.Trim();

                if (spinHourLifeSpan.EditValue != null)
                {
                    this.medicineType.HOUR_LIFESPAN = (long)spinHourLifeSpan.Value;
                }
                else
                {
                    this.medicineType.HOUR_LIFESPAN = null;
                }

                if (spinDayLifeSpan.EditValue != null)
                {
                    this.medicineType.DAY_LIFESPAN = (long)spinDayLifeSpan.Value;
                }
                else
                {
                    this.medicineType.DAY_LIFESPAN = null;
                }

                if (spinMonthLifeSpan.EditValue != null)
                {
                    this.medicineType.MONTH_LIFESPAN = (long)spinMonthLifeSpan.Value;
                }
                else
                {
                    this.medicineType.MONTH_LIFESPAN = null;
                }

                if (cboMediUseForm.EditValue != null)
                {
                    HIS_MEDICINE_USE_FORM useForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboMediUseForm.EditValue));
                    if (useForm != null)
                    {
                        this.medicineType.MEDICINE_USE_FORM_ID = useForm.ID;
                        this.medicineType.MEDICINE_USE_FORM_CODE = useForm.MEDICINE_USE_FORM_CODE;
                        this.medicineType.MEDICINE_USE_FORM_NAME = useForm.MEDICINE_USE_FORM_NAME;
                    }
                    else
                    {
                        this.medicineType.MEDICINE_USE_FORM_ID = null;
                        this.medicineType.MEDICINE_USE_FORM_CODE = null;
                        this.medicineType.MEDICINE_USE_FORM_NAME = null;
                    }
                }
                else
                {
                    this.medicineType.MEDICINE_USE_FORM_ID = null;
                    this.medicineType.MEDICINE_USE_FORM_CODE = null;
                    this.medicineType.MEDICINE_USE_FORM_NAME = null;
                }

                if (!string.IsNullOrEmpty(txtNOTE.Text))
                {
                    this.medicineType.NOTE = txtNOTE.Text;
                }
                this.ListMedicineTypeAdoProcess.Add(this.medicineType);
                this.xtraTabControlEditBid.SelectedTabPage = xtraTabPageMedicine;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addMaterial()
        {
            try
            {
                this.materialType.IMP_PRICE = spinImpPrice.Value;
                this.materialType.BID_NUM_ORDER = txtBidNumOrder.Text;
                this.materialType.IMP_VAT_RATIO = spinImpVat.Value / 100;
                this.materialType.AMOUNT = spinAmount.Value;
                ADO.MedicineTypeADO aMedicineSdo = new ADO.MedicineTypeADO();
                AutoMapper.Mapper.CreateMap<ADO.MaterialTypeADO, ADO.MedicineTypeADO>();
                aMedicineSdo = AutoMapper.Mapper.Map<ADO.MaterialTypeADO, ADO.MedicineTypeADO>(this.materialType);
                aMedicineSdo.ImpVatRatio = spinImpVat.Value;
                aMedicineSdo.ImpMoreRatio = spinImpMoreRatio.Value;
                aMedicineSdo.MEDICINE_TYPE_CODE = this.materialType.MATERIAL_TYPE_CODE;
                aMedicineSdo.MEDICINE_TYPE_NAME = this.materialType.MATERIAL_TYPE_NAME;
                aMedicineSdo.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                aMedicineSdo.AllowDelete = true;
                aMedicineSdo.BID_MATERIAL_TYPE_CODE = txtMaTT.Text ?? "";
                aMedicineSdo.BID_MATERIAL_TYPE_NAME = txtTenTT.Text ?? "";
                aMedicineSdo.JOIN_BID_MATERIAL_TYPE_CODE = txtMaDT.Text ?? "";
                aMedicineSdo.INFORMATION_BID = this.cboInformationBid.SelectedIndex != -1 ? (long?)(this.cboInformationBid.SelectedIndex + 1) : null;

                var rs = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.ID == aMedicineSdo.ID).FirstOrDefault();
                if (rs != null)
                    aMedicineSdo.SERVICE_UNIT_NAME = rs.IMP_UNIT_ID.HasValue ? rs.IMP_UNIT_NAME : rs.SERVICE_UNIT_NAME;

                if (cboSupplier.EditValue != null)
                {
                    aMedicineSdo.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier.EditValue.ToString());
                    aMedicineSdo.SUPPLIER_NAME = cboSupplier.Text.Trim();
                }
                else
                    aMedicineSdo.SUPPLIER_ID = null;
                aMedicineSdo.BID_GROUP_CODE = txtBidGroupCode.Text;
                aMedicineSdo.BID_PACKAGE_CODE = txtBidPackageCode.Text;

                if (DtExpiredDate.EditValue != null)
                {
                    if (DtExpiredDate.DateTime < DateTime.Today)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.HanSuDungKhongDuocNhoHonNgayHienTai, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    }
                    aMedicineSdo.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DtExpiredDate.DateTime);
                }
                else
                {
                    aMedicineSdo.EXPIRED_DATE = null;
                }

                aMedicineSdo.NATIONAL_NAME = txtNationalMainText.Text.Trim();

                if (cboManufacture.EditValue != null)
                {
                    aMedicineSdo.MANUFACTURER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboManufacture.EditValue.ToString());
                    aMedicineSdo.MANUFACTURER_NAME = cboManufacture.Text.Trim();
                }
                else
                    aMedicineSdo.MANUFACTURER_ID = null;

                aMedicineSdo.CONCENTRA = txtConcentra.Text.Trim();
                if (spinHourLifeSpan.EditValue != null)
                {
                    aMedicineSdo.HOUR_LIFESPAN = (long)spinHourLifeSpan.Value;
                }
                else
                {
                    aMedicineSdo.HOUR_LIFESPAN = null;
                }

                if (spinDayLifeSpan.EditValue != null)
                {
                    aMedicineSdo.DAY_LIFESPAN = (long)spinDayLifeSpan.Value;
                }
                else
                {
                    aMedicineSdo.DAY_LIFESPAN = null;
                }

                if (spinMonthLifeSpan.EditValue != null)
                {
                    aMedicineSdo.MONTH_LIFESPAN = (long)spinMonthLifeSpan.Value;
                }
                else
                {
                    aMedicineSdo.MONTH_LIFESPAN = null;
                }
                if (!string.IsNullOrEmpty(txtNOTE.Text))
                {
                    aMedicineSdo.NOTE = txtNOTE.Text;
                }
                this.ListMedicineTypeAdoProcess.Add(aMedicineSdo);
                this.xtraTabControlEditBid.SelectedTabPage = xtraTabPageMaterial;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addBlood()
        {
            try
            {
                this.bloodType.IMP_PRICE = spinImpPrice.Value;
                this.bloodType.BID_NUM_ORDER = txtBidNumOrder.Text;
                this.bloodType.IMP_VAT_RATIO = spinImpVat.Value / 100;
                this.bloodType.AMOUNT = spinAmount.Value;
                ADO.MedicineTypeADO aMedicineSdo = new ADO.MedicineTypeADO();
                AutoMapper.Mapper.CreateMap<ADO.BloodTypeADO, ADO.MedicineTypeADO>();
                aMedicineSdo = AutoMapper.Mapper.Map<ADO.BloodTypeADO, ADO.MedicineTypeADO>(this.bloodType);
                aMedicineSdo.ImpVatRatio = spinImpVat.Value;
                aMedicineSdo.MEDICINE_TYPE_CODE = this.bloodType.BLOOD_TYPE_CODE;
                aMedicineSdo.MEDICINE_TYPE_NAME = this.bloodType.BLOOD_TYPE_NAME;
                aMedicineSdo.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                aMedicineSdo.AllowDelete = true;
                if (cboSupplier.EditValue != null)
                {
                    aMedicineSdo.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier.EditValue.ToString());
                    aMedicineSdo.SUPPLIER_NAME = cboSupplier.Text.Trim();
                }
                else
                    aMedicineSdo.SUPPLIER_ID = null;
                aMedicineSdo.BID_GROUP_CODE = txtBidGroupCode.Text;
                aMedicineSdo.BID_PACKAGE_CODE = txtBidPackageCode.Text;
                this.ListMedicineTypeAdoProcess.Add(aMedicineSdo);
                this.xtraTabControlEditBid.SelectedTabPage = xtraTabPageBlood;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private double setIdRow(List<ADO.MedicineTypeADO> medicineTypes)
        {
            double currentIdRow = 0;
            try
            {
                if (medicineTypes != null && medicineTypes.Count > 0)
                {
                    var maxIdRow = medicineTypes.Max(o => o.IdRow);
                    currentIdRow = ++maxIdRow;
                }
                else
                {
                    currentIdRow = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentIdRow;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (lciBtnAdd.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                positionHandleLeft = -1;
                if (!btnAdd.Enabled && this.ActionType == GlobalVariables.ActionEdit)
                    return;
                if (!dxValidationProviderLeft.Validate()) return;
                if (xtraTabControl1.SelectedTabPageIndex == 0) // thuoc
                {
                    if (!WarningBhytInfo()) return;
                    var aMedicineType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.medicineType.ID &&
                        o.Type == Base.GlobalConfig.THUOC &&
                        o.SUPPLIER_ID == (long)cboSupplier.EditValue
                        && o.BID_GROUP_CODE == txtBidGroupCode.Text);
                    if (aMedicineType != null && aMedicineType.ID > 0)
                    {
                        this.ListMedicineTypeAdoProcess.RemoveAll(o => o == aMedicineType);
                        addMedicine();
                    }
                    else addMedicine();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1) // vat tu
                {
                    var aMaterialType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o =>
                        o.ID == this.materialType.ID &&
                        o.IsMaterialTypeMap == this.materialType.IsMaterialTypeMap &&
                        o.Type == Base.GlobalConfig.VATTU &&
                        o.SUPPLIER_ID == (long)cboSupplier.EditValue
                        && o.BID_GROUP_CODE == txtBidGroupCode.Text);
                    if (aMaterialType != null && aMaterialType.ID > 0)
                    {
                        this.ListMedicineTypeAdoProcess.RemoveAll(o => o == aMaterialType);
                        addMaterial();
                    }
                    else addMaterial();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2) // Mau
                {
                    var aBloodType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.bloodType.ID &&
                        o.Type == Base.GlobalConfig.MAU &&
                        o.SUPPLIER_ID == (long)cboSupplier.EditValue
                        && o.BID_GROUP_CODE == txtBidGroupCode.Text);
                    if (aBloodType != null && aBloodType.ID > 0)
                    {
                        this.ListMedicineTypeAdoProcess.RemoveAll(o => o == aBloodType);
                        addBlood();
                    }
                    else addBlood();
                }
                UpdateGrid();
                SetValueForAdd();

                //FocusTab();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleLeft = -1;
                if (lciBtnUpdate.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                if (!dxValidationProviderLeft.Validate() && this.ActionType == GlobalVariables.ActionAdd)
                    return;
                if (!dxValidationProviderLeft.Validate())
                    return;
                if (medicineType != null)
                {
                    ListMedicineTypeAdoProcess.RemoveAll(o => o == this.medicineType);
                    if (this.medicineType.Type == Base.GlobalConfig.THUOC)
                    {
                        if (!WarningBhytInfo()) return;
                        addMedicine();
                    }
                    if (this.medicineType.Type == Base.GlobalConfig.VATTU) addMaterial();
                    if (this.medicineType.Type == Base.GlobalConfig.MAU) addBlood();
                }
                this.ActionType = GlobalVariables.ActionAdd;
                VisibleButton(this.ActionType);
                UpdateGrid();
                SetDefaultValueControlLeft();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnDiscard_Click(object sender, EventArgs e)
        {
            try
            {
                if (lciBtnDiscard.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                SetDefaultValueControlLeft();
                this.ActionType = GlobalVariables.ActionAdd;
                VisibleButton(this.ActionType);
                txtBidNumOrder.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ucBlood.PostEditor();
            ucMaterial.PostEditor();
            ucMedicine.PostEditor();
            CommonParam paramCommon = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandleRight = -1;

                if (!dxValidationProviderRight.Validate())
                    return;

                if (CheckValidDataInGridService(ref paramCommon, ListMedicineTypeAdoProcess))
                {
                    WaitingManager.Hide();
                    getDataForProcess();
                    if (this.bidModel == null ||
                        this.bidModel.HIS_BID_MEDICINE_TYPE == null ||
                        this.bidModel.HIS_BID_MATERIAL_TYPE == null ||
                        this.bidModel.HIS_BID_BLOOD_TYPE == null ||
                        (this.bidModel.HIS_BID_MEDICINE_TYPE.Count <= 0 &&
                        this.bidModel.HIS_BID_MATERIAL_TYPE.Count <= 0 &&
                        this.bidModel.HIS_BID_BLOOD_TYPE.Count <= 0))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show
                            (Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc);
                        return;
                    }

                    if (bidModel.VALID_FROM_TIME.HasValue || bidModel.VALID_TO_TIME.HasValue)
                    {
                        List<string> lstAdjustTime = new List<string>();
                        if (this.bidModel.HIS_BID_MEDICINE_TYPE != null && this.bidModel.HIS_BID_MEDICINE_TYPE.Count > 0)
                        {
                            CommonParam param = new CommonParam();
                            MOS.Filter.HisBidMetyAdjustFilter Bmafilter = new MOS.Filter.HisBidMetyAdjustFilter();
                            Bmafilter.TDL_BID_ID = bid_id;
                            var BidMetys = new BackendAdapter(param)
                        .Get<List<HIS_BID_METY_ADJUST>>("api/HisBidMetyAdjust/Get", ApiConsumers.MosConsumer, Bmafilter, param).ToList();
                            if (BidMetys != null && BidMetys.Count > 0)
                            {
                                var lstMety = BidMetys.Where(o => this.bidModel.HIS_BID_MEDICINE_TYPE.ToList().Exists(p => p.ID == o.BID_MEDICINE_TYPE_ID) && o.ADJUST_TIME.HasValue && ((bidModel.VALID_FROM_TIME.HasValue && o.ADJUST_TIME < bidModel.VALID_FROM_TIME) || (bidModel.VALID_TO_TIME.HasValue && o.ADJUST_TIME > bidModel.VALID_TO_TIME))).ToList();
                                if (lstMety != null && lstMety.Count > 0)
                                {
                                    lstAdjustTime.AddRange(listHisMedicineType.Where(p => this.bidModel.HIS_BID_MEDICINE_TYPE.ToList().Exists(k => lstMety.Exists(o => o.BID_MEDICINE_TYPE_ID == k.ID) && k.MEDICINE_TYPE_ID == p.ID)).ToList().Select(o => o.MEDICINE_TYPE_NAME).Distinct());
                                }

                            }
                        }

                        if (this.bidModel.HIS_BID_MATERIAL_TYPE != null && this.bidModel.HIS_BID_MATERIAL_TYPE.Count > 0)
                        {
                            CommonParam param = new CommonParam();
                            MOS.Filter.HisBidMatyAdjustFilter Bmafilter = new MOS.Filter.HisBidMatyAdjustFilter();
                            Bmafilter.TDL_BID_ID = bid_id;
                            var BidMatys = new BackendAdapter(param)
                        .Get<List<HIS_BID_MATY_ADJUST>>("api/HisBidMatyAdjust/Get", ApiConsumers.MosConsumer, Bmafilter, param).ToList();
                            if (BidMatys != null && BidMatys.Count > 0)
                            {
                                var lstMaty = BidMatys.Where(o => this.bidModel.HIS_BID_MATERIAL_TYPE.ToList().Exists(p => p.ID == o.BID_MATERIAL_TYPE_ID) && o.ADJUST_TIME.HasValue && ((bidModel.VALID_FROM_TIME.HasValue && o.ADJUST_TIME < bidModel.VALID_FROM_TIME) || (bidModel.VALID_TO_TIME.HasValue && o.ADJUST_TIME > bidModel.VALID_TO_TIME))).ToList();
                                if (lstMaty != null && lstMaty.Count > 0)
                                {
                                    lstAdjustTime.AddRange(listHisMaterialType.Where(p => this.bidModel.HIS_BID_MATERIAL_TYPE.ToList().Exists(k => lstMaty.Exists(o => o.BID_MATERIAL_TYPE_ID == k.ID) && (k.MATERIAL_TYPE_ID == p.ID))).ToList().Select(o => o.MATERIAL_TYPE_NAME).Distinct());
                                }

                            }
                        }
                        if (lstAdjustTime != null && lstAdjustTime.Count > 0)
                        {
                            XtraMessageBox.Show(string.Format("Tồn tại dữ liệu thuốc/vật tư {0} có ngày nhập điều tiết nằm ngoài khoảng thời gian hiệu lực thầu", string.Join(", ", lstAdjustTime)), "Thông báo");
                            return;
                        }
                    }

                    if (ListMedicineTypeAdoProcess != null && ListMedicineTypeAdoProcess.Count > 0)
                    {
                        List<string> messages = new List<string>();
                        var checkMinAmount = ListMedicineTypeAdoProcess.Where(o => o.AMOUNT < o.Min_AMOUNT).ToList();
                        if (checkMinAmount != null && checkMinAmount.Count > 0)
                        {
                            foreach (var item in checkMinAmount)
                            {
                                string mes = String.Format(Resources.ResourceMessage.SoLuongKhongDuocNhoHonSoLuongYeuCauNhap, item.MEDICINE_TYPE_CODE, item.Min_AMOUNT);
                                messages.Add(mes);
                            }

                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Join(", ", messages) + ". Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return;
                            }
                        }
                    }
                    bidModel.ID = bid_id;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bidModel), bidModel));
                    bidPrint = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<MOS.EFMODEL.DataModels.HIS_BID>("api/HisBid/Update", ApiConsumer.ApiConsumers.MosConsumer, bidModel, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (bidPrint != null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Tệp có kq");
                        success = true;
                        //EnableButton(GlobalVariables.ActionView);
                        spinAmount.Value = 0;
                        spinImpPrice.Value = 0;
                        spinImpVat.Value = 0;
                        spinImpMoreRatio.Value = 0;
                        txtBidNumOrder.Text = "";
                        txtBidGroupCode.Text = "";
                        txtBidPackageCode.Text = "";
                        txtNationalMainText.Text = "";
                        txtConcentra.Text = "";
                        txtSupplierCode.Text = "";
                        cboSupplier.EditValue = null;
                        cboNational.EditValue = null;
                        cboManufacture.EditValue = null;
                        txtNOTE.Text = "";
                        if (refeshData != null)
                            refeshData();
                        ListMedicineTypeAdoProcess = new List<MedicineTypeADO>();
                        CreateThreadLoadCurrentData();
                        ucBlood.IsFirstLoad = true;
                        ucMaterial.IsFirstLoad = true;
                        ucMedicine.IsFirstLoad = true;
                        var medi = ListMedicineTypeAdoProcess.Where(o => o.Type == Base.GlobalConfig.THUOC).ToList();
                        var mate = ListMedicineTypeAdoProcess.Where(o => o.Type == Base.GlobalConfig.VATTU).ToList();
                        var blo = ListMedicineTypeAdoProcess.Where(o => o.Type == Base.GlobalConfig.MAU).ToList();

                        ucBlood.Reload(blo);
                        ucMaterial.Reload(mate);
                        ucMedicine.Reload(medi);

                    }
                    else
                        Inventec.Common.Logging.LogSystem.Warn("Tệp rỗng");
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, paramCommon, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void getDataForProcess()
        {
            try
            {
                this.bidModel = new MOS.EFMODEL.DataModels.HIS_BID();
                this.bidModel.HIS_BID_BLOOD_TYPE = new List<MOS.EFMODEL.DataModels.HIS_BID_BLOOD_TYPE>();
                this.bidModel.HIS_BID_MATERIAL_TYPE = new List<MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE>();
                this.bidModel.HIS_BID_MEDICINE_TYPE = new List<MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE>();
                this.bidModel.BID_NAME = txtBidName.Text.Trim();
                this.bidModel.BID_EXTRA_CODE = txtBID.Text.Trim();
                if (cboBidForm.EditValue != null)
                    this.bidModel.BID_FORM_ID = Int64.Parse(cboBidForm.EditValue.ToString());
                this.bidModel.BID_NUMBER = txtBidNumber.Text.Trim();
                this.bidModel.BID_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBidType.EditValue.ToString());
                this.bidModel.BID_YEAR = txtBidYear.Text.Trim();
                this.bidModel.BID_APTHAU_CODE = txtBidApthauCode.Text.Trim();
                this.bidModel.ALLOW_UPDATE_LOGINNAMES = this.currenthisBid != null ? this.currenthisBid.ALLOW_UPDATE_LOGINNAMES : "";

                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                {
                    this.bidModel.VALID_FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFromTime.DateTime);
                }
                else
                {
                    this.bidModel.VALID_FROM_TIME = null;
                }

                if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    this.bidModel.VALID_TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtToTime.DateTime);
                }
                else
                {
                    this.bidModel.VALID_TO_TIME = null;
                }

                foreach (var item in this.ListMedicineTypeAdoProcess)
                {
                    if (item.Type == Base.GlobalConfig.THUOC)
                    {
                        var bidMedicineType = new MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE();
                        bidMedicineType.ID = item.BID_MEDI_MATY_BLO_ID;
                        bidMedicineType.BID_ID = this.bid_id;
                        bidMedicineType.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                        bidMedicineType.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                        bidMedicineType.ADJUST_AMOUNT = item.ADJUST_AMOUNT;
                        bidMedicineType.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        bidMedicineType.IMP_MORE_RATIO = item.ImpMoreRatio != null ? item.ImpMoreRatio / 100 : null;
                        bidMedicineType.MEDICINE_TYPE_ID = item.ID;
                        bidMedicineType.BID_NUM_ORDER = item.BID_NUM_ORDER;
                        bidMedicineType.SUPPLIER_ID = (long)(item.SUPPLIER_ID ?? 0);
                        string bid_group_code = null;
                        if (!string.IsNullOrEmpty(item.BID_GROUP_CODE))
                        {
                            bid_group_code = item.BID_GROUP_CODE;
                        }
                        bidMedicineType.BID_GROUP_CODE = bid_group_code;
                        bidMedicineType.BID_PACKAGE_CODE = item.BID_PACKAGE_CODE;
                        bidMedicineType.EXPIRED_DATE = item.EXPIRED_DATE;
                        bidMedicineType.CONCENTRA = item.CONCENTRA;
                        bidMedicineType.MANUFACTURER_ID = item.MANUFACTURER_ID;
                        bidMedicineType.MEDICINE_TYPE_ID = item.ID;
                        bidMedicineType.MEDICINE_REGISTER_NUMBER = item.REGISTER_NUMBER;

                        bidMedicineType.NATIONAL_NAME = item.NATIONAL_NAME;

                        bidMedicineType.MONTH_LIFESPAN = item.MONTH_LIFESPAN;
                        bidMedicineType.HOUR_LIFESPAN = item.HOUR_LIFESPAN;
                        bidMedicineType.DAY_LIFESPAN = item.DAY_LIFESPAN;

                        bidMedicineType.MEDICINE_USE_FORM_ID = item.MEDICINE_USE_FORM_ID;
                        bidMedicineType.DOSAGE_FORM = item.DOSAGE_FORM ?? "";
                        bidMedicineType.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME ?? "";
                        bidMedicineType.HEIN_SERVICE_BHYT_NAME = item.HEIN_SERVICE_BHYT_NAME ?? "";
                        bidMedicineType.PACKING_TYPE_NAME = item.PACKING_TYPE_NAME ?? "";
                        bidMedicineType.NOTE = item.NOTE ?? "";
                        this.bidModel.HIS_BID_MEDICINE_TYPE.Add(bidMedicineType);
                    }
                    else if (item.Type == Base.GlobalConfig.VATTU)
                    {
                        var bidMaterialType = new MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE();
                        bidMaterialType.ID = item.BID_MEDI_MATY_BLO_ID;
                        bidMaterialType.BID_ID = this.bid_id;
                        bidMaterialType.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                        bidMaterialType.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                        bidMaterialType.ADJUST_AMOUNT = item.ADJUST_AMOUNT;
                        bidMaterialType.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        bidMaterialType.IMP_MORE_RATIO = item.ImpMoreRatio != null ? item.ImpMoreRatio / 100 : null;
                        if (item.IsMaterialTypeMap)
                        {
                            bidMaterialType.MATERIAL_TYPE_MAP_ID = item.ID;
                        }
                        else
                        {
                            bidMaterialType.MATERIAL_TYPE_ID = item.ID;
                        }
                        bidMaterialType.BID_NUM_ORDER = item.BID_NUM_ORDER;
                        bidMaterialType.SUPPLIER_ID = (long)(item.SUPPLIER_ID ?? 0);
                        string bid_group_code = null;
                        if (!string.IsNullOrEmpty(item.BID_GROUP_CODE))
                        {
                            bid_group_code = item.BID_GROUP_CODE;
                        }
                        bidMaterialType.BID_GROUP_CODE = bid_group_code;
                        bidMaterialType.BID_PACKAGE_CODE = item.BID_PACKAGE_CODE;
                        bidMaterialType.EXPIRED_DATE = item.EXPIRED_DATE;

                        bidMaterialType.CONCENTRA = item.CONCENTRA;
                        bidMaterialType.MANUFACTURER_ID = item.MANUFACTURER_ID;

                        bidMaterialType.NATIONAL_NAME = item.NATIONAL_NAME;

                        bidMaterialType.MONTH_LIFESPAN = item.MONTH_LIFESPAN;
                        bidMaterialType.HOUR_LIFESPAN = item.HOUR_LIFESPAN;
                        bidMaterialType.DAY_LIFESPAN = item.DAY_LIFESPAN;
                        bidMaterialType.MATERIAL_TYPE_ID = item.ID;
                        bidMaterialType.JOIN_BID_MATERIAL_TYPE_CODE = item.JOIN_BID_MATERIAL_TYPE_CODE;
                        bidMaterialType.BID_MATERIAL_TYPE_CODE = item.BID_MATERIAL_TYPE_CODE;
                        bidMaterialType.BID_MATERIAL_TYPE_NAME = item.BID_MATERIAL_TYPE_NAME;
                        bidMaterialType.NOTE = item.NOTE ?? "";
                        bidMaterialType.INFORMATION_BID = item.INFORMATION_BID;
                        this.bidModel.HIS_BID_MATERIAL_TYPE.Add(bidMaterialType);
                    }
                    else if (item.Type == Base.GlobalConfig.MAU)
                    {
                        var bidBloodType = new MOS.EFMODEL.DataModels.HIS_BID_BLOOD_TYPE();
                        bidBloodType.ID = item.BID_MEDI_MATY_BLO_ID;
                        bidBloodType.BID_ID = this.bid_id;
                        bidBloodType.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                        bidBloodType.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                        bidBloodType.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        bidBloodType.BLOOD_TYPE_ID = item.ID;
                        bidBloodType.BID_NUM_ORDER = item.BID_NUM_ORDER;
                        bidBloodType.SUPPLIER_ID = (long)(item.SUPPLIER_ID ?? 0);
                        this.bidModel.HIS_BID_BLOOD_TYPE.Add(bidBloodType);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckValidDataInGridService(ref CommonParam param, List<ADO.MedicineTypeADO> MedicineCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (MedicineCheckeds__Send != null && MedicineCheckeds__Send.Count > 0)
                {
                    if (param.Messages == null)
                    {
                        param.Messages = new List<string>();
                    }
                    bool IsFirst = true;
                    foreach (var item in MedicineCheckeds__Send)
                    {
                        string messageErr = "";
                        bool result = true;
                        if (item.Type == Base.GlobalConfig.THUOC)
                        {
                            messageErr = String.Format(Resources.ResourceMessage.CanhBaoThuoc, item.MEDICINE_TYPE_NAME);
                        }
                        else if (item.Type == Base.GlobalConfig.VATTU)
                        {
                            messageErr = String.Format(Resources.ResourceMessage.CanhBaoVatTu, item.MEDICINE_TYPE_NAME);
                        }
                        else if (item.Type == Base.GlobalConfig.MAU)
                        {
                            messageErr = String.Format(Resources.ResourceMessage.CanhBaoMau, item.MEDICINE_TYPE_NAME);
                        }

                        if (item.SUPPLIER_ID == null || item.SUPPLIER_ID <= 0)
                        {
                            result = false;
                            messageErr += " " + Resources.ResourceMessage.KhongCoNhaCungCap;
                        }

                        if (item.AMOUNT < 0)
                        {
                            result = false;
                            messageErr += " " + Resources.ResourceMessage.SoLuongKhongDuocAm;
                        }
                        if (item.AMOUNT == 0 && (item.ADJUST_AMOUNT == null || item.ADJUST_AMOUNT == 0))
                        {
                            result = false;
                            messageErr += "bắt buộc phải nhập số lượng điều tiết";
                            if (IsFirst)
                            {
                                if (item.Type == Base.GlobalConfig.THUOC)
                                {
                                    ucMedicine.FocusAdjAmount();
                                }
                                else if (item.Type == Base.GlobalConfig.VATTU)
                                {
                                    ucMaterial.FocusAdjAmount();
                                }
                                else if (item.Type == Base.GlobalConfig.MAU)
                                {
                                    ucBlood.FocusAdjAmount();
                                }
                                IsFirst = false;
                            }
                        }

                        if (!String.IsNullOrEmpty(item.BID_NUM_ORDER) && item.BID_NUM_ORDER.Length > 50)
                        {
                            result = false;
                            messageErr += " " + Resources.ResourceMessage.SttThauQuaDai;
                        }

                        if (!String.IsNullOrWhiteSpace(item.BID_GROUP_CODE) && item.BID_GROUP_CODE.Length > 4)
                        {
                            result = false;
                            messageErr += " " + Resources.ResourceMessage.NhomThauQuaDai;
                        }


                        if (!String.IsNullOrWhiteSpace(item.BID_PACKAGE_CODE) && item.Type == Base.GlobalConfig.VATTU && item.BID_PACKAGE_CODE.Length > 4)
                        {
                            result = false;
                            messageErr += " mã gói thầu dài hơn 4 ký tự";
                        }
                        else if (!String.IsNullOrWhiteSpace(item.BID_PACKAGE_CODE) && item.Type != Base.GlobalConfig.VATTU && item.BID_PACKAGE_CODE.Length > 4)
                        {
                            result = false;
                            messageErr += " " + Resources.ResourceMessage.GoiThauQuaDai;
                        }

                        var listItem = MedicineCheckeds__Send.Where(o => o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE).ToList();
                        if (listItem != null && listItem.Count > 1)
                        {
                            foreach (var i in listItem)
                            {
                                if (i.SUPPLIER_ID == item.SUPPLIER_ID && i.IdRow != item.IdRow
                                    && i.BID_GROUP_CODE == item.BID_GROUP_CODE
                                    )
                                {
                                    result = false;
                                    messageErr += " " + Resources.ResourceMessage.BiTrung;
                                    break;
                                }
                            }
                        }

                        if (!result)
                        {
                            param.Messages.Add(messageErr);
                        }
                    }
                }
                else
                {
                    param.Messages.Add(Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc);
                }

                if (param.Messages.Count > 0)
                {
                    param.Messages = param.Messages.Distinct().ToList();
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                valid = false;
            }
            return valid;
        }

        bool WarningBhytInfo()
        {
            bool valid = true;
            try
            {
                if (this.medicineType.IS_BUSINESS != (short)1)
                {
                    List<string> fields = new List<string>();
                    Control focusControl = null;
                    if (String.IsNullOrWhiteSpace(txtRegisterNumber.Text))
                    {
                        fields.Add(Resources.ResourceMessage.SoDangKy);
                        if (focusControl == null) focusControl = txtRegisterNumber;
                    }
                    if (String.IsNullOrWhiteSpace(txtPackingType.Text))
                    {
                        fields.Add(Resources.ResourceMessage.QuyCachDongGoi);
                        if (focusControl == null) focusControl = txtPackingType;
                    }
                    if (String.IsNullOrWhiteSpace(txtTenBHYT.Text))
                    {
                        fields.Add(Resources.ResourceMessage.TenBHYT);
                        if (focusControl == null) focusControl = txtTenBHYT;
                    }
                    if (String.IsNullOrWhiteSpace(txtActiveBhyt.Text))
                    {
                        fields.Add(Resources.ResourceMessage.HoatChat);
                        if (focusControl == null) focusControl = txtActiveBhyt;
                    }
                    if (cboMediUseForm.EditValue == null)
                    {
                        fields.Add(Resources.ResourceMessage.DuongDung);
                        if (focusControl == null) focusControl = cboMediUseForm;
                    }

                    if (fields.Count > 0)
                    {
                        if (XtraMessageBox.Show(String.Format(Resources.ResourceMessage.BanChuaNhapCacTruongMuonTiepTuc, string.Join(", ", fields)), Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) != DialogResult.Yes)
                        {
                            if (focusControl != null) focusControl.Focus();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
        //private bool CheckValidDataInGridService(ref CommonParam param, List<ADO.MedicineTypeADO> MedicineCheckeds__Send)
        //{
        //    bool valid = true;
        //    try
        //    {
        //        if (MedicineCheckeds__Send != null && MedicineCheckeds__Send.Count > 0)
        //        {
        //            foreach (var item in MedicineCheckeds__Send)
        //            {
        //                bool result = true;
        //                string messageErr = "";
        //                if (item.Type == Base.GlobalConfig.THUOC)
        //                {
        //                    messageErr = String.Format(Resources.ResourceMessage.CanhBaoThuoc, item.MEDICINE_TYPE_NAME);
        //                }
        //                else if (item.Type == Base.GlobalConfig.VATTU)
        //                {
        //                    messageErr = String.Format(Resources.ResourceMessage.CanhBaoVatTu, item.MEDICINE_TYPE_NAME);
        //                }
        //                else if (item.Type == Base.GlobalConfig.MAU)
        //                {
        //                    messageErr = String.Format(Resources.ResourceMessage.CanhBaoMau, item.MEDICINE_TYPE_NAME);
        //                }

        //                if (item.SUPPLIER_ID == null || item.SUPPLIER_ID <= 0)
        //                {
        //                    result = false;
        //                    messageErr += Resources.ResourceMessage.KhongCoNhaCungCap;
        //                }
        //                if (item.AMOUNT < 0)
        //                {
        //                    result = false;
        //                    messageErr += Resources.ResourceMessage.SoLuongKhongDuocAm;
        //                }

        //                //if (item.AMOUNT < item.Min_AMOUNT)
        //                //{
        //                //    result = false;
        //                //    messageErr += messageErr = String.Format(Resources.ResourceMessage.SoLuongKhongDuocNhoHonSoLuongYeuCauNhap, item.Min_AMOUNT);
        //                //}

        //                if (!result)
        //                {
        //                    param.Messages.Add(messageErr + ";");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            param.Messages.Add("Dữ liệu trống");
        //        }

        //        if (param.Messages.Count > 0)
        //        {
        //            valid = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return valid;
        //}

        private void txtSupplierCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtSupplierCode.Text.Trim()))
                    {
                        string code = txtSupplierCode.Text.Trim().ToLower();
                        var listData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().Where(o => o.SUPPLIER_CODE.ToLower().Contains(code)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            showCbo = false;
                            txtSupplierCode.Text = listData.First().SUPPLIER_CODE;
                            cboSupplier.EditValue = listData.First().ID;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (showCbo)
                    {
                        cboSupplier.Focus();
                        cboSupplier.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSupplier_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboSupplier.EditValue != null)
                    {
                        var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                            <MOS.EFMODEL.DataModels.HIS_SUPPLIER>().FirstOrDefault
                            (o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtSupplierCode.Text = data.SUPPLIER_CODE;
                            cboSupplier.Properties.Buttons[1].Visible = true;
                            SendKeys.Send("{TAB}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableLeftControl(bool Enable)
        {
            try
            {
                spinImpMoreRatio.Enabled = Enable;
                txtConcentra.Enabled = Enable;
                txtManufacture.Enabled = Enable;
                cboNational.Enabled = Enable;
                cboManufacture.Enabled = Enable;
                txtNationalMainText.Enabled = Enable;
                txtRegisterNumber.Enabled = Enable;
                spinDayLifeSpan.Enabled = Enable;
                spinHourLifeSpan.Enabled = Enable;
                spinMonthLifeSpan.Enabled = Enable;
                txtPackingType.Enabled = Enable;
                txtTenBHYT.Enabled = Enable;
                cboDosageForm.Enabled = Enable;
                txtActiveBhyt.Enabled = Enable;
                cboMediUseForm.Enabled = Enable;
                txtTenTT.Enabled = Enable;
                txtMaDT.Enabled = Enable;
                txtMaTT.Enabled = Enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetLeftControl()
        {
            try
            {
                spinImpMoreRatio.EditValue = null;
                txtConcentra.Text = "";
                txtManufacture.Text = "";
                txtNationalMainText.Text = "";
                cboNational.EditValue = null;
                cboManufacture.EditValue = null;
                txtRegisterNumber.Text = "";
                spinDayLifeSpan.EditValue = null;
                spinHourLifeSpan.EditValue = null;
                spinMonthLifeSpan.EditValue = null;
                txtMaDT.Text = "";
                txtMaTT.Text = "";
                txtTenTT.Text = "";
                txtNOTE.Text = "";
                txtPackingType.Text = "";
                txtTenBHYT.Text = "";
                cboDosageForm.Text = "";
                txtActiveBhyt.Text = "";
                cboMediUseForm.EditValue = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                ResetLeftControl();
                if (xtraTabControl1.SelectedTabPageIndex == 0) // thuoc
                {
                    ValidMaxlengthDosageForm();
                    txtBidPackageCode.Properties.MaxLength = 4;
                    medicineTypeProcessor.FocusKeyword(ucMedicineType);
                    EnableLeftControl(true);
                    if (spinImpMoreRatio.EditValue == null)
                        spinImpMoreRatio.EditValue = 0;
                    txtMaTT.Enabled = false;
                    txtMaDT.Enabled = false;
                    txtTenTT.Enabled = false;
                    layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    cboDosageForm.Enabled = true;
                    cboInformationBid.SelectedIndex = -1;
                    lciTenBHYT.Size = lciMaTT.Size;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1) // vat tu
                {

                    dxValidationProviderLeft.RemoveControlError(cboDosageForm);
                    dxValidationProviderLeft.SetValidationRule(cboDosageForm, null);
                    txtBidPackageCode.Properties.MaxLength = 4;
                    EnableLeftControl(true);
                    if (spinImpMoreRatio.EditValue == null)
                        spinImpMoreRatio.EditValue = 0;
                    txtRegisterNumber.Enabled = false;
                    txtPackingType.Enabled = false;
                    txtTenBHYT.Enabled = false;
                    cboDosageForm.Enabled = false;
                    txtActiveBhyt.Enabled = false;
                    cboMediUseForm.Enabled = false;
                    if (tabMaterial)
                    {
                        WaitingManager.Show();
                        await taskMaterialType;
                        this.FillDataMaterial();
                        WaitingManager.Hide();
                        tabMaterial = false;
                    }
                    materialTypeProcessor.FocusKeyword(ucMaterialType);
                    layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem26.Size = lciMaDT.Size;
                    lciTenBHYT.Size = lciMaTT.Size;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2) // Mau
                {
                    dxValidationProviderLeft.RemoveControlError(cboDosageForm);
                    dxValidationProviderLeft.SetValidationRule(cboDosageForm, null);
                    txtBidPackageCode.Properties.MaxLength = 4;
                    EnableLeftControl(false);
                    spinImpMoreRatio.EditValue = null;
                    if (tabBlood)
                    {
                        WaitingManager.Show();
                        await taskBloodType;
                        this.bloodTypeProcessor.Reload(this.ucBloodType, listHisBloodType);
                        WaitingManager.Hide();
                        tabBlood = false;
                    }

                    cboDosageForm.Enabled = false;
                    bloodTypeProcessor.FocusKeyword(ucBloodType);
                    layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    cboInformationBid.SelectedIndex = -1;
                    lciTenBHYT.Size = lciMaTT.Size;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void FillDataMaterial()
        {
            try
            {
                if (chkIsOnlyShowByBusiness.Checked)
                {
                    if (this.currentStock != null && this.currentStock.IS_BUSINESS == (short)1)
                    {
                        listHisMaterialType = listHisMaterialType != null ? listHisMaterialType.Where(o => o.IS_BUSINESS == (short)1).ToList() : null;
                    }
                }

                this.materialTypeProcessor.Reload(this.ucMaterialType, listHisMaterialType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpPrice.Focus();
                    spinImpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpVat.Focus();
                    spinImpVat.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpVat_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBidNumOrder.Focus();
                    txtBidNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBidNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSupplierCode.Focus();
                    txtSupplierCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderLeft_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleLeft == -1)
                {
                    positionHandleLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleLeft > edit.TabIndex)
                {
                    positionHandleLeft = edit.TabIndex;
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

        private void dxValidationProviderRight_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleRight == -1)
                {
                    positionHandleRight = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleRight > edit.TabIndex)
                {
                    positionHandleRight = edit.TabIndex;
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

        private void txtBidYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar))
                {
                    if (!Char.IsControl(e.KeyChar))
                    {
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBidYear_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtFromTime.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnAdd.Enabled && lciBtnAdd.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnUpdate.Enabled && lciBtnUpdate.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    btnUpdate_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion

        private void dtFromTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar))
                {
                    if (!Char.IsControl(e.KeyChar))
                    {
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion

        private void dtFromTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtToTime.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtToTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar))
                {
                    if (!Char.IsControl(e.KeyChar))
                    {
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtToTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBID.Focus();
                    txtBID.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtRegisterNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtNationalMainText.Visible)
                    {
                        txtNationalMainText.Focus();
                        txtNationalMainText.SelectAll();
                    }
                    else
                    {
                        cboNational.Focus();
                        cboNational.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNationalCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditNational.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNational_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtNationalMainText.Text = "";
                    cboNational.EditValue = null;
                    cboNational.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNational_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboNational.EditValue != null)
                    {
                        var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                            <SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault
                            (o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboNational.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtNationalMainText.Text = data.NATIONAL_NAME;
                            cboNational.Properties.Buttons[1].Visible = true;
                            if (txtConcentra.Enabled)
                            {
                                txtConcentra.Focus();
                                txtConcentra.SelectAll();
                            }
                            else
                            {
                                txtManufacture.Focus();
                                txtManufacture.SelectAll();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConcentra_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtManufacture.Focus();
                    txtManufacture.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtManufacture_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtManufacture.Text.Trim()))
                    {
                        string code = txtManufacture.Text.Trim().ToLower();
                        var listData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>().Where(o => o.MANUFACTURER_CODE.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.MANUFACTURER_CODE.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtManufacture.Text = result.First().MANUFACTURER_CODE;
                            cboManufacture.EditValue = result.First().ID;
                            if (spinMonthLifeSpan.Enabled)
                            {
                                spinMonthLifeSpan.Focus();
                                spinMonthLifeSpan.SelectAll();
                            }
                            else
                            {
                                btnAdd.Focus();
                                e.Handled = true;
                            }
                        }
                    }
                    if (showCbo)
                    {
                        cboManufacture.Focus();
                        cboManufacture.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboManufacture_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboManufacture.EditValue != null)
                    {
                        var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                            <MOS.EFMODEL.DataModels.HIS_MANUFACTURER>().FirstOrDefault
                            (o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboManufacture.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtManufacture.Text = data.MANUFACTURER_CODE;
                            cboManufacture.Properties.Buttons[1].Visible = true;
                            if (spinMonthLifeSpan.Enabled)
                            {
                                spinMonthLifeSpan.Focus();
                                spinMonthLifeSpan.SelectAll();
                            }
                            else
                            {
                                btnSave.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditNational_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditNational.CheckState == CheckState.Checked)
                {
                    cboNational.Visible = false;
                    txtNationalMainText.Visible = true;
                    if (!String.IsNullOrEmpty(cboNational.Text))
                    {
                        txtNationalMainText.Text = cboNational.Text;
                    }

                    txtNationalMainText.Focus();
                    txtNationalMainText.SelectAll();
                }
                else if (chkEditNational.CheckState == CheckState.Unchecked)
                {
                    txtNationalMainText.Visible = false;
                    cboNational.Visible = true;
                    txtNationalMainText.Text = cboNational.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboManufacture_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboManufacture.EditValue = null;
                    txtManufacture.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DtExpiredDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtMaTT.Enabled)
                    {
                        txtMaTT.Focus();
                        txtMaTT.SelectAll();
                    }
                    else if (txtTenBHYT.Enabled)
                    {
                        txtTenBHYT.Focus();
                        txtTenBHYT.SelectAll();
                    }
                    else if (txtRegisterNumber.Enabled)
                    {
                        txtRegisterNumber.Focus();
                        txtRegisterNumber.SelectAll();
                    }
                    else if (cboNational.Visible)
                    {
                        cboNational.Focus();
                        cboNational.ShowPopup();
                    }
                    else
                    {
                        txtNationalMainText.Focus();
                        txtNationalMainText.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditNational_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtConcentra.Focus();
                    txtConcentra.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkClearToAdd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAdd = (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0) ? this.currentControlStateRDO__Create.Where(o => o.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_CREATE).FirstOrDefault() : null;

                HIS.Desktop.Library.CacheClient.ControlStateRDO csUpdate = (this.currentControlStateRDO__Update != null && this.currentControlStateRDO__Update.Count > 0) ? this.currentControlStateRDO__Update.Where(o => o.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_UPDATE).FirstOrDefault() : null;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAdd), csAdd));
                if (csAdd != null)
                {
                    csAdd.VALUE = (chkClearToAdd.Checked ? "1" : "");
                }
                else if (csUpdate != null)
                {
                    csUpdate.VALUE = (chkClearToAdd.Checked ? "1" : "");
                }
                else
                {
                    csAdd = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAdd.KEY = ControlStateConstant.CHECK_CLEAR_TO_ADD;
                    csAdd.VALUE = (chkClearToAdd.Checked ? "1" : "");
                    csAdd.MODULE_LINK = ControlStateConstant.MODULE_LINK_CREATE;
                    if (this.currentControlStateRDO__Create == null)
                        this.currentControlStateRDO__Create = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Create.Add(csAdd);

                    csUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csUpdate.KEY = ControlStateConstant.CHECK_CLEAR_TO_ADD;
                    csUpdate.VALUE = (chkClearToAdd.Checked ? "1" : "");
                    csUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK_UPDATE;
                    if (this.currentControlStateRDO__Update == null)
                        this.currentControlStateRDO__Update = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Update.Add(csUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO__Create);
                this.controlStateWorker.SetData(this.currentControlStateRDO__Update);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsOnlyShowByBusiness_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                {
                    return;
                }
                WaitingManager.Show();
                if (xtraTabControl1.SelectedTabPageIndex == 0 || xtraTabControl1.SelectedTabPageIndex == 2)
                {
                    tabMaterial = true;
                    this.FillDataToGrid();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1)
                {
                    this.FillDataToGrid();
                    this.FillDataMaterial();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinMonthLifeSpan_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!spinMonthLifeSpan.Enabled) return;
                if (spinMonthLifeSpan.EditValue != null)
                {
                    spinDayLifeSpan.Enabled = false;
                    spinHourLifeSpan.Enabled = false;
                    spinDayLifeSpan.EditValue = null;
                    spinHourLifeSpan.EditValue = null;
                }
                else
                {
                    spinDayLifeSpan.Enabled = true;
                    spinHourLifeSpan.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDayLifeSpan_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!spinDayLifeSpan.Enabled) return;
                if (spinDayLifeSpan.EditValue != null)
                {
                    spinMonthLifeSpan.Enabled = false;
                    spinHourLifeSpan.Enabled = false;
                    spinMonthLifeSpan.EditValue = null;
                    spinHourLifeSpan.EditValue = null;
                }
                else
                {
                    spinMonthLifeSpan.Enabled = true;
                    spinHourLifeSpan.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinHourLifeSpan_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!spinHourLifeSpan.Enabled) return;
                if (spinHourLifeSpan.EditValue != null)
                {
                    spinDayLifeSpan.Enabled = false;
                    spinMonthLifeSpan.Enabled = false;
                    spinDayLifeSpan.EditValue = null;
                    spinMonthLifeSpan.EditValue = null;
                }
                else
                {
                    spinDayLifeSpan.Enabled = true;
                    spinMonthLifeSpan.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinMonthLifeSpan_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinDayLifeSpan.Enabled)
                    {
                        spinDayLifeSpan.Focus();
                        spinDayLifeSpan.SelectAll();
                    }
                    else if (spinHourLifeSpan.Enabled)
                    {
                        spinHourLifeSpan.Focus();
                        spinHourLifeSpan.SelectAll();
                    }
                    else
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDayLifeSpan_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinHourLifeSpan.Enabled)
                    {
                        spinHourLifeSpan.Focus();
                        spinHourLifeSpan.SelectAll();
                    }
                    else
                    {
                        btnAdd.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinHourLifeSpan_KeyDown(object sender, KeyEventArgs e)
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

        private void txtTenBHYT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPackingType.Focus();
                    txtPackingType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackingType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtActiveBhyt.Focus();
                    txtActiveBhyt.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtActiveBhyt_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMediUseForm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediUseForm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboDosageForm.Focus();
                    cboDosageForm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediUseForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMediUseForm.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDosageForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRegisterNumber.Focus();
                    txtRegisterNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMaTT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTenTT.Focus();
                    txtTenTT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTenTT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaDT.Focus();
                    txtMaDT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBID_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboBidForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboBidForm.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInformationBid_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboInformationBid.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDosageForm_Properties_ButtonClick_1(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Plus)
            {
                try
                {
                    Inventec.Desktop.Common.Modules.Module Module = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisDosageForm").FirstOrDefault();
                    if (Module == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisDosageForm");
                    if (Module.IsPlugin && Module.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.bid_id);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(Module, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                        LoaddatatoCboDosageForm();

                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                cboDosageForm.EditValue = null;
            }
        }

        private void cboInformationBid_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                cboDosageForm.EditValue = null;
            }
        }

        private void txtMaDT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboNational.Enabled)
                    {
                        cboNational.Focus();
                    }
                    else
                    {
                        txtNationalMainText.Focus();
                        txtNationalMainText.SelectAll();
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
