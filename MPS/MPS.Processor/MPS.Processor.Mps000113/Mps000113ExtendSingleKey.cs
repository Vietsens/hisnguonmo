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

namespace MPS.Processor.Mps000113
{
    class Mps000113ExtendSingleKey : CommonKey
    {
        internal const string DOB_STR = "DOB_STR";
        internal const string YEAR_STR = "YEAR_STR";
        internal const string AGE_STR = "AGE_STR";
        internal const string AMOUNT = "AMOUNT";
        internal const string AMOUNT_NUM = "AMOUNT_NUM";
        internal const string AMOUNT_TEXT = "AMOUNT_TEXT";
        internal const string AMOUNT_TEXT_UPPER_FIRST = "AMOUNT_TEXT_UPPER_FIRST";
        internal const string PRINT_COUNT = "PRINT_COUNT";
        internal const string CREATE_DATE_SEPARATE_STR = "CREATE_DATE_SEPARATE_STR";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";
        internal const string RATIO = "RATIO";
        internal const string RATIO_STR = "RATIO_STR";
        internal const string TRANSACTION_CODE_BAR = "TRANSACTION_CODE_BAR";
        internal const string TREATMENT_CODE_BAR = "TREATMENT_CODE_BAR";

        internal const string TOTAL_DIRECTLY_BILL_PRICE = "TOTAL_DIRECTLY_BILL_PRICE";
        internal const string BILL_NUM_ORDER = "BILL_NUM_ORDER";
        internal const string BILL_ACCOUNT_BOOK_CODE = "BILL_ACCOUNT_BOOK_CODE";
        internal const string BILL_ACCOUNT_BOOK_NAME = "BILL_ACCOUNT_BOOK_NAME";
        internal const string BILL_TEMPLATE_CODE = "BILL_TEMPLATE_CODE";
        internal const string BILL_SYMBOL_CODE = "BILL_SYMBOL_CODE";
        internal const string TOTAL_DEPOSIT_NUM_ORDER = "TOTAL_DEPOSIT_NUM_ORDER";
    }
}
