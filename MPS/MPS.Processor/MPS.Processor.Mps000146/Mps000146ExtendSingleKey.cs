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

namespace MPS.Processor.Mps000146
{
    class Mps000146ExtendSingleKey : CommonKey
    {
        internal static string AGE = "AGE";
        internal static string ICD_NAME = "ICD_NAME";
        internal static string YEAR = "YEAR";
        internal static string GENDER_NAME = "GENDER_NAME";
        internal static string START_TIME_STR = "START_TIME";
        internal static string FINISH_TIME_STR = "FINISH_TIME";
    }
}
