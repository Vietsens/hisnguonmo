﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboCommune.Set.SetDelegateSetFocusControl
{
    interface ISetDelegateSetFocusControl
    {
        bool SetDelegateSetFocusNext(UserControl UC, SetFocusNextControl Focus);
    }
}
