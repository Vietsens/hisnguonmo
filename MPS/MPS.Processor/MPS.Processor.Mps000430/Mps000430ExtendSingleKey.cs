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

namespace MPS.Processor.Mps000430
{
    class Mps000430ExtendSingleKey : CommonKey
    {
        internal const string DOB_STR = "DOB_STR";
        internal const string YEAR_STR = "YEAR_STR";
        internal const string AGE_STR = "AGE_STR";
        internal const string DESCRIPTION = "DESCRIPTION";

        internal const string AMOUNT = "AMOUNT";
        internal const string AMOUNT_TEXT = "AMOUNT_TEXT";
        internal const string AMOUNT_TEXT_UPPER_FIRST = "AMOUNT_TEXT_UPPER_FIRST";

        internal const string BARCODE_TREATMENT_CODE = "TREATMENT_CODE_BAR";
        internal const string BARCODE_PATIENT_CODE = "PATIENT_CODE_BAR";

        internal const string PATIENT_TYPE_NAME = "PATIENT_TYPE_NAME";

        internal const string PAY_FORM_CODE = "PAY_FORM_CODE";
        internal const string PAY_FORM_NAME = "PAY_FORM_NAME";


    }
}
