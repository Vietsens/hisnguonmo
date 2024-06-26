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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.TransactionDebtCollect.ADO;
using HIS.Desktop.Plugins.TransactionDebtCollect.Base;
using HIS.Desktop.Plugins.TransactionDebtCollect.Config;
using HIS.Desktop.Plugins.TransactionDebtCollect.Validtion;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.HcmPoorFund;
using MOS.SDO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionDebtCollect
{
    public partial class frmTransactionDebtCollect : HIS.Desktop.Utility.FormBase
    {

        private const string SIGNED_EXTENSION = ".pdf";

        Inventec.Desktop.Common.Modules.Module currentModule = null;

        V_HIS_TRANSACTION resultTranBill = null;
        List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        V_HIS_CASHIER_ROOM cashierRoom;
        long? treatmentId = null;
        V_HIS_TREATMENT_FEE currentTreatment = null;
        private int positionHandleControl = -1;
        bool isInit = true;
        string departmentName = "";
        internal string statusTreatmentOut { get; set; }
        decimal totalPatientPrice = 0;
        V_HIS_PATIENT_TYPE_ALTER resultPatientType;
        List<TransactionADO> ListTransactionADO;
        List<TransactionADO> listData;
        HIS_BRANCH branch = null;
        string userName = "";
        bool isCheckAll;
        List<long> _ListTransactionId = new List<long>();
        RefeshReference refeshReference = null;
        decimal totalDiscount = 0;

        public frmTransactionDebtCollect(Inventec.Desktop.Common.Modules.Module module, List<long> ListTransactionId)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 1");
            InitializeComponent();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 2");
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.currentModule = module;
                if (ListTransactionId != null && ListTransactionId.Count > 0)
                {
                    this._ListTransactionId = ListTransactionId;
                    Inventec.Common.Logging.LogSystem.Debug("ListTransactionId: " + Inventec.Common.Logging.LogUtil.TraceData("", ListTransactionId));
                }

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 3");
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionDebtCollect(Inventec.Desktop.Common.Modules.Module module, List<long> ListTransactionId, long _treatmentId)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 1");
            InitializeComponent();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 2");
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.currentModule = module;
                this.treatmentId = _treatmentId;
                if (ListTransactionId != null && ListTransactionId.Count > 0)
                {
                    this._ListTransactionId = ListTransactionId;
                    Inventec.Common.Logging.LogSystem.Debug("ListTransactionId: " + Inventec.Common.Logging.LogUtil.TraceData("", ListTransactionId));
                }

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 3");
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionDebtCollect(Inventec.Desktop.Common.Modules.Module module, List<long> ListTransactionId, long _treatmentId, RefeshReference _refreshReference)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 1");
            InitializeComponent();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 2");
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.currentModule = module;
                this.treatmentId = _treatmentId;
                this.refeshReference = _refreshReference;
                if (ListTransactionId != null && ListTransactionId.Count > 0)
                {
                    this._ListTransactionId = ListTransactionId;
                    Inventec.Common.Logging.LogSystem.Debug("ListTransactionId: " + Inventec.Common.Logging.LogUtil.TraceData("", ListTransactionId));
                }

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 3");
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionDebtCollect(Inventec.Desktop.Common.Modules.Module module, V_HIS_TREATMENT_FEE data)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 1");
            InitializeComponent();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 2");
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.currentModule = module;
                if (data != null)
                {
                    this.treatmentId = data.ID;
                    this.currentTreatment = data;
                }

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 3");
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionDebtCollect(Inventec.Desktop.Common.Modules.Module module, long treatmentId)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 1");
            InitializeComponent();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 2");
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.currentModule = module;
                this.treatmentId = treatmentId;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 3");
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionDebtCollect(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.3. 1");
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.3. 2");
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.3. 3");
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

        private void timerInitForm_Tick()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                StopTimer(GetModuleLink(), "timerInitForm");

                this.ListTransactionADO = new List<TransactionADO>();
                this.LoadDataToGridViewSereServ();//TODO
                gridControlSereServDebt.DataSource = ListTransactionADO;
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                this.FillInfoPatient(this.currentTreatment);
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 2");
                this.CalcuTotalPrice();
                this.CalcuCanThu();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionDebt_Load(object sender, EventArgs e)
        {
            try
            {
                gridColumnCheck.Image = imageCollectionMediStock.Images[0];
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 1");
                WaitingManager.Show();
                this.LoadKeyFrmLanguage();
                HisConfigCFG.LoadConfig();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 2");
                this.LoadCashierRoomAndBranch();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 3");
                this.ValidControl();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 4");
                this.LoadAccountBookToLocal();
                this.LoadDataToComboPayForm();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 5");
                this.GeneratePopupMenu();
                this.ResetControlValue();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 6");
                this.cboAccountBook.Focus();
                if (this.currentTreatment.TREATMENT_CODE != null)
                {
                    this.txtFindTreatmentCode.Text = this.currentTreatment.TREATMENT_CODE;
                    this.txtFindTreatmentCode.SelectionStart = this.txtFindTreatmentCode.Text.Length;
                    this.txtFindTreatmentCode.DeselectAll();
                }

                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 7");
                isInit = false;
                timerInitForm.Interval = 100;
                timerInitForm.Enabled = true;
                RegisterTimer(GetModuleLink(), "timerInitForm", timerInitForm.Interval, timerInitForm_Tick);
                StartTimer(GetModuleLink(), "timerInitForm");
                

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataToComboPayForm()
        {
            try
            {
                List<HIS_PAY_FORM> lData = null;
                if (BackendDataWorker.IsExistsKey<HIS_PAY_FORM>())
                {
                    lData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    lData = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>>("api/HisPayForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (lData != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM), lData, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                cboPayForm.Properties.DataSource = lData;
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

                var PayFormMinByCode = lData.OrderBy(o => o.PAY_FORM_CODE);
                var payFormDefault = PayFormMinByCode.FirstOrDefault();
                if (payFormDefault != null)
                {
                    cboPayForm.EditValue = payFormDefault.ID;
                    CheckPayFormTienMatChuyenKhoan(payFormDefault);
                }
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

        private void CheckPayFormTienMatChuyenKhoan(HIS_PAY_FORM payForm)
        {
            try
            {
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LCI_TRANFER_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LCI_TRANFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;

                }
                else if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LCI_SWIPE_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LCI_SWIPE_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;

                }
                else
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(false);
                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LCI_TRANFER_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LCI_TRANFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTranferAmount.Enabled = false;
                }
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

        private void ValidControlPayForm()
        {
            try
            {
                PayFormValidationRule payFormRule = new PayFormValidationRule();
                payFormRule.cboPayForm = cboPayForm;
                dxValidationProvider1.SetValidationRule(cboPayForm, payFormRule);
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

        private void SetDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            try
            {
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                        }

                        layoutTongTuDen.Enabled = true;
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new HisAccountBookViewFilter();
                        hisAccountBookViewFilter.ID = accountBook.ID;
                        var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                        if (accountBooks != null && accountBooks.Count > 0)
                        {
                            var accountBookNew = accountBooks.FirstOrDefault();
                            decimal num = 0;
                            if ((accountBookNew.CURRENT_NUM_ORDER ?? 0) > 0)
                            {
                                num = accountBookNew.CURRENT_NUM_ORDER ?? 0;
                            }
                            else
                            {
                                num = (decimal)accountBookNew.FROM_NUM_ORDER - 1;
                            }

                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
                            spinTongTuDen.Value = ((HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1);
                        }
                    }
                    else
                    {
                        layoutTongTuDen.Enabled = true;
                        spinTongTuDen.Value = ((HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1);
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

        private void LoadCashierRoomAndBranch()
        {
            try
            {
                if (this.currentModule != null)
                {
                    this.cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId && o.ROOM_TYPE_ID == currentModule.RoomTypeId);
                    if (cashierRoom != null)
                    {
                        departmentName = cashierRoom.DEPARTMENT_NAME;
                    }

                    branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
                }

                if (this.currentTreatment == null || this.currentTreatment.ID == 0)
                {
                    if (this.treatmentId.HasValue)
                    {
                        HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
                        feeFilter.ID = this.treatmentId.Value;
                        var treatmentFees = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetView2", ApiConsumers.MosConsumer, feeFilter, null);
                        if (treatmentFees == null || treatmentFees.Count == 0)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc treatmentFee theo TreatmentId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId));
                            return;
                        }

                        this.currentTreatment = treatmentFees.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAccountBookToLocal()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                //Sửa lại đoạn code này
                //Api bổ sung filter chứ không get nhiều api
                //TODO               
                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.CASHIER_ROOM_ID = this.cashierRoom.ID;//Kiểm tra sổ còn hay k
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                acFilter.FOR_BILL = true;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                ListAccountBook = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                LoadDataToComboAccountBook();
                SetDefaultAccountBook();//TODO
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

                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.Last();
                    }
                }

                if (accountBook != null)
                {
                    cboAccountBook.EditValue = accountBook.ID;
                    SetDataToDicNumOrderInAccountBook(accountBook);
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

        private void LoadDataToGridViewSereServ()
        {
            try
            {
                if (this.treatmentId.HasValue)
                {
                    MOS.Filter.HisTransactionFilter transactionFilter = new HisTransactionFilter();
                    transactionFilter.TREATMENT_ID = this.treatmentId.Value;
                    transactionFilter.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO;
                    transactionFilter.DEBT_BILL_ID = null;
                    transactionFilter.IS_CANCEL = false;
                    var ListTransactionApi = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumer.ApiConsumers.MosConsumer, transactionFilter, null);
                    if (ListTransactionApi != null && ListTransactionApi.Count > 0)
                    {
                        foreach (var item in ListTransactionApi)
                        {
                            if (item.DEBT_BILL_ID.HasValue)
                                continue;

                            TransactionADO ado = new TransactionADO();
                            AutoMapper.Mapper.CreateMap<HIS_TRANSACTION, TransactionADO>();
                            ado = AutoMapper.Mapper.Map<TransactionADO>(item);
                            if (_ListTransactionId != null && _ListTransactionId.Count > 0)
                            {
                                var checkTranfer = _ListTransactionId.Where(o => o == item.ID).ToList();
                                if (checkTranfer != null && checkTranfer.Count > 0)
                                    ado.Check = true;
                            }

                            this.ListTransactionADO.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalcuTotalPrice()
        {
            try
            {
                totalPatientPrice = 0;
                List<TransactionADO> listData = new List<TransactionADO>();
                List<TransactionADO> ListAll = (List<TransactionADO>)gridControlSereServDebt.DataSource;
                if (ListAll == null || ListAll.Count == 0)
                {
                    totalPatientPrice = 0;
                    lblTotalExemtion.Text = "0";
                    lblTotalPrice.Text = "0";
                }
                else
                {
                    listData = ListAll.Where(o => o.Check).ToList();
                }

                if (listData != null)
                {
                    totalPatientPrice = listData.Sum(o => o.AMOUNT) - listData.Sum(o => o.EXEMPTION ?? 0);
                    lblTotalExemtion.Text = ConvertNumberToString(listData.Sum(o => o.EXEMPTION ?? 0));
                    lblTotalPrice.Text = ConvertNumberToString(listData.Sum(o => o.AMOUNT));
                }

                txtTotalAmount.Value = totalPatientPrice;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlValue()
        {
            try
            {
                resultTranBill = null;
                totalPatientPrice = 0;
                dxValidationProvider1.RemoveControlError(dtTransactionTime);

                spinTongTuDen.Value = 0;
                //SetDefaultAccountBook();//TODO
                //SetDefaultPayFormForUser();//TODO
                txtDescription.Text = "";
                txtTotalAmount.Value = 0;
                txtTransactionCode.Text = "";
                spinTransferAmount.EditValue = null;
                btnNew.Enabled = true;
                btnSave.Enabled = true;
                spinAmountBNDua.Value = 0;
                spinAmountTraBN.Value = 0;
                lciBtnSave.Enabled = true;
                btnSavePrint.Enabled = true;
                ddBtnPrint.Enabled = false;
                dtTransactionTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetFillPatientDefault()
        {
            try
            {
                txtPatient.Text = "";
                txtPatientName.Text = "";
                txtDOB.Text = "";
                txtGender.Text = "";
                txtAddress.Text = "";
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
                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.DefaultAccountBookDebtCollect != null && GlobalVariables.DefaultAccountBookDebtCollect.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.DefaultAccountBookDebtCollect.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }

                if (accountBook != null)
                {
                    cboAccountBook.EditValue = accountBook.ID;
                    SetDataToDicNumOrderInAccountBook(accountBook);
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

        private void ValidControl()
        {
            try
            {
                ValidControlAccountBook();
                ValidControlTransactionTime();
                ValidControlVAT();
                ValidControlDescription();
                ValidControlPayForm();
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

        private void ValidControlAccountBook()
        {
            try
            {
                AccountBookValidationRule accBookRule = new AccountBookValidationRule();
                accBookRule.cboAccountBook = cboAccountBook;
                dxValidationProvider1.SetValidationRule(cboAccountBook, accBookRule);
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
                TransactionTimeValidationRule transactionTimeRule = new TransactionTimeValidationRule();
                transactionTimeRule.dtTransactionTime = dtTransactionTime;
                dxValidationProvider1.SetValidationRule(dtTransactionTime, transactionTimeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlVAT()
        {
            try
            {
                SpinVATValidationRule transactionTimeRule = new SpinVATValidationRule();
                transactionTimeRule.spinVAT = spinDiscountRatio;
                dxValidationProvider1.SetValidationRule(spinDiscountRatio, transactionTimeRule);
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

        private void LoadKeyFrmLanguage()
        {
            try
            {
                //Button
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__BTN_NEW", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__BTN_SAVE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__BTN_PRINT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__BTN_SEARCH", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Layout
                this.layoutAccountBook.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_ACCOUNT_BOOK", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_DESCRIPTION", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_TONG_TU_DEN", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTotalAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_TOTAL_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTransactionCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTotalPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_TOTAL_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTotalExemption.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_TOTAL_EXEMPTION", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTotalExemption.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_TOTAL_EXEMPTION_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciReceiveAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_RECEIVER_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciAmountBNDua.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_AMOUNT_BN_DUA", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciAmountBNDua.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_AMOUNT_BN_DUA_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciAmountTraBN.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_AMOUNT_TRA_BN", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciAmountTraBN.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_AMOUNT_TRA_BN_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Repository Button
                this.layoutTransactionTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__TRANSACTION_TIME", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTransactionTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__TRANSACTION_TIME__TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //InfoPatient
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__layoutControlItem13", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__layoutControlItem15", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__layoutControlItem21", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__layoutControlItem20", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__layoutControlItem16", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__btnSearch", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LCI_TRANFER_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LCI_TRANFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumnSereServDebtSTT.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_STT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumnSereServDebtTransactionCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumnSereServDebtAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumnSereServDebtExemption.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_EXEMPTION", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumnSereServDebtCashierUserName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_CASHIER_USERNAME", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumnSereServDebtTransactionTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_TRANSACTION_TIME", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                isInit = true;
                treatmentId = null;
                currentTreatment = null;
                //resultPatientType = null;
                ResetFillPatientDefault();
                ResetControlValue();

                this.ListTransactionADO = new List<TransactionADO>();
                if (!String.IsNullOrWhiteSpace(txtMaCongNo.Text))
                {
                    LoadDataToGridViewSereServByTransactionCode();
                }
                else if (!String.IsNullOrWhiteSpace(txtFindTreatmentCode.Text))
                {
                    LoadSearch();
                    LoadDataToGridViewSereServ();
                    FillInfoPatient(currentTreatment);
                }

                gridControlSereServDebt.DataSource = ListTransactionADO;

                LoadAccountBookToLocal();
                //LoadDataToComboAccountBook();
                SetDefaultPayFormForUser();
                //SetDefaultAccountBook();//TODO
                //LoadDataForBordereau();
                CalcuTotalPrice();
                CalcuCanThu();
                txtTotalAmount.Value = this.totalPatientPrice;
                cboAccountBook.Focus();
                isInit = false;
                this._ListTransactionId = new List<long>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridViewSereServByTransactionCode()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTransactionFilter filter = new HisTransactionFilter();
                if (!String.IsNullOrEmpty(txtMaCongNo.Text))
                {
                    string code = txtMaCongNo.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtMaCongNo.Text = code;
                    }

                    filter.TRANSACTION_CODE__EXACT = code;
                    filter.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO;
                    filter.DEBT_BILL_ID = null;
                    filter.IS_CANCEL = false;
                    var ListTransactionApi = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (ListTransactionApi != null && ListTransactionApi.Count > 0)
                    {
                        foreach (var item in ListTransactionApi)
                        {
                            if (item.DEBT_BILL_ID.HasValue)
                                continue;

                            TransactionADO ado = new TransactionADO();
                            AutoMapper.Mapper.CreateMap<HIS_TRANSACTION, TransactionADO>();
                            ado = AutoMapper.Mapper.Map<TransactionADO>(item);
                            if (_ListTransactionId != null && _ListTransactionId.Count > 0)
                            {
                                var checkTranfer = _ListTransactionId.Where(o => o == item.ID).ToList();
                                if (checkTranfer != null && checkTranfer.Count > 0)
                                    ado.Check = true;
                            }

                            this.ListTransactionADO.Add(ado);
                        }

                        HisTreatmentFeeViewFilter treatFilter = new HisTreatmentFeeViewFilter();
                        treatFilter.ID = ListTransactionApi.First().TREATMENT_ID ?? 0;
                        var listTreatment = new BackendAdapter(param)
                                   .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, treatFilter, param);
                        if (listTreatment != null && listTreatment.Count > 0)
                        {
                            currentTreatment = listTreatment.FirstOrDefault();
                            treatmentId = currentTreatment.ID;
                            FillInfoPatient(currentTreatment);
                        }
                        else
                        {
                            FillInfoPatient(ListTransactionApi.First());
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.KhongTimThayMaCongNo);
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.KhongTimThayMaCongNo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckPayformCard(MOS.EFMODEL.DataModels.HIS_PAY_FORM payForm)
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

        private void LoadSearch()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                if (!String.IsNullOrEmpty(txtFindTreatmentCode.Text))
                {
                    string code = txtFindTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtFindTreatmentCode.Text = code;
                    }

                    filter.TREATMENT_CODE__EXACT = code;
                }
                //else if (_ListTransactionId != null && _ListTransactionId.Count > 0)
                //{
                //    MOS.Filter.HisTransactionFilter transactionFIlter = new HisTransactionFilter();
                //    transactionFIlter.ID = _ListTransactionId.FirstOrDefault();
                //    var transaction = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumer.ApiConsumers.MosConsumer, transactionFIlter, null).FirstOrDefault();
                //    if (transaction != null)
                //    {
                //        filter.TREATMENT_CODE__EXACT = transaction.TDL_TREATMENT_CODE;
                //    }
                //}

                if (String.IsNullOrEmpty(filter.TREATMENT_CODE__EXACT))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.KhongTimThayMaDieuTri);
                    //param.Messages.Add(Base.ResourceMessageLang.KhongTimThayMaDieuTri);
                    return;
                }

                var listTreatment = new BackendAdapter(param)
                           .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    currentTreatment = listTreatment.FirstOrDefault();
                    treatmentId = currentTreatment.ID;

                    Inventec.Common.Logging.LogSystem.Debug("LoadSearch: " + Inventec.Common.Logging.LogUtil.TraceData("", currentTreatment.TREATMENT_CODE));
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.KhongTimThayMaDieuTri);
                    //param.Messages.Add(Base.ResourceMessageLang.KhongTimThayMaDieuTri);
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormatGridcol(DevExpress.XtraGrid.Columns.GridColumn grdColName, decimal number)
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
                    if (number != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToDecimal(number.ToString() ?? "") % 1 > 0)
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

                grdColName.DisplayFormat.FormatString = Format;
                grdColName.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFindTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtFindTreatmentCode.Text))
                    {
                        txtMaCongNo.Text = "";
                        btnSearch.Focus();
                    }
                    else
                    {
                        txtMaCongNo.Focus();
                        txtMaCongNo.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

                    if (layoutTotalAmount.Enabled)
                    {
                        txtTotalAmount.Focus();
                        txtTotalAmount.SelectAll();
                    }
                    //else if (lciTranferAmount.Enabled)
                    //{
                    //    spinTransferAmount.Focus();
                    //    spinTransferAmount.SelectAll();
                    //}
                    //else
                    //{
                    //    txtDiscount.Focus();
                    //    txtDiscount.SelectAll();
                    //}
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

        private void frmTransactionBill_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {

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
                ddBtnPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServDebt_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (TransactionADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    FormatGridcol(gridColumnSereServDebtAmount, data.AMOUNT);
                    FormatGridcol(gridColumnSereServDebtExemption, (data.EXEMPTION ?? 0));
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                        else if (e.Column.FieldName == "TRANSACTION_TIME_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRANSACTION_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void CalcuCanThu()
        {
            try
            {
                decimal canthuAmount = 0;
                decimal SoTienChuyenKhoan = 0;
                if (spinTransferAmount.EditValue != null)
                {
                    SoTienChuyenKhoan = Inventec.Common.TypeConvert.Parse.ToDecimal(spinTransferAmount.Text.Trim());
                }

                canthuAmount = totalPatientPrice - totalDiscount - SoTienChuyenKhoan;
                lblReceiveAmount.Text = Inventec.Common.Number.Convert.NumberToString(((totalPatientPrice - SoTienChuyenKhoan - this.totalDiscount)), ConfigApplications.NumberSeperator);
                spinAmountTraBN.Value = (spinAmountBNDua.Value - canthuAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransactionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinTongTuDen.Enabled)
                    {
                        spinTongTuDen.Focus();
                        spinTongTuDen.SelectAll();
                    }
                    else
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServDebt_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                CalcuTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServDebt_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "Check")
                        {
                            var lstCheckAll = this.ListTransactionADO;
                            List<TransactionADO> lstChecks = new List<TransactionADO>();
                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var MediStockCheckedNum = lstCheckAll.Where(o => o.Check).Count();
                                var MediStocktmNum = lstCheckAll.Count();
                                if ((MediStockCheckedNum > 0 && MediStockCheckedNum < MediStocktmNum) || MediStockCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionMediStock.Images[1];
                                }

                                if (MediStockCheckedNum == MediStocktmNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionMediStock.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.Check = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }

                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.Check = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }

                                    isCheckAll = true;
                                }

                                gridControlSereServDebt.BeginUpdate();
                                gridControlSereServDebt.DataSource = null;
                                gridControlSereServDebt.DataSource = lstChecks;
                                gridControlSereServDebt.EndUpdate();
                            }
                        }
                    }

                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            int rowHandle = gridViewSereServDebt.GetVisibleRowHandle(hi.RowHandle);
                            var dataRow = (TransactionADO)gridViewSereServDebt.GetRow(rowHandle);
                            if (dataRow != null)
                            {
                                //if (hi.Column.FieldName == "Check" && ((dataRow.VIR_TOTAL_PATIENT_PRICE ?? 0) - (dataRow.TOTAL_DEBT_PRICE ?? 0) <= 0))
                                //{
                                //    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                                //    return;
                                //}
                            }

                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            if (checkEdit == null)
                                return;

                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo1 = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo1.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo1.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }

                    CalcuTotalPrice();
                    CalcuCanThu();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServDebt_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Check")
                {
                    //CalcuTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServDebt_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (TransactionADO)gridViewSereServDebt.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "Check")
                        {
                            try
                            {
                                if (data.AMOUNT <= 0)
                                {
                                    e.RepositoryItem = CheckEditDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = CheckEditEnable;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void spinTransferAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalcuCanThu();
                FormatSpint(spinTransferAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    HIS_PAY_FORM payForm = null;
                    if (cboPayForm.EditValue != null)
                    {
                        payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                        if (payForm != null)
                        {
                            dtTransactionTime.Focus();
                            dtTransactionTime.ShowPopup();
                        }
                    }
                    CheckPayFormTienMatChuyenKhoan(payForm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinAmountBNDua_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.CalcuCanThu();
                FormatSpint(spinAmountBNDua);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTotalAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FormatSpint(txtTotalAmount);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinAmountTraBN_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FormatSpint(spinAmountTraBN);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spinDiscount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinDiscount.EditValue != null)
                    {
                        this.totalDiscount = spinDiscount.Value;
                        if (this.totalPatientPrice > 0)
                        {
                            spinDiscountRatio.EditValue = (this.totalDiscount / this.totalPatientPrice) * 100;
                        }
                    }
                    else
                    {
                        this.totalDiscount = 0;
                        spinDiscountRatio.EditValue = null;
                    }

                    CalcuCanThu();
                    spinDiscountRatio.Focus();
                    spinDiscountRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscountRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinDiscountRatio.EditValue != null)
                    {
                        var ratio = spinDiscountRatio.Value / 100;
                        this.totalDiscount = this.totalPatientPrice * ratio;
                        spinDiscount.Value = this.totalDiscount;
                    }
                    else
                    {
                        this.totalDiscount = 0;
                        spinDiscount.Value = 0;
                        spinDiscount.EditValue = null;
                    }

                    CalcuCanThu();
                    spinAmountBNDua.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMaCongNo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !String.IsNullOrWhiteSpace(txtMaCongNo.Text))
                    btnSearch.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonFocusCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtFindTreatmentCode.Focus();
                txtFindTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
