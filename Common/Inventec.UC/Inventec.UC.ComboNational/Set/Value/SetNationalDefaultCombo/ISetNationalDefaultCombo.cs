using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboNational.Set.Value.SetNationalDefaultCombo
{
    interface ISetNationalDefaultCombo
    {
        void SetDefaultNational(UserControl UC, SDA.EFMODEL.DataModels.SDA_NATIONAL National);
    }
}
