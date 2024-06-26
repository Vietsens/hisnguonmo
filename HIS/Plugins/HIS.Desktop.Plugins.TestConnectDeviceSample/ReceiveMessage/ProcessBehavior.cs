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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.BackendData.Core.TestDeviceSample;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using LIS.EFMODEL.DataModels;
using LIS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THE.Desktop.Plugins.TestConnectDeviceSample.Base;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.ReceiveMessage
{
    public class ProcessBehavior : ReceiveBase, IRun
    {
        string msgId;
        string teminalCode;
        public ProcessBehavior(CommonParam param, string message, Inventec.Common.Rs232.Connector connectCom, ConnectStore connectStore)
            : base(param, message, connectCom, connectStore)
        {

        }

        bool IRun.Run()
        {
            bool result = false;
            try
            {
                String[] element = null;
                bool check = this.Check(ref element);
                msgId = element[0];

                if (check)
                {
                    string barcode = element[2];
                    teminalCode = element[3];

                    if (barcode.Equals(MessageConstant.BARCODE__QUIT))
                    {
                        TestDeviceSampleADO testDeviceSampleADO = null;
                        if (!CheckQuit(ref testDeviceSampleADO))
                            throw new Exception();

                        TestDeviceSampleDataWorker.TestDeviceSamples.Remove(testDeviceSampleADO);
                        param.Messages.Add(String.Format("Kết thúc phiên làm việc!\r\nMã thiết bị: {0}", teminalCode));
                    }
                    else
                    {
                        //kiem tra xem barcode day co phai la taikhoan dang nhap hay khong
                        List<ACS_USER> users = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>();
                        if (users == null)
                        {
                            param.Messages.Add(String.Format("Không lấy được danh sách tài khoản của hệ thống"));
                            throw new Exception("Khong lay duoc danh sach tai khoan cua he thong");
                        }

                        ACS_USER user = users.FirstOrDefault(o => o.LOGINNAME.ToUpper() == barcode.ToUpper());
                        if (user != null)
                        {
                            TestDeviceSampleADO testDeviceSampleExist = TestDeviceSampleDataWorker.TestDeviceSamples.FirstOrDefault(o => o.Loginname == user.LOGINNAME && o.TeminalCode == teminalCode);
                            if (testDeviceSampleExist != null)
                            {
                                param.Messages.Add(String.Format("Phiên làm việc tài khoản {0} vẫn hoạt động", user.LOGINNAME));
                                throw new Exception("Phien lam viec tai khoan van hoat dong");
                            }

                            TestDeviceSampleADO testDeviceSampleADO = new TestDeviceSampleADO();
                            testDeviceSampleADO.Loginname = user.LOGINNAME;
                            testDeviceSampleADO.Username = user.USERNAME;
                            testDeviceSampleADO.SessionTimeStart = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                            testDeviceSampleADO.TeminalCode = teminalCode;
                            TestDeviceSampleDataWorker.TestDeviceSamples.Add(testDeviceSampleADO);
                            param.Messages.Add(String.Format("Bắt đầu phiên làm việc . Tài khoản {0}, mã thiết bị {1}", user.LOGINNAME, teminalCode));
                        }
                        else
                        {
                            TestDeviceSampleADO testDeviceSampleADO = null;
                            if (!CheckUpdateSttSample(ref testDeviceSampleADO))
                                throw new Exception();

                            UpdateSampleSttByBarcodeSDO sdo = new UpdateSampleSttByBarcodeSDO();
                            sdo.Barcode = barcode;
                            sdo.Loginname = testDeviceSampleADO.Loginname;
                            sdo.Username = testDeviceSampleADO.Username;
                            LIS_SAMPLE updateSttSample = new BackendAdapter(param)
                                .Post<LIS_SAMPLE>("api/LisSample/UpdateSttByBarCode", ApiConsumers.LisConsumer, sdo, param);

                            if (updateSttSample == null)
                            {
                                param.Messages.Add(String.Format("Cập nhập trạng thái lấy mẫu thất bại. Barcode: {0}", barcode));
                                throw new Exception("Cap nhat trang thai lay mau that bai");
                            }
                            if (updateSttSample != null)
                            {
                                param.Messages.Add(String.Format("Barcode: {0} . Người lấy mẫu: {1}", 
                                    barcode, testDeviceSampleADO.Loginname));
                            }
                        }
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                param.HasException = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            try
            {
                if (!String.IsNullOrEmpty(teminalCode))
                {
                    string sendMessage = new StringBuilder()
                    .Append(msgId)
                    .Append(ConnectConstant.MESSAGE_SEPARATOR)
                    .Append(result ? ConnectConstant.RESPONSE_SUCCESS : ConnectConstant.RESPONSE_CORRECT)
                    .Append(ConnectConstant.MESSAGE_SEPARATOR)
                    .Append(teminalCode)
                    .ToString();
                    this.Send(sendMessage);
                }
            }
            catch (Exception)
            {

            }

            return result;
        }

        private bool CheckUpdateSttSample(ref TestDeviceSampleADO testDeviceSampleADO)
        {
            bool result = true;
            try
            {
                testDeviceSampleADO = TestDeviceSampleDataWorker.TestDeviceSamples
                                .FirstOrDefault(o => o.TeminalCode == teminalCode);
                if (testDeviceSampleADO == null)
                {
                    param.Messages.Add(String.Format("Không tìm thấy thiết bị {0} được kết nối", teminalCode));
                    throw new Exception("Khong tim thay thiet bi duoc ket noi");
                }

                if (String.IsNullOrEmpty(testDeviceSampleADO.Loginname))
                {
                    param.Messages.Add(String.Format("Thiết bị chưa được cấu hình với tài khoản. Thiết bị {0}", teminalCode));
                    throw new Exception("Khong tim thay thiet bi duoc ket noi");
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckQuit(ref TestDeviceSampleADO testDeviceSampleADO)
        {
            bool result = true;
            try
            {
                testDeviceSampleADO = TestDeviceSampleDataWorker.TestDeviceSamples.FirstOrDefault(o => o.TeminalCode == teminalCode);
                if (testDeviceSampleADO == null)
                {
                    param.Messages.Add(String.Format("Không tìm thấy mã thiết bị: {0} trên hệ thống", teminalCode));
                    throw new Exception("QUIT: Khong tim thay ma thiet bi: " + teminalCode);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public bool Check(ref String[] element)
        {
            bool result = true;
            try
            {
                element = message.Split(ConnectConstant.MESSAGE_SEPARATOR);
                if (element != null && element.Length != 4)
                {
                    param.Messages.Add("Thiết bị gửi sai định dạng thông tin");
                    throw new Exception("element.Length != 4 .");
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
