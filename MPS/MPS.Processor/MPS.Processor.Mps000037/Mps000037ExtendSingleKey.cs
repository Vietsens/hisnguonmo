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
namespace MPS.Processor.Mps000037
{
    class Mps000037ExtendSingleKey : CommonKey
    {
        internal const string ADDRESS = "ADDRESS";
        internal const string AGE = "AGE";
        internal const string BARCODE_PATIENT_CODE_STR = "PATIENT_CODE_BAR";
        internal const string BARCODE_TREATMENT_CODE_STR = "TREATMENT_CODE_BAR";
        internal const string BED_ROOM_NAME = "BED_ROOM_NAME";
        internal const string CAREER_NAME = "CAREER_NAME";
        internal const string D_O_B = "DOB_YEAR";
        internal const string DEPARTMENT_NAME = "DEPARTMENT_NAME";
        internal const string DISTRICT_CODE = "DISTRICT_CODE";
        internal const string DOB = "DOB";
        internal const string DOB_STR = "DOB_STR";
        internal const string FINISH_DATE_FULL_STR = "FINISH_DATE_FULL_STR";
        internal const string FINISH_TIME_FULL_STR = "FINISH_TIME_FULL_STR";
        internal const string FINISH_TIME_STR = "FINISH_TIME_STR";
        internal const string FIRST_EXAM_ROOM_NAME = "FIRST_EXAM_ROOM_NAME";
        //internal const string GENDER_FEMALE = "GENDER_FEMALE";
        //internal const string GENDER_MALE = "GENDER_MALE";
        internal const string GENDER_NAME = "GENDER_NAME";
        internal const string HEIN_ADDRESS = "HEIN_ADDRESS";
        internal const string HEIN_CARD_NUMBER_1 = "HEIN_CARD_NUMBER_1";
        internal const string HEIN_CARD_NUMBER_2 = "HEIN_CARD_NUMBER_2";
        internal const string HEIN_CARD_NUMBER_3 = "HEIN_CARD_NUMBER_3";
        internal const string HEIN_CARD_NUMBER_4 = "HEIN_CARD_NUMBER_4";
        internal const string HEIN_CARD_NUMBER_5 = "HEIN_CARD_NUMBER_5";
        internal const string HEIN_CARD_NUMBER_6 = "HEIN_CARD_NUMBER_6";
        internal const string HEIN_CARD_NUMBER_SEPARATE = "HEIN_CARD_NUMBER_SEPARATE";
        internal const string INSTRUCTION_TIME_STR = "INSTRUCTION_TIME_STR";
        internal const string INTRUCTION_DATE_FULL_STR = "INTRUCTION_DATE_FULL_STR";
        internal const string INTRUCTION_TIME_FULL_STR = "INTRUCTION_TIME_FULL_STR";
        internal const string IS_HEIN = "IS_HEIN";
        internal const string IS_NOT_HEIN = "IS_NOT_HEIN";
        internal const string IS_VIENPHI = "IS_VIENPHI";
        internal const string MILITARY_RANK_NAME = "MILITARY_RANK_NAME";
        internal const string NATIONAL_NAME = "NATIONAL_NAME";
        internal const string NOT_RIGHT_ROUTE_TYPE_NAME = "NOT_RIGHT_ROUTE_TYPE_NAME";
        internal const string PATIENT_CODE = "PATIENT_CODE";
        internal const string PATIENT_CODE_BAR = "PATIENT_CODE_BAR";
        internal const string RATIO = "RATIO";
        internal const string RATIO_STR = "RATIO_STR";
        internal const string REQ_ICD_CODE = "REQ_ICD_CODE";
        internal const string REQ_ICD_MAIN_TEXT = "REQ_ICD_MAIN_TEXT";
        internal const string REQ_ICD_NAME = "REQ_ICD_NAME";
        internal const string REQ_ICD_SUB_CODE = "REQ_ICD_SUB_CODE";
        internal const string REQ_ICD_TEXT = "REQ_ICD_TEXT";
        internal const string RIGHT_ROUTE_TYPE_NAME = "RIGHT_ROUTE_TYPE_NAME";
        internal const string RIGHT_ROUTE_TYPE_NAME_CC = "RIGHT_ROUTE_TYPE_NAME_CC";
        internal const string SERVICE_REQ_CODE_BAR = "SERVICE_REQ_CODE_BAR";
        internal const string START_TIME_STR = "START_TIME_STR";
        internal const string STR_DOB = "STR_DOB";
        internal const string STR_HEIN_CARD_FROM_TIME = "STR_HEIN_CARD_FROM_TIME";
        internal const string STR_HEIN_CARD_TO_TIME = "STR_HEIN_CARD_TO_TIME";
        internal const string STR_YEAR = "STR_YEAR";
        internal const string TOTAL_PRICE = "TOTAL_PRICE";
        internal const string TOTAL_PRICE_HEIN = "TOTAL_PRICE_HEIN";
        internal const string TOTAL_PRICE_HEIN_TEXT = "TOTAL_PRICE_HEIN_TEXT";
        internal const string TOTAL_PRICE_PATIENT = "TOTAL_PRICE_PATIENT";
        internal const string TOTAL_PRICE_OTHER = "TOTAL_PRICE_OTHER";
        internal const string TOTAL_PRICE_PATIENT_TEXT = "TOTAL_PRICE_PATIENT_TEXT";
        internal const string TOTAL_PRICE_TEXT = "TOTAL_PRICE_TEXT";
        internal const string TREATMENT_CODE_BAR = "TREATMENT_CODE_BAR";
        internal const string VIR_ADDRESS = "VIR_ADDRESS";
        internal const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string WORK_PLACE_NAME = "WORK_PLACE_NAME";
        internal const string OPEN_TIME_SEPARATE_STR = "OPEN_TIME_SEPARATE_STR";

        internal const string BARCODE_SERVICE_REQ_CODE = "BARCODE_SERVICE_REQ_CODE";
        internal const string QRCODE_PATIENT = "QRCODE_PATIENT";
        internal const string ESTIMATE_TIME_ORDER = "ESTIMATE_TIME_ORDER";
        internal const string REQ_PROVISIONAL_DIAGNOSIS = "REQ_PROVISIONAL_DIAGNOSIS";
        internal const string MIN_ASSIGN_TURN_CODE = "MIN_ASSIGN_TURN_CODE";
    }
}
