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

namespace HIS.Desktop.Plugins.ExecuteRoom.ADO
{
    class CallPatientInfoADO
    {
        public string PatientName { get; set; }
        public string Dob { get; set; }
        public long NumOrder { get; set; }
        public long PatientType { get; set; }
        public long ServiceReqId { get; set; }
        public CallPatientInfoADO() { }
        public CallPatientInfoADO(string PatientName_, string Dob_, long NumOrder_, long PatientType_, long ServiceReqId_)
        {
            this.PatientName = PatientName_;
            this.Dob = Dob_;
            this.NumOrder = NumOrder_;
            this.PatientType = PatientType_;
            this.ServiceReqId = ServiceReqId_;
        }
    }
}
