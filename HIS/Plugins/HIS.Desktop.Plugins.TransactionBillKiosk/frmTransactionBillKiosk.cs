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
using HIS.Desktop.Plugins.TransactionBillKiosk.ADO;
using HIS.Desktop.Plugins.TransactionBillKiosk.Base;
using HIS.Desktop.Plugins.TransactionBillKiosk.Config;
using HIS.Desktop.Plugins.TransactionBillKiosk.Validtion;
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

namespace HIS.Desktop.Plugins.TransactionBillKiosk
{
    public partial class frmTransactionBillKiosk : HIS.Desktop.Utility.FormBase
    {

        private const string SIGNED_EXTENSION = ".pdf";

        Inventec.Desktop.Common.Modules.Module currentModule = null;

        V_HIS_TRANSACTION resultTranBill = null;
        List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        HIS_CASHIER_ROOM cashierRoom;
        long treatmentId = 0;
        V_HIS_TREATMENT_FEE currentTreatment = null;
        private int positionHandleControl = -1;
        bool isInit = true;
        string departmentName = "";
        internal string statusTreatmentOut { get; set; }
        decimal totalPatientPrice = 0;
        V_HIS_PATIENT_TYPE_ALTER resultPatientType;
        List<HIS_SERE_SERV> listData;
        HIS_BRANCH branch = null;
        string userName = "";
        RefeshReference refeshReference = null;
        //List<HIS_SERE_SERV> ListSereServ;
        V_HIS_ACCOUNT_BOOK accountBook = null;
        bool isMaximize = false;
        DelegateCloseForm_Uc DelegateClose;
        System.Threading.Thread CloseThread;
        int loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
        private bool stopThread;

        public frmTransactionBillKiosk(Inventec.Desktop.Common.Modules.Module module, long _treatmentId, HIS_CASHIER_ROOM cashierRoomData, DelegateCloseForm_Uc closingForm)
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
                this.cashierRoom = cashierRoomData;
                if (this.currentModule != null)
                {
                    //this.Text = this.currentModule.text;
                    //this.Hide();
                }
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 3");
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 4");
                this.DelegateClose = closingForm;

                CloseThread = new System.Threading.Thread(ClosingForm);
                CloseThread.Start();
                // LoadRequestMessageUseCard();
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

        private void LoadSearch()
        {
            try
            {
                stopThread = true;
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = this.treatmentId;

                var listTreatment = new BackendAdapter(param)
                           .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                stopThread = false;
                ResetLoopCount();

                if (listTreatment != null && listTreatment.Count > 0)
                {
                    currentTreatment = listTreatment.FirstOrDefault();

                    Inventec.Common.Logging.LogSystem.Debug("LoadSearch: " + Inventec.Common.Logging.LogUtil.TraceData("", currentTreatment.TREATMENT_CODE));
                }
                else
                {
                    param.Messages.Add(Base.ResourceMessageLang.KhongTimThayMaDieuTri);
                    return;
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerInitForm_Tick(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                this.timerInitForm.Stop();

                LoadSearch();

                LoadDataToGridViewSereServ();

                //this.FillInfoPatient(this.currentTreatment);
                this.CalcuTotalPrice();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 4");
                this.CalcuCanThu();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 6");
                //this.FillDataToButtonPrint();
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
                // Ẩn yêu cầu quẹt thẻ
                labelCardRequire.Visible = false;
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 1");
                WaitingManager.Show();
                this.LoadKeyFrmLanguage();
                HisConfigCFG.LoadConfig();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 2");
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 3");
                this.ValidControl();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 4");
                this.LoadAccountBookToLocal();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 5");
                this.GeneratePopupMenu();
                this.ResetControlValue();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 6");
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 7");
                isInit = false;
                timerInitForm.Interval = 100;
                timerInitForm.Enabled = true;
                timerInitForm.Start();
                //  LoadRequestMessageUseCard();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadRequestMessageUseCard()
        {

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

        void CheckPayFormTienMatChuyenKhoan(HIS_PAY_FORM payForm)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    //HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = spinTongTuDen.Value;
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

                        stopThread = true;
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new HisAccountBookViewFilter();
                        hisAccountBookViewFilter.ID = accountBook.ID;
                        var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                        stopThread = false;
                        ResetLoopCount();

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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAccountBookToLocal()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                stopThread = true;
                //Sửa lại đoạn code này
                //Api bổ sung filter chứ không get nhiều api
                //TODO               
                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.CASHIER_ROOM_ID = this.cashierRoom.ID;//Kiểm tra sổ còn hay k
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                acFilter.FOR_BILL = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.IS_NOT_GEN_TRANSACTION_ORDER = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                ListAccountBook = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                stopThread = false;
                ResetLoopCount();
                if (ListAccountBook != null && ListAccountBook.Count > 0)
                {
                    if (WorkPlace.WorkInfoSDO != null && WorkPlace.WorkInfoSDO.WorkingShiftId.HasValue)
                    {
                        ListAccountBook = ListAccountBook.Where(o => !o.WORKING_SHIFT_ID.HasValue || o.WORKING_SHIFT_ID == WorkPlace.WorkInfoSDO.WorkingShiftId.Value).ToList();
                    }
                    else
                    {
                        ListAccountBook = ListAccountBook.Where(o => !o.WORKING_SHIFT_ID.HasValue).ToList();
                    }
                }

                LoadDataToComboAccountBook();
                SetDefaultAccountBook();//TODO
                LoadRequestMessageUseCard();
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboAccountBook()
        {
            try
            {
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
                    SetDataToDicNumOrderInAccountBook(accountBook);
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
                this.listData = new List<HIS_SERE_SERV>();
                var dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();
                if (this.treatmentId > 0)
                {
                    stopThread = true;
                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.TDL_TREATMENT_ID = this.treatmentId;
                    var listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (listSSBill != null && listSSBill.Count > 0)
                    {
                        foreach (var item in listSSBill)
                        {
                            if (item.IS_CANCEL == (short)1)
                                continue;
                            if (!dicSereServBill.ContainsKey(item.SERE_SERV_ID))
                                dicSereServBill[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_BILL>();
                            dicSereServBill[item.SERE_SERV_ID].Add(item);
                        }
                    }

                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.TREATMENT_ID = this.treatmentId;
                    var hisSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                    if (hisSereServs != null && hisSereServs.Count > 0)
                    {
                        foreach (var item in hisSereServs)
                        {
                            if (dicSereServBill.ContainsKey(item.ID))
                                continue;
                            if (item.IS_NO_EXECUTE == 1 && item.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK)
                            {
                                this.listData.Add(item);
                                continue;
                            }

                            if (item.IS_NO_PAY == 1 || item.VIR_TOTAL_PATIENT_PRICE == 0 || item.IS_NO_EXECUTE == 1)
                                continue;

                            this.listData.Add(item);
                        }
                    }
                }

                // bỏ những dịch vụ đã chốt nợ
                if (this.treatmentId > 0 && this.listData != null && this.listData.Count > 0)
                {
                    MOS.Filter.HisSereServDebtFilter sereServDebtFilter = new HisSereServDebtFilter();
                    sereServDebtFilter.TDL_TREATMENT_ID = this.treatmentId;
                    var sereServDebtList = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDebtFilter, null);
                    if (sereServDebtList != null && sereServDebtList.Count > 0)
                    {
                        sereServDebtList = sereServDebtList.Where(o => o.IS_CANCEL != 1).ToList();

                        this.listData = sereServDebtList != null && sereServDebtList.Count > 0
                            ? this.listData.Where(o => !sereServDebtList.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList()
                            : this.listData;
                    }
                }

                stopThread = false;
                ResetLoopCount();

                gridControlSereServ.DataSource = this.listData;
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalcuTotalPrice()
        {
            try
            {
                totalPatientPrice = 0;
                List<HIS_SERE_SERV> ListAll = (List<HIS_SERE_SERV>)gridControlSereServ.DataSource;
                if (ListAll == null || ListAll.Count == 0)
                {
                    totalPatientPrice = 0;
                    lblTotalPrice.Text = "0";
                }

                if (listData != null)
                {
                    totalPatientPrice = ListAll.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    lblTotalPrice.Text = ConvertNumberToString(ListAll.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string ConvertNumberToString(decimal number)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Number.Convert.NumberToString(number, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void ResetControlValue()
        {
            try
            {
                resultTranBill = null;
                totalPatientPrice = 0;

                //SetDefaultAccountBook();//TODO
                //SetDefaultPayFormForUser();//TODO
                btnSave.Enabled = true;
                lciBtnSave.Enabled = true;
                ddBtnPrint.Enabled = false;
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
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }
                if (accountBook == null) accountBook = ListAccountBook.FirstOrDefault();

                if (accountBook != null)
                {
                    SetDataToDicNumOrderInAccountBook(accountBook);
                }

                if (this.accountBook == null || this.accountBook.ID == 0)
                {
                    MessageBox.Show("Không chọn được số thu chi. Vui lòng liên hệ với nhân viên bệnh viện");
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
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__BTN_SAVE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__BTN_PRINT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.lciTotalPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_TOTAL_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Repository Button
                //InfoPatient
                this.gridColumnSereServServiceCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumnSereServServiceName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_SERVICE_NAME", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumnSereServTotalPatientPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                //resultPatientType = null;
                //LoadSearch();
                //FillInfoPatient(currentTreatment);
                LoadAccountBookToLocal();
                LoadDataToComboAccountBook();
                ResetControlValue();
                //SetDefaultAccountBook();//TODO
                LoadDataToGridViewSereServ();
                //LoadDataForBordereau();
                CalcuTotalPrice();
                CalcuCanThu();
                isInit = false;
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

        private void frmTransactionBill_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                CloseThread.Abort();
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
                onClickPhieuThuThanhToanKiosk(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServDebt_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
        }

        private void CalcuCanThu()
        {
            try
            {
                decimal canthuAmount = 0;
                canthuAmount = totalPatientPrice;
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

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMinimize_MouseHover(object sender, EventArgs e)
        {
            btnMinimize.BackColor = Color.Silver;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {

            if (isMaximize == false)
            {
                this.WindowState = FormWindowState.Maximized;
                isMaximize = true;
                btnMaximize.Image = HIS.Desktop.Plugins.TransactionBillKiosk.Properties.Resources.filter__none_svg_vector_icon_16px;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                isMaximize = false;
                btnMaximize.Image = HIS.Desktop.Plugins.TransactionBillKiosk.Properties.Resources.crop__square_svg_vector_icon_16px;
            }

        }

        private void btnMaximize_MouseHover(object sender, EventArgs e)
        {
            btnMaximize.BackColor = Color.Silver;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_MouseHover(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.Red;
        }

        private void btnMinimize_MouseLeave(object sender, EventArgs e)
        {
            btnMinimize.BackColor = Color.LightSteelBlue;
        }

        private void btnMaximize_MouseLeave(object sender, EventArgs e)
        {
            btnMaximize.BackColor = Color.LightSteelBlue;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.LightSteelBlue;
        }

        private void gridViewSereServ_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HIS_SERE_SERV)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_STR")
                        {
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClosingForm()
        {
            try
            {
                if (HisConfigCFG.timeWaitingMilisecond > 0)
                {
                    bool time_out = false;
                    ResetLoopCount();
                    while (!time_out)
                    {
                        if (stopThread)
                        {
                            ResetLoopCount();
                        }

                        if (loopCount <= 0)
                        {
                            time_out = true;
                        }

                        System.Threading.Thread.Sleep(50);
                        loopCount--;
                    }

                    this.Invoke(new MethodInvoker(delegate() { this.Close(); }));
                    if (DelegateClose != null)
                    {
                        DelegateClose(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetLoopCount()
        {
            try
            {
                this.loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
