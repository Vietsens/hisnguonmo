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
using Inventec.Common.Adapter;
using System.Threading;
using System.Diagnostics;
using HIS.Desktop.LocalStorage.BackendData;
using LIS.EFMODEL.DataModels;
using LIS.SDO;
using HIS.Desktop.Plugins.TestConnectDeviceSample.ReceiveMessage;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample
{
    public partial class UCTestConnectDeviceSample : HIS.Desktop.Utility.UserControlBase
    {
        private async void ConnectComProcess()
        {
            try
            {
                string com = cboPortCom.Text;
                if (!String.IsNullOrEmpty(com))
                {
                    if (connectCom != null)
                    {
                        connectCom = null;
                    }
                    connectCom = new Inventec.Common.Rs232.Connector(ReceiveMessage, ConnectConstant.HEADER, ConnectConstant.FOOTER, com);
                    if (!connectCom.IsOpen)
                    {
                        try
                        {
                            connectCom.Open();
                            WaitingManager.Show();
                            //Connect
                            connectStore.messageIdConnect = GenerateMessageId.Generate("Connect");
                            string sendMessage = new StringBuilder().Append(connectStore.messageIdConnect).Append(ConnectConstant.MESSAGE_SEPARATOR).Append(ConnectConstant.MESSAGE_CONNECT).ToString();
                            this.Send(sendMessage);
                            //
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            if (connectCom.IsOpen) connectCom.Close();
                            connectCom = null;
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            MessageBox.Show("Cổng đã kết nối với thiết bị khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (System.IO.IOException ex)
                        {
                            if (connectCom.IsOpen) connectCom.Close();
                            connectCom = null;
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            MessageBox.Show("Thiết bị chưa kết nối với cổng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            if (connectCom.IsOpen) connectCom.Close();
                            connectCom = null;
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            MessageBox.Show("Có Exception xẩy ra.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void Send(string message)
        {
            try
            {
                if (connectCom != null && connectCom.IsOpen && !string.IsNullOrEmpty(message))
                {
                    string sendMessage = sendMessage = new StringBuilder().Append(ConnectConstant.HEADER).Append(message).Append(ConnectConstant.FOOTER).ToString();
                    if (!string.IsNullOrEmpty(sendMessage))
                    {
                        connectCom.Send(sendMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Thiet bi gui len: PROCESS, PING
        /// PC gui : CONNECT, DISCONECT
        /// </summary>
        /// <param name="message"></param>
        private void ReceiveMessage(string message)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("Message thiet bi gui len: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => message), message));

                Task.Run(() => ProcessMessage(message));
                //ProcessMessage(message);

                //Thread threadProcessMessage = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ProcessMessage));
                //threadProcessMessage.Priority = ThreadPriority.Highest;
                //try
                //{
                //    threadProcessMessage.Start(message);
                //}
                //catch (Exception ex)
                //{
                //    LogSystem.Error(ex);
                //    threadProcessMessage.Abort();
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ProcessMessage(string message)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Message thiet bi gui len: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => message), message));

                CommonParam param = new CommonParam();
                IRun receiveMessage = ReceiveMessageFactory.MakeReceiveMessage(param, message, connectCom, connectStore, callBackLoad);
                bool success = receiveMessage != null ? receiveMessage.Run() : false;
                if (!success)
                    MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void callBackLoad(string connectConstant)
        {
            try
            {
                switch (connectConstant)
                {
                    case ConnectConstant.MESSAGE_CONNECT:
                        cboPortCom.Invoke(new MethodInvoker(delegate() { cboPortCom.Enabled = false; }));
                        btnConnect.Invoke(new MethodInvoker(delegate() { btnConnect.Enabled = false; }));
                        btnDisconnect.Invoke(new MethodInvoker(delegate() { btnDisconnect.Enabled = true; }));
                        break;
                    case ConnectConstant.MESSAGE_DISCONNECT:
                        cboPortCom.Invoke(new MethodInvoker(delegate() { cboPortCom.Enabled = true; }));
                        btnConnect.Invoke(new MethodInvoker(delegate() { btnConnect.Enabled = true; }));
                        btnDisconnect.Invoke(new MethodInvoker(delegate() { btnDisconnect.Enabled = false; }));
                        if (connectCom != null)
                        {
                            connectCom.Close();
                        }
                        connectCom = null;
                        break;
                    default:
                        break;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckReceiveMessage(string message, ref  String[] element)
        {
            bool result = true;
            try
            {
                if (String.IsNullOrEmpty(message))
                {
                    MessageBox.Show("Không tìm thấy bản tin thiết bị gửi lên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                element = message.Split(ConnectConstant.MESSAGE_SEPARATOR);
                if (element == null || element.Length < 2)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong cat duoc ban tin: ");
                    return false;
                }

                string messageId = element[0];
                if (string.IsNullOrEmpty(messageId))
                {
                    Inventec.Common.Logging.LogSystem.Error("Thiet bi khong gui len messageid: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageId), messageId));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool UpdateStatusSample(string barcode, string loginname, string username)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                UpdateSampleSttByBarcodeSDO sdo = new UpdateSampleSttByBarcodeSDO();
                sdo.Barcode = barcode;
                sdo.Loginname = loginname;
                sdo.Username = username;
                LIS_SAMPLE updateSttSample = new BackendAdapter(param)
                    .Post<LIS_SAMPLE>("api/LisSample/UpdateSttByBarCode", ApiConsumers.LisConsumer, sdo, param);
                if (updateSttSample != null)
                {
                    success = true;
                    MessageBox.Show(String.Format("Barcode: {0} . Người lấy mẫu: {1}", barcode, loginname), "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                    MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private async void ConnectComDisconnect()
        {
            try
            {
                //Connect
                connectStore.messageIdDisconnect = GenerateMessageId.Generate("Disconnect");
                string sendMessage = new StringBuilder().Append(connectStore.messageIdDisconnect).Append(ConnectConstant.MESSAGE_SEPARATOR).Append(ConnectConstant.MESSAGE_DISCONNECT).ToString();
                this.Send(sendMessage);
                //
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
