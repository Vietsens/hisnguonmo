using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ChangePassword.Set.SetDelegateChangePassSuccess
{
    interface ISetDelegateChangePassSuccess
    {
        bool SetDelegateChangeSucess(UserControl UC, ChangePasswordSuccess ChangeSuccess);
    }
}
