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

namespace HIS.UC.AddressCombo.ADO
{
    public class SetADO
    {
        public SetADO() { }

        public List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> Provinces { get; set; }
        public List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> Districts { get; set; }
        public List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> Communes { get; set; }
    }
}
