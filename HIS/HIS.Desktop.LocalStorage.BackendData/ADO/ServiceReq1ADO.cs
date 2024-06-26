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

namespace HIS.Desktop.LocalStorage.BackendData.ADO
{
    public class ServiceReq1ADO : MOS.EFMODEL.DataModels.HIS_SERVICE_REQ
    {
        public bool CallPatientSTT { get; set; }

        public bool IsCalling { get; set; }

        public long? VaccinationId { get; set; }
        public string TDL_PATIENT_NAME_OLD { get; set; }

        public ServiceReq1ADO()
        {

        }
    }
}
