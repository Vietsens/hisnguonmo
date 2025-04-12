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
        long? expMestIdForEdit = null;
        long? patientIdForEdit = null;
        List<long> expMestIdForEdits = null;
        List<V_HIS_EXP_MEST> ExpMests;
        List<HIS_PATIENT> Patients;
        MOS.EFMODEL.DataModels.HIS_PATIENT patient = null;
        //V_HIS_PATIENT patient;
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

        public frmMedicineSaleBill()
        {
            InitializeComponent();
        }

        public frmMedicineSaleBill(Inventec.Desktop.Common.Modules.Module module, long expMestId, DelegateSelectData _delegateSelectData)
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
                expMestIdForEdit = expMestId;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmMedicineSaleBill(Inventec.Desktop.Common.Modules.Module module, List<long> expMestIds, DelegateSelectData _delegateSelectData)
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
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmMedicineSaleBill(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.module = module;
                if (this.module != null)
                {
                    this.roomId = module.RoomId;
                    this.roomTypeId = module.RoomTypeId;
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
                    //btnSave.Enabled = true;
                    //GenerateMenuPrint();
                    ValidateForm();
                    SetDefaultAccountBook();
                    SetDefaultPayForm();
                    SetBuyerInfo();
                    LoadTreatmentFee();
                    SetDefaultCreateQR();
                }

                if (expMestIdForEdit > 0 || (expMestIdForEdits != null && expMestIdForEdits.Count > 0))
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

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
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
                ValidControlBuyerAccountCode();
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
                validRule.txtBuyerOrganization = txtBuyerOgranization;
                dxValidationProviderEditorInfo.SetValidationRule(txtBuyerOgranization, validRule);
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

        private void ValidControlBuyerAccountCode()
        {
            try
            {
                BuyerAccountCodeValidationRule validRule = new BuyerAccountCodeValidationRule();
                validRule.txtBuyerAccountCode = txtBuyerAccountCode;
                dxValidationProviderEditorInfo.SetValidationRule(txtBuyerAccountCode, validRule);
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
                if (this.expMestIdForEdit.HasValue && this.expMestIdForEdit.Value > 0)
                {
                    HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                    expMestFilter.ID = this.expMestIdForEdit.Value;
                    expMestFilter.HAS_BILL_ID = false;
                    var listExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, null);
                    if (listExpMest == null || listExpMest.Count != 1)
                    {
                        throw new Exception("Khong lay duoc expMest theo id: " + expMestIdForEdit.Value);
                    }
                    this.ExpMests = listExpMest;
                }
                else if (expMestIdForEdits != null && expMestIdForEdits.Count > 0)
                {
                    HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                    expMestFilter.IDs = this.expMestIdForEdits;
                    expMestFilter.ORDER_FIELD = "MODIFY_TIME";
                    expMestFilter.ORDER_DIRECTION = "DESC";
                    expMestFilter.HAS_BILL_ID = false;
                    var listExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, null);
                    if (listExpMest == null || listExpMest.Count == 0)
                    {
                        throw new Exception("Khong lay duoc expMest theo id: " + expMestIdForEdit.Value);
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
                    HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_IDs = this.ExpMests.Select(s => s.ID).ToList();
                    listExpMestMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, null);

                    HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.EXP_MEST_IDs = this.ExpMests.Select(s => s.ID).ToList();
                    listExpMestMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, null);

                    listMediMateAdo = new List<HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO>();

                    var listExpMestMedicineGroup = listExpMestMedicine.GroupBy(o => new { o.EXP_MEST_ID, o.MEDICINE_ID, o.PRICE, o.VAT_RATIO });
                    foreach (var ExpMestMedicineGroup in listExpMestMedicineGroup)
                    {
                        HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO MediMateTypeADO = new HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO(ExpMestMedicineGroup.ToList(), this.ExpMests.FirstOrDefault(o => o.ID == ExpMestMedicineGroup.Key.EXP_MEST_ID));
                        if (MediMateTypeADO.EXP_AMOUNT > 0)
                            listMediMateAdo.Add(MediMateTypeADO);
                    }

                    var listExpMestMaterialGroup = listExpMestMaterial.GroupBy(o => new { o.EXP_MEST_ID, o.MATERIAL_ID, o.PRICE, o.VAT_RATIO });
                    foreach (var ExpMestMedicineGroup in listExpMestMaterialGroup)
                    {
                        HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO MediMateTypeADO = new HIS.Desktop.Plugins.MedicineSaleBill.ADO.MediMateTypeADO(ExpMestMedicineGroup.ToList(), this.ExpMests.FirstOrDefault(o => o.ID == ExpMestMedicineGroup.Key.EXP_MEST_ID));
                        if (MediMateTypeADO.EXP_AMOUNT > 0)
                            listMediMateAdo.Add(MediMateTypeADO);
                    }
                    if (listMediMateAdo.Count <= 0)
                    {
                        XtraMessageBox.Show("Không có chi tiết thuốc/ vật tư, hoặc thuốc/ vật tư đã bị thu hồi", "Thông báo", DefaultBoolean.True);
                        return;
                    }
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
                decimal totalPrice = 0;
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBuyerInfo()
        {
            
            if (this.ExpMests != null && this.ExpMests.Count > 0)
            {
                
                var expMest = ExpMests.FirstOrDefault();

                txtBuyerAccountCode.Text = ExpMests.FirstOrDefault().TDL_PATIENT_ACCOUNT_NUMBER;
                txtAddress.Text = ExpMests.FirstOrDefault().TDL_PATIENT_ADDRESS;
                txtBuyerPhone.Text = ExpMests.FirstOrDefault().TDL_PATIENT_PHONE;
                txtBuyerTaxCode.Text = ExpMests.FirstOrDefault().TDL_PATIENT_TAX_CODE;
                txtBuyerOgranization.Text = ExpMests.FirstOrDefault().TDL_PATIENT_WORK_PLACE;
                txtName.Text = ExpMests.FirstOrDefault().TDL_PATIENT_NAME;

                long tdlPatientId = expMest.TDL_PATIENT_ID ?? 0;
                HisPatientFilter patientFilter = new HisPatientFilter();
                patientFilter.ID = tdlPatientId;
                var listPatient = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get",
                    ApiConsumers.MosConsumer, patientFilter, new CommonParam());
                if (listPatient != null && listPatient.Count > 0)
                {
                    HIS_PATIENT HisPatientResult = listPatient.FirstOrDefault();
                    txtEmail.Text = HisPatientResult.EMAIL;
                }
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

                if (!string.IsNullOrEmpty(txtName.Text))
                {
                    data.HisTransaction.BUYER_NAME = txtName.Text;
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

                data.HisTransaction.BUYER_ACCOUNT_NUMBER = txtBuyerAccountCode.Text;
                data.HisTransaction.BUYER_ADDRESS = txtAddress.Text;
                data.HisTransaction.BUYER_NAME = txtName.Text;
                data.HisTransaction.BUYER_EMAIL = txtEmail.Text;
                data.HisTransaction.BUYER_ORGANIZATION = txtBuyerOgranization.Text;
                data.HisTransaction.BUYER_TAX_CODE = txtBuyerTaxCode.Text;
                data.HisTransaction.BUYER_PHONE = txtBuyerPhone.Text;

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

                //if (isLuuKy && InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt)
                //{
                //    //Tao hoa don dien thu ben thu3 
                //    ElectronicBillResult electronicBillResult = TaoHoaDonDienTuBenThu3CungCap(data.HisTransaction, seleteds);
                //    if (electronicBillResult == null || !electronicBillResult.Success)
                //    {
                //        param.Messages.Add("Tạo hóa đơn điện tử thất bại");
                //        MessageManager.Show(this.ParentForm, param, success);
                //        return false;
                //    }

                //    data.HisTransaction.INVOICE_CODE = electronicBillResult.InvoiceCode;
                //    data.HisTransaction.INVOICE_SYS = electronicBillResult.InvoiceSys;
                //    data.HisTransaction.EINVOICE_NUM_ORDER = electronicBillResult.InvoiceNumOrder;
                //}

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

                    if (isLuuKy && InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt)
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
                                btnQR.Enabled = true;
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
                MessageManager.Show(this.ParentForm, param, success);
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

        //private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            bool valid = false;
        //            if (!String.IsNullOrEmpty(txtAccountBookCode.Text))
        //            {
        //                string key = txtAccountBookCode.Text.ToUpper();
        //                var data = ListAccountBook.Where(o => o.ACCOUNT_BOOK_CODE.ToUpper().Contains(key) || o.ACCOUNT_BOOK_NAME.ToUpper().Contains(key)).ToList();
        //                if (data != null && data.Count == 1)
        //                {
        //                    valid = true;
        //                    txtAccountBookCode.Text = data.First().ACCOUNT_BOOK_CODE;
        //                    cboAccountBook.EditValue = data.First().ID;
        //                    if (spinNumOrder.Enabled)
        //                    {
        //                        spinNumOrder.Focus();
        //                        spinNumOrder.SelectAll();
        //                    }
        //                    else
        //                    {
        //                        txtCashierRoomCode.Focus();
        //                        txtCashierRoomCode.SelectAll();
        //                    }
        //                }
        //            }
        //            if (!valid)
        //            {
        //                cboAccountBook.Focus();
        //                cboAccountBook.ShowPopup();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

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

        //private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            bool valid = false;
        //            if (!String.IsNullOrEmpty(txtPayFormCode.Text))
        //            {
        //                string key = txtPayFormCode.Text.ToUpper();
        //                var data = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.PAY_FORM_CODE.ToUpper().Contains(key) ||
        //                    o.PAY_FORM_NAME.ToUpper().Contains(key)).ToList();
        //                if (data != null && data.Count == 1)
        //                {
        //                    valid = true;
        //                    txtPayFormCode.Text = data.First().PAY_FORM_CODE;
        //                    cboPayFrom.EditValue = data.First().ID;
        //                    dtTransactionTime.Focus();
        //                    dtTransactionTime.SelectAll();
        //                }
        //            }
        //            if (!valid)
        //            {
        //                cboPayFrom.Focus();
        //                cboPayFrom.ShowPopup();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

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

        private void txtBuyerOgranization_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtBuyerTaxCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerAccountCode.Focus();
                    txtBuyerAccountCode.SelectAll();
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
                this.expMestIdForEdit = null;
                this.ExpMests = null;
                this.Patients = null;
                //this.patient = null;
                this.transactionBillResult = null;
                this.delegateSelectData = null;
                txtBuyerAccountCode.Text = "";
                txtBuyerOgranization.Text = "";
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
                    SetBuyerInfo();
                    LoadTreatmentFee();
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
                if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                {
                    HisExpMestViewFilter filter = new HisExpMestViewFilter();
                    filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN;
                    filter.MEDI_STOCK_ID = this.mediStock.ID;
                    filter.HAS_BILL_ID = false;
                    filter.IS_NOT_TAKEN = false;
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TDL_TREATMENT_CODE__EXACT = code;

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
                else if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    HisExpMestViewFilter filter = new HisExpMestViewFilter();
                    filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN;
                    filter.MEDI_STOCK_ID = this.mediStock.ID;
                    filter.HAS_BILL_ID = false;
                    filter.IS_NOT_TAKEN = false;
                    string code = txtExpMestCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpMestCode.Text = code;
                    }
                    filter.EXP_MEST_CODE__EXACT = code;

                    var listExpMest = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (listExpMest != null && listExpMest.Count == 1)
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
                    if (Config.PrintNowMps == "Mps000339")
                    {

                        this.onClickInHoaDonXuatBan(null, null);
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

                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MEDICINE_SALE_BILL__BTN_DROP_DOWN__ITEM_HOA_DON_XUAT_BAN", Base.ResourceLangManager.LanguagefrmMedicineSaleBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInHoaDonXuatBan)));

                menu.Items.Add(new DXMenuItem("In hóa đơn điện tử", new EventHandler(onClickInHoaDonDienTu)));

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInHoaDonXuatBan(object sender, EventArgs e)
        {
            try
            {
                if (this.transactionBillResult == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000339", deletePrintTemplate);
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

        private void txtBuyerTaxCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerAccountCode.Focus();
                    txtBuyerAccountCode.SelectAll();
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerAccountCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerOgranization.Focus();
                    txtBuyerOgranization.SelectAll();
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

                if (this.SaveProcess(true))
                {
                    if (!chkHideHddt.Checked)
                    {
                        //Nothing
                        System.Threading.Thread.Sleep(2000);
                        this.onClickInHoaDonDienTu(null, null);
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

        private void onClickInHoaDonDienTu(object sender, EventArgs e)
        {
            try
            {
                if (this.transactionBillResult == null && String.IsNullOrEmpty(this.transactionBillResult.INVOICE_CODE))
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
                List<V_HIS_TRANSACTION> vtran = new List<V_HIS_TRANSACTION>();
                vtran.Add(transactionBillResult);
                dataInput.ListTransaction = vtran;
                //V_HIS_TREATMENT_2 treatment2 = new V_HIS_TREATMENT_2();
                //AutoMapper.Mapper.CreateMap<V_HIS_TREATMENT_FEE, V_HIS_TREATMENT_2>();
                //treatment2 = AutoMapper.Mapper.Map<V_HIS_TREATMENT_2>(this.currentTreatment);
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
                //if (TransactionBillConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceHIS)
                //{
                electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);
                //}
                //else
                //{
                //    electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.downloadInvPDFFkeyNoPay);
                //}

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
    }
}
