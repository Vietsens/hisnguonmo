using Inventec.UC.ComboDistrict.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Init
{
    interface IInit
    {
        System.Windows.Forms.UserControl InitCombo(string Template, DataInitDistrict Data);
    }
}
