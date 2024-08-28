using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince
{
    public delegate void LoadComboDistrictFromProvince(string provinceCode);
    public delegate void SetValueCboDistrictAndCboCommune();
    public delegate void SetFocusCboDistrict(bool valid);
}
