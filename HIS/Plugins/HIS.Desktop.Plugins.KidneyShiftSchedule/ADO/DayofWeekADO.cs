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
    internal class DayofWeekADO
    {
        internal DayofWeekADO()
        {
        }
        public int Day { get; set; }
        public string DayofWeek { get; set; }

        internal List<DayofWeekADO> DayofWeekADOs
        {
            get
            {
                List<DayofWeekADO> rs = new List<DayofWeekADO>();
                rs.Add(new DayofWeekADO() { Day = 1, DayofWeek = "2" });
                rs.Add(new DayofWeekADO() { Day = 2, DayofWeek = "3" });
                rs.Add(new DayofWeekADO() { Day = 3, DayofWeek = "4" });
                rs.Add(new DayofWeekADO() { Day = 4, DayofWeek = "5" });
                rs.Add(new DayofWeekADO() { Day = 5, DayofWeek = "6" });
                rs.Add(new DayofWeekADO() { Day = 6, DayofWeek = "7" });
                rs.Add(new DayofWeekADO() { Day = 0, DayofWeek = "CN" });
                return rs;
            }
        }

    }
}
