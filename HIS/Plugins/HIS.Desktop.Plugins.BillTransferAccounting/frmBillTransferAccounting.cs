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
using AutoMapper;
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.LocalStorage.SdaConfigKey;
using HIS.Desktop.Plugins.BillTransferAccounting;
using HIS.Desktop.Plugins.BillTransferAccounting.Validtion;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BillTransferAccounting
{
    public partial class frmBillTransferAccounting : HIS.Desktop.Utility.FormBase
    {
        List<V_HIS_ACCOUNT_BOOK> listAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        V_HIS_TREATMENT_2 treatment;

        long treatmentId;
        long cashierRoomId;

        int positionHandleControl = -1;

        public frmBillTransferAccounting(Inventec.Desktop.Common.Modules.Module _Module, BillTransferADO data)
		:base(_Module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.treatmentId = data.TreatmentId;
                this.cashierRoomId = data.CashierRoomId;
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

        public void frmBillTransferAccounting_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadFrmKeyLanguage();
                ValidControl();
                LoadAccountBookToLocal();
                LoadDataToComboAccountBook();
                LoadDataToComboPayForm();
                SetDefaultPayForm();
                SetDefaultAccountBook();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAccountBookToLocal()
        {
            try
            {
                listAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
                HisAccountBookViewFilter accountBookFilter = new HisAccountBookViewFilter();
                accountBookFilter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                accountBookFilter.FOR_BILL = true;
                accountBookFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                accountBookFilter.IS_OUT_OF_BILL = false;
                listAccountBook = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumers.MosConsumer, accountBookFilter, null);
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
                cboAccountBook.Properties.DataSource = listAccountBook;
                cboAccountBook.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                cboAccountBook.Properties.ValueMember = "ID";
                cboAccountBook.Properties.ForceInitialize();
                cboAccountBook.Properties.Columns.Clear();
                cboAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_CODE", "", 50));
                cboAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_NAME", "", 100));
                cboAccountBook.Properties.ShowHeader = false;
                cboAccountBook.Properties.ImmediatePopup = true;
                cboAccountBook.Properties.DropDownRows = 10;
                cboAccountBook.Properties.PopupWidth = 150;
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
                cboPayForm.Properties.DataSource = BackendDataWorker.Get<HIS_PAY_FORM>();
                cboPayForm.Properties.DisplayMember = "PAY_FORM_NAME";
                cboPayForm.Properties.ValueMember = "ID";
                cboPayForm.Properties.ForceInitialize();
                cboPayForm.Properties.Columns.Clear();
                cboPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_CODE", "", 50));
                cboPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_NAME", "", 100));
                cboPayForm.Properties.ShowHeader = false;
                cboPayForm.Properties.ImmediatePopup = true;
                cboPayForm.Properties.DropDownRows = 10;
                cboPayForm.Properties.PopupWidth = 150;
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
                var data = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.PAY_FORM_CODE == Config.AppConfig.HIS_PAY_FORM_CODE__DEFAULT);
                if (data != null)
                {
                    txtPayFormCode.Text = data.PAY_FORM_CODE;
                    cboPayForm.EditValue = data.ID;
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
                var data = listAccountBook.FirstOrDefault();
                if (data != null)
                {
                    txtAccountBookCode.Text = data.ACCOUNT_BOOK_CODE;
                    cboAccountBook.EditValue = data.ID;
                    txtTotalFromNumberOder.Text = data.TOTAL + "/" + data.FROM_NUM_ORDER + "/" + (int)(data.CURRENT_NUM_ORDER ?? 0);
                }
                else
                {
                    txtTotalFromNumberOder.Text = "";
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
                ValidControlPayForm();
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
                accBookRule.txtAccountBookCode = txtAccountBookCode;
                accBookRule.cboAccountBook = cboAccountBook;
                dxValidationProvider2.SetValidationRule(txtAccountBookCode, accBookRule);
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
                payFormRule.txtPayFormCode = txtPayFormCode;
                payFormRule.cboPayForm = cboPayForm;
                dxValidationProvider2.SetValidationRule(txtPayFormCode, payFormRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                        var listData = listAccountBook.Where(o => o.ACCOUNT_BOOK_CODE.Contains(txtAccountBookCode.Text)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            txtAccountBookCode.Text = listData.First().ACCOUNT_BOOK_CODE;
                            cboAccountBook.EditValue = listData.First().ID;
                            txtTotalFromNumberOder.Text = listData.First().TOTAL + "/" + listData.First().FROM_NUM_ORDER + "/" + listData.First().CURRENT_NUM_ORDER;
                            txtPayFormCode.Focus();
                            txtPayFormCode.SelectAll();
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

        private void cboAccountBook_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null)
                    {
                        var accountBook = listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                        if (accountBook != null)
                        {
                            txtAccountBookCode.Text = accountBook.ACCOUNT_BOOK_CODE;
                            txtTotalFromNumberOder.Text = accountBook.TOTAL + "/" + accountBook.FROM_NUM_ORDER + "/" + accountBook.CURRENT_NUM_ORDER;
                        }
                    }
                    txtPayFormCode.Focus();
                    txtPayFormCode.SelectAll();
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
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtPayFormCode.Text))
                    {
                        var listData = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.PAY_FORM_CODE.Contains(txtPayFormCode.Text)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            txtPayFormCode.Text = listData.First().PAY_FORM_CODE;
                            cboPayForm.EditValue = listData.First().ID;
                            btnSave.Focus();
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (!valid)
                    {
                        cboPayForm.Focus();
                        cboPayForm.ShowPopup();
                    }
                }
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
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPayForm.EditValue != null)
                    {
                        var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                        if (payForm != null)
                        {
                            //btnSave_Click(null, null);
                            txtPayFormCode.Text = payForm.PAY_FORM_CODE;
                        }
                    }
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider2.Validate() || this.treatmentId <= 0)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                if (cboAccountBook.EditValue == null || cboPayForm.EditValue == null)
                {
                    MessageUtil.SetMessage(param, LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    goto End;
                }
                HisBillSDO data = new HisBillSDO();
                data.Bill = new HIS_BILL();
                data.SereServIds = new List<long>();
                data.Transaction = new HIS_TRANSACTION();
                data.Transaction.CASHIER_ROOM_ID = cashierRoomId;
                var account = listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (account != null)
                {
                    data.Transaction.ACCOUNT_BOOK_ID = account.ID;
                }

                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                if (payForm != null)
                {
                    data.Transaction.PAY_FORM_ID = payForm.ID;
                }

                data.Transaction.TREATMENT_ID = treatmentId;
                //data.Bill.IS_TRANSFER_ACCOUNTING = IMSys.DbConfig.HIS_RS.HIS_BILL.IS_TRANSFER_ACCOUNTING__TRUE;
                //data.Bill.KC_AMOUNT = data.Transaction.AMOUNT;
                data.IsTransferAccounting = true;

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_BILL>(HisRequestUriStore.HIS_BILL_CREATE, ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                }

            End:
                WaitingManager.Hide();
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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

        private void dxValidationProvider2_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        private void LoadFrmKeyLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BillTransferAccounting.Resources.Lang", typeof(HIS.Desktop.Plugins.BillTransferAccounting.frmBillTransferAccounting).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.cboPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.cboAccountBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutAccountBook.Text = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.layoutAccountBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutNumberOrder.Text = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.layoutNumberOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalFromNumberOder.Text = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.layoutTotalFromNumberOder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutPayForm.Text = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.layoutPayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.bbtnRCSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmBillTransferAccounting.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPayFormCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //btnSave_Click(null, null);
                btnSave.Focus();
                //    //textBox2.Focus();
            }
        }

        private void btnSave_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    btnSave_Click(null, null);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
