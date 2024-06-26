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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using System.Text;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.UserInfo.UserInfo
{
    public partial class frmUserInfo : Form
    {
        HIS_EMPLOYEE currentEmployee { get; set; }
        public List<HIS_DEPARTMENT> ListDepartment { get; private set; }
        public List<V_HIS_MEDI_STOCK> SelectMediStock { get; private set; }
        public List<V_HIS_MEDI_STOCK> MediStockDefaut { get; private set; }

        public frmUserInfo()
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void InitComboDepartment()
        {

            ListDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(cboDepartment, ListDepartment.ToList(), controlEditorADO);
            cboDepartment.Properties.ImmediatePopup = true;
        }

        private void LoadDataMediStock()
        {
            try
            {
                this.MediStockDefaut = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCheck(DevExpress.XtraEditors.GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionMediStock(object sender, EventArgs e)
        {
            try
            {
                this.SelectMediStock = new List<V_HIS_MEDI_STOCK>();
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<V_HIS_MEDI_STOCK> sgSelectedNews = new List<V_HIS_MEDI_STOCK>();
                    foreach (V_HIS_MEDI_STOCK rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0)
                            {
                                sb.Append(";");
                            }
                            sb.Append(rv.MEDI_STOCK_NAME.ToString());
                            sgSelectedNews.Add(rv);

                        }

                    }
                    this.SelectMediStock.AddRange(sgSelectedNews);

                }
                this.cboMediStock.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCombo(DevExpress.XtraEditors.GridLookUpEdit cbo, List<V_HIS_MEDI_STOCK> data)
        {
            try
            {
                if (data != null)
                {
                    cbo.Properties.DataSource = data;
                    cbo.Properties.DisplayMember = "MEDI_STOCK_NAME";
                    cbo.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "Tất cả";
                    cbo.Properties.PopupFormWidth = 200;
                    cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                    cbo.Properties.View.OptionsSelection.MultiSelect = true;
                    cbo.Properties.ImmediatePopup = true;

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmUserInfo_Load(object sender, EventArgs e)
        {
            try
            {
                Config.ConfigCFG.LoadConfig();
                SetCaptionByLanguageKey();
                ValidateForm();
                InitComboDepartment();
                LoadDataMediStock();
                InitCheck(cboMediStock, SelectionMediStock);
                InitCombo(cboMediStock, this.MediStockDefaut);
                currentEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME.Equals(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()));
                timer1.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboStock_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (V_HIS_MEDI_STOCK rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboDepartment.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.SelectMediStock = new List<V_HIS_MEDI_STOCK>();
                    RestCombo(cboMediStock);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void RestCombo(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
                cbo.EditValue = null;
                cbo.Focus();
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
                btnSave1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate())
                    return;
                if (!string.IsNullOrEmpty(Config.ConfigCFG.InterconnectionPrescription) && Config.ConfigCFG.InterconnectionPrescription.Split('|').Count() >= 3)
                {
                    if (!string.IsNullOrEmpty(txtErxLoginName.Text.Trim()) && !string.IsNullOrEmpty(txtErxPassWord.Text.Trim()))
                    {
                        var cfg = Config.ConfigCFG.InterconnectionPrescription.Split('|');
                        var message = (new ERXConnect.ERXConnectProcessor()).Login(new ERXConnect.DataAuth() { Loginname = txtErxLoginName.Text.Trim(), Password = txtErxPassWord.Text.Trim(), Url = cfg[0], HospitalLoginname = cfg[1], HospitalPassword = cfg[2] });
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => message), message));
                        if (string.IsNullOrEmpty(message))
                        {
                            XtraMessageBox.Show("Mã liên thông hoặc mật khẩu không đúng.");
                            return;
                        }
                    }
                }
                WaitingManager.Show();
                HIS_EMPLOYEE data = new HIS_EMPLOYEE();
                List<string> valueChange = new List<string>();
                if (this.currentEmployee != null)
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EMPLOYEE>(data, currentEmployee);
                data.LOGINNAME = !string.IsNullOrEmpty(txtLoginName.Text.Trim()) ? txtLoginName.Text.Trim() : null;
                data.TDL_USERNAME = !string.IsNullOrEmpty(txtUserName.Text.Trim()) ? txtUserName.Text.Trim() : null;
                if (!data.TDL_USERNAME.Equals(currentEmployee.TDL_USERNAME))
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Họ và tên", currentEmployee.TDL_USERNAME ?? "null", data.TDL_USERNAME ?? "null"));
                if (dteDob.EditValue != null && dteDob.DateTime != DateTime.MinValue)
                    data.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteDob.DateTime);
                else
                    data.DOB = null;
                if (data.DOB != currentEmployee.DOB)
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Ngày sinh", currentEmployee.DOB.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentEmployee.DOB ?? 0) : "null", data.DOB.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0) : "null"));
                data.TDL_EMAIL = !string.IsNullOrEmpty(txtEmail.Text.Trim()) ? txtEmail.Text.Trim() : null;
                if (data.TDL_EMAIL != (currentEmployee.TDL_EMAIL))
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Email", currentEmployee.TDL_EMAIL ?? "null", data.TDL_EMAIL ?? "null"));
                data.TDL_MOBILE = !string.IsNullOrEmpty(txtPhone.Text.Trim()) ? txtPhone.Text.Trim() : null;
                if (data.TDL_MOBILE != (currentEmployee.TDL_MOBILE))
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Điện thoại", currentEmployee.TDL_MOBILE ?? "null", data.TDL_MOBILE ?? "null"));
                data.TITLE = !string.IsNullOrEmpty(txtTitle.Text.Trim()) ? txtTitle.Text.Trim() : null;
                if (data.TITLE != (currentEmployee.TITLE))
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Chức danh", currentEmployee.TITLE ?? "null", data.TITLE ?? "null"));
                data.SOCIAL_INSURANCE_NUMBER = !string.IsNullOrEmpty(txtSocialNumber.Text.Trim()) ? txtSocialNumber.Text.Trim() : null;
                if (data.SOCIAL_INSURANCE_NUMBER != (currentEmployee.SOCIAL_INSURANCE_NUMBER))
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Số BHXH", currentEmployee.SOCIAL_INSURANCE_NUMBER ?? "null", data.SOCIAL_INSURANCE_NUMBER ?? "null"));
                data.DIPLOMA = !string.IsNullOrEmpty(txtDiploma.Text.Trim()) ? txtDiploma.Text.Trim() : null;
                if (data.DIPLOMA != (currentEmployee.DIPLOMA))
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Chứng chỉ", currentEmployee.DIPLOMA ?? "null", data.DIPLOMA ?? "null"));
                data.BANK = !string.IsNullOrEmpty(txtBank.Text.Trim()) ? txtBank.Text.Trim() : null;
                if (data.BANK != (currentEmployee.BANK))
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Ngân hàng", currentEmployee.BANK ?? "null", data.BANK ?? "null"));
                if (cboDepartment.EditValue != null)
                    data.DEPARTMENT_ID = Int64.Parse(cboDepartment.EditValue.ToString());
                else
                    data.DEPARTMENT_ID = null;
                if (data.DEPARTMENT_ID != currentEmployee.DEPARTMENT_ID)
                {
                    string deNameCurrent = null;
                    if (currentEmployee.DEPARTMENT_ID.HasValue && ListDepartment.FirstOrDefault(o => o.ID != currentEmployee.DEPARTMENT_ID) != null)
                        deNameCurrent = ListDepartment.FirstOrDefault(o => o.ID != currentEmployee.DEPARTMENT_ID).DEPARTMENT_NAME;
                    string deNameData = null;
                    if (data.DEPARTMENT_ID.HasValue && ListDepartment.FirstOrDefault(o => o.ID != data.DEPARTMENT_ID) != null)
                        deNameData = ListDepartment.FirstOrDefault(o => o.ID != data.DEPARTMENT_ID).DEPARTMENT_NAME;
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Khoa", deNameCurrent ?? "null", deNameData ?? "null"));
                }
                if (SelectMediStock != null && SelectMediStock.Count > 0)
                {
                    data.DEFAULT_MEDI_STOCK_IDS = String.Join(",", SelectMediStock.Select(o => o.ID));
                }
                else
                    data.DEFAULT_MEDI_STOCK_IDS = null;
                string stockData = null;
                string stockCurrent = null;
                if (string.IsNullOrEmpty(data.DEFAULT_MEDI_STOCK_IDS) && !string.IsNullOrEmpty(currentEmployee.DEFAULT_MEDI_STOCK_IDS))
                {
                    stockCurrent = string.Join(", ", MediStockDefaut.Where(o => currentEmployee.DEFAULT_MEDI_STOCK_IDS.Split(',').ToList().Exists(p => p == o.ID.ToString())).Select(o => o.MEDI_STOCK_NAME));
                }
                else if (!string.IsNullOrEmpty(data.DEFAULT_MEDI_STOCK_IDS) && string.IsNullOrEmpty(currentEmployee.DEFAULT_MEDI_STOCK_IDS))
                {
                    stockData = string.Join(", ", SelectMediStock.Select(o => o.MEDI_STOCK_NAME));
                }
                else if (!string.IsNullOrEmpty(data.DEFAULT_MEDI_STOCK_IDS) && data.DEFAULT_MEDI_STOCK_IDS != currentEmployee.DEFAULT_MEDI_STOCK_IDS)
                {
                    stockData = string.Join(", ", SelectMediStock.Select(o => o.MEDI_STOCK_NAME));
                    stockCurrent = string.Join(", ", MediStockDefaut.Where(o => (currentEmployee.DEFAULT_MEDI_STOCK_IDS ?? " ").Split(',').ToList().Exists(p => p == o.ID.ToString())).Select(o => o.MEDI_STOCK_NAME));
                }
                if (!string.IsNullOrEmpty(stockData) || !string.IsNullOrEmpty(stockCurrent))
                {
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Kho mặc định", stockCurrent ?? "null", stockData ?? "null"));
                }
                data.ACCOUNT_NUMBER = !string.IsNullOrEmpty(txtAccountNum.Text.Trim()) ? txtAccountNum.Text.Trim() : null;
                if (data.ACCOUNT_NUMBER != (currentEmployee.ACCOUNT_NUMBER))
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Số tài khoản", currentEmployee.ACCOUNT_NUMBER ?? "null", data.ACCOUNT_NUMBER ?? "null"));
                data.ERX_LOGINNAME = !string.IsNullOrEmpty(txtErxLoginName.Text.Trim()) ? txtErxLoginName.Text.Trim() : null;
                if (data.ERX_LOGINNAME != (currentEmployee.ERX_LOGINNAME))
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Tên đăng nhập ERX", currentEmployee.ERX_LOGINNAME ?? "null", data.ERX_LOGINNAME ?? "null"));
                data.ERX_PASSWORD = !string.IsNullOrEmpty(txtErxPassWord.Text.Trim()) ? txtErxPassWord.Text.Trim() : null;
                if (data.ERX_PASSWORD != (currentEmployee.ERX_PASSWORD))
                    valueChange.Add(string.Format("{0}: {1} => {2}", "Mật khẩu ERX", currentEmployee.ERX_PASSWORD ?? "null", data.ERX_PASSWORD ?? "null"));
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>
                        ("api/HisEmployee/Update", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                        data, param);

                WaitingManager.Hide();
                if (result != null)
                {
                    BackendDataWorker.Reset<HIS_EMPLOYEE>();
                    CallSda(valueChange);
                }
                MessageManager.Show(this, param, result != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

                WaitingManager.Hide();
            }
        }
        private void CallSda(List<string> lst)
        {
            try
            {
                string message = string.Format("Sửa tài khoản nhân viên. LOGINNAME {0}. {1}", currentEmployee.LOGINNAME, (lst != null && lst.Count > 0) ? string.Join(", ", lst) : null);
                string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SdaEventLogCreate eventlog = new SdaEventLogCreate();
                eventlog.Create(login, null, true, message);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReset1_Click(object sender, EventArgs e)
        {
            try
            {
                txtUserName.Text = null;
                dteDob.EditValue = null;
                txtPhone.Text = null;
                txtEmail.Text = null;
                txtDiploma.Text = null;
                txtTitle.Text = null;
                txtAccountNum.Text = null;
                txtBank.Text = null;
                cboDepartment.EditValue = null;
                RestCombo(cboMediStock);
                txtUserName.Focus();
                txtSocialNumber.Text = null;
                txtErxLoginName.Text = null;
                txtErxPassWord.Text = null;
                dxValidationProvider1.RemoveControlError(txtEmail);
                dxValidationProvider1.RemoveControlError(txtUserName);
                dxValidationProvider1.RemoveControlError(txtSocialNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave2_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset2_Click(object sender, EventArgs e)
        {
            try
            {
                btnReset1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                btnReset1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {

        }
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtUserName, dxValidationProvider1, "", validUsername);
                ValidationEmail(txtEmail);
                validMalength(this.txtErxLoginName, 100);
                validMalength(this.txtErxPassWord, 400);
                validMalength(this.txtTitle, 100);
                validMalength(this.txtDiploma, 50);
                ValidationBhxh(this.txtSocialNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationBhxh(TextEdit txt)
        {
            try
            {
                ValidateBhxh validRule = new ValidateBhxh();
                validRule.txt = txt;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationEmail(TextEdit control)
        {
            if (control.Text != null || control.Text.Equals(""))
            {
                ValidateEmail validRule = new ValidateEmail();
                validRule.txt = control;
                validRule.ErrorText = "E-mail sai định dạng";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
        }

        private void validMalength(BaseEdit control, int? maxLength)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                //validate.IsRequired = true;
                validate.ErrorText = "Nhập quá kí tự cho phép";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        bool validUsername()
        {
            bool valid = true;
            int? textLength = Inventec.Common.String.CountVi.Count(txtUserName.Text.Trim());
            if (txtUserName.Text.Trim().Equals(""))
                valid = false;
            if (textLength == null || textLength >= 50)
                valid = false;
            return valid;
        }
        protected void ValidationSingleControl(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, string messageErr, IsValidControl isValidControl)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                if (isValidControl != null)
                {
                    validRule.isUseOnlyCustomValidControl = true;
                    validRule.isValidControl = isValidControl;
                }
                if (!String.IsNullOrEmpty(messageErr))
                    validRule.ErrorText = messageErr;
                else
                    validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                txtLoginName.Text = currentEmployee.LOGINNAME;
                txtUserName.Text = currentEmployee.TDL_USERNAME;
                if (currentEmployee.DOB.HasValue)
                    dteDob.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentEmployee.DOB ?? 0) ?? DateTime.MinValue;
                txtEmail.Text = currentEmployee.TDL_EMAIL;
                txtPhone.Text = currentEmployee.TDL_MOBILE;
                txtDiploma.Text = currentEmployee.DIPLOMA;
                txtTitle.Text = currentEmployee.TITLE;
                txtAccountNum.Text = currentEmployee.ACCOUNT_NUMBER;
                txtBank.Text = currentEmployee.BANK;
                cboDepartment.EditValue = currentEmployee.DEPARTMENT_ID;
                this.SelectMediStock = new List<V_HIS_MEDI_STOCK>();
                if (currentEmployee.DEFAULT_MEDI_STOCK_IDS != null && currentEmployee.DEFAULT_MEDI_STOCK_IDS.Length > 0)
                {
                    string[] array = currentEmployee.DEFAULT_MEDI_STOCK_IDS.Split(',');
                    if (array != null && array.Count() > 0)
                    {
                        for (int i = 0; i < array.Count(); i++)
                        {
                            V_HIS_MEDI_STOCK data = new V_HIS_MEDI_STOCK();
                            data = this.MediStockDefaut.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(array[i]));
                            this.SelectMediStock.Add(data);
                        }
                    }

                }
                GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(this.SelectMediStock);
                }
                txtSocialNumber.Text = currentEmployee.SOCIAL_INSURANCE_NUMBER;
                txtErxLoginName.Text = currentEmployee.ERX_LOGINNAME;
                txtErxPassWord.Text = currentEmployee.ERX_PASSWORD;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dteDob.Focus();
                    dteDob.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmail_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPhone.Focus();
                    txtPhone.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiploma.Focus();
                    txtDiploma.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiploma_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTitle.Focus();
                    txtTitle.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTitle_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountNum.Focus();
                    txtAccountNum.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccountNum_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBank.Focus();
                    txtBank.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBank_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void cboMediStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtSocialNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        } /// <summary>
          ///Hàm xét ngôn ngữ cho giao diện frmUserInfo
          /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.UserInfo.Resources.Lang", typeof(frmUserInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset1.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.btnReset1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave1.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.btnSave1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("frmUserInfo.cboMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmUserInfo.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem13.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset2.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.btnReset2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave2.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.btnSave2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmUserInfo.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmUserInfo.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmUserInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

