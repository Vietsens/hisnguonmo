using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Set.SetDelegateChangePassSuccess
{
    class SetDelegateChangePassSuccessFactory
    {
        internal static ISetDelegateChangePassSuccess MakeISetDelegateChangePassSuccess()
        {
            ISetDelegateChangePassSuccess result = null;
            try
            {
                result = new SetDelegateChangePassSuccess();
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
