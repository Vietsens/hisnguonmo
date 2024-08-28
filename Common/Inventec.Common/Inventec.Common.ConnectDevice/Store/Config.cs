using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Store
{
    class Config
    {
        internal static string TOS_CONNECTOR_CODE = System.Configuration.ConfigurationSettings.AppSettings["Tos.Connector.Code"] ?? "";
        internal static int Time = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["Cos.SendToDevice.TimeOut"] ?? "15000");
    }
}
