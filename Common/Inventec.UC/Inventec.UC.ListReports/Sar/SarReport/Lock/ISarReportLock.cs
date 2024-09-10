using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Sar.SarReport.Lock
{
    interface ISarReportLock
    {
        SAR.EFMODEL.DataModels.SAR_REPORT Lock();
    }
}
