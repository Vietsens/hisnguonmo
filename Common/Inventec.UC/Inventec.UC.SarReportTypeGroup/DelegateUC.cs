using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportTypeGroup
{
    public delegate void RowCellClickDelegate(object reportTypeGroup);
    public delegate object UpdateReportTypeGroup(Data.SearchData data, object param, ref CommonParam resultParam);
}
