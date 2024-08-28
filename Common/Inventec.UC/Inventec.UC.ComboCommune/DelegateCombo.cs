using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune
{
    public delegate void SetValueComboTHX(SDA.EFMODEL.DataModels.V_SDA_COMMUNE Commune);
    public delegate void SetFocusNextControl();
    public delegate object GetValueComboProvince();
    public delegate object GetValueComboDistrict();
}
