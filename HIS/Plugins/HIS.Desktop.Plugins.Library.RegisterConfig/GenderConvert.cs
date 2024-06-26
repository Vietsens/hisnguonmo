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

namespace HIS.Desktop.Plugins.Library.RegisterConfig
{
    public class GenderConvert
    {
        public static string TextToNumber(string ge)
        {
            return (ge == "Nữ") ? "2" : "1";
        }

        public static string HisToHein(string ge)
        {
            return (ge == "1") ? "2" : "1";
        }

        public static long HeinToHisNumber(string ge)
        {
            return (ge == "1" ? IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE : IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE);
        }
    }
}
