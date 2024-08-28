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
