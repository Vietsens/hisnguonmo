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

namespace MPS.Processor.Mps000361
{
    public class Mps000361ExtendSingleKey : CommonKey
    {
        internal static string AMOUNT_TEXT_UPPER_FIRST = "AMOUNT_TEXT_UPPER_FIRST";
        internal const string AMOUNT_AFTER_EXEMPTION_TEXT = "AMOUNT_AFTER_EXEMPTION_TEXT";
        internal const string AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST = "AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST";
        internal const string CT_AMOUNT_TEXT_UPPER_FIRST = "CT_AMOUNT_TEXT_UPPER_FIRST";
        internal const string CT_AMOUNT = "CT_AMOUNT";
        internal const string DBO_DATE = "DBO_DATE";
        internal const string TOTAL_PRICE_STR = "TOTAL_PRICE_STR";
        internal const string TOTAL_HEIN_PRICE_STR = "TOTAL_HEIN_PRICE_STR";
        internal const string TOTAL_DEPOSIT_AMOUNT_STR = "TOTAL_DEPOSIT_AMOUNT_STR";
        internal const string TOTAL_PATIENT_PRICE_STR = "TOTAL_PATIENT_PRICE_STR";
        internal const string CURRENT_TIME_SEPARATE_BEGIN_TIME_STR = "CURRENT_TIME_SEPARATE_BEGIN_TIME_STR";
        internal const string CURRENT_MONTH_SEPARATE_STR = "CURRENT_MONTH_SEPARATE_STR";
        internal const string CURRENT_MONTH_STR = "CURRENT_MONTH_STR";
        internal const string CURRENT_TIME_SEPARATE_STR = "CURRENT_TIME_SEPARATE_STR";
        internal const string CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR = "CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR";
        internal const string TOTAL_RETURN_STR = "TOTAL_RETURN_STR";
        internal const string TOTAL_PRICE_STR_1 = "TOTAL_PRICE_STR_1";
        internal const string TOTAL_BILL_AMOUNT_STR = "TOTAL_BILL_AMOUNT_STR";
        internal const string END_DEPARTMENT_NAME = "END_DEPARTMENT_NAME";
        internal const string TOTAL_REPAY_AMOUNT_STR = "TOTAL_REPAY_AMOUNT_STR";
        internal const string TOTAL_HEIN_PRICE_INFO_STR = "TOTAL_HEIN_PRICE_INFO_STR";
        internal const string TOTAL_HEIN_PRICE_INFO = "TOTAL_HEIN_PRICE_INFO";
        internal const string TOTAL_DIRECTLY_BILL_PRICE = "TOTAL_DIRECTLY_BILL_PRICE";
    }
}
