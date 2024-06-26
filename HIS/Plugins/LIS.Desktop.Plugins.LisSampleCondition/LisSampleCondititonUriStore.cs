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

namespace LIS.Desktop.Plugins.LisSampleCondition
{
    class LisSampleCondititonUriStore
    {
        internal const string SAMPLE_CONDITION_CREATE = "api/LisSampleCondition/Create";
        internal const string SAMPLE_CONDITION_DELETE = "api/LisSampleCondition/Delete";
        internal const string SAMPLE_CONDITION_UPDATE = "api/LisSampleCondition/Update";
        internal const string SAMPLE_CONDITION_GET = "api/LisSampleCondition/Get";
        internal const string SAMPLE_CONDITION_CHANGE_LOCK = "api/LisSampleCondition/ChangeLock";
    }
}
