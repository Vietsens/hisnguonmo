using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Set.Delegate
{
    class SetDelegateCloseContainerFormFactory
    {
        internal static ISetDelegateCloseContainerForm MakeISetDelegateCloseContainerFormCreateReport()
        {
            ISetDelegateCloseContainerForm result = null;
            try
            {
                result = new SetDelegateCloseContainerForm();
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
