using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ChangePassword.Init
{
    interface IInit
    {
        UserControl InitUC(MainChangePassword.EnumTemplate Template, Data.DataInitChangePass Data);
    }
}
