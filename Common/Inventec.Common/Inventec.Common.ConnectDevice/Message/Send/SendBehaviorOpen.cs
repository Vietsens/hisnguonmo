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

namespace Inventec.Common.ConnectDevice.Message.Send
{
    class SendBehaviorOpen : ISend
    {
        void ISend.Run(ConnectToDevice connectDevice)
        {
            try
            {
                connectDevice.connectStore.isConnecting = false;
                connectDevice.connectStore.messageIdOpen = GenerateMessageId.Generate("Open");
                string sendMessage = new StringBuilder().Append(connectDevice.connectStore.messageIdOpen).Append(Store.ConnectConstant.MESSAGE_SEPARATOR).Append(Store.ConnectConstant.MESSAGE_OPEN).Append(Store.ConnectConstant.MESSAGE_SEPARATOR).Append(connectDevice.connectStore.APPCODE).ToString();
                int? checksum = Inventec.Common.Checksum.Rs232.Calc(sendMessage);
                if (checksum.HasValue)
                {
                    string message = new StringBuilder().Append(checksum.Value).Append(Store.ConnectConstant.MESSAGE_SEPARATOR).Append(sendMessage).ToString();
                    connectDevice.Send(message, true, true);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("Tinh check sum cho ban tin that bai: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checksum), checksum));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
