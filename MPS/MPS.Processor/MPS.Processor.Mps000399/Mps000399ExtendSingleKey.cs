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

namespace MPS.Processor.Mps000399
{
    class Mps000399ExtendSingleKey : CommonKey
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
        //internal const string PHONE = "PHONE";
        internal const string CREATE_TIME_TRAN = "CREATE_TIME_TRAN";
        internal const string ICD_NGT_TEXT = "ICD_NGT_TEXT";
        internal const string NEXT_DEPARTMENT_TRAN = "NEXT_DEPARTMENT_TRAN";
        //internal const string HOSPITALIZATION_REASON = "HOSPITALIZATION_REASON";
        //internal const string PATHOLOGICAL_PROCESS = "PATHOLOGICAL_PROCESS";
        //internal const string PATHOLOGICAL_HISTORY = "PATHOLOGICAL_HISTORY";
        //internal const string PATHOLOGICAL_HISTORY_FAMILY = "PATHOLOGICAL_HISTORY_FAMILY";
        //internal const string FULL_EXAM = "FULL_EXAM";
        //internal const string PART_EXAM_OEND = "PART_EXAM_OEND";
        //internal const string PART_EXAM_STOMATOLOGY = "PART_EXAM_STOMATOLOGY";
        //internal const string PART_EXAM_MUSCLE_BONE = "PART_EXAM_MUSCLE_BONE";
        //internal const string PART_EXAM_KIDNEY_UROLOGY = "PART_EXAM_KIDNEY_UROLOGY";
        //internal const string PART_EXAM_RESPIRATORY = "PART_EXAM_RESPIRATORY";
        //internal const string PART_EXAM_EYE = "PART_EXAM_EYE";
        //internal const string PART_EXAM_ENT = "PART_EXAM_ENT";
        //internal const string PART_EXAM_NEUROLOGICAL = "PART_EXAM_NEUROLOGICAL";
        //internal const string PART_EXAM_CIRCULATION = "PART_EXAM_CIRCULATION";
        //internal const string PULSE = "PULSE";
        //internal const string TEMPERATURE = "TEMPERATURE";
        //internal const string BLOOD_PRESSURE = "BLOOD_PRESSURE";
        //internal const string BLOOD_PRESSURE_MAX = "BLOOD_PRESSURE_MAX";
        //internal const string BLOOD_PRESSURE_MIN = "BLOOD_PRESSURE_MIN";
        //internal const string BREATH_RATE = "BREATH_RATE";
        //internal const string WEIGHT = "WEIGHT";
        //internal const string PART_EXAM = "PART_EXAM";
        //internal const string DESCRIPTION = "DESCRIPTION";
        //internal const string CONCLUDE = "CONCLUDE";
        //internal const string ICD_CODE = "ICD_CODE";
        //internal const string ICD_NAME = "ICD_NAME";
        //internal const string DEPARTMENT_NAME = "DEPARTMENT_NAME";
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

        public static string END_DEPARTMENT_NAME = "END_DEPARTMENT_NAME";
    }
}
