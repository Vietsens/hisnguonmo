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
