using Inventec.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType
{
    public delegate void CreateReport_Click(object reportType);
    public delegate object UpdateDataForPaging(Data.SearchData data, object param, ref CommonParam resultParam);
    //public delegate void ReLoadGridViewDelegate(object lisReportType);
}
