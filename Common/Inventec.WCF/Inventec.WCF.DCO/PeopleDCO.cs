using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.WCF.DCO
{
    [DataContract()]
    public class PeopleDCO
    {
        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public long? CmndDate { get; set; }

        [DataMember]
        public string CmndNumber { get; set; }

        [DataMember]
        public string CmndPlace { get; set; }

        [DataMember]
        public string CommuneName { get; set; }

        [DataMember]
        public string DistrictName { get; set; }

        [DataMember]
        public long? Dob { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string EthnicName { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string GenderCode { get; set; }

        [DataMember]
        public string GenderName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string MtcnCode { get; set; }

        [DataMember]
        public string NationalName { get; set; }

        [DataMember]
        public string NickName { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public string PeopleCode { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string ProvinceName { get; set; }

        [DataMember]
        public string ReligionName { get; set; }

        [DataMember]
        public string WorkPlace { get; set; }
    }
}
