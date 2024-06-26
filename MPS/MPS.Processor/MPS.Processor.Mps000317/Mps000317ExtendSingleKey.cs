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

namespace MPS.Processor.Mps000317
{
    class Mps000317ExtendSingleKey : CommonKey
    {
        internal static string AMOUNT_TEXT_UPPER_FIRST = "AMOUNT_TEXT_UPPER_FIRST";
        internal const string AMOUNT_AFTER_EXEMPTION = "AMOUNT_AFTER_EXEMPTION";
        internal const string AMOUNT_AFTER_EXEMPTION_TEXT = "AMOUNT_AFTER_EXEMPTION_TEXT";
        internal const string AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST = "AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST";
        internal static string TRANSACTION_CODE_BAR = "TRANSACTION_CODE_BAR";
        internal static string AMOUNT_AWAY_ZERO_TEXT_UPPER_FIRST = "AMOUNT_AWAY_ZERO_TEXT_UPPER_FIRST";
        internal const string CT_AMOUNT = "CT_AMOUNT";
        internal const string CT_AMOUNT_TEXT_UPPER_FIRST = "CT_AMOUNT_TEXT_UPPER_FIRST";
    }
}
