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

namespace MPS.Processor.Mps000309
{
    class Mps000309ExtendSingleKey : CommonKey
    {
        internal const string PARENT_ORGANIZATION_NAME = "PARENT_ORGANIZATION_NAME";
        internal const string ORGANIZATION_NAME = "ORGANIZATION_NAME";
        internal const string NATIONAL_NOT_VN_NAME = "NATIONAL_NOT_VN_NAME";
        internal const string NATIONAL_NOT_VN_CODE = "NATIONAL_NOT_VN_CODE";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string GENDER_MALE_TICK = "GENDER_MALE_TICK";
        internal const string GENDER_FEMALE_TICK = "GENDER_FEMALE_TICK";
        internal const string GENDER_UNKNOWN_TICK = "GENDER_UNKNOWN_TICK";
        internal const string CURRENT_DATE_SEPARATE_STR = "CURRENT_DATE_SEPARATE_STR";
        internal const string BARCODE_PATIENT_CODE_STR = "PATIENT_CODE_BAR";
        internal const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE";
        internal const string LOGIN_USER_NAME = "LOGIN_USER_NAME";
        internal const string LOGIN_NAME = "LOGIN_NAME";
        internal const string HEIN_CARD_TO_TIME_STR = "HEIN_CARD_TO_TIME_STR";
        internal const string HEIN_CARD_FROM_TIME_STR = "HEIN_CARD_FROM_TIME_STR";
    }
}
