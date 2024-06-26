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

namespace MPS.Processor.Mps000239
{
    class Mps000239ExtendSingleKey : CommonKey
    {
        internal const string CREATE_TIME_STR = "CREATE_TIME_STR";
        internal const string CREATE_DATE_STR = "CREATE_DATE_STR";
        internal const string CREATE_DATE_SEPARATE_STR = "CREATE_DATE_SEPARATE_STR";
        internal const string DAY_USE = "DAY_USE";
        internal const string EXP_MEST_CODE_BARCODE = "EXP_MEST_CODE_BARCODE";
        internal const string KEY_NAME_TITLES = "KEY_NAME_TITLES";

        internal const string MIN_INTRUCTION_TIME_DISPLAY = "MIN_INTRUCTION_TIME_DISPLAY";

        internal const string MIN_INTRUCTION_DATE_DISPLAY = "MIN_INTRUCTION_DATE_DISPLAY";

        internal const string MIN_INTRUCTION_DATE_SEPARATE_DISPLAY = "MIN_INTRUCTION_DATE_SEPARATE_DISPLAY";

        internal const string MAX_INTRUCTION_TIME_DISPLAY = "MAX_INTRUCTION_TIME_DISPLAY";

        internal const string MAX_INTRUCTION_DATE_DISPLAY = "MAX_INTRUCTION_DATE_DISPLAY";

        internal const string MAX_INTRUCTION_DATE_SEPARATE_DISPLAY = "MAX_INTRUCTION_DATE_SEPARATE_DISPLAY";

        internal const string TOTAL_REQ_ROOM_NAME_DISPLAY = "TOTAL_REQ_ROOM_NAME";
    }
}
