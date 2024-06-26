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

namespace MPS.Processor.Mps000155
{
    class Mps000155ExtendSingleKey : CommonKey
    {
        internal static string CREATE_TIME_STR = "CREATE_TIME_STR";
        internal static string CREATE_DATE_STR = "CREATE_DATE_STR";
        internal static string CREATE_DATE_SEPARATE = "CREATE_DATE_SEPARATE";
        internal static string EXP_TIME_STR = "EXP_TIME_STR";
        internal static string EXP_DATE_STR = "EXP_DATE_STR";
        internal static string EXP_DATE_SEPARATE = "EXP_DATE_SEPARATE";
        internal static string SUM_TOTAL_PRICE = "SUM_TOTAL_PRICE";// gia nhap
        internal static string SUM_TOTAL_PRICE_TEXT = "SUM_TOTAL_PRICE_TEXT";// gia nhap
        internal static string SUM_TOTAL_EXP_PRICE = "SUM_TOTAL_EXP_PRICE";// gia xuat
        internal static string SUM_TOTAL_EXP_PRICE_TEXT = "SUM_TOTAL_EXP_PRICE_TEXT";// gia xuat
    }
}
