using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Sar.SarReport.Send
{
    class SarReportSend : Inventec.UC.ListReports.Base.BusinessBase
    {
        internal SarReportSend()
            : base()
        {

        }

        internal SarReportSend(CommonParam paramSend)
            : base(paramSend)
        {

        }

        internal ISarReportSend Behavior { get; set; }

        internal SAR.EFMODEL.DataModels.SAR_REPORT Send()
        {
            SAR.EFMODEL.DataModels.SAR_REPORT result = null;
            try
            {
                result = Behavior.Send();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
