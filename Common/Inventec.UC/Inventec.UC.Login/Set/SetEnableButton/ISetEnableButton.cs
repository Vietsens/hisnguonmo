using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Login.Set.SetEnableButton
{
    interface ISetEnableButton
    {
        void SetButtonEnable(UserControl UC, bool valid);
    }
}
