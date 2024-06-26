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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000371.PDO
{
    public class Mps000371PDO : RDOBase
    {
        public List<V_HIS_SERVICE_REQ> _ListServiceReq;
        public List<V_HIS_SERE_SERV_15> _ListSereServ;
        public List<HIS_SERVICE_UNIT> ListServiceUnit;
        public List<HIS_TREATMENT_BED_ROOM> TreatmentBedRoom;
        public List<V_HIS_BED_LOG> HisBedLog;

        public Mps000371PDO(List<V_HIS_SERVICE_REQ> ServiceReq, List<V_HIS_SERE_SERV_15> SereServ, List<HIS_SERVICE_UNIT> listServiceUnit, List<HIS_TREATMENT_BED_ROOM> treatmentBedRoom, List<V_HIS_BED_LOG> hisBedLog)
        {
            this._ListServiceReq = ServiceReq;
            this._ListSereServ = SereServ;
            this.ListServiceUnit = listServiceUnit;
            this.TreatmentBedRoom = treatmentBedRoom;
            this.HisBedLog = hisBedLog;
        }
    }
}
