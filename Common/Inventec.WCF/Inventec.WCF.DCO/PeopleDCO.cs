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
