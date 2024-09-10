using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Feedback.Set.Delegate.SetDelegateCloseFormFeedback
{
    interface ISetDelegateCloseFormFeedback
    {
        bool SetDelegateClose(UserControl UC, CloseForm close);
    }
}
