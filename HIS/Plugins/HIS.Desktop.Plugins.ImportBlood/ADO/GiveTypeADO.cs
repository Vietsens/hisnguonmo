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

namespace HIS.Desktop.Plugins.ImportBlood.ADO
{
    public class GiveTypeADO
    {
        public long ID { get; set; }
        public string GiveType { get; set; }

        public static List<GiveTypeADO> ListGiveType
        {
            get
            {
                List<GiveTypeADO> result = new List<GiveTypeADO>();
                result.Add(new GiveTypeADO { ID = 1, GiveType = "Tình nguyện" });
                result.Add(new GiveTypeADO { ID = 2, GiveType = "Chuyên nghiệp" });
                result.Add(new GiveTypeADO { ID = 3, GiveType = "Người nhà" });
                result.Add(new GiveTypeADO { ID = 4, GiveType = "Tự thân" });
                return result;
            }
        }
    }
}
