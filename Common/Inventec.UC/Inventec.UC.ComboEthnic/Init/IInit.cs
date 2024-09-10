using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboEthnic.Init
{
    interface IInit
    {
        UserControl InitCombo(string Template, Data.DataInitEthnic Data);
    }
}
