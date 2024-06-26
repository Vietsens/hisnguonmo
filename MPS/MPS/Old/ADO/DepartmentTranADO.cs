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

namespace MPS.ADO
{
    public class DepartmentTranADO : MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN
    {
        public string OPEN_TIME_SEPARATE_STR { get; set; }
        public string OPEN_DATE_SEPARATE_STR { get; set; }
        public string CLOSE_TIME_SEPARATE_STR { get; set; }
        public string CLOSE_TIME_SEPARATE_FROM_TIME_STR { get; set; }
        public string CLOSE_DATE_SEPARATE_FROM_TIME_STR { get; set; }
        public string CLOSE_DATE_SEPARATE_STR { get; set; }
        public string DEPARTMENT_NAME_CLOSE { get; set; }
    }
}
