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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
//using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using HIS.Desktop.Utility;
using DevExpress.XtraLayout;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.MetyMaty.ADO;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using HIS.Desktop.Plugins.MetyMaty.Change;
using HIS.Desktop.Plugins.MetyMaty.Validation;
using MOS.SDO;
namespace HIS.Desktop.Plugins.MetyMaty
{
    public partial class MetyMatyForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        int startPage1 = 0;
        int HaveData = 0;
        long MetyProductID;
        DelegateReturnMutilObject delegate1 = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<MetyProductADO> ListMetyProductADO = new List<MetyProductADO>();
        List<MaterialTypeADO> ListMaterial = new List<MaterialTypeADO>();
        List<HIS_METY_MATY> ListMetMaty = new List<HIS_METY_MATY>();
        List<HIS_METY_METY> ListMetMety = new List<HIS_METY_METY>();

        MOS.EFMODEL.DataModels.V_HIS_METY_PRODUCT currentData;
        MedicineTypeADO expMestMedicineADO = null;
        MaterialTypeADO expMestMaterialTypeADO = null;
        List<MedicineTypeADO> mediMatyTypeCombos;
        object[] ListDelegate = new object[2];
        List<HIS_SERVICE_UNIT> ListServiceUnit = null;

        int positionHandle = -1;
        #endregion

        #region construct

        public MetyMatyForm(Inventec.Desktop.Common.Modules.Module module, List<HIS_METY_MATY> _ListMetMaty, List<HIS_METY_METY> _ListMetMety, long _MetProductID, DelegateReturnMutilObject _delegate1)
            : base(module)
        {

            try
            {
                InitializeComponent();
                //pagingGrid = new PagingGrid();
                currentModule = module;
                this.ListMetMaty = _ListMetMaty;
                this.ListMetMety = _ListMetMety;
                this.MetyProductID = _MetProductID;
                this.HaveData = 1;
                this.delegate1 = _delegate1;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public MetyMatyForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {

            try
            {
                InitializeComponent();
                //pagingGrid = new PagingGrid();
                currentModule = module;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Private method

        private void MetyMatyForm_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

        }

        private void GetServiceUnit()
        {
            try
            {
                if (this.ListServiceUnit == null || this.ListServiceUnit.Count == 0)
                {
                    MOS.Filter.HisServiceUnitFilter serviceUnitFilter = new HisServiceUnitFilter();
                    this.ListServiceUnit = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_UNIT>>("api/HisServiceUnit/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceUnitFilter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtSearchMaterial.Text = "";
                txtSearchMedicine.Text = "";
                cboType.EditValue = "Vật tư";


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDatagctFormListMedicine()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging1(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging1, param, numPageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void FillDatagctFormListMaterial()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging2(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(LoadPaging2, param, numPageSize, this.gridControl2);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {

                Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MetyMaty.Resource.Lang", typeof(HIS.Desktop.Plugins.MetyMaty.MetyMatyForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.layoutControl1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.layoutControl2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.layoutControl3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.btnSave.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclSTT2.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.grclSTT2.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclCodeMateril.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.grclCodeMateril.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclNameMaterial.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.grclNameMaterial.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclAmount.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.grclAmount.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclCheck1.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.grclCheck1.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclSTT.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.grclSTT.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclCodeMedicine.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.grclCodeMedicine.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclNameMedicine.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.grclNameMedicine.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.layoutControlItem3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.layoutControlItem4.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.btnFind1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicineType.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.lciMedicineType.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.lciAmount.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.btnAdd.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclAmount.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.grclAmount.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclDelete.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.grclDelete.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnFind2.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.btnFind2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.layoutControl5.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("MetyMatyForm.bar1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFind1.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.bbtnFind1.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFind2.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.bbtnFind2.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("MetyMatyForm.bbtnSave.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = this.currentModule.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDefaultFocus()
        {
            try
            {
                //txtSearchMedicine.Focus();
                //txtSearchMedicine.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void LoadPaging1(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_METY_PRODUCT>> apiResult = null;
                if (HaveData == 0)
                {
                    HisMetyProductViewFilter filter = new HisMetyProductViewFilter();
                    filter.IS_ACTIVE = 1;
                    SetFilterNavBar(ref filter);
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    gridView1.BeginUpdate();
                    apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_METY_PRODUCT>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_METY_PRODUCT_GET_VIEW, ApiConsumers.MosConsumer, filter, paramCommon);

                    if (apiResult != null)
                    {
                        var data = (List<MOS.EFMODEL.DataModels.V_HIS_METY_PRODUCT>)apiResult.Data;
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("data", data.Count));
                        ListMetyProductADO = new List<MetyProductADO>();
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                MetyProductADO x = new MetyProductADO();
                                x.ID = item.ID;
                                x.MEDICINE_TYPE_ID = item.MEDICINE_TYPE_ID;
                                x.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                                x.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                x.MEDICINE_LINE_NAME = item.MEDICINE_LINE_NAME;
                                x.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                x.AMOUNT = item.AMOUNT;
                                x.check = false;
                                ListMetyProductADO.Add(x);
                            }
                            gridControl1.DataSource = null;
                            gridControl1.DataSource = ListMetyProductADO;
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("  ", ListMetyProductADO.Count));
                            rowCount = (data == null ? 0 : data.Count);
                            dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        }
                    }
                    gridView1.EndUpdate();
                }
                else
                {
                    HisMetyProductViewFilter filter = new HisMetyProductViewFilter();
                    filter.IS_ACTIVE = 1;
                    SetFilterNavBar(ref filter);
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ID = MetyProductID;
                    gridView1.BeginUpdate();
                    apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_METY_PRODUCT
>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_METY_PRODUCT_GET_VIEW, ApiConsumers.MosConsumer, filter, paramCommon);

                    if (apiResult != null)
                    {
                        var data = (List<MOS.EFMODEL.DataModels.V_HIS_METY_PRODUCT>)apiResult.Data;
                        ListMetyProductADO = new List<MetyProductADO>();
                        if (data != null)
                        {
                            MetyProductADO x = new MetyProductADO();
                            x.ID = data.FirstOrDefault().ID;
                            x.MEDICINE_TYPE_ID = data.FirstOrDefault().MEDICINE_TYPE_ID;
                            x.MEDICINE_TYPE_CODE = data.FirstOrDefault().MEDICINE_TYPE_CODE;
                            x.MEDICINE_TYPE_NAME = data.FirstOrDefault().MEDICINE_TYPE_NAME;
                            x.MEDICINE_LINE_NAME = data.FirstOrDefault().MEDICINE_LINE_NAME;
                            x.SERVICE_UNIT_NAME = data.FirstOrDefault().SERVICE_UNIT_NAME;
                            x.AMOUNT = data.FirstOrDefault().AMOUNT;
                            x.check = true;
                            ListMetyProductADO.Add(x);
                        }

                        gridControl1.DataSource = null;
                        gridControl1.DataSource = ListMetyProductADO;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    gridView1.EndUpdate();
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadPaging2(object param)
        {
            try
            {

                startPage1 = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage1, limit);

                if (HaveData == 0)
                {

                    gridView2.BeginUpdate();
                    if (cboType.EditValue == "Vật tư")
                    {
                        HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                        ListMaterial = new List<MaterialTypeADO>();
                        filter.IS_ACTIVE = 1;
                        SetFilterNavBar2(ref filter);
                        filter.ORDER_DIRECTION = "DESC";
                        filter.ORDER_FIELD = "MODIFY_TIME";
                        filter.IS_RAW_MATERIAL = true;
                        Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>> apiResult = null;
                        apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_MATERIAL_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);

                        if (apiResult != null)
                        {
                            var data = (List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>)apiResult.Data;

                            //var data = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                            if (data != null)
                            {
                                foreach (var item in data)
                                {
                                    MaterialTypeADO x = new MaterialTypeADO();
                                    x.ID = item.ID;
                                    x.MATERIAL_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                                    x.MATERIAL_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                                    if (this.ListServiceUnit != null && this.ListServiceUnit.Count > 0)
                                    {
                                        var checkServiceUnit = this.ListServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                                        if (checkServiceUnit != null)
                                            x.SERVICE_UNIT_NAME = checkServiceUnit.SERVICE_UNIT_NAME;
                                    }
                                    ListMaterial.Add(x);
                                }
                            }

                            ListMaterial = ListMaterial.OrderByDescending(o => o.check).ToList();
                            gridControl2.DataSource = null;
                            gridControl2.DataSource = ListMaterial;
                            rowCount1 = (data == null ? 0 : data.Count);
                            dataTotal1 = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        }
                    }
                    else
                    {
                        HisMedicineTypeFilter filter = new HisMedicineTypeFilter();
                        ListMaterial = new List<MaterialTypeADO>();
                        filter.IS_ACTIVE = 1;
                        SetFilterNavBar2(ref filter);
                        filter.ORDER_DIRECTION = "DESC";
                        filter.ORDER_FIELD = "MODIFY_TIME";
                        filter.IS_RAW_MEDICINE = true;
                        Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>> apiResult = null;
                        apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_MEDICINE_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);

                        if (apiResult != null)
                        {
                            var data = (List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>)apiResult.Data;

                            //var data = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                            if (data != null)
                            {
                                foreach (var item in data)
                                {
                                    MaterialTypeADO x = new MaterialTypeADO();
                                    x.ID = item.ID;
                                    x.MATERIAL_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                                    x.MATERIAL_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                    if (this.ListServiceUnit != null && this.ListServiceUnit.Count > 0)
                                    {
                                        var checkServiceUnit = this.ListServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                                        if (checkServiceUnit != null)
                                            x.SERVICE_UNIT_NAME = checkServiceUnit.SERVICE_UNIT_NAME;
                                    }
                                    ListMaterial.Add(x);
                                }
                            }

                            ListMaterial = ListMaterial.OrderByDescending(o => o.check).ToList();
                            gridControl2.DataSource = null;
                            gridControl2.DataSource = ListMaterial;
                            rowCount1 = (data == null ? 0 : data.Count);
                            dataTotal1 = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        }
                    }
                }
                else
                {
                    gridView2.BeginUpdate();
                    if (cboType.EditValue == "Vật tư")
                    {
                        HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                        ListMaterial = new List<MaterialTypeADO>();
                        filter.IS_ACTIVE = 1;
                        SetFilterNavBar2(ref filter);
                        filter.ORDER_DIRECTION = "DESC";
                        filter.ORDER_FIELD = "MODIFY_TIME";
                        filter.IS_RAW_MATERIAL = true;
                        Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>> apiResult = null;
                        apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_MATERIAL_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);

                        if (apiResult != null)
                        {
                            var data = (List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>)apiResult.Data;
                            if (data != null)
                            {
                                foreach (var item in data)
                                {
                                    MaterialTypeADO x = new MaterialTypeADO();
                                    x.ID = item.ID;
                                    x.MATERIAL_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                                    x.MATERIAL_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                                    if (this.ListServiceUnit != null && this.ListServiceUnit.Count > 0)
                                    {
                                        var checkServiceUnit = this.ListServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                                        if (checkServiceUnit != null)
                                            x.SERVICE_UNIT_NAME = checkServiceUnit.SERVICE_UNIT_NAME;
                                    }
                                    x.amount = 0;
                                    ListMaterial.Add(x);
                                }
                                foreach (var item in ListMaterial)
                                {
                                    foreach (var item2 in ListMetMaty)
                                    {
                                        if (item.ID == item2.MATERIAL_TYPE_ID)
                                        {
                                            item.check = true;
                                            item.amount = item2.MATERIAL_TYPE_AMOUNT;
                                        }
                                    }
                                }
                                ListMaterial = ListMaterial.OrderByDescending(o => o.check).ToList();
                                gridControl2.DataSource = null;
                                gridControl2.DataSource = ListMaterial;
                                rowCount1 = (data == null ? 0 : data.Count);
                                dataTotal1 = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                            }
                        }
                    }
                    else
                    {
                        HisMedicineTypeFilter filter = new HisMedicineTypeFilter();
                        ListMaterial = new List<MaterialTypeADO>();
                        filter.IS_ACTIVE = 1;
                        SetFilterNavBar2(ref filter);
                        filter.ORDER_DIRECTION = "DESC";
                        filter.ORDER_FIELD = "MODIFY_TIME";
                        filter.IS_RAW_MEDICINE = true;
                        Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>> apiResult = null;
                        apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_MEDICINE_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);

                        if (apiResult != null)
                        {
                            var data = (List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>)apiResult.Data;
                            if (data != null)
                            {
                                foreach (var item in data)
                                {
                                    MaterialTypeADO x = new MaterialTypeADO();
                                    x.ID = item.ID;
                                    x.MATERIAL_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                                    x.MATERIAL_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                    if (this.ListServiceUnit != null && this.ListServiceUnit.Count > 0)
                                    {
                                        var checkServiceUnit = this.ListServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                                        if (checkServiceUnit != null)
                                            x.SERVICE_UNIT_NAME = checkServiceUnit.SERVICE_UNIT_NAME;
                                    }
                                    x.amount = 0;
                                    ListMaterial.Add(x);
                                }

                                foreach (var item in ListMaterial)
                                {
                                    foreach (var item2 in ListMetMety)
                                    {

                                        if (item.ID == item2.PREPARATION_MEDICINE_TYPE_ID)
                                        {
                                            item.check = true;
                                            item.amount = item2.PREPARATION_AMOUNT;
                                        }
                                    }
                                }

                                ListMaterial = ListMaterial.OrderByDescending(o => o.check).ToList();
                                gridControl2.DataSource = null;
                                gridControl2.DataSource = ListMaterial;
                                rowCount1 = (data == null ? 0 : data.Count);
                                dataTotal1 = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                            }
                        }
                    }
                }
                //var row = (MetyProductADO)gridView1.GetFocusedRow();
                //MetyProductID = row.ID;
                RefeshMaterial(MetyProductID);
                gridView2.EndUpdate();
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar2(ref HisMedicineTypeFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearchMaterial.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar2(ref HisMaterialTypeFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearchMaterial.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisMetyProductViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearchMedicine.Text.Trim();
                if (!string.IsNullOrEmpty(cboMedicineLine.EditValue.ToString()))
                {
                    filter.MEDICINE_LINE_ID = (long)cboMedicineLine.EditValue;

                }
                if (!string.IsNullOrEmpty(cboMedicineType.EditValue.ToString()))
                {
                    filter.MEDICINE_TYPE_ID = (long)cboMedicineType.EditValue;

                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void RefeshMaterial(long MetyProductID)
        {
            if (cboType.EditValue == "Vật tư")
            {
                if (HaveData == 0)
                {
                    CommonParam param = new CommonParam();
                    HisMetyMatyFilter filter = new HisMetyMatyFilter();
                    filter.METY_PRODUCT_ID = MetyProductID;
                    var api = new BackendAdapter(param).GetRO<List<HIS_METY_MATY>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_METY_MATY_GET, ApiConsumers.MosConsumer, filter, param);
                    ListMetMaty = (List<HIS_METY_MATY>)api.Data;
                }

                foreach (var item in ListMaterial)
                {
                    item.check = false;
                    item.amount = 0;

                    foreach (var item2 in ListMetMaty)
                    {
                        if (item2.MATERIAL_TYPE_ID == item.ID)
                        {
                            item.check = true;
                            item.amount = item2.MATERIAL_TYPE_AMOUNT;
                        }
                    }
                }

                ListMaterial = ListMaterial.OrderByDescending(o => o.check).ToList();
                gridControl2.DataSource = null;
                gridControl2.DataSource = ListMaterial;
            }

            else
            {

                if (HaveData == 0)
                {
                    CommonParam param = new CommonParam();
                    HisMetyMetyFilter filter = new HisMetyMetyFilter();
                    filter.METY_PRODUCT_ID = MetyProductID;
                    var api = new BackendAdapter(param).GetRO<List<HIS_METY_METY>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_METY_METY_GET, ApiConsumers.MosConsumer, filter, param);
                    ListMetMety = (List<HIS_METY_METY>)api.Data;
                }

                foreach (var item in ListMaterial)
                {
                    item.check = false;
                    item.amount = 0;

                    foreach (var item2 in ListMetMety)
                    {
                        if (item2.PREPARATION_MEDICINE_TYPE_ID == item.ID)
                        {
                            item.check = true;
                            item.amount = item2.PREPARATION_AMOUNT;
                        }
                    }
                }

                ListMaterial = ListMaterial.OrderByDescending(o => o.check).ToList();
                gridControl2.DataSource = null;
                gridControl2.DataSource = ListMaterial;
            }
        }
        #endregion

        #region Public method

        public void MeShow()
        {
            SetCaptionByLanguageKey();

            GetServiceUnit();

            SetDefaultValue();
            LoadCombo();
            //Set validate rule
            ValidControl();
            FillDatagctFormListMedicine();
            FillDatagctFormListMaterial();
            SetDefaultFocus();
            this.mediMatyTypeCombos = SetDataToMedicineTypeCombo();

        }

        private void LoadCombo()
        {

            LoadComboMedicineLine();


        }

        private List<MedicineTypeADO> SetDataToMedicineTypeCombo()
        {
            List<MedicineTypeADO> result = new List<MedicineTypeADO>();
            try
            {
                //if (this.ListMetMety != null)
                //{
                //HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                //mediFilter.MEDI_STOCK_ID =BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault().ID;
                //mediFilter.IS_LEAF = true;

                //mediFilter.MEDICINE_TYPE_IDs = medicineTypeIds;

                var listMediType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                //new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>("api/HisMedicineType/GetInStockMedicineType", ApiConsumers.MosConsumer, mediFilter, null);

                foreach (var item in listMediType)
                {
                    MedicineTypeADO ado = new MedicineTypeADO(item);
                    ado.ADDITION_INFO = item.MEDICINE_TYPE_NAME;
                    ado.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                    ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                    ado.ID = item.ID;
                    result.Add(ado);
                }
                //}
                //else if (this.ListMetMety != null)
                //{
                //    //HisMaterialTypeStockViewFilter mediFilter = new HisMaterialTypeStockViewFilter();
                //    //mediFilter.MEDI_STOCK_ID = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault().ID;
                //    ////mediFilter.IS_LEAF = true;

                //    //mediFilter.MEDICINE_TYPE_IDs = medicineTypeIds;

                //    var listMaterialInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>("api/HisMaterialType/GetInStockMaterialType", ApiConsumers.MosConsumer, mediFilter, null);



                //    foreach (var item in listMaterialInStock)
                //    {
                //        MetyMatyTypeADO ado = new MetyMatyTypeADO(item);
                //        ado.ADDITION_INFO = item.MaterialTypeName;
                //        ado.MedicineTypeCode= item.MaterialTypeCode;
                //        ado.MedicineTypeName = item.MaterialTypeName;
                //        ado.ServiceUnitName = item.ServiceUnitName;
                //        ado.AvailableAmount = item.AvailableAmount;
                //        ado.Id = item.Id;
                //        ado.IsLeaf = item.IsLeaf;
                //        ado.TotalAmount = item.TotalAmount;
                //        result.Add(ado);
                //    }
                //}

                InItMedicineTypeCombo(result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void InItMedicineTypeCombo(List<MedicineTypeADO> result)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "Mã", 300, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "Tên", 300, 2));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "Đơn vị", 300, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ADDITION_INFO", "ID", columnInfos, false, 750);
                ControlEditorLoader.Load(this.cboMedicineType, result, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboMedicineLine()
        {
            List<HIS_MEDICINE_LINE> ListMedicineLine = BackendDataWorker.Get<HIS_MEDICINE_LINE>();
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("MEDICINE_LINE_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("MEDICINE_LINE_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_LINE_NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(cboMedicineLine, ListMedicineLine, controlEditorADO);
        }


        #endregion

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MaterialTypeADO pData = (MaterialTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage1; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "check")
                    {
                        e.Value = pData.check;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MetyProductADO pData = (MetyProductADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "check")
                    {
                        e.Value = pData.check;
                    }
                    //else if (e.Column.FieldName == "MEDICINE_LINE_NAME")
                    //{
                    //    var medicineLine = BackendDataWorker.Get<HIS_MEDICINE_LINE>().Where(o => o.ID ==data.MEDICINE_LINE_ID).FirstOrDefault();
                    //    e.Value = medicineLine.MEDICINE_LINE_NAME;
                    //}
                    //else if (e.Column.FieldName == "MEDICINE_TYPE_CODE")
                    //{
                    //    e.Value = data.MEDICINE_TYPE_CODE; 
                    //}
                    //else if (e.Column.FieldName == "MEDICINE_TYPE_NAME")
                    //{
                    //    e.Value = data.MEDICINE_TYPE_NAME;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                MetyProductADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (MetyProductADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "check")
                    {
                        e.RepositoryItem = check;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void check1Enable_Click(object sender, EventArgs e)
        {
            var row = (MetyProductADO)gridView1.GetFocusedRow();
            if (row.check == true)
            {
                row.check = false;
                MetyProductID = 0;
                FillDatagctFormListMaterial();
                return;
            }
            foreach (var item in ListMetyProductADO)
            {
                item.check = false;
                if (item.ID == row.ID)
                {
                    MetyProductID = row.ID;
                    item.check = true;
                }
            }
            gridControl1.DataSource = ListMetyProductADO;
            gridControl1.RefreshDataSource();
            RefeshMaterial(row.ID);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                gridView2.PostEditor();
                int i = 1;
                int j = 1;
                CommonParam param = new CommonParam();
                Boolean success = false;
                WaitingManager.Show();
                if (MetyProductID == null || MetyProductID == 0)
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Chưa chọn thuốc thành phẩm", "Thông báo");
                    return;
                }
                foreach (var item in ListMaterial)
                {
                    if (item.check && item.amount == 0)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Chế phẩm chưa nhập số lượng", "Thông báo");
                        return;
                    }
                }
                UpdateTableMetyMaty(ref success, ref param, i);
                UpdateTableMetyMety(ref success, ref param, j);
                if (cboType.EditValue == "Vật tư")
                {
                    ListDelegate[0] = ListMetMaty;
                }
                else
                {
                    ListDelegate[1] = ListMetMety;
                }
                if (i * j == 0)
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Chưa có thay đổi chế phẩm", "thông báo");
                    return;
                }
                else
                {
                    success = true;
                }
                //else
                //{
                //foreach (var item in ListMaterial)
                //{
                //    if (item.amount!=0)
                //    {
                //        HIS_METY_MATY x = new HIS_METY_MATY();
                //        x.MEDICINE_TYPE_ID = MetyProductID;
                //        x.MATERIAL_TYPE_ID = item.ID;
                //        x.MATERIAL_TYPE_AMOUNT = item.amount;
                //        ListCreate.Add(x);
                //    }
                //}
                //var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_METY_MATY>(HisRequestUriStore.MOSHIS_METY_MATY_CREATELIST, ApiConsumers.MosConsumer, ListMetMaty, param);
                //FillDatagctFormListMedicine();

                // success = true;
                //}
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                if (HaveData == 1)
                {

                    delegate1(ListDelegate);
                    this.Close();
                }

                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void UpdateTableMetyMety(ref Boolean success, ref CommonParam param, int j)
        {
            success = false;
            List<HIS_METY_METY> ListCreate = new List<HIS_METY_METY>();
            List<HIS_METY_METY> ListUpdate = new List<HIS_METY_METY>();
            List<long> ListDelete = new List<long>();

            List<HIS_METY_METY> ListMetyMety1 = new List<HIS_METY_METY>();
            List<MaterialTypeADO> ListMaterialOld = new List<MaterialTypeADO>();
            param = new CommonParam();
            DataOld2(ref ListMaterialOld, ref ListMetyMety1);

            if (ListMaterialOld.Count != 0)
            {
                foreach (var item in ListMaterialOld)
                {
                    foreach (var item1 in ListMaterial)
                    {
                        if (item.ID == item1.ID)
                        {
                            if (item.amount != item1.amount && item.check && item1.check)
                            {
                                HIS_METY_METY o = new HIS_METY_METY();
                                o.PREPARATION_MEDICINE_TYPE_ID = item.ID;
                                o.METY_PRODUCT_ID = MetyProductID;
                                o.PREPARATION_AMOUNT = item1.amount;
                                foreach (var item2 in ListMetyMety1)
                                {
                                    if (item2.PREPARATION_MEDICINE_TYPE_ID == o.PREPARATION_MEDICINE_TYPE_ID && item2.PREPARATION_AMOUNT != 0)
                                    {
                                        o.ID = item2.ID;
                                    }
                                }
                                ListUpdate.Add(o);
                            }
                            else if (item.check && !item1.check)
                            {
                                long x = 0;
                                foreach (var item2 in ListMetyMety1)
                                {
                                    if (item2.PREPARATION_MEDICINE_TYPE_ID == item.ID && item2.PREPARATION_AMOUNT != 0)
                                    {
                                        x = item2.ID;
                                    }
                                }
                                ListDelete.Add(x);
                            }
                            else if (!item.check && item1.check)
                            {
                                HIS_METY_METY o = new HIS_METY_METY();
                                o.PREPARATION_MEDICINE_TYPE_ID = item.ID;
                                o.METY_PRODUCT_ID = MetyProductID;
                                o.PREPARATION_AMOUNT = item1.amount;
                                ListCreate.Add(o);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var item1 in ListMaterial)
                {
                    HIS_METY_METY o = new HIS_METY_METY();
                    o.PREPARATION_MEDICINE_TYPE_ID = item1.ID;
                    o.METY_PRODUCT_ID = MetyProductID;
                    o.PREPARATION_AMOUNT = item1.amount;
                    ListCreate.Add(o);
                }
            }

            if (ListCreate != null && ListCreate.Count > 0)
            {
                var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_METY_METY>(HisRequestUriStore.MOSHIS_METY_METY_CREATELIST, ApiConsumers.MosConsumer, ListCreate, param);
                success = true;
                if (HaveData == 1)
                {
                    foreach (var item in ListCreate)
                    {
                        ListMetyMety1.Add(item);
                    }
                }
            }
            else if (ListUpdate != null && ListUpdate.Count > 0)
            {
                var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_METY_METY>(HisRequestUriStore.MOSHIS_METY_METY_UPDATELIST, ApiConsumers.MosConsumer, ListUpdate, param);
                success = true;
                if (HaveData == 1)
                {
                    foreach (var item in ListUpdate)
                    {
                        foreach (var item1 in ListMetyMety1)
                        {
                            if (item.ID == item1.ID)
                            {
                                item1.PREPARATION_AMOUNT = Math.Round(item.PREPARATION_AMOUNT, 6);
                            }
                        }
                    }
                }

            }
            else if (ListDelete.Count > 0)
            {
                List<HIS_METY_METY> ListMetyMetyRemove = new List<HIS_METY_METY>();
                var resultData = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_METY_METY_DELETELIST, ApiConsumers.MosConsumer, ListDelete, param);
                success = true;
                if (HaveData == 1)
                {
                    foreach (var item in ListDelete)
                    {
                        foreach (var item2 in ListMetyMety1)
                        {
                            if (item2.ID == item)
                            {
                                ListMetyMetyRemove.Add(item2);
                            }
                        }
                    }





                }
                foreach (var item in ListMetyMetyRemove)
                {
                    ListMetyMety1.Remove(item);
                }
            }
            else if (ListUpdate.Count == 0 && ListCreate.Count == 0 && ListDelete.Count == 0)
            {
                j = 0;
            }
            if (HaveData == 1)
            {
                ListDelegate[0] = ListMetyMety1;
            }
        }

        private void DataOld2(ref List<MaterialTypeADO> ListMaterialOld, ref List<HIS_METY_METY> ListMetyMety2)
        {
            List<HIS_MEDICINE_TYPE> ListMaterialOld1 = BackendDataWorker.Get<HIS_MEDICINE_TYPE>();
            foreach (var item in ListMaterialOld1)
            {
                if (item.IS_RAW_MEDICINE == 1)
                {
                    MaterialTypeADO o = new MaterialTypeADO();
                    o.check = false;
                    o.ID = item.ID;
                    o.amount = 0;
                    ListMaterialOld.Add(o);
                }

            }
            CommonParam param = new CommonParam();
            HisMetyMetyFilter filter1 = new HisMetyMetyFilter();
            filter1.METY_PRODUCT_ID = MetyProductID;
            var api2 = new BackendAdapter(param).GetRO<List<HIS_METY_METY>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_METY_METY_GET, ApiConsumers.MosConsumer, filter1, param);
            ListMetyMety2 = (List<HIS_METY_METY>)api2.Data;
            foreach (var item in ListMaterialOld)
            {
                foreach (var item2 in ListMetyMety2)
                {
                    if (item.ID == item2.PREPARATION_MEDICINE_TYPE_ID)
                    {
                        item.check = true;
                        item.amount = item2.PREPARATION_AMOUNT;
                    }
                }
            }
        }
        private void UpdateTableMetyMaty(ref Boolean success, ref CommonParam param, int i)
        {
            success = false;
            List<HIS_METY_MATY> ListCreate = new List<HIS_METY_MATY>();
            List<HIS_METY_MATY> ListUpdate = new List<HIS_METY_MATY>();
            List<long> ListDelete = new List<long>();
            List<HIS_METY_MATY> ListMetyMaty1 = new List<HIS_METY_MATY>();
            List<MaterialTypeADO> ListMaterialOld = new List<MaterialTypeADO>();
            param = new CommonParam();
            DataOld(ref ListMaterialOld, ref ListMetyMaty1);

            if (ListMaterialOld.Count != 0)
            {
                foreach (var item in ListMaterialOld)
                {
                    foreach (var item1 in ListMaterial)
                    {
                        if (item.ID == item1.ID)
                        {
                            if (item.amount != item1.amount && item.check && item1.check)
                            {
                                HIS_METY_MATY o = new HIS_METY_MATY();
                                o.MATERIAL_TYPE_ID = item.ID;
                                o.METY_PRODUCT_ID = MetyProductID;
                                o.MATERIAL_TYPE_AMOUNT = item1.amount;
                                foreach (var item2 in ListMetyMaty1)
                                {
                                    if (item2.MATERIAL_TYPE_ID == o.MATERIAL_TYPE_ID && item2.MATERIAL_TYPE_AMOUNT != 0)
                                    {
                                        o.ID = item2.ID;
                                    }
                                }
                                ListUpdate.Add(o);
                            }
                            else if (item.check && !item1.check)
                            {
                                long x = 0;
                                foreach (var item2 in ListMetyMaty1)
                                {
                                    if (item2.MATERIAL_TYPE_ID == item.ID && item2.MATERIAL_TYPE_AMOUNT != 0)
                                    {
                                        x = item2.ID;
                                    }
                                }
                                ListDelete.Add(x);
                            }
                            else if (!item.check && item1.check)
                            {
                                HIS_METY_MATY o = new HIS_METY_MATY();
                                o.MATERIAL_TYPE_ID = item.ID;
                                o.METY_PRODUCT_ID = MetyProductID;
                                o.MATERIAL_TYPE_AMOUNT = item1.amount;
                                ListCreate.Add(o);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var item1 in ListMaterial)
                {
                    HIS_METY_MATY o = new HIS_METY_MATY();
                    o.MATERIAL_TYPE_ID = item1.ID;
                    o.METY_PRODUCT_ID = MetyProductID;
                    o.MATERIAL_TYPE_AMOUNT = item1.amount;
                    ListCreate.Add(o);
                }
            }

            if (ListCreate.Count != null && ListCreate.Count > 0)
            {
                var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_METY_MATY>(HisRequestUriStore.MOSHIS_METY_MATY_CREATELIST, ApiConsumers.MosConsumer, ListCreate, param);
                success = true;
                if (HaveData == 1)
                {
                    foreach (var item in ListCreate)
                    {
                        ListMetyMaty1.Add(item);
                    }
                }
            }
            else if (ListUpdate.Count != null && ListUpdate.Count > 0)
            {
                var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_METY_MATY>(HisRequestUriStore.MOSHIS_METY_MATY_UPDATELIST, ApiConsumers.MosConsumer, ListUpdate, param);
                success = true;
                if (HaveData == 1)
                {
                    foreach (var item in ListUpdate)
                    {
                        foreach (var item1 in ListMetyMaty1)
                        {
                            if (item.ID == item1.ID)
                            {
                                item1.MATERIAL_TYPE_AMOUNT = Math.Round(item.MATERIAL_TYPE_AMOUNT, 6);
                            }
                        }
                    }
                }

            }
            else if (ListDelete.Count > 0)
            {
                List<HIS_METY_MATY> ListMetyMatyRemove = new List<HIS_METY_MATY>();
                var resultData = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_METY_MATY_DELETELIST, ApiConsumers.MosConsumer, ListDelete, param);
                success = true;
                if (HaveData == 1)
                {
                    foreach (var item in ListDelete)
                    {
                        foreach (var item2 in ListMetyMaty1)
                        {
                            if (item2.ID == item)
                            {
                                ListMetyMatyRemove.Add(item2);
                            }
                        }
                    }
                }
                foreach (var item in ListMetyMatyRemove)
                {
                    ListMetyMaty1.Remove(item);
                }
            }
            else if (ListUpdate.Count == 0 && ListCreate.Count == 0 && ListDelete.Count == 0)
            {
                i = 0;
            }
            if (HaveData == 1)
            {
                ListDelegate[0] = ListMetyMaty1;
            }
        }

        private void DataOld(ref List<MaterialTypeADO> ListMaterialOld, ref List<HIS_METY_MATY> ListMetyMaty2)
        {
            List<HIS_MATERIAL_TYPE> ListMaterialOld1 = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
            foreach (var item in ListMaterialOld1)
            {
                if (item.IS_RAW_MATERIAL == 1)
                {
                    MaterialTypeADO o = new MaterialTypeADO();
                    o.check = false;
                    o.ID = item.ID;
                    o.amount = 0;
                    ListMaterialOld.Add(o);
                }

            }
            CommonParam param = new CommonParam();
            HisMetyMatyFilter filter1 = new HisMetyMatyFilter();
            filter1.METY_PRODUCT_ID = MetyProductID;
            var api2 = new BackendAdapter(param).GetRO<List<HIS_METY_MATY>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_METY_MATY_GET, ApiConsumers.MosConsumer, filter1, param);
            ListMetyMaty2 = (List<HIS_METY_MATY>)api2.Data;
            foreach (var item in ListMaterialOld)
            {
                foreach (var item2 in ListMetyMaty2)
                {
                    if (item.ID == item2.MATERIAL_TYPE_ID)
                    {
                        item.check = true;
                        item.amount = item2.MATERIAL_TYPE_AMOUNT;
                    }
                }
            }
        }

        private void btnFind1_Click(object sender, EventArgs e)
        {
            FillDatagctFormListMedicine();
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            FillDatagctFormListMaterial();
            RefeshMaterial(MetyProductID);
        }

        private void bbtnFind1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            btnFind1_Click(null, null);
        }

        private void bbtnFind2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind2_Click(null, null);
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave.Focus();
            btnSave_Click(null, null);
        }

        private void txtSearchMaterial_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind2_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtSearchMedicine_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind1_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MaterialTypeADO)gridView2.GetFocusedRow();
                if (row.check)
                {
                    row.check = false;
                    row.amount = 0;
                }
                else
                {
                    row.check = true;
                }

                foreach (var item in ListMaterial)
                {
                    if (row.ID == item.ID)
                    {
                        item.check = row.check;
                    }
                }
                ListMaterial = ListMaterial.OrderByDescending(o => o.check).ToList();
                gridControl2.DataSource = ListMaterial;
                gridControl2.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void cboMedicineLine_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMedicineLine.Properties.Buttons[1].Visible = true;
                    cboMedicineLine.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                FillDatagctFormListMaterial();
                RefeshMaterial(MetyProductID);
            }
            catch (Exception ex)
            {

            }
        }

        PopupMenuProcessor _PopupMenuProcessor;
        MaterialTypeADO _MaterialTypeADO;
        private void gridView2_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = gridView2.GetVisibleRowHandle(hi.RowHandle);
                    this._MaterialTypeADO = new MaterialTypeADO();
                    this._MaterialTypeADO = (MaterialTypeADO)gridView2.GetRow(rowHandle);
                    gridView2.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridView2.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }

                    this._PopupMenuProcessor = new PopupMenuProcessor(this._MaterialTypeADO, this.MouseRight_Click, barManager1);
                    this._PopupMenuProcessor.InitMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void MouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this._MaterialTypeADO != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ModuleType type = (PopupMenuProcessor.ModuleType)(e.Item.Tag);

                    switch (type)
                    {
                        case PopupMenuProcessor.ModuleType.T1:
                            bool isMedicine = false;
                            if (cboType.EditValue == "Thuốc")
                                isMedicine = true;
                            frmChangePrepa frm = new frmChangePrepa(this.currentModuleBase, _MaterialTypeADO.ID, isMedicine);
                            frm.ShowDialog();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bntAdd_Click(object sender, EventArgs e)
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
        private void ValidControl()
        {
            try
            {
                ValidControlRepayReason();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidControlRepayReason()
        {
            try
            {
                MetyProductValidationRule metyProductRule = new MetyProductValidationRule();
                metyProductRule.spAmount = spAmount;
                metyProductRule.cboMedicineType = cboMedicineType;
                metyProductRule.isRequred = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.Repay.Is_Required_Repay_Reason");
                dxValidationProvider1.SetValidationRule(spAmount, metyProductRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SaveProcess()
        {
            CommonParam param = new CommonParam();

            try
            {
                bool success = false;
                WaitingManager.Show();
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                HIS_METY_PRODUCT metyProduct = new HIS_METY_PRODUCT();
                if (string.IsNullOrEmpty(cboMedicineType.EditValue.ToString()) || string.IsNullOrEmpty(spAmount.EditValue.ToString()))
                {
                    success = false;
                }
                if (cboMedicineType.EditValue != null) metyProduct.MEDICINE_TYPE_ID = (long)cboMedicineType.EditValue;
                if (spAmount.EditValue != null) metyProduct.AMOUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spAmount.EditValue.ToString());
                metyProduct.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var resultData = new BackendAdapter(param).Post<HIS_METY_PRODUCT>(HisRequestUriStore.MOSHIS_METY_PRODUCT_CREATE, ApiConsumers.MosConsumer, metyProduct, param);
                if (resultData != null)
                {
                    success = true;
                    FillDatagctFormListMedicine();
                    cboMedicineType.EditValue = "";
                    spAmount.EditValue = 1;
                }
                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditDelete_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (MetyProductADO)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HisMetyProductFilter filter = new HisMetyProductFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<HIS_METY_PRODUCT>>(HisRequestUriStore.MOSHIS_METY_PRODUCT_GET_VIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_METY_PRODUCT_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                        if (success)
                        {
                            FillDatagctFormListMedicine();
                            currentData = ((List<V_HIS_METY_PRODUCT>)gridView1.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void repositoryItemSpinEditAmount_EditValueChanged(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                var spinEdit = sender as DevExpress.XtraEditors.SpinEdit;

                HIS_METY_PRODUCT metyProduct = new HIS_METY_PRODUCT();
                var rowData = (MetyProductADO)gridView1.GetFocusedRow();
                if (spinEdit.Value == rowData.AMOUNT)
                {
                    return;
                }
                else
                {
                    metyProduct.ID = rowData.ID;
                    metyProduct.MEDICINE_TYPE_ID = rowData.MEDICINE_TYPE_ID;
                    if (spinEdit.Value < 1) metyProduct.AMOUNT = 1;
                    else
                    {
                        metyProduct.AMOUNT = spinEdit.Value;
                    }
                    var resultData = new BackendAdapter(param).Post<HIS_METY_PRODUCT>(HisRequestUriStore.MOSHIS_METY_PRODUCT_UPDATE, ApiConsumers.MosConsumer, metyProduct, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDatagctFormListMedicine();
                    }
                    WaitingManager.Hide();

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void bbtnAdd_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnAdd.Focus();
            bntAdd_Click(null, null);
        }

        private void cboMedicineType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void spAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                    bntAdd_Click(null, null);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void SelectFirstRowPopup(GridLookUpEdit cbo)
        {
            try
            {
                if (cbo != null && cbo.IsPopupOpen)
                {
                    DevExpress.Utils.Win.IPopupControl popupEdit = cbo as DevExpress.Utils.Win.IPopupControl;
                    DevExpress.XtraEditors.Popup.PopupLookUpEditForm popupWindow = popupEdit.PopupWindow as DevExpress.XtraEditors.Popup.PopupLookUpEditForm;
                    if (popupWindow != null)
                    {
                        popupWindow.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spAmount.Focus();
                }
                else
                {
                    cboMedicineType.ShowPopup();
                    SelectFirstRowPopup(cboMedicineType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
