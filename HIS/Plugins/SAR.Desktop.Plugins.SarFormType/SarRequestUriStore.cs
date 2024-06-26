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

namespace SAR.Desktop.Plugins.SarFormType
{
    public class SarRequestUriStore
    {
      internal const string SAR_FORMTYPE_GET_VIEW = "api/SarFormType/GetView";
      internal const string SAR_FORMTYPE_DELETE = "api/SarFormType/Delete";
      internal const string SAR_FORMTYPE_CREATE = "api/SarFormType/Create";
      internal const string SAR_FORMTYPE_UPDATE = "api/SarFormType/Update";
      internal const string SAR_FORMTYPE_CHANGELOCK = "api/SarFormType/ChangeLock";
      internal const string SAR_FORMTYPE_GET = "api/SarFormType/Get";
      internal const string ACS_USER_GET = "api/AcsUser/Get";
    }
}
