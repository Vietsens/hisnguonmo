using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Base
{
    internal abstract class GetBase : BusinessBase
    {
        protected GetBase()
            : base()
        {

        }

        protected GetBase(CommonParam paramGet)
            : base(paramGet)
        {

        }
    }
}
