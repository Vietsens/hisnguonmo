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

namespace HIS.Desktop.ADO
{
    public class AssignPrescriptionEditADO
    {
        public delegate void DelegateRefeshData(object data);

        public MOS.EFMODEL.DataModels.HIS_SERVICE_REQ ServiceReq { get; set; }
        public MOS.EFMODEL.DataModels.HIS_EXP_MEST ExpMest { get; set; }
        public DelegateRefeshData DgRefeshData { get; set; }

        public AssignPrescriptionEditADO(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ _serviceReq, HIS.Desktop.ADO.AssignPrescriptionEditADO.DelegateRefeshData refeshData)
        {
            this.DgRefeshData = refeshData;
            this.ServiceReq = _serviceReq;
        }

        public AssignPrescriptionEditADO(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ _serviceReq, MOS.EFMODEL.DataModels.HIS_EXP_MEST _expMest, HIS.Desktop.ADO.AssignPrescriptionEditADO.DelegateRefeshData refeshData)
        {
            this.DgRefeshData = refeshData;
            this.ServiceReq = _serviceReq;
            this.ExpMest = _expMest;
        }
    }
}
