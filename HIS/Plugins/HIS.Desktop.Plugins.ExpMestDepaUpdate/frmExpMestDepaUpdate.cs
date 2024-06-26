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
using MOS.EFMODEL.DataModels;
using HIS.UC.ExpMestMaterialGrid.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using HIS.Desktop.Plugins.ExpMestDepaUpdate.Validation;
using MOS.SDO;
using HIS.Desktop.Plugins.ExpMestDepaUpdate.ADO;
using DevExpress.Utils.Menu;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestDepaUpdate.ADO;
using HIS.Desktop.Plugins.ExpMestDepaUpdate.Validation;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using HIS.Desktop.LibraryMessage;

namespace HIS.Desktop.Plugins.ExpMestDepaUpdate
{
    public partial class frmExpMestDepaUpdate : HIS.Desktop.Utility.FormBase
    {
        MedicineTypeInStockProcessor mediInStockProcessor = null;
        MaterialTypeInStockTreeProcessor mateInStockProcessor = null;
        MaterialTypeInStockTreeProcessor mateHCInStockProcessor = null;

        UserControl ucMediInStock = null;
        UserControl ucMateInStock = null;
        UserControl ucMateHCInStock = null;

        V_HIS_MEDI_STOCK currentMediStock = null;
        List<HisMedicineTypeInStockSDO> listMedicineInStocks;
        List<HisMaterialTypeInStockSDO> listMaterialInStocks;
        List<HisMaterialTypeInStockSDO> listMaterialHCInStocks;

        Dictionary<long, HisMedicineTypeInStockSDO> dicExpMetyInStock = new Dictionary<long, HisMedicineTypeInStockSDO>();
        Dictionary<long, HisMaterialTypeInStockSDO> dicExpMatyInStock = new Dictionary<long, HisMaterialTypeInStockSDO>();

        Dictionary<long, MediMateTypeADO> dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();
        MediMateTypeADO currentMediMate = null;

        HisExpMestResultSDO resultSdo = null;
        V_HIS_EXP_MEST hisExpMest;
        HIS_EXP_MEST hisDepaExpMest;

        Inventec.Desktop.Common.Modules.Module currentModule;

        bool isUpdate = false;

        bool isSupplement = false;

        long roomId;
        long roomTypeId;

        int positionHandleControl = -1;

        public frmExpMestDepaUpdate(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_EXP_MEST expMest)
		:base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.Text = currentModule.text;
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = currentModule;
                this.roomTypeId = currentModule.RoomTypeId;
                this.roomId = currentModule.RoomId;
                this.hisExpMest = expMest;
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
                var langManager = Base.ResourceLangManager.LanguageUCExpMestChmsCreate;
                this.mediInStockProcessor = new MedicineTypeInStockProcessor();
                MedicineTypeInStockInitADO ado = new MedicineTypeInStockInitADO();
                ado.IsShowButtonAdd = false;
                ado.IsShowCheckNode = false;
                ado.IsShowSearchPanel = true;
                ado.IsAutoWidth = true;
                ado.MedicineTypeInStockClick = medicineInStockTree_CLick;
                ado.MedicineTypeInStockRowEnter = medicineInStockTree_RowEnter;
                ado.MedicineTypeInStockColumns = new List<MedicineTypeInStockColumn>();
                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__TXT_KEYWORD__NULL_VALUE", langManager, culture);

                MedicineTypeInStockColumn colMedicineTypeCode = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MEDICINE_TREE__COLUMN_MEDICINE_TYPE_CODE", langManager, culture), "MedicineTypeCode", 70, false);
                colMedicineTypeCode.VisibleIndex = 0;
                ado.MedicineTypeInStockColumns.Add(colMedicineTypeCode);

                MedicineTypeInStockColumn colMedicineTypeName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MEDICINE_TREE__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "MedicineTypeName", 250, false);
                colMedicineTypeName.VisibleIndex = 1;
                ado.MedicineTypeInStockColumns.Add(colMedicineTypeName);

                MedicineTypeInStockColumn colServiceUnitName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MEDICINE_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 50, false);
                colServiceUnitName.VisibleIndex = 2;
                ado.MedicineTypeInStockColumns.Add(colServiceUnitName);

                MedicineTypeInStockColumn colAvailableAmount = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MEDICINE_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 100, false);
                colAvailableAmount.VisibleIndex = 3;
                colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
                colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAvailableAmount.Format.FormatString = "#,##0.00";
                ado.MedicineTypeInStockColumns.Add(colAvailableAmount);

                MedicineTypeInStockColumn colNationalName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MEDICINE_TREE__COLUMN_NATIONAL_NAME", langManager, culture), "NationalName", 100, false);
                colNationalName.VisibleIndex = 4;
                ado.MedicineTypeInStockColumns.Add(colNationalName);

                MedicineTypeInStockColumn colManufacturerName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MEDICINE_TREE__COLUMN_MANUFACTURER_NAME", langManager, culture), "ManufacturerName", 120, false);
                colManufacturerName.VisibleIndex = 5;
                ado.MedicineTypeInStockColumns.Add(colManufacturerName);

                MedicineTypeInStockColumn colActiveIngrBhytName = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MEDICINE_TREE__COLUMN_ACTIVE_INGR_BHYT_NAME", langManager, culture), "ActiveIngrBhytName", 120, false);
                colActiveIngrBhytName.VisibleIndex = 6;
                ado.MedicineTypeInStockColumns.Add(colActiveIngrBhytName);

                MedicineTypeInStockColumn colRegisterNumber = new MedicineTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MEDICINE_TREE__COLUMN_REGISTER_NUMBER", langManager, culture), "RegisterNumber", 70, false);
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
                var langManager = Base.ResourceLangManager.LanguageUCExpMestChmsCreate;

                this.mateInStockProcessor = new MaterialTypeInStockTreeProcessor();
                MaterialTypeInStockInitADO ado = new MaterialTypeInStockInitADO();
                ado.IsShowButtonAdd = false;
                ado.IsShowCheckNode = false;
                ado.IsShowSearchPanel = true;
                ado.IsAutoWidth = true;
                ado.MaterialTypeInStockClick = materialInStockTree_Click;
                ado.MaterialTypeInStockRowEnter = materialInStockTree_EnterRow;
                ado.MaterialTypeInStockColumns = new List<MaterialTypeInStockColumn>();
                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__TXT_KEYWORD__NULL_VALUE", langManager, culture);

                MaterialTypeInStockColumn colMaterialTypeCode = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_CODE", langManager, culture), "MaterialTypeCode", 80, false);
                colMaterialTypeCode.VisibleIndex = 0;
                ado.MaterialTypeInStockColumns.Add(colMaterialTypeCode);

                MaterialTypeInStockColumn colMaterialTypeName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MaterialTypeName", 300, false);
                colMaterialTypeName.VisibleIndex = 1;
                ado.MaterialTypeInStockColumns.Add(colMaterialTypeName);

                MaterialTypeInStockColumn colServiceUnitName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 60, false);
                colServiceUnitName.VisibleIndex = 2;
                ado.MaterialTypeInStockColumns.Add(colServiceUnitName);

                MaterialTypeInStockColumn colAvailableAmount = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 110, false);
                colAvailableAmount.VisibleIndex = 3;
                colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
                colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAvailableAmount.Format.FormatString = "#,##0.00";
                ado.MaterialTypeInStockColumns.Add(colAvailableAmount);

                MaterialTypeInStockColumn colNationalName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_NATIONAL_NAME", langManager, culture), "NationalName", 150, false);
                colNationalName.VisibleIndex = 4;
                ado.MaterialTypeInStockColumns.Add(colNationalName);

                MaterialTypeInStockColumn colManufacturerName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_MANUFACTURER_NAME", langManager, culture), "ManufacturerName", 200, false);
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

        private void InitMaterialHCTree()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Base.ResourceLangManager.LanguageUCExpMestChmsCreate;

                this.mateHCInStockProcessor = new MaterialTypeInStockTreeProcessor();
                MaterialTypeInStockInitADO ado = new MaterialTypeInStockInitADO();
                ado.IsShowButtonAdd = false;
                ado.IsShowCheckNode = false;
                ado.IsShowSearchPanel = true;
                ado.IsAutoWidth = true;
                ado.MaterialTypeInStockClick = materialHCInStockTree_Click;
                ado.MaterialTypeInStockRowEnter = materialHCInStockTree_EnterRow;
                ado.MaterialTypeInStockColumns = new List<MaterialTypeInStockColumn>();
                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__TXT_KEYWORD__NULL_VALUE", langManager, culture);

                MaterialTypeInStockColumn colMaterialTypeCode = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_CODE", langManager, culture), "MaterialTypeCode", 80, false);
                colMaterialTypeCode.VisibleIndex = 0;
                ado.MaterialTypeInStockColumns.Add(colMaterialTypeCode);

                MaterialTypeInStockColumn colMaterialTypeName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MaterialTypeName", 300, false);
                colMaterialTypeName.VisibleIndex = 1;
                ado.MaterialTypeInStockColumns.Add(colMaterialTypeName);

                MaterialTypeInStockColumn colServiceUnitName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 60, false);
                colServiceUnitName.VisibleIndex = 2;
                ado.MaterialTypeInStockColumns.Add(colServiceUnitName);

                MaterialTypeInStockColumn colAvailableAmount = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 110, false);
                colAvailableAmount.VisibleIndex = 3;
                colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
                colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAvailableAmount.Format.FormatString = "#,##0.00";
                ado.MaterialTypeInStockColumns.Add(colAvailableAmount);

                MaterialTypeInStockColumn colNationalName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_NATIONAL_NAME", langManager, culture), "NationalName", 150, false);
                colNationalName.VisibleIndex = 4;
                ado.MaterialTypeInStockColumns.Add(colNationalName);

                MaterialTypeInStockColumn colManufacturerName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__MATERIAL_TREE__COLUMN_MANUFACTURER_NAME", langManager, culture), "ManufacturerName", 200, false);
                colManufacturerName.VisibleIndex = 5;
                ado.MaterialTypeInStockColumns.Add(colManufacturerName);

                this.ucMateHCInStock = (UserControl)this.mateHCInStockProcessor.Run(ado);
                if (this.ucMateHCInStock != null)
                {
                    this.xtraTabPageMaterialHC.Controls.Add(this.ucMateHCInStock);
                    this.ucMateHCInStock.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmExpMestDepaUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                InitMedicineTree();
                InitMaterialTree();
                InitMaterialHCTree();
                SetIcon();
                ResetValueControlDetail();
                WaitingManager.Show();
                LoadKeyUCLanguage();
                ValidControl();
                LoadChmsExpMest();
                ddBtnPrint.Enabled = false;
                if (this.hisDepaExpMest != null)
                {
                    LoadDataToComboExpMediStock();
                    LoadDataToComboExpMestReason();
                    LoadListCurrentImpMediStock();
                    LoadDataToTreeListBegin(hisExpMest);
                    GetDataExpMest(hisExpMest);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadChmsExpMest()
        {
            try
            {
                if (this.hisExpMest != null)
                {
                    HisExpMestFilter chmsFilter = new HisExpMestFilter();
                    chmsFilter.ID = this.hisExpMest.ID;
                    var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, chmsFilter, null);
                    if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                        throw new Exception("khong lay duoc EXP_MEST theo ExpMestId: " + this.hisExpMest.ID);
                    this.hisDepaExpMest = listChmsExpMest.First();

                    this.resultSdo = new HisExpMestResultSDO();
                    this.resultSdo.ExpMest = this.hisDepaExpMest;

                    cboExpMediStock.EditValue = this.hisExpMest.MEDI_STOCK_ID;
                    txtReqDepartmentName.Text = this.hisExpMest.REQ_DEPARTMENT_NAME;
                    txtReqRoomName.Text = this.hisExpMest.REQ_ROOM_NAME;
                    if (!String.IsNullOrEmpty(this.hisExpMest.REQ_USERNAME))
                    {
                        txtReqUsername.Text = this.hisExpMest.REQ_USERNAME;
                    }
                    else
                    {
                        txtReqUsername.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    }
                    txtDescription.Text = hisExpMest.DESCRIPTION;
                    cboExpMestReason.EditValue = hisExpMest.EXP_MEST_REASON_ID;
                    spinRemedyCount.EditValue = hisDepaExpMest.REMEDY_COUNT;
                }
                else
                {
                    txtReqDepartmentName.Text = "";
                    txtReqRoomName.Text = "";
                    txtReqUsername.Text = "";
                    txtDescription.Text = "";
                    spinRemedyCount.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataExpMest(V_HIS_EXP_MEST expMest)
        {
            try
            {
                dicMediMateAdo = new Dictionary<long, ADO.MediMateTypeADO>();
                //Medicine
                MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                metyReqFilter.EXP_MEST_ID = expMest.ID;
                var _ExpMestMetyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, null);
                if (_ExpMestMetyReqs != null && _ExpMestMetyReqs.Count > 0)
                {
                    //  var dataGroups = _ExpMestMetyReqs.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    string message = "";

                    foreach (var item in _ExpMestMetyReqs)
                    {
                        if (dicExpMetyInStock.ContainsKey(item.MEDICINE_TYPE_ID))
                        {
                            var data = dicExpMetyInStock[item.MEDICINE_TYPE_ID];
                            MediMateTypeADO ado = new MediMateTypeADO(data);
                            ado.EXP_AMOUNT = item.AMOUNT;
                            ado.DESCRIPTION = item.DESCRIPTION;
                            ado.NUM_ORDER = item.NUM_ORDER;
                            ado.MEDI_MATE_REQ_ID = item.ID;
                            ado.AVAILABLE_AMOUNT = data.AvailableAmount;
                            dicMediMateAdo[ado.SERVICE_ID] = ado;
                        }
                        else
                        {
                            var medi = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            message += medi != null ? medi.MEDICINE_TYPE_NAME : "";
                        }
                    }
                    if (message != "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Thuốc " + message + "không có trong kho xuất");
                    }
                }

                //Material
                MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                matyReqFilter.EXP_MEST_ID = expMest.ID;
                var _ExpMestMatyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, null);
                if (_ExpMestMatyReqs != null && _ExpMestMatyReqs.Count > 0)
                {
                    string message = "";

                    foreach (var item in _ExpMestMatyReqs)
                    {
                        if (dicExpMatyInStock.ContainsKey(item.MATERIAL_TYPE_ID))
                        {
                            var data = dicExpMatyInStock[item.MATERIAL_TYPE_ID];
                            MediMateTypeADO ado = new MediMateTypeADO(data);
                            ado.EXP_AMOUNT = item.AMOUNT;
                            ado.DESCRIPTION = item.DESCRIPTION;
                            ado.NUM_ORDER = item.NUM_ORDER;
                            ado.MEDI_MATE_REQ_ID = item.ID;
                            ado.AVAILABLE_AMOUNT = data.AvailableAmount;

                            var hc = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                            if (hc != null && hc.IS_CHEMICAL_SUBSTANCE == 1)
                            {
                                ado.IsHoaChat = true;
                            }
                            dicMediMateAdo[ado.SERVICE_ID] = ado;
                        }
                        else
                        {
                            var mate = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                            message += mate != null ? mate.MATERIAL_TYPE_NAME : "";
                        }
                    }
                    if (message != "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Vật tư " + message + "không có trong kho xuất");
                    }
                }
                this.lciRemedyCount.Enabled = !(dicMediMateAdo.Select(o => o.Value).ToList().Exists(o => !o.IsMedicine));
                gridControlExpMestChmsDetail.BeginUpdate();
                gridControlExpMestChmsDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpMestChmsDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataToTreeListBegin(V_HIS_EXP_MEST hisExpMest)
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
                listMedicineInStocks = new List<HisMedicineTypeInStockSDO>();
                listMaterialInStocks = new List<HisMaterialTypeInStockSDO>();
                dicExpMatyInStock = new Dictionary<long, HisMaterialTypeInStockSDO>();
                dicExpMetyInStock = new Dictionary<long, HisMedicineTypeInStockSDO>();
                if (hisExpMest != null)
                {
                    HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                    mediFilter.MEDI_STOCK_ID = hisExpMest.MEDI_STOCK_ID;
                    mediFilter.IS_LEAF = true;
                    mediFilter.IS_AVAILABLE = true;
                    mediFilter.IS_ACTIVE = true;
                    listMedicineInStocks = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, null);
                    if (listMedicineInStocks != null && listMedicineInStocks.Count > 0)
                    {
                        var mestRoom = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == hisExpMest.MEDI_STOCK_ID);
                        if (mestRoom != null && mestRoom.IS_BUSINESS == 1)
                        {
                            var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                            if (dataMediTypes != null && dataMediTypes.Count > 0)
                            {
                                listMedicineInStocks = listMedicineInStocks.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMedicineInStocks = new List<HisMedicineTypeInStockSDO>();
                            }
                        }
                        else
                        {
                            var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                            if (dataMediTypes != null && dataMediTypes.Count > 0)
                            {
                                listMedicineInStocks = listMedicineInStocks.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMedicineInStocks = new List<HisMedicineTypeInStockSDO>();
                            }
                        }
                    }

                    HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                    mateFilter.MEDI_STOCK_ID = hisExpMest.MEDI_STOCK_ID;
                    mateFilter.IS_LEAF = true;
                    mateFilter.IS_AVAILABLE = true;
                    mateFilter.IS_ACTIVE = true;
                    listMaterialInStocks = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mateFilter, null);

                    if (listMaterialInStocks != null && listMaterialInStocks.Count > 0)
                    {
                        var mestRoom = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == hisExpMest.MEDI_STOCK_ID);
                        if (mestRoom != null && mestRoom.IS_BUSINESS == 1)
                        {
                            var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                            if (dataMateTypes != null && dataMateTypes.Count > 0)
                            {
                                listMaterialInStocks = listMaterialInStocks.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMaterialInStocks = new List<HisMaterialTypeInStockSDO>();
                            }
                        }
                        else
                        {
                            var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                            if (dataMateTypes != null && dataMateTypes.Count > 0)
                            {
                                listMaterialInStocks = listMaterialInStocks.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMaterialInStocks = new List<HisMaterialTypeInStockSDO>();
                            }
                        }
                    }
                }

                var metyDepa = BackendDataWorker.Get<HIS_MEST_METY_DEPA>();
                if (metyDepa != null && metyDepa.Count > 0)
                {
                    metyDepa = metyDepa.Where(o => o.DEPARTMENT_ID == hisExpMest.REQ_DEPARTMENT_ID && (!o.MEDI_STOCK_ID.HasValue || o.MEDI_STOCK_ID == hisExpMest.MEDI_STOCK_ID) && o.IS_JUST_PRESCRIPTION != 1).ToList();
                }

                if (metyDepa != null && metyDepa.Count > 0)
                {
                    listMedicineInStocks = listMedicineInStocks.Where(o => !metyDepa.Select(s => s.MEDICINE_TYPE_ID).Contains(o.Id)).ToList();
                }

                var matyDepa = BackendDataWorker.Get<HIS_MEST_MATY_DEPA>();
                if (matyDepa != null && matyDepa.Count > 0)
                {
                    matyDepa = matyDepa.Where(o => o.DEPARTMENT_ID == hisExpMest.REQ_DEPARTMENT_ID && (!o.MEDI_STOCK_ID.HasValue || o.MEDI_STOCK_ID == hisExpMest.MEDI_STOCK_ID) && o.IS_JUST_PRESCRIPTION != 1).ToList();
                }

                if (matyDepa != null && matyDepa.Count > 0)
                {
                    listMaterialInStocks = listMaterialInStocks.Where(o => !matyDepa.Select(s => s.MATERIAL_TYPE_ID).Contains(o.Id)).ToList();
                }

                if (listMedicineInStocks != null)
                {
                    foreach (var item in listMedicineInStocks)
                    {
                        dicExpMetyInStock[item.Id] = item;
                    }
                }
                List<HisMaterialTypeInStockSDO> listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                List<HisMaterialTypeInStockSDO> listMateTypeInStock_HCs = new List<HisMaterialTypeInStockSDO>();
                if (listMaterialInStocks != null)
                {
                    foreach (var item in listMaterialInStocks)
                    {
                        dicExpMatyInStock[item.Id] = item;

                        var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.Id);

                        if (key == 1 && data != null && data.IS_CHEMICAL_SUBSTANCE == 1)
                        {
                            listMateTypeInStock_HCs.Add(item);
                        }
                        else
                            listMateTypeInStock.Add(item);
                    }
                }
                this.mediInStockProcessor.Reload(this.ucMediInStock, listMedicineInStocks);
                this.mateInStockProcessor.Reload(this.ucMateInStock, listMateTypeInStock);
                this.mateHCInStockProcessor.Reload(this.ucMateHCInStock, listMateTypeInStock_HCs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadListCurrentImpMediStock()
        {
            try
            {
                currentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_TYPE_ID == this.roomTypeId && o.ROOM_ID == this.roomId);
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
                spinExpAmount.Value = 0;
                txtNote.Text = "";
                SetEnableButton(false);
                if (this.currentMediMate != null && !isSupplement)
                {
                    btnAddd.Enabled = true;
                    spinExpAmount.Focus();
                    spinExpAmount.SelectAll();
                }
                else
                {
                    btnAddd.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetGridControlDetail()
        {
            try
            {
                dicMediMateAdo.Clear();
                gridControlExpMestChmsDetail.BeginUpdate();
                gridControlExpMestChmsDetail.DataSource = null;
                gridControlExpMestChmsDetail.EndUpdate();
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
                this.isSupplement = false;
                dicMediMateAdo.Clear();
                this.currentMediMate = null;
                this.resultSdo = null;
                isUpdate = false;
                ResetValueControlDetail();
                ResetGridControlDetail();
                ddBtnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboExpMestReason()
        {
            try
            {
                //cboExpMestReason.Properties.Buttons[1].Visible = false;
                var reason = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON>().Where(p => p.IS_DEPA == 1 && p.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboExpMestReason, reason, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboExpMediStock()
        {
            try
            {
                cboExpMediStock.Properties.DataSource = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                cboExpMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboExpMediStock.Properties.ValueMember = "ID";
                cboExpMediStock.Properties.ForceInitialize();
                cboExpMediStock.Properties.Columns.Clear();
                cboExpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 50));
                cboExpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboExpMediStock.Properties.ShowHeader = false;
                cboExpMediStock.Properties.ImmediatePopup = true;
                cboExpMediStock.Properties.DropDownRows = 10;
                cboExpMediStock.Properties.PopupWidth = 250;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboMediStock()
        {
            try
            {
                //listImpMediStock = new List<V_HIS_MEDI_STOCK>();
                //listExpMediStock = new List<V_HIS_MEDI_STOCK>();
                //if (chkLinh.Checked)
                //{

                //    //Lúc sai cần sửa lại
                //    listExpMediStock = listCurrentMediStock;
                //    listImpMediStock = listCurrentMediStock;

                //    cboImpMediStock.EditValue = null;
                //    cboImpMediStock.Enabled = false;
                //    txtImpMediStock.Enabled = false;
                //    cboExpMediStock.EditValue = null;
                //    cboExpMediStock.Enabled = true;
                //    txtExpMediStock.Enabled = true;
                //    cboImpMediStock.Properties.DataSource = listImpMediStock;
                //    cboExpMediStock.Properties.DataSource = listExpMediStock;



                //    if (currentMediStock != null)
                //    {
                //        cboExpMediStock.EditValue = currentMediStock.ID;
                //        var listMediStockId = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == currentMediStock.ROOM_ID).Select(s => s.MEDI_STOCK_ID).ToList();
                //        if (listMediStockId != null && listMediStockId.Count > 0)
                //        {
                //            listExpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listMediStockId.Contains(o.ID)).ToList();
                //        }
                //    }

                //    cboImpMediStock.EditValue = hisExpMest.MEDI_STOCK_ID;
                //    LoadDataToTreeListBegin(hisExpMest);
                //    loadComboExpMediStock(hisExpMest);
                //    GetDataExpMest(hisExpMest);


                //    checkLoadMedicine = false;
                //    cboExpMediStock.Properties.DataSource = listExpMediStock;

                //}
                //else
                //{
                //    listExpMediStock = listCurrentMediStock;
                //    cboImpMediStock.EditValue = null;
                //    cboImpMediStock.Enabled = true;
                //    txtImpMediStock.Enabled = true;
                //    cboExpMediStock.EditValue = null;
                //    cboExpMediStock.Enabled = false;
                //    txtExpMediStock.Enabled = false;

                //    //Lúc sai cần sửa lại
                //    cboExpMediStock.Properties.DataSource = listExpMediStock;
                //    cboImpMediStock.Properties.DataSource = listImpMediStock;


                //    if (currentMediStock != null)
                //    {
                //        cboExpMediStock.EditValue = currentMediStock.ID;
                //        var listRoomId = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.MEDI_STOCK_ID == currentMediStock.ID).Select(s => s.ROOM_ID).ToList();
                //        if (listRoomId != null && listRoomId.Count > 0)
                //        {
                //            listImpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listRoomId.Contains(o.ROOM_ID)).ToList();
                //        }
                //    }

                //    cboExpMediStock.EditValue = hisExpMest.MEDI_STOCK_ID;
                //    LoadDataToTreeListBegin(hisExpMest);
                //    loadComboImpMediStock(hisExpMest);
                //    GetDataExpMest(hisExpMest);

                //    checkLoadMaterial = false;
                //    cboImpMediStock.Properties.DataSource = listImpMediStock;

                //}
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
                //Review
                //List<V_HIS_EXP_MEST_MEDICINE> listMedicine = null;
                //List<V_HIS_EXP_MEST_MATERIAL> listMaterial = null;
                //if (this.resultSdo != null)
                //{
                //    listMedicine = this.resultSdo.ExpMedicines;
                //    listMaterial = this.resultSdo.ExpMaterials;
                //}
                //if (listMedicine != null && listMedicine.Count > 0)
                //{
                //    listMedicine = listMedicine.OrderBy(o => o.ID).ToList();
                //}
                //if (listMaterial != null && listMaterial.Count > 0)
                //{
                //    listMaterial = listMaterial.OrderBy(o => o.ID).ToList();
                //}
                //this.expMestMediProcessor.Reload(this.ucExpMestMedi, listMedicine);
                //this.expMestMateProcessor.Reload(this.ucExpMestMate, listMaterial);
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
                long keyCFG_IsRequiredReason = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ExpMestDepaCreate_IsRequiredReason));
                string keyCFG_MOS_IsReasonRequired = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__MOS__EXP_MEST__IS_REASON_REQUIRED) ?? "";
                if (keyCFG_IsRequiredReason == 1 || keyCFG_MOS_IsReasonRequired.Trim() == "1")
                {
                    ValidControlIsRequiredReason();
                    lciExpMestReason.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    lciExpMestReason.AppearanceItemCaption.ForeColor = Color.Black;
                }

                ValidControlExpAmount();
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
                ExpMediStockValidationRule expMestRule = new ExpMediStockValidationRule();
                expMestRule.cboExpMediStock = cboExpMediStock;
                dxValidationProvider1.SetValidationRule(cboExpMediStock, expMestRule);
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
                ExpAmountValidationRule expAmountRule = new ExpAmountValidationRule();
                expAmountRule.spinExpAmount = spinExpAmount;
                dxValidationProvider2.SetValidationRule(spinExpAmount, expAmountRule);
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
                IsRequiredReason.cbo = cboExpMestReason;
                dxValidationProvider1.SetValidationRule(cboExpMestReason, IsRequiredReason);
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
                //Button
                this.btnAddd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_ADD", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                // this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_NEW", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_SAVE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                // this.btnUpdate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_UPDATE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_PRINT", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);


                //Layout
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_DESCRIPTION", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutExpAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_EXP_AMOUNT", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutExpMediStock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_EXP_MEDI_STOCK", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutNote.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_NOTE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);

                //GridControl Detail
                this.gridColumn_ExpMestChmsDetail_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_ManufactureName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_MANUFACTURER_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_MediMateTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_MEDI_MATE_TYPE_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_NationalName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_NATIONAL_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);


                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__XTRA_TAB_MATERIAL", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__XTRA_TAB_MEDICINE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);

                //Repository Button
                this.repositoryItemBtnDeleteDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__REPOSITORY_BTN_DELETE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
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
                gridViewExpMestChmsDetail.PostEditor();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnUpdate()
        {
            try
            {
                if (btnCapNhat.Enabled)
                {
                    gridViewExpMestChmsDetail.PostEditor();
                    btnCapNhat_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDeleteDetail_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (MediMateTypeADO)gridViewExpMestChmsDetail.GetFocusedRow();
                if (data != null)
                {
                    dicMediMateAdo.Remove(data.SERVICE_ID);
                }
                this.lciRemedyCount.Enabled = !(dicMediMateAdo.Select(o => o.Value).ToList().Exists(o => !o.IsMedicine)) && xtraTabControlMain.SelectedTabPageIndex == 0;
                gridControlExpMestChmsDetail.BeginUpdate();
                gridControlExpMestChmsDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpMestChmsDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Edit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    this.currentMediMate = null;
                    var data = (ADO.MediMateTypeADO)gridViewExpMestChmsDetail.GetFocusedRow();
                    if (data != null)
                    {
                        this.currentMediMate = data;
                        spinExpAmount.Value = data.EXP_AMOUNT;
                        txtNote.Text = data.DESCRIPTION;
                        SetEnableButton(true);
                        if (data.IsMedicine)
                        {
                            xtraTabControlMain.SelectedTabPageIndex = 0;
                        }
                        else
                        {
                            // long key = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__TACH_VAT_TU_HOA_CHAT));
                            if (xtraTabControlMain.TabPages[2].PageVisible && data.IsHoaChat)
                            {
                                xtraTabControlMain.SelectedTabPageIndex = 2;
                            }
                            else
                                xtraTabControlMain.SelectedTabPageIndex = 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnHuy.Enabled)
                    return;
                SetEnableButton(false);
                ResetValueControlDetail();
                xtraTabControlMain.SelectedTabPageIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetEnableButton(bool enable)
        {
            try
            {
                if (enable)
                {
                    btnAddd.Visible = false;
                    btnAddd.Enabled = false;
                    btnCapNhat.Enabled = true;
                    btnHuy.Enabled = true;
                    btnCapNhat.Visible = true;
                    btnHuy.Visible = true;
                }
                else
                {
                    btnAddd.Visible = true;
                    btnAddd.Enabled = true;
                    btnCapNhat.Enabled = false;
                    btnHuy.Enabled = false;
                    btnCapNhat.Visible = false;
                    btnHuy.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnCapNhat.Enabled || !dxValidationProvider2.Validate() || this.currentMediMate == null)
                    return;
                WaitingManager.Show();

                if ((decimal?)spinExpAmount.EditValue > this.currentMediMate.AVAILABLE_AMOUNT)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho,
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        WaitingManager.Hide();
                        return;
                    }
                }

                this.currentMediMate.EXP_AMOUNT = spinExpAmount.Value;
                this.currentMediMate.DESCRIPTION = txtNote.Text;

                if (this.currentMediMate.IsMedicine)
                {
                    this.currentMediMate.ExpMedicine.Amount = spinExpAmount.Value;
                    this.currentMediMate.ExpMedicine.Description = txtNote.Text;
                }
                else
                {
                    this.currentMediMate.ExpMaterial.Amount = spinExpAmount.Value;
                    this.currentMediMate.ExpMaterial.Description = txtNote.Text;
                }

                dicMediMateAdo[this.currentMediMate.SERVICE_ID] = this.currentMediMate;
                gridControlExpMestChmsDetail.BeginUpdate();
                gridControlExpMestChmsDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpMestChmsDetail.EndUpdate();
                ResetValueControlDetail();
                SetEnableButton(false);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barCapNhat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnHuy_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ddBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                onClickPhieuXuatSuDung(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMestReason_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExpMestReason.EditValue = null;
                    cboExpMestReason.Properties.Buttons[1].Visible = false;
                    cboExpMestReason.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestReason_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExpMestReason.EditValue != null)
                {
                    cboExpMestReason.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboExpMestReason.Properties.Buttons[1].Visible = false;
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

        private void spinRemedyCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExpMestReason.Focus();
                    cboExpMestReason.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
