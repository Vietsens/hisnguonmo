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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SessionInfo.Base;
using HIS.Desktop.Plugins.SessionInfo.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SessionInfo
{
    public partial class frmSessionInfo : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        List<V_HIS_ACCOUNT_BOOK> listRecieptAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_ACCOUNT_BOOK> listInvoiceAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_ACCOUNT_BOOK> listDepositAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        V_HIS_CASHIER_ROOM cashierRoom;
        HIS_BRANCH branch = null;

        List<AuthorityAccountBookSDO> listAuthority = null;
        List<V_HIS_USER_ROOM> listUserRoom = null;

        private int hight_form = 0;
        private bool isInit = true;

        public frmSessionInfo(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
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

                this.StartPosition = FormStartPosition.Manual;
                foreach (var scrn in Screen.AllScreens)
                {
                    if (scrn.Bounds.Contains(this.Location))
                    {
                        this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
                        return;
                    }
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
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSessionInfo_Load(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = true;
                this.TopLevel = true;
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                InitComboPayForm();
                InitComboUserRoomTN();
                LoadCashierRoomAndBranch();
                LoadDataToControl();
                GlobalVariables.RefreshSessionModule = this.RefreshData;
                GlobalVariables.RefreshSessionDepositInfo = this.RefreshDepositData;
                LogSystem.Debug("Set RefreshSessionModule: " + (GlobalVariables.RefreshSessionModule != null).ToString());
                LogSystem.Debug("Set RefreshSessionDepositInfo: " + (GlobalVariables.RefreshSessionDepositInfo != null).ToString());
                LogSystem.Debug("GlobalVariables.SessionInfo___FIRST: \n" + LogUtil.TraceData("GlobalVariables.SessionInfo", GlobalVariables.SessionInfo));
                           
                this.isInit = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLangManager.LanguageFrmTransactionBillDetail = new ResourceManager("HIS.Desktop.Plugins.SessionInfo.Resources.Lang", typeof(HIS.Desktop.Plugins.SessionInfo.frmSessionInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.layoutControl1.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.lciCurrentDate.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.lciCurrentDate.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.lciWorkingShirt.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.lciWorkingShirt.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.cboRecieptAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSessionInfo.cboRecieptAccountBook.Properties.NullText", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.layoutControlItem8.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.layoutControlItem10.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.lciCboRecieptAccountBook.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.lciCboRecieptAccountBook.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.cboInvoiceAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSessionInfo.cboInvoiceAccountBook.Properties.NullText", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.layoutControlItem9.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.layoutControlItem11.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.lciCboInvoiceAccountBook.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.lciCboInvoiceAccountBook.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.lciCashierLoginName.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.lciCashierLoginName.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
                this.lciUserRoomTN.Text = Inventec.Common.Resource.Get.Value("frmSessionInfo.lciUserRoomTN.Text", ResourceLangManager.LanguageFrmTransactionBillDetail, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControlsValue()
        {
            try
            {
                cboPayForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM;

                if (this.currentModule.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN)
                {
                    cboUserRoomTN.EditValue = this.listUserRoom.Where(o => o.ROOM_ID == this.currentModule.RoomId).Select(o=>o.ID).FirstOrDefault();
                    cboUserRoomTN.Enabled = false;
                }
                else
                {
                    if (this.listUserRoom != null && this.listUserRoom.Count() == 1)
                    {
                        cboUserRoomTN.EditValue = this.listUserRoom.First().ID;
                    }
                    else if (this.listUserRoom != null && this.listUserRoom.Count() > 1)
                    {
                        if (GlobalVariables.SessionInfo != null && GlobalVariables.SessionInfo.CashierWorkingRoomId != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("GlobalVariables.SessionInfo.CashierWorkingRoomId: " + GlobalVariables.SessionInfo.CashierWorkingRoomId);
                            cboUserRoomTN.EditValue = this.listUserRoom.Where(o=>o.ROOM_ID== GlobalVariables.SessionInfo.CashierWorkingRoomId).Select(o=>o.ID).FirstOrDefault();
                        }
                        else
                        {
                            cboUserRoomTN.EditValue = null;
                        }
                    }
                    cboUserRoomTN.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToControl()
        {
            try
            {
                LoadAccountBookToLocal();
                LoadDataGeneralInfo();
                SetDefaultControlsValue();
                LoadDataToGridAuthority();
                lblCurrentDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.DateTime.Get.Now() ?? 0);
                var currentWorkingShift = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO != null && HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO.WorkingShiftId.HasValue
                    ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORKING_SHIFT>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO.WorkingShiftId.Value)
                    : null;
                lblWorkingShirt.Text = currentWorkingShift != null ? currentWorkingShift.WORKING_SHIFT_NAME : "";
                lblCashierLoginName.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() + " - " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                if (GlobalVariables.SessionInfo != null)
                {
                    if (GlobalVariables.SessionInfo.PayForm != null)
                    {
                        cboPayForm.EditValue = GlobalVariables.SessionInfo.PayForm.ID;
                    }

                    if (GlobalVariables.SessionInfo.NextDepositNumOrder.HasValue)
                    {
                        spinDepositNumOrder.Value = GlobalVariables.SessionInfo.NextDepositNumOrder.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTotalPrice()
        {
            try
            {
                List<long> accountBookIds = new List<long>();
                if (cboInvoiceAccountBook.EditValue != null)
                {
                    accountBookIds.Add(Convert.ToInt64(cboInvoiceAccountBook.EditValue));
                }
                else
                {
                    lblSumPriceInvoice.Text = "0";
                }
                if (cboRecieptAccountBook.EditValue != null)
                {
                    accountBookIds.Add(Convert.ToInt64(cboRecieptAccountBook.EditValue));
                }
                else
                {
                    lblSumPriceReceipt.Text = "0";
                }
                if (accountBookIds != null && accountBookIds.Count > 0)
                {
                    HisAccountBookGeneralInfoFilter filter = new HisAccountBookGeneralInfoFilter();
                    filter.ACCOUNT_BOOK_IDs = accountBookIds;
                    filter.CASHIER_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    filter.TRANSACTON_DATE = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                    List<HisAccountBookGeneralInfoSDO> results = new BackendAdapter(new CommonParam()).Get<List<HisAccountBookGeneralInfoSDO>>("api/HisAccountBook/GetGeneralInfo", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (results != null && results.Count > 0)
                    {
                        foreach (var item in results)
                        {
                            if (cboInvoiceAccountBook.EditValue != null && Convert.ToInt64(cboInvoiceAccountBook.EditValue) == item.AccountBookId)
                            {
                                lblSumPriceInvoice.Text = Inventec.Common.Number.Convert.NumberToString(item.TotalBillAmount ?? 0, ConfigApplications.NumberSeperator);
                            }
                            else if (cboRecieptAccountBook.EditValue != null && Convert.ToInt64(cboRecieptAccountBook.EditValue) == item.AccountBookId)
                            {
                                lblSumPriceReceipt.Text = Inventec.Common.Number.Convert.NumberToString(item.TotalBillAmount ?? 0, ConfigApplications.NumberSeperator);
                            }
                        }
                    }
                    else
                    {
                        lblSumPriceInvoice.Text = "0";
                        lblSumPriceReceipt.Text = "0";
                    }
                }
                else
                {
                    lblSumPriceInvoice.Text = "0";
                    lblSumPriceReceipt.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetNumOrder()
        {
            try
            {
                if (cboRecieptAccountBook.EditValue != null)
                {
                    var account = listRecieptAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRecieptAccountBook.EditValue));
                    lblRecieptNumOrder.Text = Inventec.Common.Number.Convert.NumberToStringRoundMax4(setDataToDicNumOrderInAccountBook(account));
                }
                else
                {
                    lblRecieptNumOrder.Text = "";
                }
                if (cboInvoiceAccountBook.EditValue != null)
                {
                    var account = listInvoiceAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoiceAccountBook.EditValue));
                    lblInvoiceNumOrder.Text = Inventec.Common.Number.Convert.NumberToStringRoundMax4(setDataToDicNumOrderInAccountBook(account));
                }
                else
                {
                    lblInvoiceNumOrder.Text = "";
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
                    this.cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId && o.ROOM_TYPE_ID == this.currentModule.RoomTypeId);
                    branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
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
                this.listInvoiceAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
                this.listRecieptAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.CASHIER_ROOM_ID = this.cashierRoom != null ? (long?)this.cashierRoom.ID : null;//Kiểm tra sổ còn hay k
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //acFilter.FOR_BILL = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                List<V_HIS_ACCOUNT_BOOK> listUserAcountBoook = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                if (listUserAcountBoook != null && listUserAcountBoook.Count > 0)
                {
                    string BillSelect = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.TransactionBillSelect");
                    var currentWorkingShift = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO != null && HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO.WorkingShiftId.HasValue
                   ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORKING_SHIFT>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO.WorkingShiftId.Value)
                   : null;

                    listUserAcountBoook = listUserAcountBoook.Where(o => !o.WORKING_SHIFT_ID.HasValue
                   || (currentWorkingShift != null && o.WORKING_SHIFT_ID.Value == currentWorkingShift.ID)).ToList();

                    foreach (var item in listUserAcountBoook)
                    {
                        if ((item.FROM_NUM_ORDER + item.TOTAL - 1) <= item.CURRENT_NUM_ORDER)
                        {
                            continue;
                        }
                        if (item.IS_FOR_BILL == (short)1)
                        {
                            if (BillSelect == "2")
                            {
                                if (item.BILL_TYPE_ID == 2)
                                {
                                    listInvoiceAccountBook.Add(item);
                                }
                                else
                                {
                                    listRecieptAccountBook.Add(item);
                                }
                            }
                            else
                            {
                                listRecieptAccountBook.Add(item);
                            }
                        }
                        if (item.IS_FOR_DEPOSIT == (short)1)
                        {
                            listDepositAccountBook.Add(item);
                        }
                    }
                }

                LoadDataToComboAccountBook();
                SetDefaultAccountBook();
                LoadTotalPrice();
                SetNumOrder();
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
                cboRecieptAccountBook.Properties.DataSource = listRecieptAccountBook;
                cboRecieptAccountBook.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                cboRecieptAccountBook.Properties.ValueMember = "ID";
                cboRecieptAccountBook.Properties.ForceInitialize();
                cboRecieptAccountBook.Properties.Columns.Clear();
                cboRecieptAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_CODE", "", 50));
                cboRecieptAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_NAME", "", 200));
                cboRecieptAccountBook.Properties.ShowHeader = false;
                cboRecieptAccountBook.Properties.ImmediatePopup = true;
                cboRecieptAccountBook.Properties.DropDownRows = 10;
                cboRecieptAccountBook.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            try
            {
                cboInvoiceAccountBook.Properties.DataSource = listInvoiceAccountBook;
                cboInvoiceAccountBook.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                cboInvoiceAccountBook.Properties.ValueMember = "ID";
                cboInvoiceAccountBook.Properties.ForceInitialize();
                cboInvoiceAccountBook.Properties.Columns.Clear();
                cboInvoiceAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_CODE", "", 50));
                cboInvoiceAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_NAME", "", 200));
                cboInvoiceAccountBook.Properties.ShowHeader = false;
                cboInvoiceAccountBook.Properties.ImmediatePopup = true;
                cboInvoiceAccountBook.Properties.DropDownRows = 10;
                cboInvoiceAccountBook.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            try
            {
                cboDepositAccountBook.Properties.DataSource = listDepositAccountBook;
                cboDepositAccountBook.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                cboDepositAccountBook.Properties.ValueMember = "ID";
                cboDepositAccountBook.Properties.ForceInitialize();
                cboDepositAccountBook.Properties.Columns.Clear();
                cboDepositAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_CODE", "", 50));
                cboDepositAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_NAME", "", 200));
                cboDepositAccountBook.Properties.ShowHeader = false;
                cboDepositAccountBook.Properties.ImmediatePopup = true;
                cboDepositAccountBook.Properties.DropDownRows = 10;
                cboDepositAccountBook.Properties.PopupWidth = 250;
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
                cboRecieptAccountBook.EditValue = null;
                cboInvoiceAccountBook.EditValue = null;
                string BillSelect = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.TransactionBillSelect");

                if (BillSelect == "2")
                {
                    if (listRecieptAccountBook != null && listRecieptAccountBook.Count > 0)
                    {
                        V_HIS_ACCOUNT_BOOK data = null;
                        //chọn mặc định sổ nếu có sổ tương ứng
                        if (GlobalVariables.DefaultAccountBookBillTwoInOne_VP != null && GlobalVariables.DefaultAccountBookBillTwoInOne_VP.Count > 0)
                        {
                            var lstBook = listRecieptAccountBook.Where(o => GlobalVariables.DefaultAccountBookBillTwoInOne_VP.Select(s => s.ID).Contains(o.ID)).ToList();
                            if (lstBook != null && lstBook.Count > 0)
                            {
                                data = lstBook.Last();
                            }
                        }

                        if (data != null)
                            cboRecieptAccountBook.EditValue = data.ID;
                    }

                    if (listInvoiceAccountBook != null && listInvoiceAccountBook.Count > 0)
                    {
                        V_HIS_ACCOUNT_BOOK data = null;
                        //chọn mặc định sổ nếu có sổ tương ứng
                        if (GlobalVariables.DefaultAccountBookBillTwoInOne_DV != null && GlobalVariables.DefaultAccountBookBillTwoInOne_DV.Count > 0)
                        {
                            var lstBook = listInvoiceAccountBook.Where(o => GlobalVariables.DefaultAccountBookBillTwoInOne_DV.Select(s => s.ID).Contains(o.ID)).ToList();
                            if (lstBook != null && lstBook.Count > 0)
                            {
                                data = lstBook.Last();
                            }
                        }

                        if (data != null)
                            cboInvoiceAccountBook.EditValue = data.ID;
                    }
                }
                else
                {
                    if (listRecieptAccountBook != null && listRecieptAccountBook.Count > 0)
                    {
                        V_HIS_ACCOUNT_BOOK data = null;
                        //chọn mặc định sổ nếu có sổ tương ứng
                        if (GlobalVariables.DefaultAccountBookTransactionBill != null && GlobalVariables.DefaultAccountBookTransactionBill.Count > 0)
                        {
                            var lstBook = listRecieptAccountBook.Where(o => GlobalVariables.DefaultAccountBookTransactionBill.Select(s => s.ID).Contains(o.ID)).ToList();

                            if (lstBook != null && lstBook.Count > 0)
                            {
                                data = lstBook.Last();
                            }
                        }

                        if (data != null)
                            cboRecieptAccountBook.EditValue = data.ID;
                    }
                }

                if (listDepositAccountBook != null && listDepositAccountBook.Count > 0)
                {
                    if (GlobalVariables.SessionInfo != null && GlobalVariables.SessionInfo.DepositAccountBook != null)
                    {
                        V_HIS_ACCOUNT_BOOK data = listDepositAccountBook.FirstOrDefault(o => o.ID == GlobalVariables.SessionInfo.DepositAccountBook.ID);
                        if (data != null)
                        {
                            cboDepositAccountBook.EditValue = data.ID;
                            spinDepositNumOrder.Value = GlobalVariables.SessionInfo.NextDepositNumOrder ?? 0;
                            spinDepositNumOrder.Enabled = (GlobalVariables.SessionInfo.DepositAccountBook.IS_NOT_GEN_TRANSACTION_ORDER == (short)1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                ControlEditorLoader.Load(this.cboPayForm, BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == (short)1).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboUserRoomTN()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitComboUserRoomTN(): Begin");
                List<V_HIS_USER_ROOM> data = new List<V_HIS_USER_ROOM>();
                data = LoadDataToComboUserRoomTN();
                Inventec.Common.Logging.LogSystem.Debug("data.Count:" + data.Count());
                this.listUserRoom = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboUserRoomTN, data, controlEditorADO);
                Inventec.Common.Logging.LogSystem.Debug("InitComboUserRoomTN(): End");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_USER_ROOM> LoadDataToComboUserRoomTN()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisUserRoomViewFilter filter = new HisUserRoomViewFilter();
                filter.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN;
                filter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                Inventec.Common.Logging.LogSystem.Debug("HisUserRoomViewFilter: " + LogUtil.TraceData("filter", filter));
                List<V_HIS_USER_ROOM> result = new BackendAdapter(param).Get<List<V_HIS_USER_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_USER_ROOM_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                result = (result != null) ? result.Where(o=>o.IS_PAUSE != 1).ToList() : null;
                var listCashierRooms = BackendDataWorker.Get<HIS_CASHIER_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o=>o.ROOM_ID).ToList();
                if (listCashierRooms == null)
                {
                    return new List<V_HIS_USER_ROOM>();
                }
                result = result.Where(o => listCashierRooms.Contains(o.ROOM_ID)).ToList();
                if (result != null)
                {
                    return result;
                }
                return new List<V_HIS_USER_ROOM>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<V_HIS_USER_ROOM>();
            }
        }

        private void cboRecieptAccountBook_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRecieptAccountBook.EditValue = null;
                    WaitingManager.Show();
                    LoadTotalPrice();
                    SetNumOrder();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoiceAccountBook_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboInvoiceAccountBook.EditValue = null;
                    WaitingManager.Show();
                    LoadTotalPrice();
                    SetNumOrder();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRecieptAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                lblRecieptNumOrder.Text = "";
                cboRecieptAccountBook.Properties.Buttons[1].Visible = false;
                if (cboRecieptAccountBook.EditValue != null)
                {
                    cboRecieptAccountBook.Properties.Buttons[1].Visible = true;
                    var account = listRecieptAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRecieptAccountBook.EditValue));
                    if (account != null)
                    {
                        string BillSelect = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.TransactionBillSelect");
                        if (BillSelect == "2")
                        {
                            GlobalVariables.DefaultAccountBookBillTwoInOne_VP = new List<V_HIS_ACCOUNT_BOOK>();
                            GlobalVariables.DefaultAccountBookBillTwoInOne_VP.Add(account);
                        }
                        else
                        {
                            GlobalVariables.DefaultAccountBookTransactionBill = new List<V_HIS_ACCOUNT_BOOK>();
                            GlobalVariables.DefaultAccountBookTransactionBill.Add(account);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal setDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            decimal result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
            try
            {
                if (accountBook != null)
                {
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                        }

                        CommonParam param = new CommonParam();
                        MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new MOS.Filter.HisAccountBookViewFilter();
                        hisAccountBookViewFilter.ID = accountBook.ID;
                        var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(ApiConsumer.HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                        if (accountBooks != null && accountBooks.Count > 0)
                        {
                            var accountBookNew = accountBooks.FirstOrDefault();
                            decimal num = 0;
                            if ((accountBookNew.CURRENT_NUM_ORDER ?? 0) > 0)
                            {
                                num = (accountBookNew.CURRENT_NUM_ORDER ?? 0);
                            }
                            else
                            {
                                num = (decimal)accountBookNew.FROM_NUM_ORDER - 1;
                            }
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
                            result = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                        }
                    }
                    else
                    {
                        result = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                    }
                }
                else
                {
                    result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void cboInvoiceAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                lblInvoiceNumOrder.Text = "";
                cboInvoiceAccountBook.Properties.Buttons[1].Visible = false;
                if (cboInvoiceAccountBook.EditValue != null)
                {
                    cboInvoiceAccountBook.Properties.Buttons[1].Visible = true;
                    var account = listInvoiceAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoiceAccountBook.EditValue));
                    if (account != null)
                    {
                        GlobalVariables.DefaultAccountBookBillTwoInOne_DV = new List<V_HIS_ACCOUNT_BOOK>();
                        GlobalVariables.DefaultAccountBookBillTwoInOne_DV.Add(account);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshData()
        {
            try
            {
                LogSystem.Debug("RefreshData");
                Thread thread = new Thread(new ThreadStart(this.InitThreadLoadData));
                //thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitThreadLoadData()
        {
            try
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    this.LoadData();
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadData()
        {
            try
            {
                LogSystem.Debug("LoadData");
                this.LoadAccountBookToLocal();
                this.LoadDataGeneralInfo();
                LogSystem.Debug("LoadData");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataGeneralInfo()
        {
            try
            {
                HisTransactionGeneralInfoFilter filter = new HisTransactionGeneralInfoFilter();
                filter.CASHIER_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                filter.TRANSACTION_DATE = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                LogSystem.Debug("LoadDataGeneralInfo: \n" + LogUtil.TraceData("Filter", filter));
                HisTransactionGeneralInfoSDO sdo = new BackendAdapter(new CommonParam()).Get<HisTransactionGeneralInfoSDO>("api/HisTransaction/GetGeneralInfo", ApiConsumers.MosConsumer, filter, null);
                LogSystem.Debug("LoadDataGeneralInfo: \n" + LogUtil.TraceData("Sdo", sdo));
                if (sdo != null)
                {
                    lblTotalBillDirectly.Text = Inventec.Common.Number.Convert.NumberToString(sdo.TotalBillDirectly, ConfigApplications.NumberSeperator);
                    lblTotalBillNotDirectly.Text = Inventec.Common.Number.Convert.NumberToString(sdo.TotalBillNotDirectly, ConfigApplications.NumberSeperator);
                }
                else
                {
                    lblTotalBillDirectly.Text = "0";
                    lblTotalBillNotDirectly.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataToGridAuthority()
        {
            try
            {
                this.listAuthority = new BackendAdapter(new CommonParam()).Get<List<AuthorityAccountBookSDO>>("api/HisAccountBook/RequestToMe", ApiConsumers.MosConsumer, this.currentModuleBase.RoomId, null);
                gridControlAuthority.BeginUpdate();
                gridControlAuthority.DataSource = listAuthority;
                gridControlAuthority.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshDepositData(object data)
        {
            try
            {
                LogSystem.Debug("RefreshDepositData");
                Thread thread = new Thread(new ParameterizedThreadStart(this.InitThreadLoadDepositData));
                //thread.Priority = ThreadPriority.Highest;
                thread.Start(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitThreadLoadDepositData(object data)
        {
            try
            {
                long currentNumOrder = (long)data;
                this.Invoke(new MethodInvoker(delegate()
                {
                    this.LoadDepositData(currentNumOrder);
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDepositData(long currentNumOrder)
        {
            try
            {
                if (GlobalVariables.SessionInfo != null) GlobalVariables.SessionInfo.CurrentDepositNumOrder = currentNumOrder;
                spinDepositNumOrder.Value = (currentNumOrder + 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSessionInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GlobalVariables.RefreshSessionModule = null;
                GlobalVariables.RefreshSessionDepositInfo = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRecieptAccountBook_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadTotalPrice();
                SetNumOrder();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoiceAccountBook_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadTotalPrice();
                SetNumOrder();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnApprovalAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnUnApprovalAll.Enabled) return;

                if (listAuthority == null || listAuthority.Count <= 0)
                {
                    XtraMessageBox.Show(ResourceMessage.KhongCoDuLieuUyQuyen, ResourceMessage.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();

                UnapprovalAccountBookSDO sdo = new UnapprovalAccountBookSDO();
                sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                bool rs = new BackendAdapter(param).Post<bool>("api/HisAccountBook/Unapprove", ApiConsumers.MosConsumer, sdo, param);
                if (rs)
                {
                    this.LoadDataToGridAuthority();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, rs);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnReload.Enabled) return;
                WaitingManager.Show();
                this.LoadDataToGridAuthority();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAuthority_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Object)
                {
                    AuthorityAccountBookSDO data = (AuthorityAccountBookSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "AUTHORITY_USER")
                        {
                            e.Value = string.Format("{0} - {1}", data.RequestLoginName, data.RequestUserName);
                        }
                        else if (e.Column.FieldName == "AUTHORITY_ROOM")
                        {
                            V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == data.RequestRoomId);
                            if (room != null)
                            {
                                e.Value = room.ROOM_NAME;
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        else if (e.Column.FieldName == "AUTHORITY_TIME")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.RequestTime);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAuthority_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                AuthorityAccountBookSDO data = (AuthorityAccountBookSDO)gridViewAuthority.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "APPROVE_UNAPPROVE")
                    {
                        if (data.AccountBookId.HasValue)
                        {
                            e.RepositoryItem = repositoryItemButton_Unapprove;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton_Approve;
                        }
                    }
                    else if (e.Column.FieldName == "REJECT")
                    {
                        if (data.AccountBookId.HasValue)
                        {
                            e.RepositoryItem = repositoryItemButton_Reject_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton_Reject;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Approve_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                AuthorityAccountBookSDO data = (AuthorityAccountBookSDO)gridViewAuthority.GetFocusedRow();
                if (data != null)
                {
                    if (cboDepositAccountBook.EditValue == null)
                    {
                        XtraMessageBox.Show(ResourceMessage.BanChuaChonSoTamThu, ResourceMessage.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();

                    ApprovalAccountBookSDO sdo = new ApprovalAccountBookSDO();
                    sdo.AccountBookId = Convert.ToInt64(cboDepositAccountBook.EditValue);
                    sdo.RequestLoginName = data.RequestLoginName;
                    sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                    bool rs = new BackendAdapter(param).Post<bool>("api/HisAccountBook/Approve", ApiConsumers.MosConsumer, sdo, param);
                    if (rs)
                    {
                        this.LoadDataToGridAuthority();
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this, param, rs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Unapprove_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                AuthorityAccountBookSDO data = (AuthorityAccountBookSDO)gridViewAuthority.GetFocusedRow();
                if (data != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();

                    UnapprovalAccountBookSDO sdo = new UnapprovalAccountBookSDO();
                    sdo.RequestLoginName = data.RequestLoginName;
                    sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                    bool rs = new BackendAdapter(param).Post<bool>("api/HisAccountBook/Unapprove", ApiConsumers.MosConsumer, sdo, param);
                    if (rs)
                    {
                        this.LoadDataToGridAuthority();
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this, param, rs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Reject_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                AuthorityAccountBookSDO data = (AuthorityAccountBookSDO)gridViewAuthority.GetFocusedRow();
                if (data != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();

                    RejectAccountBookSDO sdo = new RejectAccountBookSDO();
                    sdo.RequestLoginName = data.RequestLoginName;
                    sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                    bool rs = new BackendAdapter(param).Post<bool>("api/HisAccountBook/Reject", ApiConsumers.MosConsumer, sdo, param);
                    if (rs)
                    {
                        this.LoadDataToGridAuthority();
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this, param, rs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void layoutControl1_GroupExpandChanged(object sender, DevExpress.XtraLayout.Utils.LayoutGroupEventArgs e)
        {
            try
            {
                LogSystem.Debug("layoutControl1_GroupExpandChanged.1");
                if (e.Group.Name == "lciAuthority")
                {
                    LogSystem.Debug("layoutControl1_GroupExpandChanged.1.1: " + e.Group.Expanded);
                    if (e.Group.Expanded)
                    {
                        this.Height = this.hight_form;
                    }
                    else
                    {
                        this.hight_form = this.Height;
                        this.Height = (int)170;
                    }
                    LogSystem.Debug("layoutControl1_GroupExpandChanged.1.2: " + this.Height);
                }
                LogSystem.Debug("layoutControl1_GroupExpandChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVariables.SessionInfo == null) GlobalVariables.SessionInfo = new LocalStorage.Global.ADO.SessionInfoADO();
                if (cboPayForm.EditValue != null)
                {
                    GlobalVariables.SessionInfo.PayForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                }
                else
                {
                    GlobalVariables.SessionInfo.PayForm = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositAccountBook_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepositAccountBook.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositAccountBook_Closed(object sender, ClosedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                {
                    return;
                }
                if (GlobalVariables.SessionInfo == null) GlobalVariables.SessionInfo = new LocalStorage.Global.ADO.SessionInfoADO();
                if (cboDepositAccountBook.EditValue != null)
                {
                    GlobalVariables.SessionInfo.DepositAccountBook = this.listDepositAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepositAccountBook.EditValue));
                }

                spinDepositNumOrder.Value = (GlobalVariables.SessionInfo.DepositAccountBook != null) ? ((GlobalVariables.SessionInfo.DepositAccountBook.CURRENT_NUM_ORDER ?? 0) + 1) : 0;
                spinDepositNumOrder.Enabled = (GlobalVariables.SessionInfo.DepositAccountBook != null && GlobalVariables.SessionInfo.DepositAccountBook.IS_NOT_GEN_TRANSACTION_ORDER == (short)1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDepositNumOrder_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                {
                    return;
                }

                GlobalVariables.SessionInfo.NextDepositNumOrder = (long)spinDepositNumOrder.Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUserRoomTN_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboUserRoomTN.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUserRoomTN_Closed(object sender, ClosedEventArgs e)
        {

        }

        private void cboUserRoomTN_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVariables.SessionInfo == null) GlobalVariables.SessionInfo = new LocalStorage.Global.ADO.SessionInfoADO();
                if (cboUserRoomTN.EditValue != null && this.listUserRoom != null)
                {
                    var cashierWorkingRoomId = this.listUserRoom.Where(o => o.ID == (long)cboUserRoomTN.EditValue).Select(o => o.ROOM_ID).FirstOrDefault();
                    GlobalVariables.SessionInfo.CashierWorkingRoomId = cashierWorkingRoomId;
                }
                else
                    GlobalVariables.SessionInfo.CashierWorkingRoomId = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
