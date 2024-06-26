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

namespace HIS.Desktop.Plugins.AllergyCard
{
    class HisRequestUriStore
    {
        internal const string HIS_TREATMENT_GETVIEW = "api/HisTreatment/GetView";
        internal const string HIS_ALLERGY_CARD_GETVIEW = "api/HisAllergyCard/GetView";
        internal const string HIS_ALLERGEIC_GET = "api/HisAllergenic/Get";
        internal const string HIS_ALLERGEIC_GETVIEW = "api/HisAllergenic/GetView";
        internal const string HIS_PATIENT_GETVIEW = "api/HisPatient/GetView";
    }
}
