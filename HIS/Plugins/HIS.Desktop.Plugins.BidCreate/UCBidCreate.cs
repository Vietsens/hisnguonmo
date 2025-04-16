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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.BidCreate.Config;
using DevExpress.Utils;
using HIS.Desktop.Plugins.BidCreate.ADO;
using Inventec.Common.Logging;
using System.IO;
using SDA.EFMODEL.DataModels;
using Inventec.Core;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.Plugins.HisDosageForm;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.BidCreate.Validation;

namespace HIS.Desktop.Plugins.BidCreate
{
    public partial class UCBidCreate : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        private double idRow = -1;
        internal MOS.EFMODEL.DataModels.HIS_BID bidModel = null;
        internal MOS.EFMODEL.DataModels.HIS_BID bidPrint = null;
        internal List<ADO.MedicineTypeADO> ListMedicineTypeAdoProcess;
        internal List<ADO.MedicineTypeADO> listErrorImport;
        internal ADO.BloodTypeADO bloodType;
        internal ADO.MedicineTypeADO medicineType;
        internal ADO.MaterialTypeADO materialType;


        private V_HIS_MEDI_STOCK currentStock = null;
        private bool isInit = true;

        HIS.UC.MedicineType.MedicineTypeProcessor medicineTypeProcessor;
        HIS.UC.MaterialType.MaterialTypeTreeProcessor materialTypeProcessor;
        HIS.UC.BloodType.BloodTypeProcessor bloodTypeProcessor;
        UserControl ucMedicineType;

        UserControl ucMaterialType;
        UserControl ucBloodType;
        bool tabMaterial = true;
        bool tabBlood = true;

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
        List<HIS_DOSAGE_FORM> dataDosageForm;
        #endregion

        #region Construct
        public UCBidCreate(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO__Create = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK_CREATE);
                this.currentControlStateRDO__Update = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK_UPDATE);

                InitializeMedicineType();
                InitializeMaterialType();
                InitializeBloodType();
                LoadMedicine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCBidCreate_Load(object sender, EventArgs e)
        {
            try
            {
                // LoadConfigSystem
                HisConfigCFG.LoadConfig();
                TaskAll();
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //load cob nha thau
                //LoadDataToCboSupplier();

                this.InitControlState();

                //load cbo loại thầu
                //LoadDataToCboBidType();

                //LoadDataToCboNational();

                //LoadDataToCboManufacture();

                //LoadDataToCboMediUseForm();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                SetIsShowOnly();

                //Load du lieu
                //FillDataToGrid();

                //InitializeMedicineType();

                //Load Validation
                ValidControlLeft();
                ValidControlRight();
                timer1.Start();
                //print
                SetPrintTypeToMps();
                this.isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method

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

        private void LoadKeysFromlanguage()
        {
            try
            {
                #region LAYOUT
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__XTRA_TAB_MEDICINE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__XTRA_TAB_MATERIAL",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.xtraTabPageBlood.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__XTRA_TAB_BLOOD",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciSupplierCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_SUPPLIER",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_AMOUNT",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciImpPice.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_IMP_PICE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BTN_ADD",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.btnNew.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BTN_NEW",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.btnUpdate.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BTN_UPDATE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.btnDiscard.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BTN_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BTN_PRINT",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciBidName.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_BID_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciBidNumber.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_BID_NUMBER",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciBidNumOrder.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_BID_NUM_ORDER",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciImpVat.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_IMP_VAT_RATIO",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.txtBidName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_BID_NAME_NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciBidType.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_BID_TYPE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciBidGroupCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_BID_GROUP_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciBidPackageCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_BID_PACKAGE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciBidYear.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_BID_YEAR",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciFromTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_VALID_FROM_TIME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciToTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_VALID_TO_TIME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.LciExpiredDate.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_EXPIRED_DATE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);

                this.lciConcentra.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciConcentra.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_CONCENTRA_TOOLTIP",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciRegisterNumber.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_REGISTER_NUMBER",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.LciNational.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_NATIONAL",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.LciNational.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_NATIONAL_TOOLTIP",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciManufacture.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_MANUFACTURER",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciManufacture.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__LCI_MANUFACTURER_TOOLTIP",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);

                this.lciMaTT.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BID_CREATE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciTenTT.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BID_CREATE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciMaDT.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BID_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciTenBHYT.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.lciQCĐG.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BHYT_QCDG",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);

                this.gvProcess_MaTT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_BID_MATERIAL_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.gvProcess_TenTT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_BID_MATERIAL_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.gvProcess_MaDT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_JOIN_BID_MATERIAL_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.gvProcess_TenBHYT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_HEIN_SERVICE_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.gvProcess_QCDG.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_PACKING_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.btnFileDownload.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__BTN_FILE_DOWNLOAD",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);

                #endregion
                #region GRID PROCESS
                this.GvProcess_GcActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_ACTIVE_INGR_BHYT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcAmount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_PROCESS__GC_AMOUNT",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcConcentra.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_PROCESS__GC_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcImpPrice.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_PROCESS__GC_IMP_PRICE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcManufactureName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_PROCESS_GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_PROCESS_GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcNationalName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcPackingTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_PROCESS__GC_PACKING_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcServiceUnitName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_PROCESS__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcSTT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_STT",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcSupplierName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_PROCESS__GC_SUPPLIER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcTypeDisplay.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_PROCESS__GC_TYPE_DISPLAY",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.ButtonDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__TOOL_TIP_BUTTON_DELETE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.ButtonEdit.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__TOOL_TIP_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                this.GvProcess_GcBidNumOrder.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_BID_NUM_ORDER",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                xtraTabControl1.SelectedTabPageIndex = 0;
                medicineTypeProcessor.ResetKeyword(this.ucMedicineType);
                materialTypeProcessor.ResetKeyword(this.ucMaterialType);
                txtSupplierCode.Text = "";
                txtConcentra.Text = "";
                txtManufacture.Text = "";
                txtNationalMainText.Text = "";
                txtRegisterNumber.Text = "";
                cboSupplier.EditValue = null;
                txtActiveBhyt.Text = "";
                cboDosageForm.EditValue = null;
                txtQCĐG.Text = "";
                txtTenBHYT.Text = "";
                txtTenTT.Text = "";
                txtMaTT.Text = "";
                cboMediUserForm.EditValue = null;
                txtBidName.Text = "";
                txtBID.Text = "";
                cboBidForm.EditValue = null;
                txtBidNumber.Text = "";
                txtBidYear.Text = "";
                txtBidApThauCode.Text = "";
                dtFromTime.EditValue = null;
                dtToTime.EditValue = null;
                txtNationalMainText.Visible = false;
                cboNational.Visible = true;
                chkEditNational.CheckState = CheckState.Unchecked;
                cboBidType.EditValue = (long)1;
                ListMedicineTypeAdoProcess = new List<ADO.MedicineTypeADO>();
                this.ActionType = GlobalVariables.ActionAdd;
                gridControlProcess.DataSource = null;
                cboSupplier.Properties.Buttons[1].Visible = false;
                cboNational.EditValue = null;
                //cboNational.Properties.Buttons[1].Visible = false;
                cboManufacture.EditValue = null;
                spinMonthLifeSpan.EditValue = null;
                spinHourLifeSpan.EditValue = null;
                spinDayLifeSpan.EditValue = null;
                //cboManufacture.Properties.Buttons[1].Visible = false;
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

        private void SetIsShowOnly()
        {
            try
            {
                this.currentStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
                if (this.currentStock != null && this.currentStock.IS_BUSINESS == (short)1)
                {
                    chkIsOnlyShowByBusiness.Text = Resources.ResourceMessage.ChiHienThiThuocVatTuKD;
                    chkIsOnlyShowByBusiness.ToolTip = Resources.ResourceMessage.ChiHienThiThuocVatTuKinhDoanh;
                    chkIsOnlyShowByBusiness.Checked = true;
                }
                else
                {
                    chkIsOnlyShowByBusiness.Text = Resources.ResourceMessage.ChiHienThiThuocVatTuKhongKD;
                    chkIsOnlyShowByBusiness.ToolTip = Resources.ResourceMessage.ChiHienThiThuocVatTuKhongKinhDoanh;
                    chkIsOnlyShowByBusiness.Checked = true;
                }
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
                txtRegisterNumber.Text = "";
                txtManufacture.Text = "";
                txtNationalMainText.Text = "";
                txtConcentra.Text = "";
                txtTenTT.Text = "";
                txtMaTT.Text = "";
                cboSupplier.Text = "";
                cboSupplier.EditValue = null;
                cboSupplier.Properties.Buttons[1].Visible = false;
                cboDosageForm.Text = "";
                cboDosageForm.EditValue = null;
                cboNational.Text = "";
                cboNational.EditValue = null;
                //cboNational.Properties.Buttons[1].Visible = false;
                cboManufacture.Text = "";
                cboManufacture.EditValue = null;
                cboInformationBid.SelectedIndex = -1;
                //cboManufacture.Properties.Buttons[1].Visible = false;
                dxValidationProviderLeft.RemoveControlError(spinImpPrice);
                dxValidationProviderLeft.RemoveControlError(spinAmount);
                dxValidationProviderLeft.RemoveControlError(spinImpVat);
                dxValidationProviderLeft.RemoveControlError(txtBidNumOrder);
                dxValidationProviderLeft.RemoveControlError(cboSupplier);
                dxValidationProviderLeft.RemoveControlError(cboDosageForm);
                dxValidationProviderLeft.RemoveControlError(cboNational);
                dxValidationProviderLeft.RemoveControlError(cboManufacture);
                DtExpiredDate.EditValue = null;
                //trang thai nut
                EnableButton(this.ActionType);
                VisibleButton(this.ActionType);
                FocusTab();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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


                if (listHisMedicineType != null)
                {
                    foreach (var item in listHisMedicineType)
                    {
                        var medicineTypeInfo = listHisMedicineType.FirstOrDefault(x => x.ID == item.ID);
                        if (medicineTypeInfo != null && !string.IsNullOrEmpty(medicineTypeInfo.DOSAGE_FORM))
                        {
                            item.DOSAGE_FORM = medicineTypeInfo.DOSAGE_FORM;
                        }
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

        #region grid left
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
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);

                //Column MedicineTypeCode
                HIS.UC.MedicineType.MedicineTypeColumn GcMedicineTypeCode = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "MEDICINE_TYPE_CODE", 80, false);
                GcMedicineTypeCode.VisibleIndex = 0;
                ado.MedicineTypeColumns.Add(GcMedicineTypeCode);

                //Column MedicineTypeName
                HIS.UC.MedicineType.MedicineTypeColumn GcMedicineTypeName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "MEDICINE_TYPE_NAME", 150, false);
                GcMedicineTypeName.VisibleIndex = 1;
                ado.MedicineTypeColumns.Add(GcMedicineTypeName);

                //Column ServiceUnitName
                HIS.UC.MedicineType.MedicineTypeColumn GcServiceUnitName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "SERVICE_UNIT_NAME_STR", 80, false);
                GcServiceUnitName.VisibleIndex = 2;
                GcServiceUnitName.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.MedicineTypeColumns.Add(GcServiceUnitName);

                //HIS.UC.MedicineType.MedicineTypeColumn GcImpUnitName = new HIS.UC.MedicineType.MedicineTypeColumn
                //   (Inventec.Common.Resource.Get.Value
                //   ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_SERVICE_UNIT_NAME",
                //   Resources.ResourceLanguageManager.LanguageUCBidCreate,
                //   cultureLang), "IMP_UNIT_NAME_STR", 80, false);
                //GcImpUnitName.VisibleIndex = 3;
                //ado.MedicineTypeColumns.Add(GcImpUnitName);

                //Column ManufacturerName
                HIS.UC.MedicineType.MedicineTypeColumn GcManufacturerName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 3;
                ado.MedicineTypeColumns.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.MedicineType.MedicineTypeColumn GcNationalName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 4;
                ado.MedicineTypeColumns.Add(GcNationalName);

                //Column UseFromName
                HIS.UC.MedicineType.MedicineTypeColumn GcUseFromName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MEDICINE__GC_MEDICINE_USE_FROM_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "MEDICINE_USE_FORM_NAME", 80, false);
                GcUseFromName.VisibleIndex = 5;
                ado.MedicineTypeColumns.Add(GcUseFromName);

                //Column AvtiveIngrBhytName
                HIS.UC.MedicineType.MedicineTypeColumn GcAvtiveIngrBhytName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "ACTIVE_INGR_BHYT_NAME", 100, false);
                GcAvtiveIngrBhytName.VisibleIndex = 6;
                ado.MedicineTypeColumns.Add(GcAvtiveIngrBhytName);

                //column so dang ky 
                HIS.UC.MedicineType.MedicineTypeColumn colSodangky = new UC.MedicineType.MedicineTypeColumn("Số đăng ký", "REGISTER_NUMBER", 70, false);
                colSodangky.VisibleIndex = 7;
                ado.MedicineTypeColumns.Add(colSodangky);

                ado.MedicineType_CustomUnboundColumnData = MedicineType_CustomUnboundColumnData;



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

                //if (e.IsGetData && e.Column.UnboundType != DevExpress.XtraTreeList.Data.UnboundColumnType.Bound)
                //{
                //    var data = (ADO.MedicineTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                //    var listData = this.listHisMedicineType;
                //   // var data = this.medicineType;
                //    foreach (var item in listData)
                //        if (item != null)
                //    {
                //        if (e.Column.FieldName == "SERVICE_UNIT_NAME_STR")
                //        {
                //            if (item.IMP_UNIT_ID.HasValue)
                //                e.Value = item.IMP_UNIT_NAME;
                //            else
                //                e.Value = item.SERVICE_UNIT_NAME;
                //        }

                //    }
                //}
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
                this.medicineType = new ADO.MedicineTypeADO();
                if (data != null)
                {
                    //ValidBidPackage(false);
                    ADO.MedicineTypeADO medicine = new ADO.MedicineTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicine, data);

                    this.ActionType = GlobalVariables.ActionAdd;
                    VisibleButton(this.ActionType);
                    this.medicineType = medicine;
                    this.medicineType.Type = Base.GlobalConfig.THUOC;
                    DtExpiredDate.Enabled = true;
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
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);

                //Column MaterialTypeCode
                HIS.UC.MaterialType.MaterialTypeColumn GcMaterialTypeCode = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MATERIAL__GC_MATERIAL_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "MATERIAL_TYPE_CODE", 80, false);
                GcMaterialTypeCode.VisibleIndex = 0;
                ado.MaterialTypeColumns.Add(GcMaterialTypeCode);

                //Column MaterialTypeName
                HIS.UC.MaterialType.MaterialTypeColumn GcMaterialTypeName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MATERIAL__GC_MATERIAL_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "MATERIAL_TYPE_NAME", 150, false);
                GcMaterialTypeName.VisibleIndex = 1;
                ado.MaterialTypeColumns.Add(GcMaterialTypeName);

                //Column ServiceUnitName
                HIS.UC.MaterialType.MaterialTypeColumn GcServiceUnitName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "SERVICE_UNIT_NAME_STR", 80, false);
                GcServiceUnitName.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                GcServiceUnitName.VisibleIndex = 2;
                ado.MaterialTypeColumns.Add(GcServiceUnitName);

                //Column ManufacturerName
                HIS.UC.MaterialType.MaterialTypeColumn GcManufacturerName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 3;
                ado.MaterialTypeColumns.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.MaterialType.MaterialTypeColumn GcNationalName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 4;
                ado.MaterialTypeColumns.Add(GcNationalName);

                ado.MaterialType_CustomUnboundColumnData = MaterialType_CustomUnboundColumnData;

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

        private void chkGroupByMap_CheckChanged(CheckState checkState)
        {
            try
            {
                if (this.isInit) return;
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAdd = (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0) ? this.currentControlStateRDO__Create.Where(o => o.KEY == ControlStateConstant.CHECK_GROUP_BY_MAP && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_CREATE).FirstOrDefault() : null;

                HIS.Desktop.Library.CacheClient.ControlStateRDO csUpdate = (this.currentControlStateRDO__Update != null && this.currentControlStateRDO__Update.Count > 0) ? this.currentControlStateRDO__Update.Where(o => o.KEY == ControlStateConstant.CHECK_GROUP_BY_MAP && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_UPDATE).FirstOrDefault() : null;

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
                this.materialType = new ADO.MaterialTypeADO();
                if (data != null)
                {
                    //ValidBidPackage(true);
                    ADO.MaterialTypeADO material = new ADO.MaterialTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MaterialTypeADO>(material, data);

                    this.ActionType = GlobalVariables.ActionAdd;
                    VisibleButton(this.ActionType);
                    this.materialType = material;
                    this.materialType.Type = Base.GlobalConfig.VATTU;
                    DtExpiredDate.Enabled = true;
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
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang);

                //Column BloodTypeCode
                HIS.UC.BloodType.BloodTypeColumn GcBloodTypeCode = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_BLOOD__GC_BLOOD_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "BLOOD_TYPE_CODE", 80, false);
                GcBloodTypeCode.VisibleIndex = 0;
                ado.BloodTypeColumns.Add(GcBloodTypeCode);

                //Column BloodTypeName
                HIS.UC.BloodType.BloodTypeColumn GcBloodTypeName = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_BLOOD__GC_BLOOD_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "BLOOD_TYPE_NAME", 150, false);
                GcBloodTypeName.VisibleIndex = 1;
                ado.BloodTypeColumns.Add(GcBloodTypeName);

                //Column ServiceUnitName
                HIS.UC.BloodType.BloodTypeColumn GcServiceUnitName = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "SERVICE_UNIT_NAME", 80, false);
                GcServiceUnitName.VisibleIndex = 2;
                ado.BloodTypeColumns.Add(GcServiceUnitName);

                //column volume
                HIS.UC.BloodType.BloodTypeColumn GcVolume = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_BLOOD_GC_VOLUME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "VOLUME", 60, false);
                GcVolume.VisibleIndex = 3;
                ado.BloodTypeColumns.Add(GcVolume);

                //Column ManufacturerName
                HIS.UC.BloodType.BloodTypeColumn GcManufacturerName = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 4;
                ado.BloodTypeColumns.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.BloodType.BloodTypeColumn GcNationalName = new HIS.UC.BloodType.BloodTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCBidCreate,
                    cultureLang), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 5;
                ado.BloodTypeColumns.Add(GcNationalName);



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


        private void BloodType_Click(UC.BloodType.ADO.BloodTypeADO data)
        {
            try
            {
                this.bloodType = new ADO.BloodTypeADO();
                if (data != null)
                {
                    //ValidBidPackage(false);
                    ADO.BloodTypeADO blood = new ADO.BloodTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.BloodTypeADO>(blood, data);

                    this.ActionType = GlobalVariables.ActionAdd;
                    VisibleButton(this.ActionType);
                    this.bloodType = blood;
                    this.bloodType.Type = Base.GlobalConfig.MAU;
                    DtExpiredDate.Enabled = false;
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

        private void SetValueForAdd()
        {
            try
            {
                spinAmount.Value = 0;
                spinImpVat.Value = 0;
                spinImpMoreRatio.Value = 0;
                spinImpPrice.Value = 0;
                txtBidNumOrder.Text = "";
                this.DtExpiredDate.EditValue = null;
                spinAmount.Focus();
                spinAmount.SelectAll();
                txtMaTT.Text = null;
                txtTenTT.Text = null;
                if (this.materialType != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("this.materialType.NATIONAL_NAME: " + this.materialType.NATIONAL_NAME);
                }

                // thiet lap mac dinh
                if (xtraTabControl1.SelectedTabPageIndex == 0 && this.medicineType != null && !String.IsNullOrEmpty(this.medicineType.MEDICINE_TYPE_CODE))
                {
                    var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_NAME == this.medicineType.NATIONAL_NAME).ToList();
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
                    txtRegisterNumber.Text = this.medicineType.REGISTER_NUMBER;

                    // Set default value for medicine selection
                    txtTenBHYT.Text = (Int64.Parse(HisConfigCFG.IsSet__BHYT) == 1) ? this.medicineType.HEIN_SERVICE_BHYT_NAME : "";
                    txtQCĐG.Text = (Int64.Parse(HisConfigCFG.IsSet__BHYT) == 1) ? this.medicineType.PACKING_TYPE_NAME : "";
                    txtActiveBhyt.Text = (Int64.Parse(HisConfigCFG.IsSet__BHYT) == 1) ? this.medicineType.ACTIVE_INGR_BHYT_NAME : "";
                    cboMediUserForm.EditValue = (Int64.Parse(HisConfigCFG.IsSet__BHYT) == 1) ? this.medicineType.MEDICINE_USE_FORM_ID : null;
                    cboDosageForm.EditValue = (Int64.Parse(HisConfigCFG.IsSet__BHYT) == 1) ? this.medicineType.DOSAGE_FORM : "";

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
                    if (!string.IsNullOrEmpty(this.medicineType.DOSAGE_FORM))
                    {
                        //this.medicineType.DOSAGE_FORM = dataDosageForm.FirstOrDefault(o => o.ID == (long)cboDosageForm.EditValue).DOSAGE_FORM_NAME;
                        var selectedItem = dataDosageForm.FirstOrDefault(o => o.DOSAGE_FORM_NAME == this.medicineType.DOSAGE_FORM);
                        if (selectedItem != null)
                        {
                            cboDosageForm.EditValue = selectedItem.ID;
                        }                       
                    }
                    else
                    {
                        cboDosageForm.EditValue = null;
                        this.medicineType.DOSAGE_FORM = null;
                    }
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1 && this.materialType != null && !String.IsNullOrEmpty(this.materialType.MATERIAL_TYPE_CODE))
                {
                    var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_NAME.Equals(this.materialType.NATIONAL_NAME)).ToList();

                    txtRegisterNumber.Text = "";
                    txtRegisterNumber.Enabled = false;
                    if (national != null && national.Count() > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("national: " + national[0].NATIONAL_NAME);
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
                    cboInformationBid.SelectedIndex = 0;

                    MOS.Filter.HisBidMaterialTypeViewFilter bidMatyFilter = new MOS.Filter.HisBidMaterialTypeViewFilter();
                    bidMatyFilter.MATERIAL_TYPE_ID = materialType.ID;
                    var bidMaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_BID_MATERIAL_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_BID_MATERIAL_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bidMatyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
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
                }
                else
                {
                    spinImpMoreRatio.Enabled = false;
                    spinImpMoreRatio.EditValue = null;
                    txtManufacture.Enabled = false;
                    txtManufacture.Text = "";
                    txtNationalMainText.Text = "";
                    txtNationalMainText.Enabled = false;
                    cboNational.Enabled = false;
                    cboNational.EditValue = null;
                    cboManufacture.Enabled = false;
                    cboManufacture.EditValue = null;
                    spinDayLifeSpan.EditValue = null;
                    spinMonthLifeSpan.EditValue = null;
                    spinHourLifeSpan.EditValue = null;
                    txtTenTT.Text = "";
                    txtMaTT.Text = "";
                }

                dxValidationProviderLeft.RemoveControlError(spinImpPrice);
                dxValidationProviderLeft.RemoveControlError(spinAmount);
                dxValidationProviderLeft.RemoveControlError(spinImpVat);
                dxValidationProviderLeft.RemoveControlError(txtBidNumOrder);
                dxValidationProviderLeft.RemoveControlError(cboSupplier);
                dxValidationProviderLeft.RemoveControlError(cboNational);
                dxValidationProviderLeft.RemoveControlError(cboManufacture);
                dxValidationProviderLeft.RemoveControlError(txtBidPackageCode);
                dxValidationProviderLeft.RemoveControlError(txtBidGroupCode);
                dxValidationProviderLeft.RemoveControlError(txtConcentra);
                dxValidationProviderLeft.RemoveControlError(txtRegisterNumber);
                dxValidationProviderLeft.RemoveControlError(txtTenTT);
                dxValidationProviderLeft.RemoveControlError(txtTenBHYT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
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
                cboSupplier.Properties.PopupFormSize = new Size(400, 250);

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

        private void LoadDataToCboMediUseForm()
        {
            try
            {
                cboMediUserForm.Properties.DataSource = lstMediUseForm;
                cboMediUserForm.Properties.DisplayMember = "MEDICINE_USE_FORM_NAME";
                cboMediUserForm.Properties.ValueMember = "ID";
                cboMediUserForm.Properties.TextEditStyle = TextEditStyles.Standard;
                cboMediUserForm.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboMediUserForm.Properties.ImmediatePopup = true;
                cboMediUserForm.ForceInitialize();
                cboMediUserForm.Properties.View.Columns.Clear();
                cboMediUserForm.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = cboMediUserForm.Properties.View.Columns.AddField("MEDICINE_USE_FORM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 70;

                GridColumn aColumnName = cboMediUserForm.Properties.View.Columns.AddField("MEDICINE_USE_FORM_NAME");
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

        private void EnableButton(int action)
        {
            try
            {
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                btnUpdate.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
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
                    //emptySpaceItemForEdit.Size = new Size(root.Width - 110, 26);
                    lciBtnAdd.Size = new Size(110, 26);
                }
                else
                {
                    //emptySpaceItemForEdit.Size = new Size(root.Width - 220, 26);
                    lciBtnUpdate.Size = new Size(110, 26);
                    lciBtnDiscard.Size = new Size(110, 26);
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
                txtConcentra.Text = "";
                txtManufacture.Text = "";
                txtNationalMainText.Text = "";
                cboNational.EditValue = null;
                cboManufacture.EditValue = null;
                txtRegisterNumber.Text = "";
                txtMaTT.Text = "";
                txtTenTT.Text = "";
                txtMaDT.Text = "";
                txtTenBHYT.Text = "";
                txtQCĐG.Text = "";
                spinMonthLifeSpan.EditValue = null;
                spinDayLifeSpan.EditValue = null;
                spinHourLifeSpan.EditValue = null;
                txtActiveBhyt.Text = "";
                cboDosageForm.EditValue = null;
                cboMediUserForm.EditValue = null;

                if (chkClearToAdd.Checked)
                {
                    txtSupplierCode.Text = "";
                    cboSupplier.EditValue = null;
                    txtBidPackageCode.Text = "";
                    txtBidGroupCode.Text = "";
                }
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
                txtBidPackageCode.Properties.MaxLength = 4;
                txtMaTT.Enabled = true;
                txtTenTT.Enabled = true;
                txtMaDT.Enabled = true;
                txtTenBHYT.Enabled = true;
                txtQCĐG.Enabled = true;
                txtActiveBhyt.Enabled = true;
                cboMediUserForm.Enabled = true;
                
                if (xtraTabControl1.SelectedTabPageIndex == 0) // thuoc
                {
                    medicineTypeProcessor.FocusKeyword(ucMedicineType);
                    EnableLeftControl(true);
                    if (spinImpMoreRatio.EditValue == null)
                        spinImpMoreRatio.EditValue = 0;
                    txtMaTT.Text = "";
                    txtMaTT.Enabled = false;
                    txtTenTT.Text = "";
                    txtTenTT.Enabled = false;
                    txtMaDT.Text = "";
                    txtMaDT.Enabled = false;
                    cboDosageForm.Enabled = true;
                    cboInformationBid.SelectedIndex = -1;
                    ValidDosageForm();
                    //layoutControlItem21.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1) // vat tu
                {
                    EnableLeftControl(true);
                    dxValidationProviderLeft.SetValidationRule(cboDosageForm, null);
                    layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem19.Size = lciMaDT.Size;
                    cboInformationBid.SelectedIndex = 0;
                    if (spinImpMoreRatio.EditValue == null)
                        spinImpMoreRatio.EditValue = 0;
                    txtRegisterNumber.Enabled = false;
                    txtBidPackageCode.Properties.MaxLength = 4;
                    txtTenBHYT.Text = "";
                    txtTenBHYT.Enabled = false;
                    txtQCĐG.Text = "";
                    txtQCĐG.Enabled = false;
                    txtActiveBhyt.Enabled = false;
                    cboMediUserForm.Enabled = false;
                    cboDosageForm.Enabled = false;
                    txtActiveBhyt.Text = "";
                    cboMediUserForm.EditValue = null;
                    cboDosageForm.EditValue = null;
                    cboDosageForm.Enabled = false;
                    //layoutControlItem21.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    if (tabMaterial)
                    {
                        WaitingManager.Show();
                        await taskMaterialType;
                        this.FillDataMaterial();
                        WaitingManager.Hide();
                        tabMaterial = false;
                    }
                    materialTypeProcessor.FocusKeyword(ucMaterialType);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2) // Mau
                {
                    EnableLeftControl(false);
                    dxValidationProviderLeft.SetValidationRule(cboDosageForm, null);
                    cboInformationBid.SelectedIndex = -1;
                    layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    spinImpMoreRatio.EditValue = null;
                    txtMaTT.Text = "";
                    txtMaTT.Enabled = false;
                    txtTenTT.Text = "";
                    txtTenTT.Enabled = false;
                    txtMaDT.Text = "";
                    txtMaDT.Enabled = false;
                    txtTenBHYT.Text = "";
                    txtTenBHYT.Enabled = false;
                    txtQCĐG.Text = "";
                    txtQCĐG.Enabled = false;
                    txtActiveBhyt.Enabled = false;
                    cboMediUserForm.Enabled = false;
                    cboDosageForm.Enabled = false;
                    txtActiveBhyt.Text = "";
                    cboMediUserForm.EditValue = null;
                    cboDosageForm.EditValue = null;
                    cboDosageForm.Enabled = false;
                    //layoutControlItem21.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    if (tabBlood)
                    {
                        WaitingManager.Show();
                        await taskBloodType;
                        this.bloodTypeProcessor.Reload(this.ucBloodType, listHisBloodType);
                        WaitingManager.Hide();
                        tabBlood = false;
                    }
                    bloodTypeProcessor.FocusKeyword(ucBloodType);
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
                    else
                    {
                        listHisMaterialType = listHisMaterialType != null ? listHisMaterialType.Where(o => o.IS_BUSINESS != (short)1).ToList() : null;
                    }
                }

                this.materialTypeProcessor.Reload(this.ucMaterialType, listHisMaterialType);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                        else if (e.Column.FieldName == "SERVICE_UNIT_NAME_STR")
                        {
                            e.Value = data.IMP_UNIT_ID.HasValue ? data.IMP_UNIT_NAME : data.SERVICE_UNIT_NAME;
                        }


                        else if (e.Column.FieldName == "GV_HEIN_SERVICE_BHYT_NAME")
                        {
                            if (data.Type == Base.GlobalConfig.THUOC)
                            {
                                e.Value = data.HEIN_SERVICE_BHYT_NAME;
                            }
                            else
                            {
                                e.Value = "";
                            }

                        }
                        else if (e.Column.FieldName == "GV_PACKING_TYPE_NAME")
                        {
                            if (data.Type == Base.GlobalConfig.THUOC)
                            {
                                e.Value = data.PACKING_TYPE_NAME;
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "GV_BID_MATERIAL_TYPE_CODE")
                        {
                            if (data.Type == Base.GlobalConfig.VATTU)
                            {
                                e.Value = data.BID_MATERIAL_TYPE_CODE;
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "GV_BID_MATERIAL_TYPE_NAME")
                        {
                            if (data.Type == Base.GlobalConfig.VATTU)
                            {
                                e.Value = data.BID_MATERIAL_TYPE_NAME;
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "GV_JOIN_BID_MATERIAL_TYPE_CODE")
                        {
                            if (data.Type == Base.GlobalConfig.VATTU)
                            {
                                e.Value = data.JOIN_BID_MATERIAL_TYPE_CODE;
                            }
                            else
                            {
                                e.Value = "";
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

        private void gridViewProcess_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var data = (ADO.MedicineTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (e.Column.FieldName == "ImpVatRatio")
                    {
                        data.IMP_VAT_RATIO = data.ImpVatRatio / 100;
                    }
                    gridControlProcess.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        #endregion

        #region Public method
        public void Update()
        {
            try
            {
                if (btnUpdate.Enabled) btnUpdate_Click(null, null);
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

        public void BidNameFocus()
        {
            try
            {
                txtBidName.Focus();
                txtBidName.SelectAll();
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

        private void txtNationalCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    bool showCbo = true;
                //    if (!String.IsNullOrEmpty(txtNationalCode.Text.Trim()))
                //    {
                //        string code = txtNationalCode.Text.Trim().ToLower();
                //        var listData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_CODE.ToLower().Contains(code)).ToList();
                //        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.NATIONAL_CODE.ToLower() == code).ToList() : listData) : null;
                //        if (result != null && result.Count > 0)
                //        {
                //            showCbo = false;
                //            txtNationalCode.Text = result.First().NATIONAL_CODE;
                //            cboNational.EditValue = result.First().ID;
                //            //SendKeys.Send("{TAB}");
                //            //SendKeys.Send("{TAB}");
                //            txtConcentra.Focus();
                //            txtConcentra.SelectAll();
                //        }
                //    }
                //    if (showCbo)
                //    {
                //        cboNational.Focus();
                //        cboNational.ShowPopup();
                //    }
                //}

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
                            txtConcentra.Focus();
                            txtConcentra.SelectAll();
                        }
                    }
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
                                e.Handled = true;
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

        private void cboNational_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(cboNational.Text))
                    {
                        var key = cboNational.Text.ToLower();
                        var listData = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_CODE.ToLower().Contains(key) || o.NATIONAL_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            cboNational.EditValue = listData[0].ID;
                            txtNationalMainText.Text = listData[0].NATIONAL_NAME;
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
                    else
                    {
                        chkEditNational.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void txtNationalMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                    else
                    {
                        chkEditNational.Focus();
                        chkEditNational.SelectAll();
                    }
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

        private void txtMaTT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                LogSystem.Warn(ex);
            }
        }

        private void txtTenTT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                LogSystem.Warn(ex);
            }
        }

        private void txtMaDT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboNational.Focus();
                    cboNational.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtTenBHYT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtQCĐG.Focus();
                    txtQCĐG.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtQCĐG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                LogSystem.Warn(ex);
            }
        }

        private void btnFileDownload_Click_1(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_BID.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_BID";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog1.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

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

        private void txtActiveBhyt_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMediUserForm.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboMediUserForm_Closed(object sender, ClosedEventArgs e)
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
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
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

        private void UCBidCreate_Leave(object sender, EventArgs e)
        {
            try
            {
                // cleartoAdd
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddClear = (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0) ? this.currentControlStateRDO__Create.Where(o => o.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_CREATE).FirstOrDefault() : null;

                HIS.Desktop.Library.CacheClient.ControlStateRDO csUpdateClear = (this.currentControlStateRDO__Update != null && this.currentControlStateRDO__Update.Count > 0) ? this.currentControlStateRDO__Update.Where(o => o.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_UPDATE).FirstOrDefault() : null;
                if (csAddClear != null)
                {
                    csAddClear.VALUE = (chkClearToAdd.Checked ? "1" : "");
                }
                else if (csUpdateClear != null)
                {
                    csUpdateClear.VALUE = (chkClearToAdd.Checked ? "1" : "");
                }
                else
                {
                    csAddClear = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddClear.KEY = ControlStateConstant.CHECK_CLEAR_TO_ADD;
                    csAddClear.VALUE = (chkClearToAdd.Checked ? "1" : "");
                    csAddClear.MODULE_LINK = ControlStateConstant.MODULE_LINK_CREATE;
                    if (this.currentControlStateRDO__Create == null)
                        this.currentControlStateRDO__Create = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Create.Add(csAddClear);

                    csUpdateClear = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csUpdateClear.KEY = ControlStateConstant.CHECK_CLEAR_TO_ADD;
                    csUpdateClear.VALUE = (chkClearToAdd.Checked ? "1" : "");
                    csUpdateClear.MODULE_LINK = ControlStateConstant.MODULE_LINK_UPDATE;
                    if (this.currentControlStateRDO__Update == null)
                        this.currentControlStateRDO__Update = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Update.Add(csUpdateClear);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO__Create);
                this.controlStateWorker.SetData(this.currentControlStateRDO__Update);
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

        private void gridViewProcess_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                ADO.MedicineTypeADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (ADO.MedicineTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "ADJUST_AMOUNT")
                    {
                        e.RepositoryItem = data.Type != Base.GlobalConfig.MAU ? ((data.Type == Base.GlobalConfig.VATTU && BidMaterialType != null && BidMaterialType.Count > 0 && BidMaterialType.FirstOrDefault(o => o.MATERIAL_TYPE_ID == data.ID || o.MATERIAL_TYPE_MAP_ID == data.ID) != null) || (data.Type == Base.GlobalConfig.THUOC && BidMedicineType != null && BidMedicineType.Count > 0 && BidMedicineType.FirstOrDefault(o => o.MEDICINE_TYPE_ID == data.ID) != null)) ? repAdjustAmount : repAdjustAmountDis : SpinEditAdjustAmountDisable;
                    }

                }
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

        private void repAdjustAmount_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                var row = (MedicineTypeADO)gridViewProcess.GetFocusedRow();
                if (row.Type == Base.GlobalConfig.THUOC && BidMedicineType != null && BidMedicineType.Count > 0 && BidMedicineType.FirstOrDefault(o => o.MEDICINE_TYPE_ID == row.ID) != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(BidMedicineType.FirstOrDefault(o => o.MEDICINE_TYPE_ID == row.ID));
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)((data) => {
                        row.ADJUST_AMOUNT = data as decimal?;
                        gridControlProcess.RefreshDataSource();
                    }));

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.BidRegulation", this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId, listArgs);
                }
                else if (row.Type == Base.GlobalConfig.VATTU && BidMaterialType != null && BidMaterialType.Count > 0 && BidMaterialType.FirstOrDefault(o => o.MATERIAL_TYPE_ID == row.ID || o.MATERIAL_TYPE_MAP_ID == row.ID) != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(BidMaterialType.FirstOrDefault(o => o.MATERIAL_TYPE_ID == row.ID || o.MATERIAL_TYPE_MAP_ID == row.ID));
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)((data) => {
                        row.ADJUST_AMOUNT = data as decimal?;
                        gridControlProcess.RefreshDataSource();
                    }));
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.BidRegulation", this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediUserForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMediUserForm.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDosageForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDosageForm.Properties.Buttons[2].Visible = true;
                    cboDosageForm.EditValue = null;
                }
                if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    cboDosageForm.Properties.Buttons[1].Visible = true;
                    cboDosageForm.EditValue = null;
                    var module = new Inventec.Desktop.Common.Modules.Module();
                    DelegateSelectData delegateSelectData = new DelegateSelectData((selectedItem) =>
                    {
                        if (selectedItem is HIS_DOSAGE_FORM dosageForm)
                        {
                            cboDosageForm.EditValue = dosageForm.ID;
                        }
                    });
                    frmHisDosageForm form = new frmHisDosageForm(module, delegateSelectData);
                    form.ShowDialog();
                    //LoadDosageForm();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            
        }

        private void cboDosageForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboDosageForm.Text))
                    {
                        string key = cboDosageForm.Text.ToLower();
                        var listData = dataDosageForm.Where(o => o.DOSAGE_FORM_CODE.ToLower().Contains(key) || o.DOSAGE_FORM_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboDosageForm.EditValue = listData.First().ID;
                            txtRegisterNumber.Focus();
                            txtRegisterNumber.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboDosageForm.Focus();
                        cboDosageForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDosageForm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDosageForm.EditValue != null)
                    {
                        var dosageform = dataDosageForm.SingleOrDefault(o => o.DOSAGE_FORM_NAME == (cboDosageForm.EditValue ?? "").ToString());
                        if (dosageform != null)
                        {
                            cboDosageForm.Properties.Buttons[1].Visible = true;
                            txtRegisterNumber.Focus();
                            txtRegisterNumber.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDosageForm_EditValueChanged(object sender, EventArgs e)
        {
           try
            {
                if (cboDosageForm.EditValue != null)
                {
                    cboDosageForm.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboDosageForm.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }        
        private void LoadDataToCboDosageForm()
        {
            try
            {
                cboDosageForm.Properties.DataSource = dataDosageForm;
                cboDosageForm.Properties.DisplayMember = "DOSAGE_FORM_NAME";
                cboDosageForm.Properties.ValueMember = "ID";
                cboDosageForm.EditValue = null;
                cboDosageForm.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboDosageForm.Properties.View.Columns.AddField("DOSAGE_FORM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 0;
                aColumnCode.Width = 150;
                GridColumn aColumnName = cboDosageForm.Properties.View.Columns.AddField("DOSAGE_FORM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 1;
                aColumnName.Width = 450;

                cboDosageForm.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cboDosageForm.Properties.PopupFilterMode = PopupFilterMode.Contains;
                cboDosageForm.Properties.TextEditStyle = TextEditStyles.Standard;      

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);           
            }
        }

    }
}
