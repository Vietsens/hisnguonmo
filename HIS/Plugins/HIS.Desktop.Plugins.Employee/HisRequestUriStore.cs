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

namespace HIS.Desktop.Plugins.Employee
{
    public class HisRequestUriStore
    {
      internal const string HIS_EMPLOYEE_GET_VIEW = "api/HisEmployee/GetView";
      internal const string HIS_EMPLOYEE_DELETE = "api/HisEmployee/Delete";
      internal const string HIS_EMPLOYEE_CREATE = "api/HisEmployee/Create";
      internal const string HIS_EMPLOYEE_UPDATE = "api/HisEmployee/Update";
      internal const string HIS_EMPLOYEE_CHANGELOCK = "api/HisEmployee/ChangeLock";
      internal const string HIS_EMPLOYEE_GET = "api/HisEmployee/Get";
      internal const string ACS_USER_GET = "api/AcsUser/Get";
    }
}
