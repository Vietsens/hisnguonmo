using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Get.GetAccountLogin
{
    class GetAccountLogin : IGetAccountLogin
    {
        public UCD.LoginSuccessUCD GetLoginAccount(System.Windows.Forms.UserControl UC)
        {
            UCD.LoginSuccessUCD result = null;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCLogin = (Design.Template1.Template1)UC;
                    result = UCLogin.GetAccountLogin();
                }
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
