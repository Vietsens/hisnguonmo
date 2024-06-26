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
using HIS.Desktop.Utilities.Extensions;
using System.Threading;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.Plugins.AnticipateCreate.ADO;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.AnticipateCreate
{
    public partial class UCAnticipateCreate : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        internal int ActionType = 0;
        private double idRow = -1;
        int positionHandleLeft = -1;
        int positionHandleRight = -1;
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
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_ANTICIPATE currentAnticipate;
        HIS.Desktop.Common.DelegateRefreshData delegateRefresh;

        bool showMaterial = true;
        bool showBlood = true;
        bool checkLoadBid = false;
        public static bool isBusiness = false;

        System.Globalization.CultureInfo cultureLang;
        long RoomId;
        long DepartmentId;
        long mediStockId;

        long currentSupplierId = 0;
        long currentBidId = 0;

        List<HIS_MEDI_STOCK> mediStockSelecteds;

        List<HIS_BID> ListBid;
        List<HIS_SUPPLIER> ListSupplier;
        List<V_HIS_BID_MATERIAL_TYPE> ListBidMaterialType;
        List<V_HIS_BID_MEDICINE_TYPE> ListBidMedicineType;

        List<V_HIS_BID_MEDICINE_TYPE> bidMedicineTypes;
        List<V_HIS_BID_MATERIAL_TYPE> bidMaterialTypes;

        List<ADO.BidADO> lstBidAdo;

        List<HIS_MEDICINE> ListMedicine;
        List<HIS_MATERIAL> ListMaterial;
        Inventec.Desktop.Common.Modules.Module moduleData;
        public HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        public List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isInit;
        #endregion

        #region Construct
        public UCAnticipateCreate()
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

        public UCAnticipateCreate(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = _moduleData;
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                InitializeMedicineType();
                InitializeMaterialType();
                InitializeBloodType();
                RoomId = _moduleData != null ? _moduleData.RoomId : 0;
                var workPlace = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomTypeId == (_moduleData != null ? _moduleData.RoomTypeId : 0) && o.RoomId == RoomId);
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


        public UCAnticipateCreate(Inventec.Desktop.Common.Modules.Module module, V_HIS_ANTICIPATE anticipate, HIS.Desktop.Common.DelegateRefreshData refresh)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                InitializeMedicineType();
                InitializeMaterialType();
                InitializeBloodType();
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
                BtnAutoFillGrid.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCAnticipateCreate_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MODULE__________: ", this.moduleData));

                //Gan ngon ngu
                LoadKeysFromlanguage();

                InitControlState();

                CreateThreadLoadData();

                InitSupplierAndBid(chkItemType.Checked);

                //load cbo kho nhan
                InitComboReceiveMediStock();
                //load cbo kho
                InitMediStockCheck();
                InitComboMediStock();

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
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__XTRA_TAB_MATERIAL",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__XTRA_TAB_BLOOD",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.lciSupplierCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__LCI_SUPPLIER",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__LCI_AMOUNT",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__LCI_DESCRIPTION",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.lciImpPice.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__LCI_IMP_PICE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.lciUseTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__LCI_USE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_ADD",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.btnNew.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_NEW",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.btnUpdate.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_UPDATE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.btnDiscard.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__BTN_PRINT",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.txtUseTime.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__TXT_USE_TIME_FOCUS",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                #endregion
                #region GRID PROCESS
                this.GvProcess_GcActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_ACTIVE_INGR_BHYT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcAmount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_AMOUNT",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcConcentra.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcImpPrice.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_IMP_PRICE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcManufactureName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS_GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS_GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcNationalName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcPackingTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_PACKING_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcServiceUnitName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcSTT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_STT",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcSupplierName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_SUPPLIER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.GvProcess_GcTypeDisplay.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_PROCESS__GC_TYPE_DISPLAY",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.ButtonDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__TOOL_TIP__BUTTON_DELETE",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang);
                this.ButtonEdit.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__TOOL_TIP__BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
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
                    //    LoadEditNotBid();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitControlState()
        {
            this.isInit = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData("HIS.Desktop.Plugins.AnticipateCreate");
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkItemType.Name)
                        {
                            chkItemType.Checked = item.VALUE == "1";
                        }

                        else if (item.KEY == chkSupply.Name)
                        {
                            chkSupply.Checked = item.VALUE == "1";
                        }
                    }
                }
                else
                {
                    this.isInit = false;
                    chkItemType.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            this.isInit = false;
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
                    btnNew.Enabled = false;
                    btnImport.Enabled = false;
                    cboBid.Enabled = false;
                    txtUseTime.Text = this.currentAnticipate.USE_TIME;
                    txtDescription.Text = this.currentAnticipate.DESCRIPTION;
                    if (this.currentAnticipate.RECEIVE_MEDI_STOCK_ID != null)
                    {
                        cboReceiveMediStock.EditValue = this.currentAnticipate.RECEIVE_MEDI_STOCK_ID;
                    }
                    else
                    {
                        cboReceiveMediStock.EditValue = null;
                    }
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
                txtSupplierCode.Enabled = true;
                txtDescription.Text = "";
                txtUseTime.Text = "";
                cboReceiveMediStock.EditValue = null;
                ListMedicineTypeAdoProcess = new List<ADO.MedicineTypeADO>();
                this.ActionType = GlobalVariables.ActionAdd;
                gridControlProcess.DataSource = null;
                xtraTabControl1.SelectedTabPageIndex = 0;
                DtTimeFrom.DateTime = DateTime.Now;
                DtTimeTo.DateTime = DateTime.Now;
                SpinCoefficient.EditValue = 100;
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
                txtSupplierCode1.Text = "";
                cboSupplier1.EditValue = null;
                cboSupplier1.Properties.Buttons[1].Visible = false;
                cboBid1.EditValue = null;
                cboBid1.Properties.Buttons[1].Visible = false;
                SetDefaultValueControlLeftDown();
                EnableButton(this.ActionType);
                VisibleButton(this.ActionType);
                InitSupplierAndBid(chkItemType.Checked);
                FocusTab();
                materialTypeProcessor.ResetKeyword(ucMaterialType);
                showBlood = true;
                showMaterial = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultValueControlLeftDown()
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
                gridControlBid.DataSource = null;
                cboBid.Properties.DataSource = null;
                cboSupplier.Properties.DataSource = null;
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
                    mediFilter.IS_BUSINESS = isBusiness;
                    mediFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    var listMedicineType = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_MEDICINE_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO> MedicineTypeADOs = new List<UC.BidMedicineTypeGrid.MedicineTypeADO>();

                    if (isBusiness == false && ListBidMedicineType != null && ListBidMedicineType.Count > 0)
                    {
                        List<V_HIS_BID_MEDICINE_TYPE> listBidMedicineType = new List<V_HIS_BID_MEDICINE_TYPE>();
                        if (currentSupplierId > 0 && currentBidId == 0)
                        {
                            listBidMedicineType = ListBidMedicineType.Where(o => o.SUPPLIER_ID == currentSupplierId).ToList();
                        }
                        else if (currentSupplierId == 0 && currentBidId > 0)
                        {
                            listBidMedicineType = ListBidMedicineType.Where(o => o.BID_ID == currentBidId).ToList();
                        }
                        else if (currentSupplierId > 0 && currentBidId > 0)
                        {
                            listBidMedicineType = ListBidMedicineType.Where(o => o.SUPPLIER_ID == currentSupplierId && o.BID_ID == currentBidId).ToList();
                        }
                        else
                        {
                            listBidMedicineType = ListBidMedicineType;
                        }
                        if (listBidMedicineType != null && listBidMedicineType.Count > 0)
                        {
                            int dem = 0;
                            foreach (var itemBid in listBidMedicineType)
                            {
                                var checkBid = ListBid != null && ListBid.Count > 0 ? ListBid.FirstOrDefault(o => o.ID == itemBid.BID_ID) : null;

                                if (checkBid != null && checkBid.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    if (dem > 0)
                                    {
                                        if (MedicineTypeADOs.FirstOrDefault(o => o.ID == itemBid.MEDICINE_TYPE_ID) != null)
                                        {
                                            MedicineTypeADOs.FirstOrDefault(o => o.ID == itemBid.MEDICINE_TYPE_ID).AllowAmount += itemBid.AMOUNT - (itemBid.IN_AMOUNT != null ? itemBid.IN_AMOUNT : 0) + (itemBid.ADJUST_AMOUNT ?? 0) + (itemBid.AMOUNT * (itemBid.IMP_MORE_RATIO ?? 0));
                                            continue;
                                        }
                                    }

                                    var medicineAdd = listMedicineType.FirstOrDefault(o => o.ID == itemBid.MEDICINE_TYPE_ID);
                                    if (medicineAdd != null)
                                    {
                                        HIS.UC.BidMedicineTypeGrid.MedicineTypeADO mediAdo = new UC.BidMedicineTypeGrid.MedicineTypeADO(medicineAdd);
                                        mediAdo.AllowAmount = itemBid.AMOUNT - (itemBid.IN_AMOUNT != null ? itemBid.IN_AMOUNT : 0) + (itemBid.ADJUST_AMOUNT ?? 0) + (itemBid.AMOUNT * (itemBid.IMP_MORE_RATIO ?? 0));
                                        mediAdo.IMP_PRICE = itemBid.IMP_PRICE;
                                        mediAdo.SUPPLIER_ID = itemBid.SUPPLIER_ID;
                                        mediAdo.SUPPLIER_CODE = itemBid.SUPPLIER_CODE;
                                        mediAdo.SUPPLIER_NAME = itemBid.SUPPLIER_NAME;
                                        if (mediAdo.AllowAmount.HasValue && mediAdo.AllowAmount.Value > 0)
                                        {
                                            dem++;
                                            MedicineTypeADOs.Add(mediAdo);
                                        }
                                        //addMedicine(itemBid, medicineAdd);
                                    }
                                }
                            }
                        }

                    }
                    else if (isBusiness == true)
                    {
                        foreach (var item in listMedicineType)
                        {
                            HIS.UC.BidMedicineTypeGrid.MedicineTypeADO medicineAdo = new HIS.UC.BidMedicineTypeGrid.MedicineTypeADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeADO>(medicineAdo, item);
                            MedicineTypeADOs.Add(medicineAdo);
                        }
                    }
                    this.medicineTypeProcessor.Reload(this.ucMedicineType, MedicineTypeADOs);
                    Base.GlobalConfig.HisMedicineTypes = listMedicineType;

                }
                else
                {
                    if (ListBidMedicineType != null && ListBidMedicineType.Count > 0 && isBusiness == false)
                    {
                        List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO> medicineTypeDataSource = new List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO>();
                        int dem = 0;
                        foreach (var itemBid in ListBidMedicineType)
                        {
                            var checkBid = ListBid != null && ListBid.Count > 0 ? ListBid.FirstOrDefault(o => o.ID == itemBid.BID_ID) : null;

                            if (checkBid != null && checkBid.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                if (dem > 0)
                                {
                                    if (medicineTypeDataSource.FirstOrDefault(o => o.ID == itemBid.MEDICINE_TYPE_ID) != null)
                                    {
                                        continue;
                                    }
                                }

                                var medicineAdd = Base.GlobalConfig.HisMedicineTypes.FirstOrDefault(o => o.ID == itemBid.MEDICINE_TYPE_ID);
                                if (medicineAdd != null)
                                {
                                    HIS.UC.BidMedicineTypeGrid.MedicineTypeADO mediAdo = new UC.BidMedicineTypeGrid.MedicineTypeADO(medicineAdd);
                                    mediAdo.AllowAmount = itemBid.AMOUNT - (itemBid.IN_AMOUNT != null ? itemBid.IN_AMOUNT : 0) + (itemBid.ADJUST_AMOUNT ?? 0) + (itemBid.AMOUNT * (itemBid.IMP_MORE_RATIO ?? 0));
                                    mediAdo.IMP_PRICE = itemBid.IMP_PRICE;
                                    mediAdo.SUPPLIER_ID = itemBid.SUPPLIER_ID;
                                    mediAdo.SUPPLIER_CODE = itemBid.SUPPLIER_CODE;
                                    mediAdo.SUPPLIER_NAME = itemBid.SUPPLIER_NAME;
                                    if (mediAdo.AllowAmount.HasValue && mediAdo.AllowAmount.Value > 0)
                                    {
                                        dem++;
                                        medicineTypeDataSource.Add(mediAdo);
                                    }
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
                MOS.Filter.HisMaterialTypeViewFilter mateFilter = new MOS.Filter.HisMaterialTypeViewFilter();
                mateFilter.IS_STOP_IMP = false;
                mateFilter.IS_LEAF = true;
                mateFilter.IS_BUSINESS = isBusiness;
                mateFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var listMaterialType = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_MATERIAL_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                List<HIS.UC.BidMaterialTypeGrid.MaterialTypeADO> MaterialTypeADOs = new List<HIS.UC.BidMaterialTypeGrid.MaterialTypeADO>();
                if (ListBidMaterialType != null && ListBidMaterialType.Count() > 0 && isBusiness == false)
                {
                    List<V_HIS_BID_MATERIAL_TYPE> listBidMaterialType = new List<V_HIS_BID_MATERIAL_TYPE>();
                    if (currentSupplierId > 0 && currentBidId == 0)
                    {
                        listBidMaterialType = ListBidMaterialType.Where(o => o.SUPPLIER_ID == currentSupplierId).ToList();
                    }
                    else if (currentSupplierId == 0 && currentBidId > 0)
                    {
                        listBidMaterialType = ListBidMaterialType.Where(o => o.BID_ID == currentBidId).ToList();
                    }
                    else if (currentSupplierId > 0 && currentBidId > 0)
                    {
                        listBidMaterialType = ListBidMaterialType.Where(o => o.SUPPLIER_ID == currentSupplierId && o.BID_ID == currentBidId).ToList();
                    }
                    else
                    {
                        listBidMaterialType = ListBidMaterialType;
                    }
                    if (listBidMaterialType != null && listBidMaterialType.Count > 0)
                    {
                        int dem = 0;
                        foreach (var itemBid in listBidMaterialType)
                        {
                            HIS_BID checkBid = ListBid != null && ListBid.Count > 0 ? ListBid.FirstOrDefault(o => o.ID == itemBid.BID_ID) : null;

                            if (checkBid != null && checkBid.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                if (dem > 0)
                                {
                                    if (MaterialTypeADOs.FirstOrDefault(o => o.ID == itemBid.MATERIAL_TYPE_ID) != null)
                                    {
                                        MaterialTypeADOs.FirstOrDefault(o => o.ID == itemBid.MATERIAL_TYPE_ID).AllowAmount += itemBid.AMOUNT - (itemBid.IN_AMOUNT != null ? itemBid.IN_AMOUNT : 0) + (itemBid.ADJUST_AMOUNT ?? 0) + (itemBid.AMOUNT * (itemBid.IMP_MORE_RATIO ?? 0));
                                        continue;
                                    }
                                }
                                var materialAdd = listMaterialType.FirstOrDefault(o => o.ID == itemBid.MATERIAL_TYPE_ID);
                                if (materialAdd != null)
                                {
                                    HIS.UC.BidMaterialTypeGrid.MaterialTypeADO mateAdo = new HIS.UC.BidMaterialTypeGrid.MaterialTypeADO(materialAdd);
                                    mateAdo.AllowAmount = itemBid.AMOUNT - (itemBid.IN_AMOUNT != null ? itemBid.IN_AMOUNT : 0) + (itemBid.ADJUST_AMOUNT ?? 0) + (itemBid.AMOUNT * (itemBid.IMP_MORE_RATIO ?? 0));
                                    mateAdo.IMP_PRICE = itemBid.IMP_PRICE;
                                    mateAdo.SUPPLIER_ID = itemBid.SUPPLIER_ID;
                                    mateAdo.SUPPLIER_CODE = itemBid.SUPPLIER_CODE;
                                    mateAdo.SUPPLIER_NAME = itemBid.SUPPLIER_NAME;
                                    if (mateAdo.AllowAmount.HasValue && mateAdo.AllowAmount > 0)
                                    {
                                        dem++;
                                        MaterialTypeADOs.Add(mateAdo);
                                    }

                                }
                            }
                        }
                    }
                }
                else if (isBusiness == true)
                {
                    foreach (var item in listMaterialType)
                    {
                        HIS.UC.BidMaterialTypeGrid.MaterialTypeADO materialAdo = new HIS.UC.BidMaterialTypeGrid.MaterialTypeADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeADO>(materialAdo, item);
                        MaterialTypeADOs.Add(materialAdo);

                    }
                }

                this.materialTypeProcessor.Reload(this.ucMaterialType, MaterialTypeADOs);

                Base.GlobalConfig.HisMaterialTypes = listMaterialType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "MEDICINE_TYPE_CODE", 80, false);
                GcMedicineTypeCode.VisibleIndex = 0;
                ado.ListBidMedicineTypeGridColumn.Add(GcMedicineTypeCode);

                //Column MedicineTypeName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcMedicineTypeName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "MEDICINE_TYPE_NAME", 150, false);
                GcMedicineTypeName.VisibleIndex = 1;
                ado.ListBidMedicineTypeGridColumn.Add(GcMedicineTypeName);

                //Column AvtiveIngrBhytName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcAvtiveIngrBhytName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "ACTIVE_INGR_BHYT_NAME", 100, false);
                GcAvtiveIngrBhytName.VisibleIndex = 2;
                ado.ListBidMedicineTypeGridColumn.Add(GcAvtiveIngrBhytName);

                //Column ServiceUnitName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcServiceUnitName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "SERVICE_UNIT_NAME", 80, false);
                GcServiceUnitName.VisibleIndex = 3;
                ado.ListBidMedicineTypeGridColumn.Add(GcServiceUnitName);

                //Column AMount
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcAmountName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    ("SL nhập còn lại", "AllowAmount", 100, false);
                GcAmountName.VisibleIndex = 4;
                ado.ListBidMedicineTypeGridColumn.Add(GcAmountName);

                //Column ManufacturerName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcManufacturerName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 5;
                ado.ListBidMedicineTypeGridColumn.Add(GcManufacturerName);

                //Column SĐK
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcSDK = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn("Số đăng ký", "REGISTER_NUMBER", 80, false);
                GcSDK.VisibleIndex = 6;
                ado.ListBidMedicineTypeGridColumn.Add(GcSDK);

                //Column NationalName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcNationalName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 7;
                ado.ListBidMedicineTypeGridColumn.Add(GcNationalName);

                //Column UseFromName
                HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn GcUseFromName = new HIS.UC.BidMedicineTypeGrid.BidMedicineTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_MEDICINE__GC_MEDICINE_USE_FROM_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "MEDICINE_USE_FORM_NAME", 80, false);
                GcUseFromName.VisibleIndex = 8;
                ado.ListBidMedicineTypeGridColumn.Add(GcUseFromName);

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

                    spinAmount.Value = 0;
                    spinImpPrice.Value = (data.IMP_PRICE ?? 0);
                    txtSupplierCode.Text = "";
                    cboSupplier.EditValue = null;
                    cboBid.EditValue = null;
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    dxValidationProviderLeftPanel.RemoveControlError(spinImpPrice);
                    dxValidationProviderLeftPanel.RemoveControlError(spinAmount);

                    lstBidAdo = new List<ADO.BidADO>();
                    bidMedicineTypes = new List<V_HIS_BID_MEDICINE_TYPE>();
                    bidMedicineTypes = ListBidMedicineType.Where(o => o.MEDICINE_TYPE_ID == data.ID).ToList();
                    if (bidMedicineTypes != null && bidMedicineTypes.Count > 0)
                    {
                        List<HIS_BID> hisBids = new List<HIS_BID>();

                        foreach (var item in bidMedicineTypes)
                        {
                            var bid = ListBid.FirstOrDefault(o => o.ID == item.BID_ID);
                            if (bid != null)
                            {
                                hisBids.Add(bid);

                                var medicines = ListMedicine.Where(o => o.MEDICINE_TYPE_ID == data.ID && o.BID_ID == bid.ID).ToList();
                                var ado = new ADO.BidADO();
                                ado.ID = bid.ID;
                                ado.BID_NAME = bid.BID_NAME;
                                ado.BID_NUMBER = bid.BID_NUMBER;
                                ado.BID_YEAR = bid.BID_YEAR;
                                ado.AMOUNT = item.AMOUNT + (item.ADJUST_AMOUNT ?? 0) + (item.AMOUNT * (item.IMP_MORE_RATIO ?? 0));
                                if (medicines != null && medicines.Count > 0)
                                {
                                    ado.AMOUNT -= medicines.Sum(s => s.AMOUNT);
                                }
                                ado.SUPPLIER_ID = item.SUPPLIER_ID;
                                lstBidAdo.Add(ado);
                            }
                        }

                        //load cob nha thau
                        LoadDataToCboSupplier(bidMedicineTypes.Select(o => o.SUPPLIER_ID).ToList());
                    }
                    else
                    {
                        LoadDataToCboSupplier(null);
                    }



                    //Load combo goi thau
                    LoadDataToCboBid(lstBidAdo);

                    ReloadGridBid(lstBidAdo);
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
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "MATERIAL_TYPE_CODE", 80, false);
                GcMaterialTypeCode.VisibleIndex = 0;
                ado.ListBidMaterialTypeGridColumn.Add(GcMaterialTypeCode);

                //Column MaterialTypeName
                HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn GcMaterialTypeName = new HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_MATERIAL__GC_MATERIAL_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "MATERIAL_TYPE_NAME", 150, false);
                GcMaterialTypeName.VisibleIndex = 1;
                ado.ListBidMaterialTypeGridColumn.Add(GcMaterialTypeName);

                //Column ServiceUnitName
                HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn GcServiceUnitName = new HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
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
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 4;
                ado.ListBidMaterialTypeGridColumn.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn GcNationalName = new HIS.UC.BidMaterialTypeGrid.BidMaterialTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
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

                    spinAmount.Value = 0;
                    spinImpPrice.Value = (material.IMP_PRICE ?? 0);
                    txtSupplierCode.Text = "";
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    dxValidationProviderLeftPanel.RemoveControlError(spinImpPrice);
                    dxValidationProviderLeftPanel.RemoveControlError(spinAmount);

                    lstBidAdo = new List<ADO.BidADO>();
                    var bidMaterialTypes = ListBidMaterialType.Where(o => o.MATERIAL_TYPE_ID == data.ID).ToList();
                    if (bidMaterialTypes != null && bidMaterialTypes.Count > 0)
                    {
                        List<HIS_BID> hisBids = new List<HIS_BID>();
                        foreach (var item in bidMaterialTypes)
                        {
                            var bid = ListBid.FirstOrDefault(o => o.ID == item.BID_ID);
                            if (bid != null)
                            {
                                hisBids.Add(bid);

                                var materials = ListMaterial.Where(o => o.MATERIAL_TYPE_ID == data.ID && o.BID_ID == bid.ID).ToList();
                                var ado = new ADO.BidADO();
                                ado.ID = bid.ID;
                                ado.BID_NAME = bid.BID_NAME;
                                ado.BID_NUMBER = bid.BID_NUMBER;
                                ado.BID_YEAR = bid.BID_YEAR;
                                ado.AMOUNT = item.AMOUNT + (item.ADJUST_AMOUNT ?? 0) + (item.AMOUNT * (item.IMP_MORE_RATIO ?? 0));
                                if (materials != null && materials.Count > 0)
                                {
                                    ado.AMOUNT -= materials.Sum(s => s.AMOUNT);
                                }
                                ado.SUPPLIER_ID = item.SUPPLIER_ID;
                                lstBidAdo.Add(ado);
                            }
                        }
                        //load cob nha thau
                        LoadDataToCboSupplier(bidMaterialTypes.Select(o => o.SUPPLIER_ID).ToList());
                    }
                    else
                    {
                        LoadDataToCboSupplier(null);
                    }

                    //Load combo goi thau
                    LoadDataToCboBid(lstBidAdo);

                    ReloadGridBid(lstBidAdo);
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
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "BLOOD_TYPE_CODE", 80, false);
                GcBloodTypeCode.VisibleIndex = 0;
                ado.ListBidBloodTypeGridColumn.Add(GcBloodTypeCode);

                //Column BloodTypeName
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcBloodTypeName = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GV_BLOOD__GC_BLOOD_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "BLOOD_TYPE_NAME", 150, false);
                GcBloodTypeName.VisibleIndex = 1;
                ado.ListBidBloodTypeGridColumn.Add(GcBloodTypeName);

                //Column ServiceUnitName
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcServiceUnitName = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
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
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "VOLUME", 60, false);
                GcVolume.VisibleIndex = 4;
                ado.ListBidBloodTypeGridColumn.Add(GcVolume);

                //Column ManufacturerName
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcManufacturerName = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 5;
                ado.ListBidBloodTypeGridColumn.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn GcNationalName = new HIS.UC.BidBloodTypeGrid.BidBloodTypeGridColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ANTICIPATE_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCAnticipateCreate,
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

                    spinAmount.Value = 0;
                    spinImpPrice.Value = (blood.IMP_PRICE ?? 0);
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    dxValidationProviderLeftPanel.RemoveControlError(spinImpPrice);
                    dxValidationProviderLeftPanel.RemoveControlError(spinAmount);

                    //load cob nha thau
                    LoadDataToCboSupplier(null);

                    //Load combo goi thau
                    LoadDataToCboBid(null);

                    ReloadGridBid(null);
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

        private void LoadDataToCboSupplier(List<long> SupplierIds)
        {
            try
            {
                var data = SupplierIds != null && SupplierIds.Count > 0 ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().Where(o => SupplierIds.Contains(o.ID)).OrderByDescending(o => o.CREATE_TIME).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SUPPLIER_CODE", "Mã", 60, 1));
                columnInfos.Add(new ColumnInfo("SUPPLIER_NAME", "Tên", 440, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SUPPLIER_NAME", "ID", columnInfos, false, 500);
                ControlEditorLoader.Load(cboSupplier, data, controlEditorADO);
                cboSupplier.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboBid(List<ADO.BidADO> listData)
        {
            try
            {
                if (listData != null && listData.Count > 0)
                {
                    listData = listData.Where(o => o.AMOUNT.HasValue && o.AMOUNT.Value != 0).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BID_NAME", "Tên", 500, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BID_NAME", "ID", columnInfos, false, 500);
                ControlEditorLoader.Load(cboBid, listData, controlEditorADO);
                cboBid.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboSupplier1(List<HIS_SUPPLIER> listSuppliers)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SUPPLIER_CODE", "Mã", 50, 1));
                columnInfos.Add(new ColumnInfo("SUPPLIER_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SUPPLIER_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboSupplier1, listSuppliers, controlEditorADO);
                cboSupplier1.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDataToCboBid1(List<HIS_BID> listData)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BID_NAME", "Tên", 300, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BID_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboBid1, listData, controlEditorADO);
                cboBid1.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitSupplierAndBid(bool isCheckItemType)
        {
            try
            {
                if (isCheckItemType) //Du tru theo loai mat hang
                {
                    //load combo nha thau
                    LoadDataToCboSupplier(null);

                    //Load combo goi thau
                    LoadDataToCboBid(null);
                }
                else //Du tru theo thau
                {
                    //load combo nha thau
                    LoadDataToCboSupplier1(ListSupplier);

                    //Load combo goi thau
                    LoadDataToCboBid1(ListBid);
                }
                DevExpress.XtraLayout.Utils.LayoutVisibility always = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                DevExpress.XtraLayout.Utils.LayoutVisibility never = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                //Hien thi vung theo Loai mat hang
                this.lciSupplierCode.Visibility = isCheckItemType ? always : never;
                this.lciSupplier.Visibility = isCheckItemType ? always : never;
                this.lciBid.Visibility = isCheckItemType ? always : never;
                this.emptySpaceItem1.Visibility = isCheckItemType ? always : never;

                //Hien thi vung theo thau
                this.lciSupplierCode1.Visibility = isCheckItemType ? never : always;
                this.lciSupplier1.Visibility = isCheckItemType ? never : always;
                this.lciBid1.Visibility = isCheckItemType ? never : always;
                this.emptySpaceItem1.Visibility = isCheckItemType ? never : always;
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
                        SetDefaultValueControlLeftDown();
                    }
                    else if (xtraTabControl1.SelectedTabPageIndex == 1) // vat tu
                    {
                        if (showMaterial)
                        {
                            FillDataToGridMaterialType();
                            showMaterial = false;
                        }
                        SetDefaultValueControlLeftDown();
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
                        SetDefaultValueControlLeftDown();
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

        #region Thread Load Data
        private void CreateThreadLoadData()
        {
            Inventec.Common.Logging.LogSystem.Info("begin thread");
            Thread bid = new Thread(GetBid);
            Thread supplier = new Thread(GetSupplier);
            Thread bidMaterialType = new Thread(GetBidMaterialType);
            Thread bidMedicineType = new Thread(GetBidMedicineType);
            Thread medicine = new Thread(GetMedicine);
            Thread material = new Thread(GetMaterial);

            try
            {
                bid.Start();
                supplier.Start();
                bidMaterialType.Start();
                bidMedicineType.Start();
                medicine.Start();
                material.Start();

                bid.Join();
                supplier.Join();
                bidMaterialType.Join();
                bidMedicineType.Join();
                medicine.Join();
                material.Join();
            }
            catch (Exception ex)
            {
                bid.Abort();
                supplier.Abort();
                bidMaterialType.Abort();
                bidMedicineType.Abort();
                medicine.Abort();
                material.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            Inventec.Common.Logging.LogSystem.Info("end thread");
        }

        private void GetMaterial()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMaterialFilter materialFilter = new HisMaterialFilter();
                ListMaterial = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetMedicine()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineFilter medicineFilter = new HisMedicineFilter();
                ListMedicine = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetBidMedicineType()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBidMedicineTypeViewFilter bidMedicineFilter = new HisBidMedicineTypeViewFilter();
                bidMedicineFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                ListBidMedicineType = new BackendAdapter(param).Get<List<V_HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/GetView", ApiConsumer.ApiConsumers.MosConsumer, bidMedicineFilter, param);

                if (ListBidMedicineType != null && ListBidMedicineType.Count > 0)
                {
                    long? dateNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);

                    ListBidMedicineType = ListBidMedicineType.Where(o => (o.VALID_TO_TIME == null || o.VALID_TO_TIME >= dateNow)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetBidMaterialType()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBidMaterialTypeViewFilter bidMaterialFilter = new HisBidMaterialTypeViewFilter();
                bidMaterialFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                ListBidMaterialType = new BackendAdapter(param).Get<List<V_HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/GetView", ApiConsumer.ApiConsumers.MosConsumer, bidMaterialFilter, param);

                if (ListBidMaterialType != null && ListBidMaterialType.Count > 0)
                {
                    long? dateNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);

                    ListBidMaterialType = ListBidMaterialType.Where(o => (o.VALID_TO_TIME == null || o.VALID_TO_TIME >= dateNow)).ToList();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetBid()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBidFilter bidFilter = new HisBidFilter();
                bidFilter.IS_ACTIVE = 1;
                ListBid = new BackendAdapter(param).Get<List<HIS_BID>>("api/HisBid/Get", ApiConsumer.ApiConsumers.MosConsumer, bidFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void GetSupplier()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSupplierFilter supplierFilter = new HisSupplierFilter();
                supplierFilter.IS_ACTIVE = 1;
                ListSupplier = new BackendAdapter(param).Get<List<HIS_SUPPLIER>>("api/HisSupplier/Get", ApiConsumer.ApiConsumers.MosConsumer, supplierFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
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

        public void FillAuto()
        {
            try
            {
                BtnAutoFillGrid_Click(null, null);
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
                        if (xtraTabControl1.SelectedTabPageIndex == 0)
                        {
                            var lst = bidMedicineTypes.Where(o => o.BID_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString())).ToList();
                            if (lst != null && lst.Count > 0)
                            {
                                LoadDataToCboSupplier(lst.Select(o => o.SUPPLIER_ID).ToList());
                            }
                        }
                        else if (xtraTabControl1.SelectedTabPageIndex == 1)
                        {
                            var lst = bidMaterialTypes.Where(o => o.BID_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString())).ToList();
                            if (lst != null && lst.Count > 0)
                            {
                                LoadDataToCboSupplier(lst.Select(o => o.SUPPLIER_ID).ToList());
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

        //Không dùng
        private void LoadBidPackageToGrid()
        {
            try
            {
                long? dateNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);

                ListMedicineTypeAdoProcess = new List<ADO.MedicineTypeADO>();
                medicineTypeProcessor.Reload(this.ucMedicineType, null);
                materialTypeProcessor.Reload(this.ucMaterialType, null);
                bloodTypeProcessor.Reload(this.ucBloodType, null);

                CommonParam param = new CommonParam();
                HisBidMedicineTypeViewFilter bidMedicineFilter = new HisBidMedicineTypeViewFilter();
                bidMedicineFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                bidMedicineFilter.BID_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString());
                var bidMedicineTypes = new BackendAdapter(param).Get<List<V_HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/GetView", ApiConsumer.ApiConsumers.MosConsumer, bidMedicineFilter, param);

                HisBidMaterialTypeViewFilter bidMaterialFilter = new HisBidMaterialTypeViewFilter();
                bidMaterialFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                bidMaterialFilter.BID_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString());
                var bidMaterialTypes = new BackendAdapter(param).Get<List<V_HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/GetView", ApiConsumer.ApiConsumers.MosConsumer, bidMaterialFilter, param);

                HisBidBloodTypeViewFilter bidBloodFilter = new HisBidBloodTypeViewFilter();
                bidBloodFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                bidBloodFilter.BID_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString());
                var bidBloodTypes = new BackendAdapter(param).Get<List<V_HIS_BID_BLOOD_TYPE>>("api/HisBidBloodType/GetView", ApiConsumer.ApiConsumers.MosConsumer, bidBloodFilter, param);

                if (bidMedicineTypes != null)
                {
                    if (bidMedicineTypes.Count > 0)
                    {
                        bidMedicineTypes = bidMedicineTypes.Where(o => (o.VALID_TO_TIME == null || o.VALID_TO_TIME >= dateNow)).ToList();

                        if (Base.GlobalConfig.HisMedicineTypes != null && Base.GlobalConfig.HisMedicineTypes.Count > 0)
                        {
                            List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO> medicineTypeDataSource = new List<HIS.UC.BidMedicineTypeGrid.MedicineTypeADO>();
                            foreach (var itemBid in bidMedicineTypes)
                            {
                                var medicineAdd = Base.GlobalConfig.HisMedicineTypes.FirstOrDefault(o => o.ID == itemBid.MEDICINE_TYPE_ID);
                                if (medicineAdd != null)
                                {
                                    HIS.UC.BidMedicineTypeGrid.MedicineTypeADO mediAdo = new UC.BidMedicineTypeGrid.MedicineTypeADO(medicineAdd);
                                    mediAdo.AllowAmount = itemBid.AMOUNT - (itemBid.IN_AMOUNT != null ? itemBid.IN_AMOUNT : 0);
                                    mediAdo.IMP_PRICE = itemBid.IMP_PRICE;
                                    mediAdo.SUPPLIER_ID = itemBid.SUPPLIER_ID;
                                    mediAdo.SUPPLIER_CODE = itemBid.SUPPLIER_CODE;
                                    mediAdo.SUPPLIER_NAME = itemBid.SUPPLIER_NAME;
                                    if (mediAdo.AllowAmount.HasValue && mediAdo.AllowAmount > 0)
                                    {
                                        medicineTypeDataSource.Add(mediAdo);
                                    }
                                }
                            }
                            addMedicine(bidMedicineTypes);
                            this.medicineTypeProcessor.Reload(this.ucMedicineType, medicineTypeDataSource);
                        }
                    }
                    else
                    {
                        addMedicine(bidMedicineTypes);
                    }
                }

                if (bidMaterialTypes != null)
                {
                    if (bidMaterialTypes.Count > 0)
                    {
                        bidMaterialTypes = bidMaterialTypes.Where(o => (o.VALID_TO_TIME == null || o.VALID_TO_TIME >= dateNow) && o.AMOUNT > 0).ToList();

                        if (Base.GlobalConfig.HisMaterialTypes != null && Base.GlobalConfig.HisMaterialTypes.Count > 0)
                        {
                            List<HIS.UC.BidMaterialTypeGrid.MaterialTypeADO> materialTypeDataSource = new List<HIS.UC.BidMaterialTypeGrid.MaterialTypeADO>();
                            foreach (var itemBid in bidMaterialTypes)
                            {
                                var materialAdd = Base.GlobalConfig.HisMaterialTypes.FirstOrDefault(o => o.ID == itemBid.MATERIAL_TYPE_ID);
                                if (materialAdd != null)
                                {
                                    HIS.UC.BidMaterialTypeGrid.MaterialTypeADO mateAdo = new HIS.UC.BidMaterialTypeGrid.MaterialTypeADO(materialAdd);
                                    mateAdo.AllowAmount = itemBid.AMOUNT - (itemBid.IN_AMOUNT != null ? itemBid.IN_AMOUNT : 0);
                                    mateAdo.IMP_PRICE = itemBid.IMP_PRICE;
                                    mateAdo.SUPPLIER_ID = itemBid.SUPPLIER_ID;
                                    mateAdo.SUPPLIER_CODE = itemBid.SUPPLIER_CODE;
                                    mateAdo.SUPPLIER_NAME = itemBid.SUPPLIER_NAME;
                                    if (mateAdo.AllowAmount.HasValue && mateAdo.AllowAmount > 0)
                                    {
                                        materialTypeDataSource.Add(mateAdo);
                                    }

                                }
                            }
                            addMaterial(bidMaterialTypes);
                            this.materialTypeProcessor.Reload(this.ucMaterialType, materialTypeDataSource);
                        }
                    }
                    else
                    {
                        addMaterial(bidMaterialTypes);
                    }
                }

                if (bidBloodTypes != null)
                {
                    if (bidBloodTypes.Count > 0)
                    {
                        if (Base.GlobalConfig.HisBloodTypes != null && Base.GlobalConfig.HisBloodTypes.Count > 0)
                        {
                            List<HIS.UC.BidBloodTypeGrid.BloodTypeADO> bloodTypeDataSource = new List<HIS.UC.BidBloodTypeGrid.BloodTypeADO>();
                            foreach (var itemBid in bidBloodTypes)
                            {
                                var bloodAdd = Base.GlobalConfig.HisBloodTypes.FirstOrDefault(o => o.ID == itemBid.BLOOD_TYPE_ID);
                                if (bloodAdd != null)
                                {
                                    HIS.UC.BidBloodTypeGrid.BloodTypeADO bloodAdo = new HIS.UC.BidBloodTypeGrid.BloodTypeADO(bloodAdd);
                                    bloodAdo.AllowAmount = itemBid.AMOUNT - (itemBid.IN_AMOUNT != null ? itemBid.IN_AMOUNT : 0);
                                    bloodAdo.IMP_PRICE = itemBid.IMP_PRICE;
                                    bloodAdo.SUPPLIER_ID = itemBid.SUPPLIER_ID;
                                    bloodAdo.SUPPLIER_CODE = itemBid.SUPPLIER_CODE;
                                    bloodAdo.SUPPLIER_NAME = itemBid.SUPPLIER_NAME;
                                    if (bloodAdo.AllowAmount.HasValue && bloodAdo.AllowAmount > 0)
                                    {
                                        bloodTypeDataSource.Add(bloodAdo);
                                    }
                                }
                            }
                            addBlood(bidBloodTypes);
                            this.bloodTypeProcessor.Reload(this.ucBloodType, bloodTypeDataSource);
                        }
                    }
                    else
                    {
                        addBlood(bidBloodTypes);
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
                    if (row.isAmount)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    if (row.IsNotSave)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Strikeout);
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
        public void InitComboReceiveMediStock()
        {
            try
            {
                var listReceiveMediStocks = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_SHOW_ANTICIPATE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboReceiveMediStock, listReceiveMediStocks, controlEditorADO);
                cboReceiveMediStock.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitMediStockCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(CboMediStock.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__MediStock);
                CboMediStock.Properties.Tag = gridCheck;
                CboMediStock.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = CboMediStock.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(CboMediStock.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__MediStock(object sender, EventArgs e)
        {
            try
            {
                mediStockSelecteds = new List<HIS_MEDI_STOCK>();
                foreach (HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        mediStockSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMediStock()
        {
            try
            {
                var datas = BackendDataWorker.Get<HIS_MEDI_STOCK>();
                if (datas != null)
                {
                    datas = datas.Where(o => o.IS_ACTIVE == 1).ToList();
                    if (datas != null)
                    {
                        var currMediStock = datas.FirstOrDefault(o => o.ID == mediStockId);
                        if (currMediStock != null)
                        {
                            Inventec.Common.Logging.LogAction.Debug("IS_BUSINESS_______________" + currMediStock);
                            isBusiness = currMediStock.IS_BUSINESS == 1 ? true : false;
                            datas = datas.Where(o => o.IS_BUSINESS == currMediStock.IS_BUSINESS && o.ID != currMediStock.ID).ToList();
                        }
                        long branchId = (long)HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId();
                        var hisRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.BRANCH_ID == branchId).ToList();
                        if (hisRoom != null && hisRoom.Count > 0)
                        {
                            datas = datas.Where(o => hisRoom.Select(s => s.ID).Contains(o.ROOM_ID)).ToList();
                        }

                        if (datas != null)
                        {
                            List<HIS_MEDI_STOCK> source = new List<HIS_MEDI_STOCK>();
                            if (currMediStock != null)
                            {
                                source.Add(currMediStock);
                            }
                            if (isBusiness == true)
                            {
                                source.AddRange(datas.Where(o => o.IS_BUSINESS == 1 && o.IS_ACTIVE == 1).OrderBy(o => o.MEDI_STOCK_NAME).ToList());
                            }
                            else
                            {
                                source.AddRange(datas.Where(o => o.IS_BUSINESS != 1 && o.IS_ACTIVE == 1).OrderBy(o => o.MEDI_STOCK_NAME).ToList());
                            }

                            CboMediStock.Properties.DataSource = source;
                            CboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                            CboMediStock.Properties.ValueMember = "ID";
                            DevExpress.XtraGrid.Columns.GridColumn col2 = CboMediStock.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                            col2.VisibleIndex = 1;
                            col2.Width = 200;
                            col2.Caption = "Kho";
                            CboMediStock.Properties.PopupFormWidth = 400;
                            //CboMediStock.Properties.View.OptionsView.ShowColumnHeaders = false;
                            CboMediStock.Properties.View.OptionsSelection.MultiSelect = true;
                            //CboMediStock.Properties.View.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
                            //CboMediStock.Properties.View.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
                            GridCheckMarksSelection gridCheckMark = CboMediStock.Properties.Tag as GridCheckMarksSelection;
                            if (gridCheckMark != null)
                            {
                                gridCheckMark.ClearSelection(CboMediStock.Properties.View);
                                gridCheckMark.SelectAll(new List<HIS_MEDI_STOCK>() { currMediStock });
                                CboMediStock.Focus();
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

        private void ReloadGridBid(List<ADO.BidADO> listData)
        {
            try
            {
                if (listData != null && listData.Count > 0)
                {
                    listData = listData.Where(o => o.AMOUNT.HasValue && o.AMOUNT.Value != 0).ToList();
                }

                gridControlBid.BeginUpdate();
                gridControlBid.DataSource = null;
                gridControlBid.DataSource = listData;
                gridControlBid.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBid_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ADO.BidADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnAutoFillGrid_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                gridViewProcess.PostEditor();
                BtnAutoFillGrid.Focus();
                if (!BtnAutoFillGrid.Enabled) return;
                Inventec.Common.Logging.LogSystem.Debug("isBusiness 1____________________________");
                if (mediStockSelecteds != null && mediStockSelecteds.Count() > 0)
                {
                    isBusiness = mediStockSelecteds.FirstOrDefault().IS_BUSINESS == 1 ? true : false;
                    Inventec.Common.Logging.LogSystem.Debug("isBusiness 2____________________________" + isBusiness);
                }

                if (CheckControlFilter())
                {
                    //lấy lượng thuốc, vật tư xuất ra khỏi kho trong khoảng thời gian.
                    // số lượng  = tổng số xuất ra - số tồn(nhập mà chưa xuất) - số lượng thầu chưa nhập.
                    long timeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(DtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                    long timeTo = Inventec.Common.TypeConvert.Parse.ToInt64(DtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");

                    //List<HIS_BID_MATERIAL_TYPE> LstBidMaterialType = new List<HIS_BID_MATERIAL_TYPE>();
                    //AutoMapper.Mapper.CreateMap<V_HIS_BID_MATERIAL_TYPE, HIS_BID_MATERIAL_TYPE>();
                    //LstBidMaterialType = AutoMapper.Mapper.Map<List<HIS_BID_MATERIAL_TYPE>>(this.ListBidMaterialType);

                    //List<HIS_BID_MEDICINE_TYPE> LstBidMedicineType = new List<HIS_BID_MEDICINE_TYPE>();
                    //AutoMapper.Mapper.CreateMap<V_HIS_BID_MEDICINE_TYPE, HIS_BID_MEDICINE_TYPE>();
                    //LstBidMedicineType = AutoMapper.Mapper.Map<List<HIS_BID_MEDICINE_TYPE>>(this.ListBidMedicineType);

                    var autoFill = new Base.AutoFillGridProcessor(timeFrom, timeTo, mediStockSelecteds.Select(s => s.ID).ToList(), (long)this.SpinCoefficient.Value);
                    autoFill.ListBid = this.ListBid;
                    autoFill.ListBidMaterialType = this.ListBidMaterialType;
                    autoFill.ListBidMedicineType = this.ListBidMedicineType;
                    autoFill.ListMaterial = this.ListMaterial;
                    autoFill.ListMedicine = this.ListMedicine;

                    ListMedicineTypeAdoProcess = new List<ADO.MedicineTypeADO>();
                    ListMedicineTypeAdoProcess = autoFill.GetListMedicineTypeAdo();

                    gridControlProcess.BeginUpdate();
                    gridControlProcess.DataSource = null;
                    gridControlProcess.DataSource = this.ListMedicineTypeAdoProcess;
                    gridControlProcess.EndUpdate();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckControlFilter()
        {
            bool result = true;
            try
            {
                result = result && (mediStockSelecteds != null && mediStockSelecteds.Count > 0);
                result = result && (DtTimeFrom.DateTime != DateTime.MinValue && DtTimeFrom.DateTime != DateTime.MaxValue);
                result = result && (DtTimeTo.DateTime != DateTime.MinValue && DtTimeTo.DateTime != DateTime.MaxValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void CboMediStock_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_MEDI_STOCK rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewProcess_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                var row = (ADO.MedicineTypeADO)gridViewProcess.GetFocusedRow();
                if (row != null)
                {
                    spinAmount.Value = row.AMOUNT ?? 0;
                    spinImpPrice.Value = (row.IMP_PRICE ?? 0);
                    txtSupplierCode.Text = row.SUPPLIER_CODE;
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    dxValidationProviderLeftPanel.RemoveControlError(spinImpPrice);
                    dxValidationProviderLeftPanel.RemoveControlError(spinAmount);

                    lstBidAdo = new List<ADO.BidADO>();

                    if (row.Type == Base.GlobalConfig.THUOC)
                    {
                        var bidMedicineTypes = ListBidMedicineType.Where(o => o.MEDICINE_TYPE_ID == row.ID).ToList();
                        if (bidMedicineTypes != null && bidMedicineTypes.Count > 0)
                        {
                            foreach (var item in bidMedicineTypes)
                            {
                                var bid = ListBid.FirstOrDefault(o => o.ID == item.BID_ID);
                                if (bid != null)
                                {

                                    var medicines = ListMedicine.Where(o => o.MEDICINE_TYPE_ID == row.ID && o.BID_ID == bid.ID).ToList();
                                    var ado = new ADO.BidADO();
                                    ado.ID = bid.ID;
                                    ado.BID_NAME = bid.BID_NAME;
                                    ado.BID_NUMBER = bid.BID_NUMBER;
                                    ado.BID_YEAR = bid.BID_YEAR;
                                    ado.AMOUNT = item.AMOUNT + (item.ADJUST_AMOUNT ?? 0) + (item.AMOUNT * (item.IMP_MORE_RATIO ?? 0));
                                    if (medicines != null && medicines.Count > 0)
                                    {
                                        ado.AMOUNT -= medicines.Sum(s => s.AMOUNT);
                                    }
                                    ado.SUPPLIER_ID = item.SUPPLIER_ID;
                                    lstBidAdo.Add(ado);
                                }
                            }

                            //load cob nha thau
                            LoadDataToCboSupplier(bidMedicineTypes.Select(o => o.SUPPLIER_ID).ToList());
                        }
                        else
                        {
                            LoadDataToCboSupplier(null);
                        }
                    }
                    else if (row.Type == Base.GlobalConfig.VATTU)
                    {
                        bidMaterialTypes = new List<V_HIS_BID_MATERIAL_TYPE>();
                        bidMaterialTypes = ListBidMaterialType.Where(o => o.MATERIAL_TYPE_ID == row.ID).ToList();
                        if (bidMaterialTypes != null && bidMaterialTypes.Count > 0)
                        {
                            foreach (var item in bidMaterialTypes)
                            {
                                var bid = ListBid.FirstOrDefault(o => o.ID == item.BID_ID);
                                if (bid != null)
                                {
                                    var materials = ListMaterial.Where(o => o.MATERIAL_TYPE_ID == row.ID && o.BID_ID == bid.ID).ToList();
                                    var ado = new ADO.BidADO();
                                    ado.ID = bid.ID;
                                    ado.BID_NAME = bid.BID_NAME;
                                    ado.BID_NUMBER = bid.BID_NUMBER;
                                    ado.BID_YEAR = bid.BID_YEAR;
                                    ado.AMOUNT = item.AMOUNT + (item.ADJUST_AMOUNT ?? 0) + (item.AMOUNT * (item.IMP_MORE_RATIO ?? 0));
                                    if (materials != null && materials.Count > 0)
                                    {
                                        ado.AMOUNT -= materials.Sum(s => s.AMOUNT);
                                    }
                                    ado.SUPPLIER_ID = item.SUPPLIER_ID;

                                    lstBidAdo.Add(ado);
                                }
                            }

                            //load cob nha thau
                            LoadDataToCboSupplier(bidMaterialTypes.Select(o => o.SUPPLIER_ID).ToList());

                        }
                        else
                        {
                            LoadDataToCboSupplier(null);
                        }

                    }

                    //Load combo goi thau
                    LoadDataToCboBid(lstBidAdo);

                    ReloadGridBid(lstBidAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProviderRightPanel_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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

        private void SpinCoefficient_Leave(object sender, EventArgs e)
        {
            try
            {
                positionHandleRight = -1;
                if (!dxValidationProviderRightPanel.Validate()) return;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SpinCoefficient_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dxValidationProviderRightPanel.RemoveControlError(spinAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkItemType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                {
                    return;
                }
                InitSupplierAndBid(chkItemType.Checked);
                SetDefaultValueControlLeft();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkItemType.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.AnticipateCreate").FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkItemType.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkItemType.Name;
                    csAddOrUpdate.VALUE = (chkItemType.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.AnticipateCreate";
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSupply_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSupply.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.AnticipateCreate").FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSupply.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSupply.Name;
                    csAddOrUpdate.VALUE = (chkSupply.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.AnticipateCreate";
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
