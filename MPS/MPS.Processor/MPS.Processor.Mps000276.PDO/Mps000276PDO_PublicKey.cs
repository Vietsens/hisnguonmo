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

namespace MPS.Processor.Mps000276.PDO
{
    public partial class Mps000276PDO : RDOBase
    {
        public List<V_HIS_SERVICE_REQ> _vServiceReqs { get; set; }
        public HIS_TREATMENT _Treatment { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER _PatientTypeAlter { get; set; }
        public V_HIS_ROOM _vHisRoom { get; set; }
        public List<HIS_SERE_SERV> _SereServs { get; set; }
        public List<V_HIS_SERVICE> _Services { get; set; }
        public List<V_HIS_ROOM> _Rooms { get; set; }
        public List<V_HIS_CASHIER_ROOM> _CashierRooms { get; set; }
        public List<HIS_SERVICE_NUM_ORDER> _ServiceNumOrder { get; set; }
        public List<V_HIS_DESK> _Desks { get; set; }
        public List<HIS_CONFIG> lstConfig { get; set; }
        public HIS_TRANS_REQ transReq { get; set; }
    }

}
