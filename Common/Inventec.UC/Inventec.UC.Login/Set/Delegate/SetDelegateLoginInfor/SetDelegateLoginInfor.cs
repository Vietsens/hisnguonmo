using Inventec.UC.Login.Design.Template1;
using Inventec.UC.Login.Design.Template3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetDelegateLoginInfor
{
    class SetDelegateLoginInfor : ISetDelegateLoginInfor
    {
        public bool SetDelegateLogin(System.Windows.Forms.UserControl UC, LoginInfor Infor)
        {
            bool valid = false;
            try
            {
                if (UC.GetType() == typeof(Template1))
                {
                    Template1 UCLogin = (Template1)UC;
                    valid = UCLogin.SetDelegateLoginInfor(Infor);

                }
                else if (UC.GetType() == typeof(Template3))
                {
                    Template3 UCLogin = (Template3)UC;
                    valid = UCLogin.SetDelegateLoginInfor(Infor);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}
