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
    [DataContract]
    public class ResultSaleDCO
    {
        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string AuditKey { get; set; }

        [DataMember]
        public string CdaTraceCode { get; set; }

        [DataMember]
        public decimal Fee { get; set; }

        [DataMember]
        public string PgwResultCode { get; set; }

        [DataMember]
        public string ResultCode { get; set; }

        [DataMember]
        public decimal SendAccountBalance { get; set; }

        [DataMember]
        public string ServiceCode { get; set; }
    }
}
