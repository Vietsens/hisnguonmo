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

namespace MPS.Processor.Mps000174
{
    class Mps000174ExtendSingleKey : CommonKey
    {
        internal const string PARENT_ORGANIZATION_NAME = "PARENT_ORGANIZATION_NAME";
        internal const string ORGANIZATION_NAME = "ORGANIZATION_NAME";
        internal const string EXECUTE_ROOM_NAME = "EXECUTE_ROOM_NAME";
        internal const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        internal const string GENDER_MALE = "GENDER_MALE";
        internal const string GENDER_FEMALE = "GENDER_FEMALE";
        internal const string DOB = "DOB";
        internal const string AGE = "AGE111";
        internal const string CAREER_NAME = "CAREER_NAME";
        internal const string CAREER_CODE = "CAREER_CODE";
        internal const string ETHNIC_NAME = "ETHNIC_NAME";
        internal const string NATIONAL_NOT_VN_NAME = "NATIONAL_NOT_VN_NAME";
        internal const string NATIONAL_NOT_VN_CODE = "NATIONAL_NOT_VN_CODE";
        internal const string DHST_NOTE = "DHST_NOTE";
        internal const string PROVINCE_NAME = "PROVINCE_NAME";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string IS_HEIN = "IS_HEIN";
        internal const string PATIENT_TYPE_NAME = "PATIENT_TYPE_NAME";
        internal const string TO_DATE_SEPARATE_STR = "TO_DATE_SEPARATE_STR";
        internal const string STR_HEIN_CARD_TO_TIME = "STR_HEIN_CARD_TO_TIME";
        internal const string HEIN_CARD_NUMBER = "HEIN_CARD_NUMBER";
        internal const string PHONE = "PHONE";
        internal const string CREATE_TIME = "CREATE_TIME_TRAN";
        internal const string ICD_NGT_TEXT = "ICD_NGT_TEXT";
        internal const string SERE_SERV_PTTTs = "SERE_SERV_PTTT";
        internal const string HOSPITALIZATION_REASON = "HOSPITALIZATION_REASON";
        internal const string PATHOLOGICAL_PROCESS = "PATHOLOGICAL_PROCESS";
        internal const string PATHOLOGICAL_HISTORY = "PATHOLOGICAL_HISTORY";
        internal const string PATHOLOGICAL_HISTORY_FAMILY = "PATHOLOGICAL_HISTORY_FAMILY";
        internal const string FULL_EXAM = "FULL_EXAM";
        internal const string PART_EXAM_OEND = "PART_EXAM_OEND";
        internal const string PART_EXAM_STOMATOLOGY = "PART_EXAM_STOMATOLOGY";
        internal const string PART_EXAM_MUSCLE_BONE = "PART_EXAM_MUSCLE_BONE";
        internal const string PART_EXAM_KIDNEY_UROLOGY = "PART_EXAM_KIDNEY_UROLOGY";
        internal const string PART_EXAM_RESPIRATORY = "PART_EXAM_RESPIRATORY";
        internal const string PART_EXAM_EYE = "PART_EXAM_EYE";
        internal const string PART_EXAM_ENT = "PART_EXAM_ENT";
        internal const string PART_EXAM_NEUROLOGICAL = "PART_EXAM_NEUROLOGICAL";
        internal const string PART_EXAM_CIRCULATION = "PART_EXAM_CIRCULATION";
        internal const string PULSE = "PULSE";
        internal const string TEMPERATURE = "TEMPERATURE";
        internal const string BLOOD_PRESSURE = "BLOOD_PRESSURE";
        internal const string BLOOD_PRESSURE_MAX = "BLOOD_PRESSURE_MAX";
        internal const string BLOOD_PRESSURE_MIN = "BLOOD_PRESSURE_MIN";
        internal const string BREATH_RATE = "BREATH_RATE";
        internal const string WEIGHT = "WEIGHT";
        internal const string PART_EXAM = "PART_EXAM";
        internal const string DESCRIPTION = "DESCRIPTION";
        internal const string CONCLUDE = "CONCLUDE";
        internal const string ICD_CODE = "ICD_CODE";
        internal const string ICD_NAME = "ICD_NAME";
        internal const string DEPARTMENT_NAME = "DEPARTMENT_NAME";
        internal const string RESULT_NOTE = "RESULT_NOTE";
        internal const string CURRENT_DATE_SEPARATE_STR = "CURRENT_DATE_SEPARATE_STR";
        internal const string NEXT_DEPARTMENT_NAME = "NEXT_DEPARTMENT_NAME";
        internal const string CREATE_TIME_TRAN_PATI = "CREATE_TIME_TRAN_PATI";
        internal const string FINISH_TIME_TRAN_PATI = "FINISH_TIME_TRAN_PATI";
        internal const string BARCODE_PATIENT_CODE_STR = "PATIENT_CODE_BAR";
        internal const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE";
        internal const string CREATE_TIME_TRAN = "CREATE_TIME_TRAN";
        internal const string USE_TIME_TO_STR = "USE_TIME_TO_STR";
        internal const string USE_DATE_TO_STR = "USE_DATE_TO_STR";
        internal const string ICD_TREATMENT_NAME = "ICD_TREATMENT_NAME";
        internal const string ICD_TREATMENT_CODE = "ICD_TREATMENT_CODE";
        internal const string ICD_TREATMENT_TEXT = "ICD_TREATMENT_TEXT";
        internal const string TREATMENT_IN_TIME = "TREATMENT_IN_TIME";
        internal const string TREATMENT_OUT_TIME = "TREATMENT_OUT_TIME";
        internal const string REQUEST_DEPARTMENT_NAME = "REQUEST_DEPARTMENT_NAME";

        internal const string RIGHT_ROUTE_TYPE_NAME_CC = "RIGHT_ROUTE_TYPE_NAME_CC";
        internal const string RIGHT_ROUTE_TYPE_NAME = "RIGHT_ROUTE_TYPE_NAME";
        internal const string NOT_RIGHT_ROUTE_TYPE_NAME = "NOT_RIGHT_ROUTE_TYPE_NAME";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";
        internal const string IS_NOT_HEIN = "IS_NOT_HEIN";
        internal const string HEIN_MEDI_ORG_CODE = "HEIN_MEDI_ORG_CODE";
        internal const string STR_HEIN_CARD_FROM_TIME = "STR_HEIN_CARD_FROM_TIME";

        internal const string TREATMENT_RESULT_NAME = "TREATMENT_RESULT_NAME";
        internal const string TREATMENT_RESULT_CODE = "TREATMENT_RESULT_CODE";
    }
}
