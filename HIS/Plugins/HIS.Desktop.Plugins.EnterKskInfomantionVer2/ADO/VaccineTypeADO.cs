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

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO
{
    class VaccineTypeADO
    {
        public long ID { get; set; }
        public long UNEI_VATY_ID { get; set; }
        public string VACCINE_TYPE_NAME { get; set; }
        public long? CONDITION_TYPE { get; set; }
        public bool IS_YES { get; set; }
        public bool IS_NO { get; set; }
        public bool IS_FORGOT { get; set; }
    }
}
