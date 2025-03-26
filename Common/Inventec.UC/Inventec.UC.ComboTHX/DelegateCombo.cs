/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
