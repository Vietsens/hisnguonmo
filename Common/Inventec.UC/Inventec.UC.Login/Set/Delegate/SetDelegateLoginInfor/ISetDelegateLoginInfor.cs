using Inventec.UC.Login.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Login.Set.SetDelegateLoginInfor
{
    interface ISetDelegateLoginInfor
    {
        bool SetDelegateLogin(UserControl UC, LoginInfor Infor);
    }
}
