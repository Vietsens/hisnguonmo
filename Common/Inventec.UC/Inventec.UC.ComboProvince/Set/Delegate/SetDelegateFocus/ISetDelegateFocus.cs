using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.SetDelegateFocus
{
    interface ISetDelegateFocus
    {
        bool SetDelegateLoadFocus(System.Windows.Forms.UserControl UC, SetFocusCboDistrict Focus);
    }
}
