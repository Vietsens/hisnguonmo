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
    class SendBehaviorFactory
    {
        internal enum EnumSend
        {
            WHO,
            CONNECT,
            OPEN,
            DISCONNECT
        }

        internal static ISend MakeISend(EnumSend enumS)
        {
            ISend result = null;
            try
            {
                switch (enumS)
                {
                    case EnumSend.WHO:
                        result = new SendBehaviorWho();
                        break;
                    case EnumSend.CONNECT:
                        result = new SendBehaviorConnect();
                        break;
                    case EnumSend.OPEN:
                        result = new SendBehaviorOpen();
                        break;
                    case EnumSend.DISCONNECT:
                        result = new SendBehaviorDisconnect();
                        break;
                    default:
                        break;
                }
                if (result == null) Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => enumS), enumS));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
