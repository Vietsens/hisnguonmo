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
using Inventec.Core;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.Desktop.Plugins.AnticipateUpdate
{
    public partial class UCAnticipateUpdate : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        internal int ActionType = 0;
        private double idRow = -1;
        int positionHandleLeft = -1;
        internal MOS.EFMODEL.DataModels.HIS_ANTICIPATE anticipateModel = null;
        internal MOS.EFMODEL.DataModels.HIS_ANTICIPATE anticipatePrint = null;
        internal List<ADO.MedicineTypeADO> ListMedicineTypeAdoProcess;
        internal ADO.BloodTypeADO bloodType;
        internal ADO.MedicineTypeADO medicineType;
        internal ADO.MaterialTypeADO materialType;
        internal ADO.MedicineTypeADO editMedicineTypeAdo;

        HIS.UC.BidMedicineTypeGrid.UCBidMedicineTypeGridProcessor medicineTypeProcessor;
        HIS.UC.BidMaterialTypeGrid.UCBidMaterialTypeGridProcessor materialTypeProcessor;
        HIS.UC.BidBloodTypeGrid.UCBidBloodTypeGridProcessor bloodTypeProcessor;
        UserControl ucMedicineType;
        UserControl ucMaterialType;
        UserControl ucBloodType;

        List<V_HIS_ANTICIPATE_BLTY> listAticipateBltys;
        List<V_HIS_ANTICIPATE_MATY> listAticipateMatys;
        List<V_HIS_ANTICIPATE_METY> listAticipateMetys;

        List<V_HIS_BID_MEDICINE_TYPE> listBidMedicineTypes;
        List<V_HIS_BID_MATERIAL_TYPE> listBidMaterialTypes;

        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_ANTICIPATE currentAnticipate;
        HIS.Desktop.Common.DelegateRefreshData delegateRefresh;

        bool showMaterial = true;
        bool showBlood = true;
        bool checkLoadBid = false;

        System.Globalization.CultureInfo cultureLang;
        long RoomId;
        long DepartmentId;
        long mediStockId;
        #endregion

        #region Construct
        public UCAnticipateUpdate()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                InitializeMedicineType();
                InitializeMaterialType();
                InitializeBloodType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCAnticipateUpdate(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                InitializeMedicineType();
                InitializeMaterialType();
                InitializeBloodType();
                RoomId = moduleData.RoomId;
                var workPlace = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomTypeId == moduleData.RoomTypeId && o.RoomId == moduleData.RoomId);
                if (workPlace != null)
                {
                    DepartmentId = workPlace.DepartmentId;
                    mediStockId = workPlace.MediStockId ?? 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCAnticipateUpdate(Inventec.Desktop.Common.Modules.Module module, V_HIS_ANTICIPATE anticipate, HIS.Desktop.Common.DelegateRefreshData refresh)
            : this()
        {
            try
            {
                this.currentModule = module;
                this.currentAnticipate = anticipate;
                this.delegateRefresh = refresh;
                RoomId = module.RoomId;
                var workPlace = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomTypeId == module.RoomTypeId && o.RoomId == module.RoomId);
                if (workPlace != null)
                {
                    DepartmentId = workPlace.DepartmentId;
                    mediStockId = workPlace.MediStockId ?? 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCAnticipateUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //load cob nha thau
                LoadDataToCboSupplier();

                //Load combo goi thau
                LoadDataToCboBid();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                SetUpdateAnticipate();

                SetDefaultValueControlUpdate();
                //Load du lieu
                FillDataToGridMedicineType();

                //trang thai nut              

                //Load Validation
                ValidControls();

                //print
                SetPrintTypeToMps();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void LoadKeysFromlanguage()
        {
            try
            {
                #region LAYOUT
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__XTRA_TAB_MEDICINE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__XTRA_TAB_MATERIAL",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__XTRA_TAB_BLOOD",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.lciSupplier.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__LCI_SUPPLIER",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__LCI_AMOUNT",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__LCI_DESCRIPTION",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.lciImpPice.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__LCI_IMP_PICE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.lciUseTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__LCI_USE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_ADD",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.btnNew.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_NEW",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.btnUpdate.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_UPDATE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.btnDiscard.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_PRINT",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.txtUseTime.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__TXT_USE_TIME_FOCUS",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                #endregion
                #region GRID PROCESS
                this.GvProcess_GcActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_ACTIVE_INGR_BHYT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcAmount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_AMOUNT",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcConcentra.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcImpPrice.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_IMP_PRICE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcManufactureName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS_GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS_GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcNationalName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcPackingTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_PACKING_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcServiceUnitName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcSTT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_STT",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcSupplierName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_SUPPLIER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.GvProcess_GcTypeDisplay.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_TYPE_DISPLAY",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.ButtonDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__TOOL_TIP__BUTTON_DELETE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                this.ButtonEdit.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__TOOL_TIP__BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetUpdateAnticipate()
        {
            try
            {
                if (this.currentAnticipate != null)
                {
                    this.listAticipateBltys = new List<V_HIS_ANTICIPATE_BLTY>();
                    this.listAticipateMatys = new List<V_HIS_ANTICIPATE_MATY>();
                    this.listAticipateMetys = new List<V_HIS_ANTICIPATE_METY>();
                    getBloodType(this.currentAnticipate, ref this.listAticipateBltys);
                    getMaterialType(this.currentAnticipate, ref this.listAticipateMatys);
                    getMedicineType(this.currentAnticipate, ref this.listAticipateMetys);

                    //if (this.currentAnticipate.BID_ID == null)
                    //{
                    LoadEditNotBid();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void getMedicineType(V_HIS_ANTICIPATE data, ref List<V_HIS_ANTICIPATE_METY> anticipateMetys)
        {
            try
            {
                if (data != null)
                {
                    CommonParam param = new CommonParam();
                    HisAnticipateMetyViewFilter filter = new HisAnticipateMetyViewFilter();
                    filter.ANTICIPATE_ID = data.ID;
                    anticipateMetys = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_METY>>("api/HisAnticipateMety/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void getMaterialType(V_HIS_ANTICIPATE data, ref List<V_HIS_ANTICIPATE_MATY> anticipateMatys)
        {
            try
            {
                try
                {
                    if (data != null)
                    {
                        CommonParam param = new CommonParam();
                        HisAnticipateMatyViewFilter filter = new HisAnticipateMatyViewFilter();
                        filter.ANTICIPATE_ID = data.ID;
                        anticipateMatys = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_MATY>>("api/HisAnticipateMaty/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void getBloodType(V_HIS_ANTICIPATE data, ref List<V_HIS_ANTICIPATE_BLTY> anticipateBltys)
        {
            try
            {
                if (data != null)
                {
                    CommonParam param = new CommonParam();
                    HisAnticipateBltyViewFilter filter = new HisAnticipateBltyViewFilter();
                    filter.ANTICIPATE_ID = data.ID;
                    anticipateBltys = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_BLTY>>("api/HisAnticipateBlty/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControlUpdate()
        {
            try
            {
                if (this.currentAnticipate != null)
                {
                    //if (this.currentAnticipate.BID_ID != null)
                    //{
                    //    cboBid.EditValue = this.currentAnticipate.BID_ID;
                    //}
                    btnNew.Enabled = false;
                    btnImport.Enabled = false;
                    cboBid.Enabled = false;
                    txtUseTime.Text = this.currentAnticipate.USE_TIME;
                    txtDescription.Text = this.currentAnticipate.DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetPrintTypeToMps()
        {
            try
            {
                if (MPS.PrintConfig.PrintTypes == null || MPS.PrintConfig.PrintTypes.Count == 0)
                {
                    MPS.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                checkLoadBid = false;
                cboSupplier.Enabled = true;
                txtSupplierCode.Enabled = true;
                txtDescription.Text = "";
                txtUseTime.Text = "";
                ListMedicineTypeAdoProcess = new List<ADO.MedicineTypeADO>();
                this.ActionType = GlobalVariables.ActionAdd;
                gridControlProcess.DataSource = null;
                cboSupplier.Properties.Buttons[1].Visible = false;
                xtraTabControl1.SelectedTabPageIndex = 0;
                cboBid.EditValue = null;
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
                txtSupplierCode.Text = "";
                cboSupplier.EditValue = null;
                cboSupplier.Properties.Buttons[1].Visible = false;
                cboBid.EditValue = null;
                cboBid.Properties.Buttons[1].Visible = false;
                spinAmount.Value = 0;
                spinImpPrice.Value = 0;
                EnableButton(this.ActionType);
                VisibleButton(this.ActionType);
                FocusTab();
                showBlood = true;
                showMaterial = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMedicineType()
        {
            try
            {
                WaitingManager.Show();

                if (this.currentAnticipate == null)// || (this.currentAnticipate != null && this.currentAnticipate.BID_ID == null))
                {
                    MOS.Filter.HisMedicineTypeViewFilter mediFilter = new MOS.Filter.HisMedicineTypeViewFilter();
                    mediFilter.IS_STOP_IMP = false;
                    mediFilter.IS_LEAF = true;
                    mediFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var listMedicineType = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_MEDICINE_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO> MedicineTypeADOs = new List<UC.BidMedicineTypeGrid.MedicineTypeADO>();
                    if (listMedicineType != null && listMedicineType.Count > 0)
                    {
                        MedicineTypeADOs = (from r in listMedicineType select new UC.BidMedicineTypeGrid.MedicineTypeADO(r)).ToList();
                    }

                    this.medicineTypeProcessor.Reload(this.ucMedicineType, MedicineTypeADOs);
                    Base.GlobalConfig.HisMedicineTypes = listMedicineType;
                }
                else
                {
                    CommonParam param = new CommonParam();
                    HisBidMedicineTypeViewFilter bidMedicineFilter = new HisBidMedicineTypeViewFilter();
                    //bidMedicineFilter.BID_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString());
                    listBidMedicineTypes = new BackendAdapter(param).Get<List<V_HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/GetView", ApiConsumer.ApiConsumers.MosConsumer, bidMedicineFilter, param);

                    if (listBidMedicineTypes != null && listBidMedicineTypes.Count > 0)
                    {
                        if (Base.GlobalConfig.HisMedicineTypes != null && Base.GlobalConfig.HisMedicineTypes.Count > 0)
                        {
                            List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO> medicineTypeDataSource = new List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO>();
                            foreach (var itemBid in listBidMedicineTypes)
                            {
                                var medicineAdd = Base.GlobalConfig.HisMedicineTypes.FirstOrDefault(o => o.ID == itemBid.MEDICINE_TYPE_ID);
                                if (medicineAdd != null)
                                {
                                    HIS.UC.BidMedicineTypeGrid.MedicineTypeADO mediAdo = new UC.BidMedicineTypeGrid.MedicineTypeADO(medicineAdd);
                                    mediAdo.AllowAmount = itemBid.AMOUNT - (itemBid.IN_AMOUNT != null ? itemBid.IN_AMOUNT : 0) + (itemBid.ADJUST_AMOUNT ?? 0) + (itemBid.AMOUNT * itemBid.IMP_MORE_RATIO ?? 0);
                                    mediAdo.IMP_PRICE = itemBid.IMP_PRICE;
                                    mediAdo.SUPPLIER_ID = itemBid.SUPPLIER_ID;
                                    mediAdo.SUPPLIER_CODE = itemBid.SUPPLIER_CODE;
                                    mediAdo.SUPPLIER_NAME = itemBid.SUPPLIER_NAME;
                                    mediAdo.BID_ID = itemBid.BID_ID;
                                    medicineTypeDataSource.Add(mediAdo);
                                    //addMedicine(itemBid, medicineAdd);
                                }
                            }
                            this.medicineTypeProcessor.Reload(this.ucMedicineType, medicineTypeDataSource);
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
        private void FillDataToGridMaterialType()
        {
            try
            {
                WaitingManager.Show();

                if (this.currentAnticipate == null)
                {
                    MOS.Filter.HisMaterialTypeViewFilter mateFilter = new MOS.Filter.HisMaterialTypeViewFilter();
                    mateFilter.IS_STOP_IMP = false;
                    mateFilter.IS_LEAF = true;
                    mateFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var listMaterialType = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_MATERIAL_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    List<HIS.UC.BidMaterialTypeGrid.MaterialTypeADO> MaterialTypeADOs = new List<HIS.UC.BidMaterialTypeGrid.MaterialTypeADO>();
                    if (listMaterialType != null && listMaterialType.Count > 0)
                    {
                        MaterialTypeADOs = (from r in listMaterialType select new HIS.UC.BidMaterialTypeGrid.MaterialTypeADO(r)).ToList();
                    }

                    this.materialTypeProcessor.Reload(this.ucMaterialType, MaterialTypeADOs);
                    Base.GlobalConfig.HisMaterialTypes = listMaterialType;
                }
                else
                {
                    CommonParam param = new CommonParam();
                    HisBidMaterialTypeViewFilter bidMaterialFilter = new HisBidMaterialTypeViewFilter();
                    listBidMaterialTypes = new BackendAdapter(param).Get<List<V_HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/GetView", ApiConsumer.ApiConsumers.MosConsumer, bidMaterialFilter, param);

                    if (listBidMaterialTypes != null && listBidMaterialTypes.Count > 0)
                    {
                        if (Base.GlobalConfig.HisMaterialTypes != null && Base.GlobalConfig.HisMaterialTypes.Count > 0)
                        {
                            List<HIS.UC.BidMaterialTypeGrid.MaterialTypeADO> materialTypeDataSource = new List<HIS.UC.BidMaterialTypeGrid.MaterialTypeADO>();
                            foreach (var itemBid in listBidMaterialTypes)
                            {
                                var materialAdd = Base.GlobalConfig.HisMaterialTypes.FirstOrDefault(o => o.ID == itemBid.MATERIAL_TYPE_ID);
                                if (materialAdd != null)
                                {
                                    HIS.UC.BidMaterialTypeGrid.MaterialTypeADO mateAdo = new UC.BidMaterialTypeGrid.MaterialTypeADO(materialAdd);
                                    mateAdo.AllowAmount = itemBid.AMOUNT - (itemBid.IN_AMOUNT != null ? itemBid.IN_AMOUNT : 0) + (itemBid.ADJUST_AMOUNT ?? 0) + (itemBid.AMOUNT * itemBid.IMP_MORE_RATIO ?? 0);
                                    mateAdo.IMP_PRICE = itemBid.IMP_PRICE;
                                    mateAdo.SUPPLIER_ID = itemBid.SUPPLIER_ID;
                                    mateAdo.SUPPLIER_CODE = itemBid.SUPPLIER_CODE;
                                    mateAdo.SUPPLIER_NAME = itemBid.SUPPLIER_NAME;
                                    mateAdo.BID_ID = itemBid.BID_ID;
                                    materialTypeDataSource.Add(mateAdo);
                                }
                            }
                            this.materialTypeProcessor.Reload(this.ucMaterialType, materialTypeDataSource);
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

        private void FocusTab()
        {
            try
            {
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    medicineTypeProcessor.FocusKeyword(ucMedicineType);
                    medicineTypeProcessor.ResetKeyword(ucMedicineType);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1) // vat tu
                {
                    materialTypeProcessor.FocusKeyword(ucMaterialType);
                    materialTypeProcessor.ResetKeyword(ucMaterialType);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2) // Mau
                {
                    bloodTypeProcessor.FocusKeyword(ucBloodType);
                    //bloodTypeProcessor.ResetKeyword(ucBloodType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region grid Left
        private void InitializeMedicineType()
        {
            try
            {
                medicineTypeProcessor = new UC.BidMedicineTypeGrid.UCBidMedicineTypeGridProcessor();
                HIS.UC.BidMedicineTypeGrid.ADO.BidMedicineTypeGridInitADO ado = new UC.BidMedicineTypeGrid.ADO.BidMedicineTypeGridInitADO();
                ado.IsShowSearchPanel = true;
                ado.BidMedicineTypeGridGrid_Click = MedicineType_Click;
                ado.BidMedicineTypeGridGrid_Enter = MedicineType_RowEnter;
                ado.ListBidMedicineTypeGridColumn = new List<HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn>();

                //Column MedicineTypeCode
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcMedicineTypeCode = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "MEDICINE_TYPE_CODE", 80, false);
                GcMedicineTypeCode.VisibleIndex = 0;
                ado.ListBidMedicineTypeGridColumn.Add(GcMedicineTypeCode);

                //Column MedicineTypeName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcMedicineTypeName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "MEDICINE_TYPE_NAME", 150, false);
                GcMedicineTypeName.VisibleIndex = 1;
                ado.ListBidMedicineTypeGridColumn.Add(GcMedicineTypeName);

                //Column ServiceUnitName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcServiceUnitName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "SERVICE_UNIT_NAME", 80, false);
                GcServiceUnitName.VisibleIndex = 2;
                ado.ListBidMedicineTypeGridColumn.Add(GcServiceUnitName);

                //Column AMount
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcAmountName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    ("SL nhập còn lại", "AllowAmount", 100, false);
                GcAmountName.VisibleIndex = 3;
                ado.ListBidMedicineTypeGridColumn.Add(GcAmountName);

                //Column ManufacturerName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcManufacturerName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 4;
                ado.ListBidMedicineTypeGridColumn.Add(GcManufacturerName);

                //Column SĐK
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcSDK = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn("Số đăng ký", "REGISTER_NUMBER", 80, false);
                GcSDK.VisibleIndex = 5;
                ado.ListBidMedicineTypeGridColumn.Add(GcSDK);

                //Column NationalName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcNationalName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 6;
                ado.ListBidMedicineTypeGridColumn.Add(GcNationalName);

                //Column UseFromName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcUseFromName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_MEDICINE__GC_MEDICINE_USE_FROM_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "MEDICINE_USE_FORM_NAME", 80, false);
                GcUseFromName.VisibleIndex = 7;
                ado.ListBidMedicineTypeGridColumn.Add(GcUseFromName);

                //Column AvtiveIngrBhytName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcAvtiveIngrBhytName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "ACTIVE_INGR_BHYT_NAME", 100, false);
                GcAvtiveIngrBhytName.VisibleIndex = 8;
                ado.ListBidMedicineTypeGridColumn.Add(GcAvtiveIngrBhytName);

                this.ucMedicineType = (UserControl)medicineTypeProcessor.Run(ado);
                if (this.ucMedicineType != null)
                {
                    this.panelControlMedicineType.Controls.Add(this.ucMedicineType);
                    this.ucMedicineType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineType_RowEnter(UC.BidMedicineTypeGrid.MedicineTypeADO data)
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

        private void MedicineType_Click(UC.BidMedicineTypeGrid.MedicineTypeADO data)
        {
            try
            {
                this.medicineType = new ADO.MedicineTypeADO();
                if (data != null)
                {
                    ADO.MedicineTypeADO medicine = new ADO.MedicineTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicine, data);

                    this.ActionType = GlobalVariables.ActionAdd;
                    VisibleButton(this.ActionType);
                    this.medicineType = medicine;
                    this.medicineType.Type = Base.GlobalConfig.THUOC;
                    if (data.SUPPLIER_ID != null)
                    {
                        cboSupplier.EditValue = data.SUPPLIER_ID;
                        txtSupplierCode.Text = data.SUPPLIER_CODE;
                    }
                    else
                    {
                        cboSupplier.EditValue = null;
                        txtSupplierCode.Text = "";
                    }
                    if (data.BID_ID != null)
                    {
                        cboBid.EditValue = data.BID_ID;
                        cboBid.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        cboBid.EditValue = null;
                        cboBid.Properties.Buttons[1].Visible = false;
                    }
                    spinAmount.Value = 0;
                    spinImpPrice.Value = (data.IMP_PRICE ?? 0);
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    dxValidationProviderLeftPanel.RemoveControlError(spinImpPrice);
                    dxValidationProviderLeftPanel.RemoveControlError(spinAmount);
                }
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
                materialTypeProcessor = new UC.BidMaterialTypeGrid.UCBidMaterialTypeGridProcessor();
                HIS.UC.BidMaterialTypeGrid.ADO.BidMaterialTypeGridInitADO ado = new HIS.UC.BidMaterialTypeGrid.ADO.BidMaterialTypeGridInitADO();
                ado.IsShowSearchPanel = true;
                ado.BidMaterialTypeGridGrid_Click = MaterialType_Click;
                ado.BidMaterialTypeGridGrid_Enter = MaterialType_RowEnter;
                ado.ListBidMaterialTypeGridColumn = new List<HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn>();

                //Column MaterialTypeCode
                HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn GcMaterialTypeCode = new HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_MATERIAL__GC_MATERIAL_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "MATERIAL_TYPE_CODE", 80, false);
                GcMaterialTypeCode.VisibleIndex = 0;
                ado.ListBidMaterialTypeGridColumn.Add(GcMaterialTypeCode);

                //Column MaterialTypeName
                HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn GcMaterialTypeName = new HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_MATERIAL__GC_MATERIAL_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "MATERIAL_TYPE_NAME", 150, false);
                GcMaterialTypeName.VisibleIndex = 1;
                ado.ListBidMaterialTypeGridColumn.Add(GcMaterialTypeName);

                //Column ServiceUnitName
                HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn GcServiceUnitName = new HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "SERVICE_UNIT_NAME", 80, false);
                GcServiceUnitName.VisibleIndex = 2;
                ado.ListBidMaterialTypeGridColumn.Add(GcServiceUnitName);

                //Column Amount
                HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn GcAmount = new HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn
                    ("SL nhập còn lại", "AllowAmount", 100, false);
                GcAmount.VisibleIndex = 3;
                ado.ListBidMaterialTypeGridColumn.Add(GcAmount);

                //Column ManufacturerName
                HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn GcManufacturerName = new HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 4;
                ado.ListBidMaterialTypeGridColumn.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn GcNationalName = new HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 5;
                ado.ListBidMaterialTypeGridColumn.Add(GcNationalName);

                //ado.MaterialTypes = Base.GlobalConfig.HisMaterialTypes;
                this.ucMaterialType = (UserControl)materialTypeProcessor.Run(ado);
                if (this.ucMaterialType != null)
                {
                    this.panelControlMaterialType.Controls.Add(this.ucMaterialType);
                    this.ucMaterialType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialType_Click(UC.BidMaterialTypeGrid.MaterialTypeADO data)
        {
            try
            {
                this.materialType = new ADO.MaterialTypeADO();
                if (data != null)
                {
                    ADO.MaterialTypeADO material = new ADO.MaterialTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MaterialTypeADO>(material, data);

                    this.ActionType = GlobalVariables.ActionAdd;
                    VisibleButton(this.ActionType);
                    this.materialType = material;
                    this.materialType.Type = Base.GlobalConfig.VATTU;
                    if (data.SUPPLIER_ID != null)
                    {
                        cboSupplier.EditValue = data.SUPPLIER_ID;
                        txtSupplierCode.Text = data.SUPPLIER_CODE;
                    }
                    else
                    {
                        cboSupplier.EditValue = null;
                        txtSupplierCode.Text = "";
                    }
                    if (data.BID_ID != null)
                    {
                        cboBid.EditValue = data.BID_ID;
                        cboBid.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        cboBid.EditValue = null;
                        cboBid.Properties.Buttons[1].Visible = false;
                    }
                    spinAmount.Value = 0;
                    spinImpPrice.Value = (material.IMP_PRICE ?? 0);
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    dxValidationProviderLeftPanel.RemoveControlError(spinImpPrice);
                    dxValidationProviderLeftPanel.RemoveControlError(spinAmount);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialType_RowEnter(UC.BidMaterialTypeGrid.MaterialTypeADO data)
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
                bloodTypeProcessor = new UC.BidBloodTypeGrid.UCBidBloodTypeGridProcessor();
                HIS.UC.BidBloodTypeGrid.ADO.BidBloodTypeGridInitADO ado = new HIS.UC.BidBloodTypeGrid.ADO.BidBloodTypeGridInitADO();
                ado.IsShowSearchPanel = true;
                ado.BidBloodTypeGridGrid_Click = BloodType_Click;
                ado.BidBloodTypeGridGrid_Enter = BloodType_RowEnter;
                ado.ListBidBloodTypeGridColumn = new List<UC.BidBloodTypeGrid.BidBloodTypeGridColumn>();

                //Column BloodTypeCode
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcBloodTypeCode = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_BLOOD__GC_BLOOD_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "BLOOD_TYPE_CODE", 80, false);
                GcBloodTypeCode.VisibleIndex = 0;
                ado.ListBidBloodTypeGridColumn.Add(GcBloodTypeCode);

                //Column BloodTypeName
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcBloodTypeName = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_BLOOD__GC_BLOOD_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "BLOOD_TYPE_NAME", 150, false);
                GcBloodTypeName.VisibleIndex = 1;
                ado.ListBidBloodTypeGridColumn.Add(GcBloodTypeName);

                //Column ServiceUnitName
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcServiceUnitName = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "SERVICE_UNIT_NAME", 80, false);
                GcServiceUnitName.VisibleIndex = 2;
                ado.ListBidBloodTypeGridColumn.Add(GcServiceUnitName);

                //Column Amount
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcAmount = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    ("SL nhập còn lại", "AllowAmount", 100, false);
                GcAmount.VisibleIndex = 3;
                ado.ListBidBloodTypeGridColumn.Add(GcAmount);

                //column volume
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcVolume = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_BLOOD__GC_VOLUME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "VOLUME", 60, false);
                GcVolume.VisibleIndex = 4;
                ado.ListBidBloodTypeGridColumn.Add(GcVolume);

                //Column ManufacturerName
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcManufacturerName = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 5;
                ado.ListBidBloodTypeGridColumn.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcNationalName = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateUpdate,
                    cultureLang), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 6;
                ado.ListBidBloodTypeGridColumn.Add(GcNationalName);

                this.ucBloodType = (UserControl)bloodTypeProcessor.Run(ado);
                if (this.ucBloodType != null)
                {
                    this.panelControlBloodType.Controls.Add(this.ucBloodType);
                    this.ucBloodType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BloodType_Click(UC.BidBloodTypeGrid.BloodTypeADO data)
        {
            try
            {
                this.bloodType = new ADO.BloodTypeADO();
                if (data != null)
                {
                    ADO.BloodTypeADO blood = new ADO.BloodTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.BloodTypeADO>(blood, data);

                    this.ActionType = GlobalVariables.ActionAdd;
                    VisibleButton(this.ActionType);
                    this.bloodType = blood;
                    this.bloodType.Type = Base.GlobalConfig.MAU;
                    if (data.SUPPLIER_ID != null)
                    {
                        cboSupplier.EditValue = data.SUPPLIER_ID;
                        txtSupplierCode.Text = data.SUPPLIER_CODE;
                    }
                    else
                    {
                        cboSupplier.EditValue = null;
                        txtSupplierCode.Text = "";
                    }
                    spinAmount.Value = 0;
                    spinImpPrice.Value = (blood.IMP_PRICE ?? 0);
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    dxValidationProviderLeftPanel.RemoveControlError(spinImpPrice);
                    dxValidationProviderLeftPanel.RemoveControlError(spinAmount);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BloodType_RowEnter(UC.BidBloodTypeGrid.BloodTypeADO data)
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
        #endregion

        private void EnableButton(int action)
        {
            try
            {
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                btnImport.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                btnSave.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                //btnNew.Enabled = (action == GlobalVariables.ActionView);
                btnPrint.Enabled = (action == GlobalVariables.ActionView);
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
                    emptySpaceItemForEdit.Size = new Size(root.Width - 110, 26);
                    lciBtnAdd.Size = new Size(110, 26);
                }
                else
                {
                    emptySpaceItemForEdit.Size = new Size(root.Width - 220, 26);
                    lciBtnUpdate.Size = new Size(110, 26);
                    lciBtnDiscard.Size = new Size(110, 26);
                }
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
                cboSupplier.Properties.DataSource = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().OrderByDescending(o => o.CREATE_TIME).ToList();
                cboSupplier.Properties.DisplayMember = "SUPPLIER_NAME";
                cboSupplier.Properties.ValueMember = "ID";
                cboSupplier.Properties.TextEditStyle = TextEditStyles.Standard;
                //cboSupplier.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Default;
                cboSupplier.Properties.ImmediatePopup = true;
                cboSupplier.ForceInitialize();
                cboSupplier.Properties.View.Columns.Clear();
                cboSupplier.Properties.PopupFormSize = new Size(500, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboSupplier.Properties.View.Columns.AddField("SUPPLIER_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboSupplier.Properties.View.Columns.AddField("SUPPLIER_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 440;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboBid()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBidFilter bidFilter = new HisBidFilter();
                bidFilter.IS_ACTIVE = 1;
                var data = new BackendAdapter(param).Get<List<HIS_BID>>("api/HisBid/Get", ApiConsumer.ApiConsumers.MosConsumer, bidFilter, param);
                cboBid.Properties.DataSource = data;
                cboBid.Properties.DisplayMember = "BID_NAME";
                cboBid.Properties.ValueMember = "ID";
                cboBid.Properties.TextEditStyle = TextEditStyles.Standard;
                //cboSupplier.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Default;
                cboBid.Properties.ImmediatePopup = true;
                cboBid.ForceInitialize();
                cboBid.Properties.View.Columns.Clear();
                cboBid.Properties.PopupFormSize = new Size(500, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboBid.Properties.View.Columns.AddField("BID_NAME");
                aColumnCode.Caption = "Tên thầu";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 500;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewProcess_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ADO.MedicineTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TYPE_DISPLAY")
                        {
                            if (data.Type == Base.GlobalConfig.THUOC)
                            {
                                e.Value = "Thuốc";
                            }
                            else if (data.Type == Base.GlobalConfig.VATTU)
                            {
                                e.Value = "Vật tư";
                            }
                            else if (data.Type == Base.GlobalConfig.MAU)
                            {
                                e.Value = "Máu";
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

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (!checkLoadBid)
                {
                    if (xtraTabControl1.SelectedTabPageIndex == 0)//thuốc
                    {
                        medicineTypeProcessor.FocusKeyword(ucMedicineType);
                    }
                    else if (xtraTabControl1.SelectedTabPageIndex == 1) // vat tu
                    {
                        if (showMaterial)
                        {
                            FillDataToGridMaterialType();
                            showMaterial = false;
                        }
                    }
                    else if (xtraTabControl1.SelectedTabPageIndex == 2) // Mau
                    {
                        if (showBlood)
                        {
                            MOS.Filter.HisBloodTypeViewFilter bloodFilter = new MOS.Filter.HisBloodTypeViewFilter();
                            bloodFilter.IS_LEAF = true;
                            bloodFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                            var listBloodType = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_BLOOD_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bloodFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                            List<HIS.UC.BidBloodTypeGrid.BloodTypeADO> BloodTypeADOs = new List<HIS.UC.BidBloodTypeGrid.BloodTypeADO>();
                            if (listBloodType != null && listBloodType.Count > 0)
                            {
                                BloodTypeADOs = (from r in listBloodType select new HIS.UC.BidBloodTypeGrid.BloodTypeADO(r)).ToList();
                            }

                            this.bloodTypeProcessor.Reload(this.ucBloodType, BloodTypeADOs);
                            Base.GlobalConfig.HisBloodTypes = listBloodType;
                            showBlood = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProviderLeftPanel_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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
        #endregion

        #region Public method
        public void Add()
        {
            try
            {
                if (btnAdd.Enabled) btnAdd_Click(null, null);
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
                if (btnSave.Enabled) btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void New()
        {
            try
            {
                if (btnNew.Enabled) btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Print()
        {
            try
            {
                if (btnPrint.Enabled) btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusUseTime()
        {
            try
            {
                txtUseTime.Focus();
                txtUseTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Update()
        {
            try
            {
                btnUpdate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Discard()
        {
            try
            {
                btnDiscard_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void cboBid_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboBid.EditValue != null)
                    {
                        cboBid.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadEditNotBid()
        {
            try
            {
                ListMedicineTypeAdoProcess = new List<ADO.MedicineTypeADO>();
                if (this.listAticipateMetys != null && this.listAticipateMetys.Count > 0)
                {
                    foreach (var mety in this.listAticipateMetys)
                    {
                        if (Base.GlobalConfig.HisMedicineTypes != null && Base.GlobalConfig.HisMedicineTypes.Count > 0)
                        {
                            List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO> medicineTypeDataSource = new List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO>();
                            var medicineAdd = Base.GlobalConfig.HisMedicineTypes.FirstOrDefault(o => o.ID == mety.MEDICINE_TYPE_ID);
                            if (medicineAdd != null)
                            {
                                addMedicine(mety, medicineAdd);
                            }
                        }
                    }
                }
                if (this.listAticipateMatys != null && this.listAticipateMatys.Count > 0)
                {
                    foreach (var maty in this.listAticipateMatys)
                    {
                        if (Base.GlobalConfig.HisMaterialTypes != null && Base.GlobalConfig.HisMaterialTypes.Count > 0)
                        {
                            List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO> medicineTypeDataSource = new List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO>();
                            var materialAdd = Base.GlobalConfig.HisMaterialTypes.FirstOrDefault(o => o.ID == maty.MATERIAL_TYPE_ID);
                            if (materialAdd != null)
                            {
                                addMaterial(maty, materialAdd);
                            }
                        }
                    }
                }
                if (this.listAticipateBltys != null && this.listAticipateBltys.Count > 0)
                {
                    foreach (var blty in this.listAticipateBltys)
                    {
                        if (Base.GlobalConfig.HisBloodTypes != null && Base.GlobalConfig.HisBloodTypes.Count > 0)
                        {
                            List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO> medicineTypeDataSource = new List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO>();
                            var bloodAdd = Base.GlobalConfig.HisBloodTypes.FirstOrDefault(o => o.ID == blty.BLOOD_TYPE_ID);
                            if (bloodAdd != null)
                            {
                                addBlood(blty, bloodAdd);
                            }
                        }
                    }
                }
                if (this.ListMedicineTypeAdoProcess != null && this.ListMedicineTypeAdoProcess.Count > 0)
                {
                    if (this.ListMedicineTypeAdoProcess.Exists(o => o.Type == Base.GlobalConfig.THUOC))
                    {
                        xtraTabControl1.SelectedTabPage = xtraTabPage1;
                    }
                    else if (this.ListMedicineTypeAdoProcess.Exists(o => o.Type == Base.GlobalConfig.VATTU))
                    {
                        xtraTabControl1.SelectedTabPage = xtraTabPage2;
                    }
                    else if (this.ListMedicineTypeAdoProcess.Exists(o => o.Type == Base.GlobalConfig.MAU))
                    {
                        xtraTabControl1.SelectedTabPage = xtraTabPage3;
                    }
                    else
                        xtraTabControl1.SelectedTabPage = xtraTabPage1;
                }

                gridControlProcess.BeginUpdate();
                gridControlProcess.DataSource = null;
                gridControlProcess.DataSource = this.ListMedicineTypeAdoProcess;
                gridControlProcess.EndUpdate();
                this.ActionType = GlobalVariables.ActionAdd;
                VisibleButton(this.ActionType);
                dxValidationProviderLeftPanel.RemoveControlError(spinImpPrice);
                dxValidationProviderLeftPanel.RemoveControlError(spinAmount);
                setValueAfterAdd();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboBid_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (cboBid.EditValue != null)
                //{
                //    checkLoadBid = true;
                //    LoadBidPackageToGrid();
                //    cboSupplier.EditValue = null;
                //    cboSupplier.Enabled = false;
                //    txtSupplierCode.Text = "";
                //    txtSupplierCode.Enabled = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewProcess_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                var row = (ADO.MedicineTypeADO)gridViewProcess.GetFocusedRow();
                GridView view = sender as GridView;
                if (view.FocusedColumn.FieldName == "AMOUNT")
                {
                    if (Inventec.Common.TypeConvert.Parse.ToDecimal(e.Value.ToString()) < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Số lượng phải lớn hơn hoặc bằng 0";
                    }
                    else if (row != null && Inventec.Common.TypeConvert.Parse.ToDecimal(e.Value.ToString()) > row.AllowAmount)
                    {
                        e.Valid = false;
                        e.ErrorText = "Số lượng không được lớn hơn số lượng nhập còn lại";
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewProcess_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                //Loại xử lý khi xảy ra exception Hiển thị. k cho nhập
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.DisplayError;
                //Show thông báo lỗi ở cột
                gridViewProcess.SetColumnError(gridViewProcess.FocusedColumn, e.ErrorText, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewProcess_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var row = (ADO.MedicineTypeADO)gridViewProcess.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (row.isAmount || row.isDelete)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboBid_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBid.Properties.Buttons[1].Visible = false;
                    cboBid.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
