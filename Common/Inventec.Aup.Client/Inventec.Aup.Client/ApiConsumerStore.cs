using Inventec.Aup.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Client.Base
{
    internal class ApiConsumerStore
    {
        internal static Inventec.Common.WebApiClient.ApiConsumer AupConsumer { get { return new Inventec.Common.WebApiClient.ApiConsumer(AupConstant.BASE_URI, AupConstant.APP_CODE); } }
    }
}
