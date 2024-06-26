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

namespace HIS.UC.UCHeniInfo
{
    class RequestUriStore
    {
        public const string HIS_PATIENT_GETSDOADVANCE = "api/HisPatient/GetSdoAdvance";
        public const string HIS_PATIENT_TYPE_ALTER__GET = "/api/HisPatientTypeAlter/Get";
        public const string HIS_PATIENT_TYPE_ALTER__GETVIEW = "/api/HisPatientTypeAlter/GetView";       
        public const string HIS_TRAN_PATI__GETVIEW = "/api/HisTranPati/GetView";
        public const string HIS_TREATMENT__GET = "/api/HisTreatment/Get";
    }
}
