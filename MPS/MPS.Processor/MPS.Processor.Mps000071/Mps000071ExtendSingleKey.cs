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

namespace MPS.Processor.Mps000071
{
    public class Mps000071ExtendSingleKey : CommonKey
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

        internal const string START_TIME_STR = "START_TIME_STR";
        internal const string FINISH_TIME_STR = "FINISH_TIME_STR";
        internal const string INTRUCTION_TIME_FULL_STR = "INTRUCTION_TIME_FULL_STR";
        internal const string INTRUCTION_DATE_FULL_STR = "INTRUCTION_DATE_FULL_STR";
        internal const string FINISH_TIME_FULL_STR = "FINISH_TIME_FULL_STR";
        internal const string FINISH_DATE_FULL_STR = "FINISH_DATE_FULL_STR";

        internal const string SERVICE_REQ_CODE_BAR = "SERVICE_REQ_CODE_BAR";

        internal const string RATIO = "RATIO";
        internal const string RATIO_STR = "RATIO_STR";

        internal const string IS_HEIN = "IS_HEIN";
        internal const string IS_NOT_HEIN = "IS_NOT_HEIN";
        internal const string HEIN_CARD_NUMBER_SEPARATE = "HEIN_CARD_NUMBER_SEPARATE";
        internal const string RIGHT_ROUTE_TYPE_NAME_CC = "RIGHT_ROUTE_TYPE_NAME_CC";
        internal const string RIGHT_ROUTE_TYPE_NAME = "RIGHT_ROUTE_TYPE_NAME";
        internal const string NOT_RIGHT_ROUTE_TYPE_NAME = "NOT_RIGHT_ROUTE_TYPE_NAME";
        internal const string HEIN_CARD_NUMBER_1 = "HEIN_CARD_NUMBER_1";
        internal const string HEIN_CARD_NUMBER_2 = "HEIN_CARD_NUMBER_2";
        internal const string HEIN_CARD_NUMBER_3 = "HEIN_CARD_NUMBER_3";
        internal const string HEIN_CARD_NUMBER_4 = "HEIN_CARD_NUMBER_4";
        internal const string HEIN_CARD_NUMBER_5 = "HEIN_CARD_NUMBER_5";
        internal const string HEIN_CARD_NUMBER_6 = "HEIN_CARD_NUMBER_6";
        internal const string STR_HEIN_CARD_FROM_TIME = "STR_HEIN_CARD_FROM_TIME";
        internal const string STR_HEIN_CARD_TO_TIME = "STR_HEIN_CARD_TO_TIME";
        internal const string IS_VIENPHI = "IS_VIENPHI";
        internal const string NUM_ORDER = "NUM_ORDER";
        internal const string HEIN_ADDRESS = "HEIN_ADDRESS";

        internal const string FIRST_EXAM_ROOM_NAME = "FIRST_EXAM_ROOM_NAME";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";

        internal const string ADDRESS = "ADDRESS";
        internal const string CAREER_NAME = "CAREER_NAME";
        internal const string DISTRICT_CODE = "DISTRICT_CODE";
        internal const string PATIENT_CODE = "PATIENT_CODE";
        internal const string GENDER_NAME = "GENDER_NAME";
        internal const string MILITARY_RANK_NAME = "MILITARY_RANK_NAME";
        internal const string VIR_ADDRESS = "VIR_ADDRESS";
        internal const string NATIONAL_NAME = "NATIONAL_NAME";
        internal const string REQ_ICD_CODE = "REQ_ICD_CODE";
        internal const string REQ_ICD_MAIN_TEXT = "REQ_ICD_MAIN_TEXT";
        internal const string REQ_ICD_NAME = "REQ_ICD_NAME";
        internal const string REQ_ICD_SUB_CODE = "REQ_ICD_SUB_CODE";
        internal const string REQ_ICD_TEXT = "REQ_ICD_TEXT";
        internal const string PREV_REQ_ICD_CODE = "PREV_REQ_ICD_CODE";
        internal const string PREV_REQ_ICD_MAIN_TEXT = "PREV_REQ_ICD_MAIN_TEXT";
        internal const string PREV_REQ_ICD_NAME = "PREV_REQ_ICD_NAME";
        internal const string PREV_REQ_ICD_SUB_CODE = "PREV_REQ_ICD_SUB_CODE";
        internal const string PREV_REQ_ICD_TEXT = "PREV_REQ_ICD_TEXT";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string DOB = "DOB";
        internal const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        internal const string WORK_PLACE_NAME = "WORK_PLACE_NAME";
        internal const string QRCODE_PATIENT = "QRCODE_PATIENT";
        internal const string REQ_PROVISIONAL_DIAGNOSIS = "REQ_PROVISIONAL_DIAGNOSIS";

        internal const string CARD_CODE = "CARD_CODE";
        internal const string BANK_CARD_CODE = "BANK_CARD_CODE";
        internal const string PAYMENT_AMOUNT = "PAYMENT_AMOUNT";
    }
}
