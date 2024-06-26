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

namespace MPS.Processor.Mps000320
{
    class Mps000320ExtendSingleKey : CommonKey
    {
        internal static string DISPLAY_TIME__FROM_TO = "DISPLAY_TIME__FROM_TO";

        internal static string EXP_TIME = "EXP_TIME";
        internal static string APPROVAL_LOGINNAME = "APPROVAL_LOGINNAME";
        internal static string CREATE_TIME_STR = "CREATE_TIME_STR";
        internal static string CREATE_DATE_STR = "CREATE_DATE_STR";
        internal static string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        internal static string PATIENT_NAME = "PATIENT_NAME";
    }
}
