using Inventec.UC.ChangePassword.Init;
using Inventec.UC.ChangePassword.Set.SetDelegateChangePassSuccess;
using Inventec.UC.ChangePassword.Set.SetDelegateHasException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ChangePassword
{
    public partial class MainChangePassword
    {
        public enum EnumTemplate
        {
            TEMPLATE2
        }

        public UserControl Init(EnumTemplate Template, Data.DataInitChangePass Data)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(Template, Data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public bool SetDelegateHasExceptionApiChangePass(UserControl UC, HasExceptionApi HasExcep)
        {
            bool result = false;
            try
            {
                result = SetDelegateHasExceptionFactory.MakeISetDelegateHasException().SetDelegateException(UC, HasExcep);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateChangeSuccess(UserControl UC, ChangePasswordSuccess ChangeSuccess)
        {
            bool result = false;
            try
            {
                result = SetDelegateChangePassSuccessFactory.MakeISetDelegateChangePassSuccess().SetDelegateChangeSucess(UC, ChangeSuccess);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
