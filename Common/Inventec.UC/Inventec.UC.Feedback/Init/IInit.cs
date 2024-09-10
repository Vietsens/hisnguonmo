using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Feedback.Init
{
    interface IInit
    {
        UserControl InitUC(MainFeedback.EnumTemplate Template, Data.DataInitFeedback Data);
    }
}
