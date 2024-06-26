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
using HIS.Desktop.Plugins.TestConnectDeviceSample.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.ReceiveMessage
{
    public class SendDevice
    {
        internal void Send(Inventec.Common.Rs232.Connector connectCom, string message)
        {
            try
            {
                if (connectCom != null && connectCom.IsOpen && !string.IsNullOrEmpty(message))
                {
                    string sendMessage = sendMessage = new StringBuilder().Append(ConnectConstant.HEADER).Append(message).Append(ConnectConstant.FOOTER).ToString();
                    if (!string.IsNullOrEmpty(sendMessage))
                    {
                        connectCom.Send(sendMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
