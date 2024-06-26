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

namespace MPS.Processor.Mps000427
{
    class Mps000427ExtendSingleKey : CommonKey
    {
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

        internal const string BloodPressure1 = "BloodPressure1";
        internal const string BloodPressure2 = "BloodPressure2";
        internal const string BloodPressure3 = "BloodPressure3";
        internal const string BloodPressure4 = "BloodPressure4";
        internal const string BloodPressure5 = "BloodPressure5";
        internal const string BloodPressure6 = "BloodPressure6";
        internal const string BloodPressure7 = "BloodPressure7";
        internal const string BloodPressure8 = "BloodPressure8";
        internal const string BloodPressure9 = "BloodPressure9";
        internal const string BloodPressure10 = "BloodPressure10";
        internal const string BloodPressure11 = "BloodPressure11";
        internal const string BloodPressure12 = "BloodPressure12";

        internal const string SPO21 = "SPO21";
        internal const string SPO22 = "SPO22";
        internal const string SPO23 = "SPO23";
        internal const string SPO24 = "SPO24";
        internal const string SPO25 = "SPO25";
        internal const string SPO26 = "SPO26";
        internal const string SPO27 = "SPO27";
        internal const string SPO28 = "SPO28";
        internal const string SPO29 = "SPO29";
        internal const string SPO210 = "SPO210";
        internal const string SPO211 = "SPO211";
        internal const string SPO212 = "SPO212";

        internal const string THO1 = "THO1";
        internal const string THO2 = "THO2";
        internal const string THO3 = "THO3";
        internal const string THO4 = "THO4";
        internal const string THO5 = "THO5";
        internal const string THO6 = "THO6";
        internal const string THO7 = "THO7";
        internal const string THO8 = "THO8";
        internal const string THO9 = "THO9";
        internal const string THO10 = "THO10";
        internal const string THO11 = "THO11";
        internal const string THO12 = "THO12";

        internal const string DT1 = "DT1";
        internal const string DT2 = "DT2";
        internal const string DT3 = "DT3";
        internal const string DT4 = "DT4";
        internal const string DT5 = "DT5";
        internal const string DT6 = "DT6";
        internal const string DT7 = "DT7";
        internal const string DT8 = "DT8";
        internal const string DT9 = "DT9";
        internal const string DT10 = "DT10";
        internal const string DT11 = "DT11";
        internal const string DT12 = "DT12";

        internal const string EAT1 = "EAT1";
        internal const string EAT2 = "EAT2";
        internal const string EAT3 = "EAT3";
        internal const string EAT4 = "EAT4";
        internal const string EAT5 = "EAT5";
        internal const string EAT6 = "EAT6";
        internal const string EAT7 = "EAT7";
        internal const string EAT8 = "EAT8";
        internal const string EAT9 = "EAT9";
        internal const string EAT10 = "EAT10";
        internal const string EAT11 = "EAT11";
        internal const string EAT12 = "EAT12";

        internal const string OTHER1 = "OTHER1";
        internal const string OTHER2 = "OTHER2";
        internal const string OTHER3 = "OTHER3";
        internal const string OTHER4 = "OTHER4";
        internal const string OTHER5 = "OTHER5";
        internal const string OTHER6 = "OTHER6";
        internal const string OTHER7 = "OTHER7";
        internal const string OTHER8 = "OTHER8";
        internal const string OTHER9 = "OTHER9";
        internal const string OTHER10 = "OTHER10";
        internal const string OTHER11 = "OTHER11";
        internal const string OTHER12 = "OTHER12";

        internal const string NT1 = "NT1";
        internal const string NT2 = "NT2";
        internal const string NT3 = "NT3";
        internal const string NT4 = "NT4";
        internal const string NT5 = "NT5";
        internal const string NT6 = "NT6";
        internal const string NT7 = "NT7";
        internal const string NT8 = "NT8";
        internal const string NT9 = "NT9";
        internal const string NT10 = "NT10";
        internal const string NT11 = "NT11";
        internal const string NT12 = "NT12";

        internal const string DL1 = "DL1";
        internal const string DL2 = "DL2";
        internal const string DL3 = "DL3";
        internal const string DL4 = "DL4";
        internal const string DL5 = "DL5";
        internal const string DL6 = "DL6";
        internal const string DL7 = "DL7";
        internal const string DL8 = "DL8";
        internal const string DL9 = "DL9";
        internal const string DL10 = "DL10";
        internal const string DL11 = "DL11";
        internal const string DL12 = "DL12";

        internal const string Feces1 = "Feces1";
        internal const string Feces2 = "Feces2";
        internal const string Feces3 = "Feces3";
        internal const string Feces4 = "Feces4";
        internal const string Feces5 = "Feces5";
        internal const string Feces6 = "Feces6";
        internal const string Feces7 = "Feces7";
        internal const string Feces8 = "Feces8";
        internal const string Feces9 = "Feces9";
        internal const string Feces10 = "Feces10";
        internal const string Feces11 = "Feces11";
        internal const string Feces12 = "Feces12";

        internal const string ROOM_CODE = "RoomCode";
        internal const string BED_CODE = "BedCode";
        internal const string CHARTTem = "CHARTTem";
        internal const string CHARTPUL = "CHARTPul";
        internal const string CHARTPUL_AND_CHARTTem = "CHARTPUL_AND_CHARTTem";


    }
}
