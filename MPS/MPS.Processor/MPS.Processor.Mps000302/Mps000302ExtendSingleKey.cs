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

namespace MPS.Processor.Mps000302
{
    class Mps000302ExtendSingleKey : CommonKey
    {
        internal const string DOB_STR = "DOB_STR";
        internal const string INTRUCTION_TIME_STR = "INTRUCTION_TIME_STR";
        internal const string HEIN_CARD_NUMBER_SEPERATOR = "HEIN_CARD_NUMBER_SEPERATOR";
        internal const string FROM_DATE_STR = "FROM_DATE_STR";
        internal const string TO_DATE_STR = "TO_DATE_STR";
        internal const string BARCODE_PATIENT_CODE_STR = "PATIENT_CODE_BAR";
        internal const string TREATMENT_CODE_BAR = "TREATMENT_CODE_BAR";
        internal const string PATIENT_NAME = "PATIENT_NAME";
        internal const string TREATMENT_CODE = "TREATMENT_CODE";
        internal const string HEIN_MEDI_ORG_NAME = "HEIN_MEDI_ORG_NAME";
        internal const string HEIN_CARD_NUMBER = "HEIN_CARD_NUMBER";
        internal const string HEIN_CARD_FROM_TIME = "HEIN_CARD_FROM_TIME";
        internal const string HEIN_CARD_TO_TIME = "HEIN_CARD_TO_TIME";
        internal const string IS_NOT_HEIN = "IS_NOT_HEIN";
        internal const string IS_HEIN = "IS_HEIN";
        internal const string OPEN_TIME_SEPARATE_STR = "OPEN_TIME_SEPARATE_STR";
        internal const string CLOSE_TIME_SEPARATE_STR = "CLOSE_TIME_SEPARATE_STR";
        internal const string RIGHT_ROUTE_TYPE_NAME_CC = "RIGHT_ROUTE_TYPE_NAME_CC";
        internal const string RIGHT_ROUTE_TYPE_NAME = "RIGHT_ROUTE_TYPE_NAME";
        internal const string TRAN_PATI_MEDI_ORG_CODE = "TRAN_PATI_MEDI_ORG_CODE";
        internal const string TRAN_PATI_MEDI_ORG_NAME = "TRAN_PATI_MEDI_ORG_NAME";
        internal const string NOT_RIGHT_ROUTE_TYPE_NAME = "NOT_RIGHT_ROUTE_TYPE_NAME";
        internal const string ICD_MAIN_CODE = "ICD_MAIN_CODE";
        internal const string ICD_MAIN_TEXT = "ICD_MAIN_TEXT";
        internal const string TOTAL_PRICE = "TOTAL_PRICE";
        internal const string TOTAL_PRICE_HEIN = "TOTAL_PRICE_HEIN";
        internal const string TOTAL_PRICE_PATIENT = "TOTAL_PRICE_PATIENT";
        internal const string TOTAL_PRICE_OTHER = "TOTAL_PRICE_OTHER";
        internal const string CURRENT_DATE_SEPARATE_FULL_STR = "CURRENT_DATE_SEPARATE_FULL_STR";
        internal const string HEIN_MEDI_ORG_CODE = "HEIN_MEDI_ORG_CODE";
        internal const string CREATOR_USERNAME = "CREATOR_USERNAME";
        internal const string TOTAL_PRICE_TEXT = "TOTAL_PRICE_TEXT";
        internal const string TOTAL_PRICE_HEIN_TEXT = "TOTAL_PRICE_HEIN_TEXT";
        internal const string TOTAL_PRICE_PATIENT_TEXT = "TOTAL_PRICE_PATIENT_TEXT";
        internal const string TOTAL_PRICE_OTHER_TEXT = "TOTAL_PRICE_OTHER_TEXT";
        internal const string TOTAL_DEPOSIT_AMOUNT = "TOTAL_DEPOSIT_AMOUNT";
        internal const string TOTAL_DAY = "TOTAL_DAY";
        internal const string DEPARTMENT_NAME = "DEPARTMENT_NAME";
        internal const string DEPARTMENT_NAME_CLOSE_TREATMENT = "DEPARTMENT_NAME_CLOSE_TREATMENT";
        internal const string ROOM_NAME = "ROOM_NAME";
        internal const string EXECUTE_ROOM_EXAM = "EXECUTE_ROOM_EXAM";
        internal const string FIRST_EXAM_ROOM_NAME = "FIRST_EXAM_ROOM_NAME";
        internal const string LAST_EXAM_ROOM_NAME = "LAST_EXAM_ROOM_NAME";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";
        internal const string USERNAME_RETURN_RESULT = "USERNAME_RETURN_RESULT";
        internal const string CLINICAL_IN_TIME_STR = "CLINICAL_IN_TIME_STR";
        internal const string STATUS_TREATMENT_OUT = "STATUS_TREATMENT_OUT";
        internal const string TREATMENT_FEE_TOTAL_PRICE = "TREATMENT_FEE_TOTAL_PRICE";
        internal const string TREATMENT_FEE_TOTAL_PATIENT_PRICE = "TREATMENT_FEE_TOTAL_PATIENT_PRICE";
        internal const string TREATMENT_FEE_TOTAL_OBTAINED_PRICE = "TREATMENT_FEE_TOTAL_OBTAINED_PRICE";
        internal const string TREATMENT_FEE_TRANSFER = "TREATMENT_FEE_TRANSFER";
        internal const string TOTAL_NUMBER_FILM = "TOTAL_NUMBER_FILM";
        internal const string TOTAL_NUMBER_FILM_STR = "TOTAL_NUMBER_FILM_STR";
        internal const string TOTAL_PRICE_PATIENT_SELF = "TOTAL_PRICE_PATIENT_SELF";
        internal const string TYPE_INDEX = "TYPE_INDEX";
        internal const string TYPE_NAME = "TYPE_NAME";
        internal const string GENDER_NAME = "GENDER_NAME";
        internal const string GENDER_INDEX = "GENDER_INDEX";
        internal const string FREE_CO_PAID_TIME_STR = "FREE_CO_PAID_TIME_STR";
        internal const string THONG_TUYEN = "THONG_TUYEN";
        internal const string TOTAL_PRICE_BHYT = "TOTAL_PRICE_BHYT";
        internal const string STR_HEIN_CARD_FROM_TIME = "STR_HEIN_CARD_FROM_TIME";
        internal const string STR_HEIN_CARD_TO_TIME = "STR_HEIN_CARD_TO_TIME";
        internal const string RATIO_STR = "RATIO_STR";
        internal const string LIVE_AREA_CODE = "LIVE_AREA_CODE";
        internal const string DEPARTMENT_BHYT_CODE = "DEPARTMENT_BHYT_CODE";
        internal const string END_DEPARTMENT_CODE = "END_DEPARTMENT_CODE";
        internal const string END_DEPARTMENT_NAME = "END_DEPARTMENT_NAME";
        internal const string TREATMENT_RESULT_INDEX = "TREATMENT_RESULT_INDEX";
        internal const string JOIN_5_YEAR_TIME_STR = "JOIN_5_YEAR_TIME_STR";
        internal const string IMG_BHYT = "IMG_BHYT";
        internal const string IMG_AVATAR = "IMG_AVATAR";
        internal const string AVT_AND_BHYT_NULL = "AVT_AND_BHYT_NULL";

        internal const string TREATMENT_DAY_COUNT_6556 = "TREATMENT_DAY_COUNT_6556";
        internal const string TOTAL_PRICE_PATIENT_VIR = "TOTAL_PRICE_PATIENT_VIR";
        internal const string TOTAL_PRICE_PATIENT_VIR_TEXT = "TOTAL_PRICE_PATIENT_VIR_TEXT";
        internal const string TRANSACTION_TREATMENT_DEPOSIT_AMOUNT = "TRANSACTION_TREATMENT_DEPOSIT_AMOUNT";
        internal const string TRANSACTION_TREATMENT_REPAY_AMOUNT = "TRANSACTION_TREATMENT_REPAY_AMOUNT";
        internal const string TRANSACTION_TREATMENT_PATIENT_PRICE = "TRANSACTION_TREATMENT_PATIENT_PRICE";
        internal const string TRANSACTION_TREATMENT_BILL_AMOUNT = "TRANSACTION_TREATMENT_BILL_AMOUNT";

        internal const string LAST_TRANSACTION_AMOUNT = "LAST_TRANSACTION_AMOUNT";
    }
}
