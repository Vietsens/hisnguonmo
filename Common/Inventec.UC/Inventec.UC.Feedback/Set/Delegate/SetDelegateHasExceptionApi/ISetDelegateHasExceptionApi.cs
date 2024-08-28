using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Feedback.Set.Delegate.SetDelegateHasExceptionApi
{
    interface ISetDelegateHasExceptionApi
    {
        bool SetDelegateHasException(UserControl UC, HasExceptionApi HasException);
    }
}
