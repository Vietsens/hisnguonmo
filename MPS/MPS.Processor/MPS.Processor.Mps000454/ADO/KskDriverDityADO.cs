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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000454.ADO
{
    class KskDriverDityADO : HIS_PERIOD_DRIVER_DITY
    {
        public string NAME_DITY { get; set; }
        public string IS_YES { get; set; }
        public string IS_NO { get; set; }

        public string NAME_DITY_1 { get; set; }
        public string IS_YES_1 { get; set; }
        public string IS_NO_1 { get; set; }

        public string NAME_DITY_2 { get; set; }
        public string IS_YES_2 { get; set; }
        public string IS_NO_2 { get; set; }
    }
}
