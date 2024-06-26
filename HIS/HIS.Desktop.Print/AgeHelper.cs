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

namespace HIS.Desktop.Print
{
    public class AgeHelper
    {
        public static string CalculateAgeFromYear(long ageYearNumber)
        {
            string tuoi = "";
            try
            {
                DateTime? dtAge = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ageYearNumber);
                long age = DateTime.UtcNow.Year - dtAge.Value.Year;
                if (age <= 0)
                {
                    age = 1;
                }
                tuoi = string.Format("{0:00}", age);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                tuoi = "";
            }
            return tuoi;
        }
    }
}
