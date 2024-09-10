using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.VoiceCommand
{
    public class ApiConsumers
    {
        private static Inventec.Common.WebApiClient.ApiConsumer rikkeiaiConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer RIKKEIAIConsumer
        {
            get
            {
                if (rikkeiaiConsumer == null)
                {
                    rikkeiaiConsumer = new Inventec.Common.WebApiClient.ApiConsumer(CommandCFG.RikkeiAI__URI, "HIS");
                }
                return rikkeiaiConsumer;
            }
            set
            {
                rikkeiaiConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer wwitaiConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer WITAIConsumer
        {
            get
            {
                if (wwitaiConsumer == null)
                {
                    wwitaiConsumer = new Inventec.Common.WebApiClient.ApiConsumer(CommandCFG.WitAI__URI, "HIS");
                }
                return wwitaiConsumer;
            }
            set
            {
                wwitaiConsumer = value;
            }
        }
       
    }
}
