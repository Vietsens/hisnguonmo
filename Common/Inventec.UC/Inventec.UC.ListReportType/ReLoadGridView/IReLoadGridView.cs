using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ListReportType.ReLoadGridView
{
    interface IReLoadGridView
    {
        bool ReLoad(UserControl UC);
    }
}
