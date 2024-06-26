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

namespace MPS.Processor.Mps000443
{
   public  class Mps000443ExtendSingleKey : CommonKey
    {
        internal const string TDL_PATIENT_NAME = "TDL_PATIENT_NAME";
        internal const string TDL_PATIENT_CODE = "TDL_PATIENT_CODE";
        internal const string TDL_PATIENT_DOB = "TDL_PATIENT_DOB";
        internal const string TDL_PATIENT_GENDER_NAME = "TDL_PATIENT_GENDER_NAME";
        internal const string TDL_PATIENT_GENDER_CODE = "TDL_PATIENT_GENDER_CODE";
        internal const string TDL_PATIENT_ADDRESS = "TDL_PATIENT_ADDRESS";
        internal const string EXECUTE_ROOM_NAME = "EXECUTE_ROOM_NAME";
        internal const string EXECUTE_ROOM_CODE = "EXECUTE_ROOM_CODE";
        internal const string VACCINATION_EXAM_CODE = "VACCINATION_EXAM_CODE";
        internal const string VACCINATION_EXAM_CODE_BAR = "VACCINATION_EXAM_CODE_BAR";
        internal const string EXECUTE_LOGINNAME = "EXECUTE_LOGINNAME";
        internal const string EXECUTE_USERNAME = "EXECUTE_USERNAME";
        internal const string ADVISE = "ADVISE";
        internal const string NOTE = "NOTE";
        internal const string APPOINTMENT_TIME = "APPOINTMENT_TIME";


        internal const string VACCINATION_EXAM_ID = "VACCINATION_EXAM_ID";

    }
}
