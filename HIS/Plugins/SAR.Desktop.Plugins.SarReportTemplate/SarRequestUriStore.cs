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

namespace SAR.Desktop.Plugins.SarReportTemplate
{
    class SarRequestUriStore 
    {
        internal const string SAR_REPORT_TEMPLATE_CREATE = "api/SarReportTemplate/Create";
        internal const string SAR_REPORT_TEMPLATE_DELETE = "api/SarReportTemplate/Delete";
        internal const string SAR_REPORT_TEMPLATE_UPDATE = "api/SarReportTemplate/Update";
        internal const string SAR_REPORT_TEMPLATE_GET = "api/SarReportTemplate/Get";
        internal const string SAR_REPORT_TEMPLATE_CHANGELOCK = "api/SarReportTemplate/ChangeLock";
        internal const string SAR_REPORT_TEMPLATE_GETVIEW= "api/SarReportTemplate/GetView";
    }
}
