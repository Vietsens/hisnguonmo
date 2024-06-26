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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Validation;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Base;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Reflection;
using DevExpress.XtraSplashScreen;
using System.Net;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;
using System.IO.Ports;
using Inventec.Common.Adapter;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample
{
    public partial class UCTestConnectDeviceSample : HIS.Desktop.Utility.UserControlBase
    {

        Inventec.Desktop.Common.Modules.Module currentModule = null;
        Inventec.Common.Rs232.Connector connectCom;
        bool isClose = false;
        int positionHandleControl = -1;
        string terminalCode = "";
        ConnectStore connectStore;

        public UCTestConnectDeviceSample()
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception)
            {

            }
        }

        public UCTestConnectDeviceSample(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = moduleData;
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCTestConnectDeviceSample_Load(object sender, EventArgs e)
        {
            try
            {
                if (connectCom == null) connectStore = new ConnectStore();
                LoadKeyUcLanguage();
                ValidControl();
                RefreshData();
                cboPortCom.Focus();
                cboPortCom.ShowPopup();

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
                if (connectCom == null || !connectCom.IsOpen)
                {
                    string[] listPort = SerialPort.GetPortNames();
                    List<string> ports = new List<string>();
                    foreach (string com in listPort)
                    {
                        try
                        {
                            SerialPort myCom = new SerialPort();
                            myCom.PortName = com;
                            myCom.Open();
                            if (myCom.IsOpen)
                            {
                                myCom.Close();
                                myCom = null;
                                ports.Add(com);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    cboPortCom.Properties.DataSource = ports;
                    btnConnect.Enabled = true;
                    btnDisconnect.Enabled = false;
                    cboPortCom.Enabled = true;
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
                ValidCboPort();
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
                portRule.cboPortCom = cboPortCom;
                dxValidationProvider1.SetValidationRule(cboPortCom, portRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPortCom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
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
                ConnectComProcess();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnDisconnect.Enabled)
                    return;
                WaitingManager.Show();
                ConnectComDisconnect();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshData();
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

        private void LoadKeyUcLanguage()
        {
            try
            {
                this.btnConnect.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SALE_TRANSACTION__BTN_CONNECT", Base.ResourceLangManager.LanguageUCTestConnectDeviceSample, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnDisconnect.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SALE_TRANSACTION__BTN_DISCONNECT", Base.ResourceLangManager.LanguageUCTestConnectDeviceSample, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SALE_TRANSACTION__BTN_REFRESH", Base.ResourceLangManager.LanguageUCTestConnectDeviceSample, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutPortCom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SALE_TRANSACTION__LAYOUT_PORT_COM", Base.ResourceLangManager.LanguageUCTestConnectDeviceSample, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public void BtnConnect()
        {
            try
            {
                btnConnect_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CloseSerialPort()
        {
            try
            {
                if (connectCom != null && connectCom.IsOpen)
                    connectCom.Close();
                connectCom = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
