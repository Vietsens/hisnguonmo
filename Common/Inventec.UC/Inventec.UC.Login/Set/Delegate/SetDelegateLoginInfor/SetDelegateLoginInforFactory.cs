using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetDelegateLoginInfor
{
    class SetDelegateLoginInforFactory
    {
        internal static ISetDelegateLoginInfor MakeISetDelegateLoginInfor()
        {
            ISetDelegateLoginInfor result = null;
            try
            {
                result = new SetDelegateLoginInfor();
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
