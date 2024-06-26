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

namespace HIS.Desktop.ApiConsumer
{
    public class SarRequestUriStore
    {
        public const string SAR_REPORT_CREATE = "api/SarReport/Create";
        public const string SAR_REPORT_TYPE_GET = "api/SarReportType/Get";
        public const string SAR_REPORT_TEMPLATE_GET = "api/SarReportTemplate/Get";
        public const string SAR_REPORT_STT_GET = "api/SarReportStt/Get";
        public const string SAR_PRINT_TYPE_GET = "api/SarPrintType/Get";
        public const string SAR_PRINT_GET = "api/SarPrint/Get";
        public const string SAR_PRINT_CREATE = "api/SarPrint/Create";
        public const string SAR_PRINT_UPDATE = "api/SarPrint/Update";
        public const string SAR_FORM_FIELD_GET = "api/SarFormField/Get";
        public const string SAR_RETY_FOFI_GET = "api/SarRetyFofi/Get";
        public const string SAR_RETY_FOFI_GETVIEW = "api/SarRetyFofi/GetView";
    }
}
