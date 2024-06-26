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

namespace MPS.Processor.Mps000108
{
    class Mps000108ExtendSingleKey : CommonKey
    {
        internal const string PATIENT_CODE = "PATIENT_CODE";
        internal const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        internal const string AGE_STR = "AGE_STR";
        internal const string GENDER_NAME = "GENDER_NAME";
        internal const string DOB_YEAR_STR = "DOB_YEAR_STR";
        internal const string VIR_ADDRESS = "VIR_ADDRESS";
        internal const string ICD_MAIN_TEXT = "ICD_MAIN_TEXT";
        internal const string BLOOD_ABO_CODE = "BLOOD_ABO_CODE";
        internal const string BLOOD_HR_CODE = "BLOOD_HR_CODE";
        internal const string EXP_MEST_CODE = "EXP_MEST_CODE";
        internal const string VIR_TOTAL_PRICE = "VIR_TOTAL_PRICE";
        internal const string VIR_TOTAL_PRICE_OTHER = "VIR_TOTAL_PRICE_OTHER";
        internal const string ADDRESS = "ADDRESS";
        internal const string CAREER_NAME = "CAREER_NAME";
        internal const string DOB = "DOB";
        internal const string MILITARY_RANK_NAME = "MILITARY_RANK_NAME";
        internal const string NATIONAL_NAME = "NATIONAL_NAME";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string WORK_PLACE_NAME = "WORK_PLACE_NAME";
        internal const string EXP_MEST_CODE_BARCODE = "EXP_MEST_CODE_BARCODE";
        internal const string BED_CODE = "BED_CODE";
        internal const string BED_NAME = "BED_NAME";
    }
}
