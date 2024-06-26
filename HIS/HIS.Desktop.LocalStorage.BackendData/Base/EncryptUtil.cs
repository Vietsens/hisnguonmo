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

namespace HIS.Desktop.LocalStorage.BackendData
{
    public class EncryptUtil
    {
        public static string EncodeData(object data)
        {
            string result = "";
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data));
            result = System.Convert.ToBase64String(plainTextBytes);
            return result;
        }

        public static string DecodeData(string data)
        {
            string result = "";
            var base64EncodedBytes = System.Convert.FromBase64String(data);
            result = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            return result;
        }
    }
}
