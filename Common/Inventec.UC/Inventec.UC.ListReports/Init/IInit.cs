using Inventec.UC.ListReports.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ListReports.Init
{
    interface IInit
    {
        UserControl InitUC(MainListReports.EnumTemplate template, InitData Data);
    }
}
