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
using HIS.Desktop.DelegateRegister;

namespace HIS.UC.AddressCombo.ADO
{
    public class UCAddressADO
    {
        public UCAddressADO() { }

        public string Province_Code { get; set; }
        public string Province_Name { get; set; }
        public string District_Code { get; set; }
        public string District_Name { get; set; }
        public string Commune_Code { get; set; }
        public string Commune_Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public DelegateFocusNextUserControl _FocusNextUserControl { get; set; }
    }
}
