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

namespace Inventec.Common.ElectronicBillViettel.Base
{
    class Constants
    {
        internal static int TIME_OUT = 90;
        internal const string ErrorCode = "ERROR_APP";
        internal const string keyHeaderToken = "Cookie";
        internal const string valueHeaderToken = "access_token={0}";
    }

    /// <summary>
    /// Lớp chứa các key định nghĩa phương thức gọi request lên meInvoice Cloud
    /// </summary>
    class HttpMethod
    {
        /// <summary>
        /// Phương thức GET
        /// </summary>
        public const string GET = "GET";

        /// <summary>
        /// Phương thức POST
        /// </summary>
        public const string POST = "POST";

        /// <summary>
        /// Phương thức PUT
        /// </summary>
        public const string PUT = "PUT";

        /// <summary>
        /// Phương thức Xóa
        /// </summary>
        public const string DELETE = "DELETE";
    }
}
