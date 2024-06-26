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
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000116
{
    class Mps000116ExtendSingleKey : CommonKey
    {
        internal const string BARCODE_PATIENT_CODE_STR = "BARCODE_PATIENT_CODE";
        internal const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE";
        internal const string IN_TIME_STR = "IN_TIME_STR";
        internal const string OUT_TIME_STR = "OUT_TIME_STR";
        internal const string CLINICAL_IN_TIME_STR = "CLINICAL_IN_TIME_STR";
        internal const string INTRUCTION_TIME_DAY = "INTRUCTION_TIME_DAY";
        internal const string CONCENTRA = "CONCENTRA";
    } 
}
