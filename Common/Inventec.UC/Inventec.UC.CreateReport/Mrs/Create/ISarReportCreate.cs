using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Mrs.Create
{
    interface ISarReportCreate
    {
        bool Create(MRS.SDO.CreateReportSDO data);
    }
}
