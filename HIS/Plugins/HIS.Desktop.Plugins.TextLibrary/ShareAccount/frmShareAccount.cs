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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TextLibrary.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TextLibrary.ShareAccount
{
    /// <summary>
    /// Popup "Account list": choose the employee accounts a text library item is shared with (PUBLIC_LOGINNAMES).
    /// </summary>
    public partial class frmShareAccount : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        /// <summary>Max length of HIS_TEXT_LIB.PUBLIC_LOGINNAMES (VARCHAR2(4000)).</summary>
        internal const int PUBLIC_LOGINNAMES_MAX_LENGTH = 4000;

        List<string> inputLoginnames;
        string creatorLoginname;
        HIS.Desktop.Common.DelegateSelectData delegateSelect;
        List<ShareAccountADO> listAccount = new List<ShareAccountADO>();
        List<ShareAccountADO> listAccountShowing = new List<ShareAccountADO>();
        bool isUpdatingSelectAll = false;
        #endregion

        #region Construct
        public frmShareAccount()
        {
            InitializeComponent();
        }

        /// <param name="module">Module of the Content Library form</param>
        /// <param name="loginnames">Login names currently shared</param>
        /// <param name="creatorLoginname">Creator of the text library item: not listed, never returned</param>
        /// <param name="delegateSelect">Receives the chosen List&lt;string&gt; when the user presses OK</param>
        public frmShareAccount(Inventec.Desktop.Common.Modules.Module module, List<string> loginnames, string creatorLoginname, HIS.Desktop.Common.DelegateSelectData delegateSelect)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.inputLoginnames = loginnames ?? new List<string>();
                this.creatorLoginname = creatorLoginname;
                this.delegateSelect = delegateSelect;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmShareAccount_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitComboDepartment();
                LoadAccounts();
                FillDataToGrid();
                txtKeyword.Focus();
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TextLibrary.Resources.Lang", typeof(frmShareAccount).Assembly);

                this.txtKeyword.Properties.NullValuePrompt = GetLanguageValue("frmShareAccount.txtKeyword.Properties.NullValuePrompt");
                this.lciDepartment.Text = GetLanguageValue("frmShareAccount.lciDepartment.Text");
                this.chkSelectAll.Properties.Caption = GetLanguageValue("frmShareAccount.chkSelectAll.Properties.Caption");
                this.gcEmployeeCode.Caption = GetLanguageValue("frmShareAccount.gcEmployeeCode.Caption");
                this.gcUserName.Caption = GetLanguageValue("frmShareAccount.gcUserName.Caption");
                this.gcLoginName.Caption = GetLanguageValue("frmShareAccount.gcLoginName.Caption");
                this.gcDepartmentName.Caption = GetLanguageValue("frmShareAccount.gcDepartmentName.Caption");
                this.btnOk.Text = GetLanguageValue("frmShareAccount.btnOk.Text");
                this.btnClose.Text = GetLanguageValue("frmShareAccount.btnClose.Text");
                this.Text = GetLanguageValue("frmShareAccount.Text");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDepartment()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", GetLanguageValue("frmShareAccount.cboDepartment.ColumnCode"), 80, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", GetLanguageValue("frmShareAccount.cboDepartment.ColumnName"), 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, true, 330);
                ControlEditorLoader.Load(cboDepartment, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Active, not deleted employee accounts, except the creator of the item; already shared ones are pre-checked.
        /// </summary>
        private void LoadAccounts()
        {
            try
            {
                HashSet<string> selected = new HashSet<string>(this.inputLoginnames);
                this.listAccount = BackendDataWorker.Get<V_HIS_EMPLOYEE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE
                        && !String.IsNullOrWhiteSpace(o.LOGINNAME)
                        && o.LOGINNAME != this.creatorLoginname)
                    .OrderBy(o => o.TDL_USERNAME).ThenBy(o => o.LOGINNAME)
                    .Select(o => new ShareAccountADO(o)
                    {
                        IsChecked = selected.Contains(o.LOGINNAME),
                        SearchText = BuildSearchText(o.EMPLOYEE_CODE + " " + o.TDL_USERNAME + " " + o.LOGINNAME)
                    })
                    .ToList();
                Inventec.Common.Logging.LogSystem.Debug("frmShareAccount: listAccount.Count=" + this.listAccount.Count + ", inputLoginnames.Count=" + this.inputLoginnames.Count);
            }
            catch (Exception ex)
            {
                this.listAccount = new List<ShareAccountADO>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Filter
        internal static string BuildSearchText(string text)
        {
            try
            {
                return Inventec.Common.String.Convert.UnSignVNese((text ?? "").Trim().ToLower());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (text ?? "").Trim().ToLower();
        }

        /// <summary>
        /// Rebinds the grid with a sub-list of the SAME ADO objects, so checks outside the filter result are kept.
        /// </summary>
        private void FillDataToGrid()
        {
            try
            {
                gridViewAccount.CloseEditor();
                string keyword = BuildSearchText(txtKeyword.Text);
                long departmentId = cboDepartment.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString()) : 0;
                this.listAccountShowing = this.listAccount
                    .Where(o => (keyword.Length == 0 || (o.SearchText != null && o.SearchText.Contains(keyword)))
                        && (departmentId <= 0 || o.DEPARTMENT_ID == departmentId))
                    .ToList();

                gridViewAccount.BeginUpdate();
                gridControlAccount.DataSource = this.listAccountShowing;
                gridViewAccount.EndUpdate();
                UpdateSelectAllState();
            }
            catch (Exception ex)
            {
                gridViewAccount.EndUpdate();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateSelectAllState()
        {
            try
            {
                this.isUpdatingSelectAll = true;
                chkSelectAll.Checked = this.listAccountShowing.Count > 0 && this.listAccountShowing.All(o => o.IsChecked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isUpdatingSelectAll = false;
            }
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
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
                {
                    cboDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isUpdatingSelectAll) return;
                gridViewAccount.CloseEditor();
                foreach (var item in this.listAccountShowing)
                {
                    item.IsChecked = chkSelectAll.Checked;
                }
                gridViewAccount.RefreshData();
                // An empty filter result must not leave "Select all" checked
                UpdateSelectAllState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemChkChoose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // Post the clicked check box value to the ADO right away so "Select all" and OK see it
                gridViewAccount.PostEditor();
                UpdateSelectAllState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Ok / Close
        /// <summary>
        /// Result = input login names that are not listed in the popup (locked / removed accounts are kept)
        /// or still checked, then the newly checked ones; the creator is always removed, duplicates dropped.
        /// </summary>
        internal static List<string> MergeSelectedLoginnames(List<string> inputLoginnames, List<ShareAccountADO> accounts, string creatorLoginname)
        {
            List<string> result = new List<string>();
            try
            {
                HashSet<string> listed = new HashSet<string>(accounts.Select(o => o.LOGINNAME));
                HashSet<string> checkedLogins = new HashSet<string>(accounts.Where(o => o.IsChecked).Select(o => o.LOGINNAME));
                HashSet<string> added = new HashSet<string>();
                foreach (var login in (inputLoginnames ?? new List<string>()))
                {
                    if (!String.IsNullOrWhiteSpace(login) && login != creatorLoginname
                        && (!listed.Contains(login) || checkedLogins.Contains(login)) && added.Add(login))
                    {
                        result.Add(login);
                    }
                }
                foreach (var item in accounts)
                {
                    if (item.IsChecked && item.LOGINNAME != creatorLoginname && added.Add(item.LOGINNAME))
                    {
                        result.Add(item.LOGINNAME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewAccount.CloseEditor();
                List<string> result = MergeSelectedLoginnames(this.inputLoginnames, this.listAccount, this.creatorLoginname);
                // VARCHAR2(4000) counts bytes by default; same as characters for ASCII login names
                int length = System.Text.Encoding.UTF8.GetByteCount(String.Join(",", result));
                if (length > PUBLIC_LOGINNAMES_MAX_LENGTH)
                {
                    Inventec.Common.Logging.LogSystem.Info("frmShareAccount: PUBLIC_LOGINNAMES too long, length=" + length + ", count=" + result.Count);
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        String.Format(Resources.ResourceMessage.DanhSachTaiKhoanChiaSeVuotQuaGioiHan, PUBLIC_LOGINNAMES_MAX_LENGTH, length),
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.OK);
                    return;
                }
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(result);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
