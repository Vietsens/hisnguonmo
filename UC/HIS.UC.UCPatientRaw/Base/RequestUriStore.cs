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

namespace HIS.UC.UCPatientRaw.Base
{
    class RequestUriStore
    {
        public const string HIS_PATIENT_GETSDOADVANCE = "api/HisPatient/GetSdoAdvance";
        public const string HIS_PATIENT_GETSPREVIOUSWARNING = "api/HisPatient/GetPreviousWarning";
        public const string HIS_CARD_GETVIEWBYSERVICECODE = "api/HisCard/GetCardSdoByCode";
        public const string HIS_PATIEN_PROGRAM_GET = "/api/HisPatientProgram/GetViewByCode";
        public const string HIS_PATIEN_PROGRAM_GET_VIEW = "/api/HisPatientProgram/GetView";
        public const string HIS_CARD_GETVIEWBYMSCODE = "api/HisCard/GetCardSdoByMsCode";
        //public const string HIS_Treatment_STORE_CODE = "api/HisCard/GetCardSdoByMsCode";
    }
}
