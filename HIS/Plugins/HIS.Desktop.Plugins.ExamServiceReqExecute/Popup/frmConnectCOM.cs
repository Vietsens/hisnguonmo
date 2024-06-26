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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Validate.ValidateRule;
using Inventec.Common.CardReader;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute.ConnectCOM
{
    public partial class frmConnectCOM : Form
    {
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        int positionHandleControl = -1;
        string connectedCom = "";

        public frmConnectCOM()
        {
            CheckToContinue();
            InitializeComponent();
        }

        void CheckToContinue()
        {
            try
            {
                if (string.IsNullOrEmpty(HisConfigCFG.terminalSystemAddress))
                {
                    XtraMessageBox.Show("Chưa cấu hình địa chỉ hệ thống quản lý thiết bị. Vui lòng kiểm tra lại cấu hình hệ thống MOS.EPAYMENT.TERMINAL_SYTEM.ADDRESS", "Thông báo");
                    this.Close();
                }
                if (string.IsNullOrEmpty(HisConfigCFG.terminalSystemSecureKey))
                {
                    XtraMessageBox.Show("Chưa cấu hình mã bí mật kết nối thiết bị. Vui lòng kiểm tra lại cấu hình hệ thống MOS.EPAYMENT.TERMINAL_SYTEM.SECURE_KEY", "Thông báo");
                    this.Close();
                }
                if (BranchDataWorker.Branch == null || string.IsNullOrEmpty(BranchDataWorker.Branch.THE_BRANCH_CODE))
                {
                    XtraMessageBox.Show("Chi nhánh/cơ sở đang làm việc chưa được khai báo mã tổ chức trên hệ thống thẻ. Vui lòng vào chức năng \"Cở sở/xã phường\" để thực hiện khai báo", "Thông báo");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmConnectCOM_Load(object sender, EventArgs e)
        {
            try
            {
                this.ValidCboPort();
                this.InitControlState();
                bool hasDeviceConnected = GlobalVariables.portComConnected != null && GlobalVariables.portComConnected.IsConnected;
                if (hasDeviceConnected)
                {
                    EnableControl(false);
                    connectedCom = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == "cboPortCom").VALUE;
                    if (!String.IsNullOrWhiteSpace(connectedCom)) this.cboPortCom.EditValue = connectedCom;
                }
                this.LoadPort();
                if (!hasDeviceConnected) this.ProcessAutoConnect();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData("HIS.Desktop.Plugins.ExamServiceReqExecute");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPort()
        {
            try
            {
                List<string> coms = Inventec.Common.CardReader.CardReaderManagement.GetComs();

                if (!String.IsNullOrWhiteSpace(connectedCom) && !coms.Contains(connectedCom))
                {
                    coms.Add(connectedCom);
                }
                cboPortCom.Properties.DataSource = coms;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidCboPort()
        {
            try
            {
                PortComValidationRule portRule = new PortComValidationRule();
                portRule.cboCom = cboPortCom;
                dxValidationProvider1.SetValidationRule(cboPortCom, portRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessAutoConnect()
        {
            try
            {
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0 && this.currentControlStateRDO.Any(a => a.KEY == "cboPortCom"))
                {
                    string port = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == "cboPortCom").VALUE;
                    List<string> availPorts = cboPortCom.Properties.DataSource as List<string>;
                    if (!String.IsNullOrWhiteSpace(port) && availPorts != null && availPorts.Contains(port))
                    {
                        cboPortCom.EditValue = port;
                        btnConnect_Click(null, null);
                    }
                    else
                    {
                        cboPortCom.Focus();
                        cboPortCom.ShowPopup();
                    }
                }
                else
                {
                    cboPortCom.Focus();
                    cboPortCom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnConnect.Enabled || !dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                GlobalVariables.portComConnected = new CardReaderManagement(HisConfigCFG.terminalSystemSecureKey, BranchDataWorker.Branch.THE_BRANCH_CODE, HisConfigCFG.terminalSystemAddress, ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"]);
                var rs = GlobalVariables.portComConnected.Connect(cboPortCom.Text.Trim());
                if (rs != null && rs.IsSuccess)
                {
                    EnableControl(false);
                    this.UpdateControlState();

                }
                else EnableControl(true);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UpdateControlState()
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = this.currentControlStateRDO != null ? this.currentControlStateRDO.Where(o => o.KEY == "cboPortCom").FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = cboPortCom.Text.Trim();
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "cboPortCom";
                    csAddOrUpdate.VALUE = cboPortCom.Text.Trim();
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.ExamServiceReqExecute";
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVariables.portComConnected == null) return;
                GlobalVariables.portComConnected.Disconnect();
                GlobalVariables.portComConnected = null;
                EnableControl(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableControl(bool enableSave)
        {
            try
            {
                btnConnect.Enabled = cboPortCom.Enabled = enableSave;
                btnDisconnect.Enabled = !enableSave;

                lblStatus.Text = !enableSave ? "Đã kết nối" : "Chưa kết nối";
                lblStatus.ForeColor = !enableSave ? Color.Green : Color.Black;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        private void frmConnectCOM_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                connectedCom = null;
                positionHandleControl = 0;
                currentControlStateRDO = null;
                controlStateWorker = null;
                this.btnDisconnect.Click -= new System.EventHandler(this.btnDisconnect_Click);
                this.btnConnect.Click -= new System.EventHandler(this.btnConnect_Click);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.frmConnectCOM_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                dxValidationProvider1 = null;
                emptySpaceItem1 = null;
                gridLookUpEdit1View = null;
                cboPortCom = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                lblStatus = null;
                btnConnect = null;
                btnDisconnect = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
