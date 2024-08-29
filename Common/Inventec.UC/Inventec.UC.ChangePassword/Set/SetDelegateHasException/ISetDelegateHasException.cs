using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ChangePassword.Set.SetDelegateHasException
{
    interface ISetDelegateHasException
    {
        bool SetDelegateException(UserControl UC, HasExceptionApi HasExcep);
    }
}
