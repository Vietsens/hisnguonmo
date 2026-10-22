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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TestConnectDeviceSample.ADO;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Base;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Resources;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Validation;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Worker;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample
{
    /// <summary>
    /// Connects several barcode scanners (COM mode), binds each port to the account whose card is scanned on it,
    /// and opens "Thông tin lấy mẫu" prefilled with that account when a sample barcode is scanned.
    /// Partials: ___Port (open, close, lost COM ports), ___Scan (scan queue and rules), ___View (tiles, scan log, alerts).
    /// </summary>
    public partial class UCTestConnectDeviceSample : UserControlBase
    {
        #region Declare
        // Open readers by port name. A null reader is a port attached by the test harness (no hardware).
        private readonly Dictionary<string, ComPortReader> readers = new Dictionary<string, ComPortReader>(StringComparer.OrdinalIgnoreCase);
        // Ports whose scanner vanished: the tile stays red until the user reconnects or disconnects the port
        private readonly HashSet<string> lostPorts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private int positionHandleControl = -1;
        private V_HIS_ROOM room;
        private Form deviceListForm;
        #endregion

        public UCTestConnectDeviceSample()
            : base()
        {
            InitializeComponent();
        }

        public UCTestConnectDeviceSample(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
        }

        private void UCTestConnectDeviceSample_Load(object sender, EventArgs e)
        {
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetCaptionByLanguageKey();
                ValidControl();
                LoadRoom();
                RefreshPortList();
                RefreshTiles();
                UpdatePendingLabel();
                this.timerPortCheck.Start();
                cboPortCom.Focus();
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
                this.lblGuide.Text = GetLanguageText("UCTestConnectDeviceSample.lblGuide.Text");
                this.lciPortCom.Text = GetLanguageText("UCTestConnectDeviceSample.lciPortCom.Text");
                this.cboPortCom.Properties.NullText = GetLanguageText("UCTestConnectDeviceSample.cboPortCom.NullText");
                this.btnConnect.Text = GetLanguageText("UCTestConnectDeviceSample.btnConnect.Text");
                this.btnDisconnect.Text = GetLanguageText("UCTestConnectDeviceSample.btnDisconnect.Text");
                this.btnDisconnect.ToolTip = GetLanguageText("UCTestConnectDeviceSample.btnDisconnect.ToolTip");
                this.btnRefresh.Text = GetLanguageText("UCTestConnectDeviceSample.btnRefresh.Text");
                this.btnDeviceList.Text = GetLanguageText("UCTestConnectDeviceSample.btnDeviceList.Text");
                this.btnDeviceList.ToolTip = GetLanguageText("UCTestConnectDeviceSample.btnDeviceList.ToolTip");
                this.lciTilePorts.Text = GetLanguageText("UCTestConnectDeviceSample.lcgPorts.Text");
                this.lciScanLog.Text = GetLanguageText("UCTestConnectDeviceSample.lcgScanLog.Text");
                this.gcScanTime.Caption = GetLanguageText("UCTestConnectDeviceSample.gcScanTime.Caption");
                this.gcPortName.Caption = GetLanguageText("UCTestConnectDeviceSample.gcPortName.Caption");
                this.gcCode.Caption = GetLanguageText("UCTestConnectDeviceSample.gcCode.Caption");
                this.gcResult.Caption = GetLanguageText("UCTestConnectDeviceSample.gcResult.Caption");
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
                return Inventec.Common.Resource.Get.Value(key, Base.ResourceLangManager.LanguageUCTestConnectDeviceSample,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        private void ValidControl()
        {
            try
            {
                PortComValidationRule portRule = new PortComValidationRule();
                portRule.cboPortCom = cboPortCom;
                dxValidationProvider1.SetValidationRule(cboPortCom, portRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadRoom()
        {
            try
            {
                if (this.currentModuleBase != null)
                    this.room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModuleBase.RoomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ports known to Windows plus the ones already open (a lost port must stay selectable to disconnect it).
        /// Ports are not test-opened: opening a port in use by another program could disturb it.
        /// </summary>
        private void RefreshPortList()
        {
            try
            {
                string selected = SelectedPortName();
                List<string> ports = SerialPort.GetPortNames()
                    .Concat(this.readers.Keys)
                    .Where(o => !String.IsNullOrWhiteSpace(o))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(o => o.Length).ThenBy(o => o, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                cboPortCom.Properties.Items.BeginUpdate();
                try
                {
                    cboPortCom.Properties.Items.Clear();
                    cboPortCom.Properties.Items.AddRange(ports);
                }
                finally
                {
                    cboPortCom.Properties.Items.EndUpdate();
                }
                cboPortCom.EditValue = ports.FirstOrDefault(o => String.Equals(o, selected, StringComparison.OrdinalIgnoreCase));
                UpdateButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string SelectedPortName()
        {
            try
            {
                return cboPortCom.EditValue != null ? cboPortCom.EditValue.ToString().Trim() : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        private void SelectPort(string portName)
        {
            try
            {
                if (!cboPortCom.Properties.Items.Contains(portName))
                    cboPortCom.Properties.Items.Add(portName);
                cboPortCom.EditValue = portName;
                UpdateButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateButtonState()
        {
            try
            {
                string portName = SelectedPortName();
                bool isOpen = !String.IsNullOrEmpty(portName) && this.readers.ContainsKey(portName);
                btnConnect.Enabled = !isOpen || this.lostPorts.Contains(portName);
                btnDisconnect.Enabled = isOpen;
                foreach (DevExpress.XtraEditors.TileItem item in this.tileGroupPorts.Items)
                    item.Checked = String.Equals(item.Tag as string, portName, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerPortCheck_Tick(object sender, EventArgs e)
        {
            try
            {
                CheckLostPorts();
                RefreshTiles();
                UpdateButtonState();
                // Scans queued while a modal window was open run once it is closed
                if (!this.isProcessing && HasPendingScans() && !IsModalWindowOpen())
                    ProcessPendingScans();
                UpdatePendingLabel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OpenDeviceList()
        {
            try
            {
                if (this.deviceListForm != null && !this.deviceListForm.IsDisposed)
                {
                    if (this.deviceListForm.WindowState == FormWindowState.Minimized)
                        this.deviceListForm.WindowState = FormWindowState.Normal;
                    this.deviceListForm.Activate();
                    return;
                }

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                    .FirstOrDefault(o => o.ModuleLink == ModuleLinkString.TestDeviceSample);
                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null || this.currentModuleBase == null)
                {
                    Notify("", "", DateTime.Now, ResourceMessage.ChuaCoQuyenThietBiLayMau, false);
                    return;
                }

                object instance = PluginInstance.GetPluginInstance(
                    PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId),
                    new List<object>());
                Form form = instance as Form;
                if (form == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("TestDeviceSample did not return a form");
                    return;
                }
                this.deviceListForm = form;
                // Not modal: the list stays open beside the scanners and refreshes itself
                if (this.ParentForm != null)
                    form.Show(this.ParentForm);
                else
                    form.Show();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPortCom_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tileControlPorts_ItemClick(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {
            try
            {
                string portName = e.Item != null ? e.Item.Tag as string : null;
                if (!String.IsNullOrEmpty(portName))
                    SelectPort(portName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnConnect.Enabled || !dxValidationProvider1.Validate())
                    return;
                ConnectPort(SelectedPortName());
                UpdateButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnDisconnect.Enabled)
                    return;
                DisconnectPort(SelectedPortName());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshPortList();
                CheckLostPorts();
                RefreshTiles();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDeviceList_Click(object sender, EventArgs e)
        {
            try
            {
                OpenDeviceList();
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
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1 || positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Shortcut (KeyboardWorker: action ID = method name)
        public void BtnConnect()
        {
            try
            {
                btnConnect_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnDisconnect()
        {
            try
            {
                btnDisconnect_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnRefresh()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        /// <summary>
        /// Closing the tab closes every port and signs every account out (sessions live only while the port is open).
        /// Called from UserControlBase.Dispose, possibly more than once.
        /// </summary>
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                this.timerPortCheck.Stop();
                foreach (KeyValuePair<string, ComPortReader> pair in this.readers.ToList())
                {
                    if (pair.Value != null)
                        pair.Value.Close();
                    SignOutPort(pair.Key, ResourceMessage.LyDoDongMan);
                }
                this.readers.Clear();
                this.lostPorts.Clear();
                lock (this.pendingLock)
                {
                    this.pendingScans.Clear();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
