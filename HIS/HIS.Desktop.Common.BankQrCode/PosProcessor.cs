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
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace HIS.Desktop.Common.BankQrCode
{
    public delegate void DelegateSendMessage(bool IsSuccess, string Message);
    public enum SendType
    {
        QR,
        Text
    }
    public class PosProcessor : SerialPort
    {
        private const int QR_LENGTH = 28;
        private const int A_LENGTH = 17;
        private const string QR_FORMAT = "\"C\":\"03\",\"A\":\"0.01\",\"D\":\"{0}\"";
        private const string A_FORMAT = "\"C\":\"04\",\"T\":\"{0}\"";
        private const string RESULT_FORMAT = "{\"C\":\"03\",\"R\":\"00\"}ED";
        public SendType typeSend = SendType.QR;
        private string MessageError = null;
        private DelegateSendMessage delegateSend;
        private bool CheckDataReceived = true;
        private bool IsDisposePort = false;
        private bool IsFirstConnection = false;
        public PosProcessor(string portName, DelegateSendMessage delegateSend)
        {
            this.delegateSend = delegateSend;
            this.PortName = portName;
            this.DtrEnable = true;
            this.RtsEnable = true;
            this.ReadTimeout = 3000;
            this.WriteTimeout = 3000;
            this.DataReceived += PosProcessor_DataReceived;
        }

        private async void CheckDeviceActive()
        {
            try
            {
                await Task.Delay(5000);
                await Task.Factory.StartNew(() =>
                {
                    if (!CheckDataReceived && delegateSend != null && !IsDisposePort)
                    {
                        delegateSend(CheckDataReceived, MessageError = "Không nhận được phản hồi từ thiết bị IPOS. Vui lòng khởi động và kết nối lại tới thiết bị.");
                    }
                });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void PosProcessor_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                CheckDataReceived = true;
                Inventec.Common.Logging.LogSystem.Info("Start DataReceived");
                SerialPort com = sender as SerialPort;
                int a = com.BytesToRead;
                byte[] buffer = new byte[a];
                com.Read(buffer, 0, a);
                MessageError = System.Text.Encoding.ASCII.GetString(buffer).EndsWith(RESULT_FORMAT) ? "Kết nối tới thiết bị IPOS thành công" : string.Format("Kết nối tới thiết bị IPOS thất bại. Vui lòng kiểm tra lại kết nối cổng {0}", this.PortName);
                Inventec.Common.Logging.LogSystem.Debug(MessageError + ": " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => System.Text.Encoding.ASCII.GetString(buffer)), System.Text.Encoding.ASCII.GetString(buffer)));
                if (delegateSend != null)
                    delegateSend(System.Text.Encoding.ASCII.GetString(buffer).EndsWith(RESULT_FORMAT), MessageError);
                Inventec.Common.Logging.LogSystem.Info("End DataReceived");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public bool ConnectPort(ref string messageError)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Start ConnectPort");
                CheckDataReceived = false;
                IsDisposePort = false;
                CheckDeviceActive();
                this.Open();
                CheckDataReceived = true;
                Inventec.Common.Logging.LogSystem.Info("Check ConnectPort");
                if (this.IsOpen)
                {
                    Inventec.Common.Logging.LogSystem.Info("Check SendData");
                    IsFirstConnection = true;
                    this.Send(null);
                    try
                    {
                        Inventec.Common.Logging.LogSystem.Info("Read Result");
                        var key = this.ReadChar();//Kết nối đúng thiết bị sẽ không nhảy vào Exception
                        Inventec.Common.Logging.LogSystem.Info("Result valid, ConnectPort Success");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (System.UnauthorizedAccessException ex)
            {
                Inventec.Common.Logging.LogSystem.Info("Thiết bị đã được kết nối lại");
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            Inventec.Common.Logging.LogSystem.Info("ConnectPort Fail");
            CheckDataReceived = true;
            messageError = string.Format("Kết nối tới thiết bị IPOS thất bại. Vui lòng kiểm tra lại kết nối cổng {0}", this.PortName);
            IsFirstConnection = false;
            DisposePort();
            return false;
        }

        public void Send(string dataSend)
        {
            try
            {
                MessageError = null;
                if (this.IsOpen)
                {
                    dataSend = dataSend ?? "";
                    switch (typeSend)
                    {
                        case SendType.QR:
                            dataSend = string.Format("MF{0}", string.Format("{0:0000}", QR_LENGTH + dataSend.Length)) + "{" + string.Format(QR_FORMAT, dataSend) + "}" + "ED";
                            break;
                        case SendType.Text:
                            dataSend = string.Format("MF{0}", string.Format("{0:0000}", A_LENGTH + dataSend.Length)) + "{" + string.Format(A_FORMAT, dataSend) + "}" + "ED";
                            break;
                        default:
                            break;
                    }
                    this.Write(dataSend);
                    if (!IsFirstConnection)
                    {
                        CheckDataReceived = false;
                        this.CheckDeviceActive();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void DisposePort()
        {

            try
            {
                if (this.IsOpen)
                {
                    this.IsDisposePort = true;
                    this.Send(null);
                    this.Close();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }

    public static class PosStatic
    {
        public static PosProcessor Pos { get; set; }
        public static bool OpenPos(string portName, DelegateSendMessage delegateSend, ref string MessError)
        {
            Pos = new PosProcessor(portName, delegateSend);
            return Pos.ConnectPort(ref MessError);
        }
        public static void SendData(string dataSend)
        {
            try
            {
                if (Pos != null && Pos.IsOpen)
                {
                    Pos.Send(dataSend);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("IPOS không được kết nối");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public static void DisposePort()
        {
            try
            {
                if (Pos != null && Pos.IsOpen)
                {
                    Pos.DisposePort();
                    Pos = null;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("IPOS không được kết nối");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public static bool IsOpenPos() { return Pos != null && Pos.IsOpen; }
    }
}
