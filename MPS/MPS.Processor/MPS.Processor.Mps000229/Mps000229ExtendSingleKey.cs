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

namespace MPS.Processor.Mps000229
{
    class Mps000229ExtendSingleKey : CommonKey
    {
        internal const string PATIENT_CODE_BAR = "PATIENT_CODE_BAR";
        internal const string CREATE_TIME_TRAN = "CREATE_TIME_TRAN";
        internal const string IN_TIME_STR = "IN_TIME_STR";
        internal const string OUT_TIME_STR = "OUT_TIME_STR";
        internal const string CLINICAL_IN_TIME_STR = "CLINICAL_IN_TIME_STR";
        internal const string AGE = "AGE";
    }
}
