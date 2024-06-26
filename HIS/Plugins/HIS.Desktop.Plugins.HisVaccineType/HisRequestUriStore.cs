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

namespace LIS.Desktop.Plugins.LisPatientCondition
{
    public class HisRequestUriStore
    {
        
        internal const string LisPatientCondition_Get = "api/LisPatientCondition/Get";
        internal const string LisPatientCondition_GetView = "api/LisPatientCondition/GetView";
        internal const string LisPatientCondition_Create = "api/LisPatientCondition/Create";
        internal const string LisPatientCondition_Delete = "api/LisPatientCondition/Delete";
        internal const string LisPatientCondition_Changelock = "api/LisPatientCondition/Changelock";
        
        internal const string LisPatientCondition_Update = "api/LisPatientCondition/Update";
    }
}
