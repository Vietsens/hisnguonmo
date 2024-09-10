using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Init
{
    class InitUCFactory
    {
        internal static IInitUC MakeIInitUC()
        {
            IInitUC result = null;
            try
            {
                result = new InitUC();
            }
            catch
            {
                result = null;
            }

            return result;
        }
    }
}
