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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.BackendData.Core.TestDeviceSample;
using HIS.Desktop.Plugins.TestDeviceSample.Resources;
using HIS.Desktop.Plugins.TestDeviceSample.Worker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TestDeviceSample
{
    /// <summary>
    /// Accounts signed in on the scanners of this workstation (sessions created by TestConnectDeviceSample).
    /// The grid shows copies of the shared session list and refreshes itself while the form is open.
    /// "Đăng xuất" removes one session; the port stays connected so another account can scan its card.
    /// </summary>
    public partial class frmTestDeviceSample : HIS.Desktop.Utility.FormBase
    {
        // Rows drawn last time (port;account;start), to rebind only when the sessions changed
        private string currentSignature = null;

        public frmTestDeviceSample(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
        }

        private void frmTestDeviceSample_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetCaptionByLanguageKey();
                LoadDataToGrid(true);
                this.timerRefresh.Start();
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.lblHint.Text = GetLanguageText("frmTestDeviceSample.lblHint.Text");
                this.gcStt.Caption = GetLanguageText("frmTestDeviceSample.gcStt.Caption");
                this.gcPortName.Caption = GetLanguageText("frmTestDeviceSample.gcPortName.Caption");
                this.gcLoginname.Caption = GetLanguageText("frmTestDeviceSample.gcLoginname.Caption");
                this.gcUsername.Caption = GetLanguageText("frmTestDeviceSample.gcUsername.Caption");
                this.gcSessionTime.Caption = GetLanguageText("frmTestDeviceSample.gcSessionTime.Caption");
                this.repositoryItemButtonLogout.Buttons[0].Caption = GetLanguageText("frmTestDeviceSample.repositoryItemButtonLogout.Caption");
                this.btnRefesh.Text = GetLanguageText("frmTestDeviceSample.btnRefesh.Text");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLanguageText(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, Base.ResourceLangManager.LanguageUCTestDeviceSample,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>
        /// Signed-in sessions only (an open port without an account is not a row).
        /// </summary>
        private void LoadDataToGrid(bool force)
        {
            try
            {
                List<TestDeviceSampleADO> rows = TestDeviceSampleDataWorker.TestDeviceSamples
                    .Where(o => !String.IsNullOrEmpty(o.Loginname))
                    .OrderBy(o => (o.TeminalCode ?? "").Length).ThenBy(o => o.TeminalCode, StringComparer.OrdinalIgnoreCase)
                    .Select(o => new TestDeviceSampleADO
                    {
                        TeminalCode = o.TeminalCode,
                        Loginname = o.Loginname,
                        Username = o.Username,
                        SessionTimeStart = o.SessionTimeStart
                    })
                    .ToList();
                string signature = String.Join("|", rows.Select(o => o.TeminalCode + ";" + o.Loginname + ";" + o.SessionTimeStart));
                if (!force && signature == this.currentSignature)
                    return;

                this.currentSignature = signature;
                this.gridViewTestDeviceSample.BeginUpdate();
                try
                {
                    this.gridControlTestDeviceSample.DataSource = rows;
                }
                finally
                {
                    this.gridViewTestDeviceSample.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonLogout_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                TestDeviceSampleADO row = this.gridViewTestDeviceSample.GetFocusedRow() as TestDeviceSampleADO;
                if (row == null)
                    return;

                string userText = String.Format("{0} ({1})", row.Username, row.Loginname);
                if (XtraMessageBox.Show(String.Format(ResourceMessage.XacNhanDangXuat, userText, row.TeminalCode),
                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                // Same port, account and start: the port may have been signed in again since the row was drawn
                TestDeviceSampleADO session = TestDeviceSampleDataWorker.TestDeviceSamples.FirstOrDefault(o =>
                    String.Equals(o.TeminalCode, row.TeminalCode, StringComparison.OrdinalIgnoreCase)
                    && String.Equals(o.Loginname, row.Loginname, StringComparison.OrdinalIgnoreCase)
                    && o.SessionTimeStart == row.SessionTimeStart);
                if (session == null)
                {
                    XtraMessageBox.Show(ResourceMessage.PhienDaThayDoi,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                }
                else
                {
                    TestDeviceSampleDataWorker.TestDeviceSamples.Remove(session);
                    EventLogWorker.Log(String.Format(ResourceMessage.NhatKyDangXuat, row.TeminalCode, userText, ResourceMessage.LyDoDangXuatTaiMan));
                }
                LoadDataToGrid(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGrid(false);
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
                if (btnRefesh.Enabled)
                    btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGrid(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTestDeviceSample_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData || e.Column.UnboundType == UnboundColumnType.Bound)
                    return;
                TestDeviceSampleADO dataRow = (TestDeviceSampleADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1;
                else if (e.Column.FieldName == "SessionTimeStartDisplay")
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.SessionTimeStart);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
