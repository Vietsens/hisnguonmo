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

namespace MPS.Processor.Mps000037.PDO
{
    public partial class Mps000037PDO : RDOBase
    {
        //public HisServiceReqCombo hisServiceReqCombo { get; set; }
        public List<V_HIS_SERE_SERV> SereServs_All { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER { get; set; }
        public V_HIS_SERVICE_REQ lstServiceReq { get; set; }
        public HIS_TREATMENT currentHisTreatment { get; set; }
        public Mps000037ADO Mps000037ADO { get; set; }
        public List<V_HIS_SERVICE_REQ> ListServiceReqPrint { get; set; }
        public List<V_HIS_SERVICE> ListService { get; set; }
        public List<HIS_SERE_SERV_EXT> SereServExt { get; set; }
        public List<HIS_SERVICE_REQ_TYPE> ListServiceReqType { get; set; }
    }

    //public class HisServiceReqCombo : MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ
    //{
    //    public string EXECUTE_ROOM_NAME { get; set; }
    //    public string SERVICE_NAME { get; set; }
    //}

    public class Mps000037ADO
    {
        public string bebRoomName { get; set; }
        public string departmentName { get; set; }
        public decimal ratio { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }
}
