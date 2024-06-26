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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.ReceiveMessage
{
    public delegate void CallBackLoad(string connectConstant);
    public class ReceiveMessageFactory
    {
        public static IRun MakeReceiveMessage(CommonParam param, string message, Inventec.Common.Rs232.Connector connectCom, ConnectStore connectStore, CallBackLoad callBackLoad)
        {
            IRun result = null;
            try
            {

                if (CheckReceiveMessage(param, message))
                {
                    String[] element = message.Split(ConnectConstant.MESSAGE_SEPARATOR);
                    if (element[1] == ConnectConstant.MESSAGE_PING) //Thiet bi gui len
                    {
                        result = new PingBehavior(param, message, connectCom, connectStore);
                    }
                    else if (element[1] == ConnectConstant.MESSAGE_PROCESS) //Thiet bi gui len
                    {
                        result = new ProcessBehavior(param, message, connectCom, connectStore);
                    }
                    else
                    {
                        result = new ConnectDisconnectBehavior(param, message, connectStore, callBackLoad);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static bool CheckReceiveMessage(CommonParam param, string message)
        {
            bool result = true;
            try
            {

                if (String.IsNullOrEmpty(message))
                {
                    param.Messages.Add("Không tìm thấy bản tin thiết bị gửi lên");
                    throw new Exception("Khong tim thay ban tin thiet bi gui len");
                }

                String[] element = message.Split(ConnectConstant.MESSAGE_SEPARATOR);
                if (element == null || element.Length < 2)
                {
                    param.Messages.Add("Xử lý tập tin gửi về thất bại");
                    throw new Exception("Khong cat duoc ban tin");
                }
            }
            catch (Exception ex)
            {
                param.HasException = true;
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
