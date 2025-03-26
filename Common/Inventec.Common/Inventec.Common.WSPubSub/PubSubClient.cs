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
using log4net;
using Microsoft.AspNet.SignalR.Client;
using PSS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventec.Common.WSPubSub
{
    public delegate void ReceivedMessage(PssPubSubSDO data);

    public class PubSubClient
    {
        private static ILog logFile
        {
            get
            {
                ILog[] logger = LogManager.GetCurrentLoggers();
                if (logger == null || logger.Length <= 0)
                {
                    log4net.Config.XmlConfigurator.Configure();
                    logger = LogManager.GetCurrentLoggers();
                }

                if (LogManager.Exists("Inventec.Common.Logging.LogSystem") != null)
                {
                    return LogManager.GetLogger("Inventec.Common.Logging.LogSystem");
                }
                else if (LogManager.Exists("LogSystem") != null)
                {
                    return LogManager.GetLogger("LogSystem");
                }
                else if (logger != null && logger.Length > 0)
                {
                    return logger.First();
                }

                return null;
            }
        }

        private string _DefaultMethodName = "Publish";
        private string _DefaultSendMessage = "SendMessage";

        private string _Address = "";
        private string _HubName = "";
        private string _TokenCode = "";
        private string _SendMessage = "";

        private HubConnection _Conn = null;
        private IHubProxy _HubProxy = null;

        private ReceivedMessage _Received = null;

        private bool ConnectionSlow = false;
        private int timeOut = 100000;

        public PubSubClient(string address, string hubName, string tokenCode, ReceivedMessage receive, string serverSendMessageName = null)
        {
            this._Address = address;
            this._HubName = hubName;
            this._TokenCode = tokenCode;
            this._Received = receive;
            this._SendMessage = serverSendMessageName;
        }

        public PubSubClient(string address, string hubName, string tokenCode, ReceivedMessage receive, int millisecondsTimeOut, string serverSendMessageName = null)
        {
            this._Address = address;
            this._HubName = hubName;
            this._TokenCode = tokenCode;
            this._Received = receive;
            this._SendMessage = serverSendMessageName;
            this.timeOut = millisecondsTimeOut;
        }

        public async Task<bool> Start()
        {
            bool result = false;
            if (String.IsNullOrWhiteSpace(_Address))
            {
                if (logFile != null) logFile.Warn("Address is empty");
                throw new Exception("Address is null");
            }

            if (String.IsNullOrWhiteSpace(_HubName))
            {
                if (logFile != null) logFile.Warn("HubName is empty");
                throw new Exception("HubName is null");
            }

            if (this._Conn != null && this._Conn.State != ConnectionState.Disconnected)
            {
                return true;
            }
            else if (this._Conn != null)
            {
                this.Stop();
            }

            this._Conn = new HubConnection(this._Address);
            this._Conn.Headers["TokenCode"] = this._TokenCode;
            this._Conn.Closed += _Conn_Closed;
            this._Conn.Reconnected += Reconnected;
            this._Conn.ConnectionSlow += _Conn_ConnectionSlow;
            this._HubProxy = this._Conn.CreateHubProxy(this._HubName);
            string method = String.IsNullOrWhiteSpace(this._SendMessage) ? this._DefaultSendMessage : this._SendMessage;
            this._HubProxy.On<PssPubSubSDO>(method, ReceivedMessage);

            Task task = this._Conn.Start();
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeOut, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    await task;
                    result = this._Conn.State == ConnectionState.Connected;
                    if (logFile != null) logFile.Warn("PubSubClient Start " + result);
                }
                else
                {
                    if (logFile != null) logFile.Error("Timed out");
                    throw new TimeoutException("The operation has timed out.");
                }
            }

            return result;
        }

        private void _Conn_ConnectionSlow()
        {
            try
            {
                this.ConnectionSlow = true;
                if (logFile != null)
                {
                    logFile.Warn("___________________________ConnectionSlow_________ " + _Conn.ConnectionId);
                    if (this._Conn.LastError != null)
                    {
                        logFile.Error("________________LastError: " + this._Conn.LastError.Message);
                    }
                }

                try
                {
                    this._HubProxy.Invoke("ConnectionSlow");
                    if (logFile != null) logFile.Warn("Invoke ConnectionSlow");
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                if (logFile != null)
                {
                    logFile.Error("Exception ConnectionSlow", ex);
                    if (this._Conn.LastError != null)
                    {
                        logFile.Error("________________LastError: ", this._Conn.LastError);
                    }
                }
            }
        }

        private void _Conn_Closed()
        {
            try
            {
                if (this._Conn != null)
                {
                    if (this._Conn.LastError != null && logFile != null)
                    {
                        logFile.Error("________________LastError: " + this._Conn.LastError.Message);
                    }
                    this._Conn.Dispose();
                }

                this._Conn = null;
                this._HubProxy = null;
                this._Received = null;
                this.ConnectionSlow = false;
                if (logFile != null) logFile.Warn("Client disconnected");
            }
            catch (Exception ex)
            {
                if (logFile != null)
                {
                    logFile.Error("Exception Client Closed", ex);
                    if (this._Conn.LastError != null)
                    {
                        logFile.Error("________________LastError: ", this._Conn.LastError);
                    }
                }
            }
        }

        private void ReceivedMessage(PssPubSubSDO data)
        {
            try
            {
                if (logFile != null) logFile.Warn("PubSubClient ReceivedMessage");

                if (this._Received == null && logFile != null)
                    logFile.Error("Delegate ReceivedMessage is null");
                else
                    this._Received(data);
            }
            catch (Exception ex)
            {
                if (logFile != null)
                {
                    logFile.Error("Exception ReceivedMessage", ex);
                    if (this._Conn.LastError != null)
                    {
                        logFile.Error("________________LastError: ", this._Conn.LastError);
                    }
                }
            }
        }

        public async Task<bool> SubscribeChannel(string channelName)
        {
            if (String.IsNullOrWhiteSpace(channelName))
            {
                if (logFile != null) logFile.Warn("ChannelName is empty");
                throw new Exception("ChannelName is null");
            }

            if (this._Conn == null || this._Conn.State == ConnectionState.Disconnected)
            {
                if (logFile != null) logFile.Warn("Chua ket noi. Server is empty");
                throw new Exception("Not Connected");
            }

            bool result = false;
            Task<bool> invoke = this._HubProxy.Invoke<bool>("Subscribe", channelName);

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var task = await Task.WhenAny(invoke, Task.Delay(this.timeOut, timeoutCancellationTokenSource.Token));
                if (task == invoke)
                {
                    timeoutCancellationTokenSource.Cancel();
                    result = await invoke;
                    if (logFile != null) logFile.Warn("Subscribe " + channelName);
                }
                else
                {
                    if (logFile != null) logFile.Error("Timed out");
                    throw new TimeoutException("The operation has timed out.");
                }
            }

            return result;
        }

        public async Task<bool> UnsubscribeChannel(string channelName)
        {
            if (String.IsNullOrWhiteSpace(channelName))
            {
                if (logFile != null) logFile.Warn("ChannelName is empty");
                throw new Exception("ChannelName is null");
            }

            if (this._Conn == null || this._Conn.State == ConnectionState.Disconnected)
            {
                if (logFile != null) logFile.Warn("Chua ket noi server is empty");
                throw new Exception("Not Connected");
            }

            bool result = false;
            Task<bool> invoke = this._HubProxy.Invoke<bool>("Unsubscribe", channelName);

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var task = await Task.WhenAny(invoke, Task.Delay(this.timeOut, timeoutCancellationTokenSource.Token));
                if (task == invoke)
                {
                    timeoutCancellationTokenSource.Cancel();
                    result = await invoke;
                    if (logFile != null) logFile.Warn("Unsubscribe " + channelName);
                }
                else
                {
                    if (logFile != null) logFile.Error("Timed out");
                    throw new TimeoutException("The operation has timed out.");
                }
            }

            return result;
        }

        public async Task<bool> PublishMessage(string channelName, PssPubSubSDO obj, string methodName = null)
        {
            string method = String.IsNullOrWhiteSpace(methodName) ? this._DefaultMethodName : methodName;
            if (String.IsNullOrWhiteSpace(channelName))
            {
                if (logFile != null) logFile.Warn("ChannelName is empty");
                throw new Exception("ChannelName is null");
            }

            if (this._Conn == null || this._Conn.State == ConnectionState.Disconnected)
            {
                if (logFile != null) logFile.Warn("Chua ket noi. server is empty");
                throw new Exception("Not Connected");
            }

            bool result = false;
            Task<bool> invoke = this._HubProxy.Invoke<bool>(method, channelName, obj);
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var task = await Task.WhenAny(invoke, Task.Delay(this.timeOut, timeoutCancellationTokenSource.Token));
                if (task == invoke)
                {
                    timeoutCancellationTokenSource.Cancel();
                    result = await invoke;
                    if (logFile != null) logFile.Warn("PubSubClient PublishMessage");
                }
                else
                {
                    if (logFile != null) logFile.Error("Timed out");
                    throw new TimeoutException("The operation has timed out.");
                }
            }

            if (!result && logFile != null)
            {
                logFile.Warn("Publish message to server failed");
            }
            return result;
        }

        public async Task<bool> PublishMessage(PssPubSubSDO obj, string methodName = null)
        {
            string method = String.IsNullOrWhiteSpace(methodName) ? this._DefaultMethodName : methodName;

            if (this._Conn == null || this._Conn.State == ConnectionState.Disconnected)
            {
                if (logFile != null) logFile.Warn("Chua ket noi. server is empty");
                throw new Exception("PublishMessageUnchannel Not Connected");
            }

            bool result = false;
            Task<bool> invoke = this._HubProxy.Invoke<bool>(method, obj);

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var task = await Task.WhenAny(invoke, Task.Delay(this.timeOut, timeoutCancellationTokenSource.Token));
                if (task == invoke)
                {
                    timeoutCancellationTokenSource.Cancel();
                    result = await invoke;
                    if (logFile != null) logFile.Warn("PubSubClient PublishMessage");
                }
                else
                {
                    if (logFile != null) logFile.Error("Timed out");
                    throw new TimeoutException("The operation has timed out.");
                }
            }

            if (!result)
            {
                if (logFile != null) logFile.Warn("PublishMessageUnchannel to server failed");
            }
            return result;
        }

        public void Stop()
        {
            try
            {
                if (logFile != null) logFile.Warn("Client Stop");

                if (this._Conn == null)
                {
                    if (logFile != null) logFile.Warn("Is disconnected. Return and not process continue");
                    return;
                }

                TimeSpan timeOut = TimeSpan.FromMilliseconds(this.timeOut);
                Exception error = null;
                this._Conn.Stop(error, timeOut);
                if (logFile != null && error != null)
                {
                    logFile.Error("_________________Conn.Stop: ", error);
                }
            }
            catch (Exception ex)
            {
                if (logFile != null )
                {
                    logFile.Error("Exception Stop", ex);
                    logFile.Error("_________________Conn.Stop: ", ex);
                }

                this._Conn = null;
                this._HubProxy = null;
                this._Received = null;
                this.ConnectionSlow = false;
            }
        }

        public bool IsConnected
        {
            get
            {
                return (this._Conn != null && this._Conn.State == ConnectionState.Connected && !ConnectionSlow);
            }
        }

        public bool IsConnectionSlow
        {
            get
            {
                return ConnectionSlow;
            }
        }

        public bool IsSubscribeChannel(string channelName)
        {
            return false;
        }

        public async Task<T> InvokeMethod<T>(string methodName, params object[] args)
        {
            if (String.IsNullOrWhiteSpace(methodName))
            {
                if (logFile != null) logFile.Warn("InvokeMethodUnchannel methodName is empty");
                throw new Exception("methodName is null");
            }

            if (this._Conn == null || this._Conn.State == ConnectionState.Disconnected)
            {
                if (logFile != null) logFile.Warn("InvokeMethodUnchannel Chua ket noi server is empty");
                throw new Exception("Not Connected");
            }

            T result = default(T);
            Task<T> invoke = this._HubProxy.Invoke<T>(methodName, args);

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var task = await Task.WhenAny(invoke, Task.Delay(this.timeOut, timeoutCancellationTokenSource.Token));
                if (task == invoke)
                {
                    timeoutCancellationTokenSource.Cancel();
                    result = await invoke;
                    if (logFile != null) logFile.Warn("InvokeMethod " + methodName);
                }
                else
                {
                    if (logFile != null) logFile.Error("Timed out");
                    throw new TimeoutException("The operation has timed out.");
                }
            }

            return result;
        }

        private void Reconnected()
        {
            try
            {
                this.ConnectionSlow = false;
                if (logFile != null)
                {
                    logFile.Warn("___________________________Reconnected___________________ " + _Conn.ConnectionId);
                    if (this._Conn.LastError != null)
                    {
                        logFile.Error("________________LastError: " + this._Conn.LastError.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                if (logFile != null)
                {
                    logFile.Error("Exception Reconnected", ex);
                    if (this._Conn.LastError != null)
                    {
                        logFile.Error("________________LastError: ", this._Conn.LastError);
                    }
                }
            }
        }
    }
}
