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

namespace MPS.Processor.Mps000106
{
    class Mps000106ExtendSingleKey : CommonKey
    {
        internal const string DOB_STR = "DOB_STR";
        internal const string YEAR_STR = "YEAR_STR";
        internal const string AGE_STR = "AGE_STR";
        internal const string DESCRIPTION = "DESCRIPTION";
        internal const string AMOUNT = "AMOUNT";
        internal const string AMOUNT_TEXT = "AMOUNT_TEXT";
        internal const string AMOUNT_TEXT_UPPER_FIRST = "AMOUNT_TEXT_UPPER_FIRST";
        internal const string AMOUNT_AFTER_EXEMPTION = "AMOUNT_AFTER_EXEMPTION";
        internal const string AMOUNT_AFTER_EXEMPTION_TEXT = "AMOUNT_AFTER_EXEMPTION_TEXT";
        internal const string AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST = "AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST";
        internal const string EXEMPTION_RATIO = "EXEMPTION_RATIO";
        internal const string CREATE_DATE_SEPARATE_STR = "CREATE_DATE_SEPARATE_STR";
        internal const string EXEMPTION_REASON = "EXEMPTION_REASON";
        internal const string KC_AMOUNT_TEXT_UPPER_FIRST = "KC_AMOUNT_TEXT_UPPER_FIRST";
        internal const string CT_AMOUNT = "CT_AMOUNT";
        internal const string CT_AMOUNT_TEXT_UPPER_FIRST = "CT_AMOUNT_TEXT_UPPER_FIRST";
        internal const string TU_AMOUNT = "TU_AMOUNT";
        internal const string TU_AMOUNT_TEXT_UPPER_FIRST = "TU_AMOUNT_TEXT_UPPER_FIRST";
        internal const string RATIO_TEXT = "RATIO_TEXT";
        internal const string SO_NGAY_DIEU_TRI = "SO_NGAY_DIEU_TRI";
        internal const string TREAT_DEPARTMENT_NAME = "TREAT_DEPARTMENT_NAME";

        internal const string BARCODE_TREATMENT_CODE = "TREATMENT_CODE_BAR";
        internal const string BARCODE_PATIENT_CODE = "PATIENT_CODE_BAR";
        internal const string BARCODE_TRANSACTION_CODE = "TRANSACTION_CODE_BAR";

        internal const string TOTAL_REQUEST_ROOM_NAME = "TOTAL_REQUEST_ROOM_NAME";
        internal const string IN_TIME_FULL_STR = "IN_TIME_FULL_STR";
    }
}
