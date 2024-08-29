using Inventec.Common.ConnectDevice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice
{
    public delegate Task<GetConnectTerminalInfo> CallApiGetConnectTerminalInfo(GetConnectTerminalInfo sendTerminalInfo);
    public delegate void ConnectSuccessfull(bool success, string message, ConnectToDevice.EnumConnect enumC);
    public delegate void ReadCardInfo(string[] strArr);
    public delegate void ReceiveMessage(string message);
}
