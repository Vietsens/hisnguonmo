using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboDistrict.Set.SetDelegateGetValueProvince
{
    interface ISetDelegateGetValueProvince
    {
        bool SetDelegateGetProvince(UserControl UC, GetValueComboProvince getValue);
    }
}
