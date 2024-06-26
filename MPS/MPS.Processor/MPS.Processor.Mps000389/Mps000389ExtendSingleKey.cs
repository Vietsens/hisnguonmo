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
using MPS.ProcessorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000389
{
    class Mps000389ExtendSingleKey : CommonKey
    {
        internal const string DOB_STR = "DOB_STR";
        internal const string YEAR_STR = "YEAR_STR";
        internal const string AGE_STR = "AGE_STR";
        
        internal const string CREATE_DATE_SEPARATE_STR = "CREATE_DATE_SEPARATE_STR";
        internal const string SURG_APPOINTMENT_TIME_STR = "SURG_APPOINTMENT_TIME_STR";
        internal const string SURG_APPOINTMENT_DATE_SEPARATE_STR = "SURG_APPOINTMENT_DATE_SEPARATE_STR";
    }
}
