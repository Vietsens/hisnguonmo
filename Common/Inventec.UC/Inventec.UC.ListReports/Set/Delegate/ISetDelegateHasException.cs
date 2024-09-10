using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ListReports.Set.Delegate
{
    interface ISetDelegateHasException
    {
        bool SetDelegate(UserControl UC, ProcessHasException hasException);
    }
}
