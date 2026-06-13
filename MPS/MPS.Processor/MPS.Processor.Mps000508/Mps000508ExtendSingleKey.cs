using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000508
{
    class Mps000508ExtendSingleKey : CommonKey
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
        internal const string TOTAL_SURCHARGE = "TOTAL_SURCHARGE";
        internal const string TOTAL_SURCHARGE_TEXT = "TOTAL_SURCHARGE_TEXT";
        internal const string SURCHARGE_COUNT = "SURCHARGE_COUNT";
        internal const string SURCHARGE_SECTION_NO = "SURCHARGE_SECTION_NO";
        internal const string SURCHARGE_SECTION_LABEL = "SURCHARGE_SECTION_LABEL";
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
        internal const string TOTAL_PRICE_PATIENT_NO_PAY_RATE = "TOTAL_PRICE_PATIENT_NO_PAY_RATE";
        internal const string TOTAL_PRICE_PATIENT_NO_PAY_RATE_TEXT = "TOTAL_PRICE_PATIENT_NO_PAY_RATE_TEXT";

        internal const string TYPE_INDEX_697 = "TYPE_INDEX_697";

        internal const string HEIN_PATIENT_TYPE_CODE_697 = "HEIN_PATIENT_TYPE_CODE_697";
        internal const string MEDI_ORG_NAME_697 = "MEDI_ORG_NAME_697";
        internal const string TRANSFER_IN_MEDI_ORG_NAME_697 = "TRANSFER_IN_MEDI_ORG_NAME_697";
        internal const string TRANSFER_IN_MEDI_ORG_CODE_697 = "TRANSFER_IN_MEDI_ORG_CODE_697";

        internal const string CO_PAID_ACCUMULATE_AMOUNT_697 = "CO_PAID_ACCUMULATE_AMOUNT_697";
        internal const string CO_PAID_ACCUMULATE_AMOUNT_STR_697 = "CO_PAID_ACCUMULATE_AMOUNT_STR_697";

        // Alias tags without _697 suffix — used by Mps000509 (reuse) and other templates
        internal const string CO_PAID_ACCUMULATE_AMOUNT = "CO_PAID_ACCUMULATE_AMOUNT";
        internal const string CO_PAID_ACCUMULATE_AMOUNT_STR = "CO_PAID_ACCUMULATE_AMOUNT_STR";

        internal const string TREATMENT_RESULT_NAME_697 = "TREATMENT_RESULT_NAME_697";
        internal const string TREATMENT_RESULT_CODE_697 = "TREATMENT_RESULT_CODE_697";

        internal const string TOTAL_PRICE_PATIENT_ALL_697 = "TOTAL_PRICE_PATIENT_ALL_697";
        internal const string TOTAL_PRICE_PATIENT_ALL_TEXT_697 = "TOTAL_PRICE_PATIENT_ALL_TEXT_697";
    }
}
