using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboTHX.Set.SetValueComboTHX
{
    interface ISetValueComboTHX
    {
        void SetValue(UserControl UC, SDA.EFMODEL.DataModels.V_SDA_COMMUNE Commune);
    }
}
