using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ScheduleReport
{
    public delegate MRS.SDO.CreateReportSDO GetSarReportSDOByDelegate();
    public delegate void ProcessHasException(CommonParam param);

}
