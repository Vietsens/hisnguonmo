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
    class ReceiveBehaviorFactory
    {
        internal enum EnumReceive
        {
            WHO,
            CONNECT,
            OPEN,
            DISCONNECT
        }

        internal static IReceive MakeIReceive(EnumReceive enumR, string[] data)
        {
            IReceive result = null;
            try
            {
                if (enumR == EnumReceive.WHO)
                {
                    result = new ReceiveBehaviorWho(data);
                }
                else if (enumR == EnumReceive.CONNECT)
                {
                    result = new ReceiveBehaviorConnect(data);
                }
                else if (enumR == EnumReceive.OPEN)
                {
                    result = new ReceiveBehaviorOpen(data);
                }
                else if (enumR == EnumReceive.DISCONNECT)
                {
                    result = new ReceiveBehaviorDisconnect(data);
                }
                if (result == null) Inventec.Common.Logging.LogSystem.Info("Khoi tao factory that bai: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => enumR), enumR));
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
