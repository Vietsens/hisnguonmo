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
    public class PingBehavior : ReceiveBase, IRun
    {
        public PingBehavior(CommonParam param, string message, Inventec.Common.Rs232.Connector connectCom, ConnectStore connectStore)
            : base(param, message,connectCom, connectStore)
        { 
        
        }

        bool IRun.Run()
        {
            bool result = true;
            try
            {
                String[] element = null;
                result = this.Check(ref element);
                string sendMessage = new StringBuilder()
                    .Append(element[0]).Append(ConnectConstant.MESSAGE_SEPARATOR)
                    .Append(result ? ConnectConstant.RESPONSE_SUCCESS : ConnectConstant.RESPONSE_CORRECT).ToString();
                this.Send(sendMessage);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            try
            {

            }
            catch (Exception)
            {

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
