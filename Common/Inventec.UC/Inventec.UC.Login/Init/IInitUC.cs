using Inventec.UC.Login.UCD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Inventec.UC.Login.Init
{
    interface IInitUC
    {
        UserControl Init(MainLogin.EnumTemplate Template, InitUCD Data);
    }
}
