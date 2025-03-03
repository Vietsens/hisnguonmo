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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Common.BankQrCode
{
    public delegate void DelegateSendMessage(bool IsSuccess, string Message);
    public enum SendType
    {
        QR,
        Text
    }
    public enum OptionPos
    {
        Port = 0,
        Library = 1
    }
    public class PosProcessor : SerialPort
    {
        #region IPOS Library
        [DllImport("mpos_sdk.dll", EntryPoint = "mf_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mf_init(int id, int mode);
        [DllImport("mpos_sdk.dll", EntryPoint = "mf_connect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mf_connect();
        [DllImport("mpos_sdk.dll", EntryPoint = "mf_showText", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mf_showText(byte timeOut, byte[] text, int len);

        [DllImport("mpos_sdk.dll", EntryPoint = "mf_genQrCode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mf_genQrCode(byte timeOut, byte[] qrCode, int len);

        [DllImport("mpos_sdk.dll", EntryPoint = "mf_resetPos", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mf_resetPos();

        [DllImport("mpos_sdk.dll", EntryPoint = "mf_disconnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mf_disconnect();

        #endregion

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
        public static bool IsConnectDevice = false;
        private bool? ReconnectService = null;
        IPOS.WCFService.Client.IPOSClientManager clienManager = null;
        public PosProcessor(string portName, DelegateSendMessage delegateSend)
        {
            try
            {
                this.delegateSend = delegateSend;
                this.PortName = portName;
                this.DtrEnable = true;
                this.RtsEnable = true;
                this.ReadTimeout = 7000;
                this.WriteTimeout = 5000;
                this.DataReceived += PosProcessor_DataReceived;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public PosProcessor(DelegateSendMessage delegateSend)
        {
            try
            {
                if (VerifyServiceIPOSProcessorIsRunning())
                {
                    clienManager = new IPOS.WCFService.Client.IPOSClientManager();
                    clienManager.SetupDevice();
                }
                this.delegateSend = delegateSend;
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
        private async void CheckDeviceActive()
        {
            try
            {
                await Task.Delay(5000);
                await Task.Factory.StartNew(() =>
                {
                    if (!CheckDataReceived && delegateSend != null && !IsDisposePort)
                    {
                        IsConnectDevice = false;
                        delegateSend(CheckDataReceived, MessageError = "Không nhận được phản hồi từ thiết bị IPOS. Vui lòng khởi động và kết nối lại tới thiết bị.");
                    }
                });
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
                        IsConnectDevice = true;
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
            IsConnectDevice = false;
            messageError = string.Format("Kết nối tới thiết bị IPOS thất bại. Vui lòng kiểm tra lại kết nối cổng {0}", this.PortName);
            IsFirstConnection = false;
            DisposePort();
            return false;
        }
        public bool ConnectDevice(bool showMessageFirstConnect, ref string messageError)
        {
            try
            {
                var dt = clienManager.ConnectDevice(showMessageFirstConnect);
                IsConnectDevice = dt.Success;
                messageError = dt.MessageError;
                if (IsConnectDevice)
                    return true;
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
            return false;
        }

        public void SendDevice(string dataSend)
        {
            try
            {
                MessageError = null;
                if (IsConnectDevice)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(dataSend))
                        {
                            try
                            {
                                clienManager.PlayAudio(dataSend == PosStatic.PAYMENT_SUCCESSS ? (byte)1 : (byte)0);
                                dataSend = dataSend == PosStatic.PAYMENT_SUCCESSS ? null : dataSend;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Phiên bản hiện tại không có môi trường phù hợp hoặc file mpos phiên bản thấp");
                            }
                        }
                        var dt = clienManager.GenQr(dataSend);
                        delegateSend(dt.Success, MessageError = dt.MessageError);
                    }
                    catch (CommunicationObjectFaultedException ex)
                    {
                        if(ReconnectService.HasValue && ReconnectService.Value)
                        {
                            ReconnectService = null;
                            IsConnectDevice = false;
                            delegateSend(false, MessageError = "Không nhận được phản hồi từ thiết bị IPOS. Vui lòng khởi động và kết nối lại tới thiết bị.");
                            return;
                        }    
                        clienManager = new IPOS.WCFService.Client.IPOSClientManager();
                        clienManager.SetupDevice();
                        SendDevice(dataSend);
                        ReconnectService = true;
                        Inventec.Common.Logging.LogSystem.Error("Khởi tạo lại service để kết nối");
                    }
                }
            }
            catch (Exception ex)
            {
                IsConnectDevice = false;
                delegateSend(false, MessageError = "Không nhận được phản hồi từ thiết bị IPOS. Vui lòng khởi động và kết nối lại tới thiết bị.");
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Send(string dataSend)
        {
            try
            {
                MessageError = null;
                if (this.IsOpen)
                {
                    dataSend = dataSend == PosStatic.PAYMENT_SUCCESSS ? "" : (dataSend ?? "");
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
        public void DisposeDevice()
        {

            try
            {
                IsConnectDevice = false;
                clienManager.ResetDevice();
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
                    IsConnectDevice = false;
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
        public string AppFilePathSignService()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "IPOS"), "IPOS.exe");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }
        internal bool VerifyServiceIPOSProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("IPOS.1");
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("IPOS"))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("IPOS.2");
                        valid = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("IPOS.3");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeSignPath), exeSignPath));
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {
                            Process.Start(startInfo);
                            Inventec.Common.Logging.LogSystem.Debug("IPOS.4");
                            Thread.Sleep(500);
                            valid = true;
                            Inventec.Common.Logging.LogSystem.Debug("IPOS.5");
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        internal bool GetStateConnectDevice()
        {
            return IsConnectDevice;
        }
    }

    public static class PosStatic
    {
        public const string PAYMENT_SUCCESSS = "SUCCESS";
        public static PosProcessor Pos { get; set; }
        public static OptionPos Option { get; set; }
        private static bool IsConnected { get; set; }
        public static bool OpenPos(OptionPos ops, bool isFirstConnect, string portName, DelegateSendMessage delegateSend, ref string MessError)
        {
            Pos = ops == OptionPos.Port ? new PosProcessor(portName, delegateSend) : new PosProcessor(delegateSend);
            Option = ops;
            return IsConnected = ops == OptionPos.Port ? Pos.ConnectPort(ref MessError) : Pos.ConnectDevice(isFirstConnect, ref MessError);
        }
        public static void SendData(string dataSend)
        {
            try
            {
                if (IsConnected)
                {
                    if (Option == OptionPos.Port)
                    {
                        Pos.Send(dataSend);
                    }
                    else
                    {
                        Pos.SendDevice(dataSend);
                    }
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
                if (IsConnected)
                {
                    if (Option == OptionPos.Port)
                    {
                        Pos.DisposePort();
                    }
                    else
                    {
                        Pos.DisposeDevice();
                    }
                    IsConnected = false;
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
        public static bool IsOpenPos() { return Pos != null ? Pos.GetStateConnectDevice() : false; }
    }
}
