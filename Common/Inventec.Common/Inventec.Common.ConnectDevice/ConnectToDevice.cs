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
using Inventec.Common.ConnectDevice.Message;
using Inventec.Common.ConnectDevice.Store;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice
{
    public partial class ConnectToDevice
    {
        private Inventec.Common.Rs232.Connector connectCom;

        internal Store.ConnectStore connectStore;

        internal CallApiGetConnectTerminalInfo _ConnectTerminal;
        internal ConnectSuccessfull _ConnectSuccess;
        internal ReadCardInfo _ReadCard;
        internal ReceiveMessage _Receive;
        internal bool? IsSend;
        private bool _IsEncrypt = false;

        public enum EnumConnect
        {
            CONNECT,
            DISCONNECT,
            DEFAULT
        }

        public ConnectToDevice() { }

        public ConnectToDevice(CallApiGetConnectTerminalInfo connectTerminal, ConnectSuccessfull connectSuccess, ReadCardInfo readCard, string ApplicationCode)
        {
            try
            {
                if (connectStore == null) connectStore = new Store.ConnectStore();
                this._ConnectTerminal = connectTerminal;
                this._ConnectSuccess = connectSuccess;
                this._ReadCard = readCard;
                connectStore.APPCODE = ApplicationCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public ConnectToDevice(CallApiGetConnectTerminalInfo connectTerminal, ConnectSuccessfull connectSuccess, ReceiveMessage receive, string ApplicationCode)
        {
            try
            {
                if (connectStore == null) connectStore = new Store.ConnectStore();
                this._ConnectTerminal = connectTerminal;
                this._ConnectSuccess = connectSuccess;
                this._Receive = receive;
                connectStore.APPCODE = ApplicationCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Connect(string com)
        {
            if (!string.IsNullOrEmpty(com))
            {
                if (connectCom != null && connectCom.IsOpen)
                {
                    connectCom.Close();
                }
                connectCom = null;
                connectCom = new Rs232.Connector(ReceiveMessage, ConnectConstant.HEADER, ConnectConstant.FOOTER, com);
                if (!connectCom.IsOpen)
                {
                    try
                    {
                        connectCom.Open();
                        Message.Send.SendBehaviorFactory.MakeISend(Message.Send.SendBehaviorFactory.EnumSend.WHO).Run(this);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        if (connectCom.IsOpen) connectCom.Close();
                        connectCom = null;
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        if (_ConnectSuccess != null) _ConnectSuccess(false, "Cổng đã kết nối với thiết bị khác", EnumConnect.CONNECT);
                    }
                    catch (System.IO.IOException ex)
                    {
                        if (connectCom.IsOpen) connectCom.Close();
                        connectCom = null;
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        if (_ConnectSuccess != null) _ConnectSuccess(false, "Thiết bị chưa kết nối với cổng.", EnumConnect.CONNECT);
                    }
                    catch (Exception ex)
                    {
                        if (connectCom.IsOpen) connectCom.Close();
                        connectCom = null;
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        if (_ConnectSuccess != null) _ConnectSuccess(false, "Có Exception xẩy ra.", EnumConnect.CONNECT);
                    }
                }
            }
            else
            {
                throw new Exception("khong co du lieu cong com (null).");
            }
        }

        public void Disconnect()
        {
            Message.Send.SendBehaviorFactory.MakeISend(Message.Send.SendBehaviorFactory.EnumSend.DISCONNECT).Run(this);
        }

        public void Send(string message, bool IsEncrypt)
        {
            string sendMessage = null;
            if (!string.IsNullOrEmpty(message) && connectCom.IsOpen)
            {
                connectStore.messageIdSend = Message.GenerateMessageId.Generate("Send");
                message = connectStore.messageIdSend + ConnectConstant.MESSAGE_SEPARATOR + message;
                _IsEncrypt = IsEncrypt;
                if (IsEncrypt)
                {
                    if (!string.IsNullOrEmpty(connectStore.SessionKey))
                    {
                        int? checksum = Inventec.Common.Checksum.Rs232.Calc(message);
                        if (checksum.HasValue)
                        {
                            message = new StringBuilder().Append(checksum.Value).Append(Store.ConnectConstant.MESSAGE_SEPARATOR).Append(message).ToString();
                            var data = Inventec.Common.TripleDes.Cryptor.Encrypt(message, connectStore.SessionKey, false);
                            sendMessage = new StringBuilder().Append(ConnectConstant.HEADER).Append(data).Append(ConnectConstant.FOOTER).ToString();
                        }
                    }
                }
                else
                {
                    int? checksum = Inventec.Common.Checksum.Rs232.Calc(message);
                    if (checksum.HasValue)
                    {
                        sendMessage = new StringBuilder().Append(ConnectConstant.HEADER).Append(checksum.Value).Append(Store.ConnectConstant.MESSAGE_SEPARATOR).Append(message).Append(ConnectConstant.FOOTER).ToString();
                    }
                }
            }
            if (!string.IsNullOrEmpty(sendMessage))
            {
                LogSystem.Info("SendMessage to device: " + sendMessage);
                connectCom.Send(sendMessage);
                IsSend = true;
            }
            else
            {
                throw new Exception("Khong xu ly duoc goi tin gui xuong thiet bi");
            }
        }

        internal void Send(string message, bool IsEncrypt, bool initTimeout)
        {
            try
            {
                if (!string.IsNullOrEmpty(message) && connectCom.IsOpen)
                {
                    _IsEncrypt = IsEncrypt;
                    string sendMessage = null;
                    if (IsEncrypt)
                    {
                        if (!string.IsNullOrEmpty(connectStore.SessionKey))
                        {
                            var data = Inventec.Common.TripleDes.Cryptor.Encrypt(message, connectStore.SessionKey, false);
                            sendMessage = new StringBuilder().Append(ConnectConstant.HEADER).Append(data).Append(ConnectConstant.FOOTER).ToString();
                        }
                    }
                    else
                    {
                        sendMessage = new StringBuilder().Append(ConnectConstant.HEADER).Append(message).Append(ConnectConstant.FOOTER).ToString();
                    }
                    if (!string.IsNullOrEmpty(sendMessage))
                    {
                        LogSystem.Info("SendMessage to device: " + sendMessage);
                        connectCom.Send(sendMessage);
                        if (initTimeout)
                        {
                            InitTimeout();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool OpenApp(string ApplicationCode, bool isTimeout = true)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(ApplicationCode))
                {
                    connectStore.isConnecting = false;
                    connectStore.messageIdOpen = GenerateMessageId.Generate("Open");
                    string sendMessage = new StringBuilder().Append(connectStore.messageIdOpen).Append(Store.ConnectConstant.MESSAGE_SEPARATOR).Append(Store.ConnectConstant.MESSAGE_OPEN).Append(Store.ConnectConstant.MESSAGE_SEPARATOR).Append(ApplicationCode).ToString();
                    int? checksum = Inventec.Common.Checksum.Rs232.Calc(sendMessage);
                    if (checksum.HasValue)
                    {
                        string message = new StringBuilder().Append(checksum.Value).Append(Store.ConnectConstant.MESSAGE_SEPARATOR).Append(sendMessage).ToString();
                        Send(message, true, isTimeout);
                        result = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info("Tinh check sum cho ban tin that bai: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checksum), checksum));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateReceiveMessage(ReceiveMessage receive)
        {
            bool result = false;
            try
            {
                this._Receive = receive;
                if (this._Receive != null)
                    result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private async void ReceiveMessage(string message)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Message thiet bi gui len: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => message), message));
                connectStore.timeout.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                if (connectStore.isTimeout)
                {
                    connectStore.isTimeout = false;
                    return;
                }
                string plainMessage = null;
                if (connectStore.isConnecting)
                {
                    object data = Inventec.Common.Checksum.Rs232.Check(message);
                    if (data == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Check sum message that bai: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => message), message));
                        return;
                    }
                    plainMessage = (string)data;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("SessionKey: " + connectStore.SessionKey);
                    if (string.IsNullOrEmpty(connectStore.SessionKey))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Ban tin ma hoa ma khong co sessionKey: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => connectStore.SessionKey), connectStore.SessionKey));
                        return;
                    }
                    string messageData = "";
                    if (this._IsEncrypt)
                    {
                        messageData = Inventec.Common.TripleDes.Cryptor.Decrypt(message, connectStore.SessionKey, false);
                    }
                    else
                    {
                        messageData = message;
                    }
                    if (string.IsNullOrEmpty(messageData))
                    {
                        Inventec.Common.Logging.LogSystem.Error("giai ma ban tin that bai: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => connectStore), connectStore));
                        return;
                    }
                    object data = Inventec.Common.Checksum.Rs232.Check(messageData);
                    if (data == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Check sum message that bai: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageData), messageData));
                        return;
                    }
                    plainMessage = (string)data;
                }

                if (string.IsNullOrEmpty(plainMessage))
                {
                    Inventec.Common.Logging.LogSystem.Error("ban tin khong ton tai: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => plainMessage), plainMessage));
                    return;
                }
                var element = plainMessage.Split(Store.ConnectConstant.MESSAGE_SEPARATOR);
                if (element == null || element.Length == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong cat duoc ban tin: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => element), element));
                    return;
                }
                string code = element[1];
                if (!System.String.IsNullOrEmpty(code) && code == ConnectConstant.MESSAGE_READCARD)
                {
                    if (this._ReadCard != null)
                        this._ReadCard(element);
                    else if (this._Receive != null)
                        this._Receive(plainMessage);
                    return;
                }
                string messageId = element[0];
                if (string.IsNullOrEmpty(messageId))
                {
                    Inventec.Common.Logging.LogSystem.Error("Thiet bi khong gui len messageid: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageId), messageId));
                    return;
                }
                if (messageId.Equals(connectStore.messageIdWho) && element.Length == 3)
                {
                    await Message.Receive.ReceiveBehaviorFactory.MakeIReceive(Message.Receive.ReceiveBehaviorFactory.EnumReceive.WHO, element).Run(this).ConfigureAwait(false); ;
                }
                else if (messageId.Equals(connectStore.messageIdConnect) && element.Length == 2)
                {
                    await Message.Receive.ReceiveBehaviorFactory.MakeIReceive(Message.Receive.ReceiveBehaviorFactory.EnumReceive.CONNECT, element).Run(this);
                }
                else if (messageId.Equals(connectStore.messageIdOpen) && element.Length == 2)
                {
                    await Message.Receive.ReceiveBehaviorFactory.MakeIReceive(Message.Receive.ReceiveBehaviorFactory.EnumReceive.OPEN, element).Run(this);
                }
                else if (messageId.Equals(connectStore.messageIdDisconnect) && element.Length == 2)
                {
                    await Message.Receive.ReceiveBehaviorFactory.MakeIReceive(Message.Receive.ReceiveBehaviorFactory.EnumReceive.DISCONNECT, element).Run(this);
                }
                else if (IsSend.HasValue && IsSend.Value && messageId.Equals(connectStore.messageIdSend))
                {
                    IsSend = null;
                    if (this._Receive != null) this._Receive(plainMessage);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong xac dinh duoc ban tin ma thiet bi gui len: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => element), element));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Close()
        {
            if (connectCom != null && connectCom.IsOpen)
            {
                LogSystem.Info("Close port com: " + connectCom.PortName);
                connectCom.Close();
                connectCom.Dispose();
            }
            else if (connectCom != null)
            {
                LogSystem.Info("Dispose port com: " + connectCom.PortName);
                connectCom.Dispose();
            }
            connectCom = null;
        }

        public bool IsOpen()
        {
            return (connectCom != null && connectCom.IsOpen);
        }

        private void InitTimeout()
        {
            try
            {
                connectStore.isTimeout = false;
                connectStore.timeout = new System.Threading.Timer(new System.Threading.TimerCallback(CallBackTimer), null, Store.Config.Time, System.Threading.Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallBackTimer(object state)
        {
            try
            {
                LogSystem.Info("CallBackTimer");
                connectStore.timeout.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                connectStore.isTimeout = true;
                if (_ConnectSuccess != null) _ConnectSuccess(false, "Thiết bị không trả lời.", EnumConnect.DEFAULT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
