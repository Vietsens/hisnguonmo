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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Message.Receive
{
    class ReceiveBehaviorConnect : IReceive
    {
        string[] element;

        internal ReceiveBehaviorConnect(string[] data)
        {
            element = data;
        }

        async Task IReceive.Run(ConnectToDevice connectDevice)
        {
            try
            {
                if (element == null || element.Length == 0)
                    return;
                if (!element[0].Equals(connectDevice.connectStore.messageIdConnect))
                {
                    if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Message id thiết bị gửi lên không trùng với phần mềm gửi xuống.", ConnectToDevice.EnumConnect.CONNECT);
                    return;
                }
                connectDevice.connectStore.messageIdConnect = null;
                string responseCode = element[1];
                if (string.IsNullOrEmpty(responseCode))
                {
                    if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Không xác định được responseCode.", ConnectToDevice.EnumConnect.CONNECT);
                    return;
                }
                if (responseCode.Equals(Store.ConnectConstant.RESPONSE_SUCCESS))
                {
                    Message.Send.SendBehaviorFactory.MakeISend(Send.SendBehaviorFactory.EnumSend.OPEN).Run(connectDevice);
                }
                else
                {
                    string message = "";
                    if (responseCode.Equals("-1"))
                    {
                        message = "Không giải mã được license";
                    }
                    else if (responseCode.Equals("-2"))
                    {
                        message = "Không giải mã được Key";
                    }
                    else if (responseCode.Equals("-3"))
                    {
                        message = "License không hợp lệ";
                    }
                    else if (responseCode.Equals("-4"))
                    {
                        message = "Key không hợp lệ";
                    }
                    else
                    {
                        message = responseCode + ".";
                    }
                    if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, message, ConnectToDevice.EnumConnect.CONNECT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Có exception xẩy ra trong quá trình xử lý bản tin trả lời CONNECT.", ConnectToDevice.EnumConnect.CONNECT);
            }
        }
    }
}
