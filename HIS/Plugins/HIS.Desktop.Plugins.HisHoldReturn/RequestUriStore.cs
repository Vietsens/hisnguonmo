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

namespace HIS.Desktop.Plugins.HisHoldReturn
{
    class RequestUriStore
    {
        internal const string HIS_HORE_DHTY_GET = "api/HisHoreDhty/Get";
        internal const string HIS_TREATMENT_GET = "api/HisTreatment/Get";
        internal const string HIS_TREATMENT_GETView = "api/HisTreatment/GetView";
        internal const string HIS_HOLD_RETURN_GET = "api/HisHoldReturn/Get";
        internal const string HIS_HOLD_RETURN_GETVIEW = "api/HisHoldReturn/GetView";
        internal const string HIS_HOLD_RETURN_CREATE = "api/HisHoldReturn/Create";
        internal const string HIS_HOLD_RETURN_CREATESDO = "api/HisHoldReturn/CreateSdo";
        internal const string HIS_HOLD_RETURN_UPDATESDO = "api/HisHoldReturn/UpdateSdo";
        internal const string HIS_HOLD_RETURN_UPDATE = "api/HisHoldReturn/Update";
        internal const string HIS_HOLD_RETURN_DELETE = "api/HisHoldReturn/Delete";
        internal const string HIS_PATIENT_GETSPREVIOUSWARNING = "api/HisPatient/GetPreviousWarning";
        internal const string HIS_PATIENT_GETSDOADVANCE = "api/HisPatient/GetSdoAdvance";
    }
}
