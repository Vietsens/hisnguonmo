using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ScheduleReport.Set.Delegate.HasException
{
    interface ISetDelegateHasException
    {
        bool SetDelegate(UserControl UC, ProcessHasException HasException);
    }
}
