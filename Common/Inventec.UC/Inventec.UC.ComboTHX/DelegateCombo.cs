using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX
{
    public delegate void LoadComboDistrict(string provinceCode);
    public delegate void SetValueComboProvince(string ProvinceCode);
    public delegate void SetValueComboDistrict(string DistrictCode);
    public delegate void SetValueComboCommune(string CommuneCode);
    public delegate void LoadComboCommune(string districtCode);
    public delegate void FocusComboProvince();
    public delegate void FocusNextControl();
}
