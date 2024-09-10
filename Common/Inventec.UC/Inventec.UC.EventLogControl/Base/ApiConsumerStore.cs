using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Base
{
    internal class ApiConsumerStore
    {
        internal const string EventLogIndex = "rawsearchdata_sda_event_log";
        internal static Inventec.Common.WebApiClient.ApiConsumer SdaConsumer { get; set; }
        internal static string UriElasticSearchServer;

        private static Nest.ElasticClient elasticClientConsumer;
        internal static Nest.ElasticClient ElasticClientConsumer
        {
            get
            {
                if (elasticClientConsumer == null)
                {
                    var esConfiguration = new Nest.ConnectionSettings(new System.Uri(UriElasticSearchServer));
                    elasticClientConsumer = new Nest.ElasticClient(esConfiguration);
                    esConfiguration.DefaultIndex(EventLogIndex);
                }
                return elasticClientConsumer;
            }
        }
    }
}
