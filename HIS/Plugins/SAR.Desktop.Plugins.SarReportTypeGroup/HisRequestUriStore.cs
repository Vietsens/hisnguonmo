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

namespace SAR.Desktop.Plugins.SarReportTypeGroup
{
    class HisRequestUriStore
    {
        internal const string SARSAR_REPORT_TYPE_GROUP_CREATE = "api/SarReportTypeGroup/Create";
        internal const string SARSAR_REPORT_TYPE_GROUP_DELETE = "api/SarReportTypeGroup/Delete";
        internal const string SARSAR_REPORT_TYPE_GROUP_UPDATE = "api/SarReportTypeGroup/Update";
        internal const string SARSAR_REPORT_TYPE_GROUP_GET = "api/SarReportTypeGroup/Get";
        internal const string SARSAR_REPORT_TYPE_GROUP_CHANGE_LOCK = "api/SarReportTypeGroup/ChangeLock";
    }
}
