using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EHoaDon
{
    public class BkavPartner
    {
        public string BkavPartnerGUID { get; set; }//-TO EDIT- (2) Do Bkav cấp cho từng đối tác, mỗi đối tác 1 GUID khác nhau
        public string BkavPartnerToken { get; set; } // -TO EDIT- (1) Do Bkav cấp cho từng đối tác, mỗi đối tác 1 mã khác nhau
        public uint Mode { get; set; } // -TO EDIT- (3) Do Bkav cấp cho từng đối tác, mỗi đối tác 1 mã khác nhau
    }
}
