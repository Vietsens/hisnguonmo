using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports
{
    public delegate void ProcessHasException(Inventec.Core.CommonParam param);
    public delegate void ProcessCopy(SAR.EFMODEL.DataModels.V_SAR_REPORT report);
}
