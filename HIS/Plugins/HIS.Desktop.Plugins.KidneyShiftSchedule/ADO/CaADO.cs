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

namespace HIS.Desktop.Plugins.KidneyShiftSchedule.ADO
{
    internal class CaADO
    {
        internal CaADO()
        {
        }
        public long Value { get; set; }

        internal List<CaADO> CaADOs
        {
            get
            {
                List<CaADO> rs = new List<CaADO>();
                rs.Add(new CaADO() { Value = 1 });
                rs.Add(new CaADO() { Value = 2 });
                rs.Add(new CaADO() { Value = 3 });
                rs.Add(new CaADO() { Value = 4 });
                rs.Add(new CaADO() { Value = 5 });
                return rs;
            }
        }

    }
}
