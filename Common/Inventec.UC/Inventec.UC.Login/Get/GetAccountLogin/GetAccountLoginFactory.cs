using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Get.GetAccountLogin
{
    class GetAccountLoginFactory
    {
        internal static IGetAccountLogin MakeIGetAccountLogin()
        {
            IGetAccountLogin result = null;
            try
            {
                result = new GetAccountLogin();
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
