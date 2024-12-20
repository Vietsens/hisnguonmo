using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType
{
    public delegate void ProcessFormType(Type type);
    public delegate void GetCashierUser();
    public delegate void GetSaleUser();
    public delegate void PagingGetDelegate(CommonParam param, object filter, ref object result);
}
