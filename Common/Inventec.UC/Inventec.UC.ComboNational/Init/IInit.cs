using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Init
{
    interface IInit
    {
        System.Windows.Forms.UserControl InitCombo(string template, Inventec.UC.ComboNational.Data.DataInitNational Data);
    }
}
