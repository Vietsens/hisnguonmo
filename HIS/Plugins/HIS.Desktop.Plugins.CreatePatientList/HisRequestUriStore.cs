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

namespace HIS.Desktop.Plugins.CreatePatientList
{
    class HisRequestUriStore
    {
        public const string HIS_PATIENT_CREATE = "api/HisPatient/Create";
        public const string HIS_PATIENT_GETVIEW = "api/HisPatient/GetView";
        public const string HIS_MILITARY_RANK_GET = "api/HisMilitaryRank/Get";
        public const string HIS_CAREER_GET = "api/HisCareer/Get";
        public const string HIS_BLOOD_ABO__GET = "api/HisBloodAbo/Get";
        public const string HIS_BLOOD_RH__GET = "api/HisBloodRh/Get";
        public const string HIS_GENDER_GET = "api/HisGender/Get";
    }
}
