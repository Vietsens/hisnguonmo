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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.CYBERBILL.Model
{
    public class LoginResultCyberbill
    {
        public ResultCyberbill result { get; set; }
        public string targetUrl { get; set; }
        public bool success { get; set; }
        public ErrorCyberbill error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
    }
    public class ResultCyberbill
    {
        public string access_token { get; set; }
        public string encrypted_access_token { get; set; }
        public int expire_in_seconds { get; set; }
        public string token_type { get; set; }
    }
    public class ErrorCyberbill
    {
        public int code { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public string validationErrors { get; set; }
    }
}
