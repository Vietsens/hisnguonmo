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

namespace Inventec.Common.ToKhaiYTe
{
    public class TkytUserADO
    {
        public string userId { get; set; }
        public string fullName { get; set; }
        public long yearOfBirthday { get; set; }
        public string gender { get; set; }
        public string countryId { get; set; }
        public string provinceId { get; set; }
        public string districtId { get; set; }
        public string townId { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string town { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string healthInsuranceNumber { get; set; }
    }

    public class TkytListKhaiBaoADO
    {
        public int total { get; set; }
        public int remain { get; set; }
        public List<object> items { get; set; }
        public TkytUserADO User { get; set; }
    }

    public class TkytRegisterADO
    {
        public string phoneNumber { get; set; }
    }

    public class TkytUserTokenADO
    {
        public string token { get; set; }
        public string userId { get; set; }
        public string qrcode { get; set; }
        public string declaration { get; set; }
    }

    public class TkytRegisterResponseADO
    {
        public TkytUserTokenADO User { get; set; }
    }
}
