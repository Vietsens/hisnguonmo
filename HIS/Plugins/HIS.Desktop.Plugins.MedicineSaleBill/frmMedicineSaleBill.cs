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
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.MedicineSaleBill.ADO;
using HIS.Desktop.Plugins.MedicineSaleBill.Validation;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.DocumentViewer;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.MedicineSaleBill
{
    public partial class frmMedicineSaleBill : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module module;
        private const string HFS_KEY__PAY_FORM_CODE = "HFS_KEY__PAY_FORM_CODE";
        long roomId;
        long roomTypeId;

        int positionHandle = -1;
        V_HIS_MEDI_STOCK mediStock = null;
        List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_CASHIER_ROOM> cashierRoom = new List<V_HIS_CASHIER_ROOM>();

        List<HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO> listMediMateAdo = new List<HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO>();
        List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine;
        List<V_HIS_EXP_MEST_MATERIAL> listExpMestMaterial;
        long? patientIdForEdit = null;
        List<long> expMestIdForEdits = null;
        List<V_HIS_EXP_MEST> ExpMests;
        List<HIS_PATIENT> Patients;
        MOS.EFMODEL.DataModels.HIS_PATIENT patient = null;
        V_HIS_TRANSACTION transactionBillResult;
        DelegateSelectData delegateSelectData;
        V_HIS_TREATMENT_FEE currentTreatment;
        string InvoiceTypeCreate;
        const string invoiceTypeCreate__CreateInvoiceVnpt = "1";
        const string invoiceTypeCreate__CreateInvoiceHIS = "2";
        List<HIS_CONFIG> listConfig = new List<HIS_CONFIG>();
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isNotLoadWhileChangeControlStateInFirst;
        V_HIS_TRANSACTION originalTransaction;
        /// <summary>Viec 3082: hanh dong tu dong do man Xuat ban truyen vao qua args (chua Config.AUTO_ACTION__SAVE_SIGN_PRINT)</summary>
        List<string> autoActions = null;
        /// <summary>Viec 3082: chuoi tu dong chi chay 1 lan sau khi form Shown</summary>
        bool isAutoSaveSignPrintStarted = false;
        List<ReplaceTransactionADO> replaceTransactionADOs = null;
        private string selectedRadio = null;
        /// <summary>Dang set thong tin nguoi mua tu du lieu benh nhan: chan handler cua combo Don vi ghi de Dia chi</summary>
        private bool isSettingBuyerInfo = false;
        /// <summary>Danh muc don vi (HIS_WORK_PLACE) da nap cho 2 combo Don vi</summary>
        private List<HIS_WORK_PLACE> workPlaces = null;
        private decimal totalPrice = 0;
        private decimal transferAmount = 0;

        private const string CFG__AUTO_LOAD_ORG_AND_TAX_BY_PATIENT = "HIS.Desktop.Plugins.TransactionBill.AutoLoadOrgAndTaxCodeByPatient";

        public frmMedicineSaleBill()
        {
            InitializeComponent();
        }



        public frmMedicineSaleBill(Inventec.Desktop.Common.Modules.Module module,
            List<long> expMestIds, DelegateSelectData _delegateSelectData, V_HIS_TRANSACTION _OriginalTransaction, List<string> _autoActions = null)
            : base(module)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            try
            {
                this.delegateSelectData = _delegateSelectData;
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.module = module;
                if (this.module != null)
                {
                    this.roomId = module.RoomId;
                    this.roomTypeId = module.RoomTypeId;
                }
                expMestIdForEdits = expMestIds;
                originalTransaction = _OriginalTransaction;

                // Viec 3082: man Xuat ban (Luu in + tick "In") yeu cau form tu chay Luu ky + duyet/thuc xuat + in roi tu dong
                this.autoActions = _autoActions;
                if (IsAutoSaveSignPrint)
                {
                    this.Shown += new EventHandler(frmMedicineSaleBill_Shown);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmMedicineSaleBill_Load(object sender, EventArgs e)
        {
            try
            {
                LoadControlByConfigShortCut();
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                InitControlState();
                LoadMediStockByRoomId();

                if (this.mediStock != null)
                {
                    dtTransactionTime.EditValue = DateTime.Now;
                    checkOverTime.Checked = GlobalVariables.MedicineSaleBill__IsOverTime;
                    LoadDataToComboCashierRoom();
                    SetDafaultCashierRoom();
                    LoadDataToComboAccountBook();
                    LoadDataToComboPayForm();
                    LoadExpMest();
                    InitResultSdoByExpMest();
                    ddBtnPrint.Enabled = false;
                    ValidateForm();
                    SetDefaultAccountBook();
                    SetDefaultPayForm();
                    LoadTreatmentFee();
                    SetBuyerInfo();
                    SetDefaultCreateQR();
                    //SetDefaultLayout();
                    InitComboIdentityType();
                    LoadComboBuyerOrganization();

                    rdoCaNhan.Checked = true;
                    rdoCaNhan_CheckedChanged(null, null);
                    WaitingManager.Show();
                }

                if (expMestIdForEdits != null && expMestIdForEdits.Count > 0)
                {
                    btnSave.Enabled = true;
                    btnSavePrint.Enabled = true;
                    BtnSaveSign.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                    btnSavePrint.Enabled = false;
                    BtnSaveSign.Enabled = false;
                }

                GeneratePopupMenu();
                InvoiceTypeCreate = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.ElectronicBill.Type");
                if (String.IsNullOrEmpty(InvoiceTypeCreate) || (InvoiceTypeCreate != invoiceTypeCreate__CreateInvoiceVnpt && InvoiceTypeCreate != invoiceTypeCreate__CreateInvoiceHIS))
                {
                    lcibtnSaveAndSign.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                // Checkbox "In" (viec 3082): chi hien khi config bat va nut Luu ky dang hien
                if (!Config.IsSaveSignPrintAutoExport
                    || lcibtnSaveAndSign.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    lciAutoExportPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidControlTransferAmount(bool IsRequiredField)
        {
            try
            {
                SpinTranferAmountValidationRule PINRule = new SpinTranferAmountValidationRule();
                PINRule.spinTranferAmount = spinTransAmountNew;
                PINRule.isRequiredPin = IsRequiredField;
                dxValidationProviderEditorInfo.SetValidationRule(spinTransAmountNew, PINRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlSwipeAmount(bool IsRequiredField)
        {
            try
            {
                SpinTranferAmountValidationRule PINRule = new SpinTranferAmountValidationRule();
                PINRule.spinTranferAmount = spinSwipeAmountNew;
                PINRule.isRequiredPin = IsRequiredField;
                dxValidationProviderEditorInfo.SetValidationRule(spinSwipeAmountNew, PINRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadComboBuyerOrganization()
        {
            try
            {
                var workPlaceFilter = new HisWorkPlaceFilter();
                var workPlaces = new BackendAdapter(new CommonParam()).Get<List<HIS_WORK_PLACE>>("api/HisWorkPlace/Get", ApiConsumers.MosConsumer, workPlaceFilter, null);
                this.workPlaces = workPlaces;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "Tên đơn vị", 200, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_CODE", "Mã đơn vị", 100, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboBuyerOrganization, workPlaces, controlEditorADO);
                ControlEditorLoader.Load(cboBuyerOrganization1, workPlaces, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboIdentityType()
        {
            try
            {
                var identificationTypes = new List<dynamic>
                {
                    new { ID = 1, NAME = "CMND" },
                    new { ID = 2, NAME = "CCCD" },
                    new { ID = 3, NAME = "PASSPORT" }
                };

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "Loại giấy tờ", 100, 1));

                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 110);
                ControlEditorLoader.Load(cboIdentityType, identificationTypes, controlEditorADO);
                cboIdentityType.Refresh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void SetDefaultLayout()
        //{
        //    layoutControl1.BeginUpdate();

        //    layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; // Họ tên
        //    layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; // Mã số thuế
        //    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; // Số tài khoản
        //    layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; // SĐT
        //    layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; // Đơn vị (Textbox)
        //    layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; // Địa chỉ
        //    layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; // Email

        //    layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never; // Đơn vị (Combobox)
        //    layoutControlItem28.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never; // TT định danh (Textbox)
        //    layoutControlItem29.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never; // TT định danh (Combobox)
        //    layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never; // Khác
        //    layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never; // ĐC BHYT
        //    layoutControlItem27.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never; // Combobox ẩn 

        //    layoutControlGroup2.BestFit();
        //    layoutControl1.EndUpdate();

        //    if (this.ExpMests != null && this.ExpMests.Count > 0)
        //    {
        //        var expMest = ExpMests.FirstOrDefault();
        //        long patientId = expMest.TDL_PATIENT_ID ?? 0;
        //        var patientFilter = new HisPatientFilter { ID = patientId };
        //        var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, null);
        //        var patient = patients?.FirstOrDefault();
        //        long workPlaceId = patient.WORK_PLACE_ID.Value;
        //        var workPlaceFilter = new HisWorkPlaceFilter { ID = workPlaceId };
        //        var workPlaces = new BackendAdapter(new CommonParam()).Get<List<HIS_WORK_PLACE>>("api/HisWorkPlace/Get", ApiConsumers.MosConsumer, workPlaceFilter, null);
        //        var workPlace = workPlaces?.FirstOrDefault();
        //        if (expMest != null)
        //        {


        //            txtName.Text = expMest.TDL_PATIENT_NAME;
        //            txtBuyerOgranization.Text = workPlace.WORK_PLACE_NAME;
        //            //txtBuyerTaxCode.Text = expMest.TDL_PATIENT_TAX_CODE;
        //            txtBuyerTaxCode.Text = patient?.TAX_CODE;
        //            //txtBuyerPhone.Text = expMest.TDL_PATIENT_MOBILE ?? expMest.TDL_PATIENT_PHONE;
        //            txtBuyerPhone.Text = patient?.PHONE;
        //            txtAddress.Text = expMest.TDL_PATIENT_ADDRESS;
        //            txtEmail.Text = GetPatientEmail(expMest.TDL_PATIENT_ID ?? 0);
        //        }
        //    }
        //}

        private void SetDefaultCreateQR()
        {
            try
            {
                listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(s => s.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(s.VALUE)).ToList();
                if (listConfig.Count > 0 && listConfig != null || mediStock.QR_CONFIG_JSON != null)
                {
                    layoutbtnQRCe.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutbtnQRCe.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                btnQR.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatmentFee()
        {
            try
            {
                if (ExpMests != null && ExpMests.Count > 0)
                {
                    List<long> treatmentIds = ExpMests.Select(s => s.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                    Inventec.Common.Logging.LogSystem.Info("treatmentIds: " + string.Join(",", treatmentIds));
                    HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
                    feeFilter.IDs = treatmentIds;
                    var treatmentFees = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, feeFilter, null);
                    if (treatmentFees != null && treatmentFees.Count > 0)
                    {
                        this.currentTreatment = treatmentFees.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadControlByConfigShortCut()
        {
            try
            {
                if (Config.IsUsingFunctionKey)
                {
                    barButtonItemSave.ItemShortcut = new BarShortcut(Keys.F5);
                    btnSave.Text = "Lưu (F5)";
                    barBtnSavePrint.ItemShortcut = new BarShortcut(Keys.F9);
                    btnSavePrint.Text = "Lưu In (F9)";
                    barBtnNew.ItemShortcut = new BarShortcut(Keys.F8);
                    btnNew.Text = "Mới (F8)";
                    barBtnPrint.ItemShortcut = new BarShortcut(Keys.F10);
                    ddBtnPrint.Text = "In (F10)";
                }
                else
                {
                    this.barButtonItemSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
                    btnSave.Text = "Lưu (Ctrl S)";
                    this.barBtnSavePrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
                    btnSavePrint.Text = "Lưu In (Ctrl I)";
                    this.barBtnNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
                    btnNew.Text = "Mới (Ctrl N)";
                    this.barBtnPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
                    ddBtnPrint.Text = "In (Ctrl P)";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDafaultCashierRoom()
        {
            try
            {
                if (cboCashierRoom.EditValue == null)
                {
                    var data = cashierRoom.FirstOrDefault();
                    if (data != null)
                    {
                        txtCashierRoomCode.Text = data.CASHIER_ROOM_CODE;
                        cboCashierRoom.EditValue = data.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultPayForm()
        {
            try
            {
                if (cboPayFrom.EditValue == null)
                {
                    string code = String.IsNullOrEmpty(ConfigApplicationWorker.Get<string>(HFS_KEY__PAY_FORM_CODE)) ? GlobalVariables.HIS_PAY_FORM_CODE__CONSTANT : ConfigApplicationWorker.Get<string>(HFS_KEY__PAY_FORM_CODE);
                    var data = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.PAY_FORM_CODE == code);
                    if (data != null)
                    {
                        //txtPayFormCode.Text = data.PAY_FORM_CODE;
                        cboPayFrom.EditValue = data.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultAccountBook()
        {
            try
            {
                cboAccountBook.EditValue = null;
                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.DefaultAccountBookMedicineSaleBill != null && GlobalVariables.DefaultAccountBookMedicineSaleBill.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.DefaultAccountBookMedicineSaleBill.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }
                if (accountBook != null)
                {
                    cboAccountBook.EditValue = accountBook.ID;
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
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MEDICINE_SALE_BILL__PRINT_MENU__ITEM_IN_PHIEU_XUAT_BAN", Base.ResourceLangManager.LanguagefrmMedicineSaleBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInPhieuXuatBan)));
                //menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_SALE_CREATE__PRINT_MENU__ITEM_IN_HUONG_DAN_SU_DUNG", Base.ResourceLangManager.LanguageUCExpMestSaleCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInHuongDanSuDung)));
                menu.Items.Add(new DXMenuItem("In hóa đơn điện tử", new EventHandler(onClickInHoaDonDienTu)));

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInPhieuXuatBan(object sender, EventArgs e)
        {
            try
            {
                if (this.transactionBillResult == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092, deletePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool deletePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092:
                            InPhieuXuatBan(ref result, printTypeCode, fileName);
                            break;
                        case "Mps000339":
                            InHoaDonXuatBan(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InHoaDonXuatBan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (this.transactionBillResult == null)
                    return;
                WaitingManager.Show();

                CommonParam param = new CommonParam();
                HisBillGoodsFilter goodsFilter = new HisBillGoodsFilter();
                goodsFilter.BILL_ID = this.transactionBillResult.ID;
                List<HIS_BILL_GOODS> billGoods = new BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, goodsFilter, param);

                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.BILL_ID = this.transactionBillResult.ID;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);

                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                HisImpMestViewFilter hisImpMestFilter = new HisImpMestViewFilter();
                hisImpMestFilter.MOBA_EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_IMP_MEST> hisImpMest = new BackendAdapter(param)
                    .Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetVIew", ApiConsumers.MosConsumer, hisImpMestFilter, param);


                MPS.Processor.Mps000339.PDO.Mps000339PDO rdo = new MPS.Processor.Mps000339.PDO.Mps000339PDO(transactionBillResult, billGoods, expMestMedicines, expMestMaterials, hisImpMest);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                MPS.ProcessorBase.Core.PrintData printdata = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printdata);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatBan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("InPhieuXuatBan_0");
                if (this.transactionBillResult == null)
                    return;
                Inventec.Common.Logging.LogSystem.Warn("InPhieuXuatBan_0.1");
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Warn("InPhieuXuatBan_0.2");
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Warn("InPhieuXuatBan_1");
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.BILL_ID = this.transactionBillResult.ID;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);
                Inventec.Common.Logging.LogSystem.Warn("InPhieuXuatBan_2");
                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, param);
                Inventec.Common.Logging.LogSystem.Warn("InPhieuXuatBan_3");
                HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, param);
                Inventec.Common.Logging.LogSystem.Warn("InPhieuXuatBan_4");
                HisImpMestViewFilter hisImpMestFilter = new HisImpMestViewFilter();
                hisImpMestFilter.MOBA_EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_IMP_MEST> hisImpMest = new BackendAdapter(param)
                    .Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, hisImpMestFilter, param);
                Inventec.Common.Logging.LogSystem.Warn("InPhieuXuatBan_5");
                V_HIS_TRANSACTION transaction = this.transactionBillResult;
                Inventec.Common.Logging.LogSystem.Warn("InPhieuXuatBan_6");
                MPS.Processor.Mps000092.PDO.Mps000092PDO rdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(expMests, expMestMedicines, expMestMaterials, transaction, hisImpMest);
                Inventec.Common.Logging.LogSystem.Warn("InPhieuXuatBan_7");
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData printdata = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(printdata);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtCashierRoomCode);
                //ValidationSingleControl(txtPayFormCode);
                //ValidationSingleControl(txtAccountBookCode);
                ValidationSingleControl(dtTransactionTime);
                ValidControlBuyerOrganization();
                ValidControlGridLookUp(cboAccountBook);
                ValidControlGridLookUp(cboPayFrom);
                ValidControlBuyerTaxCode();

                if (rdoCaNhan.Checked)
                {
                    var identityRule = new IdentityTypeValidationRule
                    {
                        cboIdentityType = cboIdentityType,
                        txtIdentityType = txtIdentityType
                    };
                    dxValidationProviderEditorInfo.SetValidationRule(cboIdentityType, identityRule);
                }
                else
                {
                    dxValidationProviderEditorInfo.SetValidationRule(cboIdentityType, null);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format("Trường dữ liệu bắt buộc");
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlBuyerOrganization()
        {
            try
            {
                BuyerOrganizationValidationRule validRule = new BuyerOrganizationValidationRule();
                validRule.txtBuyerOrganization = txtBuyerOrganization;
                dxValidationProviderEditorInfo.SetValidationRule(txtBuyerOrganization, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlGridLookUp(DevExpress.XtraEditors.GridLookUpEdit cboGridLookUp)
        {
            try
            {
                GridLookupEditValidationRule validRule = new GridLookupEditValidationRule();
                validRule.cboGridLookUp = cboGridLookUp;
                dxValidationProviderEditorInfo.SetValidationRule(cboGridLookUp, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBuyerTaxCode()
        {
            try
            {
                BuyerTaxCodeValidationRule validRule = new BuyerTaxCodeValidationRule();
                validRule.txtBuyerTaxCode = txtBuyerTaxCode;
                dxValidationProviderEditorInfo.SetValidationRule(txtBuyerTaxCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void LoadDataToComboPayForm()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPayFrom, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>(), controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboCashierRoom()
        {
            try
            {
                long branchId;
                branchId = WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId;
                var userRoomIds = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                    && o.BRANCH_ID == branchId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN).Select(s => s.ROOM_ID).ToList();

                cashierRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>();
                cashierRoom = cashierRoom.Where(o => userRoomIds.Contains(o.ROOM_ID) && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CASHIER_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCashierRoom, cashierRoom, controlEditorADO);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboTransaction(List<HIS_TRANSACTION> datas)
        {
            try
            {
                replaceTransactionADOs = new List<ReplaceTransactionADO>();
                if (datas != null && datas.Count > 0)
                {
                    this.lciOriginalTransaction.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciReplaceReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    foreach (var item in datas)
                    {
                        replaceTransactionADOs.Add(new ReplaceTransactionADO(item));
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TRANSACTION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TRANSACTION_TIME", "", 150, 2));
                columnInfos.Add(new ColumnInfo("NUM_ORDER", "", 50, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TRANSACTION_CODE", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboOriginalTransaction, replaceTransactionADOs, controlEditorADO);

                //có giao dịch thay thế truyền vào thì gắn giao dịch
                if (this.originalTransaction != null && replaceTransactionADOs.Exists(e => e.ID == this.originalTransaction.ID))
                {

                    cboOriginalTransaction.EditValue = this.originalTransaction.ID;
                    cboOriginalTransaction.Enabled = false;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboAccountBook()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (String.IsNullOrWhiteSpace(loginName))
                {
                    layoutControlGroup1.Enabled = false;
                    MessageBox.Show("Không thanh toán được, mời bạn chọn lại");
                    return;
                }
                this.ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
                List<long> ids = new List<long>();
                HisUserAccountBookFilter useAccountBookFilter = new HisUserAccountBookFilter();
                useAccountBookFilter.LOGINNAME__EXACT = loginName;
                var userAccountBooks = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_ACCOUNT_BOOK>>("api/HisUserAccountBook/Get", ApiConsumers.MosConsumer, useAccountBookFilter, null);

                List<HIS_CARO_ACCOUNT_BOOK> caroAccountBooks = null;
                if (cboCashierRoom.EditValue != null)
                {
                    HisCaroAccountBookFilter caroAccountBookFilter = new HisCaroAccountBookFilter();
                    caroAccountBookFilter.CASHIER_ROOM_ID = Convert.ToInt64(cboCashierRoom.EditValue);
                    caroAccountBooks = new BackendAdapter(new CommonParam()).Get<List<HIS_CARO_ACCOUNT_BOOK>>("api/HisCaroAccountBook/Get", ApiConsumers.MosConsumer, caroAccountBookFilter, null);
                }
                // Kiểm tra sổ còn hay k
                if (userAccountBooks != null && userAccountBooks.Count > 0)
                {
                    ids.AddRange(userAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }
                if (caroAccountBooks != null && caroAccountBooks.Count > 0)
                {
                    ids.AddRange(caroAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }
                ids = ids.Distinct().ToList();
                if (ids != null && ids.Count > 0)
                {
                    int count = ids.Count;
                    int step = 0;
                    while (count > 0)
                    {
                        var lstId = ids.Skip(step).Take(100).ToList();
                        HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                        acFilter.IDs = lstId;
                        acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        acFilter.FOR_BILL = true;
                        acFilter.IS_OUT_OF_BILL = false;
                        acFilter.ORDER_DIRECTION = "DESC";
                        acFilter.ORDER_FIELD = "ID";
                        ListAccountBook.AddRange(new BackendAdapter(new CommonParam()).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null));
                        step += 100;
                        count -= 100;
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboAccountBook, ListAccountBook, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMest()
        {
            try
            {
                this.ExpMests = null;
                if (expMestIdForEdits != null && expMestIdForEdits.Count > 0)
                {
                    HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                    expMestFilter.IDs = this.expMestIdForEdits;
                    expMestFilter.ORDER_FIELD = "MODIFY_TIME";
                    expMestFilter.ORDER_DIRECTION = "DESC";
                    expMestFilter.HAS_BILL_ID = false;
                    var listExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, null);
                    if (listExpMest == null || listExpMest.Count == 0)
                    {
                        throw new Exception("Khong lay duoc expMest theo id: " + string.Join(", ", expMestIdForEdits));
                    }
                    this.ExpMests = listExpMest;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitResultSdoByExpMest()
        {
            try
            {
                if (this.ExpMests != null && this.ExpMests.Count > 0)
                {
                    listMediMateAdo = new List<HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO>();

                    List<Task> taskall = new List<Task>();
                    Task tsMedicine = Task.Factory.StartNew((object obj) =>
                    {
                        List<V_HIS_EXP_MEST> data = obj as List<V_HIS_EXP_MEST>;
                        HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                        expMestMedicineFilter.EXP_MEST_IDs = data.Select(s => s.ID).ToList();
                        listExpMestMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, null);
                        var listExpMestMedicineGroup = listExpMestMedicine.GroupBy(o => new { o.EXP_MEST_ID, o.MEDICINE_ID, o.PRICE, o.VAT_RATIO });
                        foreach (var ExpMestMedicineGroup in listExpMestMedicineGroup)
                        {
                            HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO MediMateTypeADO = new HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO(ExpMestMedicineGroup.ToList(), this.ExpMests.FirstOrDefault(o => o.ID == ExpMestMedicineGroup.Key.EXP_MEST_ID));
                            if (MediMateTypeADO.EXP_AMOUNT > 0)
                                listMediMateAdo.Add(MediMateTypeADO);
                        }
                    }, this.ExpMests);
                    taskall.Add(tsMedicine);

                    Task tsMaterial = Task.Factory.StartNew((object obj) =>
                    {
                        List<V_HIS_EXP_MEST> data = obj as List<V_HIS_EXP_MEST>;
                        HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                        expMestMaterialFilter.EXP_MEST_IDs = data.Select(s => s.ID).ToList();
                        listExpMestMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, null);
                        var listExpMestMaterialGroup = listExpMestMaterial.GroupBy(o => new { o.EXP_MEST_ID, o.MATERIAL_ID, o.PRICE, o.VAT_RATIO });
                        foreach (var ExpMestMedicineGroup in listExpMestMaterialGroup)
                        {
                            HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO MediMateTypeADO = new HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO(ExpMestMedicineGroup.ToList(), this.ExpMests.FirstOrDefault(o => o.ID == ExpMestMedicineGroup.Key.EXP_MEST_ID));
                            if (MediMateTypeADO.EXP_AMOUNT > 0)
                                listMediMateAdo.Add(MediMateTypeADO);
                        }
                    }, this.ExpMests);
                    taskall.Add(tsMaterial);

                    List<HIS_TRANSACTION> originalTran = null;

                    Task tsTrans = Task.Factory.StartNew((object obj) =>
                    {
                        List<V_HIS_EXP_MEST> data = obj as List<V_HIS_EXP_MEST>;
                        HisTransactionReplaceFilter tranReplaceFilter = new HisTransactionReplaceFilter();
                        tranReplaceFilter.TDL_EXP_MEST_CODEs = data.Select(s => s.EXP_MEST_CODE).ToList();
                        originalTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/GetReplaceTransaction", ApiConsumers.MosConsumer, tranReplaceFilter, null);

                    }, this.ExpMests);
                    taskall.Add(tsTrans);

                    Task.WaitAll(taskall.ToArray());

                    if (listMediMateAdo.Count <= 0)
                    {
                        XtraMessageBox.Show("Không có chi tiết thuốc/ vật tư, hoặc thuốc/ vật tư đã bị thu hồi", "Thông báo", DefaultBoolean.True);
                        return;
                    }

                    LoadDataToComboTransaction(originalTran);
                }
                FillDataGridExpMestDetail(this.listMediMateAdo);
                SetTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataGridExpMestDetail(List<HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO> listMediMateAdo)
        {
            try
            {
                //listMediMateAdo.ForEach(o => o.Check = true);
                gridControlExpMestDetail.BeginUpdate();
                gridControlExpMestDetail.DataSource = listMediMateAdo;
                gridControlExpMestDetail.EndUpdate();

                //gridViewExpMestDetail.SelectRows(0, gridViewExpMestDetail.RowCount - 1);
                gridViewExpMestDetail.SelectAll();
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
                totalPrice = 0;
                decimal discount = 0;
                List<HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO> selecteds = listMediMateAdo.Where(s => s.Check).ToList();
                if (selecteds.Count > 0)
                {
                    totalPrice = selecteds.Sum(o => ((o.ADVISORY_TOTAL_PRICE ?? 0) - (o.DISCOUNT ?? 0)));
                    List<V_HIS_EXP_MEST> expMestSelects = this.ExpMests.Where(o => selecteds.Any(a => a.EXP_MEST_ID == o.ID)).ToList();
                    discount = expMestSelects.Sum(o => o.DISCOUNT ?? 0);
                    totalPrice = totalPrice - discount;
                }
                lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);
                lblDiscount.Text = Inventec.Common.Number.Convert.NumberToString(discount, ConfigApplications.NumberSeperator);
                UpdateCanThuLabel();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool IsAutoLoadOrgAndTaxCodeByPatient()
        {
            try
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CFG__AUTO_LOAD_ORG_AND_TAX_BY_PATIENT) == "1";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        private HIS_WORK_PLACE GetWorkPlaceById(long id)
        {
            try
            {
                if (this.workPlaces != null)
                {
                    var cached = this.workPlaces.FirstOrDefault(o => o.ID == id);
                    if (cached != null)
                        return cached;
                }

                var filter = new HisWorkPlaceFilter { ID = id };
                var list = new BackendAdapter(new CommonParam()).Get<List<HIS_WORK_PLACE>>("api/HisWorkPlace/Get", ApiConsumers.MosConsumer, filter, null);
                return list != null ? list.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
        private void SetBuyerInfo()
        {
            try
            {
                isSettingBuyerInfo = true;
                if (this.ExpMests == null || this.ExpMests.Count == 0)
                    return;

                var expMest = ExpMests.FirstOrDefault();
                if (expMest == null)
                {
                    MessageBox.Show("Không tìm thấy phiếu xuất!", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                long patientId = expMest.TDL_PATIENT_ID ?? 0;
                var patientFilter = new HisPatientFilter { ID = patientId };
                var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, null);
                var patient = patients?.FirstOrDefault();

                HIS_WORK_PLACE workPlace = null;
                if (patients != null && patients.Count > 0 && patients.FirstOrDefault().WORK_PLACE_ID != null)
                {
                    long workPlaceId = patients.FirstOrDefault().WORK_PLACE_ID ?? 0;
                    var workPlaceFilter = new HisWorkPlaceFilter { ID = workPlaceId };
                    var lstWorkPlace = new BackendAdapter(new CommonParam()).Get<List<HIS_WORK_PLACE>>("api/HisWorkPlace/Get", ApiConsumers.MosConsumer, workPlaceFilter, null);
                    workPlace = lstWorkPlace?.FirstOrDefault();
                }

                // Kiểm tra null cho currentTreatment
                long? feeWorkPlaceId = currentTreatment?.TDL_PATIENT_WORK_PLACE_ID;
                bool autoLoadOrgTax = IsAutoLoadOrgAndTaxCodeByPatient();

                // Cá nhân
                if (rdoCaNhan.Checked)
                {
                    txtName.Text = expMest.TDL_PATIENT_NAME ?? "";
                    txtIdentityType.Text = expMest.TDL_PATIENT_CCCD_NUMBER ?? expMest.TDL_PATIENT_CMND_NUMBER ?? expMest.TDL_PATIENT_PASSPORT_NUMBER ?? "";

                    if (!string.IsNullOrWhiteSpace(expMest.TDL_PATIENT_CMND_NUMBER) || !string.IsNullOrWhiteSpace(patient?.CMND_NUMBER))
                    {
                        txtIdentityType.Text = expMest.TDL_PATIENT_CMND_NUMBER ?? patient?.CMND_NUMBER ?? "";
                        cboIdentityType.EditValue = 1; // CMND
                    }
                    else if (!string.IsNullOrWhiteSpace(expMest.TDL_PATIENT_CCCD_NUMBER) || !string.IsNullOrWhiteSpace(patient?.CCCD_NUMBER))
                    {
                        txtIdentityType.Text = expMest.TDL_PATIENT_CCCD_NUMBER ?? patient?.CCCD_NUMBER ?? "";
                        cboIdentityType.EditValue = 2; // CCCD
                    }
                    else if (!string.IsNullOrWhiteSpace(expMest.TDL_PATIENT_PASSPORT_NUMBER) || !string.IsNullOrWhiteSpace(patient?.PASSPORT_NUMBER))
                    {
                        txtIdentityType.Text = expMest.TDL_PATIENT_PASSPORT_NUMBER ?? patient?.PASSPORT_NUMBER ?? "";
                        cboIdentityType.EditValue = 3; // PASSPORT
                    }
                    else
                    {
                        txtIdentityType.Text = "";
                        cboIdentityType.EditValue = null;
                    }

                    if (currentTreatment?.TDL_PATIENT_BUD_REL_UNIT_CODE != null || patient?.BUD_REL_UNIT_CODE != null)
                    {
                        txtBudRelUnitCode.Text = currentTreatment.TDL_PATIENT_BUD_REL_UNIT_CODE ?? patient.BUD_REL_UNIT_CODE;
                    }
                    txtBuyerPhone.Text = patient?.PHONE ?? expMest.TDL_PATIENT_PHONE ?? "";
                    txtAddress.Text = expMest.TDL_PATIENT_ADDRESS ?? "";

                    if (chkBHYT.Checked)
                    {
                        var patientTypeAlter = GetPatientTypeAlter(expMest.TDL_PATIENT_ID ?? 0);
                        if (patientTypeAlter != null)
                            txtAddress.Text = patientTypeAlter.ADDRESS ?? txtAddress.Text;
                    }

                    txtEmail.Text = GetPatientEmail(expMest.TDL_PATIENT_ID ?? 0) ?? "";

                    if (autoLoadOrgTax)
                    {
                        // Combobox “Đơn vị” theo nơi làm việc của người bệnh (TDL_PATIENT_WORK_PLACE_ID)
                        if (feeWorkPlaceId.HasValue)
                        {
                            cboBuyerOrganization.EditValue = feeWorkPlaceId.Value;
                        }
                        else
                        {
                            cboBuyerOrganization.EditValue = null;
                        }

                        // Textbox “Mã số thuế”: ưu tiên MST của người bệnh, nếu không có thì lấy MST của đơn vị
                        if (!string.IsNullOrEmpty(currentTreatment?.TDL_PATIENT_TAX_CODE))
                        {
                            txtBuyerTaxCode1.Text = currentTreatment.TDL_PATIENT_TAX_CODE;
                        }
                        else if (feeWorkPlaceId.HasValue)
                        {
                            var wp = GetWorkPlaceById(feeWorkPlaceId.Value);
                            txtBuyerTaxCode1.Text = wp != null ? (wp.TAX_CODE ?? "") : "";
                        }
                        else
                        {
                            txtBuyerTaxCode1.Text = "";
                        }
                    }
                    else
                    {
                        // Không auto-load: để trống cho người dùng tự chọn/nhập
                        cboBuyerOrganization.EditValue = null;
                        txtBuyerTaxCode1.Text = "";
                    }
                }
                // Cơ quan
                else if (rdoCoQuan.Checked)
                {
                    if (chkKhac1.Checked)
                    {
                        txtBuyerOrganization1.Visible = true;
                        cboBuyerOrganization1.Visible = false;
                    }
                    else
                    {
                        txtBuyerOrganization1.Visible = false;
                        cboBuyerOrganization1.Visible = true;

                        if (autoLoadOrgTax)
                        {
                            // Combobox “Đơn vị” theo nơi làm việc bệnh nhân, ngược lại để trống
                            cboBuyerOrganization1.EditValue = feeWorkPlaceId.HasValue ? (object)feeWorkPlaceId.Value : null;
                        }
                        else
                        {
                            // Không auto-load: để trống cho người dùng tự chọn
                            cboBuyerOrganization1.EditValue = null;
                        }
                    }

                    txtBuyerPhone.Text = expMest.TDL_PATIENT_MOBILE ?? expMest.TDL_PATIENT_PHONE ?? "";
                    txtAddress.Text = expMest.TDL_PATIENT_ADDRESS ?? "";

                    if (chkBHYT.Checked)
                    {
                        var patientTypeAlter = GetPatientTypeAlter(expMest.TDL_PATIENT_ID ?? 0);
                        if (patientTypeAlter != null)
                            txtAddress.Text = patientTypeAlter.ADDRESS ?? txtAddress.Text;
                    }

                    // Nguoi mua la co quan: uu tien dia chi cua don vi da chon
                    if (!chkKhac1.Checked)
                        SetAddressByWorkPlace(cboBuyerOrganization1.EditValue);

                    txtEmail.Text = GetPatientEmail(expMest.TDL_PATIENT_ID ?? 0) ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi định dạng dữ liệu đầu vào hoặc dữ liệu không hợp lệ!\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                isSettingBuyerInfo = false;
            }
        }

        /// <summary>
        /// Nap Dia chi theo don vi (HIS_WORK_PLACE.ADDRESS). Khong ghi de neu don vi khong co dia chi.
        /// </summary>
        private void SetAddressByWorkPlace(object workPlaceEditValue)
        {
            try
            {
                if (workPlaceEditValue == null)
                    return;

                long workPlaceId = 0;
                if (!long.TryParse(workPlaceEditValue.ToString(), out workPlaceId) || workPlaceId <= 0)
                    return;

                var workPlace = GetWorkPlaceById(workPlaceId);
                if (workPlace != null && !string.IsNullOrWhiteSpace(workPlace.ADDRESS))
                    txtAddress.Text = workPlace.ADDRESS;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string GetPatientEmail(long patientId)
        {
            var patientFilter = new HisPatientFilter { ID = patientId };
            var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, null);
            return patients?.FirstOrDefault()?.EMAIL;
        }

        private HIS_PATIENT_TYPE_ALTER GetPatientTypeAlter(long patientId)
        {
            var filter = new HisPatientTypeAlterFilter { TDL_PATIENT_ID = patientId, PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT };
            var alters = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, filter, null);
            return alters?.FirstOrDefault();
        }

        private void LoadMediStockByRoomId()
        {
            try
            {
                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_TYPE_ID == this.roomTypeId && o.ROOM_ID == this.roomId);
                if (this.mediStock == null)
                {
                    MessageBox.Show("Vui lòng truy cập kho để thực hiện chức năng này");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                if (this.module != null && !String.IsNullOrEmpty(this.module.text))
                {
                    this.Text = this.module.text;
                }

                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisDepartment.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicineSaleBill.frmMedicineSaleBill).Assembly);

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
                if (this.listMediMateAdo == null || this.listMediMateAdo.Count == 0 || this.ExpMests == null || this.ExpMests.Count <= 0)
                {
                    return;
                }
                positionHandle = -1;
                if (!btnSave.Enabled || !dxValidationProviderEditorInfo.Validate())
                    return;
                this.SaveProcess();
                if (cboPayFrom.EditValue != null && Convert.ToInt64(cboPayFrom.EditValue.ToString()) == 8)
                {
                    ShowModuleCreQr();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowModuleCreQr()
        {
            try
            {
                if (transactionBillResult == null || transactionBillResult.ID <= 0)
                {
                    return;
                }

                if (mediStock != null && !string.IsNullOrEmpty(mediStock.QR_CONFIG_JSON))
                {
                    ItemConfig config = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemConfig>(mediStock.QR_CONFIG_JSON);
                    if (config != null)
                    {
                        List<object> listArgs = new List<object>();
                        TransReqQRADO adoqr = new TransReqQRADO();
                        adoqr.TreatmentId = 0;
                        adoqr.ConfigValue = new HIS_CONFIG() { KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", config.BANK), VALUE = config.VALUE };
                        HIS_TRANSACTION tran = new HIS_TRANSACTION();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionBillResult);
                        adoqr.Transaction = tran;
                        adoqr.IssueInvoice = true;
                        adoqr.NotDisplayedInvoice = (chkHideHddt != null && chkHideHddt.Checked);
                        adoqr.TransReqId = CreateReqType.Transaction;
                        listArgs.Add(adoqr);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", roomId, roomTypeId, listArgs);
                    }
                    else
                    {
                        XtraMessageBox.Show("Định dạng Qr thiết lập trong kho phòng không hợp lệ", "Thông báo");
                    }
                }
                else if (listConfig != null && listConfig.Count == 1)
                {
                    selectedConfig = listConfig[0];
                    List<object> listArgs = new List<object>();
                    TransReqQRADO adoqr = new TransReqQRADO();
                    adoqr.TreatmentId = 0;
                    adoqr.ConfigValue = selectedConfig;
                    HIS_TRANSACTION tran = new HIS_TRANSACTION();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionBillResult);
                    adoqr.Transaction = tran;
                    adoqr.IssueInvoice = true;
                    adoqr.NotDisplayedInvoice = (chkHideHddt != null && chkHideHddt.Checked);
                    adoqr.TransReqId = CreateReqType.Transaction;
                    listArgs.Add(adoqr);
                    LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + selectedConfig.KEY);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", roomId, roomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool SaveProcess([Optional] bool isLuuKy)
        {
            bool result = false;
            try
            {
                List<HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO> seleteds = this.listMediMateAdo.Where(o => o.Check).ToList();
                if (seleteds == null || seleteds.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Người dùng chưa chọn phiếu xuất", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return false;
                }

                if (seleteds.Any(a => a.BILL_ID.HasValue))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại phiếu xuất đã thanh toán", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return false;
                }
                if (cboPayFrom.EditValue != null)
                {
                    long payFormId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPayFrom.EditValue.ToString());
                    HIS_PAY_FORM payForm = BackendDataWorker.Get<HIS_PAY_FORM>().SingleOrDefault(o => o.ID == payFormId);
                    if (payForm != null && (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT || payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK))
                    {
                        decimal value = 0;
                        if (spinTransferAmount.EditValue != null &&
                            decimal.TryParse(spinTransferAmount.EditValue.ToString(), out value))
                        {
                            transferAmount = value;
                        }
                        else
                        {
                            transferAmount = 0;
                        }

                        if (transferAmount > totalPrice)
                        {
                            string msg = string.Empty;
                            if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                            {
                                msg = "Số tiền chuyển khoản lớn hơn số tiền thanh toán của bệnh nhân";
                            }
                            else if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                            {
                                msg = "Số tiền quẹt thẻ lớn hơn số tiền thanh toán của bệnh nhân";
                            }

                            dxErrorProvider.SetError(spinTransferAmount, msg, ErrorType.Warning);
                            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            spinTransferAmount.Focus();
                            spinTransferAmount.SelectAll();
                            return false;
                        }
                        else
                        {
                            dxErrorProvider.SetError(spinTransferAmount, string.Empty);
                        }
                    }
                    else if(payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCKQT)
                    {
                        var priceTmCk = spinTransAmountNew.Value + spinSwipeAmountNew.Value;
                        if(priceTmCk > totalPrice)
                        {
                            string msg = string.Empty;
                            msg = string.Format("Tổng số tiền chuyển khoản và quẹt thẻ {0} lớn hơn số tiền thanh toán của bệnh nhân {1}", priceTmCk, totalPrice);
                            XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }

                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                HisTransactionBillGoodsSDO data = new HisTransactionBillGoodsSDO();
                data.HisBillGoods = new List<HIS_BILL_GOODS>();
                data.HisTransaction = new HIS_TRANSACTION();
                data.ExpMestIds = seleteds.Select(s => s.EXP_MEST_ID).Distinct().ToList();
                if (txtDescription.Text != null)
                {
                    data.HisTransaction.DESCRIPTION = txtDescription.Text;
                }

                if (rdoCaNhan.Checked) // Trường hợp người mua là cá nhân
                {
                    data.HisTransaction.BUYER_TYPE = 1; // Cá nhân
                    if (!string.IsNullOrEmpty(txtName.Text))
                    {
                        data.HisTransaction.BUYER_NAME = txtName.Text;
                    }
                    if (!string.IsNullOrWhiteSpace(txtIdentityType.Text))
                    {

                        data.HisTransaction.BUYER_IDENTITY_NUMBER = txtIdentityType.Text;
                        if (cboIdentityType.EditValue != null && long.TryParse(cboIdentityType.EditValue.ToString(), out long identityType))
                        {

                            data.HisTransaction.BUYER_IDENTITY_TYPE = (short?)identityType;
                        }
                        else
                        {
                            data.HisTransaction.BUYER_IDENTITY_TYPE = null;
                        }
                    }
                    if (chkKhac.Checked)
                    {
                        if (!string.IsNullOrEmpty(txtBuyerOrganization.Text))
                        {
                            data.HisTransaction.BUYER_ORGANIZATION = txtBuyerOrganization.Text;
                            data.HisTransaction.BUYER_WORK_PLACE_ID = null;
                        }
                    }
                    else
                    {
                        // Khi không tích Khác, lấy thông tin từ ComboBox
                        if (cboBuyerOrganization.EditValue != null && long.TryParse(cboBuyerOrganization.EditValue.ToString(), out long workPlaceId))
                        {
                            data.HisTransaction.BUYER_WORK_PLACE_ID = workPlaceId;
                            data.HisTransaction.BUYER_ORGANIZATION = cboBuyerOrganization.Text;
                        }
                        else
                        {
                            data.HisTransaction.BUYER_WORK_PLACE_ID = null;
                            data.HisTransaction.BUYER_ORGANIZATION = cboBuyerOrganization.Text;
                        }
                    }
                    data.HisTransaction.BUYER_SOCIAL_RELATIONS_CODE = txtBudRelUnitCode.Text;
                    data.HisTransaction.BUYER_TAX_CODE = txtBuyerTaxCode1.Text;
                    data.HisTransaction.BUYER_PHONE = txtBuyerPhone.Text;
                    data.HisTransaction.BUYER_ADDRESS = txtAddress.Text;
                    data.HisTransaction.BUYER_EMAIL = txtEmail.Text;
                    if (cboPayFrom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PAY_FORM gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayFrom.EditValue.ToString()));
                        if (gt != null)
                        {
                            data.HisTransaction.PAY_FORM_ID = gt.ID;

                            // NEW: Gán số tiền CK / QT theo cấu hình
                            decimal value = 0;
                            if (spinTransferAmount.EditValue != null &&
                                decimal.TryParse(spinTransferAmount.EditValue.ToString(), out value))
                            {
                                transferAmount = value;
                            }
                            else
                            {
                                transferAmount = 0;
                            }

                            if (gt.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK) // Tiền mặt/Chuyển khoản
                            {
                                data.HisTransaction.TRANSFER_AMOUNT = transferAmount;
                                data.HisTransaction.SWIPE_AMOUNT = null;
                            }
                            else if (gt.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT) // Tiền mặt/Quẹt thẻ
                            {
                                data.HisTransaction.SWIPE_AMOUNT = transferAmount;
                                data.HisTransaction.TRANSFER_AMOUNT = null;
                            }
                            else if(gt.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCKQT)
                            {
                                data.HisTransaction.SWIPE_AMOUNT = spinSwipeAmountNew.Value;
                                data.HisTransaction.TRANSFER_AMOUNT = spinTransAmountNew.Value;
                            }
                            else
                            {
                                data.HisTransaction.TRANSFER_AMOUNT = null;
                                data.HisTransaction.SWIPE_AMOUNT = null;
                            }
                        }
                    }

                }
                else if (rdoCoQuan.Checked) // Trường hợp người mua là cơ quan
                {
                    data.HisTransaction.BUYER_TYPE = 2; // Cơ quan
                    if (!string.IsNullOrEmpty(txtName.Text))
                    {
                        data.HisTransaction.BUYER_NAME = txtName.Text;
                    }
                    if (chkKhac1.Checked)
                    {
                        if (!string.IsNullOrEmpty(txtBuyerOrganization1.Text))
                        {
                            data.HisTransaction.BUYER_ORGANIZATION = txtBuyerOrganization1.Text;
                            data.HisTransaction.BUYER_WORK_PLACE_ID = null;
                        }
                    }
                    else
                    {
                        if (cboBuyerOrganization1.EditValue != null
                            && long.TryParse(cboBuyerOrganization1.EditValue.ToString(), out long workPlaceId))
                        {
                            data.HisTransaction.BUYER_WORK_PLACE_ID = workPlaceId;
                            data.HisTransaction.BUYER_ORGANIZATION = cboBuyerOrganization1.Text;
                        }
                        else
                        {
                            data.HisTransaction.BUYER_WORK_PLACE_ID = null;
                            data.HisTransaction.BUYER_ORGANIZATION = null;
                        }
                    }
                    data.HisTransaction.BUYER_SOCIAL_RELATIONS_CODE = txtBudRelUnitCode1.Text;
                    data.HisTransaction.BUYER_TAX_CODE = txtBuyerTaxCode.Text;
                    data.HisTransaction.BUYER_PHONE = txtBuyerPhone.Text;
                    data.HisTransaction.BUYER_ADDRESS = txtAddress.Text;
                    data.HisTransaction.BUYER_EMAIL = txtEmail.Text;
                    if (cboPayFrom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PAY_FORM gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayFrom.EditValue.ToString()));
                        if (gt != null)
                        {
                            data.HisTransaction.PAY_FORM_ID = gt.ID;

                            // NEW: Gán số tiền CK / QT theo cấu hình
                            decimal value = 0;
                            if (spinTransferAmount.EditValue != null &&
                                decimal.TryParse(spinTransferAmount.EditValue.ToString(), out value))
                            {
                                transferAmount = value;
                            }
                            else
                            {
                                transferAmount = 0;
                            }

                            if (gt.PAY_FORM_CODE == "03") // Tiền mặt/Chuyển khoản
                            {
                                data.HisTransaction.TRANSFER_AMOUNT = transferAmount;
                                data.HisTransaction.SWIPE_AMOUNT = null;
                            }
                            else if (gt.PAY_FORM_CODE == "06") // Tiền mặt/Quẹt thẻ
                            {
                                data.HisTransaction.SWIPE_AMOUNT = transferAmount;
                                data.HisTransaction.TRANSFER_AMOUNT = null;
                            }
                            else
                            {
                                data.HisTransaction.TRANSFER_AMOUNT = null;
                                data.HisTransaction.SWIPE_AMOUNT = null;
                            }
                        }
                    }
                }
                else // Trường hợp mặc định (không chọn cá nhân hoặc cơ quan)
                {
                    if (!string.IsNullOrEmpty(txtName.Text))
                    {
                        data.HisTransaction.BUYER_NAME = txtName.Text;
                    }
                    data.HisTransaction.BUYER_ADDRESS = txtAddress.Text;
                    data.HisTransaction.BUYER_EMAIL = txtEmail.Text;
                    //data.HisTransaction.BUYER_ORGANIZATION = txtBuyerOgranization.Text;
                    data.HisTransaction.BUYER_TAX_CODE = txtBuyerTaxCode.Text;
                    data.HisTransaction.BUYER_PHONE = txtBuyerPhone.Text;
                }

                if (cboPayFrom.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_PAY_FORM gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayFrom.EditValue.ToString()));
                    if (gt != null)
                    {
                        data.HisTransaction.PAY_FORM_ID = gt.ID;
                    }
                }
                if (cboAccountBook.EditValue != null)
                {
                    V_HIS_ACCOUNT_BOOK gt = this.ListAccountBook.SingleOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    if (gt != null)
                    {
                        data.HisTransaction.ACCOUNT_BOOK_ID = gt.ID;
                        if (gt.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            data.HisTransaction.NUM_ORDER = (long)spinNumOrder.Value;
                        }
                    }
                }

                data.HisTransaction.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT;
                if (cboCashierRoom.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboCashierRoom.EditValue.ToString()));
                    if (gt != null)
                    {
                        data.HisTransaction.CASHIER_ROOM_ID = gt.ID;
                    }
                }

                if (dtTransactionTime.DateTime != null)
                {
                    data.HisTransaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }

                if (this.currentTreatment != null)
                {
                    data.HisTransaction.TREATMENT_ID = this.currentTreatment.ID;
                }
                if (checkOverTime.Checked)
                {
                    data.HisTransaction.IS_NOT_IN_WORKING_TIME = 1;
                }
                else
                {
                    data.HisTransaction.IS_NOT_IN_WORKING_TIME = null;
                }

                List<HIS_BILL_GOODS> billGooDs = new List<HIS_BILL_GOODS>();

                if (seleteds != null && seleteds.Count > 0)
                {
                    foreach (var expMedicineGroup in seleteds)
                    {
                        HIS_BILL_GOODS billGoood = new HIS_BILL_GOODS();
                        billGoood.AMOUNT = expMedicineGroup.EXP_AMOUNT;
                        billGoood.PRICE = (expMedicineGroup.ADVISORY_PRICE ?? 0) * (1 + expMedicineGroup.EXP_VAT_RATIO ?? 0); ;
                        billGoood.GOODS_NAME = expMedicineGroup.MEDI_MATE_TYPE_NAME;
                        billGoood.DESCRIPTION = expMedicineGroup.DESCRIPTION;
                        billGoood.GOODS_UNIT_NAME = expMedicineGroup.SERVICE_UNIT_NAME;
                        billGoood.DISCOUNT = expMedicineGroup.DISCOUNT;
                        billGooDs.Add(billGoood);
                    }

                    data.HisBillGoods = billGooDs;
                }

                if (lciOriginalTransaction.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    && cboOriginalTransaction.EditValue != null)
                {
                    if (String.IsNullOrWhiteSpace(txtReplaceReason.Text))
                    {
                        MessageBox.Show("Vui lòng nhập lý do thay thế");
                        txtReplaceReason.Focus();
                        return false;
                    }
                    data.OriginalTransactionId = Inventec.Common.TypeConvert.Parse.ToInt64(cboOriginalTransaction.EditValue.ToString());
                    data.ReplaceReason = txtReplaceReason.Text;
                }
                Inventec.Common.Logging.LogSystem.Info("Call API api/HisTransaction/CreateBillWithBillGood | InputData=" + Inventec.Common.Logging.LogUtil.TraceData("data", data));

                this.transactionBillResult = new BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/CreateBillWithBillGood", ApiConsumers.MosConsumer, data, param);

                if (this.transactionBillResult != null)
                {
                    result = true;
                    success = true;
                    btnSave.Enabled = false;
                    btnSavePrint.Enabled = false;
                    BtnSaveSign.Enabled = false;
                    ddBtnPrint.Enabled = true;

                    if (cboPayFrom.EditValue != null && Convert.ToInt64(cboPayFrom.EditValue.ToString()) == 8)
                    {
                        btnQR.Enabled = true;
                    }
                    if (delegateSelectData != null)
                    {
                        delegateSelectData(this.transactionBillResult);
                    }

                    if (isLuuKy && InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt && Convert.ToInt64(cboPayFrom.EditValue.ToString()) != 8)
                    {
                        HIS_TRANSACTION tran = new HIS_TRANSACTION();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionBillResult);
                        //Tao hoa don dien thu ben thu3 
                        ElectronicBillResult electronicBillResult = TaoHoaDonDienTuBenThu3CungCap(tran, seleteds);
                        if (electronicBillResult == null || !electronicBillResult.Success)
                        {
                            param.Messages.Add("Tạo hóa đơn điện tử thất bại");
                            if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                            {
                                param.Messages.AddRange(electronicBillResult.Messages);
                            }

                            param.Messages = param.Messages.Distinct().ToList();
                        }
                        else
                        {
                            //goi api update
                            CommonParam paramUpdate = new CommonParam();
                            HisTransactionInvoiceInfoSDO sdo = new HisTransactionInvoiceInfoSDO();
                            sdo.EinvoiceLoginname = electronicBillResult.InvoiceLoginname;
                            sdo.InvoiceCode = electronicBillResult.InvoiceCode;
                            sdo.InvoiceSys = electronicBillResult.InvoiceSys;
                            sdo.EinvoiceNumOrder = electronicBillResult.InvoiceNumOrder;
                            sdo.EInvoiceTime = electronicBillResult.InvoiceTime ?? (Inventec.Common.DateTime.Get.Now() ?? 0);
                            sdo.Id = transactionBillResult.ID;
                            sdo.InvoiceLookupCode = electronicBillResult.InvoiceLookupCode;
                            var apiResult = new BackendAdapter(paramUpdate).Post<bool>("api/HisTransaction/UpdateInvoiceInfo", ApiConsumers.MosConsumer, sdo, paramUpdate);
                            {
                                transactionBillResult.INVOICE_CODE = electronicBillResult.InvoiceCode;
                                transactionBillResult.INVOICE_SYS = electronicBillResult.InvoiceSys;
                                transactionBillResult.EINVOICE_NUM_ORDER = electronicBillResult.InvoiceNumOrder;
                                transactionBillResult.EINVOICE_TIME = electronicBillResult.InvoiceTime;
                                transactionBillResult.EINVOICE_LOGINNAME = electronicBillResult.InvoiceLoginname;
                                transactionBillResult.EINVOICE_TIME = electronicBillResult.InvoiceTime ?? (Inventec.Common.DateTime.Get.Now() ?? 0);
                                transactionBillResult.INVOICE_LOOKUP_CODE = electronicBillResult.InvoiceLookupCode;
                                result = true;
                                success = true;
                                btnSave.Enabled = false;
                                btnSavePrint.Enabled = false;
                                BtnSaveSign.Enabled = false;
                                ddBtnPrint.Enabled = true;
                                //btnQR.Enabled = true;
                                if (delegateSelectData != null)
                                {
                                    delegateSelectData(this.transactionBillResult);
                                }
                            }
                        }
                    }
                }
                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                }
                SessionManager.ProcessTokenLost(param);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
                result = false;
            }
            return result;
        }

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap(HIS_TRANSACTION transaction, List<HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO> seleteds)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                List<V_HIS_SERE_SERV_5> sereServBills = new List<V_HIS_SERE_SERV_5>();
                if (seleteds == null)
                {
                    result.Success = false;
                    Inventec.Common.Logging.LogSystem.Debug("Khong co dich vu thanh toan nao duoc chon!");
                    return result;
                }

                //Cột đơn giá = giá bán(trên PM HIS)*100%/(100+ VAS nhập từ nhà cung cấp)
                //-Thuế xuất = VAS nhập từ nhà cung cấp
                foreach (var item in seleteds)
                {
                    V_HIS_SERE_SERV_5 sereServBill = new V_HIS_SERE_SERV_5();

                    sereServBill.AMOUNT = item.EXP_AMOUNT;
                    sereServBill.VAT_RATIO = item.IMP_VAT_RATIO ?? 0;
                    sereServBill.TDL_SERVICE_CODE = item.MEDI_MATE_TYPE_CODE;
                    sereServBill.TDL_SERVICE_NAME = item.MEDI_MATE_TYPE_NAME;
                    //sereServBill.DESCRIPTION = item.DESCRIPTION;
                    sereServBill.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                    sereServBill.DISCOUNT = item.DISCOUNT;
                    sereServBill.PRICE = (item.ADVISORY_PRICE ?? 0) * (1 + item.EXP_VAT_RATIO ?? 0) * (1 / (1 + item.IMP_VAT_RATIO ?? 0));
                    sereServBill.VIR_TOTAL_PATIENT_PRICE = sereServBill.PRICE * sereServBill.AMOUNT;
                    var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                    if (service != null)
                    {
                        sereServBill.TDL_SERVICE_TAX_RATE_TYPE = service.TAX_RATE_TYPE;
                    }

                    sereServBills.Add(sereServBill);
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.Amount = transaction.AMOUNT;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                if (!String.IsNullOrWhiteSpace(lblDiscount.Text))
                {
                    dataInput.Discount = decimal.Parse(lblDiscount.Text);
                }

                //dataInput.DiscountRatio = txtDiscountRatio.Value;
                dataInput.PaymentMethod = cboPayFrom.Text;
                dataInput.SereServs = sereServBills;
                if (currentTreatment == null || currentTreatment.ID == 0)
                {
                    this.currentTreatment = new V_HIS_TREATMENT_FEE();
                    currentTreatment.TDL_PATIENT_ACCOUNT_NUMBER = ExpMests.FirstOrDefault().TDL_PATIENT_ACCOUNT_NUMBER ?? transaction.BUYER_ACCOUNT_NUMBER;
                    currentTreatment.TDL_PATIENT_ADDRESS = ExpMests.FirstOrDefault().TDL_PATIENT_ADDRESS ?? transaction.BUYER_ADDRESS;
                    currentTreatment.TDL_PATIENT_PHONE = ExpMests.FirstOrDefault().TDL_PATIENT_PHONE ?? transaction.BUYER_PHONE;
                    currentTreatment.TDL_PATIENT_TAX_CODE = ExpMests.FirstOrDefault().TDL_PATIENT_TAX_CODE ?? transaction.BUYER_TAX_CODE;
                    currentTreatment.TDL_PATIENT_WORK_PLACE = ExpMests.FirstOrDefault().TDL_PATIENT_WORK_PLACE ?? transaction.BUYER_ORGANIZATION;
                    currentTreatment.TDL_PATIENT_NAME = ExpMests.FirstOrDefault().TDL_PATIENT_NAME ?? transaction.BUYER_NAME;
                    currentTreatment.TDL_PATIENT_CODE = ExpMests.FirstOrDefault().TDL_PATIENT_CODE;
                    currentTreatment.TDL_PATIENT_COMMUNE_CODE = ExpMests.FirstOrDefault().TDL_PATIENT_COMMUNE_CODE;
                    currentTreatment.TDL_PATIENT_DISTRICT_CODE = ExpMests.FirstOrDefault().TDL_PATIENT_DISTRICT_CODE;
                    currentTreatment.TDL_PATIENT_DOB = ExpMests.FirstOrDefault().TDL_PATIENT_DOB ?? 0;
                    currentTreatment.TDL_PATIENT_MOBILE = ExpMests.FirstOrDefault().TDL_PATIENT_MOBILE;
                    currentTreatment.TDL_PATIENT_NATIONAL_NAME = ExpMests.FirstOrDefault().TDL_PATIENT_NATIONAL_NAME;
                    currentTreatment.TDL_PATIENT_GENDER_NAME = ExpMests.FirstOrDefault().TDL_PATIENT_GENDER_NAME;
                    currentTreatment.ID = -1;//để các api trong thư viện không lấy được dữ liệu
                    currentTreatment.PATIENT_ID = -1;
                }

                dataInput.Treatment = this.currentTreatment;
                dataInput.Currency = "VND";
                dataInput.Transaction = transaction;
                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    dataInput.SymbolCode = accountBook.SYMBOL_CODE;
                    dataInput.TemplateCode = accountBook.TEMPLATE_CODE;
                    dataInput.EinvoiceTypeId = accountBook.EINVOICE_TYPE_ID;
                }

                //if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                //{
                //    dataInput.TransactionTime = Convert.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmmss"));
                //}
                dataInput.NumOrder = transaction.NUM_ORDER;
                dataInput.TransactionTime = transaction.EINVOICE_TIME ?? transaction.TRANSACTION_TIME;
                dataInput.ENumOrder = transaction.EINVOICE_NUM_ORDER;

                WaitingManager.Show();
                //Luôn hiển thị tất cả dịch vụ. Template4
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput, Library.ElectronicBill.Template.TemplateEnum.TYPE.TemplateNhaThuoc);
                result = electronicBillProcessor.Run(ElectronicBillType.ENUM.CREATE_INVOICE);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void gridViewExpMestDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {

                    var data = (HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "ADVISORY_PRICE_DISPLAY")
                        {
                            if (data.ADVISORY_PRICE != null)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.ADVISORY_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        else if (e.Column.FieldName == "ADVISORY_TOTAL_PRICE_DISPLAY")
                        {
                            if (data.ADVISORY_TOTAL_PRICE != null)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(((data.ADVISORY_TOTAL_PRICE ?? 0) - (data.DISCOUNT ?? 0)), ConfigApplications.NumberSeperator);
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        else if (e.Column.FieldName == "VAT_RATIO_STR")
                        {
                            e.Value = (data.EXP_VAT_RATIO ?? 0) * 100;
                        }
                        else if (e.Column.FieldName == "DISCOUNT_STR")
                        {
                            if (data.DISCOUNT != null)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.DISCOUNT ?? 0, ConfigApplications.NumberSeperator);
                            }
                            else
                            {
                                e.Value = null;
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

        private void cboAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (spinNumOrder.Enabled)
                    {
                        spinNumOrder.Focus();
                    }
                    else
                    {
                        txtCashierRoomCode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboAccountBook.EditValue == null)
                    {
                        cboAccountBook.ShowPopup();
                    }

                }
                else
                {
                    cboAccountBook.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCashierRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtCashierRoomCode.Text))
                    {
                        string key = txtCashierRoomCode.Text.ToUpper();
                        var data = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().Where(o => o.CASHIER_ROOM_CODE.ToUpper().Contains(key) ||
                            o.CASHIER_ROOM_NAME.ToUpper().Contains(key)).ToList();
                        if (data != null && data.Count == 1)
                        {
                            valid = true;
                            txtCashierRoomCode.Text = data.First().CASHIER_ROOM_CODE;
                            cboCashierRoom.EditValue = data.First().ID;

                        }
                    }

                    cboCashierRoom.Focus();
                    cboCashierRoom.ShowPopup();

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCashierRoom.EditValue != null && cboCashierRoom.EditValue != cboCashierRoom.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboCashierRoom.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtCashierRoomCode.Text = gt.CASHIER_ROOM_CODE;
                            cboCashierRoom.Focus();
                            cboCashierRoom.ShowPopup();
                        }
                    }
                    else
                    {
                        cboCashierRoom.Focus();
                        cboCashierRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCashierRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCashierRoom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboCashierRoom.EditValue.ToString()));
                        if (gt != null)
                        {
                            cboCashierRoom.Focus();
                            cboCashierRoom.ShowPopup();
                        }
                    }
                    else
                    {
                        cboCashierRoom.ShowPopup();
                    }
                }
                else
                {
                    cboCashierRoom.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransactionTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtTransactionTime.EditValue != null)
                    {
                        txtDescription.Focus();
                    }
                    else
                    {
                        dtTransactionTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPayFrom.EditValue != null && cboPayFrom.EditValue != cboPayFrom.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_PAY_FORM gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayFrom.EditValue.ToString()));
                        if (gt != null)
                        {
                            //txtPayFormCode.Text = gt.PAY_FORM_CODE;
                            dtTransactionTime.Focus();
                        }
                    }
                    else
                    {
                        dtTransactionTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPayFrom.EditValue != null)
                    {
                        HIS_PAY_FORM gt = BackendDataWorker.Get<HIS_PAY_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayFrom.EditValue.ToString()));
                        if (gt != null)
                        {
                            dtTransactionTime.Focus();
                        }
                    }
                    else
                    {
                        cboPayFrom.ShowPopup();
                    }
                }
                else
                {
                    cboPayFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnSave.Enabled == true)
                    {
                        btnSave.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnSave.Enabled)
                    return;
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //txtAccountBookCode.Text = "";
                spinNumOrder.Enabled = false;
                if (cboAccountBook.EditValue != null)
                {
                    V_HIS_ACCOUNT_BOOK gt = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    if (gt != null)
                    {
                        //txtAccountBookCode.Text = gt.ACCOUNT_BOOK_CODE;
                        spinNumOrder.Value = gt.CURRENT_NUM_ORDER.HasValue ? (gt.CURRENT_NUM_ORDER.Value + 1) : gt.FROM_NUM_ORDER;
                        if (gt.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spinNumOrder.Enabled = true;
                        }

                        GlobalVariables.DefaultAccountBookMedicineSaleBill = new List<V_HIS_ACCOUNT_BOOK>();
                        GlobalVariables.DefaultAccountBookMedicineSaleBill.Add(gt);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void txtBuyerAccountCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerPhone.Focus();
                    txtBuyerPhone.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerPhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SearchExpMestBill();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetDefaultValue()
        {
            try
            {
                this.listMediMateAdo = new List<HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO>();
                this.listExpMestMedicine = null;
                this.listExpMestMaterial = null;
                this.ExpMests = null;
                this.Patients = null;
                this.transactionBillResult = null;
                this.delegateSelectData = null;
                txtBuyerOrganization1.Text = "";
                txtBuyerPhone.Text = "";
                txtBuyerTaxCode.Text = "";
                txtDescription.Text = "";
                lblDiscount.Text = "";
                lblTotalPrice.Text = "";
                dtTransactionTime.EditValue = DateTime.Now;
                ddBtnPrint.Enabled = false;
                btnSave.Enabled = true;
                btnSavePrint.Enabled = true;
                BtnSaveSign.Enabled = true;
                this.currentTreatment = null;
                checkOverTime.Checked = GlobalVariables.MedicineSaleBill__IsOverTime;
                this.lciOriginalTransaction.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lciReplaceReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.cboOriginalTransaction.Enabled = true;
                this.cboOriginalTransaction.EditValue = null;
                this.txtReplaceReason.Text = null;
                totalPrice = 0;
                transferAmount = 0;
                spinTransferAmount.EditValue = null;
                spinTransAmountNew.EditValue = null;
                spinSwipeAmountNew.EditValue = null;
                dxErrorProvider.SetError(spinTransferAmount, string.Empty);
                dxErrorProvider.SetError(spinTransAmountNew, string.Empty);
                dxErrorProvider.SetError(spinSwipeAmountNew, string.Empty);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchExpMestBill()
        {
            try
            {
                WaitingManager.Show();
                this.ResetDefaultValue();
                if (this.mediStock != null)
                {
                    SetDefaultAccountBook();
                    SetDefaultPayForm();
                    SetDafaultCashierRoom();
                    this.LoadSearch();
                    this.InitResultSdoByExpMest();
                    LoadTreatmentFee();
                    SetBuyerInfo();
                }

                if (this.ExpMests != null && this.ExpMests.Count > 0)
                {
                    btnSave.Enabled = true;
                    btnSavePrint.Enabled = true;
                    BtnSaveSign.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                    btnSavePrint.Enabled = false;
                    BtnSaveSign.Enabled = false;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool LoadSearch()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestViewFilter filter = new HisExpMestViewFilter();
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN;
                filter.MEDI_STOCK_ID = this.mediStock.ID;
                filter.HAS_BILL_ID = false;
                filter.IS_NOT_TAKEN = false;

                if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TDL_TREATMENT_CODE__EXACT = code;
                }
                else if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    string code = txtExpMestCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpMestCode.Text = code;
                    }
                    filter.EXP_MEST_CODE__EXACT = code;
                }

                var listExpMest = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, filter, param);
                if (listExpMest != null && listExpMest.Count > 0)
                {
                    this.ExpMests = listExpMest;
                    return true;
                }
                else
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Không tìm thấy phiếu xuất nào", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled) return;
                this.SearchExpMestBill();
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
                if (!btnNew.Enabled) return;
                txtExpMestCode.Text = "";
                txtTreatmentCode.Text = "";
                txtTreatmentCode.Focus();
                this.SearchExpMestBill();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ddBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ddBtnPrint.ShowDropDown();
                //if (!ddBtnPrint.Enabled || this.transactionBillResult == null) return;
                //this.onClickInPhieuXuatBan(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.listMediMateAdo == null || this.listMediMateAdo.Count == 0 || this.ExpMests == null || this.ExpMests.Count <= 0)
                {
                    return;
                }
                positionHandle = -1;
                if (!btnSavePrint.Enabled || !dxValidationProviderEditorInfo.Validate())
                    return;
                if (this.SaveProcess())
                {
                    // Viec 3082: chi nut "Luu ky" duoc phep lam viec voi hoa don dien tu.
                    // Config bat -> "Luu In" chi in phieu xuat ban: bill luu bang nut nay KHONG phat hanh HDDT
                    // (SaveProcess chi phat hanh khi isLuuKy = true) nen goi GET_INVOICE_LINK se bao loi.
                    if (Config.PrintNowMps == "Mps000339" && !Config.IsSaveSignPrintAutoExport)
                    {
                        this.onClickInHoaDonDienTu(null, null);
                    }
                    else
                    {
                        this.onClickInPhieuXuatBan(null, null);
                    }
                }
                if (cboPayFrom.EditValue != null && Convert.ToInt64(cboPayFrom.EditValue.ToString()) == 8)
                {
                    ShowModuleCreQr();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSavePrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSavePrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ddBtnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridView gridView = sender as GridView;
                GridHitInfo hitInfo = gridView.CalcHitInfo(e.Location);

                if (hitInfo.Column == null || hitInfo.Column.FieldName != "DX$CheckboxSelectorColumn")
                {
                    return;
                }
                if (hitInfo.HitTest == GridHitTest.RowGroupCheckSelector || hitInfo.RowHandle >= 0)
                {
                    ((DXMouseEventArgs)e).Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                int[] selectedIndexs = gridViewExpMestDetail.GetSelectedRows();
                listMediMateAdo.ForEach(o => o.Check = false);

                foreach (int rowhandler in selectedIndexs)
                {
                    HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO ado = (HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO)gridViewExpMestDetail.GetRow(rowhandler);
                    if (ado != null)
                    {
                        ado.Check = true;
                    }
                }
                gridControlExpMestDetail.RefreshDataSource();
                this.SetTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SearchExpMestBill();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO row = (HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO)gridViewExpMestDetail.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.RowHandle != gridViewExpMestDetail.FocusedRowHandle)
                    {
                        e.Appearance.BackColor = Color.White;
                    }
                    if (row.Check)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void cboCashierRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCashierRoom.EditValue != cboCashierRoom.OldEditValue)
                {
                    WaitingManager.Show();
                    LoadDataToComboAccountBook();
                    SetDefaultAccountBook();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GeneratePopupMenu()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MEDICINE_SALE_BILL__BTN_DROP_DOWN__ITEM_PHIEU_XUAT_BAN", Base.ResourceLangManager.LanguagefrmMedicineSaleBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInPhieuXuatBan)));

                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MEDICINE_SALE_BILL__BTN_DROP_DOWN__ITEM_HOA_DON_XUAT_BAN", Base.ResourceLangManager.LanguagefrmMedicineSaleBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInHoaDonDienTu)));

                menu.Items.Add(new DXMenuItem("In hóa đơn điện tử", new EventHandler(onClickInHoaDonDienTu)));

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkOverTime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GlobalVariables.MedicineSaleBill__IsOverTime = checkOverTime.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerTaxCode.Focus();
                    txtBuyerTaxCode.SelectAll();
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerOgranization_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerPhone.Focus();
                    txtBuyerPhone.SelectAll();
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerPhone_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAddress.Focus();
                    txtAddress.SelectAll();
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAddress_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboAccountBook.ShowPopup();
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSaveSign_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.listMediMateAdo == null || this.listMediMateAdo.Count == 0 || this.ExpMests == null || this.ExpMests.Count <= 0)
                {
                    return;
                }

                positionHandle = -1;
                if (lcibtnSaveAndSign.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    return;
                }

                if (!BtnSaveSign.Enabled || !dxValidationProviderEditorInfo.Validate())
                    return;

                if (this.replaceTransactionADOs != null && this.replaceTransactionADOs.Count > 0 && this.cboOriginalTransaction.EditValue == null)
                {
                    if (MessageBox.Show("Tồn tại hóa đơn điện từ có thể thay thế. Bạn có muốn thay thế không?", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        this.cboOriginalTransaction.Focus();
                        return;
                    }
                }

                // Viec 3082 (v3 25/08/2026): tick checkbox "In" -> kiem tra ton -> Luu ky (bill + HDDT)
                // -> tu dong duyet/thuc xuat phan con thieu -> in thang. Khong tick -> luong Luu ky cu.
                bool autoExportPrint = (lciAutoExportPrint.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoExportPrint.Checked);
                ProcessSaveSignPrintCore(autoExportPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Ket qua chuoi Luu ky (+ In) — che do tu dong dua vao day de quyet dinh dong form hay giu mo</summary>
        private enum SaveSignPrintResult
        {
            /// <summary>Tao bill / phat hanh HDDT / duyet-thuc xuat / lay link in that bai — form giu mo de xu ly tiep</summary>
            Failed = 0,
            /// <summary>Da phat hanh HDDT, phieu da hoan thanh (tru kho), da in</summary>
            Success = 1,
            /// <summary>Thieu ton kho — chua luu ky, chua xuat hoa don</summary>
            StockLack = 2,
            /// <summary>Khong tick "In" hoac hinh thuc QR: chay luong Luu ky cu</summary>
            ManualFlow = 3
        }

        /// <summary>
        /// Phan xu ly sau validate cua nut Luu ky. autoExportPrint = true (tick "In"):
        /// (1) kiem tra ton -> (2) SaveProcess(true): tao bill + phat hanh HDDT -> (3) AutoApproveExportExpMests -> (4) PrintInvoiceNow.
        /// Dung ngay tai buoc fail; buoc 1 fail thi chua lam gi ca.
        /// </summary>
        private SaveSignPrintResult ProcessSaveSignPrintCore(bool autoExportPrint)
        {
            SaveSignPrintResult result = SaveSignPrintResult.Failed;
            try
            {
                bool isQrPayForm = (cboPayFrom.EditValue != null && Convert.ToInt64(cboPayFrom.EditValue.ToString()) == 8);
                List<long> selectedExpMestIds = null;
                if (autoExportPrint && !isQrPayForm)
                {
                    // Buoc 1: kiem tra ton kho cac phieu CHUA hoan thanh — thieu thi dung ngay (chua luu ky, chua xuat hoa don)
                    selectedExpMestIds = this.listMediMateAdo.Where(o => o.Check).Select(s => s.EXP_MEST_ID).Distinct().ToList();
                    if (!CheckStockBeforeExport(selectedExpMestIds))
                        return SaveSignPrintResult.StockLack;
                }

                // Buoc 2: tao bill + phat hanh (ky) HDDT — nguyen luong Luu ky cu
                if (this.SaveProcess(true))
                {
                    if (autoExportPrint && !isQrPayForm)
                    {
                        if (this.transactionBillResult != null && !String.IsNullOrEmpty(this.transactionBillResult.INVOICE_CODE))
                        {
                            // Buoc 3: tu dong duyet + thuc xuat phan con thieu (BE co the da tu thuc xuat khi luu phieu hoac khi tao bill)
                            if (AutoApproveExportExpMests(selectedExpMestIds))
                            {
                                // Buoc 4: in thang hoa don dien tu ra may in
                                if (PrintInvoiceNow())
                                {
                                    result = SaveSignPrintResult.Success;
                                }
                            }
                        }
                        else
                        {
                            // Phat hanh HDDT that bai (message da hien trong SaveProcess) -> khong thuc xuat, khong in
                            Inventec.Common.Logging.LogSystem.Warn("ProcessSaveSignPrint(tick In): HDDT chua phat hanh, khong tu dong duyet/thuc xuat/in. "
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => transactionBillResult), transactionBillResult));
                        }
                    }
                    else
                    {
                        result = SaveSignPrintResult.ManualFlow;
                        if (!chkHideHddt.Checked)
                        {
                            if (Convert.ToInt64(cboPayFrom.EditValue.ToString()) != 8)
                            {
                                System.Threading.Thread.Sleep(2000);
                                this.onClickInHoaDonDienTu(null, null);
                            }
                            System.Threading.Thread.Sleep(2000);
                            //this.onClickInHoaDonDienTu(null, null);
                        }
                    }
                }
                if (cboPayFrom.EditValue != null && Convert.ToInt64(cboPayFrom.EditValue.ToString()) == 8)
                {
                    ShowModuleCreQr();
                }
            }
            catch (Exception ex)
            {
                result = SaveSignPrintResult.Failed;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #region Viec 3082 — che do tu dong tu man Xuat ban (Luu in + tick "In")

        /// <summary>Form duoc man Xuat ban mo o che do tu dong (args co List&lt;string&gt; chua AUTO_SAVE_SIGN_PRINT)?</summary>
        private bool IsAutoSaveSignPrint
        {
            get
            {
                return this.autoActions != null && this.autoActions.Contains(Config.AUTO_ACTION__SAVE_SIGN_PRINT);
            }
        }

        private void frmMedicineSaleBill_Shown(object sender, EventArgs e)
        {
            try
            {
                if (!IsAutoSaveSignPrint || isAutoSaveSignPrintStarted)
                    return;
                isAutoSaveSignPrintStarted = true;
                // Cho form ve xong roi moi chay de nguoi dung thay tien trinh
                this.BeginInvoke(new Action(RunAutoSaveSignPrint));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Chuoi tu dong: validate -> kiem tra ton -> Luu ky (bill + HDDT) -> duyet/thuc xuat -> in -> dong form.
        /// Thieu ton: dong form (chua lam gi) de sua phieu tai man Xuat ban. Buoc khac fail: GIU FORM MO de xu ly thu cong.
        /// </summary>
        private void RunAutoSaveSignPrint()
        {
            try
            {
                if (!Config.IsSaveSignPrintAutoExport)
                {
                    Inventec.Common.Logging.LogSystem.Warn("RunAutoSaveSignPrint: key SaveSignPrintAutoExport tat -> giu form thu cong.");
                    return;
                }
                if (lcibtnSaveAndSign.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always || !BtnSaveSign.Enabled)
                {
                    XtraMessageBox.Show("Không thể tự động lưu ký hóa đơn điện tử: chưa cấu hình loại hóa đơn điện tử (HIS.Desktop.ElectronicBill.Type) hoặc nút Lưu ký không khả dụng."
                        + Environment.NewLine + "Vui lòng xử lý thủ công trên form này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (this.ExpMests == null || this.ExpMests.Count == 0 || this.listMediMateAdo == null || this.listMediMateAdo.Count == 0)
                {
                    XtraMessageBox.Show("Không tìm thấy phiếu xuất bán chưa thanh toán để lưu ký hóa đơn điện tử.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cboPayFrom.EditValue != null && Convert.ToInt64(cboPayFrom.EditValue.ToString()) == 8)
                {
                    Inventec.Common.Logging.LogSystem.Info("RunAutoSaveSignPrint: hinh thuc thanh toan QR -> giu luong thu cong.");
                    return;
                }

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                {
                    XtraMessageBox.Show("Chưa đủ thông tin hóa đơn (sổ hóa đơn, hình thức thanh toán, người mua...). Vui lòng bổ sung rồi bấm Lưu ký.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (this.replaceTransactionADOs != null && this.replaceTransactionADOs.Count > 0 && this.cboOriginalTransaction.EditValue == null)
                {
                    if (MessageBox.Show("Tồn tại hóa đơn điện từ có thể thay thế. Bạn có muốn thay thế không?", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        this.cboOriginalTransaction.Focus();
                        return;
                    }
                }

                SaveSignPrintResult rs = ProcessSaveSignPrintCore(true);
                Inventec.Common.Logging.LogSystem.Info("RunAutoSaveSignPrint: ket qua = " + rs.ToString());
                if (rs == SaveSignPrintResult.Success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else if (rs == SaveSignPrintResult.StockLack)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Lay lai trang thai MOI NHAT cua phieu (khong loc HAS_BILL_ID); loi thi dung du lieu da tai khi mo form</summary>
        private List<V_HIS_EXP_MEST> GetExpMestsFresh(List<long> expMestIds)
        {
            List<V_HIS_EXP_MEST> result = null;
            try
            {
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.IDs = expMestIds;
                result = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            if (result == null || result.Count == 0)
            {
                result = (this.ExpMests ?? new List<V_HIS_EXP_MEST>()).Where(o => expMestIds.Contains(o.ID)).ToList();
            }
            return result;
        }

        /// <summary>
        /// Buoc 1 (viec 3082): kiem tra ton kho (AMOUNT theo lo — V_HIS_MEDICINE / V_HIS_MATERIAL) cua cac phieu CHUA hoan thanh.
        /// Phieu da HOAN THANH (kho tu thuc xuat khi luu, hoac da thuc xuat truoc) da tru kho -> bo qua, tranh chan oan.
        /// Thieu -> popup danh sach mat hang thieu, tra ve false (chua luu ky, chua xuat hoa don).
        /// Loi ky thuat khi kiem tra -> tra ve true (de BE Export quyet dinh), co log.
        /// </summary>
        private bool CheckStockBeforeExport(List<long> selectedExpMestIds)
        {
            try
            {
                if (selectedExpMestIds == null || selectedExpMestIds.Count == 0)
                    return true;

                WaitingManager.Show();
                List<V_HIS_EXP_MEST> expMestFresh = GetExpMestsFresh(selectedExpMestIds);
                HashSet<long> expMestIdSet = new HashSet<long>(expMestFresh
                    .Where(o => o.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    .Select(o => o.ID));
                if (expMestIdSet.Count == 0)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Info("CheckStockBeforeExport: tat ca phieu da HOAN THANH (da tru kho) -> bo qua kiem tra ton.");
                    return true;
                }

                System.Text.StringBuilder lackInfo = new System.Text.StringBuilder();
                if (this.listExpMestMedicine != null && this.listExpMestMedicine.Count > 0)
                {
                    var medicineDetails = this.listExpMestMedicine.Where(o => expMestIdSet.Contains(o.EXP_MEST_ID ?? 0) && o.MEDICINE_ID != null).ToList();
                    if (medicineDetails.Count > 0)
                    {
                        var requiredByMedicine = medicineDetails.GroupBy(o => o.MEDICINE_ID.Value)
                            .ToDictionary(g => g.Key, g => g.Sum(s => s.AMOUNT - (s.TH_AMOUNT ?? 0)));
                        HisMedicineViewFilter medicineFilter = new HisMedicineViewFilter();
                        medicineFilter.IDs = requiredByMedicine.Keys.ToList();
                        var medicines = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICINE>>("api/HisMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, null);
                        var medicineDic = (medicines ?? new List<V_HIS_MEDICINE>()).ToDictionary(o => o.ID);
                        foreach (var required in requiredByMedicine)
                        {
                            V_HIS_MEDICINE medicine = null;
                            medicineDic.TryGetValue(required.Key, out medicine);
                            if (medicine == null || medicine.AMOUNT < required.Value)
                            {
                                var detail = medicineDetails.First(o => o.MEDICINE_ID == required.Key);
                                lackInfo.AppendLine(String.Format("- {0} ({1}): cần {2}, tồn {3}",
                                    detail.MEDICINE_TYPE_NAME,
                                    detail.MEDICINE_TYPE_CODE,
                                    Inventec.Common.Number.Convert.NumberToString(required.Value, ConfigApplications.NumberSeperator),
                                    Inventec.Common.Number.Convert.NumberToString(medicine != null ? medicine.AMOUNT : 0, ConfigApplications.NumberSeperator)));
                            }
                        }
                    }
                }

                if (this.listExpMestMaterial != null && this.listExpMestMaterial.Count > 0)
                {
                    var materialDetails = this.listExpMestMaterial.Where(o => expMestIdSet.Contains(o.EXP_MEST_ID ?? 0) && o.MATERIAL_ID != null).ToList();
                    if (materialDetails.Count > 0)
                    {
                        var requiredByMaterial = materialDetails.GroupBy(o => o.MATERIAL_ID.Value)
                            .ToDictionary(g => g.Key, g => g.Sum(s => s.AMOUNT - (s.TH_AMOUNT ?? 0)));
                        HisMaterialViewFilter materialFilter = new HisMaterialViewFilter();
                        materialFilter.IDs = requiredByMaterial.Keys.ToList();
                        var materials = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MATERIAL>>("api/HisMaterial/GetView", ApiConsumers.MosConsumer, materialFilter, null);
                        var materialDic = (materials ?? new List<V_HIS_MATERIAL>()).ToDictionary(o => o.ID);
                        foreach (var required in requiredByMaterial)
                        {
                            V_HIS_MATERIAL material = null;
                            materialDic.TryGetValue(required.Key, out material);
                            if (material == null || material.AMOUNT < required.Value)
                            {
                                var detail = materialDetails.First(o => o.MATERIAL_ID == required.Key);
                                lackInfo.AppendLine(String.Format("- {0} ({1}): cần {2}, tồn {3}",
                                    detail.MATERIAL_TYPE_NAME,
                                    detail.MATERIAL_TYPE_CODE,
                                    Inventec.Common.Number.Convert.NumberToString(required.Value, ConfigApplications.NumberSeperator),
                                    Inventec.Common.Number.Convert.NumberToString(material != null ? material.AMOUNT : 0, ConfigApplications.NumberSeperator)));
                            }
                        }
                    }
                }

                WaitingManager.Hide();

                if (lackInfo.Length > 0)
                {
                    XtraMessageBox.Show("Không đủ tồn kho để thực xuất — chưa lưu ký, chưa xuất hóa đơn. Danh sách mặt hàng thiếu:" + Environment.NewLine + lackInfo.ToString(),
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>
        /// Buoc 3 (viec 3082): tu dong DUYET (Nhap/Yeu cau) va THUC XUAT (tru kho) cac phieu xuat ban CHUA hoan thanh
        /// sau khi phat hanh HDDT thanh cong. Lay lai trang thai moi nhat truoc khi goi vi BE co the da tu duyet/thuc xuat
        /// khi luu phieu (kho tu thuc xuat) hoac ngay khi tao bill (key MOS.TRANSACTION.EXP_MEST_SALE.IS_AUTO_EXPORT).
        /// That bai -> hien ly do tu API + huong dan xu ly thu cong, tra ve false de KHONG in.
        /// </summary>
        private bool AutoApproveExportExpMests(List<long> selectedExpMestIds)
        {
            bool result = false;
            CommonParam param = new CommonParam();
            try
            {
                if (selectedExpMestIds == null || selectedExpMestIds.Count == 0)
                    return false;

                WaitingManager.Show();
                List<V_HIS_EXP_MEST> expMestFresh = GetExpMestsFresh(selectedExpMestIds);
                var expMestPendings = expMestFresh.Where(o => o.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE).ToList();
                if (expMestPendings.Count == 0)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Info("AutoApproveExportExpMests: phieu da HOAN THANH (BE da tu thuc xuat), khong can duyet/thuc xuat them.");
                    return true;
                }

                bool valid = true;
                foreach (var expMest in expMestPendings)
                {
                    long? sttId = expMest.EXP_MEST_STT_ID;
                    // Phieu chua duyet -> duyet truoc khi thuc xuat
                    if (sttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                        || sttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                    {
                        HisExpMestApproveSDO approveSdo = new HisExpMestApproveSDO();
                        approveSdo.ExpMestId = expMest.ID;
                        approveSdo.ReqRoomId = this.roomId;
                        Inventec.Common.Logging.LogSystem.Info("LuuKyAutoExportPrint: Call API api/HisExpMest/Approve"
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => approveSdo), approveSdo));
                        var approveResult = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/Approve", ApiConsumers.MosConsumer, approveSdo, param);
                        if (approveResult == null)
                        {
                            valid = false;
                            break;
                        }
                        // Kho tu thuc xuat: BE co the da xuat luon trong buoc duyet
                        sttId = (approveResult.ExpMest != null) ? approveResult.ExpMest.EXP_MEST_STT_ID : IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                    }

                    if (sttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        continue;

                    HisExpMestExportSDO exportSdo = new HisExpMestExportSDO();
                    exportSdo.ExpMestId = expMest.ID;
                    exportSdo.ReqRoomId = this.roomId;
                    exportSdo.IsFinish = true;
                    Inventec.Common.Logging.LogSystem.Info("LuuKyAutoExportPrint: Call API api/HisExpMest/Export"
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exportSdo), exportSdo));
                    var exportResult = new BackendAdapter(param).Post<HIS_EXP_MEST>(HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumers.MosConsumer, exportSdo, param);
                    if (exportResult == null)
                    {
                        valid = false;
                        break;
                    }
                }
                result = valid;
                WaitingManager.Hide();

                if (!result)
                {
                    string reason = "";
                    if (param.Messages != null && param.Messages.Count > 0)
                    {
                        reason = String.Join(". ", param.Messages.Distinct());
                    }
                    if (param.BugCodes != null && param.BugCodes.Count > 0)
                    {
                        reason += Environment.NewLine + "Mã sự cố: " + String.Join(",", param.BugCodes.Distinct());
                    }
                    XtraMessageBox.Show("Hóa đơn đã phát hành nhưng DUYỆT/THỰC XUẤT TỰ ĐỘNG THẤT BẠI, hệ thống KHÔNG in hóa đơn."
                        + (String.IsNullOrEmpty(reason) ? "" : Environment.NewLine + "Lý do: " + reason)
                        + Environment.NewLine + "Vui lòng vào màn 'Thực xuất thuốc' xử lý thủ công, sau đó in lại hóa đơn bằng nút In > In hóa đơn điện tử.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        #endregion

        private void onClickInHoaDonDienTu(object sender, EventArgs e)
        {
            try
            {
                // Bill chua phat hanh HDDT (INVOICE_CODE rong) -> khong goi GET_INVOICE_LINK,
                // neu goi nha cung cap se tra ve loi "khong tim thay hoa don tuong ung chuoi dua vao".
                if (this.transactionBillResult == null || String.IsNullOrEmpty(this.transactionBillResult.INVOICE_CODE))
                {
                    //MessageBox.Show("Hóa đơn chưa thanh toán hoặc chưa cấu hình hóa đơn điện tử.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => transactionBillResult), transactionBillResult));
                    return;
                }
                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this.transactionBillResult.INVOICE_CODE);
                dataInput.InvoiceCode = transactionBillResult.INVOICE_CODE;
                dataInput.NumOrder = transactionBillResult.NUM_ORDER;
                dataInput.SymbolCode = transactionBillResult.SYMBOL_CODE;
                dataInput.TemplateCode = transactionBillResult.TEMPLATE_CODE;
                dataInput.TransactionTime = transactionBillResult.EINVOICE_TIME ?? transactionBillResult.TRANSACTION_TIME;
                dataInput.EinvoiceTypeId = transactionBillResult.EINVOICE_TYPE_ID;
                dataInput.ENumOrder = transactionBillResult.EINVOICE_NUM_ORDER;

                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionBillResult);
                dataInput.Transaction = tran;

                if (currentTreatment == null)
                {
                    this.currentTreatment = new V_HIS_TREATMENT_FEE();
                    currentTreatment.TDL_PATIENT_ACCOUNT_NUMBER = ExpMests.FirstOrDefault().TDL_PATIENT_ACCOUNT_NUMBER;
                    currentTreatment.TDL_PATIENT_ADDRESS = ExpMests.FirstOrDefault().TDL_PATIENT_ADDRESS;
                    currentTreatment.TDL_PATIENT_PHONE = ExpMests.FirstOrDefault().TDL_PATIENT_PHONE;
                    currentTreatment.TDL_PATIENT_TAX_CODE = ExpMests.FirstOrDefault().TDL_PATIENT_TAX_CODE;
                    currentTreatment.TDL_PATIENT_WORK_PLACE = ExpMests.FirstOrDefault().TDL_PATIENT_WORK_PLACE;
                    currentTreatment.TDL_PATIENT_NAME = ExpMests.FirstOrDefault().TDL_PATIENT_NAME;
                }

                dataInput.Treatment = this.currentTreatment;
                dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
                MOS.Filter.HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.TDL_TREATMENT_ID = transactionBillResult.TREATMENT_ID;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = null;

                electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);

                if (electronicBillResult == null || String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
                {
                    if (electronicBillResult != null && electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                    {
                        MessageBox.Show("Tải hóa đơn điện tử thất bại. " + string.Join(". ", electronicBillResult.Messages));
                    }
                    else
                        MessageBox.Show("Không tìm thấy link hóa đơn điện tử");
                    return;
                }
                Inventec.Common.DocumentViewer.InputADO ado = new InputADO();
                ado.DeleteWhenClose = true;
                ado.URL = electronicBillResult.InvoiceLink;
                ado.NumberOfCopy = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY");
                Inventec.Common.DocumentViewer.DocumentViewerManager viewManager = new Inventec.Common.DocumentViewer.DocumentViewerManager(ViewType.ENUM.Pdf);
                viewManager.Run(ado, HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("Inventec.Common.DocumentViewer.PlatformOption") == 1 ? ViewType.Platform.Telerik : ViewType.Platform.Devexpress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void barBtnSaveSign_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                BtnSaveSign_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Viec 3082: in thang hoa don dien tu ra may in (khong mo man xem):
        /// lay link HDDT (retry toi da 3 lan thay Sleep 2000 cung) roi goi DocumentViewerManager.Print.
        /// Tra ve true khi da gui lenh in; false khi khong lay duoc link (da hien huong dan in lai bang nut In).
        /// </summary>
        private bool PrintInvoiceNow()
        {
            try
            {
                if (this.transactionBillResult == null || String.IsNullOrEmpty(this.transactionBillResult.INVOICE_CODE))
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => transactionBillResult), transactionBillResult));
                    return false;
                }
                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this.transactionBillResult.INVOICE_CODE);
                dataInput.InvoiceCode = transactionBillResult.INVOICE_CODE;
                dataInput.NumOrder = transactionBillResult.NUM_ORDER;
                dataInput.SymbolCode = transactionBillResult.SYMBOL_CODE;
                dataInput.TemplateCode = transactionBillResult.TEMPLATE_CODE;
                dataInput.TransactionTime = transactionBillResult.EINVOICE_TIME ?? transactionBillResult.TRANSACTION_TIME;
                dataInput.EinvoiceTypeId = transactionBillResult.EINVOICE_TYPE_ID;
                dataInput.ENumOrder = transactionBillResult.EINVOICE_NUM_ORDER;

                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionBillResult);
                dataInput.Transaction = tran;

                if (currentTreatment == null)
                {
                    this.currentTreatment = new V_HIS_TREATMENT_FEE();
                    currentTreatment.TDL_PATIENT_ACCOUNT_NUMBER = ExpMests.FirstOrDefault().TDL_PATIENT_ACCOUNT_NUMBER;
                    currentTreatment.TDL_PATIENT_ADDRESS = ExpMests.FirstOrDefault().TDL_PATIENT_ADDRESS;
                    currentTreatment.TDL_PATIENT_PHONE = ExpMests.FirstOrDefault().TDL_PATIENT_PHONE;
                    currentTreatment.TDL_PATIENT_TAX_CODE = ExpMests.FirstOrDefault().TDL_PATIENT_TAX_CODE;
                    currentTreatment.TDL_PATIENT_WORK_PLACE = ExpMests.FirstOrDefault().TDL_PATIENT_WORK_PLACE;
                    currentTreatment.TDL_PATIENT_NAME = ExpMests.FirstOrDefault().TDL_PATIENT_NAME;
                }

                dataInput.Treatment = this.currentTreatment;
                dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = null;

                // Nha cung cap can thoi gian tra link -> retry toi da 3 lan thay cho Sleep(2000) cung
                WaitingManager.Show();
                for (int i = 0; i < 3; i++)
                {
                    electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);
                    if (electronicBillResult != null && !String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
                        break;
                    System.Threading.Thread.Sleep(1000);
                }
                WaitingManager.Hide();

                if (electronicBillResult == null || String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
                {
                    string mes = "";
                    if (electronicBillResult != null && electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                    {
                        mes = " " + string.Join(". ", electronicBillResult.Messages);
                    }
                    XtraMessageBox.Show("Không lấy được link hóa đơn điện tử để in." + mes
                        + Environment.NewLine + "Vui lòng in lại bằng nút In > In hóa đơn điện tử.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                Inventec.Common.DocumentViewer.InputADO ado = new InputADO();
                ado.DeleteWhenClose = true;
                ado.URL = electronicBillResult.InvoiceLink;
                ado.NumberOfCopy = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY");
                Inventec.Common.DocumentViewer.DocumentViewerManager viewManager = new Inventec.Common.DocumentViewer.DocumentViewerManager(ViewType.ENUM.Pdf);
                viewManager.Print(ado, HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("Inventec.Common.DocumentViewer.PlatformOption") == 1 ? ViewType.Platform.Telerik : ViewType.Platform.Devexpress);
                return true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void chkHideHddt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (isNotLoadWhileChangeControlStateInFirst)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkHideHddt.Name && o.MODULE_LINK == module.ModuleLink).FirstOrDefault() : null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (chkHideHddt.Checked ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = chkHideHddt.Name;
                        csAddOrUpdate.VALUE = (chkHideHddt.Checked ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = module.ModuleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoExportPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoExportPrint.Name && o.MODULE_LINK == module.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoExportPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoExportPrint.Name;
                    csAddOrUpdate.VALUE = (chkAutoExportPrint.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = module.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
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
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(module.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkHideHddt.Name)
                        {
                            chkHideHddt.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkAutoExportPrint.Name)
                        {
                            chkAutoExportPrint.Checked = item.VALUE == "1";
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private HIS_CONFIG selectedConfig = new HIS_CONFIG();
        private void btnQR_Click(object sender, EventArgs e)
        {
            try
            {
                if (mediStock != null && !string.IsNullOrEmpty(mediStock.QR_CONFIG_JSON))
                {
                    ItemConfig config = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemConfig>(mediStock.QR_CONFIG_JSON);
                    if (config != null)
                    {
                        List<object> listArgs = new List<object>();
                        TransReqQRADO adoqr = new TransReqQRADO();
                        adoqr.TreatmentId = 0;
                        adoqr.ConfigValue = new HIS_CONFIG() { KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", config.BANK), VALUE = config.VALUE };
                        HIS_TRANSACTION tran = new HIS_TRANSACTION();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionBillResult);
                        adoqr.Transaction = tran;
                        adoqr.IssueInvoice = true;
                        adoqr.NotDisplayedInvoice = (chkHideHddt != null && chkHideHddt.Checked);
                        adoqr.TransReqId = CreateReqType.Transaction;
                        listArgs.Add(adoqr);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", roomId, roomTypeId, listArgs);
                    }
                    else
                    {
                        XtraMessageBox.Show("Định dạng Qr thiết lập trong kho phòng không hợp lệ", "Thông báo");
                    }
                }
                else
                    if (listConfig != null)
                    {
                        if (listConfig.Count > 1)
                        {
                            popupMenu1.ClearLinks();
                            foreach (var item in listConfig)
                            {
                                string key = "";
                                string value = item.KEY;
                                int index = value.IndexOf("Info");
                                if (index > 0)
                                {
                                    var shotkey = value.Substring(0, index);
                                    string[] parts = shotkey.Split('.');
                                    if (parts.Length > 0)
                                    {
                                        key = parts[parts.Length - 1]; // Lấy phần cuối cùng sau khi tách
                                    }
                                }
                                else
                                {
                                    key = item.KEY;
                                }


                                BarButtonItem btnOption = new BarButtonItem(null, key);
                                btnOption.ItemClick += (s, args) =>
                                {

                                    selectedConfig = item;
                                    List<object> listArgs = new List<object>();
                                    HIS.Desktop.ADO.TransReqQRADO adoqr = new TransReqQRADO();
                                    adoqr.TreatmentId = 0;
                                    adoqr.ConfigValue = selectedConfig;
                                    adoqr.IssueInvoice = true;
                                    adoqr.NotDisplayedInvoice = (chkHideHddt != null && chkHideHddt.Checked);
                                    adoqr.TransReqId = CreateReqType.Transaction;
                                    HIS_TRANSACTION tran = new HIS_TRANSACTION();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionBillResult);
                                    adoqr.Transaction = tran;
                                    listArgs.Add(adoqr);
                                    LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR ; KEY: " + selectedConfig.KEY);

                                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", roomId, roomTypeId, listArgs);

                                };
                                popupMenu1.AddItem(btnOption);
                            }
                            popupMenu1.Manager = barManager1;
                            popupMenu1.ShowPopup(Control.MousePosition);
                        }
                        else
                        {
                            selectedConfig = listConfig[0];
                            List<object> listArgs = new List<object>();
                            TransReqQRADO adoqr = new TransReqQRADO();
                            adoqr.TreatmentId = 0;
                            adoqr.ConfigValue = selectedConfig;
                            HIS_TRANSACTION tran = new HIS_TRANSACTION();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionBillResult);
                            adoqr.Transaction = tran;
                            adoqr.IssueInvoice = true;
                            adoqr.NotDisplayedInvoice = (chkHideHddt != null && chkHideHddt.Checked);
                            adoqr.TransReqId = CreateReqType.Transaction;
                            listArgs.Add(adoqr);
                            LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + selectedConfig.KEY);
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", roomId, roomTypeId, listArgs);

                        }
                    }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi thuc hien thanh toan QR tam thu: " + ex);
            }
        }

        private void cboOriginalTransaction_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboOriginalTransaction.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("cboOriginalTransaction_Properties_ButtonClick: " + ex);
            }
        }

        private void rdoCaNhan_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoCaNhan.Checked)
                {
                    //if (selectedRadio != "CaNhan") // Chỉ cập nhật nếu chưa ở trạng thái này
                    {
                        selectedRadio = "CaNhan";
                        rdoCoQuan.Checked = false; // Đảm bảo tắt "Cơ quan"
                        layoutControl1.BeginUpdate();


                        layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem38.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                        layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem29.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem35.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem36.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem37.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                        chkKhac.Checked = false;


                        layoutControlGroup2.BestFit();
                        layoutControl1.EndUpdate();
                        ValidateForm();
                        SetBuyerInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoCoQuan_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoCoQuan.Checked)
            {
                {
                    selectedRadio = "CoQuan";
                    rdoCaNhan.Checked = false;
                    layoutControl1.BeginUpdate();

                    layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem29.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem35.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem36.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem37.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem38.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    layoutControlGroup2.BestFit();
                    layoutControl1.EndUpdate();
                    ValidateForm();
                    SetBuyerInfo();
                }
            }
        }

        private void chkBHYT_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBHYT.Checked)
            {
                var expMest = ExpMests.FirstOrDefault();
                if (expMest != null)
                {
                    var patientTypeAlter = GetPatientTypeAlter(expMest.TDL_PATIENT_ID ?? 0);
                    if (patientTypeAlter != null)
                        txtAddress.Text = patientTypeAlter.ADDRESS;
                }
            }
            else
            {
                var expMest = ExpMests.FirstOrDefault();
                if (expMest != null)
                    txtAddress.Text = expMest.TDL_PATIENT_ADDRESS;
            }
        }

        private void rdoCaNhan_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void rdoCaNhan_Click(object sender, EventArgs e)
        {
            try
            {
                if (rdoCaNhan.Checked)
                {
                    return;
                }
                rdoCaNhan_CheckedChanged(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoCoQuan_Click(object sender, EventArgs e)
        {
            try
            {
                if (rdoCoQuan.Checked)
                {
                    return;
                }
                rdoCoQuan_CheckedChanged(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void cboBuyerOrganization_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBuyerOrganization.EditValue == null) return;
                long id = Convert.ToInt64(cboBuyerOrganization.EditValue);
                var workPlace = GetWorkPlaceById(id);
                if (workPlace != null)
                {
                    // Khi người dùng chọn lại đơn vị: nếu có MST thì luôn load theo đơn vị
                    if (!string.IsNullOrEmpty(workPlace.TAX_CODE))
                    {
                        txtBuyerTaxCode1.Text = workPlace.TAX_CODE;
                    }
                    else
                    {
                        txtBuyerTaxCode1.Text = "";
                    }

                    if (!string.IsNullOrEmpty(workPlace.BUD_REL_UNIT_CODE))
                    {
                        txtBudRelUnitCode.Text = workPlace.BUD_REL_UNIT_CODE;
                    }
                    else
                    {
                        txtBudRelUnitCode.Text = "";
                    }

                    // Nguoi dung chu dong chon don vi: nap Dia chi cua don vi
                    if (!isSettingBuyerInfo && !string.IsNullOrWhiteSpace(workPlace.ADDRESS))
                    {
                        txtAddress.Text = workPlace.ADDRESS;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhac_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkKhac.Checked)
            {
                cboBuyerOrganization.Visible = false;
                txtBuyerOrganization.Visible = true;
                txtBuyerTaxCode1.Text = "";
                txtBudRelUnitCode.Text = "";
            }
            else
            {
                cboBuyerOrganization.Visible = true;
                txtBuyerOrganization.Visible = false;
            }
        }

        private void chkKhac1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkKhac1.Checked)
            {
                cboBuyerOrganization1.Visible = false;
                txtBuyerOrganization1.Visible = true;
                txtBuyerTaxCode.Text = "";
                txtBudRelUnitCode1.Text = "";
            }
            else
            {
                cboBuyerOrganization1.Visible = true;
                txtBuyerOrganization1.Visible = false;
            }
        }

        private void cboBuyerOrganization1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBuyerOrganization1.EditValue == null) return;
                long id = Convert.ToInt64(cboBuyerOrganization1.EditValue);
                var workPlace = GetWorkPlaceById(id);
                if (workPlace != null)
                {
                    if (!string.IsNullOrEmpty(workPlace.TAX_CODE))
                    {
                        txtBuyerTaxCode.Text = workPlace.TAX_CODE;
                    }
                    else
                    {
                        txtBuyerTaxCode.Text = "";
                    }

                    if (!string.IsNullOrEmpty(workPlace.BUD_REL_UNIT_CODE))
                    {
                        txtBudRelUnitCode1.Text = workPlace.BUD_REL_UNIT_CODE;
                    }
                    else
                    {
                        txtBudRelUnitCode1.Text = "";
                    }

                    // Nguoi dung chu dong chon don vi: nap Dia chi cua don vi
                    if (!isSettingBuyerInfo && !string.IsNullOrWhiteSpace(workPlace.ADDRESS))
                    {
                        txtAddress.Text = workPlace.ADDRESS;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayFrom_EditValueChanged(object sender, EventArgs e)
        {
            layoutControlItem27.Enabled = false;

            spinTransferAmount.Enabled = false;

            // Mặc định text/tooltip cho CK
            layoutControlItem27.Text = "Số tiền CK:";
            layoutControlItem27.OptionsToolTip.ToolTip = "Số tiền chuyển khoản";

            // Reset giá trị CK/QT và cảnh báo
            transferAmount = 0;
            spinTransferAmount.EditValue = null;
            UpdateCanThuLabel();

            if (cboPayFrom.EditValue == null)
                return;

            long payFormId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPayFrom.EditValue.ToString());
            HIS_PAY_FORM gt = BackendDataWorker.Get<HIS_PAY_FORM>().SingleOrDefault(o => o.ID == payFormId);
            if (gt == null)
                return;

            // PAY_FROM_CODE = "03" : Tiền mặt/Chuyển khoản
            // PAY_FROM_CODE = "06" : Tiền mặt/Quẹt thẻ
            if (gt.PAY_FORM_CODE == "03")
            {
                dxErrorProvider.SetError(spinTransAmountNew, string.Empty);
                dxErrorProvider.SetError(spinSwipeAmountNew, string.Empty);
                ValidControlTransferAmount(false);
                ValidControlSwipeAmount(false);
                lcTransAmountNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lcSwipeAmountNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem27.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem27.Enabled = true;
                spinTransferAmount.Enabled = true;
                layoutControlItem27.AppearanceItemCaption.ForeColor = Color.Maroon;
                layoutControlItem27.Text = "Số tiền CK:";
                layoutControlItem27.OptionsToolTip.ToolTip = "Số tiền chuyển khoản";
            }
            else if (gt.PAY_FORM_CODE == "06")
            {
                dxErrorProvider.SetError(spinTransAmountNew, string.Empty);
                dxErrorProvider.SetError(spinSwipeAmountNew, string.Empty);
                ValidControlTransferAmount(false);
                ValidControlSwipeAmount(false);
                lcTransAmountNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lcSwipeAmountNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem27.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem27.Enabled = true;
                spinTransferAmount.Enabled = true;
                layoutControlItem27.AppearanceItemCaption.ForeColor = Color.Maroon;
                layoutControlItem27.Text = "Số tiền QT:";
                layoutControlItem27.OptionsToolTip.ToolTip = "Số tiền quẹt thẻ";
            }
            else if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCKQT)
            {
                dxErrorProvider.SetError(spinTransAmountNew, string.Empty);
                dxErrorProvider.SetError(spinSwipeAmountNew, string.Empty);
                lcTransAmountNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                lcSwipeAmountNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem27.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                ValidControlTransferAmount(true);
                ValidControlSwipeAmount(true);
            }

            // Sau khi đổi hình thức thanh toán thì Cần thu = Số tiền - CK/QT (hiện tại CK/QT = 0)
            UpdateCanThuLabel();

        }
        private void UpdateCanThuLabel()
        {
            try
            {
                decimal canThu = totalPrice - transferAmount;
                if (cboPayFrom.EditValue != null && (Inventec.Common.TypeConvert.Parse.ToInt64(cboPayFrom.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCKQT))
                {
                    if (spinSwipeAmountNew.EditValue != null && spinTransAmountNew.EditValue != null)
                    {
                        canThu = totalPrice - spinSwipeAmountNew.Value - spinTransAmountNew.Value;
                    }
                }
                else
                {

                }
                if (canThu < 0)
                    canThu = 0;

                lblCanThu.Text = string.Format(
                    "{0}",
                    Inventec.Common.Number.Convert.NumberToString(canThu, ConfigApplications.NumberSeperator)
                );
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTransferAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                decimal value = 0;
                if (spinTransferAmount.EditValue != null &&
                    decimal.TryParse(spinTransferAmount.EditValue.ToString(), out value))
                {
                    transferAmount = value;
                }
                else
                {
                    transferAmount = 0;
                }

                // Xóa cảnh báo cũ nếu người dùng đang sửa lại số tiền
                dxErrorProvider.SetError(spinTransferAmount, string.Empty);
                UpdateCanThuLabel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTransAmountNew_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                dxErrorProvider.SetError(spinTransAmountNew, string.Empty);
                UpdateCanThuLabel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSwipeAmountNew_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dxErrorProvider.SetError(spinSwipeAmountNew, string.Empty);
                UpdateCanThuLabel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
