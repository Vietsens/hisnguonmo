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

namespace MPS.Processor.Mps000350
{
    class Mps000350ExtendSingleKey : CommonKey
    {
        internal static string EXP_TIME = "EXP_TIME";
        internal static string APPROVAL_LOGINNAME = "APPROVAL_LOGINNAME";
        internal static string EXP_LOGINNAME = "EXP_LOGINNAME";
        internal static string CREATE_TIME_STR = "CREATE_TIME_STR";
        internal static string CREATE_DATE_STR = "CREATE_DATE_STR";
        internal static string CREATE_DATE_SEPARATE = "CREATE_DATE_SEPARATE";
        internal static string SUM_TOTAL_PRICE = "SUM_TOTAL_PRICE";
        internal static string SUM_TOTAL_PRICE_TEXT = "SUM_TOTAL_PRICE_TEXT";
        internal static string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        internal static string PATIENT_NAME = "PATIENT_NAME";
        internal static string SUM_TOTAL_PRICE_AFTER_DISCOUNT = "SUM_TOTAL_PRICE_AFTER_DISCOUNT";
        internal static string DISCOUNT = "DISCOUNT";
        internal static string SUM_TOTAL_PRICE_AFTER_DISCOUNT_TEXT = "SUM_TOTAL_PRICE_AFTER_DISCOUNT_TEXT";

        internal static string TRANSACTION_CODE_BAR = "TRANSACTION_CODE_BAR";
        internal static string EXP_MEST_CODE_BAR = "EXP_MEST_CODE_BAR";

        internal static string SUM_TOTAL_PRICE_NOT_ROUND = "SUM_TOTAL_PRICE_NOT_ROUND";
        internal static string SUM_TOTAL_PRICE_NOT_ROUND_TEXT = "SUM_TOTAL_PRICE_NOT_ROUND_TEXT";
        internal static string SUM_TOTAL_PRICE_AFTER_DISCOUNT_NOT_ROUND = "SUM_TOTAL_PRICE_AFTER_DISCOUNT_NOT_ROUND";
        internal static string SUM_TOTAL_PRICE_AFTER_DISCOUNT_NOT_ROUND_TEXT = "SUM_TOTAL_PRICE_AFTER_DISCOUNT_NOT_ROUND_TEXT";

        internal static string EXP_MEST_CODES = "EXP_MEST_CODES";

        internal static string SUM_TOTAL_PRICE_NO_TH = "SUM_TOTAL_PRICE_NO_TH";

        internal static string MOBA_IMP_MEST_COUNT = "MOBA_IMP_MEST_COUNT";
        internal static string NUM_ORDER = "NUM_ORDER";
    }
}
