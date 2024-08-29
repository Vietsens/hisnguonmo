using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateFocusControlNext
{
    interface ISetDelegateFocusControlNext
    {
        bool SetDelegateFocusNext(UserControl UC, FocusNextControl FocusNext);
    }
}
