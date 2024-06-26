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

namespace MPS.Processor.Mps000321.PDO
{
    public class SingleKeyValue
    {
        public string departmentName { get; set; }
        public string roomName { get; set; }

        public string RATIO_STR { get; set; }
        public long TOTAL_DAY { get; set; }
        public string CURRENT_DATE_SEPARATE_FULL_STR { get; set; }
        public string USERNAME_RETURN_RESULT { get; set; }
        public string STATUS_TREATMENT_OUT { get; set; }
        public string PAY_VIEW_OPTION { get; set; }
    }
}
