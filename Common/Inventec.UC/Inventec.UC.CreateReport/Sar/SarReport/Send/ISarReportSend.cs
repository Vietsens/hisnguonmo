using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Sar.SarReport.Send
{
    interface ISarReportSend
    {
        SAR.EFMODEL.DataModels.SAR_REPORT Send();
    }
}
