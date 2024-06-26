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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.MedicineVaccinBill.Base;
using HIS.Desktop.Plugins.MedicineVaccinBill.Validation;
using HIS.Desktop.Print;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMaterialGrid.ADO;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.DocumentViewer;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicineVaccinBill
{
    public partial class frmMedicineVaccinBill : HIS.Desktop.Utility.FormBase
    {

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        private const string HFS_KEY__PAY_FORM_CODE = "HFS_KEY__PAY_FORM_CODE";

        int positionHandle = -1;
        List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_CASHIER_ROOM> cashierRoom = new List<V_HIS_CASHIER_ROOM>();

        DelegateSelectData delegateSelectData;

        V_HIS_VACCINATION _Vaccination;

        V_HIS_PATIENT _vHisPatient;

        V_HIS_TRANSACTION _Transaction;

        long? _roomThuNganId = null;
        HIS_VACCINATION _VaccinationRegister;
        string _patientCodeForSearch = null;
        bool isInit = true;

        int E_BILL__PRINT_NUM_COPY;
        int PlatformOption;
        string InvoiceTypeCreate;
        const string invoiceTypeCreate__CreateInvoiceVnpt = "1";
        const string invoiceTypeCreate__CreateInvoiceHIS = "2";

        #region Constructors
        public frmMedicineVaccinBill()
        {
            InitializeComponent();
        }

        public frmMedicineVaccinBill(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmMedicineVaccinBill(Inventec.Desktop.Common.Modules.Module module, string patientCodeForSearch)
            : this(module)
        {
            this._patientCodeForSearch = patientCodeForSearch;
        }

        public frmMedicineVaccinBill(Inventec.Desktop.Common.Modules.Module module, V_HIS_VACCINATION vaccin, DelegateSelectData _delegateSelectData)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this._Vaccination = vaccin;
                delegateSelectData = _delegateSelectData;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmMedicineVaccinBill(Inventec.Desktop.Common.Modules.Module module, V_HIS_VACCINATION vaccin, DelegateSelectData _delegateSelectData, string patientCodeForSearch)
            : this(module, vaccin, _delegateSelectData)
        {
            this._patientCodeForSearch = patientCodeForSearch;
        }

        public frmMedicineVaccinBill(Inventec.Desktop.Common.Modules.Module module, HIS_VACCINATION vaccin, long? thuNganId, DelegateSelectData _delegateSelectData)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this._VaccinationRegister = vaccin;
                this._roomThuNganId = thuNganId;
                delegateSelectData = _delegateSelectData;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmMedicineVaccinBill(Inventec.Desktop.Common.Modules.Module module, HIS_VACCINATION vaccin, long? thuNganId, DelegateSelectData _delegateSelectData, string patientCodeForSearch)
            : this(module, vaccin, thuNganId, _delegateSelectData)
        {
            this._patientCodeForSearch = patientCodeForSearch;
        }
        #endregion

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

        private void frmMedicineVaccinBill_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                this.InitControlState();

                InvoiceTypeCreate = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.ElectronicBill.Type");
                if (String.IsNullOrEmpty(InvoiceTypeCreate) || (InvoiceTypeCreate != invoiceTypeCreate__CreateInvoiceVnpt && InvoiceTypeCreate != invoiceTypeCreate__CreateInvoiceHIS))
                {
                    lciSaveAndSign.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                E_BILL__PRINT_NUM_COPY = ConfigApplicationWorker.Get<int>("CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY");
                PlatformOption = HisConfigs.Get<int>("Inventec.Common.DocumentViewer.PlatformOption");

                LoadData();

                LoadDataToGridExpMestMedicine();

                SetTotalPrice();

                dtTransactionTime.EditValue = DateTime.Now;

                LoadDataToComboCashierRoom();

                SetDafaultCashierRoom();

                LoadDataToComboAccountBook();

                LoadDataToComboPayForm();

                ValidateForm();

                SetDefaultAccountBook();

                SetDefaultPayForm();

                SetBuyerInfo();

                this.EnableButtonSave(true);

                WaitingManager.Hide();
                if (!String.IsNullOrEmpty(this._patientCodeForSearch))
                {
                    txtFindPatientCode.Text = this._patientCodeForSearch;
                    this.SearchData();
                }
                this.isInit = false;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.isInit = false;
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_PRINT_EINVOICE)
                        {
                            chkPrintEInvoice.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableButtonSave(bool enable)
        {
            try
            {

                ddBtnPrint.Enabled = !enable;
                btnSave.Enabled = enable;
                btnSavePrint.Enabled = enable;
                btnSaveAndSign.Enabled = enable;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                if (_VaccinationRegister != null && _VaccinationRegister.ID > 0)
                {
                    HisVaccinationFilter filter = new HisVaccinationFilter();
                    filter.ID = _VaccinationRegister.ID;

                    var lstData = new BackendAdapter(new CommonParam()).Get<List<V_HIS_VACCINATION>>("api/HisVaccination/GetView", ApiConsumers.MosConsumer, filter, null);

                    this._Vaccination = lstData != null ? lstData.FirstOrDefault() : null;
                }

                if (this._Vaccination != null)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = this._Vaccination.PATIENT_ID;

                    var lstData = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, null);

                    this._vHisPatient = lstData != null ? lstData.FirstOrDefault() : null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetData()
        {
            try
            {
                this._VaccinationRegister = null;
                this._Vaccination = null;
                this._vHisPatient = null;
                this._Transaction = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridExpMestMedicine()
        {
            try
            {
                List<V_HIS_EXP_MEST_MEDICINE> listData = new List<V_HIS_EXP_MEST_MEDICINE>();
                if (this._Vaccination != null)
                {
                    CommonParam param = new CommonParam();
                    HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                    medicineFilter.TDL_VACCINATION_ID = this._Vaccination.ID;

                    listData = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param);

                }

                gridControlData.BeginUpdate();
                gridControlData.DataSource = listData;
                gridControlData.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDafaultCashierRoom()
        {
            try
            {
                if (this._roomThuNganId != null && this._roomThuNganId > 0)
                {
                    var data = cashierRoom.FirstOrDefault(p => p.ID == this._roomThuNganId);
                    if (data != null)
                    {
                        txtCashierRoomCode.Text = data.CASHIER_ROOM_CODE;
                        cboCashierRoom.EditValue = data.ID;
                    }
                }
                else
                {
                    var cashier = cashierRoom.FirstOrDefault(p => p.ROOM_ID == this.currentModuleBase.RoomId);
                    if (cashier != null)
                    {
                        txtCashierRoomCode.Text = cashier.CASHIER_ROOM_CODE;
                        cboCashierRoom.EditValue = cashier.ID;
                    }
                }

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
                    var data = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.PAY_FORM_CODE == code && o.IS_ACTIVE == 1);
                    if (data != null)
                    {
                        txtPayFormCode.Text = data.PAY_FORM_CODE;
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
                if (cboAccountBook.EditValue == null || !ListAccountBook.Any(a => a.ID == Convert.ToInt64(cboAccountBook.EditValue)))
                {
                    var data = ListAccountBook.FirstOrDefault();
                    if (data != null)
                    {
                        txtAccountBookCode.Text = data.ACCOUNT_BOOK_CODE;
                        cboAccountBook.EditValue = data.ID;
                    }
                }
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
                if (this._Transaction == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000334", deletePrintTemplate);
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
                        case "Mps000334":
                            InPhieuVacXin(ref result, printTypeCode, fileName);
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

        private void InPhieuVacXin(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                var dataMedicines = (List<V_HIS_EXP_MEST_MEDICINE>)gridViewData.DataSource;

                MPS.Processor.Mps000334.PDO.Mps000334PDO rdo = new MPS.Processor.Mps000334.PDO.Mps000334PDO(
                    this._Vaccination,
                    this._Transaction,
                    dataMedicines);
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
                ValidationSingleControl(txtPayFormCode);
                ValidationSingleControl(txtAccountBookCode);
                ValidationSingleControl(dtTransactionTime);
                ValidControlBuyerOrganization();
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
                var datapay = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPayFrom, datapay, controlEditorADO);

              
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
                    HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                    acFilter.IDs = ids;
                    acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    acFilter.FOR_BILL = true;
                    acFilter.IS_OUT_OF_BILL = false;
                    acFilter.ORDER_DIRECTION = "DESC";
                    acFilter.ORDER_FIELD = "ID";
                    ListAccountBook = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
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

        private void SetTotalPrice()
        {
            try
            {
                decimal totalPrice = 0;
                decimal discount = 0;
                var datas = (List<V_HIS_EXP_MEST_MEDICINE>)gridViewData.DataSource;
                if (datas != null && datas.Count > 0)
                {
                    totalPrice = datas.Sum(o => ((o.VIR_PRICE ?? 0) - (o.DISCOUNT ?? 0)));
                    discount = datas.Sum(o => o.DISCOUNT ?? 0);
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
            try
            {
                if (this._vHisPatient != null)
                {
                    lblPatientCode.Text = this._vHisPatient.PATIENT_CODE ?? "";
                    lblPatientName.Text = this._vHisPatient.VIR_PATIENT_NAME ?? "";
                    lblGender.Text = this._vHisPatient.GENDER_NAME ?? "";

                    if (this._vHisPatient.IS_HAS_NOT_DAY_DOB == (short)1)
                    {
                        lblDob.Text = this._vHisPatient.DOB.ToString().Substring(0, 4) ?? "";
                    }
                    else
                    {
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._vHisPatient.DOB) ?? "";
                    }

                    txtBuyerAccountCode.Text = _vHisPatient.ACCOUNT_NUMBER;
                    txtBuyerTaxCode.Text = _vHisPatient.TAX_CODE;
                    txtBuyerOgranization.Text = String.IsNullOrWhiteSpace(_vHisPatient.WORK_PLACE_NAME) ? _vHisPatient.WORK_PLACE_NAME : (_vHisPatient.WORK_PLACE ?? "");
                    txtBuyerPhone.Text = _vHisPatient.MOBILE != null ? _vHisPatient.MOBILE : (_vHisPatient.PHONE ?? "");
                }
                else
                {
                    txtBuyerAccountCode.Text = "";
                    txtBuyerTaxCode.Text = "";
                    txtBuyerOgranization.Text = "";
                    lblPatientCode.Text = "";
                    lblPatientName.Text = "";
                    lblGender.Text = "";
                    lblDob.Text = "";
                }
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
                positionHandle = -1;
                if (!btnSave.Enabled || !dxValidationProviderEditorInfo.Validate())
                    return;
                this.SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool SaveProcess([Optional]bool isLuuKy)
        {
            bool result = false;
            try
            {
                var datas = (List<V_HIS_EXP_MEST_MEDICINE>)gridViewData.DataSource;
                if (datas == null || datas.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return false;
                }

                if (this._Vaccination.BILL_ID.HasValue)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Phiếu xuất đã thanh toán", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return false;
                }
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                HisTransactionBillVaccinSDO data = new HisTransactionBillVaccinSDO();
                data.HisBillGoods = new List<HIS_BILL_GOODS>();
                data.HisTransaction = new HIS_TRANSACTION();
                data.VaccinationIds = new List<long>();
                data.VaccinationIds.Add(this._Vaccination.ID);
                if (txtDescription.Text != null)
                {
                    data.HisTransaction.DESCRIPTION = txtDescription.Text;
                }

                if (cboPayFrom.EditValue != null)
                {
                    HIS_PAY_FORM gt = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayFrom.EditValue));
                    if (gt != null)
                    {
                        data.HisTransaction.PAY_FORM_ID = gt.ID;
                    }
                }
                if (cboAccountBook.EditValue != null)
                {
                    V_HIS_ACCOUNT_BOOK gt = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
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
                    V_HIS_CASHIER_ROOM gt = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboCashierRoom.EditValue));
                    if (gt != null)
                    {
                        data.HisTransaction.CASHIER_ROOM_ID = gt.ID;
                    }
                }
                if (dtTransactionTime.DateTime != null)
                {
                    data.HisTransaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }

                data.HisTransaction.BUYER_ACCOUNT_NUMBER = txtBuyerAccountCode.Text;
                data.HisTransaction.BUYER_ADDRESS = _vHisPatient.VIR_ADDRESS;
                data.HisTransaction.BUYER_NAME = _vHisPatient.VIR_PATIENT_NAME;
                data.HisTransaction.BUYER_ORGANIZATION = txtBuyerOgranization.Text;
                data.HisTransaction.BUYER_TAX_CODE = txtBuyerTaxCode.Text;
                data.HisTransaction.BUYER_PHONE = txtBuyerPhone.Text;

                List<HIS_BILL_GOODS> billGooDs = new List<HIS_BILL_GOODS>();

                if (datas != null && datas.Count > 0)
                {
                    foreach (var expMedicineGroup in datas)
                    {
                        HIS_BILL_GOODS billGoood = new HIS_BILL_GOODS();
                        billGoood.AMOUNT = expMedicineGroup.AMOUNT;
                        billGoood.PRICE = (expMedicineGroup.PRICE ?? 0) * (1 + expMedicineGroup.VAT_RATIO ?? 0); ;
                        billGoood.GOODS_NAME = expMedicineGroup.MEDICINE_TYPE_NAME;
                        billGoood.DESCRIPTION = expMedicineGroup.DESCRIPTION;
                        billGoood.GOODS_UNIT_NAME = expMedicineGroup.SERVICE_UNIT_NAME;
                        billGoood.DISCOUNT = expMedicineGroup.DISCOUNT;
                        billGooDs.Add(billGoood);
                    }
                    data.HisBillGoods = billGooDs;
                }

                _Transaction = new BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/CreateBillVaccin", ApiConsumers.MosConsumer, data, param);
                if (_Transaction != null)
                {
                    result = true;
                    success = true;
                    EnableButtonSave(false);
                    if (delegateSelectData != null)
                    {
                        delegateSelectData(_Transaction);
                    }
                    if (isLuuKy && InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt)
                    {
                        HIS_TRANSACTION tran = new HIS_TRANSACTION();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, _Transaction);
                        //Tao hoa don dien thu ben thu3 
                        ElectronicBillResult electronicBillResult = TaoHoaDonDienTuBenThu3CungCap(tran, datas);
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
                            sdo.Id = _Transaction.ID;
                            sdo.InvoiceLookupCode = electronicBillResult.InvoiceLookupCode;
                            var apiResult = new BackendAdapter(paramUpdate).Post<bool>("api/HisTransaction/UpdateInvoiceInfo", ApiConsumers.MosConsumer, sdo, paramUpdate);
                            {
                                _Transaction.INVOICE_CODE = electronicBillResult.InvoiceCode;
                                _Transaction.INVOICE_SYS = electronicBillResult.InvoiceSys;
                                _Transaction.EINVOICE_NUM_ORDER = electronicBillResult.InvoiceNumOrder;
                                _Transaction.EINVOICE_TIME = electronicBillResult.InvoiceTime;
                                _Transaction.EINVOICE_LOGINNAME = electronicBillResult.InvoiceLoginname;
                                _Transaction.INVOICE_LOOKUP_CODE = electronicBillResult.InvoiceLookupCode;
                                result = true;
                                success = true;
                                if (delegateSelectData != null)
                                {
                                    delegateSelectData(this._Transaction);
                                }
                            }
                        }
                    }
                }
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

        private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtAccountBookCode.Text))
                    {
                        string key = txtAccountBookCode.Text.ToUpper();
                        var data = ListAccountBook.Where(o => o.ACCOUNT_BOOK_CODE.ToUpper().Contains(key) || o.ACCOUNT_BOOK_NAME.ToUpper().Contains(key)).ToList();
                        if (data != null && data.Count == 1)
                        {
                            valid = true;
                            txtAccountBookCode.Text = data.First().ACCOUNT_BOOK_CODE;
                            cboAccountBook.EditValue = data.First().ID;
                            if (spinNumOrder.Enabled)
                            {
                                spinNumOrder.Focus();
                                spinNumOrder.SelectAll();
                            }
                            else
                            {
                                txtCashierRoomCode.Focus();
                                txtCashierRoomCode.SelectAll();
                            }
                        }
                    }
                    if (!valid)
                    {
                        cboAccountBook.Focus();
                        cboAccountBook.ShowPopup();
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
                            txtPayFormCode.Focus();
                            txtPayFormCode.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboCashierRoom.Focus();
                        cboCashierRoom.ShowPopup();
                    }
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
                            txtPayFormCode.Focus();
                        }
                    }
                    else
                    {
                        txtPayFormCode.Focus();
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
                            txtPayFormCode.Focus();
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

        private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtPayFormCode.Text))
                    {
                        string key = txtPayFormCode.Text.ToUpper();
                        var data = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.PAY_FORM_CODE.ToUpper().Contains(key) ||
                            o.PAY_FORM_NAME.ToUpper().Contains(key)).ToList();
                        if (data != null && data.Count == 1)
                        {
                            valid = true;
                            txtPayFormCode.Text = data.First().PAY_FORM_CODE;
                            cboPayFrom.EditValue = data.First().ID;
                            dtTransactionTime.Focus();
                            dtTransactionTime.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboPayFrom.Focus();
                        cboPayFrom.ShowPopup();
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
                            txtPayFormCode.Text = gt.PAY_FORM_CODE;
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
                txtAccountBookCode.Text = "";
                spinNumOrder.Enabled = false;
                if (cboAccountBook.EditValue != null)
                {
                    V_HIS_ACCOUNT_BOOK gt = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    if (gt != null)
                    {
                        txtAccountBookCode.Text = gt.ACCOUNT_BOOK_CODE;
                        spinNumOrder.Value = gt.CURRENT_NUM_ORDER.HasValue ? (gt.CURRENT_NUM_ORDER.Value + 1) : gt.FROM_NUM_ORDER;
                        if (gt.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spinNumOrder.Enabled = true;
                        }
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

        private void ddBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ddBtnPrint.Enabled || this._Vaccination == null) return;
                this.onClickInPhieuXuatBan(null, null);
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
                //if (this.listMediMateAdo == null || this.listMediMateAdo.Count == 0 || this.ExpMests == null || this.ExpMests.Count <= 0)
                //{
                //    return;
                //}
                positionHandle = -1;
                if (!btnSavePrint.Enabled || !dxValidationProviderEditorInfo.Validate())
                    return;
                if (this.SaveProcess())
                {
                    this.onClickInPhieuXuatBan(null, null);
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
                txtFindPatientCode.Focus();
                txtFindPatientCode.SelectAll();
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

        private void gridViewData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {

                    var data = (V_HIS_EXP_MEST_MEDICINE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "VAT_RATIO_STR")
                        {
                            e.Value = (data.VAT_RATIO ?? 0) * 100;
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

        private void txtFindPatientCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter
                    && !String.IsNullOrWhiteSpace(txtFindPatientCode.Text))
                {
                    this.SearchData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFindVaccinationCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter
                    && !String.IsNullOrWhiteSpace(txtFindVaccinationCode.Text))
                {
                    this.SearchData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled) return;
                SearchData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintEInvoice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_PRINT_EINVOICE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintEInvoice.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_PRINT_EINVOICE;
                    csAddOrUpdate.VALUE = (chkPrintEInvoice.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveAndSign_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (lciSaveAndSign.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always ||
                    !btnSavePrint.Enabled ||
                    !dxValidationProviderEditorInfo.Validate())
                    return;
                if (this.SaveProcess(true) && chkPrintEInvoice.Checked)
                {
                    System.Threading.Thread.Sleep(2000);
                    this.onClickInHoaDonDienTu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void onClickInHoaDonDienTu()
        {
            try
            {
                if (this._Transaction == null || String.IsNullOrEmpty(this._Transaction.INVOICE_CODE))
                {
                    return;
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this._Transaction.INVOICE_CODE);
                dataInput.InvoiceCode = this._Transaction.INVOICE_CODE;
                dataInput.NumOrder = this._Transaction.NUM_ORDER;
                dataInput.SymbolCode = this._Transaction.SYMBOL_CODE;
                dataInput.TemplateCode = this._Transaction.TEMPLATE_CODE;
                dataInput.TransactionTime = this._Transaction.EINVOICE_TIME ?? this._Transaction.TRANSACTION_TIME;
                dataInput.ENumOrder = this._Transaction.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = this._Transaction.EINVOICE_TYPE_ID;
                //dataInput.Treatment = this.currentTreatment;
                dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = null;

                electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);

                if (electronicBillResult == null || String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
                {
                    MessageBox.Show("Không tìm thấy link hóa đơn điện tử");
                    return;
                }

                DocumentViewerManager viewManager = new DocumentViewerManager(ViewType.ENUM.Pdf);
                InputADO ado = new InputADO();
                ado.DeleteWhenClose = true;
                ado.NumberOfCopy = E_BILL__PRINT_NUM_COPY;
                ado.URL = electronicBillResult.InvoiceLink;
                ViewType.Platform type = ViewType.Platform.Telerik;
                if (PlatformOption > 0)
                {
                    type = (ViewType.Platform)(PlatformOption - 1);
                }

                viewManager.Print(ado, type);
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
                WaitingManager.Show();
                txtFindPatientCode.Text = "";
                txtFindVaccinationCode.Text = "";
                ResetData();
                LoadData();
                LoadDataToGridExpMestMedicine();
                this.SetBuyerInfo();
                SetTotalPrice();
                txtFindPatientCode.Focus();
                txtFindPatientCode.SelectAll();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchData()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(txtFindPatientCode.Text) && String.IsNullOrWhiteSpace(txtFindVaccinationCode.Text))
                {
                    return;
                }
                WaitingManager.Show();
                this.ResetData();
                this.EnableButtonSave(true);
                this.SetBuyerInfo();
                this.LoadDataToGridExpMestMedicine();
                this.SetTotalPrice();

                HisVaccinationViewFilter filter = new HisVaccinationViewFilter();
                filter.HAS_BILL = false;
                if (!String.IsNullOrWhiteSpace(txtFindVaccinationCode.Text))
                {
                    string code = txtFindVaccinationCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtFindVaccinationCode.Text = code;
                    }

                    filter.VACCINATION_CODE__EXACT = code;
                }
                else
                {
                    string code = txtFindPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtFindPatientCode.Text = code;
                    }

                    filter.TDL_PATIENT_CODE__EXACT = code;
                }

                List<V_HIS_VACCINATION> lstData = new BackendAdapter(new CommonParam()).Get<List<V_HIS_VACCINATION>>("api/HisVaccination/GetView", ApiConsumers.MosConsumer, filter, null);

                this._Vaccination = lstData != null ? lstData.FirstOrDefault() : null;

                if (this._Vaccination != null)
                {
                    this.LoadData();
                    this.LoadDataToGridExpMestMedicine();
                    this.SetBuyerInfo();
                    this.SetTotalPrice();
                }
                WaitingManager.Hide();
                if (this._Vaccination == null)
                {
                    XtraMessageBox.Show("Không tìm thấy chỉ định tiêm hoặc chỉ định tiêm đã được thanh toán", "Thông báo", DefaultBoolean.True);
                    txtFindPatientCode.Focus();
                    txtFindPatientCode.SelectAll();
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap(HIS_TRANSACTION transaction, List<V_HIS_EXP_MEST_MEDICINE> seleteds)
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

                    sereServBill.AMOUNT = item.AMOUNT;
                    sereServBill.VAT_RATIO = item.VAT_RATIO ?? 0;
                    sereServBill.TDL_SERVICE_CODE = item.MEDICINE_TYPE_CODE;
                    sereServBill.TDL_SERVICE_NAME = item.MEDICINE_TYPE_NAME;
                    sereServBill.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                    sereServBill.DISCOUNT = item.DISCOUNT;
                    sereServBill.PRICE = item.VIR_PRICE ?? 0;
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

                dataInput.Treatment = new V_HIS_TREATMENT_FEE();
                dataInput.Treatment.TDL_PATIENT_ACCOUNT_NUMBER = transaction.BUYER_ACCOUNT_NUMBER ?? _vHisPatient.ACCOUNT_NUMBER;
                dataInput.Treatment.TDL_PATIENT_ADDRESS = transaction.BUYER_ADDRESS ?? _Vaccination.TDL_PATIENT_ADDRESS;
                dataInput.Treatment.TDL_PATIENT_PHONE = transaction.BUYER_PHONE ?? _vHisPatient.PHONE;
                dataInput.Treatment.TDL_PATIENT_TAX_CODE = transaction.BUYER_TAX_CODE ?? _vHisPatient.TAX_CODE;
                dataInput.Treatment.TDL_PATIENT_WORK_PLACE = transaction.BUYER_ORGANIZATION ?? _Vaccination.TDL_PATIENT_WORK_PLACE;
                dataInput.Treatment.TDL_PATIENT_NAME = transaction.BUYER_NAME ?? _Vaccination.TDL_PATIENT_NAME;
                dataInput.Treatment.ID = -1;//để các api trong thư viện không lấy được dữ liệu
                dataInput.Treatment.PATIENT_ID = _Vaccination.PATIENT_ID;

                dataInput.Currency = "VND";
                dataInput.Transaction = transaction;
                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    dataInput.SymbolCode = accountBook.SYMBOL_CODE;
                    dataInput.TemplateCode = accountBook.TEMPLATE_CODE;
                    dataInput.EinvoiceTypeId = accountBook.EINVOICE_TYPE_ID;
                }

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

        private void barBtnSaveAndSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndSign_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
