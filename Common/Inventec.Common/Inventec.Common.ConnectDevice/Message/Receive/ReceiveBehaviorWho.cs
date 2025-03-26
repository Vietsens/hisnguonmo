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
using Inventec.Common.ConnectDevice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Message.Receive
{
    class ReceiveBehaviorWho : IReceive
    {
        string[] element;

        internal ReceiveBehaviorWho(string[] data)
        {
            element = data;
        }

        async Task IReceive.Run(ConnectToDevice connectDevice)
        {
            try
            {
                if (element != null && element.Length > 0)
                {
                    if (!element[0].Equals(connectDevice.connectStore.messageIdWho))
                    {
                        if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Message id thiết bị gửi lên không trùng với phần mềm gửi xuống.", ConnectToDevice.EnumConnect.CONNECT);
                        return;
                    }
                    connectDevice.connectStore.messageIdWho = null;
                    if (string.IsNullOrEmpty(element[1]))
                    {
                        if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Không có mã thiết bị (TerminalCode) gửi lên.", ConnectToDevice.EnumConnect.CONNECT);
                        return;
                    }

                    string key3 = Inventec.Common.String.Generate.Random(24, String.Generate.CharacterMode.ONLY_LETTER_NUMERIC, String.Generate.UpperLowerMode.ANY);
                    GetConnectTerminalInfo sendInfo = new GetConnectTerminalInfo(Store.Config.TOS_CONNECTOR_CODE, element[1], key3);
                    GetConnectTerminalInfo rs = null;
                    if (connectDevice._ConnectTerminal != null)
                        rs = await connectDevice._ConnectTerminal(sendInfo).ConfigureAwait(false);
                    if (rs == null)
                    {
                        connectDevice.connectStore.SessionKey = null;
                        connectDevice.connectStore.connectTerminalInfo = null;
                        connectDevice.Close();
                        return;
                    }
                    connectDevice.connectStore.SessionKey = key3;
                    connectDevice.connectStore.connectTerminalInfo = rs;

                    Message.Send.SendBehaviorFactory.MakeISend(Send.SendBehaviorFactory.EnumSend.CONNECT).Run(connectDevice);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                if (connectDevice._ConnectSuccess != null) connectDevice._ConnectSuccess(false, "Có exception xẩy ra trong quá trình xử lý bản tin trả lời WHO.", ConnectToDevice.EnumConnect.CONNECT);
            }
        }
    }
}
