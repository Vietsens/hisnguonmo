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

namespace Inventec.Common.WordContent
{
    public class RequestUriStore
    {
      
        //SAR
        public const string SAR_REPORT_TYPE_GET = "api/SarReportType/Get";

        public const string SAR_REPORT_TEMPLATE_GET = "api/SarReportTemplate/Get";

        public const string SAR_REPORT_STT_GET = "api/SarReportStt/Get";


        public const string SAR_PRINT_TYPE_GET = "api/SarPrintType/Get";
        public const string SAR_PRINT_TEMPLATE_GET = "api/SarPrintTemplate/Get";
        public const string SAR_PRINT_TEMPLATE_GETVIEW = "api/SarPrintTemplate/GetView";
        public const string SAR_PRINT_GET = "api/SarPrint/Get";
        public const string SAR_PRINT_CREATE = "api/SarPrint/Create";
        public const string SAR_PRINT_UPDATE = "api/SarPrint/Update";

        public const string SAR_USER_REPORT_TYPE_GET = "api/SarUserReportType/Get";
    }
}
