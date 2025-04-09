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

namespace MPS.Processor.Mps000085
{
    class Mps000085ExtendSingleKey : CommonKey
    {
        internal static string IMP_TIME_STR = "IMP_TIME_STR";
        internal static string DOCUMENT_DATE_STR = "DOCUMENT_DATE_STR";
        internal static string CREATE_TIME_STR = "CREATE_TIME_STR";
        internal static string CREATE_DATE_STR = "CREATE_DATE_STR";
        internal static string CREATE_DATE_SEPARATE_STR = "CREATE_DATE_SEPARATE_STR";
        internal static string SUM_PRICE = "SUM_PRICE";
        internal static string SUM_PRICE_SEPARATE = "SUM_PRICE_SEPARATE";
        internal static string SUM_PRICE_NO_VAT = "SUM_PRICE_NO_VAT";
        internal static string SUM_PRICE_NO_VAT_SEPARATE = "SUM_PRICE_NO_VAT_SEPARATE";
        internal static string SUM_PRICE_TEXT = "SUM_PRICE_TEXT";
        internal static string SUM_PRICE_TEXT_NO_VAT = "SUM_PRICE_TEXT_NO_VAT";
        internal static string SUM_PRICE_AFTER_DISCOUNT = "SUM_PRICE_AFTER_DISCOUNT";
        internal static string SUM_PRICE_AFTER_DISCOUNT_SEPARATE = "SUM_PRICE_AFTER_DISCOUNT_SEPARATE";
        internal static string SUM_PRICE_AFTER_DISCOUNT_TEXT = "SUM_PRICE_AFTER_DISCOUNT_TEXT";
        internal static string DOCUMENT_PRICE_TEXT = "DOCUMENT_PRICE_TEXT";
        internal static string DOCUMENT_PRICE = "DOCUMENT_PRICE";
        internal static string DOCUMENT_PRICE_SEPARATE = "DOCUMENT_PRICE_SEPARATE";
        internal static string SUPPLIER_NAME_STR = "SUPPLIER_NAME_STR";

        internal static string TOTAL_PRICE = "TOTAL_PRICE";
        internal static string TOTAL_PRICE_SEPARATE = "TOTAL_PRICE_SEPARATE";
        internal static string TOTAL_PRICE_TEXT = "TOTAL_PRICE_TEXT";
    }
}
