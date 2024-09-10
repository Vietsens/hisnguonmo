using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Design.Template1
{
    internal partial class Template1
    {
        internal UCD.LoginSuccessUCD GetAccountLogin()
        {
            UCD.LoginSuccessUCD result = null;
            try
            {
                result = new UCD.LoginSuccessUCD();
                result.LOGINNAME = txtLoginName.Text.Trim();
                result.PASSWORD = txtPassword.Text;
                //result.IS_AUTOLOGIN = chkAutoLogin.Checked ? "1" : "0";
                result.LANGUAGE = (cbbLanguage.SelectedIndex > -1 ? (cbbLanguage.SelectedIndex == 0 ? "vi" : "en") : "vi");
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
