using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.Init
{
    class InitUCFactory
    {
        internal static IInitUC MakeIInitUC(Data.InitData data)
        {
            IInitUC result = null;
            try
            {
                result = new InitUC(data);
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
