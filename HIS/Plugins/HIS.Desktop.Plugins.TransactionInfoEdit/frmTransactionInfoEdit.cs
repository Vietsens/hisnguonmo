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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Collections.Generic;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Threading.Tasks;
using DevExpress.XtraEditors.Controls;
using MOS.Filter;
using System.Linq;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.TransactionInfoEdit
{
    public partial class frmTransactionInfoEdit : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        V_HIS_TRANSACTION _HisTransaction = null;
        HIS.Desktop.Common.DelegateRefreshData _dlg = null;
        List<HIS_PAY_FORM> payForm = new List<HIS_PAY_FORM>();
        List<HIS_WORKING_SHIFT> workingShifts = new List<HIS_WORKING_SHIFT>();
        List<V_HIS_ACCOUNT_BOOK> accountBooks = new List<V_HIS_ACCOUNT_BOOK>();
        List<HIS_REPAY_REASON> repayReasons = new List<HIS_REPAY_REASON>();
        int configUpdateAccountBook;
        bool MustChooseWorkingShift;
        public frmTransactionInfoEdit(Inventec.Desktop.Common.Modules.Module module, V_HIS_TRANSACTION _transaction)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();

                this.currentModule = module;
                this._HisTransaction = _transaction;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionInfoEdit(Inventec.Desktop.Common.Modules.Module module, V_HIS_TRANSACTION _transaction, HIS.Desktop.Common.DelegateRefreshData dlg)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();

                this.currentModule = module;
                this._HisTransaction = _transaction;
                this._dlg = dlg;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionInfoEdit_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.MustChooseWorkingShift = HisConfigs.Get<string>("HIS.Desktop.Plugins.ChooseRoom.MustChooseWorkingShift") == GlobalVariables.CommonStringTrue;
                //
                configUpdateAccountBook = Convert.ToInt32(GetValue("MOS.HIS_TRANSACTION.ALLOW_UPDATE_ACCOUNT_BOOK"));
                if (configUpdateAccountBook == 1)
                {
                    txtAccountBook.Enabled = true;
                    cboAccountBook.Enabled = true;
                    ValidationSingleControl(txtAccountBook);
                }
                else
                {
                    txtAccountBook.Enabled = false;
                    cboAccountBook.Enabled = false;
                }

                LoadCombo();
                SetDataDefault();
                SetValidate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

        private void LoadCombo()
        {
            LoadAccountBookToLocal();
            LoadcboPayForm();
            LoadcboWorkingShift();
            LoadcboRepayReason();
        }

        private void LoadAccountBookToLocal()
        {
            try
            {
                CommonParam param = new CommonParam();

                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                HisUserAccountBookFilter useAccountBookFilter = new HisUserAccountBookFilter();
                useAccountBookFilter.LOGINNAME__EXACT = loginName;
                var userAccountBooks = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_USER_ACCOUNT_BOOK>>("api/HisUserAccountBook/Get", ApiConsumers.MosConsumer, useAccountBookFilter, null);

                //var cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId && o.ROOM_TYPE_ID == currentModule.RoomTypeId);
                HisCaroAccountBookFilter caroAccountBookFilter = new HisCaroAccountBookFilter();
                caroAccountBookFilter.CASHIER_ROOM_ID = _HisTransaction != null ? _HisTransaction.CASHIER_ROOM_ID : 0;//0 để không tìm đc sổ nào
                var caroAccountBooks = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_CARO_ACCOUNT_BOOK>>("api/HisCaroAccountBook/Get", ApiConsumers.MosConsumer, caroAccountBookFilter, null);

                List<long> ids = new List<long>();
                accountBooks = new List<V_HIS_ACCOUNT_BOOK>();
                // Kiểm tra sổ còn hay k
                if (userAccountBooks != null && userAccountBooks.Count > 0)
                {
                    ids.AddRange(userAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }

                if (caroAccountBooks != null && caroAccountBooks.Count > 0)
                {
                    ids.AddRange(caroAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }
                //Add sổ hiện tại
                ids.Add(this._HisTransaction.ACCOUNT_BOOK_ID);
                ids = ids.Distinct().ToList();

                if (ids != null && ids.Count > 0)
                {
                    MOS.Filter.HisAccountBookViewFilter Filter = new HisAccountBookViewFilter();
                    Filter.IDs = ids;
                    Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //Filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    //Filter.FOR_DEPOSIT = true;
                    Filter.ORDER_FIELD = "CREATE_TIME";
                    Filter.ORDER_DIRECTION = "DESC";

                    var rsData = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, Filter, param);
                    //Kiem tra so thu chi con su dug duoc
                    if (rsData != null && rsData.Count > 0)
                    {
                        foreach (var item in rsData)
                        {
                            if ((item.FROM_NUM_ORDER + item.TOTAL - 1) > item.CURRENT_NUM_ORDER)
                            {
                                accountBooks.Add(item);
                            }
                        }
                    }
                }
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => accountBooks), accountBooks));
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _HisTransaction.TRANSACTION_TYPE_ID), _HisTransaction.TRANSACTION_TYPE_ID));

                if (accountBooks != null && accountBooks.Count > 0)
                {
                    if (_HisTransaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                        accountBooks = accountBooks.Where(o => o.IS_FOR_DEPOSIT == (short)1).ToList();
                    else if (_HisTransaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                        accountBooks = accountBooks.Where(o => o.IS_FOR_BILL == (short)1).ToList();
                    else if (_HisTransaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                        accountBooks = accountBooks.Where(o => o.IS_FOR_REPAY == (short)1).ToList();
                    else if (_HisTransaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO)
                        accountBooks = accountBooks.Where(o => o.IS_FOR_DEBT == (short)1).ToList();
                }

                LoadDataToComboAccountBook();
                SetDefaultAccountBook();//TODO
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
                if (GlobalVariables.DefaultAccountBookDebt != null && GlobalVariables.DefaultAccountBookDebt.Count > 0)
                {
                    var lstBook = accountBooks.Where(o => GlobalVariables.DefaultAccountBookDebt.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }

                if (accountBook != null)
                {

                    SetDataToDicNumOrderInAccountBook(accountBook);
                    txtAccountBook.Text = accountBook.ACCOUNT_BOOK_CODE;
                    cboAccountBook.EditValue = accountBook.ID;
                }
                else
                {
                    spNumberOrder.Text = "";
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
                //cboAccountBook.Properties.DataSource = accountBooks;
                //cboAccountBook.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                //cboAccountBook.Properties.ValueMember = "ID";
                //cboAccountBook.Properties.ForceInitialize();
                //cboAccountBook.Properties.Columns.Clear();
                //cboAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_CODE", "", 50));
                //cboAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_NAME", "", 200));
                //cboAccountBook.Properties.ShowHeader = false;
                //cboAccountBook.Properties.ImmediatePopup = true;
                //cboAccountBook.Properties.DropDownRows = 10;
                //cboAccountBook.Properties.PopupWidth = 250;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("AccountBook",accountBooks));
                ControlEditorLoader.Load(cboAccountBook, accountBooks, controlEditorADO);

                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = accountBooks.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.Last();
                    }
                }

                if (accountBook != null)
                {
                    SetDataToDicNumOrderInAccountBook(accountBook);
                    txtAccountBook.Text = accountBook.ACCOUNT_BOOK_CODE;
                    cboAccountBook.EditValue = accountBook.ID;
                }
                else
                {
                    spNumberOrder.Text = "";
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
                            spNumberOrder.Value = ((HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1);
                        }
                    }
                    else
                    {
                        layoutTongTuDen.Enabled = true;
                        spNumberOrder.Value = ((HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1);
                    }
                }
                else
                {
                    spNumberOrder.Value = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
                    layoutTongTuDen.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadcboPayForm()
        {
            try
            {
                payForm = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPayForm, payForm, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadcboWorkingShift()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WORKING_SHIFT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("WORKING_SHIFT_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORKING_SHIFT_NAME", "ID", columnInfos, false, 250);
                //var branchIds = this.currentUserRoomsByBranch.Select(o => o.BRANCH_ID).Distinct().ToList();
                //this.currentBranchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().Where(o => branchIds != null && branchIds.Contains(o.ID)).ToList();
                workingShifts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORKING_SHIFT>();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workingShifts), workingShifts));
                if (workingShifts != null)
                {
                    workingShifts = workingShifts.Where(o => o.IS_ACTIVE == 1).ToList();
                }
                ControlEditorLoader.Load(this.cboWorkingShift, this.workingShifts, controlEditorADO);

                this.lciForWorkingShift.AppearanceItemCaption.ForeColor = this.MustChooseWorkingShift ? System.Drawing.Color.Maroon : System.Drawing.Color.Black;
                if (this.MustChooseWorkingShift)
                {
                    ValidationSingleControl(cboWorkingShift, dxValidationProvider1);
                }
                if (WorkPlace.WorkInfoSDO != null && WorkPlace.WorkInfoSDO.WorkingShiftId > 0)
                {
                    this.cboWorkingShift.EditValue = WorkPlace.WorkInfoSDO.WorkingShiftId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadcboRepayReason()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("REPAY_REASON_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("REPAY_REASON_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("REPAY_REASON_NAME", "ID", columnInfos, false, 350);

                this.repayReasons = BackendDataWorker.Get<HIS_REPAY_REASON>();
                if (repayReasons != null)
                {
                    repayReasons = repayReasons.Where(o => o.IS_ACTIVE == 1).ToList();
                }
                ControlEditorLoader.Load(this.cboRepayReason, repayReasons, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataDefault()
        {
            try
            {
                this.spinEditAmount.Properties.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinEditAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.spinEditAmount.Properties.EditFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinEditAmount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;

                this.spinEditChuyenKhoan.Properties.DisplayFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinEditChuyenKhoan.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.spinEditChuyenKhoan.Properties.EditFormat.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                this.spinEditChuyenKhoan.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;

                if (this._HisTransaction != null)
                {
                    this.txtNguoiMua.Text = this._HisTransaction.BUYER_NAME;
                    this.txtDiaChiNguoiMua.Text = this._HisTransaction.BUYER_ADDRESS;
                    this.txtSTKNguoiMua.Text = this._HisTransaction.BUYER_ACCOUNT_NUMBER;
                    this.txtMaSoThue.Text = this._HisTransaction.BUYER_TAX_CODE;
                    this.txtDonVi.Text = this._HisTransaction.BUYER_ORGANIZATION;
                    this.txtBuyerEmail.Text = this._HisTransaction.BUYER_EMAIL;
                    this.spinEditAmount.EditValue = Inventec.Common.Number.Convert.NumberToString(this._HisTransaction.AMOUNT, ConfigApplications.NumberSeperator);
                    this.cboPayForm.EditValue = this._HisTransaction.PAY_FORM_ID;
                    if (this._HisTransaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                    {
                        spinEditChuyenKhoan.Enabled = true;
                    }
                    else
                    {
                        spinEditChuyenKhoan.EditValue = "";
                        spinEditChuyenKhoan.Enabled = false;
                    }
                    this.spinEditChuyenKhoan.EditValue = Inventec.Common.Number.Convert.NumberToString(this._HisTransaction.TRANSFER_AMOUNT ?? 0, ConfigApplications.NumberSeperator);

                    this.cboAccountBook.EditValue = this._HisTransaction.ACCOUNT_BOOK_ID;
                    this.txtAccountBook.Text = this._HisTransaction.ACCOUNT_BOOK_CODE;
                    this.spNumberOrder.EditValue = this._HisTransaction.NUM_ORDER;
                    this.cboWorkingShift.EditValue = this._HisTransaction.WORKING_SHIFT_ID;
                    if (this._HisTransaction.WORKING_SHIFT_ID > 0)
                        this.cboWorkingShift.Properties.Buttons[1].Visible = true;

                    if (this._HisTransaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                    {
                        this.cboRepayReason.Enabled = true;
                        if (this._HisTransaction.REPAY_REASON_ID > 0)
                        {
                            this.cboRepayReason.EditValue = this._HisTransaction.REPAY_REASON_ID;
                            this.cboRepayReason.Properties.Buttons[1].Visible = true;
                            HIS_REPAY_REASON repayReasonCode = this.repayReasons.Where(o => o.ID == this._HisTransaction.REPAY_REASON_ID).FirstOrDefault();
                            this.txtRepayReason.Text = repayReasonCode.REPAY_REASON_CODE.ToString();
                        }
                        else
                        {
                            this.cboRepayReason.Properties.Buttons[1].Visible = false;
                            this.cboRepayReason.EditValue = null;
                            this.txtRepayReason.Text = "";
                        }
                    }
                    else
                    {
                        this.cboRepayReason.EditValue = null;
                        this.cboRepayReason.Enabled = false;
                        this.txtRepayReason.Text = "";
                        this.txtRepayReason.Enabled = false;

                    }
                    this.txtDescription.Text = this._HisTransaction.DESCRIPTION;
                    if (this._HisTransaction.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        layoutTongTuDen.Enabled = true;
                    else
                        layoutTongTuDen.Enabled = false;
                }
                else
                {
                    this.txtNguoiMua.Text = "";
                    this.txtDiaChiNguoiMua.Text = "";
                    this.txtSTKNguoiMua.Text = "";
                    this.txtMaSoThue.Text = "";
                    this.txtDonVi.Text = "";
                    this.txtRepayReason.Text = "";
                    this.txtDescription.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string AddStringByConfig(int num)
        {
            string str = "";
            try
            {
                if (num > 0)
                {
                    for (int i = 1; i <= num; i++)
                    {
                        str += "0";
                    }
                }
                else
                {
                    return str = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return str = "";
            }
            return str;
        }
        private void txtNguoiMua_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiaChiNguoiMua.Focus();
                    txtDiaChiNguoiMua.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiaChiNguoiMua_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSTKNguoiMua.Focus();
                    txtSTKNguoiMua.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSTKNguoiMua_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaSoThue.Focus();
                    txtMaSoThue.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMaSoThue_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDonVi.Focus();
                    txtDonVi.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDonVi_KeyDown(object sender, KeyEventArgs e)
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Focus();
                if (_HisTransaction.ACCOUNT_BOOK_ID != Convert.ToInt64(cboAccountBook.EditValue) || _HisTransaction.NUM_ORDER != spNumberOrder.Value)
                {
                    if (MessageBox.Show("Bạn có chắc muốn sửa thông tin Sổ thu chi/số chứng từ không?", MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                bool success = false;
                WaitingManager.Show();

                MOS.SDO.HisTransactionUpdateInfoSDO ado = new MOS.SDO.HisTransactionUpdateInfoSDO();

                // Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(ado, this._HisTransaction);

                ado.BuyerName = this.txtNguoiMua.Text;
                ado.BuyerAddress = this.txtDiaChiNguoiMua.Text;
                ado.BuyerAccountNumber = this.txtSTKNguoiMua.Text;
                ado.BuyerTaxCode = this.txtMaSoThue.Text;
                ado.BuyerOrganization = this.txtDonVi.Text;
                ado.TransactionId = this._HisTransaction.ID;
                ado.BuyerEmail = this.txtBuyerEmail.Text;
                ado.PayFormId = Convert.ToInt64(cboPayForm.EditValue);
                ado.TransferAmount = spinEditChuyenKhoan.Value;
                if (cboAccountBook.EditValue != null)
                    ado.AccountBookId = Convert.ToInt64(cboAccountBook.EditValue);
                else
                    ado.AccountBookId = null;
                ado.NumOrder = Convert.ToInt64(spNumberOrder.Value);
                if (cboWorkingShift.EditValue != null)
                    ado.WorkingShiftId = Convert.ToInt64(cboWorkingShift.EditValue);
                else
                    ado.WorkingShiftId = null;
                if (cboRepayReason.EditValue != null)
                    ado.RepayReasonId = Convert.ToInt64(cboRepayReason.EditValue);
                else
                    ado.RepayReasonId = null;
                ado.Description = txtDescription.Text;
                ado.RequestRoomId = currentModule.RoomId;

                CommonParam param = new CommonParam();
                var dataUpdate = new BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/UpdateInfo", ApiConsumers.MosConsumer, ado, param);
                if (dataUpdate != null)
                {
                    var accountBook = accountBooks.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                    success = true;
                    if (this._dlg != null)
                    {
                        this._dlg();
                    }
                    this.Close();
                }
                WaitingManager.Hide();

                MessageManager.Show(this.ParentForm, param, success);
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
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = spNumberOrder.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        int positionHandle = -1;

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

        private void SetValidate()
        {
            try
            {
                SetMaxlength(txtNguoiMua, 200);
                SetMaxlength(txtDiaChiNguoiMua, 500);
                SetMaxlength(txtSTKNguoiMua, 50);
                SetMaxlength(txtMaSoThue, 14);
                SetMaxlength(txtDonVi, 200);
                SetMaxlength(txtDescription, 2000);
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
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetMaxlength(BaseEdit control, int maxlenght)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá kí tự cho phép", maxlenght);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_EditValueChanged(object sender, EventArgs e)
        {
            var payFormModel = payForm.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
            if (payFormModel.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
            {
                spinEditChuyenKhoan.Enabled = true;
            }
            else
            {
                spinEditChuyenKhoan.EditValue = "";
                spinEditChuyenKhoan.Enabled = false;
            }
        }

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            var payFormModel = payForm.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
            if (payFormModel.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
            {
                spinEditChuyenKhoan.Enabled = true;
                spinEditChuyenKhoan.Focus();
            }
            else
            {
                spinEditChuyenKhoan.EditValue = "";
                spinEditChuyenKhoan.Enabled = false;
                if (txtAccountBook.Enabled == true)
                {
                    txtAccountBook.Focus();
                    txtAccountBook.SelectAll();
                }
                else if (spNumberOrder.Enabled == true)
                {
                    spNumberOrder.Focus();
                    spNumberOrder.SelectAll();
                }
                else
                {
                    cboWorkingShift.Focus();
                    cboWorkingShift.ShowPopup();
                }
            }
        }

        private void spinEditChuyenKhoan_EditValueChanged(object sender, EventArgs e)
        {
            spinEditChuyenKhoan.EditValue = Inventec.Common.Number.Convert.NumberToString(spinEditChuyenKhoan.Value, ConfigApplications.NumberSeperator);
        }

        private void spinEditChuyenKhoan_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            DevExpress.XtraEditors.SpinEdit editor;
            editor = sender as DevExpress.XtraEditors.SpinEdit;
            if (Convert.ToDecimal(e.NewValue) > spinEditAmount.Value)
            {
                //e.Cancel = true;
                this.BeginInvoke(new MethodInvoker(MyMethod));
            }
        }
        public void MyMethod()
        {
            spinEditChuyenKhoan.EditValue = spinEditAmount.Value;
        }

        private void cboAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null && cboAccountBook.EditValue != cboAccountBook.OldEditValue)
                    {
                        V_HIS_ACCOUNT_BOOK account = accountBooks.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));
                        if (account != null)
                        {
                            txtAccountBook.Text = account.ACCOUNT_BOOK_CODE;
                            GlobalVariables.DefaultAccountBookTransactionBill = new List<V_HIS_ACCOUNT_BOOK>();
                            GlobalVariables.DefaultAccountBookTransactionBill.Add(account);
                            SetDataToDicNumOrderInAccountBook(account);
                            if (spNumberOrder.Enabled == true)
                                spNumberOrder.Focus();
                            else
                            {
                                cboWorkingShift.Focus();
                                cboWorkingShift.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        if (spNumberOrder.Enabled == true)
                            spNumberOrder.Focus();
                        else
                        {
                            cboWorkingShift.Focus();
                            cboWorkingShift.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboAccountBook.EditValue != null && cboAccountBook.EditValue != cboAccountBook.OldEditValue)
                    {
                        V_HIS_ACCOUNT_BOOK gt = accountBooks.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtAccountBook.Text = gt.ACCOUNT_BOOK_CODE;
                            if (spNumberOrder.Enabled == true)
                                spNumberOrder.Focus();
                            else
                            {
                                cboWorkingShift.Focus();
                                cboWorkingShift.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        if (spNumberOrder.Enabled == true)
                            spNumberOrder.Focus();
                        else
                        {
                            cboWorkingShift.Focus();
                            cboWorkingShift.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccountBook_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtAccountBook.Text))
                    {
                        string key = Inventec.Common.String.Convert.UnSignVNese(this.txtAccountBook.Text.ToLower().Trim());
                        var data = accountBooks.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.ACCOUNT_BOOK_CODE.ToLower()).Contains(key)).ToList();

                        List<V_HIS_ACCOUNT_BOOK> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.ACCOUNT_BOOK_CODE.ToLower() == txtAccountBook.Text).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            cboAccountBook.EditValue = result[0].ID;
                            txtAccountBook.Text = result[0].ACCOUNT_BOOK_CODE;
                            cboAccountBook.Focus();
                            if (spNumberOrder.Enabled == true)
                                spNumberOrder.Focus();
                            else
                                cboWorkingShift.ShowPopup();
                        }
                        else
                        {
                            cboAccountBook.EditValue = null;
                            cboAccountBook.Focus();
                            cboAccountBook.ShowPopup();
                        }
                    }
                    else
                    {
                        cboAccountBook.EditValue = null;
                        cboAccountBook.Focus();
                        cboAccountBook.ShowPopup();

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNguoiMua_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaSoThue.Focus();
                    txtMaSoThue.SelectAll();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaSoThue_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSTKNguoiMua.Focus();
                    txtSTKNguoiMua.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSTKNguoiMua_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDonVi.Focus();
                    txtDonVi.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDonVi_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiaChiNguoiMua.Focus();
                    txtDiaChiNguoiMua.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiaChiNguoiMua_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPayForm.Focus();
                    cboPayForm.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinEditChuyenKhoan.Enabled == true)
                    {
                        spinEditChuyenKhoan.Focus();
                        spinEditChuyenKhoan.SelectAll();
                    }
                    else if (txtAccountBook.Enabled == true)
                    {
                        txtAccountBook.Focus();
                        txtAccountBook.SelectAll();
                    }
                    else if (spNumberOrder.Enabled == true)
                    {
                        spNumberOrder.Focus();
                        spNumberOrder.SelectAll();
                    }
                    else
                    {
                        cboWorkingShift.Focus();
                        cboWorkingShift.ShowPopup();
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinEditChuyenKhoan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtAccountBook.Enabled == true)
                    {
                        txtAccountBook.Focus();
                        txtAccountBook.SelectAll();
                    }
                    else if (spNumberOrder.Enabled == true)
                    {
                        spNumberOrder.Focus();
                        spNumberOrder.SelectAll();
                    }
                    else
                    {
                        cboWorkingShift.Focus();
                        cboWorkingShift.ShowPopup();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spNumberOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboWorkingShift.Focus();
                    cboWorkingShift.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkingShift_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            cboRepayReason.Focus();
            cboRepayReason.ShowPopup();
            this.cboRepayReason.Properties.Buttons[1].Visible = true;
        }

        private void cboWorkingShift_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboWorkingShift.EditValue = null;
                    this.cboWorkingShift.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboRepayReason_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRepayReason.EditValue != null && cboRepayReason.EditValue != cboRepayReason.OldEditValue)
                    {
                        HIS_REPAY_REASON repayReason = this.repayReasons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRepayReason.EditValue.ToString()));
                        if (repayReason != null)
                        {
                            cboRepayReason.Properties.Buttons[1].Visible = true;
                            txtRepayReason.Text = repayReason.REPAY_REASON_CODE;
                            txtDescription.Focus();
                        }
                    }
                    else if (cboRepayReason.EditValue != null)
                    {
                        cboRepayReason.Properties.Buttons[1].Visible = true;
                        txtDescription.Focus();
                    }
                    else
                    {
                        txtDescription.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboRepayReason_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cboRepayReason.EditValue != null && cboRepayReason.EditValue != cboRepayReason.OldEditValue)
                {
                    HIS_REPAY_REASON repayReason = this.repayReasons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRepayReason.EditValue.ToString()));
                    if (repayReason != null)
                    {
                        cboRepayReason.Properties.Buttons[1].Visible = true;
                        txtRepayReason.Text = repayReason.REPAY_REASON_CODE;
                        txtDescription.Focus();
                    }
                }
                else if (cboRepayReason.EditValue != null)
                {
                    cboRepayReason.Properties.Buttons[1].Visible = true;
                    txtDescription.Focus();
                }
                else
                {
                    txtDescription.Focus();
                }
            }
        }

        private void txtRepayReason_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtRepayReason.Text))
                    {
                        string key = Inventec.Common.String.Convert.UnSignVNese(this.txtRepayReason.Text.ToLower().Trim());
                        var data = this.repayReasons.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.REPAY_REASON_CODE.ToLower()).Contains(key)).ToList();

                        List<HIS_REPAY_REASON> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.REPAY_REASON_CODE.ToLower() == txtRepayReason.Text).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            cboRepayReason.EditValue = result[0].ID;
                            cboRepayReason.Properties.Buttons[1].Visible = true;
                            txtRepayReason.Text = result[0].REPAY_REASON_CODE;
                            cboRepayReason.Focus();
                            txtDescription.Focus();
                        }
                        else
                        {
                            cboRepayReason.EditValue = null;
                            cboRepayReason.Properties.Buttons[1].Visible = false;
                            cboRepayReason.Focus();
                            cboRepayReason.ShowPopup();
                        }
                    }
                    else
                    {
                        cboRepayReason.EditValue = null;
                        cboRepayReason.Properties.Buttons[1].Visible = false;
                        cboRepayReason.Focus();
                        cboRepayReason.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRepayReason_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRepayReason.EditValue = null;
                    cboRepayReason.Properties.Buttons[1].Visible = false;
                    cboRepayReason.Focus();
                    txtRepayReason.Text = "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtBuyerEmail_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {              
                if(Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtBuyerEmail.Text.Trim(), 100))
                {
                    MessageBox.Show("Địa chỉ email không được vượt quá 100 ký tự!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    txtBuyerEmail.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }           
        }
    }
}
