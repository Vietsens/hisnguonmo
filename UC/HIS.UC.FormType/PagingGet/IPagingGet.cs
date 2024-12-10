using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.PagingGet
{
    interface IPagingGet
    {
        ApiResultObject<List<HIS.UC.FormType.HisMultiGetString.DataGet>> Run();
    }
}
