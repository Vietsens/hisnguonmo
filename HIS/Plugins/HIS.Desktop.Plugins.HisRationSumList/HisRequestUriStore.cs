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

namespace HIS.Desktop.Plugins.HisRationSumList
{
    class HisRequestUriStore
    {
        internal const string Ration_Sum_GetView = "api/HisRationSum/GetView";
        internal const string Ration_Sum_Delete = "api/HisRationSum/Delete";
        internal const string Ration_Sum_Approve = "api/HisRationSum/Approve";
        internal const string Ration_Sum_Unapprove = "api/HisRationSum/Unapprove";
        internal const string Ration_Sum_Reject = "api/HisRationSum/Reject";
        internal const string Ration_Sum_Unreject = "api/HisRationSum/Unreject";

    }
}
