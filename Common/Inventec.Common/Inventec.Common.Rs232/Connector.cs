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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Rs232
{
    public delegate void ReceiveMessageHanler(string message);

    public class Connector : SerialPort
    {
        private ReceiveMessageHanler _receiveMessageHanler;
        private string _header;
        private string _footer;

        public Connector(ReceiveMessageHanler receiveMessageHanler, string header, string footer, string port)
        {
            try
            {
                DeviceConfig.SetPortNameToConfig(port);
                this.Init(header, footer, receiveMessageHanler);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Lỗi khởi tạo kết nối cổng: " + ex);
            }
        }

        /// <summary>
        /// Khoi tao lop xu ly ket noi cong COM voi cong COM duoc lay tu file cau hinh
        /// </summary>
        /// <param name="receiveMessageHanler">Ham xu ly khi nhan duoc goi tin</param>
        /// <param name="header">Header cua goi tin</param>
        /// <param name="footer">Footer cua goi tin</param>
        //public Connector(ReceiveMessageHanler receiveMessageHanler, string header, string footer)
        //{
        //    this.Init(header, footer, receiveMessageHanler);
        //}

        /// <summary>
        /// Khoi tao lop xu ly ket noi cong COM voi cong COM chi dinh
        /// </summary>
        /// <param name="receiveMessageHanler">Ham xu ly khi nhan duoc goi tin</param>
        /// <param name="header">Header cua goi tin</param>
        /// <param name="footer">Footer cua goi tin</param>
        /// <param name="port">Cong COM chon ket noi</param>
        //public Connector(ReceiveMessageHanler receiveMessageHanler, string header, string footer, string port)
        //{
        //    DeviceConfig.SetPortNameToConfig(port);
        //    this.Init(header, footer, receiveMessageHanler);
        //}

        /// <summary>
        /// Gui goi tin xuong thiet bi
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            try
            {
                if (this.IsOpen)
                {
                    LogSystem.Info("BeginSendToDevice: " + message);
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                    this.Write(data, 0, data.Length);
                    LogSystem.Info("EndSendToDevice: " + message);
                    //this.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetDelegate(ReceiveMessageHanler receive)
        {
            this._receiveMessageHanler = receive;
        }

        private void Init(string header, string footer, ReceiveMessageHanler receiveMessageHanler)
        {
            try
            {
                this._receiveMessageHanler = receiveMessageHanler;
                this._header = header;
                this._footer = footer;

                this.BaudRate = DeviceConfig.BaudRate;
                this.Parity = DeviceConfig.Parity;
                this.StopBits = DeviceConfig.StopBits;
                this.Handshake = DeviceConfig.Handshake;
                this.DataBits = DeviceConfig.DataBits;
                this.PortName = DeviceConfig.PortName;
                //this.DtrEnable = true;

                this.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceivedHandler);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string dataReceive = "";

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                LogSystem.Info("Begin DataReceivedHandler");
                //string message = this.ReadTo(this._footer);
                dataReceive += this.ReadExisting();
                LogSystem.Info("DataReceivedHandler. dataReceive: " + dataReceive);
                if (!dataReceive.Contains(this._footer))
                {
                    return;
                }
                string message = dataReceive;
                dataReceive = "";
                LogSystem.Info("DataReceivedHandler. Message: " + message);
                LogSystem.Info("Log 1: receive message from devide: " + message);
                int indexFooter = message.IndexOf(this._footer);
                if (indexFooter >= 0)
                {
                    message = message.Substring(0, message.Length - this._footer.Length);
                }
                int index = message.IndexOf(this._header);
                if (index >= 0)
                {
                    message = message.Substring(index + this._header.Length);
                }
                this._receiveMessageHanler.Invoke(message);

                LogSystem.Info("End DataReceivedHandler: " + message);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
