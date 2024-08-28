using Inventec.UC.Login.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Login.Set.SetDelegateButtonConfig
{
    interface ISetDelefateButtonConfig
    {
        bool SetDelegateConfig(UserControl UC, EventButtonConfig btnConfig);
    }
}
