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

namespace MPS.Processor.Mps000151
{
    class Mps000151ExtendSingleKey : CommonKey
    {
        internal const string OPEN_TIME_SEPARATE_STR = "OPEN_TIME_SEPARATE_STR";
        internal const string CLOSE_TIME_SEPARATE_STR = "CLOSE_TIME_SEPARATE_STR";
        internal const string DEPARTMENT_NAME = "DEPARTMENT_NAME";
        internal const string DEBATE_TIME_STR = "DEBATE_TIME_STR";
        internal const string USERNAME_PRESIDENT = "USERNAME_PRESIDENT";
        internal const string PRESIDENT_DESCRIPTION = "PRESIDENT_DESCRIPTION";
        internal const string USERNAME_SECRETARY = "USERNAME_SECRETARY";
        internal const string SECRETARY_DESCRIPTION = "SECRETARY_DESCRIPTION";
        internal const string BARCODE_PATIENT_CODE_STR = "PATIENT_CODE_BAR";
        internal const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE";
        internal const string CARE_ICD_MAIN_TEXT = "CARE_ICD_MAIN_TEXT";
        internal const string CARE_ICD_EXTRA_TEXT = "CARE_ICD_EXTRA_TEXT";

        internal const string GENDER_MALE = "GENDER_MALE";
        internal const string GENDER_FEMALE = "GENDER_FEMALE";
        internal const string D_O_B = "DOB_YEAR";
        internal const string AGE = "AGE";
        internal const string DOB_STR = "DOB_STR";
        internal const string CMND_DATE_STR = "CMND_DATE_STR";
        internal const string NUM_ORDER = "NUM_ORDER";

    }
}
