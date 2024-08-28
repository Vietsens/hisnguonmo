using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ScheduleReport.Data
{
    public class DataInit
    {
        internal Inventec.Common.WebApiClient.ApiConsumer sarConsumer;
        internal Inventec.Token.ClientSystem.ClientTokenManager clientToken;
        internal GetSarReportSDOByDelegate getReport;

        public DataInit(Inventec.Common.WebApiClient.ApiConsumer sarcomsumer, Inventec.Token.ClientSystem.ClientTokenManager clienttokenManager, GetSarReportSDOByDelegate getreport)
        {
            try
            {
                this.sarConsumer = sarcomsumer;
                this.clientToken = clienttokenManager;
                this.getReport = getreport;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
