using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite.Base
{
    interface IDelegacyListT
    {
        List<T> Execute<T>();
    }
}
