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

namespace HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_HIS_EXP_MEST_UPDATE_TEST_INFO = "api/HisExpMest/UpdateTestInfo";
        internal const string MOSHIS_HIS_EXP_MEST_BLOOD_GETVIEW = "api/HisExpMestBlood/GetView";
        internal const string MOSHIS_HIS_EXP_MEST_BLOOD_GET = "api/HisExpMestBlood/Get";
        internal const string MOSHIS_HIS_BLOOD_ABO_GET = "api/HisBloodAbo/Get";
        internal const string MOSHIS_HIS_BLOOD_RH_GET = "api/HisBloodRh/Get";
        internal const string MOSHIS_HIS_PATIENT_UPDATE = "api/HisPatient/Update";
    
    }
}
