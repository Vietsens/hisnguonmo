using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Sar.SarReport.Send
{
    class SarReportSendFactory : Inventec.UC.ListReports.Base.GetBase
    {
        internal SarReportSendFactory()
            : base()
        {

        }

        internal SarReportSendFactory(CommonParam paramFactory)
            : base(paramFactory)
        {

        }

        internal SarReportSend createFactory(SAR.EFMODEL.DataModels.SAR_REPORT Data)
        {
            try
            {
                SarReportSend sendConcrete = new SarReportSend(param);
                sendConcrete.Behavior = new SarReportSendBehaviorDefault(param, Data);
                return sendConcrete;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
