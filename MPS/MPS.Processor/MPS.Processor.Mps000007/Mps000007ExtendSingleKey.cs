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

namespace MPS.Processor.Mps000007
{
    class Mps000007ExtendSingleKey : CommonKey
    {
        internal const string PARENT_ORGANIZATION_NAME = "PARENT_ORGANIZATION_NAME";
        internal const string ORGANIZATION_NAME = "ORGANIZATION_NAME";
        internal const string EXECUTE_ROOM_NAME = "EXECUTE_ROOM_NAME";
        internal const string AGE = "AGE";
        internal const string NATIONAL_NOT_VN_NAME = "NATIONAL_NOT_VN_NAME";
        internal const string NATIONAL_NOT_VN_CODE = "NATIONAL_NOT_VN_CODE";
        internal const string EXECUTE_DEPARTMENT_NAME = "EXECUTE_DEPARTMENT_NAME";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string IS_HEIN = "IS_HEIN";
        internal const string PATIENT_TYPE_NAME = "PATIENT_TYPE_NAME";
        internal const string TO_DATE_SEPARATE_STR = "TO_DATE_SEPARATE_STR";
        internal const string HEIN_CARD_TO_TIME = "HEIN_CARD_TO_TIME";
        internal const string DHST_NOTE = "DHST_NOTE";
        internal const string CREATE_TIME_TRAN = "CREATE_TIME_TRAN";
        internal const string ICD_NGT_TEXT = "ICD_NGT_TEXT";
        internal const string NEXT_DEPARTMENT_TRAN = "NEXT_DEPARTMENT_TRAN";
        internal const string CLS_NAMES = "CLS_NAMES";
        internal const string RESULT_NOTE = "RESULT_NOTE";
        internal const string CURRENT_DATE_SEPARATE_STR = "CURRENT_DATE_SEPARATE_STR";
        internal const string NEXT_DEPARTMENT_NAME = "NEXT_DEPARTMENT_NAME";
        internal const string PROVISIONAL_DIAGNOSIS = "PROVISIONAL_DIAGNOSIS";
        internal const string NEXT_DEPARTMENT_CODE = "NEXT_DEPARTMENT_CODE";
        internal const string BARCODE_PATIENT_CODE_STR = "PATIENT_CODE_BAR";
        internal const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE";
        internal const string LOGIN_USER_NAME = "LOGIN_USER_NAME";
        internal const string LOGIN_NAME = "LOGIN_NAME";
        internal const string RATIO_STR = "RATIO_STR";
        internal const string TIME_IN_STR = "TIME_IN_STR";
        internal const string HEIN_CARD_TO_TIME_STR = "HEIN_CARD_TO_TIME_STR";
        internal const string HEIN_CARD_FROM_TIME_STR = "HEIN_CARD_FROM_TIME_STR";
        internal const string ICD_DEPARTMENT_TRAN = "ICD_DEPARTMENT_TRAN";
        internal const string EXP_MEST_BLOOD_LIST = "EXP_MEST_BLOOD_LIST";
        internal const string EXP_MEST_MEDICINE_LIST = "EXP_MEST_MEDICINE_LIST";
        internal const string EXP_MEST_MATERIAL_LIST = "EXP_MEST_MATERIAL_LIST";
        internal const string ORIGINAL_ICD_CODE = "ORIGINAL_ICD_CODE";
        internal const string ORIGINAL_ICD_TEXT = "ORIGINAL_ICD_TEXT";
        internal const string ORIGINAL_ICD_NAME = "ORIGINAL_ICD_NAME";
        internal const string ORIGINAL_ICD_SUB_CODE = "ORIGINAL_ICD_SUB_CODE";
        internal const string EXECUTE_USERNAME = "EXECUTE_USERNAME";
        internal const string HOSPITALIZE_DEPARTMENT_CODE = "HOSPITALIZE_DEPARTMENT_CODE";
        internal const string HOSPITALIZE_DEPARTMENT_NAME = "HOSPITALIZE_DEPARTMENT_NAME";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";
    }
}
