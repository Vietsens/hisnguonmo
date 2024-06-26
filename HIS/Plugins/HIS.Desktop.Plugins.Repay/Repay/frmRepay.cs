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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Repay.ADO;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ADO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Repay.Repay
{
    public partial class frmRepay : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        MOS.EFMODEL.DataModels.V_HIS_TREATMENT HisTreatment { get; set; }
        private decimal totalDiscount { get; set; }
        HisRepaySDO HisRepaySDO { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_REPAY HisRepay { get; set; }
        internal int ActionType = 0;// No action
        internal HideCheckBoxHelper hideCheckBoxHelper;
        int positionHandle = -1;
        decimal totalPatientPriceExemtion = 0;
        decimal totalAmountDeposit = 0;

        internal List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> ListPayForm;
        internal List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK> ListAccountBookFormBill;
        internal List<V_HIS_DERE_DETAIL> ListDereDetail;
        internal List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> ListHisPatientType;
        internal string HIS_PAY_FORM_CODE_DEFAULT;
        internal string AccountBookCodeForUser;
        internal string PayFormCodeDefault;

        SendResultToOtherForm sendResultToOtherForm;
        #endregion

        #region Construct
        public frmRepay(int action)
        {
            try
            {
                InitializeComponent();
                this.ActionType = action;
                RepayProcess.EnableButton(action, this);
                treeSereServ.ShowFindPanel();
                ValidControls();
                LoadKeysFromlanguage();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmRepay(MOS.EFMODEL.DataModels.V_HIS_TREATMENT hisTreatment, HisRepaySDO transactionData, int action, SendResultToOtherForm _sendResultToOtherForm)
        {
            try
            {
                InitializeComponent();
                this.HisTreatment = hisTreatment;
                this.HisRepaySDO = transactionData;
                this.ActionType = action;
                RepayProcess.EnableButton(action, this);
                treeSereServ.ShowFindPanel();
                ValidControls();
                LoadKeysFromlanguage();
                sendResultToOtherForm = _sendResultToOtherForm;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmBill_Load(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                positionHandle = -1;
                ValidControls();
                loadDataToControl();
                InitMenuToButtonPrint();
                if (HisTreatment != null)
                {
                    RepayProcess.FillDataToControl(HisTreatment, HisRepay, this);
                }
                if (HisRepaySDO == null)
                {
                    HisRepaySDO = new MOS.SDO.HisRepaySDO();
                    HisRepaySDO.Repay = new HIS_REPAY();
                    HisRepaySDO.Transaction = new HIS_TRANSACTION();
                    HisRepaySDO.DereDetailIds = new List<long>();
                    LoadDataTransaction();
                }
                RepayProcess.FillDataToSereServTree(HisTreatment, this);
                SetDefaultAccountBookForUser();
                SetDefaultPayFormForUser();
                ChangeCheckChildNodes(true);
                txtAccountBookCode.Focus();
                txtAccountBookCode.SelectAll();
                totalPatientPriceExemtion = CalculatePatientPrice();
                lblNumberOrder.Enabled = false;
                UpdateTotalPriceToControl();
                //CalculateTotalDiscount();
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
                ValidControls();
                txtAccountBookCode.Focus();
                txtAccountBookCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region private function        

        private void refreshDataAfterSave()
        {
            if (this.sendResultToOtherForm != null)
            {
                this.sendResultToOtherForm(this.HisRepay);
            }
        }

        private void loadDataToControl()
        {
            loadSereServByTreatment();
            loadPayForm();
            loadPatientType();
            loadAccountBook();
            InitComboPayForm();
            InitComboAccountBook();
        }

        private void loadSereServByTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.HisTreatment != null)
                {
                    this.ListDereDetail = new List<V_HIS_DERE_DETAIL>();
                    MOS.Filter.HisDereDetailViewFilter dereDetailFilter = new HisDereDetailViewFilter();
                    dereDetailFilter.TREATMENT_ID = this.HisTreatment.ID;
                    this.ListDereDetail = new BackendAdapter(param).Get<List<V_HIS_DERE_DETAIL>>(HisRequestUriStore.HIS_DERE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, dereDetailFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadPayForm()
        {
            try
            {
                MOS.Filter.HisPayFormFilter Filter = new HisPayFormFilter();
                Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                ListPayForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>(Filter).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadPatientType()
        {
            try
            {
                MOS.Filter.HisPatientTypeFilter Filter = new HisPatientTypeFilter();
                Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                ListHisPatientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(Filter).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadAccountBook()
        {
            try
            {
                MOS.Filter.HisAccountBookViewFilter Filter = new HisAccountBookViewFilter();
                Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                Filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                ListAccountBookFormBill = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>(Filter).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPayForm()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboPayForm, this.ListPayForm, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void InitComboAccountBook()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboAccountBook, this.ListAccountBookFormBill, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void CalculateTotalDiscount()
        //{
        //    try
        //    {
        //        var servServs = ListDereDetail.Where(o => o.IS_EXPEND != IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && o.REPAY_ID == null && (o.VIR_TOTAL_PATIENT_PRICE != null && o.VIR_TOTAL_PATIENT_PRICE != 0)).ToList();
        //        totalDiscount = servServs.Select(o => o. ?? 0).Sum();

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private decimal CalculatePatientPrice()
        {
            decimal totalPatientPrice = 0;
            try
            {
                CommonParam param = new CommonParam();
                //HisTreatmentFeeViewFilter searchFilter = new HisTreatmentFeeViewFilter();
                //searchFilter.ID = HisHospitalFee.ID;
                ////xemlai...
                //List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> HospitalFeeSum = new TreatmentFeeLogic(param).Get(searchFilter);
                //if (HospitalFeeSum != null && HospitalFeeSum.Count > 0)
                //{
                //totalPatientPrice = ((HospitalFeeSum[0].TOTAL_PATIENT_PRICE ?? 0) - ((HospitalFeeSum[0].TOTAL_DEPOSIT_AMOUNT ?? 0) + (HospitalFeeSum[0].TOTAL_BILL_AMOUNT ?? 0) - (HospitalFeeSum[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (HospitalFeeSum[0].TOTAL_REPAY_AMOUNT ?? 0) - (HospitalFeeSum[0].TOTAL_BILL_EXEMPTION ?? 00)));
                //}
                totalPatientPrice = spinAmount.Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return totalPatientPrice;
        }

        public void LoadKeysFromlanguage()
        {
            try
            {
                //this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_FRM_BILL", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblAccountBook.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_ACCOUNT_BOOK", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblDiscount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_DISCOUNT", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblCashier.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_CASHIER", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblNumberOrder.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_NUMBER_ORDER", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblReson.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_REASON", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblCreateTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_CREATE_TIME", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_AMOUNT", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblPayForm.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_PAY_FORM", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_DESCRIPTION", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_BTN_SAVE_SHORT_CUT", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //toggleSwitchPrintPreview.OnText = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_BTN_PRINT_SWITCH_ON_TEXT__SHORT_CUT", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //btnAdd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_BTN_ADD_SHORT_CUT", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblTotalFromNumberOder.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_TOTAL_NUMBER_ORDER", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblPrintCount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_PRINT_COUNT", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());
                //lblTransactionCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMBILL_LBL_TRANSACTION_CODE", HFS.APP.Resources.ResourceLanguageManager.LanguageFrmBill, HFS.APP.Base.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeCheckedNodes(TreeListNode node, CheckState check)
        {
            try
            {
                //var noteData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV)node.Tag;
                //if (noteData != null)
                //{
                spinAmount.Value = 0;
                totalAmountDeposit = 0;
                foreach (TreeListNode node1 in treeSereServ.Nodes)
                {
                    UpdateTotalPrice(node1);
                }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataTransaction()
        {
            try
            {
                //xemlai...
                if (this.HisTreatment != null)
                {
                    MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();
                    serviceReqFilter.TREATMENT_ID = this.HisTreatment.ID;
                    var listServiceReq = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(serviceReqFilter);
                    if (listServiceReq != null && listServiceReq.Count > 0)
                    {
                        this.HisRepaySDO.Transaction.TDL_PATIENT_ID = listServiceReq[0].TDL_PATIENT_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultAccountBookForUser()
        {
            try
            {
                if (!String.IsNullOrEmpty(AccountBookCodeForUser))
                {
                    var data = ListAccountBookFormBill.FirstOrDefault(o => o.ACCOUNT_BOOK_CODE == AccountBookCodeForUser);
                    if (data != null)
                    {
                        cboAccountBook.EditValue = data.ID;
                        txtAccountBookCode.Text = data.ACCOUNT_BOOK_CODE;
                        txtTotalFromNumberOder.Text = data.TOTAL + "/" + data.FROM_NUM_ORDER + "/" + (int)(data.CURRENT_NUM_ORDER ?? 0);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Khong tim thay so thu chi co ma " + AccountBookCodeForUser + " duoc cau hinh");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultPayFormForUser()
        {
            try
            {
                if (!String.IsNullOrEmpty(PayFormCodeDefault))
                {
                    var data = ListPayForm.FirstOrDefault(o => o.PAY_FORM_CODE == PayFormCodeDefault);
                    if (data != null)
                    {
                        cboPayForm.EditValue = data.ID;
                        txtPayFormCode.Text = data.PAY_FORM_CODE;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Khong tim thay so thu chi co ma " + AccountBookCodeForUser + " duoc cau hinh");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RemoveControlError()
        {
            try
            {
                positionHandle = -1;
                dxValidationProvider1.RemoveControlError(cboAccountBook);
                dxValidationProvider1.RemoveControlError(cboPayForm);
                dxValidationProvider1.RemoveControlError(spinAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeCheckChildNodes(bool bCheck)
        {
            try
            {
                spinAmount.Value = 0;
                totalAmountDeposit = 0;
                foreach (TreeListNode node in treeSereServ.Nodes)
                {
                    CheckAllNodeByFlag(node, bCheck);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckAllNodeByFlag(TreeListNode Nodes, bool bCheck)
        {
            try
            {
                Nodes.Checked = bCheck;
                foreach (TreeListNode node in Nodes.Nodes)
                {
                    node.Checked = bCheck;
                    if (bCheck)
                    {
                        node.CheckAll();
                    }
                    else
                    {
                        node.UncheckAll();
                    }

                    var item = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV)node.Tag;
                    if (item != null && node.Checked)
                    {
                        decimal totalPatientPrice = ((item.VIR_TOTAL_PATIENT_PRICE != null && !String.IsNullOrEmpty(item.VIR_TOTAL_PATIENT_PRICE.ToString())) ? Convert.ToDecimal(item.VIR_TOTAL_PATIENT_PRICE) : 0);
                        spinAmount.Value += (totalPatientPrice);
                        totalAmountDeposit += totalPatientPrice;
                    }
                    CheckAllNodeByFlag(node, bCheck);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateTotalPriceToControl()
        {
            try
            {
                spinAmount.Value = 0;
                foreach (TreeListNode node in treeSereServ.Nodes)
                {
                    UpdateTotalPrice(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateTotalPrice(TreeListNode Nodes)
        {
            try
            {
                foreach (TreeListNode node in Nodes.Nodes)
                {
                    if (node.Level == 2)
                    {
                        var item = (MOS.EFMODEL.DataModels.V_HIS_DERE_DETAIL)node.Tag;
                        if (item != null && node.Checked)
                        {
                            decimal totalPatientPrice = ((item.VIR_TOTAL_PATIENT_PRICE != null && !String.IsNullOrEmpty(item.VIR_TOTAL_PATIENT_PRICE.ToString())) ? Convert.ToDecimal(item.VIR_TOTAL_PATIENT_PRICE) : 0);
                            spinAmount.Value += (totalPatientPrice);
                            totalAmountDeposit += totalPatientPrice;
                        }
                    }
                    UpdateTotalPrice(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool OneOfChildsIsChecked(TreeListNode node)
        {
            bool result = false;
            foreach (TreeListNode item in node.Nodes)
            {
                if (item.CheckState == CheckState.Checked || item.CheckState == CheckState.Indeterminate)
                {
                    result = true;
                }
            }
            return result;
        }

        #endregion

        #region public function
        public void Save()
        {
            btnSave_Click(null, null);
        }

        public void Add()
        {
            btnAdd_Click(null, null);
        }

        #endregion

        #region Event handler

        #region click
        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {

                if (DevExpress.XtraEditors.XtraMessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (HisRepaySDO != null)
                    {
                        //xemlai...
                        HisTreatment.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        HisTreatment.GROUP_CODE = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetGroupCode();
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_DEPOSIT_DELETE, ApiConsumers.MosConsumer, this.HisRepay, param);
                        if (success)
                        {
                            this.ActionType = GlobalVariables.ActionAdd;
                            RepayProcess.EnableButton(GlobalVariables.ActionAdd, this);
                            //xemlai...
                            RepayProcess.FillDataToControl(this.HisTreatment, this.HisRepay, this);
                        }
                    }
                    MessageManager.Show(param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                RepayProcess.FillDataToControl(null, null, this);
                RepayProcess.EnableButton(GlobalVariables.ActionAdd, this);
                RepayProcess.FillDataToSereServTree(HisTreatment, this);
                ChangeCheckChildNodes(true);
                RemoveControlError();
                SetDefaultAccountBookForUser();
                txtAccountBookCode.SelectAll();
                txtAccountBookCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (!btnSave.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                RepayProcess.UpdateDataFormTransactionDepositToDTO(HisRepaySDO, HisTreatment, this);

                WaitingManager.Show();
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    if (CheckValidForSave())
                    {
                        this.HisRepay = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_REPAY>(HisRequestUriStore.HIS_REPAY_CREATE, ApiConsumers.MosConsumer, this.HisRepaySDO, param);
                        if (this.HisRepay != null)
                        {
                            btnSave.Enabled = false;
                            success = true;
                            this.ActionType = GlobalVariables.ActionView;
                            InitComboAccountBook();
                            RepayProcess.FillDataToControl(this.HisTreatment, this.HisRepay, this);
                            refreshDataAfterSave();
                            SetDefaultAccountBookForUser();
                            //ChangeCheckChildNodes(true);
                            ddbPrint.Enabled = true;
                        }

                        MessageManager.Show(param, success);
                        SessionManager.ProcessTokenLost(param);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
            {
                btnSave_Click(null, null);
            }
        }

        #endregion

        private void bbtnNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
            {
                btnAdd_Click(null, null);
            }
        }
        #endregion

        #region previewKeyDown
        private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    RepayProcess.LoadAccountBookCombo(strValue, false, this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    RepayProcess.LoadPayFormCombo(strValue, false, this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinNumberOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinAmount.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDiscount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region closed
        private void cboAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null && cboAccountBook.EditValue != cboAccountBook.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK accountBook = ListAccountBookFormBill.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));
                        if (accountBook != null)
                        {
                            txtAccountBookCode.Text = accountBook.ACCOUNT_BOOK_CODE;
                            txtTotalFromNumberOder.Text = accountBook.TOTAL + "/" + accountBook.FROM_NUM_ORDER + "/" + (int)(accountBook.CURRENT_NUM_ORDER ?? 0);
                            txtPayFormCode.Focus();
                            txtPayFormCode.SelectAll();
                        }
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
                    if (cboAccountBook.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK accountBook = ListAccountBookFormBill.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));
                        if (accountBook != null)
                        {
                            txtAccountBookCode.Text = accountBook.ACCOUNT_BOOK_CODE;
                            txtTotalFromNumberOder.Text = accountBook.TOTAL + "/" + accountBook.FROM_NUM_ORDER + "/" + (int)(accountBook.CURRENT_NUM_ORDER ?? 0);
                            txtPayFormCode.Focus();
                            txtPayFormCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "CURRENT_NUM_ORDER_DISPLAY")
                {
                    var item = ((List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>)cboAccountBook.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0}/{1}/{2}", (item.TOTAL), (item.FROM_NUM_ORDER), ((int)(item.CURRENT_NUM_ORDER ?? 0)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPayForm.EditValue != null && cboPayForm.EditValue != cboPayForm.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_PAY_FORM commune = ListPayForm.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString()));
                        if (commune != null)
                        {
                            txtPayFormCode.Text = commune.PAY_FORM_CODE;
                            spinAmount.Focus();
                            spinAmount.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPayForm.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PAY_FORM commune = ListPayForm.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString()));
                        if (commune != null)
                        {
                            txtPayFormCode.Text = commune.PAY_FORM_CODE;
                            txtDescription.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);

                TreeListNode node = e.Node;
                if (node.Checked)
                {
                    node.UncheckAll();
                }
                else
                {
                    node.CheckAll();
                }
                while (node.ParentNode != null)
                {
                    node = node.ParentNode;
                    bool oneOfChildIsChecked = OneOfChildsIsChecked(node);
                    if (oneOfChildIsChecked)
                    {
                        node.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        node.CheckState = CheckState.Unchecked;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                ChangeCheckedNodes(e.Node, e.Node.CheckState);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            try
            {
                var noteData = (MOS.EFMODEL.DataModels.V_HIS_DERE_DETAIL)e.Node.Tag;
                if (noteData != null)
                {
                    UpdateTotalPriceToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
          
        }

        private void treeSereServ_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                e.RepositoryItem = txtReadOnly;
                //if (e.Column.FieldName == "AMOUNT")
                //{
                //    e.RepositoryItem = txtReadOnly;

                //    var noteData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV)e.Node.Tag;
                //    if (noteData != null)
                //    {
                //        if (noteData.BILL_ID > 0 && noteData.ACCOUNT_BOOK_ID > 0)
                //            e.RepositoryItem = txtReadOnly;
                //        else
                //            e.RepositoryItem = spinSoLuong;
                //        //{
                //        //    case "true":
                //        //        e.RepositoryItem = spinSoLuong;
                //        //        break;
                //        //    case "false":
                //        //        e.RepositoryItem = spinSoLuongReadOnly;
                //        //        break;
                //        //    default:
                //        //        e.RepositoryItem = txtReadOnly;
                //        //        break;
                //        //}
                //    }
                //    else
                //        e.RepositoryItem = txtReadOnly;
                //}
                //else
                //{
                //    e.RepositoryItem = txtReadOnly;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountBookCode.SelectAll();
                    txtAccountBookCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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
        #endregion


    }
}
