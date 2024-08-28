using Inventec.UC.ComboProvince.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboProvince.Init
{
    interface IInit
    {
        UserControl InitCombo(string template, DataInitProcinve Data);
    }
}
