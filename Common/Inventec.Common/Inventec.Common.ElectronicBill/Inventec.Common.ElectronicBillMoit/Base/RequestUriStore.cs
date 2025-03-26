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

namespace Inventec.Common.ElectronicBillMoit.Base
{
    class RequestUriStore
    {
        internal const string CreateInvoice = "/api/hoadon/xuathoadon";

        internal const string CancelInvoice = "/api/business/cancelinv";

        internal const string GetFileInvoice = "/api/hoadon/chuyendoihoadon";

        internal const string CreateInvoiceTT78 = "/api/tt78/hoadon/xuathoadon";

        internal const string DeleteInvoiceTT78 = "/api/tt78/business/deleteinv";

        internal const string GetFileInvoiceTT78 = "/api/tt78/business/invoicebykey";
    }
}
