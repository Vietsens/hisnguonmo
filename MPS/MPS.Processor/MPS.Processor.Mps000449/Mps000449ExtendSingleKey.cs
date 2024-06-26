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

namespace MPS.Processor.Mps000449
{
    class Mps000449ExtendSingleKey : CommonKey
    {

        internal const string PATIENT_CODE_BARCODE = "PATIENT_CODE_BARCODE";
        internal const string TREATMENT_CODE_BARCODE = "TREATMENT_CODE_BARCODE";
        internal const string SERVICE_REQ_CODE_BARCODE = "SERVICE_REQ_CODE_BARCODE";

        internal const string TDL_PATIENT_NAME = "TDL_PATIENT_NAME";
        internal const string D_O_B = "DOB_YEAR";
        internal const string GENDER_NAME = "GENDER_NAME";
        internal const string HEALTH_EXAM_RANK = "HEALTH_EXAM_RANK";
    }
}
