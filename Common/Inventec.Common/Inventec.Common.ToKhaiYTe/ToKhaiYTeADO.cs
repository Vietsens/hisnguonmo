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
