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

namespace MPS.Processor.Mps000111
{
    class Mps000111ExtendSingleKey : CommonKey
    {
        internal const string DOB_STR = "DOB_STR";
        internal const string YEAR_STR = "YEAR_STR";
        internal const string AGE_STR = "AGE_STR";
        internal const string AMOUNT = "AMOUNT";
        internal const string AMOUNT_NUM = "AMOUNT_NUM";
        internal const string DESCRIPTION = "DESCRIPTION";
        internal const string AMOUNT_AFTER_EXEMPTION = "AMOUNT_AFTER_EXEMPTION";
        internal const string AMOUNT_AFTER_EXEMPTION_NUM = "AMOUNT_AFTER_EXEMPTION_NUM";
        internal const string AMOUNT_TEXT = "AMOUNT_TEXT";
        internal const string AMOUNT_TEXT_UPPER_FIRST = "AMOUNT_TEXT_UPPER_FIRST";
        internal const string AMOUNT_AFTER_EXEMPTION_TEXT = "AMOUNT_AFTER_EXEMPTION_TEXT";
        internal const string AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST = "AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST";
        internal const string EXEMPTION_REASON = "EXEMPTION_REASON";
        internal const string PRINT_COUNT = "PRINT_COUNT";
        internal const string EXEMPTION_RATIO = "EXEMPTION_RATIO";
        internal const string CREATE_DATE_SEPARATE_STR = "CREATE_DATE_SEPARATE_STR";
        internal const string KC_AMOUNT_TEXT_UPPER_FIRST = "KC_AMOUNT_TEXT_UPPER_FIRST";
        internal const string CT_AMOUNT = "CT_AMOUNT";
        internal const string CT_AMOUNT_TEXT_UPPER_FIRST = "CT_AMOUNT_TEXT_UPPER_FIRST";

        internal const string TOTAL_AMOUNT_BHYT = "TOTAL_AMOUNT_BHYT";
        internal const string TOTAL_AMOUNT_ND = "TOTAL_AMOUNT_ND";
        internal const string TOTAL_AMOUNT_BHYT_UPPER_TEXT = "TOTAL_AMOUNT_BHYT_UPPER_TEXT";
        internal const string TOTAL_AMOUNT_ND_UPPER_TEXT = "TOTAL_AMOUNT_ND_UPPER_TEXT";

        internal const string HEIN_RATIO_100 = "HEIN_RATIO_100";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";
        internal const string KEY_THU_PHI = "KEY_THU_PHI";
        internal const string TOTAL_PRICE = "TOTAL_PRICE";
        internal const string EXAM_EXECUTE_ROOM_NAME = "EXAM_EXECUTE_ROOM_NAME";
        internal const string EXAM_EXECUTE_ROOM_CODE = "EXAM_EXECUTE_ROOM_CODE";
        internal const string EXAM_SERVICE_DESCRIPTION = "EXAM_SERVICE_DESCRIPTION";

        internal const string REQUEST_ROOM_CODE = "REQUEST_ROOM_CODE";
        internal const string REQUEST_ROOM_NAME = "REQUEST_ROOM_NAME";
        internal const string TRANS_DESCRIPTION = "TRANS_DESCRIPTION";

        internal const string TRANS_DESCRIPTION_NO_DEPOSIT = "TRANS_DESCRIPTION_NO_DEPOSIT";

        internal const string TOTAL_REPAY_AMOUNT = "TOTAL_REPAY_AMOUNT";
        internal const string TOTAL_DEPOSIT_AMOUNT = "TOTAL_DEPOSIT_AMOUNT";
        internal const string TOTAL_DEPOSIT_SERVICE_AMOUNT = "TOTAL_DEPOSIT_SERVICE_AMOUNT";

        internal const string TREATMENT_CODE_BAR = "TREATMENT_CODE_BAR";
        internal const string PATIENT_CODE_BAR = "PATIENT_CODE_BAR";

        internal const string REPAY_NUM_ORDER = "REPAY_NUM_ORDER";
    }
}
