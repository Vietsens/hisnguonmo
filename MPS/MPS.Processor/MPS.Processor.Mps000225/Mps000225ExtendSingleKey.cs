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

namespace MPS.Processor.Mps000225
{
    class Mps000225ExtendSingleKey : CommonKey
    {
        internal const string DOB_STR = "DOB_STR";
        internal const string INTRUCTION_TIME_STR = "INTRUCTION_TIME_STR";
        internal const string HEIN_CARD_NUMBER_SEPERATOR = "HEIN_CARD_NUMBER_SEPERATOR";
        internal const string REQUEST_DEPARTMENT_NAME = "REQUEST_DEPARTMENT_NAME";
        internal const string REQUEST_BED_ROOM_NAME = "REQUEST_BED_ROOM_NAME";
        internal const string BARCODE_PATIENT_CODE_STR = "BARCODE_PATIENT_CODE";
        internal const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE";
        internal const string CREATE_TIME_TRAN = "CREATE_TIME_TRAN";
        #region ---Day---
        internal const string Day1 = "Day1";
        internal const string Day2 = "Day2";
        internal const string Day3 = "Day3";
        internal const string Day4 = "Day4";
        internal const string Day5 = "Day5";
        internal const string Day6 = "Day6";
        internal const string Day7 = "Day7";
        internal const string Day8 = "Day8";
        internal const string Day9 = "Day9";
        internal const string Day10 = "Day10";
        internal const string Day11 = "Day11";
        internal const string Day12 = "Day12";
        internal const string Day13 = "Day13";
        internal const string Day14 = "Day14";
        internal const string Day15 = "Day15";
        internal const string Day16 = "Day16";
        internal const string Day17 = "Day17";
        internal const string Day18 = "Day18";
        internal const string Day19 = "Day19";
        internal const string Day20 = "Day20";
        internal const string Day21 = "Day21";
        internal const string Day22 = "Day22";
        internal const string Day23 = "Day23";
        internal const string Day24 = "Day24";
        #endregion

        #region ---Day and year---
        internal const string DayAndYear1 = "DayAndYear1";
        internal const string DayAndYear2 = "DayAndYear2";
        internal const string DayAndYear3 = "DayAndYear3";
        internal const string DayAndYear4 = "DayAndYear4";
        internal const string DayAndYear5 = "DayAndYear5";
        internal const string DayAndYear6 = "DayAndYear6";
        internal const string DayAndYear7 = "DayAndYear7";
        internal const string DayAndYear8 = "DayAndYear8";
        internal const string DayAndYear9 = "DayAndYear9";
        internal const string DayAndYear10 = "DayAndYear10";
        internal const string DayAndYear11 = "DayAndYear11";
        internal const string DayAndYear12 = "DayAndYear12";
        internal const string DayAndYear13 = "DayAndYear13";
        internal const string DayAndYear14 = "DayAndYear14";
        internal const string DayAndYear15 = "DayAndYear15";
        internal const string DayAndYear16 = "DayAndYear16";
        internal const string DayAndYear17 = "DayAndYear17";
        internal const string DayAndYear18 = "DayAndYear18";
        internal const string DayAndYear19 = "DayAndYear19";
        internal const string DayAndYear20 = "DayAndYear20";
        internal const string DayAndYear21 = "DayAndYear21";
        internal const string DayAndYear22 = "DayAndYear22";
        internal const string DayAndYear23 = "DayAndYear23";
        internal const string DayAndYear24 = "DayAndYear24";
        #endregion
        internal const string IN_TIME_STR = "IN_TIME_STR";
        internal const string OUT_TIME_STR = "OUT_TIME_STR";
        internal const string CLINICAL_IN_TIME_STR = "CLINICAL_IN_TIME_STR";
        internal const string AGE = "AGE";
    }
}
