using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboNational.Set.Delegate.SetDelegateFocusNextControl
{
    interface ISetDelegateFocusNextControl
    {
        bool SetDelegateFocucNext(UserControl UC, FocusNextControl FocusNext);
    }
}
