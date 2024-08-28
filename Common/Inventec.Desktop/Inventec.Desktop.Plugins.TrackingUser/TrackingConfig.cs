using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Plugins.TrackingUser
{
    internal class TrackingConfig
    {
        private static Inventec.Common.WebApiClient.ApiConsumer acsConsumer;
        internal static Inventec.Common.WebApiClient.ApiConsumer AcsConsumer
        {
            get
            {
                return acsConsumer;
            }
            set
            {
                acsConsumer = value;
            }
        }


        private static string appCode;
        internal static string AppCode
        {
            get
            {
                return appCode;
            }
            set
            {
                appCode = value;
            }
        }
    }
}
