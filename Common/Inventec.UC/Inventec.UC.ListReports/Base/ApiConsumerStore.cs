using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Base
{
    class ApiConsumerStore
    {
        internal static Inventec.Common.WebApiClient.ApiConsumer SarConsumer { get; set; }
        internal static Inventec.Common.WebApiClient.ApiConsumer SdaConsumer { get; set; }
        internal static Inventec.Common.WebApiClient.ApiConsumer AcsConsumer { get; set; }
    }
}
