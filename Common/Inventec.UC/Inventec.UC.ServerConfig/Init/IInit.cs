using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ServerConfig.Init
{
    interface IInit
    {
        UserControl InitUC(MainServerConfig.EnumTemplate Template, CloseFormConfigSystem CloseForm);
    }
}
