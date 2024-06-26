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
using THE.Desktop.Plugins.TestConnectDeviceSample.Base;
using HIS.Desktop.LocalStorage.BackendData.Core.TestDeviceSample;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using ACS.EFMODEL.DataModels;
using LIS.Filter;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample
{
    public partial class UCTestConnectDeviceSample : UserControl
    {
        private void ReceiveMessageConnect(String[] element)
        {
            try
            {

                if (element[1].Equals(ConnectConstant.RESPONSE_SUCCESS))
                {
                    cboPortCom.Invoke(new MethodInvoker(delegate() { cboPortCom.Enabled = false; }));
                    btnConnect.Invoke(new MethodInvoker(delegate() { btnConnect.Enabled = false; }));
                    btnDisconnect.Invoke(new MethodInvoker(delegate() { btnDisconnect.Enabled = true; }));
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReceiveMessageDiconnect(String[] element)
        {
            try
            {
                if (element[1].Equals(ConnectConstant.RESPONSE_SUCCESS))
                {
                    cboPortCom.Invoke(new MethodInvoker(delegate() { cboPortCom.Enabled = true; }));
                    btnConnect.Invoke(new MethodInvoker(delegate() { btnConnect.Enabled = true; }));
                    btnDisconnect.Invoke(new MethodInvoker(delegate() { btnDisconnect.Enabled = false; }));
                    if (connectCom != null)
                    {
                        connectCom.Close();
                    }
                    connectCom = null;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReceiveMessagePing(String[] element)
        {
            try
            {
                string sendMessage = new StringBuilder().Append(element[0]).Append(ConnectConstant.MESSAGE_SEPARATOR).Append(ConnectConstant.RESPONSE_SUCCESS).ToString();
                this.Send(sendMessage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReceiveMessageProcess(String[] element)
        {
            try
            {
                bool success = true;
                if (element.Length != 4)
                {
                    Inventec.Common.Logging.LogSystem.Error("Sai dang goi tin PROCESS " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => element), element));
                    success = false;
                    return;
                }

                string barcode = element[2];
                string teminalCode = element[3];
                if (barcode.Equals(MessageConstant.BARCODE__QUIT))
                {
                    TestDeviceSampleADO testDeviceSampleADO = TestDeviceSampleDataWorker.TestDeviceSamples.FirstOrDefault(o => o.TeminalCode == teminalCode);
                    if (testDeviceSampleADO == null)
                    {
                        MessageBox.Show(String.Format("Không tìm thấy mã thiết bị: {0} trên hệ thống", teminalCode), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Inventec.Common.Logging.LogSystem.Error("QUIT: Khong tim thay ma thiet bi: " + teminalCode);
                        return;
                    }
                    TestDeviceSampleDataWorker.TestDeviceSamples.Remove(testDeviceSampleADO);
                    MessageBox.Show(String.Format("Kết thúc phiên làm việc!\r\nMã thiết bị: {0}", teminalCode), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    List<ACS_USER> users = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>();
                    if (users == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong lay duoc danh sach nguoi dung");
                        return;
                    }

                    ACS_USER user = users.FirstOrDefault(o => o.LOGINNAME.ToUpper() == barcode.ToUpper());
                    if (user != null)
                    {
                        //Kiem tra xem ton tai du lieu chua
                        TestDeviceSampleADO testDeviceSampleExist = TestDeviceSampleDataWorker.TestDeviceSamples.FirstOrDefault(o => o.Loginname == user.LOGINNAME && o.TeminalCode == teminalCode);
                        if (testDeviceSampleExist != null)
                        {
                            MessageBox.Show(String.Format("Mã thiết bị: {0} Người lấy mẫu: {1} tồn tại trên hệ thống", teminalCode), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        TestDeviceSampleADO testDeviceSampleADO = new TestDeviceSampleADO();
                        testDeviceSampleADO.Loginname = barcode;
                        testDeviceSampleADO.SessionTimeStart = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                        testDeviceSampleADO.TeminalCode = teminalCode;
                        TestDeviceSampleDataWorker.TestDeviceSamples.Add(testDeviceSampleADO);

                        MessageBox.Show(String.Format("Tạo phiên làm việc thành công!\r\nMã thiết bị: {0} Người lấy mẫu: {1}", teminalCode, barcode), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        user = null;
                        if (CheckBeforeUpdateSampleStt(teminalCode, ref user))
                            UpdateStatusSample(barcode, user.LOGINNAME, user.USERNAME);
                    }
                }

                string sendMessage = new StringBuilder()
                        .Append(element[0])
                        .Append(ConnectConstant.MESSAGE_SEPARATOR)
                        .Append(ConnectConstant.RESPONSE_SUCCESS)
                        .Append(ConnectConstant.MESSAGE_SEPARATOR)
                        .Append(teminalCode)
                        .ToString();
                this.Send(sendMessage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckBeforeUpdateSampleStt(string teminalCode, ref ACS_USER user)
        {
            bool result = true;
            try
            {
                TestDeviceSampleADO testDeviceSampleADO = TestDeviceSampleDataWorker.TestDeviceSamples.FirstOrDefault(o => o.TeminalCode == teminalCode);
                if (testDeviceSampleADO == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay ma thiet bi duoc ket noi. Ma thiet bi: " + teminalCode);
                    return false;
                }

                if (String.IsNullOrEmpty(testDeviceSampleADO.Loginname))
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay tai khoan voi thiet bi. Ma thiet bi: " + teminalCode);
                    return false;
                }

                user = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>()
                    .FirstOrDefault(o => o.LOGINNAME == testDeviceSampleADO.Loginname);
                if (user == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Thong tin tai khoan luu trong ram khong ton tai trong db. Loginname : " + testDeviceSampleADO.Loginname);
                    return false;
                }

            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
