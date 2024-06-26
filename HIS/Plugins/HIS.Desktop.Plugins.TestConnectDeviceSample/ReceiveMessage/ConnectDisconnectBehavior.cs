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
    public class ConnectDisconnectBehavior : ReceiveBase, IRun
    {
        private CallBackLoad callBackLoad;

        public ConnectDisconnectBehavior(CommonParam param, string message, ConnectStore connectStore, CallBackLoad callBackLoad)
            : base(param, message, connectStore)
        {
            this.callBackLoad = callBackLoad;
        }

        bool IRun.Run()
        {
            bool result = true;
            try
            {
                String[] element = null;
                if (this.Check(ref element))
                {
                    string messageId = element[0];
                    if (callBackLoad != null)
                    {
                        if (messageId.Equals(connectStore.messageIdConnect))
                        {
                            callBackLoad(ConnectConstant.MESSAGE_CONNECT);
                        }
                        else if (messageId.Equals(connectStore.messageIdDisconnect))
                        {
                            callBackLoad(ConnectConstant.MESSAGE_DISCONNECT);
                        }
                        else
                        { 
                            throw new Exception("Khong xac dinh duoc ban tin ma thiet bi gui len: ");
                        }
                    }
                    else
                    {
                        throw new Exception("Chua khoi tao ham tra ve CallBackLoad");
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => message), message));
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public bool Check(ref String[] element)
        {
            bool result = true;
            try
            {
                element = message.Split(ConnectConstant.MESSAGE_SEPARATOR);
                if (element != null && element.Length != 2)
                {
                    param.Messages.Add("Thiết bị gửi sai định dạng thông tin");
                    throw new Exception("element.Length != 2 .");
                }

                if (!element[1].Equals(ConnectConstant.RESPONSE_SUCCESS))
                {
                    param.Messages.Add("Thiết bị gửi về kết nối thất bại");
                    throw new Exception("Thiet bi gui ve ket noi that bai");
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
