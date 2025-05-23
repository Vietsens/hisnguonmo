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

namespace MPS.Processor.Mps000500
{
    class Mps000500ExtendSingleKey : CommonKey
    {
        internal const string TREATMENT_CODE = "TREATMENT_CODE";
        internal const string PATIENT_CODE = "PATIENT_CODE";

        internal const string TDL_PATIENT_NAME = "TDL_PATIENT_NAME";
        internal const string TDL_PATIENT_DOB = "TDL_PATIENT_DOB";

        internal const string TDL_PATIENT_GENDER_NAME = "TDL_PATIENT_GENDER_NAME";
        internal const string TDL_PATIENT_ADDRESS = "TDL_PATIENT_ADDRESS";
        internal const string ICD_CODE = "ICD_CODE";
        internal const string ICD_NAME = "ICD_NAME";
        internal const string ICD_SUB_CODE = "ICD_SUB_CODE";
        internal const string ICD_TEXT = "ICD_TEXT";
        internal const string BED_ROOM_CODE = "BED_ROOM_CODE";
        internal const string BED_ROOM_NAME = "BED_ROOM_NAME";
        internal const string BED_CODE = "BED_CODE";
        internal const string BED_NAME = "BED_NAME";
        internal const string EXAM_EXECUTE_CONTENT = "EXAM_EXECUTE_CONTENT";
        internal const string EXAM_EXCUTE = "EXAM_EXCUTE";

        internal const string EXAM_EXECUTE_DEPARMENT_CODE = "EXAM_EXECUTE_DEPARTMENT_CODE";
        internal const string EXAM_EXECUTE_DEPARMENT_NAME = "EXAM_EXECUTE_DEPARTMENT_NAME";
        internal const string INVITE_DEPARTMENT_CODE = "INVITE_DEPARTMENT_CODE";
        internal const string INVITE_DEPARTMENT_NAME = "INVITE_DEPARTMENT_NAME";


    }
}
