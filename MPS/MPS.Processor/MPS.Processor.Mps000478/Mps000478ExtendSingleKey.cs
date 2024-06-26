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

namespace MPS.Processor.Mps000478
{
    class Mps000478ExtendSingleKey : CommonKey
    {
        internal const string TDL_PATIENT_CODE_BAR = "TDL_PATIENT_CODE_BAR";

        internal const string TDL_PATIENT_CODE = "TDL_PATIENT_CODE";
        internal const string TREATMENT_CODE = "TREATMENT_CODE";
        internal const string TDL_PATIENT_NAME = "TDL_PATIENT_NAME";
        internal const string TDL_PATIENT_DOB = "TDL_PATIENT_DOB";
        internal const string TDL_PATIENT_GENDER_NAME = "TDL_PATIENT_GENDER_NAME";

        internal const string TDL_PATIENT_ETHNIC_NAME = "TDL_PATIENT_ETHNIC_NAME";
        internal const string TDL_PATIENT_ADDRESS = "TDL_PATIENT_ADDRESS";
        internal const string TDL_PATIENT_CAREER_NAME = "TDL_PATIENT_CAREER_NAME";
        internal const string TDL_HEIN_CARD_NUMBER = "TDL_HEIN_CARD_NUMBER";

        internal const string TDL_HEIN_CARD_FROM_TIME = "TDL_HEIN_CARD_FROM_TIME";
        internal const string TDL_HEIN_CARD_TO_TIME = "TDL_HEIN_CARD_TO_TIME";
        internal const string IN_TIME = "IN_TIME";
        internal const string OUT_TIME = "OUT_TIME";

        internal const string ICD_CODE = "ICD_CODE";
        internal const string ICD_NAME = "ICD_NAME";
        internal const string ICD_SUB_CODE = "ICD_SUB_CODE";
        internal const string ICD_TEXT = "ICD_TEXT";
    }
}
