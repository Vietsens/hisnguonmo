using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Development.Init
{
    class InitFactory
    {
        internal static IInit MakeIInit()
        {
            IInit result = null;
            try
            {
                result = new Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
