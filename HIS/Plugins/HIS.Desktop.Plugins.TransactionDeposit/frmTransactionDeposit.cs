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
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.TransactionDeposit.Base;
using HIS.Desktop.Plugins.TransactionDeposit.Config;
using HIS.Desktop.Plugins.TransactionDeposit.Validation;
using HIS.Desktop.Plugins.TransactionDeposit.Validtion;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.WCF.JsonConvert;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WCF;
using WCF.Client;

namespace HIS.Desktop.Plugins.TransactionDeposit
{
    public partial class frmTransactionDeposit : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        V_HIS_TREATMENT_FEE treatment = null;
        V_HIS_DEPOSIT_REQ depositReq = null;
        long cashierRoomId;
        V_HIS_TRANSACTION resultTranDeposit = null;
        List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

        List<HIS_DEPOSIT_REASON> lstDepositReason = null;

        int positionHandleControl = -1;

        bool isNotLoadWhilechkAutoCloseStateInFirst = true;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.TransactionDeposit";
        string creator = "";
        bool isShowMess = false;
        bool isSaveAndSign = false;
        WcfClient cll;
        DateTime dteCommonParam { get; set; }
        private bool is_true = true;

        public frmTransactionDeposit(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = module;
                //this.Size = new Size(896, 200);
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionDeposit(Inventec.Desktop.Common.Modules.Module module, TransactionDepositADO data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                if (data != null)
                {
                    this.treatment = data.Treatment;
                    this.cashierRoomId = data.CashierRoomId;
                }
                this.currentModule = module;
                //this.Size = new Size(896, 200);
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionDeposit_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.LoadKeyFrmLanguage();
                ValidControl();
                WaitingManager.Hide();

                InitLinkLabel();

                timerInitForm.Interval = 100;
                timerInitForm.Enabled = true;
                timerInitForm.Start();
                InitControlState();
                btnSave.Enabled = is_true;
                btnSavePrint.Enabled = is_true;
                btnQR.Enabled = false;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

       

        private void InitLinkLabel()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDepositReasonFilter filter = new HisDepositReasonFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.IS_COMMON = 1;
                this.lstDepositReason = new BackendAdapter(param).Get<List<HIS_DEPOSIT_REASON>>("api/HisDepositReason/Get", ApiConsumers.MosConsumer, filter, param);
                this.lstDepositReason = this.lstDepositReason.Where(o => !string.IsNullOrWhiteSpace(o.ABBREVIATION)).OrderBy(p => p.ABBREVIATION).ToList();

                this.linkLabel1.Links.Clear();
                var numberOfText = this.lstDepositReason.Count;
                if (numberOfText > 0)
                {
                    int total = 0;
                    int oldLocation = 0;
                    for (int i = 0; i < numberOfText; i++)
                    {
                        this.linkLabel1.Text += this.lstDepositReason[i].ABBREVIATION + "; ";
                        this.linkLabel1.Links.Add(oldLocation, this.lstDepositReason[i].ABBREVIATION.Length + 1, this.lstDepositReason[i].DEPOSIT_REASON_NAME);
                        oldLocation += this.lstDepositReason[i].ABBREVIATION.Length + 2;
                        total += this.lstDepositReason[i].ABBREVIATION.Length + 3;
                    }

                    string khac = "Khác...";
                    this.linkLabel1.Text += khac;
                    this.linkLabel1.Links.Add(oldLocation, khac.Length, khac);
                    int after = total / this.linkLabel1.Width;
                    this.linkLabel1.Height = after * 24;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void linkControl1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                try
                {
                    if (e.Link.LinkData == "Khác...")
                    {
                        frmDepositReason frm = new frmDepositReason(currentModule, (HIS.Desktop.Common.DelegateSelectData)dataResult);
                        frm.ShowDialog();
                    }
                    else
                    {
                        txtDescription.Text = e.Link.LinkData + "";
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

        private void dataResult(object data)
        {
            try
            {
                if (data != null && data is string)
                {
                    string dt = data as string;

                    if (!string.IsNullOrEmpty(dt))
                    {
                        txtDescription.Text = dt;
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormatMoney()
        {
            try
            {
                FormatSpint(txtTotalAmount);
                FormatSpint(spinTransferAmount);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormatSpint(DevExpress.XtraEditors.TextEdit textEdit)
        {
            try
            {
                int munberSeperator = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator;
                int munbershowDecimal = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.Desktop.Plugins.ShowDecimalOption");
                string Format = "";
                if (munbershowDecimal == 0)
                {
                    if (munberSeperator == 0)
                    {
                        Format = "#,##0";
                    }
                    else
                    {
                        Format = "#,##0.";
                        for (int i = 0; i < munberSeperator; i++)
                        {
                            Format += "0";
                        }
                    }

                }
                else if (munbershowDecimal == 1)
                {
                    if (textEdit.EditValue != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToDecimal(textEdit.EditValue.ToString() ?? "") % 1 > 0)
                        {
                            if (munberSeperator == 0)
                            {
                                Format = "#,##0";
                            }
                            else
                            {
                                Format = "#,##0.";
                                for (int i = 0; i < munberSeperator; i++)
                                {
                                    Format += "0";
                                }
                            }
                        }
                        else
                        {
                            Format = "#,##0";
                        }
                    }
                }
                textEdit.Properties.DisplayFormat.FormatString = Format;
                textEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                textEdit.Properties.EditFormat.FormatString = Format;
                textEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        
        private void LoadSearchByTreatmentCode()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                if (!String.IsNullOrEmpty(txtTreatmenCode.Text))
                {
                    
                    string code = txtTreatmenCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmenCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;

                    var listTreatment = new BackendAdapter(param)
                            .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(ApiUri.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        
                        this.treatment = listTreatment.FirstOrDefault();
                        
                        txtTotalAmount.Focus();
                        txtTotalAmount.SelectAll();
                        
                        
                    }
                    else
                    {
                        MessageBox.Show(Base.ResourceMessageLang.KhongTimThayMaDieuTri);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSearchByDepositReqCode()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDepositReqViewFilter filter = new HisDepositReqViewFilter();
                if (!String.IsNullOrEmpty(txtDepositReqCode.Text))
                {
                    string code = txtDepositReqCode.Text.Trim();
                    if (code.Length < 8)
                    {
                        code = string.Format("{0:00000000}", Convert.ToInt64(code));
                        txtDepositReqCode.Text = code;
                    }
                    filter.DEPOSIT_REQ_CODE__EXACT = code;

                    var listDepositReq = new BackendAdapter(param)
                            .Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPOSIT_REQ>>(ApiUri.HIS_DEPOSIT_REQ_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                    if (listDepositReq != null && listDepositReq.Count > 0)
                    {
                        this.depositReq = listDepositReq.FirstOrDefault();
                        this.treatment = new V_HIS_TREATMENT_FEE();
                        this.treatment.ID = this.depositReq.TREATMENT_ID;
                        this.treatment.PATIENT_ID = this.depositReq.PATIENT_ID;
                        this.treatment.TREATMENT_CODE = this.depositReq.TREATMENT_CODE;

                        txtDescription.Text = this.depositReq.DESCRIPTION;

                        txtTotalAmount.Focus();
                        txtTotalAmount.SelectAll();

                    }
                    else
                    {
                        MessageBox.Show(Base.ResourceMessageLang.KhongTimThayMaYeuCauTamUng);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region Kiem tra du lieu tim kiem
        private bool checkDK()
        {
            bool sucess = true;
            
            try
            {
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.TREATMENT_CODE__EXACT = this.treatment.TREATMENT_CODE;
                var data = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>(ApiUri.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                if (data != null)
                {
                    string treatment_code = this.treatment.TREATMENT_CODE;
                    short is_pause = data.IS_PAUSE ?? 0;
                    if (data.DIS_DEPOSIT_OPTION == 1)
                    {
                        DialogResult rs = MessageBox.Show("Không cho phép tạm ứng do diện điều trị được check vào checkbox chặn không cho phép tạm ứng trong danh mục diện điều trị");
                        sucess = false;
                    }
                    if (data.DIS_DEPOSIT_OPTION == 2 && is_pause == 1)
                    {
                        sucess = false;
                        DialogResult rs = MessageBox.Show("Không cho phép hồ sơ tạm ứng do đã kết thúc điều trị");
                    }
                }
                else sucess = false;
            }
            
                
            catch (Exception)
            {
                sucess = false;
                
                throw;
            }
            return sucess;
        }
        #endregion
        private void FillDataToCommon(V_HIS_TREATMENT_FEE data)
        {
            try
            {
                if (data != null)
                {
                    lblPatientCode.Text = data.TDL_PATIENT_CODE;
                    lblPatientName.Text = data.TDL_PATIENT_NAME;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    lblGenderName.Text = data.TDL_PATIENT_GENDER_NAME;
                    lblAddress.Text = data.TDL_PATIENT_ADDRESS;
                }
                else
                {
                    lblPatientCode.Text = "";
                    lblPatientName.Text = "";
                    lblDob.Text = "";
                    lblGenderName.Text = "";
                    lblAddress.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCommon(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                if (data != null)
                {
                    lblPatientCode.Text = data.TDL_PATIENT_CODE;
                    lblPatientName.Text = data.TDL_PATIENT_NAME;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    lblGenderName.Text = data.TDL_PATIENT_GENDER_NAME;
                    lblAddress.Text = data.TDL_PATIENT_ADDRESS;
                    txtTotalAmount.Value = data.AMOUNT;
                }
                else
                {
                    lblPatientCode.Text = "";
                    lblPatientName.Text = "";
                    lblDob.Text = "";
                    lblGenderName.Text = "";
                    lblAddress.Text = "";
                    txtTotalAmount.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerInitForm_Tick(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                this.timerInitForm.Stop();

                this.LoadDataToComboPayForm();
                //this.SetDefaultAccountBook(true);
                this.LoadAccountBookToLocal(true);
                this.ResetDefaultValueControl();
                this.GeneratePrintMenu();
                this.ValidControlDescription();
                if (this.treatment != null && this.treatment.ID > 0)
                {
                    FillDataToCommon(this.treatment);
                    txtTreatmenCode.Text = this.treatment.TREATMENT_CODE;
                }
                else
                {
                    FillDataToCommon(this.depositReq);
                    txtDepositReqCode.Text = this.depositReq != null ? this.depositReq.DEPOSIT_REQ_CODE : "";
                }
                if (this.treatment != null)
                {
                    txtTotalAmount.Focus();
                    txtTotalAmount.SelectAll();
                }
                else
                {
                    txtTreatmenCode.Focus();
                    txtTreatmenCode.SelectAll();
                }
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void CheckPayFormTienMatChuyenKhoan(HIS_PAY_FORM payForm)
        {
            try
            {
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;

                }
                else if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_SWIPE_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_SWIPE_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;

                }
                else if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(false);
                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTranferAmount.Enabled = false;
                }
                
                else
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(false);
                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTranferAmount.Enabled = false;
                }
                spinTransferAmount.EditValue = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlTransferAmount(bool IsRequiredField)
        {
            try
            {
                SpinTranferAmountValidationRule PINRule = new SpinTranferAmountValidationRule();
                PINRule.spinTranferAmount = spinTransferAmount;
                PINRule.isRequiredPin = IsRequiredField;
                dxValidationProvider1.SetValidationRule(spinTransferAmount, PINRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook, bool isFirstLoad = false)
        {
            try
            {
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    if (GlobalVariables.dicNumOrderInAccountBook == null || GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (GlobalVariables.dicNumOrderInAccountBook != null && GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                        }

                        layoutTongTuDen.Enabled = true;
                        if (!isFirstLoad)
                        {
                            CommonParam param = new CommonParam();
                            MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new HisAccountBookViewFilter();
                            hisAccountBookViewFilter.ID = accountBook.ID;
                            var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                            if (accountBooks != null && accountBooks.Count > 0)
                            {
                                accountBook = accountBooks.FirstOrDefault();
                            }
                        }
                        if (accountBook != null)
                        {
                            decimal num = 0;
                            if ((accountBook.CURRENT_NUM_ORDER ?? 0) > 0)
                            {
                                num = accountBook.CURRENT_NUM_ORDER ?? 0;
                            }
                            else
                            {
                                num = (decimal)accountBook.FROM_NUM_ORDER - 1;
                            }
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBook.ID, num);
                            spinTongTuDen.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                        }
                    }
                    else
                    {
                        layoutTongTuDen.Enabled = true;
                        spinTongTuDen.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                    }
                }
                else
                {
                    spinTongTuDen.Value = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
                    layoutTongTuDen.Enabled = false;
                }
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
                ValidControlAccountBook();
                //ValidControlPayForm();
                ValidControlAmount();
                ValidControlTransactionTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlAccountBook()
        {
            try
            {
                AccountBookValidationRule accountBookRule = new AccountBookValidationRule();
                accountBookRule.cboAccountBook = cboAccountBook;
                dxValidationProvider1.SetValidationRule(cboAccountBook, accountBookRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void ValidControlPayForm()
        //{
        //    try
        //    {
        //        PayFormValidationRule payFormRule = new PayFormValidationRule();
        //        payFormRule.txtPayFormCode = txtPayFormCode;
        //        payFormRule.cboPayForm = cboPayForm;
        //        dxValidationProvider1.SetValidationRule(txtPayFormCode, payFormRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void ValidControlAmount()
        {
            try
            {
                AmountValidationRule amountRule = new AmountValidationRule();
                amountRule.txtTotalAmount = txtTotalAmount;
                dxValidationProvider1.SetValidationRule(txtTotalAmount, amountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlTransactionTime()
        {
            try
            {
                TransactionTimeValidationRule amountRule = new TransactionTimeValidationRule();
                amountRule.dtTransactionTime = dtTransactionTime;
                dxValidationProvider1.SetValidationRule(dtTransactionTime, amountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDescription()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validate.editor = this.txtDescription;
                validate.maxLength = 2000;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá ký tự cho phép {0}", 2000);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtDescription, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_CASHIER_ROOM LoadCashierRoom()
        {
            V_HIS_CASHIER_ROOM cashierRoom = null;
            try
            {
                List<V_HIS_CASHIER_ROOM> cashierRooms = null;
                if (BackendDataWorker.IsExistsKey<V_HIS_CASHIER_ROOM>())
                {
                    cashierRooms = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisCashierRoomFilter filter = new MOS.Filter.HisCashierRoomFilter();
                    cashierRooms = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>>("api/HisCashierRoom/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (cashierRooms != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM), cashierRooms, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                cashierRoom = cashierRooms.FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId && o.ROOM_TYPE_ID == this.currentModule.RoomTypeId);

                if (cashierRoom == null)
                    Inventec.Common.Logging.LogSystem.Error("Khong lay duoc phong thu ngan: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentModule.RoomId), this.currentModule.RoomId));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Khong lay duoc phong thu ngan: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentModule.RoomId), this.currentModule.RoomId));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return cashierRoom;
        }

        private async Task LoadAccountBookToLocal(bool isFirstLoad = false)
        {
            try
            {
                if (this.cashierRoomId == 0)
                {
                    var cashierRoom = LoadCashierRoom();
                    this.cashierRoomId = cashierRoom != null ? cashierRoom.ID : 0;
                }
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam paramCommon = new CommonParam();
                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                acFilter.CASHIER_ROOM_ID = this.cashierRoomId;//Kiểm tra sổ còn hay k
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.FOR_DEPOSIT = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                this.ListAccountBook = await new BackendAdapter(paramCommon).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumer.ApiConsumers.MosConsumer, acFilter, paramCommon);
                dteCommonParam = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(paramCommon.Now) ?? DateTime.Now;
                if (HisConfigCFG.ShowServerTimeByDefault == "1")
                {
                    dtTransactionTime.DateTime = dteCommonParam;
                }
                this.ListAccountBook = this.ListAccountBook.Where(o => o.WORKING_SHIFT_ID == null || o.WORKING_SHIFT_ID == (HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO.WorkingShiftId ?? 0)).ToList();
                this.LoadDataToComboAccountBook();
                this.SetDefaultAccountBook(isFirstLoad);
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
                cboAccountBook.Properties.DataSource = ListAccountBook;
                if (cboAccountBook.Properties.Columns.Count == 0)
                {
                    cboAccountBook.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                    cboAccountBook.Properties.ValueMember = "ID";
                    cboAccountBook.Properties.ForceInitialize();
                    cboAccountBook.Properties.Columns.Clear();
                    cboAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_CODE", "", 50));
                    cboAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_NAME", "", 200));
                    cboAccountBook.Properties.ShowHeader = false;
                    cboAccountBook.Properties.ImmediatePopup = true;
                    cboAccountBook.Properties.DropDownRows = 10;
                    cboAccountBook.Properties.PopupWidth = 250;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataToComboPayForm()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> listPayForm = null;
                if (BackendDataWorker.IsExistsKey<HIS_PAY_FORM>())
                {
                    listPayForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    listPayForm = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>>("api/HisPayForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (listPayForm != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM), listPayForm, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                if(listPayForm != null && listPayForm.Count > 0 && (string.IsNullOrEmpty(HisConfigCFG.CashierRoomPaymentOption) || (HisConfigCFG.CashierRoomPaymentOption != null && !HisConfigCFG.OptionKey.IsDefined(typeof(HisConfigCFG.OptionKey), Int32.Parse(HisConfigCFG.CashierRoomPaymentOption)))))
                {
                    listPayForm = listPayForm.Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE).ToList();
                }

                cboPayForm.Properties.DataSource = listPayForm.Where(o => o.IS_ACTIVE == 1).ToList();
                cboPayForm.Properties.DisplayMember = "PAY_FORM_NAME";
                cboPayForm.Properties.ValueMember = "ID";
                cboPayForm.Properties.ForceInitialize();
                cboPayForm.Properties.Columns.Clear();
                cboPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_CODE", "", 50));
                cboPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_NAME", "", 150));
                cboPayForm.Properties.ShowHeader = false;
                cboPayForm.Properties.ImmediatePopup = true;
                cboPayForm.Properties.DropDownRows = 10;
                cboPayForm.Properties.PopupWidth = 200;

                var payFormDefault = listPayForm != null ? listPayForm.OrderBy(o => o.PAY_FORM_CODE).FirstOrDefault() : null;
                if (payFormDefault != null)
                {
                    cboPayForm.EditValue = payFormDefault.ID;
                    //txtPayFormCode.Text = payFormDefault.PAY_FORM_CODE;
                    CheckPayFormTienMatChuyenKhoan(payFormDefault);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetDefaultValueControl()
        {
            try
            {
                this.resultTranDeposit = null;
                txtDescription.Text = "";
                spinTongTuDen.Text = "";
                txtTotalAmount.Value = 0;
                txtTransactionCode.Text = "";
                HisConfigCFG.LoadConfig();
                btnPrint.Enabled = false;
                ddBtnPrint.Enabled = false;
                btnSave.Enabled = true;
                btnSaveAndSign.Enabled = true;
                spinTransferAmount.EditValue = null;
                V_HIS_TREATMENT_FEE fee = null;
                FillDataToCommon(fee);
                if (HisConfigCFG.ShowServerTimeByDefault == "1")
                {
                    dtTransactionTime.DateTime = dteCommonParam;
                }
                else
                {
                    dtTransactionTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                }
                if (HisConfigCFG.IsEditTransactionTimeCFG != null && HisConfigCFG.IsEditTransactionTimeCFG.Equals("1"))
                {
                    lciTransactionTime.Enabled = true;
                }
                else
                {
                    lciTransactionTime.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultAccountBook(bool isFirstLoad = false)
        {
            try
            {
                V_HIS_ACCOUNT_BOOK data = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        data = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }

                if (data == null) data = ListAccountBook.FirstOrDefault();
                if (data != null)
                {
                    //txtAccountBookCode.Text = data.ACCOUNT_BOOK_CODE;
                    cboAccountBook.EditValue = data.ID;
                    SetDataToDicNumOrderInAccountBook(data, isFirstLoad);
                }
                else
                {
                    spinTongTuDen.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GeneratePrintMenu()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__BTN_DROP_DOWN__ITEM_PHIEU_TAM_UNG", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuTamUng)));

                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__BTN_DROP_DOWN__ITEM_PHIEU_TAM_UNG_VA_GIU_THE", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuTamUngVaGiuThe)));

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuThuTamUngVaGiuThe(object sender, EventArgs e)
        {
            try
            {
                if (this.resultTranDeposit == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(MPS.Processor.Mps000171.PDO.PrintTypeCode.Mps000171, DelegatePrintTempalte);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuThuTamUng(object sender, EventArgs e)
        {
            try
            {
                if (this.resultTranDeposit == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(MPS.Processor.Mps000172.PDO.PrintTypeCode.Mps000172, DelegatePrintTempalte);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTotalAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtTransactionTime.Enabled)
                    {
                        dtTransactionTime.Focus();
                        dtTransactionTime.ShowPopup();
                    }
                    else if (txtTransactionCode.Enabled)
                    {
                        txtTransactionCode.Focus();
                        txtTransactionCode.SelectAll();
                    }
                    else
                    {
                        txtTransactionCode.Focus();
                        txtTransactionCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkConnectionPOS.Focus();
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
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (layoutTongTuDen.Enabled)
                    {
                        spinTongTuDen.Focus();
                        spinTongTuDen.SelectAll();
                    }
                    else
                    {
                        cboPayForm.Focus();
                        cboPayForm.SelectAll();
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
        //                var listData = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.PAY_FORM_CODE.Contains(txtPayFormCode.Text)).ToList();
        //                if (listData != null && listData.Count == 1)
        //                {
        //                    valid = true;
        //                    txtPayFormCode.Text = listData.First().PAY_FORM_CODE;
        //                    cboPayForm.EditValue = listData.First().ID;
        //                    CheckPayFormTienMatChuyenKhoan(listData.First());
        //                    cboPayForm.Focus();
        //                    SendKeys.Send("{TAB}");
        //                }
        //            }
        //            if (!valid)
        //            {
        //                cboPayForm.Focus();
        //                cboPayForm.ShowPopup();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    HIS_PAY_FORM payForm = null;
                    if (cboPayForm.EditValue != null)
                    {
                        payForm = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.ID == (long)cboPayForm.EditValue).FirstOrDefault();
                        CheckPayFormTienMatChuyenKhoan(payForm);
                        SendKeys.Send("{TAB}");
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSavePrint.Enabled || !dxValidationProvider1.Validate() || this.treatment == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                this.SaveDeposit(param, ref success);
                if (success)
                {
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                    btnSavePrint.Enabled = false;
                    btnSaveAndSign.Enabled = false;
                }
                WaitingManager.Hide();
                if (!success)
                {
                    if (!isShowMess)
                    {
                        MessageManager.Show(param, success);
                    }
                }
                else
                {
                    this.InPhieuThuTamUng(true);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = spinTongTuDen.Value;
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
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.treatment == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                this.SaveDeposit(param, ref success);
                if (success)
                {
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                    ddBtnPrint.Enabled = true;
                    btnSavePrint.Enabled = false;
                    btnSaveAndSign.Enabled = false;
                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                    if (chkAutoClose.Checked)
                    {
                        this.Close();
                    }
                }
                else
                {
                    if (!isShowMess)
                    {
                        MessageManager.Show(param, success);
                    }
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultTranDeposit == null)
                    return;
                this.InPhieuThuTamUng(false);
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
                if (!btnNew.Enabled)
                    return;
                WaitingManager.Show();
                LoadAccountBookToLocal();
                ResetDefaultValueControl();
                SetDefaultPayFormForUser();
                txtTreatmenCode.Text = "";
                txtDepositReqCode.Text = "";
                if (this.treatment != null)
                {
                    txtTotalAmount.Focus();
                    txtTotalAmount.SelectAll();
                }
                else
                {
                    txtTreatmenCode.Focus();
                    txtTreatmenCode.SelectAll();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSavePrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        bool CheckPayformCard(MOS.EFMODEL.DataModels.HIS_PAY_FORM payForm)
        {
            bool result = false;
            try
            {
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                    result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        // gọi sang WCF tạm ứng qua thẻ
        CARD.WCF.DCO.WcfDepositDCO DepositCard(CARD.WCF.DCO.WcfDepositDCO DepositDCO)
        {
            CARD.WCF.DCO.WcfDepositDCO result = null;
            CommonParam param = new CommonParam();
            try
            {
                // gọi api HisCard/Get để lấy về serviceCodes
                MOS.Filter.HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.PATIENT_ID = this.treatment.PATIENT_ID;
                var HisCards = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, param);
                if (HisCards != null && HisCards.Count > 0)
                {
                    DepositDCO.ServiceCodes = HisCards.Select(o => o.SERVICE_CODE).ToArray();
                }
                CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();

                result = transactionClientManager.Deposit(DepositDCO);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        // gọi WCF (phần mềm thẻ) để hủy giao dịch 
        CARD.WCF.DCO.WcfVoidDCO VoidCard(ref CARD.WCF.DCO.WcfVoidDCO VoidDCO)
        {
            CARD.WCF.DCO.WcfVoidDCO result = null;
            CommonParam param = new CommonParam();
            try
            {
                CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();
                result = transactionClientManager.Void(VoidDCO);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SaveDeposit(CommonParam param, ref bool success)
        {
            try
            {
                if (cboAccountBook.EditValue == null || cboPayForm.EditValue == null || txtTotalAmount.Value <= 0)
                {
                    param.Messages.Add(Base.ResourceMessageLang.ThieuTruongDuLieuBatBuoc);
                    return;
                }

                CARD.WCF.DCO.WcfDepositDCO DepositDCO = new CARD.WCF.DCO.WcfDepositDCO();
                // thanh toán qua thẻ 
                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                if (payForm == null)
                    return;

                //HisDepositSDO data = new HisDepositSDO();
                HisTransactionDepositSDO data = new HisTransactionDepositSDO();

                //xuandv mo lai neu mo tu tiep don lay phong thu ngan sai
                //if (this.currentModule != null)
                //{
                //    data.RequestRoomId = this.currentModule.RoomId;
                //}
                if (this.cashierRoomId > 0)
                {
                    var cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == this.cashierRoomId);
                    data.RequestRoomId = cashierRoom.ROOM_ID;
                }
                else if (this.currentModule != null && this.currentModule.RoomId > 0)
                {
                    var cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
                    data.RequestRoomId = cashierRoom.ROOM_ID;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("cashierRoomId is null");
                }
                data.Transaction = new HIS_TRANSACTION();
                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    data.Transaction.ACCOUNT_BOOK_ID = accountBook.ID;
                }
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    data.Transaction.NUM_ORDER = (long)(spinTongTuDen.Value);
                }

                data.Transaction.AMOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(txtTotalAmount.Text);
                if (payForm != null)
                {
                    data.Transaction.PAY_FORM_ID = payForm.ID;
                    if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && spinTransferAmount.EditValue != null)
                    {

                        if (spinTransferAmount.Value > data.Transaction.AMOUNT)
                        {
                            param.Messages.Add(String.Format("Số tiền chuyển khoản [{0}] lớn hơn số tiền tạm ứng của bệnh nhân [{1}]", Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToStringRoundAuto(data.Transaction.AMOUNT, ConfigApplications.NumberSeperator)));
                            return;
                        }
                        else
                        {
                            data.Transaction.TRANSFER_AMOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(spinTransferAmount.Text);
                        }
                    }
                    else if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && spinTransferAmount.EditValue != null)
                    {

                        if (spinTransferAmount.Value > data.Transaction.AMOUNT)
                        {
                            param.Messages.Add(String.Format("Số tiền quẹt thẻ [{0}] lớn hơn số tiền tạm ứng của bệnh nhân [{1}]", Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToStringRoundAuto(data.Transaction.AMOUNT, ConfigApplications.NumberSeperator)));
                            return;
                        }
                        else
                        {
                            data.Transaction.SWIPE_AMOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(spinTransferAmount.Text);
                        }
                    }
                }

                if (this.depositReq != null && this.depositReq.ID > 0)
                {
                    data.DepositReqId = this.depositReq.ID;
                }

                data.Transaction.TREATMENT_ID = this.treatment.ID;
                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                    data.Transaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtTransactionTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                data.Transaction.DESCRIPTION = txtDescription.Text;
                data.Transaction.CASHIER_ROOM_ID = this.cashierRoomId;

                long money = 0;
                if (spinTransferAmount.EditValue != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                {
                    money = (long)spinTransferAmount.Value;
                }
                else if (txtTotalAmount.EditValue != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                {
                    money = (long)txtTotalAmount.Value;
                }
                Inventec.Common.Logging.LogSystem.Info(" money " + money);
                // tạm ứng qua thẻ
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                {
                    //kiểm tra trước khi thanh toán nếu là giao dịch qua thẻ
                    var check = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(UriStores.HIS_TRANSACTION_CHECK_DEPOSIT, ApiConsumers.MosConsumer, data, param);

                    if (!check)
                        return;

                    // nếu hình thức thanh toán qua thẻ thì gọi WCF tab thẻ (POS)                
                    DepositDCO.DepositAmount = txtTotalAmount.Value;
                    //DepositDCO.PinCode = this.txtPin.Text.Trim();
                    DepositDCO = DepositCard(DepositDCO);
                    // nếu gọi sang POS trả về false thì kết thúc
                    if (DepositDCO == null || DepositDCO.ResultCode == null || !DepositDCO.ResultCode.Equals("00"))
                    {
                        success = false;
                        MappingErrorTHE mappingErrorTHE = new Config.MappingErrorTHE();
                        Inventec.Common.Logging.LogSystem.Info("DepositDCO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => DepositDCO), DepositDCO));

                        //param.Messages.Add(ResourceMessageLang.TamUngQuaTheThatBai);
                        if (DepositDCO != null
                            && !String.IsNullOrWhiteSpace(DepositDCO.ResultCode)
                            && mappingErrorTHE.dicMapping != null
                            && mappingErrorTHE.dicMapping.Count > 0
                            && mappingErrorTHE.dicMapping.ContainsKey(DepositDCO.ResultCode))
                        {
                            param.Messages.Add(mappingErrorTHE.dicMapping[DepositDCO.ResultCode]);
                        }
                        else if (DepositDCO != null && String.IsNullOrWhiteSpace(DepositDCO.ResultCode))
                        {
                            param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                        }
                        else if (DepositDCO != null
                            && !String.IsNullOrWhiteSpace(DepositDCO.ResultCode)
                            && mappingErrorTHE.dicMapping != null
                            && mappingErrorTHE.dicMapping.Count > 0
                            && !mappingErrorTHE.dicMapping.ContainsKey(DepositDCO.ResultCode)
                            )
                        {
                            param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                        }
                        return;
                    }
                    else
                    {
                        //nếu giao dịch thanh toán qua thẻ thì gửi lên TIG_TRANSACTION_CODE
                        data.CardCode = DepositDCO.TransServiceCode;
                        data.Transaction.TIG_TRANSACTION_CODE = DepositDCO.TransactionCode;
                        data.Transaction.TIG_TRANSACTION_TIME = DepositDCO.TransactionTime;
                    }
                }
                else if ((payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE || payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                    && chkConnectionPOS.Checked == true && money > 0)
                {
                    var check = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(UriStores.HIS_TRANSACTION_CHECK_DEPOSIT, ApiConsumers.MosConsumer, data, param);

                    if (!check)
                        return;

                    OpenAppPOS();
                    Inventec.Common.Logging.LogSystem.Error(creator + "    " + Guid.NewGuid().ToString());

                    WcfRequest wc = new WcfRequest(); // Khởi tạo data

                    wc.AMOUNT = money; // Số tiền
                    wc.billId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
                    wc.creator = creator;

                    var json = JsonConvert.Serialize<WcfRequest>(wc);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => json), json));

                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
                    try
                    {

                        try
                        {
                            if (cll == null)
                            {
                                cll = new WcfClient();
                            }
                        }
                        catch (Exception ex)
                        {
                            success = false;
                            XtraMessageBox.Show("Kiểm tra lại cấu hình NetTcpBinding_IService1", "Thông báo");
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            return;
                        }
                        var result = cll.Sale(System.Convert.ToBase64String(plainTextBytes));

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                        if (result != null && result.RESPONSE_CODE == "00")
                        {

                            data.Transaction.POS_PAN = result.PAN;
                            data.Transaction.POS_CARD_HOLDER = result.NAME;
                            data.Transaction.POS_INVOICE = result.INVOICE.ToString();
                            data.Transaction.POS_RESULT_JSON = JsonConvert.Serialize<WcfRequest>(result);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.Transaction), data.Transaction));
                        }
                        else
                        {
                            if (result != null)
                            {
                                WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.
                               Show(ResourceMessageLang.TamUngQuaPOSThatBai + "(Mã lỗi: " + result.ERROR + ")", ResourceMessageLang.ThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                                    return;
                            }
                            success = false;
                            Inventec.Common.Logging.LogSystem.Error("EX 1 #############################");
                            isShowMess = true;
                            WaitingManager.Hide();
                            MessageManager.Show(this, param, false);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error("EX 2 #############################");
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, false);
                        return;
                    }

                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>(UriStores.HIS_TRANSACTION_CREATE_DEPOSIT, ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    AddLastAccountToLocal();
                    this.resultTranDeposit = rs;
                    SetValueContronlDepositSuccess();
                    UpdateDictionaryNumOrderAccountBook(accountBook);

                    if (rs.PAY_FORM_ID == 8) btnQR.Enabled = true;
                    loadConfig();
                    if (rs.PAY_FORM_ID == 8 && rs.IS_ACTIVE == 0) CreateQR(rs,false);

                }
                else
                {
                    // nếu gọi MOS thất bại thì gọi WCF hủy giao dịch
                    if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                    {
                        CARD.WCF.DCO.WcfVoidDCO WcfVoidDCO = new CARD.WCF.DCO.WcfVoidDCO();
                        WcfVoidDCO.Amount = data.Transaction.AMOUNT;
                        WcfVoidDCO.TransactionCode = DepositDCO.TransactionCode;
                        var resultWcf = VoidCard(ref WcfVoidDCO);
                        if (resultWcf == null || (resultWcf != null && !resultWcf.ResultCode.Equals("00")))
                        {
                            success = false;
                            Inventec.Common.Logging.LogSystem.Info("[result code]: " + resultWcf.ResultCode);
                            param.Messages.Add(ResourceMessageLang.HuyGiaoDichThanhToanTheThatBai + WcfVoidDCO.TransactionCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
        }
        List<HIS_CONFIG> listConfig = new List<HIS_CONFIG>();
        HIS_CONFIG selectedConfig = new HIS_CONFIG();
        private void loadConfig()
        {
            try
            {
                listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
                if (listConfig == null || listConfig.Count == 0) btnQR.Enabled = false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreateQR(V_HIS_TRANSACTION data,bool click)
        {
            try
            {
                if (listConfig != null)
                {
                    if (listConfig.Count > 1)
                    {
                        if (!click)
                        {
                            MessageBox.Show(this, "Vui lòng sử dụng nút tạo QR để thực hiện thanh toán", "Thông báo",MessageBoxButtons.OK);
                            return;
                        }
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
                                TransReqQRADO adoqr = new TransReqQRADO();
                                adoqr.TreatmentId = this.treatment.ID;
                                adoqr.ConfigValue = selectedConfig;
                                adoqr.TransReqId = CreateReqType.Transaction;
                                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, data);
                                adoqr.Transaction = tran;
                                listArgs.Add(adoqr);
                                Inventec.Common.Logging.LogSystem.Debug("Goi den module CreateTransReqQR. TreatmentID:" + adoqr.TreatmentId + " config: " + adoqr.ConfigValue.KEY + " tran_id: " + adoqr.Transaction.ID);
                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

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
                        adoqr.TreatmentId = this.treatment.ID;
                        adoqr.ConfigValue = selectedConfig;
                        adoqr.TransReqId = CreateReqType.Transaction;
                        HIS_TRANSACTION tran = new HIS_TRANSACTION();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, data);
                        adoqr.Transaction = tran;
                        listArgs.Add(adoqr);
                        Inventec.Common.Logging.LogSystem.Debug("Goi den module CreateTransReqQR. TreatmentID:" + adoqr.TreatmentId + " config: " + adoqr.ConfigValue.KEY + " tran_id: " + adoqr.Transaction.ID);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);


                    }

                }
                
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public bool OpenAppPOS()
        {
            try
            {
                if (IsProcessOpen("WCF"))
                {
                    Inventec.Common.Logging.LogSystem.Error("OPEN POS 1 ################___________________");
                    return true;
                }
                else
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();

                    startInfo.FileName = Application.StartupPath + @"\Integrate\POS.WCFService\WCF.exe";
                    Inventec.Common.Logging.LogSystem.Info("FileName " + startInfo.FileName);
                    Process.Start(startInfo);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => startInfo), startInfo));
                    return true;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        private void ProcessAddLastAccount()
        {
            System.Threading.Thread add = new System.Threading.Thread(AddLastAccountToLocal);
            try
            {
                add.Start();
            }
            catch (Exception ex)
            {
                add.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddLastAccountToLocal()
        {
            try
            {
                if (GlobalVariables.LastAccountBook == null) GlobalVariables.LastAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    //sổ chưa có sẽ add thêm vào
                    //xóa bỏ sổ cũ cùng loại
                    if (!GlobalVariables.LastAccountBook.Exists(o => o.ID == accountBook.ID))
                    {
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_DEPOSIT == accountBook.IS_FOR_DEPOSIT && o.ID != accountBook.ID).ToList();
                        if (lstSameType != null && lstSameType.Count > 0)
                        {
                            foreach (var item in lstSameType)
                            {
                                GlobalVariables.LastAccountBook.Remove(item);
                            }
                        }
                        GlobalVariables.LastAccountBook.Add(accountBook);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultPayFormForUser()
        {
            try
            {
                var ListPayForm = BackendDataWorker.Get<HIS_PAY_FORM>();
                if (ListPayForm != null && ListPayForm.Count > 0)
                {
                    var PayFormMinByCode = ListPayForm.OrderBy(o => o.PAY_FORM_CODE);
                    var payFormDefault = PayFormMinByCode.FirstOrDefault();
                    if (payFormDefault != null)
                    {
                        var data = ListPayForm.FirstOrDefault(o => o.PAY_FORM_CODE == payFormDefault.PAY_FORM_CODE);
                        if (data != null)
                        {
                            cboPayForm.EditValue = data.ID;
                            //txtPayFormCode.Text = data.PAY_FORM_CODE;
                            CheckPayFormTienMatChuyenKhoan(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValueContronlDepositSuccess()
        {
            try
            {
                if (this.resultTranDeposit != null)
                {
                    txtTotalAmount.Value = this.resultTranDeposit.AMOUNT;
                    txtTransactionCode.Text = this.resultTranDeposit.TRANSACTION_CODE;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuThuTamUng(bool isPrintNow)
        {
            try
            {
                if (isPrintNow)
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuTamUng_MPS000112, ProcessPrintDepositAndClose);
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuTamUng_MPS000112, ProcessPrintDeposit);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessPrintDeposit(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultTranDeposit == null)
                    throw new NullReferenceException("this.resultDeposit = null");

                decimal ratio = 0;
                //MPS.Processor.Mps000112.PDO.PatyAlterBhytADO mpsPatyAlterBHYT = new MPS.Processor.Mps000112.PDO.PatyAlterBhytADO();
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(this.resultTranDeposit.TREATMENT_ID ?? 0, 0, ref PatyAlterBhyt);
                if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.LEVEL_CODE, PatyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0;
                    //Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000112.PDO.PatyAlterBhytADO>(mpsPatyAlterBHYT, PatyAlterBhyt);
                }

                HisDepartmentTranViewFilter departLastFilter = new HisDepartmentTranViewFilter();
                departLastFilter.TREATMENT_ID = this.resultTranDeposit.TREATMENT_ID.Value;
                //departLastFilter.BEFORE_LOG_TIME = transactionPrint.TRANSACTION_TIME;
                var departmentTrans = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departLastFilter, null);

                MPS.Processor.Mps000112.PDO.Mps000112ADO ado = new MPS.Processor.Mps000112.PDO.Mps000112ADO();

                HisTransactionFilter depositFilter = new HisTransactionFilter();
                depositFilter.TREATMENT_ID = this.resultTranDeposit.TREATMENT_ID;
                depositFilter.TRANSACTION_TIME_TO = this.resultTranDeposit.TRANSACTION_TIME;
                var deposit = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, depositFilter, null);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("deposit:________", deposit));

                if (deposit != null && deposit.Count > 0)
                {
                    ado.DEPOSIT_NUM_ORDER = deposit.Where(o => o.IS_CANCEL != 1 && o.IS_DELETE == 0 && o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU).Count();
                    ado.DEPOSIT_SERVICE_NUM_ORDER = deposit.Where(o => o.TDL_SERE_SERV_DEPOSIT_COUNT != null && o.IS_CANCEL != 1 && o.IS_DELETE == 0).Count().ToString();
                }

                V_HIS_TREATMENT treatment = null;
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = this.resultTranDeposit.TREATMENT_ID;
                var treatmentList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null);
                if (treatmentList != null && treatmentList.Count > 0)
                    treatment = treatmentList.First();

                MPS.Processor.Mps000112.PDO.Mps000112PDO rdo = new MPS.Processor.Mps000112.PDO.Mps000112PDO(this.resultTranDeposit, null, ratio, PatyAlterBhyt, departmentTrans, ado, treatment, BackendDataWorker.Get<HIS_TREATMENT_TYPE>());
                MPS.ProcessorBase.Core.PrintData printData = null;

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranDeposit != null ? this.resultTranDeposit.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null);
                    if (isSaveAndSign)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });

                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }     
                }
                else
                {
                    //printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                //result = MPS.MpsPrinter.Run(printData);

                //if (result && chkAutoClose.CheckState == CheckState.Checked)
                //{
                //    this.Close();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessPrintDepositAndClose(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultTranDeposit == null)
                    throw new NullReferenceException("this.resultDeposit = null");

                decimal ratio = 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(this.resultTranDeposit.TREATMENT_ID ?? 0, 0, ref PatyAlterBhyt);
                if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.LEVEL_CODE, PatyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0;
                }

                HisDepartmentTranViewFilter departLastFilter = new HisDepartmentTranViewFilter();
                departLastFilter.TREATMENT_ID = this.resultTranDeposit.TREATMENT_ID.Value;
                var departmentTrans = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departLastFilter, null);

                MPS.Processor.Mps000112.PDO.Mps000112ADO ado = new MPS.Processor.Mps000112.PDO.Mps000112ADO();

                HisTransactionFilter depositFilter = new HisTransactionFilter();
                depositFilter.TREATMENT_ID = this.resultTranDeposit.TREATMENT_ID;
                depositFilter.TRANSACTION_TIME_TO = this.resultTranDeposit.TRANSACTION_TIME;
                depositFilter.TRANSACTION_TYPE_ID = this.resultTranDeposit.TRANSACTION_TYPE_ID;
                var deposit = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, depositFilter, null);
                if (deposit != null && deposit.Count > 0)
                {
                    ado.DEPOSIT_NUM_ORDER = deposit.Where(o => o.IS_CANCEL != 1 && o.IS_DELETE == 0 && o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU).Count();
                    ado.DEPOSIT_SERVICE_NUM_ORDER = deposit.Where(o => o.TDL_SERE_SERV_DEPOSIT_COUNT != null && o.IS_CANCEL != 1 && o.IS_DELETE == 0).Count().ToString();
                }

                V_HIS_TREATMENT treatment = null;
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = this.resultTranDeposit.TREATMENT_ID;
                var treatmentList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null);
                if (treatmentList != null && treatmentList.Count > 0)
                    treatment = treatmentList.First();

                MPS.Processor.Mps000112.PDO.Mps000112PDO rdo = new MPS.Processor.Mps000112.PDO.Mps000112PDO(this.resultTranDeposit, null, ratio, PatyAlterBhyt, departmentTrans, ado, treatment, BackendDataWorker.Get<HIS_TREATMENT_TYPE>());
                //MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranDeposit != null ? this.resultTranDeposit.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                if (isSaveAndSign)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                //result = MPS.MpsPrinter.Run(printData);

                if (result && chkAutoClose.CheckState == CheckState.Checked)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        bool DelegatePrintTempalte(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000171.PDO.PrintTypeCode.Mps000171:
                        InPhieuTamUngVaGiuThe(ref result, printTypeCode, fileName);
                        break;
                    case MPS.Processor.Mps000172.PDO.PrintTypeCode.Mps000172:
                        InPhieuTamUngCoTTBhyt(ref result, printTypeCode, fileName);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuTamUngVaGiuThe(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisTreatmentView1Filter treatmentView1Filter = new HisTreatmentView1Filter();
                treatmentView1Filter.ID = this.resultTranDeposit.TREATMENT_ID;
                var treatment1s = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_1>>("/api/HisTreatment/GetView1", ApiConsumer.ApiConsumers.MosConsumer, treatmentView1Filter, param);
                if (treatment1s == null || treatment1s.Count == 0)
                {
                    return;
                }

                var treatment1 = treatment1s.FirstOrDefault();

                HisPatientTypeAlterViewAppliedFilter appFilter = new HisPatientTypeAlterViewAppliedFilter();
                long nowTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (treatment1.IN_TIME > nowTime)
                {
                    appFilter.InstructionTime = treatment1.IN_TIME;
                }
                else
                {
                    appFilter.InstructionTime = nowTime;
                }
                appFilter.TreatmentId = this.resultTranDeposit.TREATMENT_ID ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, appFilter, param);
                if (currentPatientTypeAlter == null)
                {
                    throw new Exception("Khong lay duoc thong tin doi tuong cua ho so dieu tri Id: " + this.resultTranDeposit.TREATMENT_ID);
                }
                //HIS_PATY_ALTER_BHYT patyAlter = null;
                //if (currentPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                //{
                //    HisPatyAlterBhytFilter patyBhytFilter = new HisPatyAlterBhytFilter();
                //    patyBhytFilter.PATIENT_TYPE_ALTER_ID = currentPatientTypeAlter.ID;
                //    var listPatyAlterBhyt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_PATY_ALTER_BHYT>>("api/HisPatyAlterBhyt/Get", ApiConsumers.MosConsumer, patyBhytFilter, param);
                //    if (listPatyAlterBhyt == null || listPatyAlterBhyt.Count != 1)
                //    {
                //        throw new Exception("Khong lay duoc thong tin the BHYT cua ho so dieu tri Id: " + this.resultDeposit.TREATMENT_ID);
                //    }
                //    patyAlter = listPatyAlterBhyt.FirstOrDefault();
                //}

                decimal ratio = 0;
                if (currentPatientTypeAlter != null)
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0;
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.resultTranDeposit.TREATMENT_ID ?? 0;
                if (treatment1.IN_TIME > nowTime)
                {
                    departLastFilter.BEFORE_LOG_TIME = treatment1.IN_TIME;
                }
                else
                {
                    departLastFilter.BEFORE_LOG_TIME = nowTime;
                }
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, param);

                HIS_PATIENT patient = new HIS_PATIENT();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = this.resultTranDeposit.TDL_PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.First();
                }

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranDeposit != null ? this.resultTranDeposit.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000171.PDO.Mps000171PDO pdo = new MPS.Processor.Mps000171.PDO.Mps000171PDO(this.resultTranDeposit, currentPatientTypeAlter, departmentTran, ratio, patient);
                MPS.ProcessorBase.Core.PrintData printData = null;
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {

                    //printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    //printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
                //result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        private void InPhieuTamUngCoTTBhyt(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisTreatmentView1Filter treatmentView1Filter = new HisTreatmentView1Filter();
                treatmentView1Filter.ID = this.resultTranDeposit.TREATMENT_ID;
                var treatment1s = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_1>>("/api/HisTreatment/GetView1", ApiConsumer.ApiConsumers.MosConsumer, treatmentView1Filter, param);
                if (treatment1s == null || treatment1s.Count == 0)
                {
                    return;
                }

                var treatment1 = treatment1s.FirstOrDefault();
                HisPatientTypeAlterViewAppliedFilter appFilter = new HisPatientTypeAlterViewAppliedFilter();
                long nowTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (treatment1.IN_TIME > nowTime)
                {
                    appFilter.InstructionTime = treatment1.IN_TIME;
                }
                else
                {
                    appFilter.InstructionTime = nowTime;
                }
                appFilter.TreatmentId = this.resultTranDeposit.TREATMENT_ID ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, appFilter, param);
                if (currentPatientTypeAlter == null)
                {
                    throw new Exception("Khong lay duoc thong tin doi tuong cua ho so dieu tri Id: " + this.resultTranDeposit.TREATMENT_ID);
                }
                //HIS_PATY_ALTER_BHYT patyAlter = null;
                //if (currentPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                //{
                //    HisPatyAlterBhytFilter patyBhytFilter = new HisPatyAlterBhytFilter();
                //    patyBhytFilter.PATIENT_TYPE_ALTER_ID = currentPatientTypeAlter.ID;
                //    var listPatyAlterBhyt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_PATY_ALTER_BHYT>>("api/HisPatyAlterBhyt/Get", ApiConsumers.MosConsumer, patyBhytFilter, param);
                //    if (listPatyAlterBhyt == null || listPatyAlterBhyt.Count != 1)
                //    {
                //        throw new Exception("Khong lay duoc thong tin the BHYT cua ho so dieu tri Id: " + this.resultDeposit.TREATMENT_ID);
                //    }
                //    patyAlter = listPatyAlterBhyt.FirstOrDefault();
                //}

                decimal ratio = 0;
                if (currentPatientTypeAlter != null)
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0;
                }

                HisDepartmentTranViewFilter departLastFilter = new HisDepartmentTranViewFilter();
                departLastFilter.TREATMENT_ID = this.resultTranDeposit.TREATMENT_ID ?? 0;
                departLastFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var departmentTrans = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departLastFilter, param);

                HIS_PATIENT patient = new HIS_PATIENT();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = this.resultTranDeposit.TDL_PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.First();
                }

                MPS.Processor.Mps000172.PDO.Mps000172PDO pdo = new MPS.Processor.Mps000172.PDO.Mps000172PDO(this.resultTranDeposit, currentPatientTypeAlter, departmentTrans, ratio, patient);
                MPS.ProcessorBase.Core.PrintData printData = null;
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranDeposit != null ? this.resultTranDeposit.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    //printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
                //result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                //Button
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__BTN_NEW", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__BTN_SAVE", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSavePrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__BTN_SAVE_PRINT", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__BTN_PRINT", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmTransactionDeposit.btnSave.Text", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                //Layout
                this.layoutAccountBook.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_ACCOUNT_BOOK", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_DESCRIPTION", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutPayForm.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_PAY_FORM", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TONG_TU_DEN", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTotalAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TOTAL_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTransactionCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTransactionTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSACTION_TIME", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_PATIENT_CODE", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_PATIENT_NAME", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciGenderName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_GENDER_NAME", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciDob.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_DOB", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciAddress.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_ADDRESS", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtTreatmenCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TXT_TREATMENT_CODE_NULL_VALUE_PROMPT", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtDepositReqCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TXT_DEPOSIT_REQ_CODE_NULL_VALUE_PROMPT", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionDeposit, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmTransactionDeposit.layoutControlItem5.Text", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.layoutControlItem6.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionDeposit.layoutControlItem6.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmTransactionDeposit.layoutControlItem6.Text", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.chkAutoClose.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionDeposit.chkAutoClose.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.chkAutoClose.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionDeposit.chkAutoClose.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());

                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("frmTransactionDeposit.ddBtnPrint.Text", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmTransactionDeposit.btnSearch.Text", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTransactionDeposit.Text", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());




            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransactionTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (txtTransactionCode.Enabled)
                    {
                        txtTransactionCode.Focus();
                        txtTransactionCode.SelectAll();
                    }
                    else
                    {
                        cboAccountBook.Focus();
                        cboAccountBook.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTongTuDen_ValueChanged(object sender, EventArgs e)
        {

        }

        private void spinTongTuDen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboAccountBook.EditValue != null)
                    {
                        var accountBook = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue.ToString()));
                        UpdateDictionaryNumOrderAccountBook(accountBook);
                    }
                    cboPayForm.Focus();
                    cboPayForm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (cboAccountBook.EditValue != null && e.KeyCode == Keys.Enter)
                {
                    var accountBook = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue.ToString()));
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTongTuDen_Spin(object sender, SpinEventArgs e)
        {
            try
            {
                if (cboAccountBook.EditValue != null)
                {
                    var accountBook = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue.ToString()));
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                }
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
                if (cboAccountBook.EditValue != null)
                {
                    var account = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    if (account != null)
                    {
                        SetDataToDicNumOrderInAccountBook(account);
                    }
                }
                else
                {
                    spinTongTuDen.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmenCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepositReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.LoadAccountBookToLocal(true);
                this.LoadDataToComboPayForm();

                this.ResetDefaultValueControl();

                this.treatment = null;
                this.depositReq = null;  
                if (!String.IsNullOrEmpty(txtTreatmenCode.Text))
                {
                    LoadSearchByTreatmentCode();
                    if (checkDK())
                    {
                        FillDataToCommon(this.treatment);
                        btnSavePrint.Enabled = true;
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnSavePrint.Enabled = false;
                        btnSave.Enabled = false;
                    }
                    
                }
                else
                {
                    LoadSearchByDepositReqCode();
                    if (checkDK())
                    {
                        FillDataToCommon(this.depositReq);
                        btnSavePrint.Enabled = true;
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnSavePrint.Enabled = false;
                        btnSave.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnTxtTreatmentCodeFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                txtTreatmenCode.Focus();
                txtTreatmenCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTotalAmount.Focus();
                    txtTotalAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void txtTotalAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FormatSpint(txtTotalAmount);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTransferAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FormatSpint(spinTransferAmount);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAutoClose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhilechkAutoCloseStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoClose.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoClose.CheckState.ToString());
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoClose.Name;
                    csAddOrUpdate.VALUE = (chkAutoClose.CheckState.ToString());
                    csAddOrUpdate.MODULE_LINK = moduleLink;
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
        private void InitControlState()
        {
            isNotLoadWhilechkAutoCloseStateInFirst = true;
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkAutoClose.Name)
                        {
                            switch (item.VALUE)
                            {
                                case "Unchecked":
                                    {
                                        chkAutoClose.CheckState = CheckState.Unchecked;
                                        break;
                                    }
                                case "Checked":
                                    {
                                        chkAutoClose.CheckState = CheckState.Checked;
                                        break;
                                    }
                                case "Indeterminate":
                                    {
                                        chkAutoClose.CheckState = CheckState.Indeterminate;
                                        break;
                                    }
                            }
                        }
                        else if (item.KEY == chkConnectionPOS.Name)
                        {
                            chkConnectionPOS.Checked = item.VALUE == "1";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhilechkAutoCloseStateInFirst = false;
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void chkConnectionPOS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkConnectionPOS.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkConnectionPOS.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkConnectionPOS.Name;
                    csAddOrUpdate.VALUE = (chkConnectionPOS.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
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

        private void btnConfigPos_Click(object sender, EventArgs e)
        {
            try
            {
                OpenAppPOS();
                try
                {
                    cll = new WcfClient();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    XtraMessageBox.Show("Kiểm tra lại cấu hình NetTcpBinding_IService1", "Thông báo");
                    return;
                }
                cll.cauhinh();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Cấu hình thất bại", "Thông báo");
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTransactionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTransactionCode.Focus();
                    txtTransactionCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnQR_Click(object sender, EventArgs e)
        {
            try
            {
                if(resultTranDeposit != null && resultTranDeposit.PAY_FORM_ID == 8 && resultTranDeposit.IS_ACTIVE == 0) CreateQR(resultTranDeposit, true);
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
                isSaveAndSign = true;
                positionHandleControl = -1;
                if (!btnSaveAndSign.Enabled || !dxValidationProvider1.Validate() || this.treatment == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                this.SaveDeposit(param, ref success);
                if (success)
                {
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                    btnSaveAndSign.Enabled = false;
                    btnSavePrint.Enabled = false;
                }
                WaitingManager.Hide();
                if (!success)
                {
                    if (!isShowMess)
                    {
                        MessageManager.Show(param, success);
                    }
                }
                else
                {
                    this.InPhieuThuTamUng(true);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
