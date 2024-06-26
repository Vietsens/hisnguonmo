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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000415.PDO
{
    public class Mps000415PDO : RDOBase
    {
        public V_HIS_SERVICE_REQ ServiceReq { get; set; }
        public V_HIS_TREATMENT_BED_ROOM BedRoom { get; set; }
        public V_HIS_SERE_SERV_PTTT SereServPttt { get; set; }
        public HIS_SERE_SERV_EXT SereServExt { get; set; }
        public List<HIS_SERE_SERV> ListExpend { get; set; }
        public Mps000415ADO Mps000415ADO { get; set; }
        public List<HIS_SERVICE_UNIT> ListServiceUnit { get; set; }

        public Mps000415PDO(V_HIS_SERVICE_REQ serviceReq,
            V_HIS_TREATMENT_BED_ROOM bed,
            V_HIS_SERE_SERV_PTTT sereServPttt,
            HIS_SERE_SERV_EXT sereServExt,
            List<HIS_SERE_SERV> listExpend,
             Mps000415ADO _Mps000415ADO,
            List<HIS_SERVICE_UNIT> serviceUnit)
        {
            this.ServiceReq = serviceReq;
            this.BedRoom = bed;
            this.SereServPttt = sereServPttt;
            this.SereServExt = sereServExt;
            this.ListExpend = listExpend;
            this.Mps000415ADO = _Mps000415ADO;
            this.ListServiceUnit = serviceUnit;
        }
    }

    public class Mps000415ADO
    {
        public long? BEGIN_TIME { get; set; }
        public long? END_TIME { get; set; }
    }
}
