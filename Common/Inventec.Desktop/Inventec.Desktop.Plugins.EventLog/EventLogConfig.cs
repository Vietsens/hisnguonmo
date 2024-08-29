using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Plugins.EventLog
{
    class EventLogConfig
    {
        internal static Inventec.Common.WebApiClient.ApiConsumer SdaConsumer { get; set; }
        internal static long NumPageSize { get; set; }
    }
}
