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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MOBIFONE.Model
{
    public class SignInvoiceCertFile68Init
    {
        public List<SignInvoiceCertFile68Data> data { get; set; }
    }
    public class SignInvoiceCertFile68Data
    {
        public string branch_code { get; set; }
        public string username { get; set; }
        public List<string> lsthdon_id { get; set; }
        public string cer_serial { get; set; }
        public string type_cmd { get; set; }
        public string is_api { get; set; }
    }
    public enum TypeCmd
    {
        HasCode = 200,
        NoCode = 203
    }
}
